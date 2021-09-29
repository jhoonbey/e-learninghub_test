using app.domain.Model.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace app.domain.Utilities
{
    public class Common
    {
        public static List<EnumModel> ConvertEnumToDDL(Type myEnum, List<int> extractIdList)
        {
            List<EnumModel> result = new List<EnumModel>();

            Array allValues = Enum.GetValues(myEnum);
            foreach (var value in allValues)
            {
                int id = (int)value;

                if (extractIdList == null)
                {
                    result.Add(new EnumModel() { Id = id, Name = value.ToString() });
                }
                else
                {
                    if (!extractIdList.Contains(id))
                    {
                        result.Add(new EnumModel() { Id = id, Name = value.ToString() });
                    }
                }
            }

            return result;
        }

        public static string CRP(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random();
            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        public static string ConvertIdSetToString(List<int> list)
        {
            if (list == null || list.Count() < 1)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item.ToString());
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public static List<int> ConvertStringToIdSet(string str)
        {
            List<int> result = new List<int>();

            if (string.IsNullOrEmpty(str))
            {
                return result;
            }

            List<string> list = str.Split(',').ToList();

            foreach (var item in list)
            {
                int parsed;
                if (Int32.TryParse(item, out parsed) == true)
                {
                    result.Add(parsed);
                }
            }

            return result;
        }
        public static List<string> ConvertStringToStringSet(string str)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(str))
            {
                return result;
            }

            List<string> list = str.Split(',').ToList();
            foreach (var item in list)
            {
                result.Add(item);
            }

            return result;
        }
        public static List<string> ConvertStringToStringSet(string str, char seperator)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(str))
            {
                return result;
            }

            List<string> list = str.Split(seperator).ToList();
            foreach (var item in list)
            {
                result.Add(item);
            }

            return result;
        }
        public static string RemoveItemFromIdListString(string str, int id)
        {
            List<int> result = ConvertStringToIdSet(str);

            if (result.Remove(id))
            {
                return ConvertIdSetToString(result);
            }

            return str;
        }
        public static string ConvertDateToStringDDMMYYY(DateTime date, string seperator)
        {
            if (date == null) return string.Empty;
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();
            if (month.Length == 1) month = string.Concat("0", month);
            if (day.Length == 1) day = string.Concat("0", day);
            return day + seperator + month + seperator + year;
        }
        public static string[] DateTimeStringFormats()
        {
            return new string[]
            {
               "MM/dd/yyyy",                                // 07/21/2007 
               "dddd, dd MMMM yyyy",                        //Saturday, 21 July 2007
               "dddd, dd MMMM yyyy HH:mm",                  // Saturday, 21 July 2007 14:58
               "dddd, dd MMMM yyyy hh:mm tt",               // Saturday, 21 July 2007 03:00 PM
               "dddd, dd MMMM yyyy H:mm",                   // Saturday, 21 July 2007 5:01 
               "dddd, dd MMMM yyyy h:mm tt",                // Saturday, 21 July 2007 3:03 PM
               "dddd, dd MMMM yyyy HH:mm:ss",               // Saturday, 21 July 2007 15:04:10
               "dddd, dd MMMM yyyy HH:mm:ss tt",            // Saturday, 21 July 2007 15:04:10 PM
               "MM/dd/yyyy HH:mm",                          // 07/21/2007 15:05
               "MM/dd/yyyy hh:mm tt",                       // 07/21/2007 03:06 PM
               "MM/dd/yyyy H:mm",                           // 07/21/2007 15:07
               "MM/dd/yyyy h:mm tt",                        // 07/21/2007 3:07 PM
               "MM/dd/yyyy HH:mm:ss",                       // 07/21/2007 15:09:29
               "MM/dd/yyyy HH:mm:ss tt",                    // 07/21/2007 15:09:29 PM
               "MMMM dd",                                   // July 21
               "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK",    // 2007-07-21T15:11:19.1250000+05:30    
               "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",       // Sat, 21 Jul 2007 15:12:16 GMT
               "yyyy'-'MM'-'dd'T'HH':'mm':'ss",             // 2007-07-21T15:12:57
               "HH:mm",                                     // 15:14
               "hh:mm tt",                                  // 03:14 PM
               "H:mm",                                      // 5:15
               "h:mm tt",                                   // 3:16 PM
               "HH:mm:ss",                                  // 15:16:29
               "yyyy'-'MM'-'dd HH':'mm':'ss'Z'",            // 2007-07-21 15:17:20Z
               "dddd, dd MMMM yyyy HH:mm:ss",               // Saturday, 21 July 2007 15:17:58
               "yyyy MMMM",                                 // 2007 July    
               "yyyy-MM-dd"                                 // 2007-07-21
            };
        }
        public static DateTime? ConvertStringToDateTime(string strDate, string[] formats)
        {
            DateTime result;
            if (DateTime.TryParseExact(strDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }
        public static bool IsOnlySpace(string str)
        {
            bool result = true;
            foreach (var item in str)
            {
                if (item != ' ')
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        public static bool IsOnlyEditorSpace(string str)
        {
            bool result = true;
            foreach (var item in str)
            {
                if (item != ' ' && item != '<' && item != 'p' && item != '>' && item != '/')
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        public static string StripHTML(string inputString)
        {
            try
            {
                return Regex.Replace(inputString, "<.*?>", string.Empty);
            }
            catch (Exception)
            {
                return inputString;
            }
        }
        public static string StripHTML2(string inputString)
        {
            try
            {
                string noHTML = Regex.Replace(inputString, @"<[^>]+>|&nbsp;", "").Trim();
                return Regex.Replace(noHTML, @"\s{2,}", " ");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        public static Tuple<int, int> FindImageSizeIfOverLimit(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
        {
            int newWidth = maxWidth;
            int newHeight = maxHeight;

            if (originalHeight > maxHeight || originalWidth > maxWidth)
            {
                if (originalHeight > maxHeight)
                {
                    newHeight = maxHeight;
                    float temp = ((float)originalWidth / (float)originalHeight) * (float)maxHeight;
                    newWidth = Convert.ToInt32(temp);
                    originalHeight = newHeight;
                    originalWidth = newWidth;
                }

                if (originalWidth > maxWidth)
                {
                    newWidth = maxWidth;
                    float temp = ((float)originalHeight / (float)originalWidth) * (float)maxWidth;
                    newHeight = Convert.ToInt32(temp);
                }
            }
            else
            {
                newWidth = originalWidth;
                newHeight = originalHeight;
            }

            return Tuple.Create(newWidth, newHeight);
        }
        public static long ConvertDateTimeToUnixTime(DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }
        public static DateTime ConvertUnixTimeToDateTime(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
        }

        //public static bool CVC(int id, string cName, string objectName, int mt)
        //{
        //    bool willIncrease = false;

        //    // check will calculate or not
        //    if (System.Web.HttpContext.Current.Request.Cookies[cName] != null)
        //    {
        //        if (System.Web.HttpContext.Current.Request.Cookies[cName][string.Format("tId_{0}", id)] == null)
        //        {
        //            HttpCookie cookie = (HttpCookie)System.Web.HttpContext.Current.Request.Cookies[cName];
        //            cookie[string.Format(objectName + "{0}", id)] = "1";
        //            cookie.Expires = DateTime.Now.AddMinutes(mt);
        //            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        //            willIncrease = true;
        //        }
        //    }
        //    else
        //    {
        //        HttpCookie cookie = new HttpCookie(cName);
        //        cookie[string.Format(objectName + "{0}", id)] = "1";
        //        cookie.Expires = DateTime.Now.AddMinutes(mt);
        //        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        //        willIncrease = true;
        //    }

        //    return willIncrease;
        //}
    }
}
