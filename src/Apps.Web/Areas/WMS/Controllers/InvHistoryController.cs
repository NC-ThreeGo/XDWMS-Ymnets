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
using Apps.Models;

namespace Apps.Web.Areas.WMS.Controllers
{
    public class InvHistoryController : BaseController
    {
        [Dependency]
        public IWMS_Inv_History_DBLL m_BLL { get; set; }

        [Dependency]
        public IWMS_Inv_History_HBLL m_HeaderBLL { get; set; }

        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string queryStr,string headId)
        {
            if (!String.IsNullOrEmpty(queryStr))
            {
                queryStr = queryStr.Trim();
            }
            List<WMS_Inv_History_DModel> list = m_BLL.GetListByParentId(ref pager, queryStr, headId);
            GridRows<WMS_Inv_History_DModel> grs = new GridRows<WMS_Inv_History_DModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 查看近3期的历史库存平均值
        [SupportFilter(ActionName = "Index")]
        public ActionResult ListAvg()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetListAvg(GridPager pager, string partCode)
        {
            List<WMS_InvHistoryAvg> list = m_BLL.GetInvHistoryAvg(ref pager, "PartCode.Contains(\"" + partCode + "\")");
            GridRows<WMS_InvHistoryAvg> grs = new GridRows<WMS_InvHistoryAvg>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_Inv_History_HModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, GetUserTrueName(), model.InvHistoryTitle, model.InvHistoryStatus, model.Remark))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id, "成功", "创建", "WMS_Inv_History");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + "," + ErrorCol, "失败", "创建", "WMS_Inv_History");
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
            WMS_Inv_History_HModel entity = m_HeaderBLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_Inv_History_HModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                if (m_HeaderBLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id, "成功", "修改", "WMS_Inv_History_H");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + "," + ErrorCol, "失败", "修改", "WMS_Inv_History_H");
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
            WMS_Inv_History_DModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(long id)
        {
            if(id!=0)
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "WMS_Inv_History_D");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_Inv_History_D");
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
                 LogHandler.WriteImportExcelLog(GetUserId(), "WMS_Inv_History_D", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserId(), "WMS_Inv_History_D", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_Inv_History_DModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
            List<WMS_InvHistoryAvg> list = m_BLL.GetInvHistoryAvg(ref setNoPagerAscById, "PartCode.Contains(\"" + queryStr + "\")");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("库房ID", item.InvId);
                    jo.Add("库房名称", item.InvCode);
                    jo.Add("物料ID", item.PartId);
                    jo.Add("物料编码", item.PartCode);
                    jo.Add("物料名称", item.PartName);
                jo.Add("保管员", item.StoreMan);
                jo.Add("平均库存", item.AvgQty);
                    jo.Add("当前库存", item.InvQty);
                    jo.Add("差额", item.BalanceQty);
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
            jo.Add("库房ID", "");
            jo.Add("库房名称", "");
            jo.Add("物料ID", "");
            jo.Add("物料编码", "");
            jo.Add("物料名称", "");
            jo.Add("平均库存", "");
            jo.Add("当前库存", "");
            jo.Add("差额", "");
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
        [SupportFilter(ActionName="Index")]
        public JsonResult GetListParent(GridPager pager, string queryStr)
        {
            List<WMS_Inv_History_HModel> list = m_BLL.GetListParent(ref pager, "1 == 1");
            GridRows<WMS_Inv_History_HModel> grs = new GridRows<WMS_Inv_History_HModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
    }
}

