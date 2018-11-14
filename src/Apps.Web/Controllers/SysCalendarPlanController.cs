using System.Collections.Generic;
using Apps.Web.Core;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models.Sys;
using Unity.Attributes;
using Apps.IBLL.Sys;
using System;
using System.Linq;
namespace Apps.Web.Controllers
{
    public class SysCalendarPlanController : BaseController
    {
        [Dependency]
        public ISysCalendarPlanBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        //[SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetList(DateTime start, DateTime end)
        {
            List<SysCalendarPlanModel> list = m_BLL.GetList(ref setNoPagerDescByCreateTime, start,end,GetUserId());
            var json  = (from r in list
                        select new CalendarPlanJsonModel()
                        {
                            id = r.Id,
                            title = r.Title,
                            content = r.PlanContent,
                            start = r.BeginDate,
                            end = r.EndDate,
                            url = r.Url,
                            color = r.Color,
                            textColor = r.TextColor
                        }).ToArray();
            return Json(json,JsonRequestBehavior.AllowGet);
        }
        #region 创建
        //[SupportFilter]
        public ActionResult Create(string start,string end)
        {
            DateTime beginDate = new DateTime();
            DateTime endDate = new DateTime();
            if (start == null)
            {
                beginDate = Convert.ToDateTime(ResultHelper.NowTime.ToShortDateString());
                endDate = Convert.ToDateTime(ResultHelper.NowTime.AddDays(1).ToShortDateString());
            }
            else
            {
                beginDate = Convert.ToDateTime(start);
                endDate = Convert.ToDateTime(end);
            }
            
            
            SysCalendarPlanModel model = new SysCalendarPlanModel()
            {
                BeginDate = beginDate,
                EndDate = endDate
            };
            return View(model);
        }

        [HttpPost]
        //[SupportFilter]
        public JsonResult Create(SysCalendarPlanModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            model.SysUserId = GetUserId();
            model.Editable = model.Editable == null ? "false" : model.Editable;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title, "成功", "创建", "SysCalendarPlan");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed,model.Id));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title + "," + ErrorCol, "失败", "创建", "SysCalendarPlan");
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
       //[SupportFilter]
        public ActionResult Edit(string id)
        {
            SysCalendarPlanModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        //[SupportFilter]
        public JsonResult Edit(SysCalendarPlanModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                SysCalendarPlanModel entity = m_BLL.GetById(model.Id);
                model.Editable = entity.Editable;
                model.SysUserId = entity.SysUserId;
                model.CreateTime = entity.CreateTime;
                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title, "成功", "修改", "SysCalendarPlan");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title + "," + ErrorCol, "失败", "修改", "SysCalendarPlan");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 详细
       // [SupportFilter]
        public ActionResult Details(string id)
        {
            SysCalendarPlanModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
       // [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "SysCalendarPlan");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "SysCalendarPlan");
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

