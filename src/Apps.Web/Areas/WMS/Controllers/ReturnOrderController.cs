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
    public class ReturnOrderController : BaseController
    {
        [Dependency]
        public IWMS_ReturnOrderBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<WMS_ReturnOrderModel> list = m_BLL.GetListByWhere(ref pager, "ReturnOrderNum != null");
            GridRows<WMS_ReturnOrderModel> grs = new GridRows<WMS_ReturnOrderModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 手工创建
        [SupportFilter]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_ReturnOrderModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum, "成功", "创建", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum + "," + ErrorCol, "失败", "创建", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 批量确认
        [SupportFilter(ActionName = "Create")]
        public ActionResult CreateBatch()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        [ValidateInput(false)]
        public JsonResult CreateBatch(string inserted)
        {
            try
            {
                var returnOrderNum = m_BLL.CreateBatchReturnOrder(GetUserId(), inserted);
                LogHandler.WriteServiceLog(GetUserId(), "保存退货单成功", "成功", "保存", "WMS_ReturnOrder");

                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, returnOrderNum));
            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserId(), ex.Message, "失败", "保存", "WMS_ReturnOrder");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(long id)
        {
            WMS_ReturnOrderModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_ReturnOrderModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum, "成功", "修改", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum + "," + ErrorCol, "失败", "修改", "WMS_ReturnOrder");
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
            WMS_ReturnOrderModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_ReturnOrder");
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
            if (m_BLL.ImportExcelData(GetUserId(), Utils.GetMapPath(filePath), ref errors))
            {
                 LogHandler.WriteImportExcelLog(GetUserId(), "WMS_ReturnOrder", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserId(), "WMS_ReturnOrder", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_ReturnOrderModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
        public ActionResult Export(string queryStr)
        {
            List<WMS_ReturnOrderModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("退货单ID", item.Id);
                    jo.Add("退货单号", item.ReturnOrderNum);
                    jo.Add("到货检验单ID", item.AIID);
                    jo.Add("物料编码", item.PartID);
                    jo.Add("代理商编码", item.SupplierId);
                    jo.Add("库存编码", item.InvId);
                    jo.Add("SubInvId", item.SubInvId);
                    jo.Add("应退货数量", item.ReturnQty);
                    jo.Add("实际退货数量", item.AdjustQty);
                    jo.Add("退货说明", item.Remark);
                    jo.Add("打印状态", item.PrintStaus);
                    jo.Add("打印时间", item.PrintDate);
                    jo.Add("打印人", item.PrintMan);
                    jo.Add("确认状态", item.ConfirmStatus);
                    jo.Add("确认人", item.ConfirmMan);
                    jo.Add("确认时间", item.ConfirmDate);
                    jo.Add("Attr1", item.Attr1);
                    jo.Add("Attr2", item.Attr2);
                    jo.Add("Attr3", item.Attr3);
                    jo.Add("Attr4", item.Attr4);
                    jo.Add("Attr5", item.Attr5);
                    jo.Add("创建人", item.CreatePerson);
                    jo.Add("创建时间", item.CreateTime);
                    jo.Add("修改人", item.ModifyPerson);
                    jo.Add("修改时间", item.ModifyTime);
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
              jo.Add("退货单ID", "");
              jo.Add("退货单号", "");
              jo.Add("到货检验单ID", "");
              jo.Add("物料编码", "");
              jo.Add("代理商编码", "");
              jo.Add("库存编码", "");
              jo.Add("SubInvId", "");
              jo.Add("应退货数量", "");
              jo.Add("实际退货数量", "");
              jo.Add("退货说明", "");
              jo.Add("打印状态", "");
              jo.Add("打印时间", "");
              jo.Add("打印人", "");
              jo.Add("确认状态", "");
              jo.Add("确认人", "");
              jo.Add("确认时间", "");
              jo.Add("Attr1", "");
              jo.Add("Attr2", "");
              jo.Add("Attr3", "");
              jo.Add("Attr4", "");
              jo.Add("Attr5", "");
              jo.Add("创建人", "");
              jo.Add("创建时间", "");
              jo.Add("修改人", "");
              jo.Add("修改时间", "");
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

        #region 加载指定供应商的退货信息
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetReturnOrderList(GridPager pager, string supplierId)
        {
            //TODO：指定供应商，且退货单号为空的退货行
            List<WMS_ReturnOrderModel> list = m_BLL.GetListByWhere(ref pager, "SupplierId == \"" + supplierId + "\" && ReturnOrderNum == null").ToList();
            GridRows<WMS_ReturnOrderModel> grs = new GridRows<WMS_ReturnOrderModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion
    }
}

