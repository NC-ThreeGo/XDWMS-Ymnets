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
    public class ReturnOrderDController : BaseController
    {
        [Dependency]
        public IWMS_ReturnOrder_DBLL m_BLL { get; set; }
        [Dependency]
        public IWMS_ReturnOrderBLL m_ReturnOrderBLL { get; set; }

        [Dependency]
        public IWMS_SupplierBLL m_SupplierBLL { get; set; }

        [Dependency]
        public IWMS_InvInfoBLL _InvInfoBll { get; set; }

        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            //定义送货状态下拉框的值
            List<ReportType> ROTypes = new List<ReportType>();
            //ROTypes.Add(new ReportType() { Type = 0, Name = "" });
            ROTypes.Add(new ReportType() { Type = 2, Name = "已退货" });
            ROTypes.Add(new ReportType() { Type = 1, Name = "未退货" });
            ROTypes.Add(new ReportType() { Type = 3, Name = "已失效" });
            ViewBag.ReturnOrderStatus = new SelectList(ROTypes, "Name", "Name");

            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetListParent(GridPager pager, string inspectBillNum,
                string supplierShortName, string returnOrderNum, string partCode,
                DateTime beginDate, DateTime endDate, string returnOrderStatus)
        {
            string query = "";
            if (returnOrderStatus == "未退货")
            {
                query += " 1=1 ";
                if (!String.IsNullOrEmpty(inspectBillNum))
                    query += " && WMS_AI.InspectBillNum.Contains(\"" + inspectBillNum + "\")";
                query += " && WMS_Supplier.SupplierShortName.Contains(\"" + supplierShortName + "\")";
                query += " && WMS_Part.PartCode.Contains(\"" + partCode + "\")";
            }
            if (returnOrderStatus == "已失效")
            {
                query += " 1=1 ";
                if (!String.IsNullOrEmpty(returnOrderNum))
                    query += "&&ReturnOrderNum.Contains(\"" + returnOrderNum + "\")";
                if (!String.IsNullOrEmpty(inspectBillNum))
                    query += " && WMS_AI.InspectBillNum.Contains(\"" + inspectBillNum + "\")";
                query += " && WMS_Supplier.SupplierShortName.Contains(\"" + supplierShortName + "\")";
                query += " && WMS_Part.PartCode.Contains(\"" + partCode + "\")";
            }

            if (returnOrderStatus == "已退货")
            {
                query = "ReturnOrderNum.Contains(\"" + returnOrderNum + "\")";
                if (!String.IsNullOrEmpty(inspectBillNum))
                    query += " && WMS_AI.InspectBillNum.Contains(\"" + inspectBillNum + "\")";
                query += " && WMS_Supplier.SupplierShortName.Contains(\"" + supplierShortName + "\")";
                query += " && WMS_Part.PartCode.Contains(\"" + partCode + "\")";
                query += " && PrintDate>=(\"" + beginDate + "\")&& PrintDate<=(\"" + endDate + "\")";
            }
            //query += " && PrintStaus.Contains(\"" + returnOrderStatus + "\")";
            List<WMS_ReturnOrderModel> list = m_ReturnOrderBLL.GetListByWhere(ref pager, query);
            GridRows<WMS_ReturnOrderModel> grs = new GridRows<WMS_ReturnOrderModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 手工创建库存待退货单
        [SupportFilter(ActionName = "Create")]
        public ActionResult CreateParent()
        {
            ViewBag.Inv = new SelectList(_InvInfoBll.GetList(ref setNoPagerAscById, ""), "Id", "InvName");
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult CreateParent(WMS_ReturnOrderModel model)
        {
            model.Id = 0;
            //model.PrintStaus = "未退货";
            model.CreateTime = ResultHelper.NowTime;
            if (model.Lot == "[空]")
                model.Lot = "";
            if (model != null && ModelState.IsValid)
            {
                if (m_ReturnOrderBLL.CreateReturnOrder(ref errors, GetUserTrueName(), model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum, "成功", "创建", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum + "," + ErrorCol, "失败", "创建", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 修改库存待退货单
        [SupportFilter(ActionName = "Edit")]
        public ActionResult EditParent(long id)
        {
            WMS_ReturnOrderModel entity = m_ReturnOrderBLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult EditParent(WMS_ReturnOrderModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_ReturnOrderBLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum, "成功", "修改", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ReturnOrderNum" + model.ReturnOrderNum + "," + ErrorCol, "失败", "修改", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 删除待退货单
        [HttpPost]
        [SupportFilter]
        public ActionResult DeleteParent(int id)
        {
            if (id != 0)
            {
                if (m_ReturnOrderBLL.CancelReturnOrder(ref errors, GetUserTrueName(), id))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }
        }
        #endregion

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr, string parentId)
        {
            List<WMS_ReturnOrder_DModel> list = m_BLL.GetListByParentId(ref pager, queryStr, parentId);
            GridRows<WMS_ReturnOrder_DModel> grs = new GridRows<WMS_ReturnOrder_DModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 打印
        [SupportFilter(ActionName = "Create")]
        public ActionResult Print()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        [ValidateInput(false)]
        public JsonResult Print(string inserted)
        {
            try
            {
                var returnOrderNum = m_BLL.PrintReturnOrder(ref errors, GetUserTrueName(), inserted);
                if (!String.IsNullOrEmpty(returnOrderNum))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "打印退货单成功", "成功", "打印", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, returnOrderNum));
                    //return Redirect("~/Report/ReportManager/ShowBill?reportCode=ReturnOrder&billNum=" + returnOrderNum);
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), errors.Error, "失败", "打印", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + errors.Error));
                }

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserTrueName(), ex.Message, "失败", "打印", "WMS_ReturnOrder");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
            }
        }
        #endregion

        #region 确认
        [SupportFilter(ActionName = "Edit")]
        public ActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        [ValidateInput(false)]
        public JsonResult Confirm(string returnOrderNum)
        {
            try
            {
                if (m_BLL.ConfirmReturnOrder(ref errors, GetUserTrueName(), returnOrderNum))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "ReturnOrderNum" + returnOrderNum, "成功", "确认", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, returnOrderNum));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "ReturnOrderNum" + returnOrderNum + ", " + errors.Error, "失败", "确认", "WMS_ReturnOrder");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + errors.Error));
                }

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserTrueName(), "ReturnOrderNum" + returnOrderNum + ", " + ex.Message, "失败", "确认", "WMS_ReturnOrder");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
            }
        }
        #endregion

        #region 导出导入
        [HttpPost]
        [SupportFilter]
        public ActionResult ImportParent(string filePath)
        {
            if (m_ReturnOrderBLL.ImportExcelData(GetUserId(), Utils.GetMapPath(filePath), ref errors))
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
            List<WMS_ReturnOrderModel> list = m_ReturnOrderBLL.GetList(ref setNoPagerAscById, queryStr);
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
            List<WMS_ReturnOrder_DModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("Id", item.Id);
                    jo.Add("退货单号", item.ReturnOrderDNum);
                    jo.Add("头表ID", item.HeadId);
                    jo.Add("退货数量", item.ReturnQty);
                    jo.Add("备注", item.Remark);
                    jo.Add("打印状态", item.PrintStaus);
                    jo.Add("打印日期", item.PrintDate);
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
                    jo.Add("BatchId", item.BatchId);
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
              jo.Add("Id", "");
              jo.Add("退货单号", "");
              jo.Add("头表ID", "");
              jo.Add("退货数量", "");
              jo.Add("备注", "");
              jo.Add("打印状态", "");
              jo.Add("打印日期", "");
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
              jo.Add("BatchId", "");
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

        #region 弹出当前有退货记录的供应商，以供选择
        [SupportFilter(ActionName = "Create")]
        public ActionResult SupplierLookUp(bool mulSelect = false)
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult SupplierGetList(GridPager pager, string supplierCode, string supplierShortName)
        {
            List<WMS_ReturnOrderModel> list = m_ReturnOrderBLL.GetListByWhere(ref pager, "1 = 1")
                .Where(p => Math.Abs(p.AdjustQty.Value) < Math.Abs(p.ReturnQty.Value))
                .GroupBy(p => new { p.SupplierId, p.SupplierShortName })
                .Select(g => g.First())
                .ToList();
            GridRows<WMS_ReturnOrderModel> grs = new GridRows<WMS_ReturnOrderModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion

        #region 加载指定供应商的退货信息
        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult GetReturnOrderListBySupplier(GridPager pager, string supplierId, string supplierShortName)
        {
            //TODO：指定供应商，且退货单号为空的退货行
            if (String.IsNullOrEmpty(supplierId) && String.IsNullOrEmpty(supplierShortName))
                supplierId = "0";
            //supplierId为空、supplierShortName不为空，则通过supplierShortName获取supplierId
            if (String.IsNullOrEmpty(supplierId) && !String.IsNullOrEmpty(supplierShortName))
            {
                var supplier = m_SupplierBLL.GetListByWhere(ref setNoPagerAscById, "SupplierShortName == \"" + supplierShortName + "\"").First();
                supplierId = supplier.Id.ToString();
            }
            //List<WMS_ReturnOrderModel> list = m_ReturnOrderBLL.GetListByWhere(ref pager, "SupplierId == \"" + supplierId + "\" && Abs(AdjustQty) < Abs(ReturnQty)").ToList();
            List<WMS_ReturnOrderModel> list = m_ReturnOrderBLL.GetListByWhere(ref pager, "1 = 1")
                .Where(p => p.SupplierId.ToString() == supplierId && Math.Abs(p.AdjustQty.Value) < Math.Abs(p.ReturnQty.Value))
                .ToList();
            GridRows<WMS_ReturnOrderModel> grs = new GridRows<WMS_ReturnOrderModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion

        #region 弹出已打印的退货单，以便选择
        [SupportFilter(ActionName = "Edit")]
        public ActionResult RetunOrderForPrintedLookUp(bool mulSelect = false)
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult RetunOrderForPrintedGetList(GridPager pager)
        {
            List<WMS_ReturnOrder_DModel> list = m_BLL.GetListByWhere(ref pager, "PrintStaus == \"已退货\" && ConfirmStatus == \"未确认\"")
                .GroupBy(p => new { p.ReturnOrderDNum })
                .Select(g => g.First())
                //.Select(p => new WMS_ReturnOrderModel { ReturnOrderNum = p.ReturnOrderNum, SupplierId = p.SupplierId, SupplierShortName = p.SupplierShortName })
                .ToList();
            GridRows<WMS_ReturnOrder_DModel> grs = new GridRows<WMS_ReturnOrder_DModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion

        #region 加载指定退货单号的退货信息
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult GetReturnOrderListByNum(GridPager pager, string returnOrderNum)
        {
            List<WMS_ReturnOrder_DModel> list = m_BLL.GetListByWhere(ref pager, "ReturnOrderDNum == \"" + returnOrderNum + "\"").ToList();
            GridRows<WMS_ReturnOrder_DModel> grs = new GridRows<WMS_ReturnOrder_DModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion
    }
}
