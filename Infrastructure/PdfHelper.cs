using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace OnlineExam.Infrastructure
{

    public abstract class PdfFieldType
    {
        public static PdfFieldType GetPdfFieldType(int type)
        {
            switch (type)
            {
                case 4:
                    return new PdfTextFieldType();
                case 2:
                    return new PdfCheckBoxFieldType();
                default:
                    return new PdfOtherFieldType();
            }
        }

        public abstract int Type { get; }
    }
    public class PdfOtherFieldType : PdfFieldType
    {
        public override int Type
        {
            get { return -1; }
        }

        public override string ToString()
        {
            return "Other";
        }
    }
    public class PdfTextFieldType : PdfFieldType
    {
        public override int Type
        {
            get { return 4; }
        }

        public override string ToString()
        {
            return "TextField";
        }
    }
    public class PdfCheckBoxFieldType : PdfFieldType
    {
        public override int Type
        {
            get { return 2; }
        }

        public override string ToString()
        {
            return "CheckBox";
        }
    }
    public class PdfHelper
    {
        public static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            string replace = Regex.Replace(name, invalidReStr, "_").Replace(";", "").Replace(",", "").Replace(" ", "_");
            return replace;
        }

        public static Dictionary<string, string> GetFormFieldNames(string pdfPath)
        {
            var fields = new Dictionary<string, string>();

            var reader = new PdfReader(pdfPath);
            foreach (var entry in reader.AcroFields.Fields)
                fields.Add(entry.Key.ToString(), string.Empty);
            reader.Close();

            return fields;
        }

        public static byte[] GeneratePdf(string pdfPath, Dictionary<string, string> formFieldMap)
        {
            var output = new MemoryStream();
            var reader = new PdfReader(pdfPath);
            var stamper = new PdfStamper(reader, output);
            var formFields = stamper.AcroFields;

            foreach (var fieldName in formFieldMap.Keys)
                formFields.SetField(fieldName, formFieldMap[fieldName]);

            stamper.FormFlattening = true;
            stamper.Close();
            reader.Close();

            return output.ToArray();
        }

        // See http://stackoverflow.com/questions/4491156/get-the-export-value-of-a-checkbox-using-itextsharp/
        public static string GetExportValue(AcroFields.Item item)
        {
            var valueDict = item.GetValue(0);
            var appearanceDict = valueDict.GetAsDict(PdfName.AP);

            if (appearanceDict != null)
            {
                var normalAppearances = appearanceDict.GetAsDict(PdfName.N);
                // /D is for the "down" appearances.

                // if there are normal appearances, one key will be "Off", and the other
                // will be the export value... there should only be two.
                if (normalAppearances != null)
                {
                    foreach (var curKey in normalAppearances.Keys)
                        if (!PdfName.OFF.Equals(curKey))
                            return curKey.ToString().Substring(1);
                    // string will have a leading '/' character, so remove it!
                }
            }

            // if that doesn't work, there might be an /AS key, whose value is a name with 
            // the export value, again with a leading '/', so remove it!
            var curVal = valueDict.GetAsName(PdfName.AS);
            if (curVal != null)
                return curVal.ToString().Substring(1);
            else
                return string.Empty;
        }

        public static void ReturnPdf(byte[] contents)
        {
            ReturnPdf(contents, null);
        }

        public static void ReturnPdf(byte[] contents, string attachmentFilename)
        {
            var response = HttpContext.Current.Response;

            if (!string.IsNullOrEmpty(attachmentFilename))
                response.AddHeader("Content-Disposition", "attachment; filename=" + attachmentFilename);

            response.ContentType = "application/pdf";
            response.BinaryWrite(contents);
            response.End();
        }

        public static void ExtractPages(string inputFile, string outputFile, int start, int end)
        {
            // get input document
            var inputPdf = new iTextSharp.text.pdf.PdfReader(inputFile);

            // retrieve the total number of pages
            int pageCount = inputPdf.NumberOfPages;


            if (end < start || end > pageCount)
            {
                end = pageCount;
            }

            // load the input document
            var inputDoc = new Document(inputPdf.GetPageSizeWithRotation(1));

            // create the filestream
            using (var fs = new FileStream(outputFile, FileMode.Create))
            {
                // create the output writer
                PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                inputDoc.Open();
                PdfContentByte cb1 = outputWriter.DirectContent;

                // copy pages from input to output document
                for (int i = start; i <= end; i++)
                {
                    inputDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(i));
                    inputDoc.NewPage();

                    PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, i);
                    int rotation = inputPdf.GetPageRotation(i);


                    if (rotation == 90 || rotation == 270)
                    {
                        cb1.AddTemplate(page, 0, -1f, 1f, 0, 0, inputPdf.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                }
                inputDoc.Close();
            }
        }
    }
}