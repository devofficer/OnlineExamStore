using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Web;
using OnlineExam.Models;
using OnlineExam.Areas.Visa.Models;

namespace OnlineExam.Utils
{

    public enum ActionOnQuestionError
    {
        Ignored,
        Corrected,
        Submitted
    }
    [Flags]
    public enum CompanyAction
    {
        Edit,
        View,
        Approve,
        Reject,
        Suspend,
        Active,
        Invoice
    }
    [Flags]
    public enum CompanyStatus
    {
        Pending = 1,
        Submitted = 2,
        Approved = 3,
        Active = 4,
        Suspended = 5,
        Rejected = 6
    }
    public enum InvoiceOrReceiptAction
    {
        Email = 1,
        View = 2
    }
    public enum InvoiceType
    {
        Receipt = 1,
        Invoice = 2
    }
    public enum InvoiceStatus
    {
        Draft = 1, Submitted = 2, Due = 3, Overdue = 4, Paid = 5, Canceled = 6
    }
    public enum PaymentMode
    {
        None = 0,
        Cheque = 1,
        Cash = 2
    }
    public enum ReportViewOrEmail
    {
        Email = 1,
        View = 2
    }

    public enum PaymentStatus
    {
        None = 0,
        Confirmed = 1,
        Pending = 2,
        Received = 3
    }
    public enum MembershipPlanType
    {
        Trial,
        Paid
    }

    public enum CBTType
    {
        Custom, // STUDENT CBT 
        Admin, // ADMIN CBT
        Confernce,
        Teacher 
    }
    public class AppConstants
    {
        public const string LoginFailureText = "Invalid username or password.";
        public const string RecordSavedText = "Record has been saved successfully.";
        public const string FailureText = "Some error has occured. Please contact to Administrator.";
        public const string RecordAlreadyExistsText = "Record is already exists.";
        public const string TrailMembershipPlanCode = "T0100";
        public const string DemoVoucherCode = "VC0100";

        public enum LookupType
        {
            ClassType,
            ExamType,
            Subject,
            QuestionFormatType,
            DenominationType,
            ClassCategory,
            SubjectCategory
        }

        public class NumberToEnglish
        {
            public String ChangeNumericToWords(double number)
            {
                String num = number.ToString();
                return ChangeToWords(num, false);
            }
            public String ChangeCurrencyToWords(String number)
            {
                return ChangeToWords(number, true);
            }
            public String ChangeNumericToWords(String number)
            {
                return ChangeToWords(number, false);
            }
            public String ChangeCurrencyToWords(double number)
            {
                return ChangeToWords(number.ToString(), true);
            }
            private String ChangeToWords(String numb, bool isCurrency)
            {
                String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
                String endStr = (isCurrency) ? ("Only") : ("");
                try
                {
                    int decimalPlace = numb.IndexOf(".");
                    if (decimalPlace > 0)
                    {
                        wholeNo = numb.Substring(0, decimalPlace);
                        points = numb.Substring(decimalPlace + 1);
                        if (Convert.ToInt32(points) > 0)
                        {
                            andStr = (isCurrency) ? ("and") : ("point");// just to separate whole numbers from points/cents
                            endStr = (isCurrency) ? ("Paise " + endStr) : ("");
                            pointStr = TranslateCents(points);
                        }
                    }
                    val = String.Format("{0} {1}{2} {3}", TranslateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
                }
                catch { ;}
                return val;
            }
            private String TranslateWholeNumber(String number)
            {
                string word = "";
                try
                {
                    bool beginsZero = false;//tests for 0XX
                    bool isDone = false;//test if already translated
                    double dblAmt = (Convert.ToDouble(number));
                    //if ((dblAmt > 0) && number.StartsWith("0"))
                    if (dblAmt > 0)
                    {//test for zero or digit zero in a nuemric
                        beginsZero = number.StartsWith("0");
                        int numDigits = number.Length;
                        int pos = 0;//store digit grouping
                        String place = "";//digit grouping name:hundres,thousand,etc...
                        switch (numDigits)
                        {
                            case 1://ones' range
                                word = Ones(number);
                                isDone = true;
                                break;
                            case 2://tens' range
                                word = Tens(number);
                                isDone = true;
                                break;
                            case 3://hundreds' range
                                pos = (numDigits % 3) + 1;
                                place = " Hundred ";
                                break;
                            case 4://thousands' range
                            case 5:
                            case 6:
                                pos = (numDigits % 4) + 1;
                                place = " Thousand ";
                                break;
                            case 7://millions' range
                            case 8:
                            case 9:
                                pos = (numDigits % 7) + 1;
                                place = " Million ";
                                break;
                            case 10://Billions's range
                                pos = (numDigits % 10) + 1;
                                place = " Billion ";
                                break;
                            //add extra case options for anything above Billion...
                            default:
                                isDone = true;
                                break;
                        }
                        if (!isDone)
                        {//if transalation is not done, continue...(Recursion comes in now!!)
                            word = TranslateWholeNumber(number.Substring(0, pos)) + place + TranslateWholeNumber(number.Substring(pos));
                            //check for trailing zeros
                            if (beginsZero) word = " and " + word.Trim();
                        }
                        //ignore digit grouping names
                        if (word.Trim().Equals(place.Trim())) word = "";
                    }
                }
                catch { ;}
                return word.Trim();
            }
            private String Tens(String digit)
            {
                int digt = Convert.ToInt32(digit);
                String name = null;
                switch (digt)
                {
                    case 10:
                        name = "Ten";
                        break;
                    case 11:
                        name = "Eleven";
                        break;
                    case 12:
                        name = "Twelve";
                        break;
                    case 13:
                        name = "Thirteen";
                        break;
                    case 14:
                        name = "Fourteen";
                        break;
                    case 15:
                        name = "Fifteen";
                        break;
                    case 16:
                        name = "Sixteen";
                        break;
                    case 17:
                        name = "Seventeen";
                        break;
                    case 18:
                        name = "Eighteen";
                        break;
                    case 19:
                        name = "Nineteen";
                        break;
                    case 20:
                        name = "Twenty";
                        break;
                    case 30:
                        name = "Thirty";
                        break;
                    case 40:
                        name = "Fourty";
                        break;
                    case 50:
                        name = "Fifty";
                        break;
                    case 60:
                        name = "Sixty";
                        break;
                    case 70:
                        name = "Seventy";
                        break;
                    case 80:
                        name = "Eighty";
                        break;
                    case 90:
                        name = "Ninety";
                        break;
                    default:
                        if (digt > 0)
                        {
                            name = Tens(digit.Substring(0, 1) + "0") + " " + Ones(digit.Substring(1));
                        }
                        break;
                }
                return name;
            }
            private String Ones(String digit)
            {
                int digt = Convert.ToInt32(digit);
                String name = "";
                switch (digt)
                {
                    case 1:
                        name = "One";
                        break;
                    case 2:
                        name = "Two";
                        break;
                    case 3:
                        name = "Three";
                        break;
                    case 4:
                        name = "Four";
                        break;
                    case 5:
                        name = "Five";
                        break;
                    case 6:
                        name = "Six";
                        break;
                    case 7:
                        name = "Seven";
                        break;
                    case 8:
                        name = "Eight";
                        break;
                    case 9:
                        name = "Nine";
                        break;
                }
                return name;
            }
            private String TranslateCents(String cents)
            {
                String cts = "", digit = "", engOne = "";
                for (int i = 0; i < cents.Length; i++)
                {
                    digit = cents[i].ToString();
                    if (digit.Equals("0"))
                    {
                        engOne = "Zero";
                    }
                    else
                    {
                        engOne = Ones(digit);
                    }
                    cts += " " + engOne;
                }
                return cts;
            }
        }
        /// <summary>
        /// Returns the an AlphaNumeric Code of provided Length. This Code may not be uniqe and should be check\
        /// by caller for any Uniqueness
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetAlfpaNumericCode(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static bool SendEligibilityMailWithAttachment(byte[] attachment, string body, string fileName, string ToEmail, String ToName)
        {
            bool isSent = false;
            var subject = "Work Visa Eligibility report";

            var smtp = new SmtpClient
            {
                Host = Convert.ToString(ConfigurationManager.AppSettings["MailingHost"]),
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"])
            };
            string fromAddress = Convert.ToString(ConfigurationManager.AppSettings["FromMailAddress"]);
            string toAddress = ToEmail;
            string password = Convert.ToString(ConfigurationManager.AppSettings["MailingPassword"]);
            //  string ccAddress = Convert.ToString(ConfigurationManager.AppSettings["ccMailAddress"]);

            if (string.IsNullOrWhiteSpace(fromAddress) || string.IsNullOrWhiteSpace(toAddress)
                || string.IsNullOrWhiteSpace(password)) return false;

            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(fromAddress, password);
            var mailMessage = new MailMessage { From = new MailAddress(fromAddress) };

            //Set 1 or More To Address
            if (toAddress.Contains('|'))
            {
                var x = toAddress.Split('|').ToList();
                x.ForEach(a => mailMessage.To.Add(a));
            }
            else
            {
                mailMessage.To.Add(toAddress);
            }
            //Set 1 or More CC Address
            //if (ccAddress.Contains('|'))
            //{
            //    var x = ccAddress.Split('|').ToList();
            //    x.ForEach(a => mailMessage.CC.Add(a));
            //}
            //else
            //{
            //    mailMessage.CC.Add(ccAddress);
            //}

            mailMessage.Subject = subject;
            mailMessage.Body = body;

            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), string.Format("" + subject + "-{0}.pdf", fileName.Replace(" ", ""))));
            mailMessage.IsBodyHtml = true;
            try
            {
                smtp.Send(mailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {
                isSent = false;
            }
            return isSent;
        }

        public static bool SendMailWithAttachment(byte[] attachment, string body, string fileName, InvoiceType invoiceType, Company company)
        {
            bool isSent = false;
            var subject = invoiceType == InvoiceType.Invoice ? "Sales Invoice" : "Receipt";

            var smtp = new SmtpClient
            {
                Host = Convert.ToString(ConfigurationManager.AppSettings["MailingHost"]),
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"])
            };
            string fromAddress = Convert.ToString(ConfigurationManager.AppSettings["FromMailAddress"]);
            string toAddress = company.PrimaryEmail;
            string password = Convert.ToString(ConfigurationManager.AppSettings["MailingPassword"]);
            //  string ccAddress = Convert.ToString(ConfigurationManager.AppSettings["ccMailAddress"]);

            if (string.IsNullOrWhiteSpace(fromAddress) || string.IsNullOrWhiteSpace(toAddress)
                || string.IsNullOrWhiteSpace(password)) return false;

            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(fromAddress, password);
            var mailMessage = new MailMessage { From = new MailAddress(fromAddress) };

            //Set 1 or More To Address
            if (toAddress.Contains('|'))
            {
                var x = toAddress.Split('|').ToList();
                x.ForEach(a => mailMessage.To.Add(a));
            }
            else
            {
                mailMessage.To.Add(toAddress);
            }
            //Set 1 or More CC Address
            //if (ccAddress.Contains('|'))
            //{
            //    var x = ccAddress.Split('|').ToList();
            //    x.ForEach(a => mailMessage.CC.Add(a));
            //}
            //else
            //{
            //    mailMessage.CC.Add(ccAddress);
            //}

            mailMessage.Subject = subject;
            mailMessage.Body = body;

            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), string.Format("" + subject + "-{0}.pdf", fileName)));
            mailMessage.IsBodyHtml = true;
            try
            {
                smtp.Send(mailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {
                isSent = false;
            }
            return isSent;
        }

        public class CompanyAction
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public static List<CompanyAction> CompanyActions = new List<CompanyAction>
        {
            new CompanyAction{Name = "Edit", Path = "#"},
            new CompanyAction{Name = "View", Path = "#"},
            new CompanyAction{Name = "Approved", Path = "#"},
            new CompanyAction{Name = "Rejected", Path = "#"},
            new CompanyAction{Name = "Suspend", Path = "#"},
            new CompanyAction{Name = "Resume", Path = "#"},
        };

        public static string TouristVisaDocumentFolderPath()
        {
            return string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DocumentRootFolderPath"])
                ? "~/App_Data/" : Convert.ToString(ConfigurationManager.AppSettings["DocumentRootFolderPath"]);
        }
        public class WorkVisaType
        {
            public string WorkVisaTypeName { get; set; }
            public string WorkVisaTypeValue { get; set; }
            public bool Selected { get; set; }
        }
        public static List<WorkVisaType> WorkVisaTypes = new List<WorkVisaType>
        {
            new WorkVisaType{WorkVisaTypeName = "Temp Work Visa", WorkVisaTypeValue = "1",Selected=false},
            new WorkVisaType{WorkVisaTypeName = "Permanent Work visa", WorkVisaTypeValue = "2",Selected=false},
            new WorkVisaType{WorkVisaTypeName = "Both Type", WorkVisaTypeValue = "3",Selected=true},
           
        };
        //public static List<string> GetCompanyActions()
        //{
        //    return Enum.GetNames(typeof(CompanyAction)).ToList();
        //}
        public static bool IsAgreementAccpeted(string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            bool isAggreementAccpeted =
                db.Users.Where(x => x.Id == userId).Select(x => x.IsAgreementAccpeted).FirstOrDefault();
            return isAggreementAccpeted;
        }

        public static bool IsAusvisaUser(string email)
        {
            return (!string.IsNullOrWhiteSpace(email) && (email.Contains("aus-visa.com") || email.Contains("ausvisa.com")));
        }

        public const string ReceiptIsAlreadyExistText = "Receipt is already created for this invoice, Please view/email receipt to see detail information.";
        public const string EmailIsExistMessage = "The provided email address is already exist in the system.Please try some different email address.";
        public const string DraftInvoiceText = "Draft Invoice";
        public const string GenerateInvoiceText = "Generate Invoice";
        public const string ErrorMessageText = "Some error has occurred. Please contact to Administrator.";
        public const string GenericErrorMessageText = "Some error has occurred. Please try again.";
        public const string SuccessMessageText = "Record has been saved successfully.";

        public const string ValidationWarningMessageText = "Please correct the below errors:";

        public const string RelationshipStatus = "RelationshipStatus";
        public const string Applied = "Applied";
        public const string Prospective = "Applied";
        public const string Staff = "Staff";
        public const string Client = "Client";
        public const string DomainName = "acadastore.com";
        public const string ClassDelimiter = "|";
        public const string LicenseAgreementMsg = "THIS IS A LEGAL AGREEMENT. IT GRANTS YOU CERTAIN RIGHTS AND IMPOSES CERTAIN OBLIGATIONS ON YOU" +
            "IN CONNECTION WITH YOUR USE OF THE RUBIC CAPITAL WEB SITE. YOU SHOULD READ THIS DOCUMENT CAREFULLY AND ONLY" +
            "ACCEPT ITS TERMS IF YOU UNDERSTAND AND AGREE WITH THEM. IF YOU ACCEPT AND AGREE WITH ALL OF THEM, YOU SHOULD CLICK ON THE ENTER SITE" +
            "BUTTON BELOW. IF YOU DO NOT AGREE TO ALL OF THESE TERMS AND CONDITIONS, DO NOT CLICK ON THE ENTER SITE" +
            "BUTTON BELOW, IN WHICH CASE YOU WILL NOT BE ABLE TO USE THE RUBIC CAPITAL WEBSITE. BY CLICKING ENTER SITE" +
            "RUBIC CAPITAL WILL CONSIDER THAT YOU ARE SOLICITING INFORMATION FROM US.";
        public const string MyRMA = "My RMA";

        public const string TouristVisaQ1Text = "Q. What is the full name of the Applicant?";
        public const string TouristVisaQ2Text = "Q. What is the Gender of the Applicant?";
        public const string TouristVisaQ3Text = "Q. Please provide the applicant's Birth detail.";
        public const string TouristVisaQ4Text = "Q. Please provide the applicant's Passport detail.";

        public const string TouristVisaQ5Text = "Q. Are you a citizen of any other country?";
        public const string TouristVisaQ6Text = "Q. What is the relationship of the Applicant?";

        public const string TouristVisaQ7Text = "Q. Are you or have you been known by any other name?(including name at birth, previous married names, aliases)";
        public const string TouristVisaQ8Text = "Q. Are you travelling to, or are you currently in, Australia with any family members?";
        public const string TouristVisaQ9Text = "Q. Do you have a partner, any children, or fiancé who will NOT be travelling, or has NOT travelled, to Australia with you?";
        public const string TouristVisaQ10Text = "Q. Do you have any relatives in Australia?";

        public const string TouristVisaQ11Text = "Q. Do you have any friends or contacts in Australia?";
        public const string TouristVisaQ12Text = "Q. What is the highest qualification of the Applicant?";
        public const string TouristVisaQ13Text = "Q. What is the employment status of the Applicant?";
        public const string TouristVisaQ14Text = "Q. Please provide the applicant's Address details?";
        public const string TouristVisaQ15Text = "Q. Please provide the applicant's Contact telephone numbers?";

        //STEP2, PERSONAL INFO 
        public const string TouristVisaQ16Text = "Q. What is the marital status of the person?";
        public const string TouristVisaQ17Text = "Q. Are you or have you been known by any other name? (Including name at birth, previous married names, aliases)";
        public const string TouristVisaQ18Text = "Q. Are you travelling to Australia with any family member(s)?";
        public const string TouristVisaQ19Text = "Q. Does Applicant have any partner, finance or Children who are NOT Travelling to Australia with?";
        public const string TouristVisaQ20Text = "Q. Does Applicant have any Relative in Australia?";
        public const string TouristVisaQ21Text = "Q. Does Applicant have any Friends or Contacts in Australia?";
        public const string TouristVisaQ22Text = "Q. What is the highest qualifications of the applicant?";

        // STEP3: EMPLOYEMENT STATUS
        public const string TouristVisaQ23Text = "Q. What is the employment status of the applicant?";
        public const string TouristVisaQ24Text = "Q. What is year of retirement of applicant?";
        public const string TouristVisaQ25Text = "Q. Give the name of course of the applicant is pursuing?";
        public const string TouristVisaQ26Text = "Q. Give the name of Institution, from where the applicant is pursuing course?";
        public const string TouristVisaQ27Text = "Q. How long have you been studying at this Institution?";
        public const string TouristVisaQ28Text = "Q. Please provide the Employment details?";
        public const string TouristVisaQ29Text = "Q. Please provide the Educatoinal details?";

        // STEP 4: COMMUNICATION DETAILS
        public const string TouristVisaCommQ30Text = "Q. Please provide applicant's communication details?";
        public const string TouristVisaCommQ31Text = "Q. Please provide applicant's contact details?";
        public const string TouristVisaCommQ32Text = "Q. What is mobile number of applicant?";
        public const string TouristVisaCommQ33Text = "Q. What is email address of applicant?";

        public const string TouristVisaCommQ34Text = "Q. Give details of how you will maintain yourself financially while you are in Australia";
        public const string TouristVisaCommQ35Text = "Q. Is your sponsor or someone else providing support for your visit to Australia?";
        public const string TouristVisaCommQ36Text = "Q. Have you ever been in Australia and not complied with visa conditions or departed Australia outside your authorized period of stay?";
        public const string TouristVisaCommQ37Text = "Q. Have you ever had an application for entry to or further stay in Australia refused, or had a visa for Australia cancelled?";

        // STEP 4: HEALTH DECLARATION
        public const string TouristVisaHealthQ38Text = "Q. Do applicant intend to enter a hospital or health care facility (including nursing homes) while in Australia?";
        public const string TouristVisaHealthQ39Text = "Q. Do you intend to work as, or study to be, a doctor, dentist, nurse or paramedic during your stay in Australia?";
        public const string TouristVisaHealthQ40Text = "Q. Have applicant";
        public const string TouristVisaHealthHaveApplicantOption1 = "Ever had, or currently have, tuberculosis?";
        public const string TouristVisaHealthHaveApplicantOption2 = "Been in close contact with a family member that has active tuberculosis?";
        public const string TouristVisaHealthHaveApplicantOption3 = "Ever had a chest x-ray which showed an abnormality?";

        public const string TouristVisaHealthQ41Text = "Q. During your proposed visit to Australia, do you expect to incur medical costs, or require treatment or medical follow up for?";
        public const string TouristVisaHealthQ42Text = "Q. Have you undertaken a health examination for an Australian visa in the last 12 months?";

        public const string TouristVisaHealthDetailText = "Give details (including HAP ID if available)";

        public const string TouristVisaHealthBloodDisorderText = "Have applicant had or currently have following disease:";
        public const string TouristVisaHealthBloodDisorderOption1 = "Blood disorder;";
        public const string TouristVisaHealthBloodDisorderOption2 = "Cancer";
        public const string TouristVisaHealthBloodDisorderOption3 = "Heart disease;";
        public const string TouristVisaHealthBloodDisorderOption4 = "Hepatitis B or C and/or liver disease;";

        public const string TouristVisaHealthBloodDisorderOption5 = "HIV Infection, including AIDS;";
        public const string TouristVisaHealthBloodDisorderOption6 = "Kidney disease, including dialysis;";
        public const string TouristVisaHealthBloodDisorderOption7 = "Mental illness;";

        public const string TouristVisaHealthBloodDisorderOption8 = "Pregnancy;";
        public const string TouristVisaHealthBloodDisorderOption9 = "Respiratory disease that has required hospital admission or oxygen therapy;";
        public const string TouristVisaHealthBloodDisorderOption10 = "Others";

        // STEP7: CHARACTER DECLARATIONS 
        public const string TouristVisaChar43Text = "Q. Have you, ever:";
        public const string TouristVisaCharOption1 = "been convicted of a crime or offence in any country (including any conviction which is now removed from official records)?";
        public const string TouristVisaCharOption2 = "been charged with any offence that is currently awaiting legal action?";
        public const string TouristVisaCharOption3 = "been acquitted of any criminal offence or other offence on the grounds of mental illness, insanity or unsoundness of mind?";
        public const string TouristVisaCharOption4 = "been removed or deported from any country (including Australia)?";
        public const string TouristVisaCharOption5 = "left any country to avoid being removed or deported?";
        public const string TouristVisaCharOption6 = "been excluded from or asked to leave any country (including Australia)?";
        public const string TouristVisaCharOption7 = "committed, or been involved in the commission of, war crimes or crimes against humanity or human rights?";
        public const string TouristVisaCharOption8 = "been involved in any activities that would represent a risk to Australian national security?";
        public const string TouristVisaCharOption9 = "had any outstanding debts to the Australian Government or any public authority in Australia?";
        public const string TouristVisaCharOption10 = "been involved in any activity, or been convicted of any offence, relating to the illegal movement of people to any country (including Australia)?";

        public const string TouristVisaCharOption11 = "served in a military force or state sponsored/private militia, undergone any military/paramilitary training, or been trained in weapons/explosives use (however described)?";
        public const string TouristVisaCharOption12 = "None Of the Above";

        // FINISH: ADDITIONAL DOCUMENTS 
        public const string TouristVisaDocText = "Additional Documents";
        public const string TouristVisaDocOption1 = "A valid passport with a certified copy of the identity page (showing photo and personal details) and other pages which provide evidence of travel to any other countries.";
        public const string TouristVisaDocOption2 = "A recent passport photograph (not more than 6 months old) of yourself. Please Upload a Digital Copy for Reference.";
        public const string TouristVisaDocOption3 = "A completed form 1257 Undertaking declaration, for applicants under 18 years of age, staying in Australia with someone other than a parent, legal guardian or relative (if applicable).";
        public const string TouristVisaDocOption4 = "Please Sign and Return this form for Representation to Immigration.";
        public const string TouristVisaDocOption5 = "Please Provide Evidence of Fund and Access to that Funds for your Stay and Travel to Australia.";
        public const string TouristVisaDocOption6 = "Please Provide Evidence that you have Financial and Imotional Reasons to back to Australia Australia.";
        public const string TouristVisaDocOption7 = "Please Provide A Letter of Absent from your Employer.";
        public const string TouristVisaDocOption8 = "Please Provide A Letter of Absent from School/Collage of Accompanied Children.";
        public const string TouristVisaDocOption9 = "Please Provide A Letter of Invitation from Your Relative or Friend in Australia.";
        public const string TouristVisaDocOption10 = "Please Provide A Letter of Sponsorship and Evidence of Access to Accommodation from Your Relative or Friend in Australia.";

        public const string TouristVisaDocOption11 = "Please Provide A Letter of Sponsorship and Evidence of Access to Funds from Your Relative or Friend in Australia.";
        public const string TouristVisaDocOption12 = "Please Provide Evidence of Medical/Travel Insurance.";
        public const string TouristVisaDocOption13 = "None Of the Above";

        public const string WorkVisaImm1Text = "Do you have skills in one or more Languages and would like to Claim Points for Community Language?";
        public const string WorkVisaImm2Text = "Have you completed a professional Year (12 Months) in Australia in Last 4 Year?";

        public const string WorkVisaEligibilityDeclarationText = "I HERE BY DECLARE, THAT ABOVE INFORMATION IS COMPLETE AND ACCURATE ACCOUNT OF MY PERSONAL AND PROFESSIONAL LIFE.";

        public class UserStatus
        {
            public const string Created = "Created";
            public const string Active = "Active";
        }

        public class Roles
        {
            public const string Visitor = "Visitor";
            public const string ClientOperator = "ClientOperator";
            public const string ClientManager = "ClientManager";
            public const string ClientAdmin = "ClientAdmin";
            public const string StaffAdmin = "StaffAdmin";
            public const string StaffManager = "StaffManager";
            public const string StaffOperator = "StaffOperator";
            public const string ProspectiveClient = "ProspectiveClient";

        }

        public class RegisterType
        {
            public const string BussinessAssociate = "BA";
            public const string BussinessClient = "BC";
            public const string Demo = "Demo";
            public const string Paid = "Paid";

        }
        public class EducationTitles
        {
            public const string Doctorate = "Doctorate";
            public const string Masters = "Masters";
            public const string Diploma = "Diploma";
            public const string Bachelors = "Bachelors";

        }
        public class WorkVisaEligibilityFactors
        {
            public const string Age = "Age";
            public const string EngCom = "English language competency level";
            public const string OverseasEmp = "Overseas skilled employment";
            public const string AusEmp = "Australian skilled employment";
            public const string EduQualifications = "Educational qualifications";
            public const string EduAus = "Australian study qualifications";
            public const string Otherfactors = "Other factors(Credentialled community language qualifications/Study in regional Australia/Partner skill qualifications/Professional Year completion, for a period of at least 12 months)";
            public const string NominationSponsorship = "Nomination/Sponsorship";

        }
        public class WorkVisaEligibilityFactorsMaxScores
        {
            public const int Age = 30;
            public const int EngCom = 20;
            public const int OverseasEmp = 15;
            public const int AusEmp = 20;
            public const int EduQualifications = 20;
            public const int EduAus = 5;
            public const int Otherfactors = 5;
            public const int NominationSponsorship = 20;

        }
    }
}