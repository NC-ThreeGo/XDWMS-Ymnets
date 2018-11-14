using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Apps.Models.Sys;
using Apps.BLL;
using Apps.DAL;
using Apps.Models;
using Apps.Core;
using Apps.Common;
using Apps.DAL.Sys;
using Apps.BLL.Sys;

namespace Apps.Web.Core
{
    public class SupportFilterAttribute : ActionFilterAttribute
    {
        public string ActionName { get; set; }
        private string Area;
        // 方法被执行后的更新在线用户列表
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //OnlineHttpModule.ProcessRequest();

        }
        /// <summary>
        /// Action加上[SupportFilter]在执行actin之前执行以下代码，通过[SupportFilter(ActionName="Index")]指定参数
        /// </summary>
        /// <param name="filterContext">页面传过来的上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //过滤危险字符,需要设置规则，暂时注掉，需要开启
            //var actionParameters = filterContext.ActionDescriptor.GetParameters();
            //foreach (var p in actionParameters)
            //{
            //    if (p.ParameterType == typeof(string))
            //    {
            //        if (filterContext.ActionParameters[p.ParameterName] != null)
            //        {
            //            filterContext.ActionParameters[p.ParameterName] = ResultHelper.Formatstr(filterContext.ActionParameters[p.ParameterName].ToString());
            //        }
            //    }
            //}
            //读取请求上下文中的Controller,Action,Id
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            RouteData routeData = routes.GetRouteData(filterContext.HttpContext);
            //取出区域的控制器Action,id
            string ctlName = filterContext.Controller.ToString();
            string[] routeInfo = ctlName.Split('.');
            string controller = null;
            string action = null;
            string id = null;

            int iAreas = Array.IndexOf(routeInfo, "Areas");
            if (iAreas > 0)
            {
                //取区域及控制器
                Area = routeInfo[iAreas + 1];
            }
            int ctlIndex = Array.IndexOf(routeInfo, "Controllers");
            ctlIndex++;
            controller = routeInfo[ctlIndex].Replace("Controller", "").ToLower();

            string url = HttpContext.Current.Request.Url.ToString().ToLower();
            string[] urlArray = url.Split('/');
            int urlCtlIndex = Array.IndexOf(urlArray, controller);
            urlCtlIndex++;
            if (urlArray.Count() > urlCtlIndex)
            {
                action = urlArray[urlCtlIndex];
            }
            urlCtlIndex++;
            if (urlArray.Count() > urlCtlIndex)
            {
                id = urlArray[urlCtlIndex];
            }
            //url
            action = string.IsNullOrEmpty(action) ? "Index" : action;
            int actionIndex = action.IndexOf("?", 0);
            if (actionIndex > 1)
            {
                action = action.Substring(0, actionIndex);
            }
            id = string.IsNullOrEmpty(id) ? "" : id;

            //URL路径
            string filePath = HttpContext.Current.Request.FilePath;
            AccountModel account = filterContext.HttpContext.Session["Account"] as AccountModel;
            if (LoginUserManage.ValidateIsLogined(account) && ValiddatePermission(account, controller, action, filePath) && !LoginUserManage.ValidateRelogin(account))
            {
                //已经登录，有权限，且没有单机登录限制
                return;
            }
            else
            {
                filterContext.Result = new EmptyResult();
                return;
            }
        }
        public bool ValiddatePermission(AccountModel account, string controller, string action, string filePath)
        {
            bool bResult = false;
            string actionName = string.IsNullOrEmpty(ActionName) ? action : ActionName;
            if (account != null)
            {
                List<permModel> perm = null;
                //测试当前controller是否已赋权限值，如果没有从
                //如果存在区域,Seesion保存（区域+控制器）
                if (!string.IsNullOrEmpty(Area))
                {
                    controller = Area + "/" + controller;
                }
                perm = (List<permModel>)HttpContext.Current.Session[filePath];
                if (perm == null)
                {
                    SysUserBLL userBLL = new SysUserBLL()
                    {
                        m_Rep = new SysUserRepository(new DBContainer()),
                        sysRightRep = new SysRightRepository(new DBContainer())
                    };
                    {
                        perm = userBLL.GetPermission(account.Id, controller);//获取当前用户的权限列表
                        HttpContext.Current.Session[filePath] = perm;//获取的劝降放入会话由Controller调用
                    }
                }
                //当用户访问index时，只要权限>0就可以访问
                if (actionName.ToLower() == "index")
                {
                    if (perm.Count > 0)
                    {
                        return true;
                    }
                }
                //查询当前Action 是否有操作权限，大于0表示有，否则没有
                int count = perm.Where(a => a.KeyCode.ToLower() == actionName.ToLower()).Count();
                if (count > 0)
                {
                    bResult = true;
                }
                else
                {
                    bResult = false;
                    LoginUserManage.RedirectUrl();
                }

            }
            return bResult;
        }
      
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }


}