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
    public class PartController : BaseController
    {
        [Dependency]
        public IWMS_PartBLL m_BLL { get; set; }
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
            List<WMS_PartModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<WMS_PartModel> grs = new GridRows<WMS_PartModel>();
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
        public JsonResult Create(WMS_PartModel model)
        {
            model.part_id = 0;
            model.creation_date = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "part_id" + model.part_id + ",part_code" + model.part_code, "成功", "创建", "WMS_Part");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "part_id" + model.part_id + ",part_code" + model.part_code + "," + ErrorCol, "失败", "创建", "WMS_Part");
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
            WMS_PartModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_PartModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "part_id" + model.part_id + ",part_code" + model.part_code, "成功", "修改", "WMS_Part");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "part_id" + model.part_id + ",part_code" + model.part_code + "," + ErrorCol, "失败", "修改", "WMS_Part");
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
            WMS_PartModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "part_id:" + id, "成功", "删除", "WMS_Part");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "part_id" + id + "," + ErrorCol, "失败", "删除", "WMS_Part");
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
            var list = new List<WMS_PartModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "WMS_Part");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "WMS_Part");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<WMS_PartModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
            List<WMS_PartModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("part_id", item.part_id);
                jo.Add("part_code", item.part_code);
                jo.Add("part_name", item.part_name);
                jo.Add("part_type", item.part_type);
                jo.Add("customer_code", item.customer_code);
                jo.Add("logistics_code", item.logistics_code);
                jo.Add("other_code", item.other_code);
                jo.Add("pcs", item.pcs);
                jo.Add("storeman", item.storeman);
                jo.Add("status", item.status);
                jo.Add("created_by", item.created_by);
                jo.Add("creation_date", item.creation_date);
                jo.Add("updated_by", item.updated_by);
                jo.Add("update_date", item.update_date);
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
