using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Apps.Common;
using Unity.Attributes;
using Apps.IBLL;
using Apps.Models.Sys;
using Apps.Models;
using Apps.Web.Core;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysRightGetUserRightController : Controller
    {
        //
        // GET: /SysRightGetUserRight/

        [Dependency]
        public ISysUserBLL sysUserBLL { get; set; }
        [Dependency]
        public ISysRightGetUserRightBLL sysRightGetUserRightBLL { get; set; }

        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }

        //获取用户列表
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetUserList(GridPager pager,string queryStr)
        {
            List<SysUserModel> list = sysUserBLL.GetList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysUserModel()
                        {

                            Id = r.Id,
                            UserName = r.UserName,
                            TrueName = r.TrueName,
                            MobileNumber = r.MobileNumber
                        }).ToArray()

            };

            return Json(json);
        }


        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetUserRight(GridPager pager, string userId,string isApi)
        {
            if (userId == null)
            {
                GridRows<P_Sys_GetRightByUser_Result> grs = new GridRows<P_Sys_GetRightByUser_Result>();
                grs.rows = new List<P_Sys_GetRightByUser_Result>();
                grs.total =0;
                return Json(grs);
            }

            List<P_Sys_GetRightByUser_Result> userRightList = new List<P_Sys_GetRightByUser_Result>();
            if (isApi == "api")
            {
                userRightList = sysRightGetUserRightBLL.GetList(userId).Where(a => a.moduleName.Contains("000Api")).ToList();
            }
            else
            {
                userRightList = sysRightGetUserRightBLL.GetList(userId).Where(a => !a.moduleName.Contains("000Api")).ToList();
            }

            List<P_Sys_GetRightByUser_Result> list = userRightList.Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
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
