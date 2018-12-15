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

namespace Apps.Web.Areas.Report.Controllers
{
    public class ReportParamController : BaseController
    {
        [Dependency]
        public IWMS_ReportParamBLL m_BLL { get; set; }
        [Dependency]
        public IWMS_ReportBLL m_ReportBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.ParamTypes = new SelectList(WMS_ReportParamModel.GetParamType(), "TypeCode", "TypeName");
            ViewBag.ParamElements = new SelectList(WMS_ReportParamModel.GetParamElement(), "ElementCode", "ElementName");
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr, string parentId)
        {
            List<WMS_ReportParamModel> list = m_BLL.GetListByParentId(ref pager, queryStr, parentId);
            GridRows<WMS_ReportParamModel> grs = new GridRows<WMS_ReportParamModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Report = new SelectList(m_ReportBLL.GetList(ref setNoPagerAscById, ""), "Id", "ReportName");
            ViewBag.ParamTypes = new SelectList(WMS_ReportParamModel.GetParamType(), "TypeCode", "TypeName");
            ViewBag.ParamElements = new SelectList(WMS_ReportParamModel.GetParamElement(), "ElementCode", "ElementName");
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_ReportParamModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParamCode" + model.ParamCode, "成功", "创建", "WMS_ReportParam");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParamCode" + model.ParamCode + "," + ErrorCol, "失败", "创建", "WMS_ReportParam");
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
            ViewBag.ParamTypes = new SelectList(WMS_ReportParamModel.GetParamType(), "TypeCode", "TypeName");
            ViewBag.ParamElements = new SelectList(WMS_ReportParamModel.GetParamElement(), "ElementCode", "ElementName");
            WMS_ReportParamModel entity = m_BLL.GetById(id);
            ViewBag.Report = new SelectList(m_ReportBLL.GetList(ref setNoPagerAscById, ""), "Id", "ReportName", entity.ReportId);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_ReportParamModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParamCode" + model.ParamCode, "成功", "修改", "WMS_ReportParam");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParamCode" + model.ParamCode + "," + ErrorCol, "失败", "修改", "WMS_ReportParam");
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
            WMS_ReportParamModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "WMS_ReportParam");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_ReportParam");
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
            if (m_BLL.ImportExcelData(GetUserId(), Utils.GetMapPath(filePath), ref errors))
            {
                LogHandler.WriteImportExcelLog(GetUserId(), "WMS_ReportParam", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                LogHandler.WriteImportExcelLog(GetUserId(), "WMS_ReportParam", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_ReportParamModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
            List<WMS_ReportParamModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("参数代码", item.ParamCode);
                jo.Add("报表ID", item.ReportId);
                jo.Add("InputNo", item.InputNo);
                jo.Add("参数名", item.ParamName);
                jo.Add("显示名称", item.ShowName);
                jo.Add("参数类型：varchar、int、datetime", item.ParamType);
                jo.Add("可选值", item.ParamData);
                jo.Add("默认值", item.DefaultValue);
                jo.Add("显示元素：文本框、下拉框、日期框等", item.ParamElement);
                jo.Add("备注", item.Remark);
                jo.Add("创建人", item.CreatePerson);
                jo.Add("创建时间", item.CreateTime);
                jo.Add("修改人", item.ModifyPerson);
                jo.Add("修改时间", item.ModifyTime);
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
            jo.Add("参数代码", "");
            jo.Add("报表ID", "");
            jo.Add("InputNo", "");
            jo.Add("参数名", "");
            jo.Add("显示名称", "");
            jo.Add("参数类型：varchar、int、datetime", "");
            jo.Add("可选值", "");
            jo.Add("默认值", "");
            jo.Add("显示元素：文本框、下拉框、日期框等", "");
            jo.Add("备注", "");
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
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetListParent(GridPager pager, string queryStr)
        {
            List<WMS_ReportModel> list = m_ReportBLL.GetList(ref pager, queryStr);
            GridRows<WMS_ReportModel> grs = new GridRows<WMS_ReportModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
    }
}

