using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Sys;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysImportExcelLogController : BaseController
    {
        [Dependency]
        public ISysImportExcelLogBLL m_BLL { get; set; }
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
            List<SysImportExcelLogModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<SysImportExcelLogModel> grs = new GridRows<SysImportExcelLogModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
    }
}

