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
    }
}

