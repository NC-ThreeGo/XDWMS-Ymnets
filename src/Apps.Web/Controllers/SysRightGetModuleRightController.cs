using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Apps.Common;

using Apps.IBLL;
using Unity.Attributes;
using Apps.Models;
using Apps.Models.Sys;
using Apps.Web.Core;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysRightGetModuleRightController : BaseController
    {
        //
        // GET: /SysRightGetModuleRight/
        [Dependency]
        public ISysRightGetModuleRightBLL sysRightModuleRightBLL { get; set; }
        [Dependency]
        public ISysModuleBLL sysModuleBLL { get; set; }
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        //获取模组列表
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetModelList(string id)
        {
            if (id == null)
                id = "0";
            List<SysModuleModel> list = sysModuleBLL.GetList(id);
            var jsonData = from r in list
                       select new SysModuleModel()
                       {
                           Id = r.Id,
                           Name = r.Name,
                           EnglishName = r.EnglishName,
                           ParentId = r.ParentId,
                           Url = r.Url,
                           Iconic = r.Iconic,
                           Sort = r.Sort,
                           Remark = r.Remark,
                           Enable = r.Enable,
                          CreatePerson = r.CreatePerson,
                           CreateTime = r.CreateTime,
                           IsLast = r.IsLast,
                           state = (sysModuleBLL.GetList(r.Id).Count > 0) ? "closed" : "open"
                       };


            return Json(jsonData);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetModuleOperate(GridPager pager, string moduleId)
        {
            pager.rows = 10000;
            var right = sysRightModuleRightBLL.GetModuleOperateByModuleId(moduleId);
            var jsonData = new
            {
                total = pager.totalRows,
                rows = (from r in right
                        select new SysRightModelByRoleAndModuleModel()
                        {
                            Ids = r.Id,
                            Name = r.Name,
                            KeyCode = r.KeyCode
                        }).ToArray()

            };

            return Json(jsonData);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetModuleUserRight(GridPager pager, string moduleId)
        {
         
            var rightList = sysRightModuleRightBLL.GetModuleUserRight(moduleId);

            var jsonData = new
            {
                total = pager.totalRows,
                rows = (from r in rightList
                        select new UserRight()
                        {
                            Ids = r.Id,
                            UserName = r.Name,
                            RightList = r.keyCode
                        }).ToArray()
            };

             return Json(jsonData);
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetModuleRoleRight(GridPager pager, string moduleId)
        {

            var rightList = sysRightModuleRightBLL.GetModuleRoleRight(moduleId);

            var jsonData = new
            {
                total = pager.totalRows,
                rows = (from r in rightList
                        select new RoleRight()
                        {
                            Ids = r.Id,
                            RoleName = r.Name,
                            RightList = r.keyCode
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}
