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
                jo.Add("备料数", item.StockQty);
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
    }
}

