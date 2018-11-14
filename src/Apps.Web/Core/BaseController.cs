using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models.Sys;
using Apps.BLL.Sys;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Apps.Web.Core
{
    public class BaseController : Controller
    {

        SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));

        /// <summary>
        /// 获取当前用户Id
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            if (Session["Account"] != null)
            {
                AccountModel info = (AccountModel)Session["Account"];
                return info.Id;
            }
            else
            {

                return "";
            }
        }

        /// <summary>
        /// 获取当前用户Name
        /// </summary>
        /// <returns></returns>
        public string GetUserTrueName()
        {
            if (Session["Account"] != null)
            {
                AccountModel info = (AccountModel)Session["Account"];
                return info.TrueName;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        public AccountModel GetAccount()
        {
            if (Session["Account"] != null)
            {
                return (AccountModel)Session["Account"];
            }
            return null;
        }


        /// <summary>
        /// 获取当前页或操作访问权限
        /// </summary>
        /// <returns>权限列表</returns>
        public List<permModel> GetPermission()
        {
            string filePath = HttpContext.Request.FilePath;

            List<permModel> perm = (List<permModel>)Session[filePath];
            return perm;
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new ToJsonResult
            {
                Data = data,
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                JsonRequestBehavior = behavior,
                FormateStr = "yyyy-MM-dd HH:mm:ss"
            };

        }
        //Action Handle
        protected ContentResult JsonDate(object Data)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

            return Content(JsonConvert.SerializeObject(Data, Formatting.Indented, timeConverter));
        }

        /// <summary>
        /// 返回JsonResult.24         /// </summary>
        /// <param name="data">数据</param>
        /// <param name="behavior">行为</param>
        /// <param name="format">json中dateTime类型的格式</param>
        /// <returns>Json</returns>
        protected JsonResult MyJson(object data, JsonRequestBehavior behavior, string format)
        {
            return new ToJsonResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
                FormateStr = format
            };
        }
        /// <summary>
        /// 返回JsonResult42         /// </summary>
        /// <param name="data">数据</param>
        /// <param name="format">数据格式</param>
        /// <returns>Json</returns>
        protected JsonResult MyJson(object data, string format)
        {
            return new ToJsonResult
            {
                Data = data,
                FormateStr = format
            };
        }
        /// <summary>
        /// 检查SQL语句合法性
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ValidateSQL(string sql, ref string msg)
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

            if (sql.ToLower().IndexOf("drop") > 0)
            {
                msg = "查询参数中含有非法语句drop";
                return false;
            }
            return true;
        }

        public void SetDataTransparent()
        {

        }

        //无分页获取
        public GridPager setNoPagerAscBySort = new GridPager() {
            page = 1,
            rows = 10000,
            sort = "Sort",
            order = "asc"
        };

        public GridPager setNoPagerDescBySort = new GridPager()
        {
            page = 1,
            rows = 10000,
            sort = "Sort",
            order = "desc"
        };
        public GridPager setNoPagerDescByCreateTime = new GridPager()
        {
            page = 1,
            rows = 10000,
            sort = "CreateTime",
            order = "desc"
        };
        public GridPager setNoPagerAscById = new GridPager()
        {
            page = 1,
            rows = 10000,
            sort = "Id",
            order = "asc"
        };
        public GridPager setNoPagerDescById = new GridPager()
        {
            page = 1,
            rows = 10000,
            sort = "Id",
            order = "desc"
        };

        //用于主页
        public static string GetFieldPerm(List<permModel> perm, string field)
        {
            return perm.Where(a => a.KeyCode == field).Count() > 0 ? "true" : "false";
        }
        //用于编辑 >0表示禁用。不显示
        public static string ShowFieldPerm(List<permModel> perm, string field)
        {
            return perm.Where(a => a.KeyCode == field).Count() > 0 ?  "style=display:none":"";
        }

    }
}