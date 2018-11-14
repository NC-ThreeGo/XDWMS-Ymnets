using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Apps.Web.Core
{
    public class HandleError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Exception e = filterContext.Exception;
            string conntrollerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];

            WriteLog(conntrollerName, actionName, e.Message + ":" + e.StackTrace);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("~/Account/Error");
        }

        /// <summary>
        /// 程序错误时调用此函数写txt格式的日志
        /// </summary>
        /// <param name="controllerAndActionName">是由哪个控件器的哪个方法引发的，形如"Action_InitAction"</param>
        /// <param name="argsValue">当时所有参数的描述</param>
        /// <param name="exceptionMsg">抛出的异常信息</param>
        public void WriteLog(string controllerName, string actionName, string exceptionMsg)
        {
            string subFold = DateTime.Now.Year + DateTime.Now.Month.ToString("D2");
            string fileName = subFold + DateTime.Now.Day.ToString("D2") + ".txt";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/LogFile/") + subFold;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string wholePath = path + "\\" + fileName;
            StreamWriter sw = new StreamWriter(wholePath, true, Encoding.UTF8);
            sw.WriteLine(DateTime.Now.ToString()
                         + " >> 程序异常！ At "
                         + controllerName
                         + "."
                         + actionName
                         + "()，相关信息："
                         + exceptionMsg);
            sw.Close();
        }
    }
}