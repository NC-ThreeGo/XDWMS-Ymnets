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
    public class WareDetailsController : BaseController
    {
        [Dependency]
        public ISpl_WareDetailsBLL m_BLL { get; set; }
        [Dependency]
        public ISpl_WareCategoryBLL WareCategoryBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.GetWareCateogryTree = commonHelper.GetWareCateogryTree(true);
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr, string category)
        {
            List<Spl_WareDetailsModel> list = m_BLL.GetList(ref pager, queryStr, category);
            GridRows<Spl_WareDetailsModel> grs = new GridRows<Spl_WareDetailsModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
ViewBag.WareCategory = new SelectList(WareCategoryBLL.GetList(ref setNoPagerAscById, ""), "Id", "Name");
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(Spl_WareDetailsModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "Spl_WareDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "Spl_WareDetails");
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
            Spl_WareDetailsModel entity = m_BLL.GetById(id);
            ViewBag.WareCategory = new SelectList(WareCategoryBLL.GetList(ref setNoPagerAscById, ""), "Id", "Name", entity.WareCategoryId);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Spl_WareDetailsModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "Spl_WareDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "Spl_WareDetails");
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
            Spl_WareDetailsModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_WareDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_WareDetails");
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
        public ActionResult Import(string filePath, string categoryId)
        {
            var list = new List<Spl_WareDetailsModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors, categoryId);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "Spl_WareDetails");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "Spl_WareDetails");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<Spl_WareDetailsModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
            List<Spl_WareDetailsModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("Name", item.Name);
                jo.Add("Code", item.Code);
                jo.Add("BarCode", item.BarCode);
                jo.Add("WareCategoryId", item.WareCategoryId);
                jo.Add("Unit", item.Unit);
                jo.Add("Lable", item.Lable);
                jo.Add("BuyPrice", item.BuyPrice);
                jo.Add("SalePrice", item.SalePrice);
                jo.Add("RetailPrice", item.RetailPrice);
                jo.Add("Remark", item.Remark);
                jo.Add("Vender", item.Vender);
                jo.Add("Brand", item.Brand);
                jo.Add("Color", item.Color);
                jo.Add("Material", item.Material);
                jo.Add("Size", item.Size);
                jo.Add("Weight", item.Weight);
                jo.Add("ComeFrom", item.ComeFrom);
                jo.Add("UpperLimit", item.UpperLimit);
                jo.Add("LowerLimit", item.LowerLimit);
                jo.Add("PrimeCost", item.PrimeCost);
                jo.Add("Price1", item.Price1);
                jo.Add("Price2", item.Price2);
                jo.Add("Price3", item.Price3);
                jo.Add("Price4", item.Price4);
                jo.Add("Price5", item.Price5);
                jo.Add("Photo1", item.Photo1);
                jo.Add("Photo2", item.Photo2);
                jo.Add("Photo3", item.Photo3);
                jo.Add("Photo4", item.Photo4);
                jo.Add("Photo5", item.Photo5);
                jo.Add("Enable", item.Enable);
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
    }
}

