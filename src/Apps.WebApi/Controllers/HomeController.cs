using Apps.Common;
using Apps.IBLL;
using Apps.IBLL.Sys;
using Apps.Models.Sys;
using Unity.Attributes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace Apps.WebApi.Controllers
{
    public class HomeController : Controller
    {
        [Dependency]
        public ISysModuleBLL m_BLL { get; set; }
        [Dependency]
        public ISysModuleOperateBLL operateBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            //第一次运行时候，初始化数据库的表
            InitCurrentApiInterface();
            return View();
        }



        /// <summary>
        /// 将当前所有API接口添加到数据
        /// </summary>
        private void InitCurrentApiInterface()
        {
            //插入一个约定树根数据
            SysModuleModel rootModel = m_BLL.GetById("ApiInterfaceAuth");
            if (rootModel == null)
            {
                SysModuleModel model = new SysModuleModel()
                {
                    Id = "ApiInterfaceAuth",
                    Name = "Api接口授权",
                    EnglishName = "ApiInterfaceAuth",
                    ParentId = "000",
                    Url = "",
                    Iconic = "fa fa-television",
                    Enable = true,
                    Remark = "Api接口授权",
                    Sort = 1,
                    CreatePerson = "Admin",
                    CreateTime = DateTime.Now,
                    IsLast = false
                };
                m_BLL.Create(ref errors, model);
            }
            //把控制器当成URL，把Aciton当成操作码插入到数据表做为权限设置，类似之前的权限系统
            //获得API管理器
            Collection<ApiDescription> apiColl = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            ILookup<HttpControllerDescriptor, ApiDescription> apiGroups = apiColl.ToLookup(api => api.ActionDescriptor.ControllerDescriptor);

            foreach (var group in apiGroups)
            {

                string controllerName = group.Key.ControllerName;
                //----------插入控制器
                rootModel = m_BLL.GetById(controllerName);
                if (rootModel == null)
                {
                    SysModuleModel model = new SysModuleModel()
                    {
                        Id = controllerName,
                        Name = controllerName,
                        EnglishName = "",
                        ParentId = "ApiInterfaceAuth",
                        Url = controllerName,
                        Iconic = "fa fa-television",
                        Enable = true,
                        Remark = "Api接口授权",
                        Sort = 1,
                        CreatePerson = "Admin",
                        CreateTime = DateTime.Now,
                        IsLast = true
                    };
                    m_BLL.Create(ref errors, model);
                }
                //-----------插入Action   
                foreach (var m in group)
                {
                    string actionName = m.ActionDescriptor.ActionName;
                    SysModuleOperateModel model = operateBLL.GetById(m.ActionDescriptor.ActionName);
                    if (model == null)
                    {
                        model = new SysModuleOperateModel();
                        model.Id = controllerName + actionName;
                        model.Name = m.Documentation == null ? actionName : m.Documentation;
                        model.KeyCode = actionName;
                        model.ModuleId = controllerName;
                        model.IsValid = true;
                        model.Sort = 0;
                        operateBLL.Create(ref errors, model);
                    }

                }
            }
        }
    }
}
