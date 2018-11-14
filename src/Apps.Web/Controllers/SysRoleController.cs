using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models;
using Unity.Attributes;
using Apps.IBLL;

using Apps.Models.Sys;
using Apps.Web.Core;
using Apps.Web;
using Apps.Locale;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysRoleController : BaseController
    {
        //
        // GET: /SysRole/
        [Dependency]
        public ISysRoleBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            
            return View();
        }
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager,string queryStr)
        {
            List<SysRoleModel> list = m_BLL.GetList(ref pager, queryStr);
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
                           CreatePerson = r.CreatePerson,
                            UserName = r.UserName

                        }).ToArray()

            };

            return Json(json);
        }


        #region 设置角色用户
        [SupportFilter(ActionName = "Allot")]
        public ActionResult GetUserByRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.StructTree = commonHelper.GetStructTree(true);
            return View();
        }

        [SupportFilter(ActionName="Allot")]
        public JsonResult GetUserListByRole(GridPager pager, string roleId, string depId, string queryStr)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return Json(0);
            var userList = m_BLL.GetUserByRoleId(ref pager, roleId, depId,queryStr);

            var jsonData = new
            {
                total = pager.totalRows,
                rows = (
                    from r in userList
                    select new SysUserModel()
                    {
                        Id = r.Id,
                        UserName = r.UserName,
                        TrueName = r.TrueName,
                        Flag = r.flag ==0 ? "0" : "1",
                    }
                ).ToArray()
            };
            return Json(jsonData);
        }
        #endregion

        [SupportFilter(ActionName = "Save")]
        public JsonResult UpdateUserRoleByRoleId(string roleId,string userIds)
        {
            string[] arr = userIds.Split(',');

            if (m_BLL.UpdateSysRoleSysUser(roleId,arr))
            {
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + arr, "成功", "分配用户", "角色设置");
                return Json(JsonHandler.CreateMessage(1, Resource.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + arr, "失败", "分配用户", "角色设置");
                return Json(JsonHandler.CreateMessage(0, Resource.SetFail), JsonRequestBehavior.AllowGet);
            }

            
           
        }


        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysRoleModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = GetUserId();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            
            SysRoleModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysRoleModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":"+ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            
            SysRoleModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (id == "administrator")
                {
                    LogHandler.WriteServiceLog(GetUserId(), "尝试删除管理员组", "失败", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, "超级管理员组不能被删除！"), JsonRequestBehavior.AllowGet);
                }
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion

    }
}
