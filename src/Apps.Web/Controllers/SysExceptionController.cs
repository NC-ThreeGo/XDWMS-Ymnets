using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Reflection;
using System.Text;
using Apps.Common;
using Apps.Models;
using Apps.IBLL;
using Apps.Models.Sys;
using Unity.Attributes;
using Apps.Web.Core;
using Apps.Locale;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysExceptionController : BaseController
    {
        //
        // GET: /SysException/
        [Dependency]
        public ISysExceptionBLL exceptionBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            
            return View();

        }
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<SysExceptionModel> list = exceptionBLL.GetList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysExceptionModel()
                        {
                            Id = r.Id,
                            HelpLink =r.HelpLink,
                            Message = r.Message,
                            Source = r.Source,
                            StackTrace = r.StackTrace,
                            TargetSite = r.TargetSite,
                            Data = r.Data,
                            CreateTime = r.CreateTime
                        }).ToArray()

            };
            return Json(json);
        }


        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            
            SysExceptionModel entity = exceptionBLL.GetById(id);

            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string ids)
        {
            if (!string.IsNullOrWhiteSpace(ids))
            {
                string[] deleteIds = ids.Split(',');
                if (exceptionBLL.Delete(ref errors, deleteIds))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Ids:" + ids, "成功", "删除", "系统异常");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + ids + "," + ErrorCol, "失败", "删除", "系统异常");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        public ActionResult Error()
        {

            BaseException ex = new BaseException();
            return View(ex);
        }

    }


    public class BaseException
    {
        #region 变量
        private string exceptionMessage;
        private string exceptionName;
        private string innerExceptionMessage;
        private string innerExceptionName;
        private bool isShow;
        private Exception outermostException;
        private string sourceErrorFile;
        private string sourceErrorRowID;
        private string stackInfo;
        private string targetSite;
        #endregion

        #region 属性
        public string ErrorPageUrl
        {
            get
            {
                return this.GetExceptionUrl();
            }
        }
        public Exception Exception
        {
            get
            {
                return (HttpContext.Current.Session["Exception"] as Exception);
            }
            private set
            {
                HttpContext.Current.Session["Exception"] = value;
            }
        }
        public string ExceptionMessage
        {
            get
            {
                return this.exceptionMessage;
            }
            private set
            {
                this.exceptionMessage = value;
            }
        }
        public string ExceptionName
        {
            get
            {
                return this.exceptionName;
            }
            private set
            {
                this.exceptionName = value;
            }
        }
        public string InnerExceptionMessage
        {
            get
            {
                return this.innerExceptionMessage;
            }
            private set
            {
                this.innerExceptionMessage = value;
            }
        }
        public string InnerExceptionName
        {
            get
            {
                return this.innerExceptionName;
            }
            private set
            {
                this.innerExceptionName = value;
            }
        }
        public bool IsShowStackInfo
        {
            get
            {
                return this.isShow;
            }
            private set
            {
                this.isShow = value;
            }
        }
        public string SourceErrorFile
        {
            get
            {
                return this.sourceErrorFile;
            }
            private set
            {
                this.sourceErrorFile = value;
            }
        }
        public string SourceErrorRowID
        {
            get
            {
                return this.sourceErrorRowID;
            }
            private set
            {
                this.sourceErrorRowID = value;
            }
        }
        public string StackInfo
        {
            get
            {
                return this.stackInfo;
            }
            private set
            {
                this.stackInfo = value;
            }
        }
        public string TargetSite
        {
            get
            {
                return this.targetSite;
            }
            private set
            {
                this.targetSite = value;
            }
        }
        #endregion


        public BaseException()
        {
            this.outermostException = null;
            this.exceptionName = null;
            this.exceptionMessage = null;
            this.innerExceptionName = null;
            this.innerExceptionMessage = null;
            this.targetSite = null;
            this.stackInfo = null;
            this.sourceErrorFile = null;
            this.sourceErrorRowID = null;
            this.isShow = false;
            try
            {
                this.Exception = HttpContext.Current.Application["LastError"] as Exception;
                if (this.Exception != null)
                {
                    this.outermostException = this.Exception;
                    if ((this.Exception is HttpUnhandledException) && (this.Exception.InnerException != null))
                    {
                        this.Exception = this.Exception.InnerException;
                    }
                    this.ExceptionName = this.GetExceptionName(this.Exception);
                    this.ExceptionMessage = this.GetExceptionMessage(this.Exception);
                    if (this.Exception.InnerException != null)
                    {
                        this.InnerExceptionName = this.GetExceptionName(this.Exception.InnerException);
                        this.InnerExceptionMessage = this.GetExceptionMessage(this.Exception.InnerException);
                    }
                    this.TargetSite = this.GetTargetSite(this.Exception);
                    this.StackInfo = this.GetStackInfo(this.Exception);
                    if ((this.outermostException is HttpUnhandledException) && (this.outermostException.InnerException != null))
                    {
                        this.StackInfo = this.StackInfo + "\r\n<a href='#' onclick=\"if(document.getElementById('phidden').style.display=='none') document.getElementById('phidden').style.display='block'; else document.getElementById('phidden').style.display='none'; return false;\"><b>[" + this.outermostException.GetType().ToString() + "]</b></a>\r\n";
                        this.StackInfo = this.StackInfo + "<pre id='phidden' style='display:none;'>" + this.outermostException.StackTrace + "</pre>";
                    }
                    this.SourceErrorFile = this.GetSourceErrorFile();
                    this.SourceErrorRowID = this.GetSourceErrorRowID();
                    this.IsShowStackInfo = true;
                }
                HttpContext.Current.Application["LastError"] = null;
            }
            catch (Exception exception)
            {
                this.ExceptionMessage = "异常基页出错" + exception.Message;
            }
        }

        #region 方法
        private string GetExceptionMessage(Exception ex)
        {
            return ex.Message;
        }

        private string GetExceptionMessageForLog()
        {
            StringBuilder builder = new StringBuilder(50);
            builder.AppendFormat("<ExceptionName>{0}</ExceptionName>", this.ExceptionName);
            builder.AppendFormat("<ExceptionMessage>{0}</ExceptionMessage>", this.ExceptionMessage);
            builder.AppendFormat("<InnerExceptionName>{0}</InnerExceptionName>", this.InnerExceptionName);
            builder.AppendFormat("<InnerExceptionMessage>{0}</InnerExceptionMessage>", this.InnerExceptionMessage);
            builder.AppendFormat("<TargetSite>{0}</TargetSite>", this.TargetSite);
            builder.AppendFormat("<ErrorPageUrl>{0}</ErrorPageUrl>", this.ErrorPageUrl);
            builder.AppendFormat("<SourceErrorFile>{0}</SourceErrorFile>", this.SourceErrorFile);
            builder.AppendFormat("<SourceErrorRowID>{0}</SourceErrorRowID>", this.SourceErrorRowID);
            return builder.ToString();
        }

        private string GetExceptionMessageForMail()
        {
            StringBuilder builder = new StringBuilder(50);
            builder.Append("<ExceptionInfo>");
            builder.Append(this.GetExceptionMessageForLog());
            builder.AppendFormat("<StackInfo><![CDATA[{0}]]></StackInfo>", this.StackInfo);
            builder.Append("</ExceptionInfo>");
            return builder.ToString();
        }

        private string GetExceptionName(Exception ex)
        {
            string str = null;
            if (ex != null)
            {
                str = ex.GetType().FullName;
            }

            return str;
        }

        private string GetExceptionUrl()
        {
            string str = null;
            if (HttpContext.Current.Request["ErrorUrl"] != null)
            {
                str = HttpContext.Current.Request["ErrorUrl"].ToString();
            }
            return str;
        }

        private string GetSourceErrorFile()
        {
            string stackInfo = this.StackInfo;
            string[] strArray = new string[0];
            if (stackInfo == null)
            {
                return stackInfo;
            }
            strArray = stackInfo.Split(new string[] { "位置", "行号" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length >= 3)
            {
                stackInfo = strArray[1];
                if (stackInfo.LastIndexOf(":") == (stackInfo.Length - 1))
                {
                    stackInfo = stackInfo.Substring(0, stackInfo.Length - 1);
                }
                return stackInfo;
            }
            return "";
        }
        private string GetSourceErrorRowID()
        {
            string stackInfo = this.StackInfo;
            string[] strArray = new string[0];
            if (stackInfo == null)
            {
                return stackInfo;
            }
            strArray = stackInfo.Split(new string[] { "行号" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length >= 2)
            {
                stackInfo = strArray[1].Trim();
                string[] strArray2 = stackInfo.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray2.Length >= 2)
                {
                    stackInfo = strArray2[0];
                }
                return stackInfo;
            }
            return "";
        }
        private string GetStackInfo(Exception ex)
        {
            string str = null;
            if (ex != null)
            {
                str = "<b>[" + ex.GetType().ToString() + "]</b>\r\n" + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    str = this.GetStackInfo(ex.InnerException) + "\r\n" + str;
                }
            }
            return str;
        }
        private string GetTargetSite(Exception ex)
        {
            string str = null;
            if (ex != null)
            {
                ex = this.GetBenmostException(ex);
                MethodBase targetSite = ex.TargetSite;
                if (targetSite != null)
                {
                    str = string.Format("{0}.{1}", targetSite.DeclaringType, targetSite.Name);
                }
            }
            return str;
        }
        protected Exception GetBenmostException(Exception ex)
        {
            while (true)
            {
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    return ex;
                }
            }
        }
        #endregion
    }
}
