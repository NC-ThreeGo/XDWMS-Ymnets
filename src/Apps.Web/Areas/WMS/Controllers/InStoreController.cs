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
using Apps.BLL.Sys;

namespace Apps.Web.Areas.WMS.Controllers
{
    public class InStoreController : BaseController
    {
        [Dependency]
        public IWMS_AIBLL m_BLL { get; set; }
        [Dependency]
        public IWMS_InvInfoBLL m_InvInfoBll { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        /// <summary>
        /// 检验入库的Index
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 手工入库的Index
        /// </summary>
        /// <returns></returns>
        [SupportFilter(ActionName = "Index")]
        public ActionResult Index1()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string inspectBillNum, string po, string supplierShortName, string partCode, DateTime beginDate, DateTime endDate)
        {
            //TODO：显示已入库的送检单
            //List<WMS_AIModel> list = m_BLL.GetListByWhere(ref pager, "InStoreStatus == \"已入库\"");
            //GridRows<WMS_AIModel> grs = new GridRows<WMS_AIModel>();
            List<WMS_AIModel> list = m_BLL.GetListByWhere(ref pager, "WMS_PO.PO.Contains(\"" + po + "\")&&InspectBillNum.Contains(\"" + inspectBillNum + "\") && WMS_PO.WMS_Supplier.SupplierShortName.Contains(\""
              + supplierShortName + "\")&& WMS_PO.WMS_Part.PartCode.Contains(\"" + partCode + "\")&& InStoreStatus=\"已入库\"&& CheckOutDate>=(\""
              + beginDate + "\")&& InspectDate<=(\"" + endDate + "\")");
            GridRows<WMS_AIModel> grs = new GridRows<WMS_AIModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            //ViewBag.InStoreBillNum = "RK" + DateTime.Now.ToString("yyyyMMddHHmmssff");
            
            //ViewBag.Inv = new SelectList(m_InvInfoBll.GetListByWhere("Status == \"有效\""), "InvCode", "InvName");

            return View();
        }

        [HttpPost]
        [SupportFilter]
        [ValidateInput(false)]
        public JsonResult Create(string inspectBillNum, string inserted)
        {
            if (m_BLL.ProcessInspectBill(ref errors, GetUserTrueName(), inserted))
            {
                LogHandler.WriteServiceLog(GetUserTrueName(), "检验单：" + inspectBillNum + "处理", "成功", "处理", "WMS_AI");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserTrueName(), "检验单：" + inspectBillNum + "处理" + "," + ErrorCol, "失败", "处理", "WMS_AI");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }

        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(long id)
        {
            WMS_AIModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_AIModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ArrivalBillNum" + model.ArrivalBillNum, "成功", "修改", "WMS_AI");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ArrivalBillNum" + model.ArrivalBillNum + "," + ErrorCol, "失败", "修改", "WMS_AI");
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
            WMS_AIModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_AI");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_AI");
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
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_AI", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_AI", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_AIModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
        public ActionResult Export(string inspectBillNum, string po, string supplierShortName, string partCode, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_AIModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            List<WMS_AIModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "WMS_PO.PO.Contains(\"" + po + "\")&&InspectBillNum.Contains(\"" + inspectBillNum + "\") && WMS_PO.WMS_Supplier.SupplierShortName.Contains(\""
               + supplierShortName + "\")&& WMS_PO.WMS_Part.PartCode.Contains(\"" + partCode + "\")&& InStoreStatus=\"已入库\"&& InspectDate>=(\""
               + beginDate + "\")&& InspectDate<=(\"" + endDate + "\")");            
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                //jo.Add("Id", item.Id);
                jo.Add("送检单号", item.InspectBillNum);
                //jo.Add("到货单据号", item.ArrivalBillNum);
                jo.Add("采购订单号", item.PO);
                jo.Add("物料编码", item.PartCode);
                jo.Add("物料名称", item.PartName);
                jo.Add("供应商简称", item.SupplierShortName);
                jo.Add("到货数量", item.ArrivalQty);
                jo.Add("到货箱数", item.BoxQty);
                //jo.Add("送检人", item.InspectMan);
                jo.Add("送检日期", item.InspectDate);
                jo.Add("检验日期", item.CheckOutDate);                
                jo.Add("合格数量", item.QualifyQty);
                jo.Add("不合格数量", item.NoQualifyQty);
                jo.Add("检验结果", item.CheckOutResult);
                jo.Add("检验说明", item.CheckOutRemark);
                //jo.Add("送检状态", item.InspectStatus);
                jo.Add("入库仓库", item.InvName);
                jo.Add("入库状态", item.InStoreStatus);

                //jo.Add("到货日期", item.ArrivalDate);
                //jo.Add("接收人", item.ReceiveMan);
                //jo.Add("到货状态", item.ReceiveStatus);   
                
                //jo.Add("重新送检单", item.ReInspectBillNum);
                //jo.Add("入库单号", item.InStoreBillNum);
                //jo.Add("InStoreMan", item.InStoreMan);
                //jo.Add("入库仓库", item.InvId);
                //jo.Add("子库", item.SubInvId);                
                //jo.Add("Attr1", item.Attr1);
                //jo.Add("Attr2", item.Attr2);
                //jo.Add("Attr3", item.Attr3);
                //jo.Add("Attr4", item.Attr4);
                //jo.Add("Attr5", item.Attr5);
                //jo.Add("创建人", item.CreatePerson);
                //jo.Add("创建时间", item.CreateTime);
                //jo.Add("修改人", item.ModifyPerson);
                //jo.Add("修改时间", item.ModifyTime);
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
              jo.Add("到货单据号", "");
              jo.Add("采购订单ID", "");
              jo.Add("到货箱数", "");
              jo.Add("到货数量", "");
              jo.Add("到货日期", "");
              jo.Add("接收人", "");
              jo.Add("到货状态", "");
              jo.Add("送检单号", "");
              jo.Add("送检人", "");
              jo.Add("送检日期", "");
              jo.Add("送检状体", "");
              jo.Add("检验日期", "");
              jo.Add("检验结果", "");
              jo.Add("合格数量", "");
              jo.Add("不合格数量", "");
              jo.Add("检验说明", "");
              jo.Add("重新送检单", "");
              jo.Add("入库单号", "");
              jo.Add("InStoreMan", "");
              jo.Add("入库仓库", "");
              jo.Add("入库状态", "");
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

        #region 加载指定送检单的行信息
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetInspectBillList(GridPager pager, string inspectBillNum)
        {
            List<WMS_AIModel> list = m_BLL.GetListByWhere(ref pager, "InspectBillNum == \"" + inspectBillNum + "\" && InStoreStatus == \"" + "未入库" + "\"").ToList();
            GridRows<WMS_AIModel> grs = new GridRows<WMS_AIModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion

        #region 加载检验结果
        [HttpPost]
        public JsonResult GetCheckOutResult()
        {
            var list = SysParamBLL.GetSysParamByType("CheckOutResult");
            return Json(list);
        }
        #endregion
    }
}

