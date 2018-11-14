using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Apps.BLL.Core;
using Apps.Common;
using Apps.Core;
using Unity.Attributes;
using Quartz;
using Quartz.Impl;
using Apps.Models.Sys;
using System.Collections.Specialized;
using Apps.Web.Core.Signalr;
using Apps.BLL.Sys;

namespace Apps.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            #if DEBUG
            BundleTable.EnableOptimizations = false;//关闭文件压缩功能
            #else
            BundleTable.EnableOptimizations = true;//开启文件压缩功能
            #endif
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
            //启动任务调度系统
            if (siteConfig.taskstatus == 1)
            {
                StartScheduler();
            }

            UnityConfig.RegisterComponents();


        }
        //启动调试系统
        public IScheduler sched;
        protected void StartScheduler()
        {
            NameValueCollection properties = new QuartzPara().GetQuartzPara();
            ISchedulerFactory schedFact = new StdSchedulerFactory(properties);

            // get a scheduler
            sched = schedFact.GetScheduler();
            //清空任务
            //sched.Clear();

            //Start
            sched.Start();

        }
        /// <summary>
        /// 全局的异常处理
        /// </summary>
        public void ExceptionHandlerStarter()
        {
            SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
            if (siteConfig.globalexceptionstatus == 1)
            {
                string s = HttpContext.Current.Request.Url.ToString();
                HttpServerUtility server = HttpContext.Current.Server;
                if (server.GetLastError() != null)
                {
                    Exception lastError = server.GetLastError();
                    // 此处进行异常记录，可以记录到数据库或文本，也可以使用其他日志记录组件。
                    ExceptionHander.WriteException(lastError);
                    Application["LastError"] = lastError;
                    int statusCode = HttpContext.Current.Response.StatusCode;
                    string exceptionOperator = siteConfig.globalexceptionurl;
                    try
                    {
                        if (!String.IsNullOrEmpty(exceptionOperator))
                        {
                            exceptionOperator = new System.Web.UI.Control().ResolveUrl(exceptionOperator);
                            string url = string.Format("{0}?ErrorUrl={1}", exceptionOperator, server.UrlEncode(s));
                            string script = String.Format("<script language='javascript' type='text/javascript'>window.top.location='{0}';</script>", url);
                            Response.Write(script);
                            Response.End();
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 全局的异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
#if DEBUG
            //调试状态不进行异常跟踪
#else
                   ExceptionHandlerStarter();
#endif
        }

    }
}