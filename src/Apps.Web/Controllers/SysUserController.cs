using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models;
using Unity.Attributes;
using Apps.IBLL;
using Apps.Models.Sys;
using System;
using Apps.Web.Core;
using Apps.Locale;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysUserController : BaseController
    {
        //
        // GET: /SysUser/

        [Dependency]
        public ISysUserBLL m_BLL { get; set; }
        [Dependency]
        public ISysStructBLL structBLL { get; set; }
        [Dependency]
        public ISysPositionBLL posBLL { get; set; }
        [Dependency]
        public ISysAreasBLL areasBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        [SupportFilter]
        public ActionResult Index()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.StructTree = commonHelper.GetStructTree(true);
            return View();
        }
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr, string depId)
        {
            List<SysUserModel> list = m_BLL.GetList(ref pager, queryStr, depId);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new SysUserModel()
                        {
                            Id = r.Id,
                            UserName = r.UserName,
                            Password = r.Password,
                            TrueName = r.TrueName,
                            Card = r.Card,
                            MobileNumber = r.MobileNumber,
                            PhoneNumber = r.PhoneNumber,
                            QQ = r.QQ,
                            EmailAddress = r.EmailAddress,
                            OtherContact = r.OtherContact,
                            Province = r.Province,
                            City = r.City,
                            Village = r.Village,
                            Address = r.Address,
                            State = r.State,
                            CreateTime = r.CreateTime,
                            CreatePerson = r.CreatePerson,
                            Sex = r.Sex,
                            Birthday = r.Birthday,
                            JoinDate = r.JoinDate,
                            Marital = r.Marital,
                            Political = r.Political,
                            Nationality = r.Nationality,
                            Native = r.Native,
                            School = r.School,
                            Professional = r.Professional,
                            Degree = r.Degree,
                            DepId = r.DepId,
                            PosId = r.PosId,
                            Expertise = r.Expertise,
                            JobState = r.JobState,
                            Photo = r.Photo,
                            Attach = r.Attach,
                            Lead = r.Lead,
                            LeadName = r.LeadName,
                            IsSelLead = r.IsSelLead,
                            IsReportCalendar = r.IsReportCalendar,
                            IsSecretary = r.IsSecretary,
                            RoleName = r.RoleName,
                            DepName = r.DepName,
                            PosName = r.PosName
                        }).ToArray()

            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public string GetDepName(string depId)
        {
            return structBLL.GetById(depId).Name;
        }
        public string GetPosName(string posId)
        {
            return posBLL.GetById(posId).Name;
        }


        public ActionResult LookUp(string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                ViewBag.owner = "1";
            }
            else
            {
                ViewBag.owner = owner;
            }
            return View();
        }

        #region 设置用户角色
        [SupportFilter(ActionName = "Allot")]
        public ActionResult GetRoleByUser(string userId)
        {
            ViewBag.UserId = userId;


            return View();
        }

        [SupportFilter(ActionName = "Allot")]
        public JsonResult GetRoleListByUser(GridPager pager, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Json(0);
            var userList = m_BLL.GetRoleByUserId(ref pager, userId);
            var jsonData = new
            {
                total = pager.totalRows,
                rows = (
                    from r in userList
                    select new SysRoleModel()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Flag = r.flag == "0" ? "0" : "1",
                    }
                ).ToArray()
            };
            return Json(jsonData);
        }
        #endregion
        [SupportFilter(ActionName = "Save")]
        public JsonResult UpdateUserRoleByUserId(string userId, string roleIds)
        {
            string[] arr = roleIds.Split(',');


            if (m_BLL.UpdateSysRoleSysUser(userId, arr))
            {
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + roleIds, "成功", "分配角色", "用户设置");
                return Json(JsonHandler.CreateMessage(1, Resource.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + roleIds, "失败", "分配角色", "用户设置");
                return Json(JsonHandler.CreateMessage(0, Resource.SetFail), JsonRequestBehavior.AllowGet);
            }

        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {

            ViewBag.Struct = new SelectList(structBLL.GetList("0"), "Id", "Name");
            ViewBag.Areas = new SelectList(areasBLL.GetList("0"), "Id", "Name");
            SysUserModel model = new SysUserModel()
            {
                Password = "123456",
                JoinDate = ResultHelper.NowTime

            };
            return View(model);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysUserModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                model.Id = ResultHelper.NewId;
                model.CreateTime = ResultHelper.NowTime;
                model.Password = ValueConvert.MD5(model.Password);
                model.CreatePerson = GetUserTrueName();
                model.State = true;
                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.UserName, "成功", "创建", "用户设置");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.UserName + "," + ErrorCol,

"失败", "创建", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol),

JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail), JsonRequestBehavior.AllowGet);
            }
        }
        //判断是否用户重复
        [HttpPost]
        public JsonResult JudgeUserName(string userName)
        {
            return Json("用户名已经存在！", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {



            ViewBag.Areas = new SelectList(areasBLL.GetList("0"), "Id", "Name");

            SysUserModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysUserEditModel info)
        {
            if (info != null && ModelState.IsValid)
            {
                if (m_BLL.Edit(ref errors, info))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + info.Id + ",Name:" + info.UserName, "成功", "修改", "用户设置");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + info.Id + ",Name:" + info.UserName + "," + ErrorCol, "失败", "修改", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":" + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult ReSet(string Id, string Pwd)
        {
            SysUserEditModel editModel = new SysUserEditModel();
            editModel.Id = Id;
            editModel.Password = ValueConvert.MD5(Pwd);
            if (m_BLL.EditPwd(ref errors, editModel))
            {
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id + ",密码:********", "成功", "初始化密码", "用户设置");
                return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id + ",,密码:********" + ErrorCol, "失败", "初始化密码", "用户设置");
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":" + ErrorCol), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {

            SysUserModel entity = m_BLL.GetById(id);
            //防止读取错误
            // string CityName, ProvinceName, VillageName, DepName, PosName;

            SysUserEditModel info = new SysUserEditModel()
            {
                Id = entity.Id,
                UserName = entity.UserName,
                TrueName = entity.TrueName,
                Card = entity.Card,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                QQ = entity.QQ,
                EmailAddress = entity.EmailAddress,
                OtherContact = entity.OtherContact,
                Province = entity.Province,
                City = entity.City,
                Village = entity.Village,
                Address = entity.Address,
                State = entity.State,
                CreateTime = entity.CreateTime,
                CreatePerson = entity.CreatePerson,
                Sex = entity.Sex,
                Birthday = ResultHelper.DateTimeConvertString(entity.Birthday),
                JoinDate = ResultHelper.DateTimeConvertString(entity.JoinDate),
                Marital = entity.Marital,
                Political = entity.Political,
                Nationality = entity.Nationality,
                Native = entity.Native,
                School = entity.School,
                Professional = entity.Professional,
                Degree = entity.Degree,
                DepId = entity.DepId,
                PosId = entity.PosId,
                Expertise = entity.Expertise,
                JobState = entity.JobState,
                Photo = entity.Photo,
                Attach = entity.Attach,
                RoleName = m_BLL.GetRefSysRole(id),
                CityName = entity.City,
                ProvinceName = entity.Province,
                VillageName = entity.Village,
                DepName = entity.DepName,
                PosName = entity.PosName
            };
            return View(info);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                //保护管理员不能被删除
                if (id == "admin")
                {
                    LogHandler.WriteServiceLog(GetUserId(), "尝试删除管理员", "失败", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, "管理员不能被删除！"), JsonRequestBehavior.AllowGet);
                }
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id + "," + ErrorCol, "失败", "删除", "用户设置");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        [HttpPost]
        public JsonResult GetTrueNames(string Ids)
        {
            string names = m_BLL.GetTrueNames(Ids);
            return Json(names, JsonRequestBehavior.AllowGet);
        }
    }
}
