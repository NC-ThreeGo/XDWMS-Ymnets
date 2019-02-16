using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.WMS;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.WMS;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Apps.Web.Areas.WMS.Controllers
{
    public class ReInspectController : BaseController
    {
        [Dependency]
        public IWMS_ReInspectBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [Dependency]
        public IWMS_AIBLL m_AIBLL { get; set; }


        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string po, string inspectBillNum)
        {
           // List<WMS_ReInspectModel> list = m_BLL.GetList(ref pager, queryStr);
            List<WMS_ReInspectModel> list = m_BLL.GetListByWhere(ref pager, "WMS_AI.WMS_PO.PO.Contains(\"" + po + "\")&&WMS_AI.InspectBillNum.Contains(\"" + inspectBillNum + "\")");
            GridRows<WMS_ReInspectModel> grs = new GridRows<WMS_ReInspectModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create(int AIId)
        {
            ViewBag.EditStatus = true;
            WMS_ReInspectModel entity = new WMS_ReInspectModel();
            WMS_AIModel entity_AI = m_AIBLL.GetById(AIId);
            entity.OCheckOutDate = entity_AI.CheckOutDate;
            entity.OCheckOutRemark = entity_AI.CheckOutRemark;
            entity.OCheckOutResult = entity_AI.CheckOutResult;
            entity.ONoQualifyQty = entity_AI.NoQualifyQty;
            entity.OQualifyQty = entity_AI.QualifyQty;
            entity.InspectBillNum = entity_AI.InspectBillNum;
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_ReInspectModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = GetUserTrueName();           
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",AIId" + model.AIId, "成功", "创建", "WMS_ReInspect");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",AIId" + model.AIId + "," + ErrorCol, "失败", "创建", "WMS_ReInspect");
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
        public ActionResult Edit(long id)
        {
            WMS_ReInspectModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_ReInspectModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",AIId" + model.AIId, "成功", "修改", "WMS_ReInspect");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",AIId" + model.AIId + "," + ErrorCol, "失败", "修改", "WMS_ReInspect");
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
        [SupportFilter]
        public ActionResult Details(long id)
        {
            WMS_ReInspectModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(long id)
        {
            if(id!=0)
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_ReInspect");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_ReInspect");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }
        }
        #endregion

        #region 导出导入
        [HttpPost]
        [SupportFilter]
        public ActionResult Import(string filePath)
        {
            if (m_BLL.ImportExcelData(GetUserTrueName(), Utils.GetMapPath(filePath), ref errors))
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_ReInspect", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_ReInspect", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_ReInspectModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            if (list.Count().Equals(0))
            {
                return Json(JsonHandler.CreateMessage(0, "没有可以导出的数据"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, "可以导出"));
            }
        }
        [SupportFilter]
        public ActionResult Export(string po, string inspectBillNum)
        {
            //List<WMS_ReInspectModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            List<WMS_ReInspectModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "WMS_AI.WMS_PO.PO.Contains(\"" + po + "\")&&WMS_AI.InspectBillNum.Contains(\"" + inspectBillNum + "\")");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("采购单号", item.PO);
                    jo.Add("原送检单号", item.InspectBillNum);
                    jo.Add("原送检单结果", item.OCheckOutResult);
                    jo.Add("原送检单合格数", item.OQualifyQty);
                    jo.Add("原送检单不合格数", item.ONoQualifyQty);
                    jo.Add("原送检单说明", item.OCheckOutRemark);
                    jo.Add("原送检单检验日期", item.OCheckOutDate);
                    jo.Add("新送检单结果", item.NCheckOutResult);
                    jo.Add("新送检单合格数", item.NQualifyQty);
                    jo.Add("新送检单不合格数", item.NNoQualifyQty);
                    jo.Add("新送检单说明", item.NCheckOutRemark);
                    jo.Add("新送检单检验日期", item.NCheckOutDate);
                    jo.Add("调整说明", item.Remark);
                    jo.Add("调整人", item.AdjustMan);
                    jo.Add("调整时间", item.AdjustDate);
                    //jo.Add("Attr1", item.Attr1);
                    //jo.Add("Attr2", item.Attr2);
                    //jo.Add("Attr3", item.Attr3);
                    //jo.Add("Attr4", item.Attr4);
                    //jo.Add("Attr5", item.Attr5);
                    //jo.Add("创建时间", item.CreatePerson);
                    //jo.Add("创建人", item.CreateTime);
                    //jo.Add("修改人", item.ModifyPerson);
                    //jo.Add("修改人", item.ModifyTime);
                    jObjects.Add(jo);
                }
                var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
                var exportFileName = string.Concat(
                    RouteData.Values["controller"].ToString() + "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx");
                return new ExportExcelResult
                {
                    SheetName = "Sheet1",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
        [SupportFilter(ActionName = "Export")]
        public ActionResult ExportTemplate()
        {
            JArray jObjects = new JArray();
            var jo = new JObject();
              jo.Add("重新送检单ID", "");
              jo.Add("到货送检单ID", "");
              jo.Add("原送检单结果", "");
              jo.Add("原送检单合格数量", "");
              jo.Add("原送检单不合格数量", "");
              jo.Add("原送检单说明", "");
              jo.Add("原送检单检验日期", "");
              jo.Add("新送检单结果", "");
              jo.Add("新送检单合格数量", "");
              jo.Add("新送检单不合格数量", "");
              jo.Add("新送检单检验结果", "");
              jo.Add("新送检单检验日期", "");
              jo.Add("调整说明", "");
              jo.Add("调整人", "");
              jo.Add("调整时间", "");
              jo.Add("Attr1", "");
              jo.Add("Attr2", "");
              jo.Add("Attr3", "");
              jo.Add("Attr4", "");
              jo.Add("Attr5", "");
              jo.Add("创建时间", "");
              jo.Add("创建人", "");
              jo.Add("修改人", "");
              jo.Add("修改人", "");
            jo.Add("导入的错误信息", "");
            jObjects.Add(jo);
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            var exportFileName = string.Concat(
                    RouteData.Values["controller"].ToString() + "_Template",
                    ".xlsx");
                return new ExportExcelResult
                {
                    SheetName = "Sheet1",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
        #endregion
    }
}

