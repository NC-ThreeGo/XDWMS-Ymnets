using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Sys;
using Unity.Attributes;
using Apps.IBLL.Flow;
using Apps.Models.Flow;
using Apps.Web.Core;
using Apps.Locale;

namespace Apps.Web.Areas.Flow.Controllers
{
    public class FormAttrController : BaseController
    {
        [Dependency]
        public IFlow_FormAttrBLL m_BLL { get; set; }

        [Dependency]
        public IFlow_TypeBLL typeBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {

            return View();
        }



        [HttpPost]
        [SupportFilter(ActionName ="Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<Flow_FormAttrModel> list = m_BLL.GetList(ref pager, queryStr);

            GridRows<Flow_FormAttrModel> grs = new GridRows<Flow_FormAttrModel>();
            grs.rows = permModel.SetDataTransparent(list, Request.FilePath);//启用数据过滤
            grs.total = pager.totalRows;

            return Json(grs);

       
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {

            ViewBag.FlowType = new SelectList(typeBLL.GetList(ref setNoPagerAscBySort, ""), "Id", "Name");
            return View();
        }

        [HttpPost]
        [SupportFilter]
        [ValidateInput(false)]
        public JsonResult Create(Flow_FormAttrModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Tile" + model.Name, "成功", "创建", "Flow_FormAttr");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Tile" + model.Name + "," + ErrorCol, "失败", "创建", "Flow_FormAttr");
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

            Flow_FormAttrModel model = m_BLL.GetById(id);
            ViewBag.FlowType = new SelectList(typeBLL.GetList(ref setNoPagerAscBySort, ""), "Id", "Name", model.Id);
            return View(model);
        }

        [HttpPost]
        [SupportFilter]
        [ValidateInput(false)]
        public JsonResult Edit(Flow_FormAttrModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Tile" + model.Name, "成功", "修改", "Flow_FormAttr");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Tile" + model.Name + "," + ErrorCol, "失败", "修改", "Flow_FormAttr");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":" + ErrorCol));
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

            Flow_FormAttrModel entity = m_BLL.GetById(id);
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
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Flow_FormAttr");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Flow_FormAttr");
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
