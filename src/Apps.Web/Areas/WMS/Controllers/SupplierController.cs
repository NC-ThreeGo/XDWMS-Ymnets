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
    public class SupplierController : BaseController
    {
        [Dependency]
        public IWMS_SupplierBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<WMS_SupplierModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<WMS_SupplierModel> grs = new GridRows<WMS_SupplierModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_SupplierModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = GetUserTrueName();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",SupplierCode" + model.SupplierCode, "成功", "创建", "WMS_Supplier");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    //string ErrorCol = errors.Error;
                    string ErrorCol = " ：输入错误数据";
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",SupplierCode" + model.SupplierCode + "," + ErrorCol, "失败", "创建", "WMS_Supplier");
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
        public ActionResult Edit(long id)
        {
            ViewBag.EditStatus = true;
            WMS_SupplierModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_SupplierModel model)
        {
            model.ModifyTime = ResultHelper.NowTime;
            model.ModifyPerson = GetUserTrueName();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",SupplierCode" + model.SupplierCode, "成功", "修改", "WMS_Supplier");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",SupplierCode" + model.SupplierCode + "," + ErrorCol, "失败", "修改", "WMS_Supplier");
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
            WMS_SupplierModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(long id)
        {
            if (id != 0)
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_Supplier");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_Supplier");
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
                LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Supplier", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                return Json(JsonHandler.CreateMessage(1,
                    Resource.InsertSucceed + "，记录数：" + Utils.GetRowCount(Utils.GetMapPath(filePath)).ToString(),
                    filePath));
            }
            else
            {
                LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Supplier", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_SupplierModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
            List<WMS_SupplierModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                //jo.Add("供应商ID", item.Id);
                jo.Add("供应商编码", item.SupplierCode);
                jo.Add("供应商简称", item.SupplierShortName);
                jo.Add("供应商名称", item.SupplierName);
                jo.Add("供应商类型", item.SupplierType);
                jo.Add("联系人", item.LinkMan);
                jo.Add("联系人电话", item.LinkManTel);
                jo.Add("联系人地址", item.LinkManAddress);
                //jo.Add("状态", item.Status);
                jo.Add("说明", item.Remark);
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
            //jo.Add("供应商ID", "");
            jo.Add("供应商编码(必输)", "");
            jo.Add("供应商简称(必输)", "");
            jo.Add("供应商名称(必输)", "");
            jo.Add("供应商类型", "");
            jo.Add("联系人", "");
            jo.Add("联系人电话", "");
            jo.Add("联系人地址", "");
            jo.Add("超量接收(允许/不允许)(必输)", "");
            //jo.Add("状态", "");
            jo.Add("说明", "");
            //jo.Add("创建人", "");
            //jo.Add("创建时间", "");
            //jo.Add("修改人", "");
            //jo.Add("修改时间", "");
            jo.Add("导入的错误信息", "");
            jObjects.Add(jo);
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            //var exportFileName = string.Concat(
            //        RouteData.Values["controller"].ToString() + "_Template",
            //        ".xlsx");
            var exportFileName = string.Concat("供应商导入模板",
                   ".xlsx");
            return new ExportExcelResult
            {
                SheetName = "Sheet1",
                FileName = exportFileName,
                ExportData = dt
            };
        }
        #endregion

        #region 选择供应商
        /// <summary>
        /// 弹出选择供应商
        /// </summary>
        /// <param name="mulSelect">是否多选</param>
        /// <returns></returns>
        [SupportFilter(ActionName = "Index")]
        public ActionResult SupplierLookUp(bool mulSelect = false)
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult SupplierGetList(GridPager pager, string supplierCode, string supplierShortName)
        {
            List<WMS_SupplierModel> list = m_BLL.GetListByWhere(ref pager, "Status == \"有效\" && SupplierCode.Contains(\""
                + supplierCode + "\") && SupplierShortName.Contains(\"" + supplierShortName + "\")");
            GridRows<WMS_SupplierModel> grs = new GridRows<WMS_SupplierModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetSupplierByCode(string supplierCode)
        {
            List<WMS_SupplierModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "Status == \"有效\" && SupplierCode == \""
                + supplierCode + "\"");
            if (list.Count() == 0)
            {
                return Json(JsonHandler.CreateMessage(0, "供应商编码不存在！"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed, JsonHandler.SerializeObject(list.First())));
            }
        }

        public JsonResult GetSupplierByShortName(string supplierShortName)
        {
            List<WMS_SupplierModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "Status == \"有效\" && SupplierShortName == \""
                + supplierShortName + "\"");
            if (list.Count() == 0)
            {
                return Json(JsonHandler.CreateMessage(0, "供应商简称不存在！"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed, JsonHandler.SerializeObject(list.First())));
            }
        }
        #endregion

        #region 根据物料的所属供应商，返回供应商列表
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetSupplierByBelong(string codes)
        {
            List<WMS_SupplierModel> list = m_BLL.GetListByBelong(ref setNoPagerAscById, codes);
            return Json(list);
        }
        #endregion
    }
}

