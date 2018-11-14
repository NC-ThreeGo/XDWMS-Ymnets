using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.Flow;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Flow;
using Unity.Attributes;

namespace Apps.Web.Areas.Flow.Controllers
{
    public class ExternalController : BaseController
    {
        [Dependency]
        public IFlow_ExternalBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<Flow_ExternalModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<Flow_ExternalModel> grs = new GridRows<Flow_ExternalModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        
        public JsonResult Create(Flow_ExternalModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title, "成功", "创建", "Flow_External");
                    return Json(JsonHandler.CreateMessage(1, model.Id));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title + "," + ErrorCol, "失败", "创建", "Flow_External");
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
        
        public ActionResult Edit(string id)
        {
            Flow_ExternalModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        
        public JsonResult Edit(Flow_ExternalModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title, "成功", "修改", "Flow_External");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Title" + model.Title + "," + ErrorCol, "失败", "修改", "Flow_External");
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
        
        public ActionResult Details(string id)
        {
            Flow_ExternalModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        
        public ActionResult Delete(string id)
        {
            if(!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Flow_External");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Flow_External");
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

