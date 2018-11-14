using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.Spl;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Spl;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Apps.Web.Areas.Spl.Controllers
{
    public class WareStockPileController : BaseController
    {
        [Dependency]
        public ISpl_WareStockPileBLL m_BLL { get; set; }
        [Dependency]
        public ISpl_WarehouseBLL m_WarehouseBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string queryStr,string parentId)
        {
            List<Spl_WareStockPileModel> list = m_BLL.GetListByParentId(ref pager, queryStr,parentId,GetUserId());//修改于2018年3月14
            GridRows<Spl_WareStockPileModel> grs = new GridRows<Spl_WareStockPileModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
         ViewBag.Warehouse = new SelectList(m_WarehouseBLL.GetList(ref setNoPagerAscById, "", GetUserId()), "Id", "Name");
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(Spl_WareStockPileModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WarehouseId" + model.WarehouseId, "成功", "创建", "Spl_WareStockPile");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WarehouseId" + model.WarehouseId + "," + ErrorCol, "失败", "创建", "Spl_WareStockPile");
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
            Spl_WareStockPileModel entity = m_BLL.GetById(id);
            ViewBag.Warehouse = new SelectList(m_WarehouseBLL.GetList(ref setNoPagerAscById, ""), "Id", "Name",entity.WarehouseId);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Spl_WareStockPileModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WarehouseId" + model.WarehouseId, "成功", "修改", "Spl_WareStockPile");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WarehouseId" + model.WarehouseId + "," + ErrorCol, "失败", "修改", "Spl_WareStockPile");
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
        public ActionResult Details(string id)
        {
            Spl_WareStockPileModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(string id)
        {
            if(!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_WareStockPile");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_WareStockPile");
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
            var list = new List<Spl_WareStockPileModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "Spl_WareStockPile");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "Spl_WareStockPile");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<Spl_WareStockPileModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
        public ActionResult Export()
        {
            List<Spl_WareStockPileModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("Id", item.Id);
                    jo.Add("WarehouseId", item.WarehouseId);
                    jo.Add("WareDetailsId", item.WareDetailsId);
                    jo.Add("FirstEnterDate", item.FirstEnterDate);
                    jo.Add("LastLeaveDate", item.LastLeaveDate);
                    jo.Add("Quantity", item.Quantity);
                    jo.Add("Price", item.Price);
                    jo.Add("CreateTime", item.CreateTime);
                    jObjects.Add(jo);
                }
                var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
                var exportFileName = string.Concat(
                    "File",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx");
                return new ExportExcelResult
                {
                    SheetName = "Sheet1",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
        #endregion
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetListParent(GridPager pager, string queryStr)
        {
            List<Spl_WarehouseModel> list = m_WarehouseBLL.GetList(ref pager, queryStr,GetUserId());
            GridRows<Spl_WarehouseModel> grs = new GridRows<Spl_WarehouseModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region  查看
        [SupportFilter(ActionName = "Index")]
        public ActionResult Watch(string WareDetailsId,string WarehouseId)
        {
            ViewBag.WareDetailsId = WareDetailsId;
            ViewBag.WarehouseId = WarehouseId;
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetWatchList(string WareDetailsId, string WarehouseId)
        {
            //Spl_WareStockPileModel entity = m_BLL.GetById(id);
            //ViewBag.Warehouse = new SelectList(m_WarehouseBLL.GetList(ref setNoPagerAscById, ""), "Id", "Name", entity.WarehouseId);
            //return View(entity);
            List<Spl_WreStockPileListModel> list = m_BLL.GetPileList(WareDetailsId, WarehouseId);
            GridRows<Spl_WreStockPileListModel> grs = new GridRows<Spl_WreStockPileListModel>();
            grs.rows = list;
            grs.total = list.Count;
            return Json(grs);
        }
        #endregion
        #region  设置预警货品

        #endregion
        #region 查看预警货品
        //[SupportFilter(ActionName = "Index")]
        //public ActionResult Watchlist(string WarehouseId, int Quantity, int WaringQuantity)
        //{
        //    ViewBag.Quantity = Quantity;
        //    ViewBag.WarehouseId = WarehouseId;
        //    ViewBag.WaringQuantity = WaringQuantity;
        //    return View();
        //}
        //[HttpPost]
        //[SupportFilter(ActionName = "Index")]
        //public JsonResult GetListview(GridPager pager, string queryStr, string WarehouseId, int Quantity, int WaringQuantity)
        //{
        //    List<Spl_WareStockPileModel> list = m_BLL.GetListview(WarehouseId, Quantity, WaringQuantity);
        //    GridRows<Spl_WareStockPileModel> grs = new GridRows<Spl_WareStockPileModel>();
        //    grs.rows = list;
        //    grs.total = pager.totalRows;
        //    return Json(grs);
        //}

        #endregion
    }
}

