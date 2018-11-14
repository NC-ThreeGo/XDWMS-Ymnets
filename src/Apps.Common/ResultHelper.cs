using System;
using System.Web;
using System.Text.RegularExpressions;

namespace Apps.Common
{
    public class ResultHelper
    {
        /// <summary>
        /// 创建一个全球唯一的32位ID
        /// </summary>
        /// <returns>ID串</returns>
        public static string NewId
        {
            get
            {
                string id = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                id += guid.Substring(0, 10);
                return id;
            }
        }
        public static string NewTimeId
        {
            get
            {
                string id = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
                return id.Substring(11,9);
            }
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="length">剩下长度</param>
        /// <returns>指定字符串并加...</returns>
        public static string SubValue(string value, int length)
        {
            if (value.Length > length)
            {
                value = value.Substring(0, length); value = value + "..."; return NoHtml(value);
            }
            else { return NoHtml(value); }
        }
        //还原的时候
        public static string InputText(string inputString)
        {

            if ((inputString != null) && (inputString != String.Empty))
            {
                inputString = inputString.Trim();
                //if (inputString.Length > maxLength) 
                //inputString = inputString.Substring(0, maxLength); 
                inputString = inputString.Replace("<br>", "\n");
                inputString = inputString.Replace("&", "&amp");
                inputString = inputString.Replace("'", "''");
                inputString = inputString.Replace("<", "&lt");
                inputString = inputString.Replace(">", "&gt");
                inputString = inputString.Replace("chr(60)", "&lt");
                inputString = inputString.Replace("chr(37)", "&gt");
                inputString = inputString.Replace("\"", "&quot");
                inputString = inputString.Replace(";", ";");

                return inputString;
            }
            else
            {
                return "";
            }

        }
        //添加的时候
        public static string OutputText(string outputString)
        {

            if ((outputString != null) && (outputString != String.Empty))
            {
                outputString = outputString.Trim();
                outputString = outputString.Replace("&amp", "&");
                outputString = outputString.Replace("''", "'");
                outputString = outputString.Replace("&lt", "<");
                outputString = outputString.Replace("&gt", ">");
                outputString = outputString.Replace("&lt", "chr(60)");
                outputString = outputString.Replace("&gt", "chr(37)");
                outputString = outputString.Replace("&quot", "\"");
                outputString = outputString.Replace(";", ";");
                outputString = outputString.Replace("\n", "<br>");
                return outputString;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHtml(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&hellip;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&mdash;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;

        }
        /// <summary>
        /// 格式化文本（防止SQL注入）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Formatstr(string html)
        {
            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex10 = new System.Text.RegularExpressions.Regex(@"select", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex11 = new System.Text.RegularExpressions.Regex(@"update", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex12 = new System.Text.RegularExpressions.Regex(@"delete", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
            html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件
            html = regex4.Replace(html, ""); //过滤iframe
            html = regex10.Replace(html, "s_elect");
            html = regex11.Replace(html, "u_pudate");
            html = regex12.Replace(html, "d_elete");
            html = html.Replace("'", "’");
            html = html.Replace("&nbsp;", " ");
            return html;
        }
        /// <summary>
        /// 检查SQL语句合法性
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool ValidateSQL(string sql, ref string msg)
        {
            if (sql.ToLower().IndexOf("delete") > 0)
            {
                msg = "查询参数中含有非法语句DELETE";
                return false;
            }
            if (sql.ToLower().IndexOf("update") > 0)
            {
                msg = "查询参数中含有非法语句UPDATE";
                return false;
            }

            if (sql.ToLower().IndexOf("insert") > 0)
            {
                msg = "查询参数中含有非法语句INSERT";
                return false;
            }
            return true;
        }


        //获取当前时间
        public static DateTime NowTime
        {
            get 
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 将日期转换成字符串
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>字符串</returns>
        public static string DateTimeConvertString(DateTime? dt)
        {
            if (dt == null)
            {
                return "";
            }
            else
            {
                return Convert.ToDateTime(dt.ToString()).ToShortDateString();
            }
        }
        /// <summary>
        /// 将字符串转换成日期
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>日期</returns>
        public static DateTime? StringConvertDatetime(string str)
        {
            if (str == null)
            {
                return null ;
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(str);
                }
                catch {
                    return null;
                }
            }
        }

        public static string GetUserIP()
        {
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static DateTime GetTimeByLong(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(timeStamp);
            return dtStart.Add(toNow);
        }
    }
}
