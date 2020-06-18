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
using System.Dynamic;

namespace Apps.Web.Areas.WMS.Controllers
{
    public class ReportController : BaseController
    {
        [Dependency]
        public IWMS_ReportBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        //[SupportFilter]
        public ActionResult FeedList()
        {
            return View();
        }
        public ActionResult InvAmount()
        {
            return View();
        }
        public ActionResult SupplierDelivery()
        {
            List<ReportType> Types = new List<ReportType>();
            //Types.Add(new ReportType() { Type = 0, Name = "" });
            Types.Add(new ReportType() { Type = 1, Name = "已到货" });
            Types.Add(new ReportType() { Type = 2, Name = "全部" });
            ViewBag.DeliveryType = new SelectList(Types, "Name", "Name");
            return View();
        }

        public ActionResult ReturnRate()
        {
            List<ReportType> Types = new List<ReportType>();
            //Types.Add(new ReportType() { Type = 0, Name = "" });
            Types.Add(new ReportType() { Type = 1, Name = "自制件" });
            Types.Add(new ReportType() { Type = 2, Name = "其他" });
            ViewBag.ReturnRateType = new SelectList(Types, "Name", "Name");
            return View();
        }


        //[SupportFilter(ActionName = "FeedList")]
        public JsonResult GetFeedList(GridPager pager)
        {
            List<WMS_Feed_ListModel> list = m_BLL.GetFeedList(ref pager);
            GridRows<WMS_Feed_ListModel> grs = new GridRows<WMS_Feed_ListModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvAmount(GridPager pager, string partcode, string partname)
        {
            if (!String.IsNullOrEmpty(partcode))
            {
                partcode = partcode.Trim();
            }
            if (!String.IsNullOrEmpty(partname))
            {
                partname = partname.Trim();
            }
            List<WMS_InvModel> list = m_BLL.InvAmount(ref pager,partcode,partname);
            GridRows<WMS_InvModel> grs = new GridRows<WMS_InvModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportInvAmount(GridPager pager, string partCode, string partName)
        {
            List<WMS_InvModel> list = m_BLL.InvAmount(ref setNoPagerAscById, partCode, partName);//m_BLL.GetListByWhere(ref setNoPagerAscById, query);

            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("库房名称", item.InvName);
                jo.Add("物料编码", item.PartCode);
                jo.Add("物料名称", item.PartName);
                jo.Add("安全库存", item.SafeStock);
                jo.Add("现有量", item.Qty);
                jo.Add("预扣减数", item.PreDeductionQty);
                jo.Add("可用库存", item.AvailableQty);
                //jo.Add("备料数", item.StockQty);
                //jo.Add("批次", item.Lot);
                //jo.Add("出入库类型", item.Type);
                //jo.Add("操作人", item.OperateMan);
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

        public JsonResult GetSupplierDelivery(GridPager pager, string po,string suppliername,string partcode, string partname, DateTime beginDate, DateTime endDate, string deliveryType)
        {
            if (!String.IsNullOrEmpty(po))
            {
                po = po.Trim();
            }
            if (!String.IsNullOrEmpty(suppliername))
            {
                suppliername = suppliername.Trim();
            }
            if (!String.IsNullOrEmpty(partcode))
            {
                partcode = partcode.Trim();
            }
            if (!String.IsNullOrEmpty(partname))
            {
                partname = partname.Trim();
            }
            List<WMS_AIModel> list = m_BLL.SupplierDelivery(ref pager,po, suppliername, partcode, partname, beginDate, endDate, deliveryType);
            GridRows<WMS_AIModel> grs = new GridRows<WMS_AIModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportSupplierDelivery(GridPager pager, string po, string suppliername, string partcode, string partname, DateTime beginDate, DateTime endDate, string deliveryType)
        {
            List<WMS_AIModel> list = m_BLL.SupplierDelivery(ref setNoPagerAscById, po, suppliername, partcode, partname, beginDate, endDate, deliveryType);//m_BLL.GetListByWhere(ref setNoPagerAscById, query);
            
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("采购单号", item.PO);
                jo.Add("订单状态", item.POStatus);
                jo.Add("计划到货日期", item.PlanDate);
                jo.Add("到货日期", item.ArrivalDate);
                jo.Add("供应商", item.SupplierName);
                jo.Add("物料名称", item.PartName);
                jo.Add("物料编码", item.PartCode);
                jo.Add("采购数量", item.QTY);
                jo.Add("收货数量", item.ArrivalQty);
                jo.Add("合计收货数量", item.ArrivalQtySum);
                jo.Add("检验单号", item.InspectBillNum);
                jo.Add("合格入库日期", item.CheckOutDate);
                jo.Add("合格数", item.QualifyQty);
                jo.Add("不合格数", item.NoQualifyQty);
                jo.Add("检验结果", item.CheckOutResult);               
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

        public JsonResult GetReturnRate(GridPager pager, string partcode, string partname, DateTime beginDate, DateTime endDate, string returnRateType)
        {
            if (!String.IsNullOrEmpty(partcode))
            {
                partcode = partcode.Trim();
            }
            if (!String.IsNullOrEmpty(partname))
            {
                partname = partname.Trim();
            }
            List<WMS_Product_EntryModel> list = m_BLL.ReturnRate(ref pager, partcode, partname, beginDate, endDate, returnRateType);
            GridRows<WMS_Product_EntryModel> grs = new GridRows<WMS_Product_EntryModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportReturnRate(GridPager pager, string partcode, string partname, DateTime beginDate, DateTime endDate, string returnRateType)
        {
            List<WMS_Product_EntryModel> list = m_BLL.ReturnRate(ref setNoPagerAscById, partcode, partname, beginDate, endDate, returnRateType);//m_BLL.GetListByWhere(ref setNoPagerAscById, query);

            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("物料编码", item.PartCode);
                jo.Add("物料名称", item.PartName);
                jo.Add("退货率", item.ReturnRate);
                
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

    }
}

