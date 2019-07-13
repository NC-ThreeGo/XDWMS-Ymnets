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
    public class POController : BaseController
    {
        [Dependency]
        public IWMS_POBLL m_BLL { get; set; }
        [Dependency]
        public IWMS_SupplierBLL m_SupplierBLL { get; set; }
        [Dependency]
        public IWMS_PartBLL m_PartBLL { get; set; }

        [Dependency]
        public IWMS_AIBLL m_AIBLL { get; set; }

        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string po,string supplierShortName,string partCode, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_POModel> list = m_BLL.GetList(ref pager, queryStr);
            //GridRows<WMS_POModel> grs = new GridRows<WMS_POModel>();
            //grs.rows = list;
            //grs.total = pager.totalRows;
            //return Json(grs);

            List<WMS_POModel> list = m_BLL.GetListByWhere(ref pager, "PO.Contains(\"" + po + "\") && WMS_Supplier.SupplierShortName.Contains(\""
                + supplierShortName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& CreateTime>=(\""
                + beginDate + "\")&& CreateTime<=(\"" + endDate.AddDays(1) + "\")");
            GridRows<WMS_POModel> grs = new GridRows<WMS_POModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);

        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Supplier = new SelectList(m_SupplierBLL.GetList(ref setNoPagerAscById, ""), "Id", "SupplierShortName");

            WMS_POModel model = new WMS_POModel()
            {

                PO = "PO" + DateTime.Now.ToString("yyyyMMddHHmmssff"),
            };
            return View(model);
            //return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_POModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = GetUserTrueName();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",PO" + model.PO, "成功", "创建", "WMS_PO");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",PO" + model.PO + "," + ErrorCol, "失败", "创建", "WMS_PO");
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
            ViewBag.Supplier = new SelectList(m_SupplierBLL.GetList(ref setNoPagerAscById, ""), "Id", "SupplierShortName");
            //输入框内值是否可以修改
            ViewBag.EditStatus = true;
            WMS_POModel entity = m_BLL.GetById(id);
            //给关联字段代理商简称赋值
            WMS_SupplierModel entity_s = m_SupplierBLL.GetById(entity.SupplierId);
            entity.SupplierShortName = entity_s.SupplierShortName;
            //给关联字段物料编码赋值
            WMS_PartModel entity_p = m_PartBLL.GetById(entity.PartId);
            entity.PartCode = entity_p.PartCode;            
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_POModel model)
        {
            model.ModifyTime = ResultHelper.NowTime;
            model.ModifyPerson = GetUserTrueName();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",PO" + model.PO, "成功", "修改", "WMS_PO");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",PO" + model.PO + "," + ErrorCol, "失败", "修改", "WMS_PO");
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
            WMS_POModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(long id)
        {
            List<WMS_AIModel> list = m_AIBLL.GetListByWhere(ref setNoPagerAscById, "POId = " + id);
            if (id!=0 && list.Count().Equals(0))
            {
                
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_PO");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_PO");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, "删除失败：订单已入库"));
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
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_PO", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_PO", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string po, string supplierShortName, string partCode, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_POModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            List<WMS_POModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "PO.Contains(\"" + po + "\") && WMS_Supplier.SupplierShortName.Contains(\""
               + supplierShortName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& CreateTime>=(\""
               + beginDate + "\")&& CreateTime<=(\"" + endDate + "\")");
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
        public ActionResult Export(string po, string supplierShortName, string partCode, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_POModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            List<WMS_POModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "PO.Contains(\"" + po + "\") && WMS_Supplier.SupplierShortName.Contains(\""
              + supplierShortName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& CreateTime>=(\""
              + beginDate + "\")&& CreateTime<=(\"" + endDate.AddDays(1) + "\")");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    //jo.Add("采购订单ID", item.Id);
                    jo.Add("采购订单", item.PO);
                    jo.Add("采购日期", item.PODate);
                //jo.Add("供应商编码", item.SupplierId);
                //jo.Add("物料编码", item.PartId);
                    jo.Add("供应商简称", item.SupplierShortName);
                    jo.Add("物料编码", item.PartCode);
                    jo.Add("数量", item.QTY);
                    jo.Add("计划到货日期", item.PlanDate);
                    jo.Add("采购订单类型", item.POType);
                    //jo.Add("允许超量接收", item.MoreAccept);
                    jo.Add("状态", item.Status);
                    jo.Add("说明", item.Remark);
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
              //jo.Add("采购订单ID", "");
              jo.Add("采购订单(必输)", "");
              jo.Add("采购日期(必输格式:YYYY-MM-DD)", "");
              jo.Add("供应商简称(必输)", "");
              jo.Add("物料编码(必输)", "");
              jo.Add("数量(必输)", "");
              jo.Add("计划到货日期", "");
              jo.Add("采购订单类型", "");
              //jo.Add("状态", "");
              jo.Add("说明", "");
              //jo.Add("Attr1", "");
              //jo.Add("Attr2", "");
              //jo.Add("Attr3", "");
              //jo.Add("Attr4", "");
              //jo.Add("Attr5", "");
              //jo.Add("创建人", "");
              //jo.Add("创建时间", "");
              //jo.Add("修改人", "");
              //jo.Add("修改时间", "");
            jo.Add("导入的错误信息", "");
            jObjects.Add(jo);
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            var exportFileName = string.Concat("采购订单导入模板",
                    ".xlsx");
                return new ExportExcelResult
                {
                    SheetName = "Sheet1",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
        #endregion

        #region 选择PO
        /// <summary>
        /// 弹出选择PO
        /// </summary>
        /// <param name="mulSelect">是否多选</param>
        /// <returns></returns>
        [SupportFilter(ActionName = "Create")]
        public ActionResult POLookUp(bool mulSelect = false)
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult POGetList(GridPager pager, string po, string supplierShortName)
        {
            //TODO:显示有效且未关闭的采购订单。是否需要显示PO的物料信息（还是只显示PO的头信息）？？？
            //List<WMS_POModel> list = m_BLL.GetListByWhere(ref pager, "PO.Contains(\"" + po + "\") && WMS_Supplier.SupplierShortName.Contains(\"" + supplierShortName + "\") && Status == \"有效\"")
            //    .OrderBy(p => p.PODate).ToList();
            List<WMS_POModel> list = m_BLL.GetListByWhereAndGroupBy(ref pager, "PO.Contains(\"" + po + "\") && WMS_Supplier.SupplierShortName.Contains(\"" + supplierShortName + "\") && Status == \"有效\"");
            GridRows<WMS_POModel> grs = new GridRows<WMS_POModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
           
           
        }
        #endregion

        #region 获取指定PO指定物料的行信息
        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult GetPOLineByPartId(string po, string partCode)
        {
            var part = m_PartBLL.GetListByWhere(ref setNoPagerAscById, "PartCode == \"" + partCode + "\"").First();
            if (part == null)
            {
                return Json(JsonHandler.CreateMessage(0, "物料编码不存在！"));
            }

            var line = m_BLL.GetListByWhere(ref setNoPagerAscById, "PO == \"" + po + "\" && PartId == " + part.Id + "").First();
            if (line != null)
            {
                return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed, JsonHandler.SerializeObject(line)));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.CheckFail));
            }
        }
        #endregion
    }
}

