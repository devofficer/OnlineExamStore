using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Helpers
{
    public class ControlGroup : IDisposable
    {
        private readonly HtmlHelper _html;

        public ControlGroup(HtmlHelper html)
        {
            _html = html;
        }

        public void Dispose()
        {
            _html.ViewContext.Writer.Write(_html.EndControlGroup());
        }
    }

    public static class ControlGroupExtensions
    {
        public static IHtmlString BeginControlGroupFor<T>(this HtmlHelper<T> html, Expression<Func<T, object>> modelProperty)
        {
            return BeginControlGroupFor(html, modelProperty, null);
        }

        public static IHtmlString BeginControlGroupFor<T>(this HtmlHelper<T> html, Expression<Func<T, object>> modelProperty, object htmlAttributes)
        {
            return BeginControlGroupFor(html, modelProperty,
                                        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlString BeginControlGroupFor<T>(this HtmlHelper<T> html, Expression<Func<T, object>> modelProperty, IDictionary<string, object> htmlAttributes)
        {
            string propertyName = ExpressionHelper.GetExpressionText(modelProperty);
            return BeginControlGroupFor(html, propertyName, null);
        }

        public static IHtmlString BeginControlGroupFor<T>(this HtmlHelper<T> html, string propertyName)
        {
            return BeginControlGroupFor(html, propertyName, null);
        }

        public static IHtmlString BeginControlGroupFor<T>(this HtmlHelper<T> html, string propertyName, object htmlAttributes)
        {
            return BeginControlGroupFor(html, propertyName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlString BeginControlGroupFor<T>(this HtmlHelper<T> html, string propertyName, IDictionary<string, object> htmlAttributes)
        {
            var controlGroupWrapper = new TagBuilder("div");
            controlGroupWrapper.MergeAttributes(htmlAttributes);
            controlGroupWrapper.AddCssClass("control-group");
            string partialFieldName = propertyName;
            string fullHtmlFieldName =
                html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(partialFieldName);
            if (!html.ViewData.ModelState.IsValidField(fullHtmlFieldName))
            {
                controlGroupWrapper.AddCssClass("error");
            }
            string openingTag = controlGroupWrapper.ToString(TagRenderMode.StartTag);
            return MvcHtmlString.Create(openingTag);
        }

        public static IHtmlString EndControlGroup(this HtmlHelper html)
        {
            return MvcHtmlString.Create("</div>");
        }

        public static ControlGroup ControlGroupFor<T>(this HtmlHelper<T> html, Expression<Func<T, object>> modelProperty)
        {
            return ControlGroupFor(html, modelProperty, null);
        }

        public static ControlGroup ControlGroupFor<T>(this HtmlHelper<T> html, Expression<Func<T, object>> modelProperty, object htmlAttributes)
        {
            string propertyName = ExpressionHelper.GetExpressionText(modelProperty);
            return ControlGroupFor(html, propertyName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static ControlGroup ControlGroupFor<T>(this HtmlHelper<T> html, string propertyName)
        {
            return ControlGroupFor(html, propertyName, null);
        }

        public static ControlGroup ControlGroupFor<T>(this HtmlHelper<T> html, string propertyName, object htmlAttributes)
        {
            return ControlGroupFor(html, propertyName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static ControlGroup ControlGroupFor<T>(this HtmlHelper<T> html, string propertyName, IDictionary<string, object> htmlAttributes)
        {
            html.ViewContext.Writer.Write(BeginControlGroupFor(html, propertyName, htmlAttributes));
            return new ControlGroup(html);
        }
    }

    public static class Alerts
    {
        public const string SUCCESS = "success";
        public const string ATTENTION = "attention";
        public const string ERROR = "error";
        public const string INFORMATION = "info";

        public static string[] ALL
        {
            get { return new[] { SUCCESS, ATTENTION, INFORMATION, ERROR }; }
        }
    }

    public static class QueryExtensions
    {
        /// <summary>
        /// A generic routine to sort a two dimensional array of a specified type based on the specified column.
        /// </summary>
        /// <param name="array">The array to sort.</param>
        /// <param name="sortCol">The index of the column to sort.</param>
        /// <param name="order">Specify "DESC" or "DESCENDING" for a descending sort otherwise
        /// leave blank or specify "ASC" or "ASCENDING".</param>
        /// <remarks>The original array is sorted in place.</remarks>
        /// <see cref="http://stackoverflow.com/questions/232395/how-do-i-sort-a-two-dimensional-array-in-c"/>
        public static void Sort<T>(this T[,] array, int sortCol, string order)
        {
            int colCount = array.GetLength(1), rowCount = array.GetLength(0);
            if (sortCol >= colCount || sortCol < 0)
                throw new System.ArgumentOutOfRangeException("sortCol", "The column to sort on must be contained within the array bounds.");

            DataTable dt = new DataTable();
            // Name the columns with the second dimension index values, e.g., "0", "1", etc.
            for (int col = 0; col < colCount; col++)
            {
                DataColumn dc = new DataColumn(col.ToString(), typeof(T));
                dt.Columns.Add(dc);
            }
            // Load data into the data table:
            for (int rowindex = 0; rowindex < rowCount; rowindex++)
            {
                DataRow rowData = dt.NewRow();
                for (int col = 0; col < colCount; col++)
                    rowData[col] = array[rowindex, col];
                dt.Rows.Add(rowData);
            }
            // Sort by using the column index = name + an optional order:
            DataRow[] rows = dt.Select("", sortCol.ToString() + " " + order);

            for (int row = 0; row <= rows.GetUpperBound(0); row++)
            {
                DataRow dr = rows[row];
                for (int col = 0; col < colCount; col++)
                {
                    array[row, col] = (T)dr[col];
                }
            }

            dt.Dispose();
        }

        public static void SortList<T, TValue>(this List<T> list, Func<T, TValue> valueSelector, bool ascending) where TValue : IComparable
        {
           
            if (ascending)
            {
                list.Sort((i1, i2) =>
                {
                    var v1 = valueSelector(i1);
                    var v2 = valueSelector(i2);
                    if (v1 == null)
                        return 0;
                    return v1.CompareTo(v2);
                });
            }
            else
            {
                list.Sort((i1, i2) =>
                {
                    var v1 = valueSelector(i2);
                    var v2 = valueSelector(i1);
                    if (v1 == null)
                        return 0;
                    return v1.CompareTo(v2);
                });
            }
        }
    }
}
