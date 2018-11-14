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
using Apps.Models;

namespace Apps.Web.Areas.Spl.Controllers
{
    public class WareCheckTotalController : BaseController
    {
        [Dependency]
        public ISpl_WareCheckTotalBLL m_BLL { get; set; }
        [Dependency]
        public ISpl_WarehouseBLL m_WarehouseBLL { get; set; }
        [Dependency]
        public ISpl_WareDetailsBLL wareDetailsBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        //[Dependency]
        //public ISpl_WarehouseBLL WarehouseBLL { get; set; }
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr, string parentId)
        {
            List<Spl_WareCheckTotalModel> list = m_BLL.GetListByParentId(ref pager, queryStr, parentId,GetUserId());
            GridRows<Spl_WareCheckTotalModel> grs = new GridRows<Spl_WareCheckTotalModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult GetQuantity(string warehouseid,string waredetailsid)
        {
            Spl_WareStockPileModel entity = m_BLL.GetQuantity(warehouseid, waredetailsid);
            return Json(entity);
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Warehouse = new SelectList(m_WarehouseBLL.GetList(ref setNoPagerAscById, "", GetUserId()), "Id", "Name");
            ViewBag.Checker = new SelectList(m_WarehouseBLL.GetList(ref setNoPagerAscById, "", GetUserId()), "Id", "ContactPerson");//x修改2018年3月13
            //AccountModel accountModel = GetAccount();
            Spl_WareCheckTotalModel model = new Spl_WareCheckTotalModel()
            {
                Id = "CKD" + DateTime.Now.ToString("yyyyMMddHHmmssff"),
                State = 0
                //Handler = accountModel.Id,
                //HandlerName = accountModel.TrueName,
                

            };
           
            return View(model);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(Spl_WareCheckTotalModel model)
        {
            model.CreateTime = ResultHelper.NowTime;
            model.State = 0;
            model.Creater = GetUserId();
            model.Checker= GetUserId();//修改2018年3月13
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WareDetailsId" + model.WareDetailsId, "成功", "创建", "Spl_WareCheckTotal");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WareDetailsId" + model.WareDetailsId + "," + ErrorCol, "失败", "创建", "Spl_WareCheckTotal");
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
            Spl_WareCheckTotalModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Spl_WareCheckTotalModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WareDetailsId" + model.WareDetailsId, "成功", "修改", "Spl_WareCheckTotal");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",WareDetailsId" + model.WareDetailsId + "," + ErrorCol, "失败", "修改", "Spl_WareCheckTotal");
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
            Spl_WareCheckTotalModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_WareCheckTotal");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_WareCheckTotal");
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
            var list = new List<Spl_WareCheckTotalModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "Spl_WareCheckTotal");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "Spl_WareCheckTotal");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<Spl_WareCheckTotalModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
            List<Spl_WareCheckTotalModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("WareDetailsId", item.WareDetailsId);
                jo.Add("WarehouseId", item.WarehouseId);
                jo.Add("Remark", item.Remark);
                jo.Add("DiffQuantity", item.DiffQuantity);
                jo.Add("Quantity", item.Quantity);
                jo.Add("Price", item.Price);
                jo.Add("State", item.State);
                jo.Add("Creater", item.Creater);
                jo.Add("Checker", item.Checker);
                jo.Add("CheckTime", item.CheckTime);
                jo.Add("Confirmation", item.Confirmation);
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
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetListParent(GridPager pager, string queryStr)
        {
            List<Spl_WarehouseModel> list = m_WarehouseBLL.GetList(ref pager, queryStr,GetUserId());
            GridRows<Spl_WarehouseModel> grs = new GridRows<Spl_WarehouseModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }



        [SupportFilter(ActionName = "Create")]
        public ActionResult WareDetails()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.GetWareCateogryTree = commonHelper.GetWareCateogryTree(true);
          

            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult WareDetailsGetList(GridPager pager, string queryStr, string category)
        {
            List<Spl_WareDetailsModel> list = wareDetailsBLL.GetList(ref pager, queryStr, category);
            GridRows<Spl_WareDetailsModel> grs = new GridRows<Spl_WareDetailsModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }


        #region  审查
        [HttpPost]
        [SupportFilter]
        public JsonResult Check(string Id)
        {

            if (!string.IsNullOrWhiteSpace(Id))
            {

                if (m_BLL.Check(ref errors, Id, 1, GetUserId()))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id, "成功", "审核", "信息中心");
                    return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id + "," + ErrorCol, "失败", "审核", "信息中心");
                    return Json(JsonHandler.CreateMessage(0, Resource.CheckFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.CheckFail));
            }
        }

        #endregion
    }
}
