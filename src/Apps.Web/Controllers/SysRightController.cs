using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity.Attributes;
using Apps.IBLL;
using Apps.Models;
using Apps.Common;
using Apps.Models.Sys;
using Apps.Web.Core;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysRightController : BaseController
    {
        //
        // GET: /SysRight/
        [Dependency]
        public ISysRightBLL sysRightBLL { get; set; }
        [Dependency]
        public ISysRoleBLL sysRoleBLL { get; set; }
        [Dependency]
        public ISysModuleBLL sysModuleBLL { get; set; }
        [SupportFilter]
        public ActionResult Index()
        {
            CommonHelper commonHelper = new CommonHelper();
            return View();
        }
        //获取角色列表
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetRoleList(GridPager pager)
        {
            List<SysRoleModel> list = sysRoleBLL.GetList(ref pager, "");
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysRoleModel()
                        {

                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description,
                            CreateTime = r.CreateTime,
                           CreatePerson = r.CreatePerson

                        }).ToArray()

            };

            return Json(json);
        }
         //获取模组列表
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetModelList(string id)
         {
             if (id == null)
                 id = "0";
             List<SysModuleModel> list = sysModuleBLL.GetList(id);
             var json = from r in list
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


             return Json(json);
         }

         //根据角色与模块得出权限
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetRightByRoleAndModule(GridPager pager, string roleId, string moduleId)
         {
             pager.rows = 100000;
             var right = sysRightBLL.GetRightByRoleAndModule(roleId,moduleId);
             var json = new
             {
                 total = pager.totalRows,
                 rows = (from r in right
                         select new SysRightModelByRoleAndModuleModel()
                         {
                            Ids= r.RightId+ r.KeyCode,
                            Name= r.Name,
                            KeyCode =r.KeyCode,
                            IsValid=r.isvalid,
                            RightId=r.RightId
                         }).ToArray()

             };

             return Json(json);
         }


        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetRightDataByRoleAndModule(GridPager pager, string roleId, string moduleId)
        {
            pager.rows = 100000;
            var right = sysRightBLL.GetRightDataByRoleAndModule(roleId, moduleId);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in right
                        select new SysRightDataModelByRoleAndModuleModel()
                        {
                            Ids = r.RightId + r.KeyCode,
                            Name = r.Name,
                            KeyCode = r.KeyCode,
                            IsValid = r.isvalid,
                            RightId = r.RightId
                        }).ToArray()

            };

            return Json(json);
        }

        //保存
        [HttpPost]
        [SupportFilter(ActionName = "Save")]
        public int UpdateRight(SysRightOperateModel model)
        {
            return sysRightBLL.UpdateRight(model);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Save")]
        public int UpdateDataRight(SysRightDataFilterModel model)
        {
            return sysRightBLL.UpdateDataRight(model);
        }
    }
}
