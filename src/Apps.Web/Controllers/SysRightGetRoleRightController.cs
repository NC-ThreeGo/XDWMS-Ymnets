using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models.Sys;
using Unity.Attributes;
using Apps.IBLL;
using Apps.Models;
using Apps.Web.Core;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysRightGetRoleRightController : BaseController
    {
        //
        // GET: /SysRightGetRoleRight/
        [Dependency]
        public ISysRoleBLL sysRoleBLL { get; set; }
        [Dependency]
        public ISysRightGetRoleRightBLL sysRightGetRoleRightBLL { get; set; }
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }

        //获取角色列表
        [SupportFilter(ActionName="Index")]
        public JsonResult GetRoleList(GridPager pager, string queryStr)
        {
            List<SysRoleModel> list = sysRoleBLL.GetList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysRoleModel()
                        {

                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description
                        }).ToArray()

            };

            return Json(json);

      
        }


        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetRoleRight(GridPager pager, string roleId,string isApi)
        {

            if (roleId == null)
                return Json(0);
            List<P_Sys_GetRightByRole_Result> userRightList = new List<P_Sys_GetRightByRole_Result>();
            if (isApi == "api")
            {
                userRightList = sysRightGetRoleRightBLL.GetList(roleId).Where(a=>a.moduleName.Contains("000Api")).ToList();
            }
            else
            {
                userRightList = sysRightGetRoleRightBLL.GetList(roleId).Where(a => !a.moduleName.Contains("000Api")).ToList();
            }
            List<P_Sys_GetRightByRole_Result> list = userRightList.Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
            int totalRecords = userRightList.Count();
            var json = new
            {
                total = totalRecords,
                rows = (from r in list
                        select new SysRightUserRight()
                        {

                            ModuleId = r.moduleId,
                            ModuleName = r.moduleName,
                            KeyCode = r.keyCode,


                        }).ToArray()

            };
            return Json(json);
        }
    }
}
