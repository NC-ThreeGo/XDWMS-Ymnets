using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;

namespace Apps.Common
{
    public class CookieHelper
    {
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue,bool isEncrypt)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            if (isEncrypt)
            {
                cookie.Value =DESEncrypt.Encrypt(strValue);
            }
            else
            {
                cookie.Value = strValue;
            }
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires,bool isEncrypt)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            if (isEncrypt)
            {
                cookie.Value = DESEncrypt.Encrypt(strValue);
            }
            else
            {
                cookie.Value = strValue;
            }
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName,bool isEncrypt)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                if (isEncrypt)
                {
                    return  DESEncrypt.Decrypt(HttpContext.Current.Request.Cookies[strName].Value.ToString());
                }
                else
                {
                    return HttpContext.Current.Request.Cookies[strName].Value.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// 获取所有购物车产品 网上超市
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllChoppingCarByPocket()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["Products"] != null)
            {
                int count = System.Web.HttpContext.Current.Request.Cookies["Products"].Values.Count;
                DataColumn dcid = new DataColumn("id");
                DataColumn dcnum = new DataColumn("num");
                DataTable dt = new DataTable();
                dt.Columns.Add(dcid);
                dt.Columns.Add(dcnum);
                string[] str = System.Web.HttpContext.Current.Request.Cookies["Products"].Value.Split('&');
                for (int i = 0; i < str.Length; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["id"] = (str[i].Split('='))[0].ToString();
                    dr["num"] = (str[i].Split('='))[1].ToString();
                    if (int.Parse((str[i].Split('='))[1].ToString()) != 0)
                    {
                        dt.Rows.Add(dr);
                    }
                }
                return dt;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 删除产品 网上超市
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveShoppingCarByPocket(string id)
        {
            if (System.Web.HttpContext.Current.Request.Cookies["Products"] != null && System.Web.HttpContext.Current.Request.Cookies["Products"].Values[id] != null)
            {
                System.Web.HttpCookie cookie;
                System.Web.HttpContext.Current.Request.Cookies["Products"].Values[id] = "0";
                string cookievalue = System.Web.HttpContext.Current.Request.Cookies["Products"].Value;
                cookie = new System.Web.HttpCookie("Products", cookievalue);
                System.Web.HttpContext.Current.Response.AppendCookie(cookie);
            }
           
        }
    }
}
