using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.Linq.Expressions;
using System.Data.Linq.SqlClient;
using System.Data.Entity;
using System.Data.Common;
using System.Threading.Tasks;

namespace OnlineExam.Helpers
{
    public static class DataReaderExtensions
    {
        public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
        {
            if (dr != null && dr.HasRows)
            {
                var entity = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

                while (dr.Read())
                {
                    T newObject = new T();
                    for (int index = 0; index < dr.FieldCount; index++)
                    {
                        if (propDict.ContainsKey(dr.GetName(index).ToUpper()))
                        {
                            var info = propDict[dr.GetName(index).ToUpper()];
                            if ((info != null) && info.CanWrite)
                            {
                                var val = dr.GetValue(index);
                                info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                            }
                        }
                    }
                    entities.Add(newObject);
                }
                return entities;
            }
            return null;
        }
    }

    public static class IDbSetExtension
    {
        public static Task<TEntity> FindAsync<TEntity>(this IDbSet<TEntity> set, params object[] keyValues)
            where TEntity : class
        {
            return Task<TEntity>.Run(() =>
            {
                var entity = set.Find(keyValues);
                return entity;
            });
        }
    }
    public static class Extension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        public static int? NullableTryParseInt32(string text)
        {
            int value;
            return int.TryParse(text, out value) ? (int?)value : null;
        }

        public static MvcHtmlString EncodedReplace(this HtmlHelper helper, string input, string pattern, string replacement)
        {
            return new MvcHtmlString(Regex.Replace(input, pattern, replacement));
        }
        public static string ToFirstCapital(this string value, int a)
        {
            return value.Length > 0
                ? value.Length > 1 ? char.ToUpper(value[0]) + value.Substring(1) : char.ToUpper(value[0]).ToString()
                : string.Empty;
        }

        /// <summary>
        /// Converts string value into Integer. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Integer Equilent of string value or Zero if invalid integer string</returns>
        public static int ToInt(this string value)
        {
            int result = 0;
            if (value != null)
                int.TryParse(value, out result);
            return result;

        }

        /// <summary>
        /// Converts Integer Equailent.
        /// </summary>
        /// <param name="value">string value that need to be converted</param>
        /// <param name="default">Integer Default value that will be returned if Invalid string provided into parameter.</param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Integer Equilent of string value or Default if invalid integer string</returns>
        public static int? ToIntOrDefault(this string value, int? @default)
        {
            int result = 0;
            if (int.TryParse(value, out result))
                return result;
            else
                return @default;

        }

        /// <summary>
        /// Converts string value into Long. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Long Equilent of string value or Zero if invalid Long string</returns>
        public static long ToLong(this string value)
        {
            long result = 0;
            long.TryParse(value, out result);
            return result;

        }

        /// <summary>
        /// Converts string value into Float. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Float Equilent of string value or Zero if invalid float string</returns>
        public static float ToFloat(this string value)
        {
            float result = 0;
            float.TryParse(value, out result);
            return result;

        }

        /// <summary>
        /// Converts string value into Double. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Double Equilent of string value or Zero if invalid Double string</returns>
        public static double ToDouble(this string value)
        {
            double result = 0;
            double.TryParse(value, out result);
            return result;

        }

        /// <summary>
        /// Converts string value into Decimal. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Double Equilent of string value or Zero if invalid Double string</returns>
        public static decimal ToDecimal(this string value)
        {
            decimal result = 0;
            decimal.TryParse(value, out result);
            return result;

        }

        /// <summary>
        /// Converts string value into Boolean. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Boolean Equilent of string value or Zero if invalid Boolean string</returns>
        public static bool ToBool(this string value)
        {
            bool result = false;
            if (string.IsNullOrEmpty(value))
            {
                return result;
            }
            if (value.Trim() == "1")
                result = true;
            else if (value.Trim() == "0")
                result = false;
            else
                bool.TryParse(value, out result);

            return result;
        }

        /// <summary>
        /// Converts string value into Boolean. 
        /// </summary> 
        /// <param name="value"></param>
        /// <remarks>Extended By : Anirudh Krishan Vaishnav</remarks>
        /// <returns>Boolean Equilent of string value or Default if invalid Boolean string</returns>
        public static bool ToBool(this string value, bool @default)
        {
            bool result = false;
            if (value.Trim() == "1")
                result = true;
            else if (value.Trim() == "0")
                result = false;
            else
            {
                if (bool.TryParse(value, out result))
                    return result;
                else
                    return @default;

            }

            return result;
        }

        /// <summary>
        /// Upper case First later of any string
        /// </summary>
        /// <param name="str"></param>
        /// <remarks>Extended By : Manish sharma</remarks>
        /// <returns>Upper case First later of any string</returns>
        public static string UppercaseFirst(this string value)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            if (value.Length == 1)
            {
                return char.ToUpper(value[0]).ToString();
            }
            else
            {
                return char.ToUpper(value[0]) + value.Substring(1);
            }
        }

        public static bool IsBetweenLamdaDateTime(this DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;

        }

        public static bool IsBetweenLamdaDate(this DateTime dt, DateTime start, DateTime end)
        {
            return dt.Date >= start.Date && dt.Date <= end.Date;

        }

        public static bool IsBetweenLamdaMonth(this DateTime dt, DateTime start, DateTime end)
        {
            return dt.Month >= start.Month && dt.Month <= end.Month;
        }

        public static bool IsBetweenLamdaTimeZoneDateTime(this DateTime dt, DateTime start, DateTime end,
            string timeZone)
        {
            TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), mountain) >=
                   TimeZoneInfo.ConvertTimeFromUtc(start.ToUniversalTime(), mountain) &&
                   TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), mountain) <=
                   TimeZoneInfo.ConvertTimeFromUtc(end.ToUniversalTime(), mountain);
            // return dt >= start && dt <= end;

        }

        public static DateTime TimeZoneDateTime(this DateTime dt, string timeZone, bool IsUserValid = true)
        {
            try
            {
                return TimeZoneDateTime(dt);
                //TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                //return TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), mountain);
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        public static DateTime TimeZoneDateTime(this DateTime dt)
        {
            try
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                foreach (TimeZoneInfo timeZone in timeZones)
                {
                    string offsetString = timeZone.BaseUtcOffset.ToString().Replace(":", "");
                    if (Convert.ToInt32(offsetString) >= 0)
                    {
                        offsetString = "+" + offsetString;
                    }
                    if (context.Request.Cookies["dmfuserTimezone"] != null)
                    {
                        if (context.Request.Cookies["dmfuserTimezone"].Value.ToString() + "00" == offsetString)
                        {
                            if (dt != null)
                            {
                                DateTime thisTime = dt; // Convert.ToDateTime(str);
                                TimeZoneInfo tst1 = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Id);
                                DateTime tstTime1 = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, tst1);
                                dt = Convert.ToDateTime(tstTime1);
                            }
                            return dt;
                        }
                    }
                }
                return dt;

            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        public static string EclipseString(this string str, int maxlength)
        {
            string tmpstr = string.Empty;
            try
            {
                if (str.Length > maxlength)
                    tmpstr = str.Substring(0, maxlength) + "...";
                else
                    tmpstr = str;
            }
            catch (Exception ex)
            {
                tmpstr = str;
            }
            return tmpstr;
        }

        public static bool IsMobile(this string userAgent)
        {
            userAgent = userAgent.ToLower();

            return userAgent.Contains("iphone") |
                   userAgent.Contains("ppc") |
                   userAgent.Contains("windows ce") |
                   userAgent.Contains("blackberry") |
                   userAgent.Contains("opera mini") |
                   userAgent.Contains("mobile") |
                   userAgent.Contains("palm") |
                   userAgent.Contains("portable") |
                   userAgent.Contains("ipad") |
                   userAgent.Contains("android") |
                   userAgent.Contains("meego");
        }

        public static string ToTrim(this object value)
        {
            try
            {
                var str = value.ToString();
                str = str.Trim();
                return str;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetQueryString(this string value, string queryStr)
        {
            try
            {
                string strv = string.Empty;
                string str = value;
                var strsLinks = str.Split('&');
                for (int i = 0; i < strsLinks.Length; i++)
                {
                    var querystring = strsLinks[i].Split('=');
                    if (querystring.Length > 0)
                    {
                        if (querystring[0] == queryStr)
                        {
                            return querystring[1];
                        }
                    }
                }
                return strv;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        public static string ToNiceNumber(this float num)
        {
            // first strip any formatting;
            //$n = (0+str_replace(",","",$n));

            // is this a number?
            // if(!is_numeric($n)) return false;

            // now filter it;
            if (num > 1000000000000) return Math.Round((decimal)(num / 1000000000000), 2).ToString() + " T";
            else if (num > 1000000000) return Math.Round((decimal)(num / 1000000000), 2).ToString() + " B";
            else if (num > 1000000) return Math.Round((decimal)(num / 1000000), 2).ToString() + " M";
            else if (num > 1000) return Math.Round((decimal)(num / 1000), 2).ToString() + " K";

            return string.Format("{0}", num); //number_format(num);
        }

        /// <summary>
        /// Converts an <see cref="int"/> to its textual representation
        /// </summary>
        /// <param name="num">
        /// The number to convert to text
        /// </param>
        /// <returns>
        /// A textual representation of the given number
        /// </returns>
        public static string ToText(this int num)
        {
            StringBuilder result = new StringBuilder();
            if (num <= 99999)
            {
                result.Append(num / 1000 + "K");
            }
            return result.ToString();
        }

        public static string ToText1(this int num)
        {
            StringBuilder result;

            if (num < 0)
            {
                return string.Format("Minus {0}", ToText(-num));
            }

            if (num == 0)
            {
                return "Zero";
            }

            if (num <= 19)
            {
                var oneToNineteen = new[]
                {
                    "One",
                    "Two",
                    "Three",
                    "Four",
                    "Five",
                    "Six",
                    "Seven",
                    "Eight",
                    "Nine",
                    "Ten",
                    "Eleven",
                    "Twelve",
                    "Thirteen",
                    "Fourteen",
                    "Fifteen",
                    "Sixteen",
                    "Seventeen",
                    "Eighteen",
                    "Nineteen"
                };

                return oneToNineteen[num - 1];
            }

            if (num <= 99)
            {
                result = new StringBuilder();

                var multiplesOfTen = new[]
                {
                    "Twenty",
                    "Thirty",
                    "Forty",
                    "Fifty",
                    "Sixty",
                    "Seventy",
                    "Eighty",
                    "Ninety"
                };

                result.Append(multiplesOfTen[(num / 10) - 2]);

                if (num % 10 != 0)
                {
                    result.Append(" ");
                    result.Append(ToText(num % 10));
                }

                return result.ToString();
            }

            if (num == 100)
            {
                return "One Hundred";
            }

            if (num <= 199)
            {
                return string.Format("One Hundred and {0}", ToText(num % 100));
            }

            if (num <= 999)
            {
                result = new StringBuilder((num / 100).ToText());
                result.Append(" Hundred");
                if (num % 100 != 0)
                {
                    result.Append(" and ");
                    result.Append((num % 100).ToText());
                }

                return result.ToString();
            }

            if (num <= 999999)
            {
                result = new StringBuilder((num / 1000).ToText());
                result.Append(" Thousand");
                if (num % 1000 != 0)
                {
                    switch ((num % 1000) < 100)
                    {
                        case true:
                            result.Append(" and ");
                            break;
                        case false:
                            result.Append(", ");
                            break;
                    }

                    result.Append((num % 1000).ToText());
                }

                return result.ToString();
            }

            if (num <= 999999999)
            {
                result = new StringBuilder((num / 1000000).ToText());
                result.Append(" Million");
                if (num % 1000000 != 0)
                {
                    switch ((num % 1000000) < 100)
                    {
                        case true:
                            result.Append(" and ");
                            break;
                        case false:
                            result.Append(", ");
                            break;
                    }

                    result.Append((num % 1000000).ToText());
                }

                return result.ToString();
            }

            result = new StringBuilder((num / 1000000000).ToText());
            result.Append(" Billion");
            if (num % 1000000000 != 0)
            {
                switch ((num % 1000000000) < 100)
                {
                    case true:
                        result.Append(" and ");
                        break;
                    case false:
                        result.Append(", ");
                        break;
                }

                result.Append((num % 1000000000).ToText());
            }
            return result.ToString();
        }

        public static string ToRequestUserHostAddress(this string hostAddressIP)
        {
            string myLocalDevelopmentIP = "115.113.63.163";
            try
            {
                if (!string.IsNullOrEmpty(hostAddressIP) && hostAddressIP.Equals("::1"))
                {
                    return myLocalDevelopmentIP;
                }
                else
                {
                    return hostAddressIP;
                }
            }
            catch (Exception ex)
            {
                return myLocalDevelopmentIP;
            }
        }

        #region IsValidGuid

        private static Regex isGuid =
            new Regex(
                @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
                RegexOptions.Compiled);

        public static bool IsValidGuid(this string candidate, out Guid output)
        {
            bool isValid = false;
            output = Guid.Empty;

            if (candidate != null)
            {

                if (isGuid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }

            return isValid;
        }

        #endregion

        #region Serialize/Deserialize Methods

        /// <summary>
        /// Returns an XML string based on the ShoppingCart object.
        /// </summary>
        /// <returns>string</returns>
        public static string Serialize(this object type)
        {
            try
            {
                var sb = new StringBuilder();
                var sw = new StringWriter(sb);
                var ser = new XmlSerializer(type.GetType());
                ser.Serialize(sw, type);
                sw.Close();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Deserializes the provided string into the object cast as the provided type.
        /// </summary>
        /// <typeparam name="T">The type of item to base the XML schema on.</typeparam>
        /// <param name="s">The serialized XML based on the XML schema of the provided type.</param>
        /// <returns>Type</returns>
        public static T Deserialize<T>(string s) where T : class
        {
            try
            {
                var ser = new XmlSerializer(typeof(T));
                var sr = new StringReader(s);
                return ser.Deserialize(sr) as T;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Truncate a String
        /// </summary>
        /// <param name="str">String to truncate</param>
        /// <param name="MaxLength">MaxLength</param>
        /// <returns></returns>
        public static string ToTruncateString(this string str, int MaxLength, bool IsShowEllipses = false)
        {
            string tmpStr = str;
            try
            {
                if (!string.IsNullOrEmpty(str) && str.Length > MaxLength)
                {
                    tmpStr = str.Substring(0, MaxLength);

                    if (IsShowEllipses)
                        tmpStr = tmpStr.Substring(0, (MaxLength - 3)) + "...";
                }
                else
                {
                    tmpStr = str;
                }
                return tmpStr;
            }
            catch (Exception ex)
            {
                return str;
            }

        }

        public static string ToTruncateString(this string str, int MaxLength)
        {
            string tmpStr = str;
            try
            {
                if (!string.IsNullOrEmpty(str) && str.Length > MaxLength)
                {
                    tmpStr = str.Substring(0, MaxLength);
                }
                else
                {
                    tmpStr = str;
                }
                return tmpStr;
            }
            catch (Exception ex)
            {
                return str;
            }

        }

        public static string ToTruncateHtmlString(this string str, int MaxLength, bool IsShowEllipses)
        {
            string tmpStr = str;
            try
            {
                tmpStr = System.Text.RegularExpressions.Regex.Replace(tmpStr, @"<(.|\n)*?>", string.Empty);
                if (!string.IsNullOrEmpty(tmpStr) && tmpStr.Length > MaxLength)
                {
                    if (IsShowEllipses)
                        tmpStr = tmpStr.Substring(0, (MaxLength - 3)) + "...";
                    else
                        tmpStr = tmpStr.Substring(0, MaxLength);
                }
                else
                {

                }
                return tmpStr;
            }
            catch (Exception ex)
            {
                return str;
            }

        }

        public static DateTime ToDateTime(this string value)
        {
            return Convert.ToDateTime(value);
        }

        public static int ToTotalMonths(this DateTime startDate, DateTime endDate)
        {
            return System.Data.Linq.SqlClient.SqlMethods.DateDiffMonth(startDate, endDate);
        }

        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static IEnumerable<DateTime> EachMonth(this DateTime from, DateTime thru)
        {
            for (var month = from.Date;
                month.Date <= thru.Date || month.Month == thru.Month;
                month = month.AddMonths(1))
                yield return month;
        }

        public static IEnumerable<DateTime> EachDayTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> EachMonthTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo);
        }

        //#region Encryption/Decryption methods
        //public static string DecryptString(this string coded, string key = "", string ivKey = "")
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(key)) { key = Settings.EncryptionKeys.EncryptionKey; }
        //        if (string.IsNullOrEmpty(ivKey)) { ivKey = Settings.EncryptionKeys.IVKey; }
        //        RijndaelManaged cryptProvider = new RijndaelManaged();
        //        cryptProvider.KeySize = 256;
        //        cryptProvider.BlockSize = 256;
        //        cryptProvider.Mode = CipherMode.CBC;
        //        SHA256Managed hashSHA256 = new SHA256Managed();
        //        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        //        string iv = ivKey;
        //        cryptProvider.IV = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(iv));
        //        byte[] cipherTextByteArray = EncodeBase64(coded);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateDecryptor(), CryptoStreamMode.Write);
        //        cs.Write(cipherTextByteArray, 0, cipherTextByteArray.Length);
        //        cs.FlushFinalBlock();
        //        cs.Close();
        //        byte[] byt = ms.ToArray();
        //        return Encoding.ASCII.GetString(byt);
        //    }
        //    catch (Exception ex) { return string.Empty; }
        //}

        //// NOTE: We don't use the Encrypt method within the backoffice, but we've supplied the encryption method here as an 
        ////       example to your vendors and others of how to write a simple Rijndael enryption method in C#.
        ////       
        //public static string EncryptString(this string uncoded, string key = "", string ivKey = "")
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(key)) { key = Settings.EncryptionKeys.EncryptionKey; }
        //        if (string.IsNullOrEmpty(ivKey)) { ivKey = Settings.EncryptionKeys.IVKey; }
        //        RijndaelManaged cryptProvider = new RijndaelManaged();
        //        cryptProvider.KeySize = 256;
        //        cryptProvider.BlockSize = 256;
        //        cryptProvider.Mode = CipherMode.CBC;
        //        SHA256Managed hashSHA256 = new SHA256Managed();
        //        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        //        string iv = ivKey;
        //        cryptProvider.IV = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(iv));
        //        byte[] plainTextByteArray = ASCIIEncoding.ASCII.GetBytes(uncoded);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateEncryptor(), CryptoStreamMode.Write);
        //        cs.Write(plainTextByteArray, 0, plainTextByteArray.Length);
        //        cs.FlushFinalBlock();
        //        cs.Close();
        //        byte[] byt = ms.ToArray();
        //        return Convert.ToBase64String(byt);
        //    }
        //    catch (Exception ex) { return string.Empty; }
        //}
        //private static byte[] EncodeBase64(string data)
        //{
        //    string s = data.Trim().Replace(" ", "+");
        //    if (s.Length % 4 > 0)
        //        s = s.PadRight(s.Length + 4 - s.Length % 4, '=');
        //    return Convert.FromBase64String(s);
        //}
        //#endregion

    }

    public class Varience
    {
        private string _prop;

        public string Property
        {
            get { return _prop; }
            set { _prop = value; }
        }

        private object _valA;

        public object ValA
        {
            get { return _valA; }
            set { _valA = value; }
        }

        private object _valB;

        public object ValB
        {
            get { return _valB; }
            set { _valB = value; }
        }

    }

    public static class ExtensionMethods
    {
        public static IQueryable<T> OrderByPrperty<T>(
            this IQueryable<T> source, string propertyName, bool asc)
        {
            var type = typeof(T);
            string methodName = asc ? "OrderBy" : "OrderByDescending";
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
                new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> LikeByProperty<T>(this IQueryable<T> source,
            string keyword, params string[] propertyNames)
        {

            foreach (var propertyName in propertyNames)
            {
                var type = typeof(T);
                var property = type.GetProperty(propertyName);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var constant = Expression.Constant("%" + keyword + "%");
                var like = typeof(SqlMethods).GetMethod("Like",
                    new Type[] { typeof(string), typeof(string) });
                MethodCallExpression methodExp =
                    Expression.Call(null, like, propertyAccess, constant);
                Expression<Func<T, bool>> lambda =
                    Expression.Lambda<Func<T, bool>>(methodExp, parameter);
                source = source.Where(lambda);
            }
            return source;
        }

        public static TTarget Transfer<TSource, TTarget>(TSource source)
            where TTarget : class, new()
        {
            var target = new TTarget();
            Transfer(source, target);
            return target;
        }

        private static void Transfer(object source, object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var targetParameter = Expression.Parameter(typeof(object), "target");

            var sourceVariable = Expression.Variable(sourceType, "castedSource");
            var targetVariable = Expression.Variable(targetType, "castedTarget");

            var expressions = new List<Expression>();

            expressions.Add(Expression.Assign(sourceVariable, Expression.Convert(sourceParameter, sourceType)));
            expressions.Add(Expression.Assign(targetVariable, Expression.Convert(targetParameter, targetType)));

            foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead)
                    continue;

                var targetProperty = targetType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                if (targetProperty != null
                    && targetProperty.CanWrite
                    && targetProperty.PropertyType.IsAssignableFrom(property.PropertyType))
                {
                    expressions.Add(
                        Expression.Assign(
                            Expression.Property(targetVariable, targetProperty),
                            Expression.Convert(
                                Expression.Property(sourceVariable, property), targetProperty.PropertyType)));
                }
            }

            var lambda =
                Expression.Lambda<Action<object, object>>(
                    Expression.Block(new[] { sourceVariable, targetVariable }, expressions),
                    new[] { sourceParameter, targetParameter });

            var del = lambda.Compile();

            del(source, target);
        }
    }
}