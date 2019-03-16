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
    public class ReturnGoodsController : BaseController
    {
        [Dependency]
        public IWMS_Inv_AdjustBLL m_BLL { get; set; }

        [Dependency]
        public IWMS_InvInfoBLL _InvInfoBll { get; set; }

        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            //ViewBag.Type = "0";
           
            ViewBag.AdjustType = Apps.BLL.Sys.SysParamBLL.GetSysParamByType("ReturnGoodsType",true);
            
            return View("~/Areas/WMS/Views/InvAdjust/Index.cshtml");            
        }
        
       [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string invAdjustBillNum, string partCode, string partName, string adjustType, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_Inv_AdjustModel> list = m_BLL.GetList(ref pager, queryStr);
            List<WMS_Inv_AdjustModel> list = m_BLL.GetListByWhere(ref pager, "InvAdjustBillNum.Contains(\"" + invAdjustBillNum + "\") && WMS_Part.PartName.Contains(\""
              + partName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& AdjustType.Contains(\"" + adjustType + "\")&& CreateTime>=(\""
              + beginDate + "\")&& CreateTime<=(\"" + endDate + "\")&& AdjustType.Contains(\"售\")");
            GridRows<WMS_Inv_AdjustModel> grs = new GridRows<WMS_Inv_AdjustModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Type = "0";
            ViewBag.Inv = new SelectList(_InvInfoBll.GetList(ref setNoPagerAscById, ""), "Id", "InvName");
            WMS_Inv_AdjustModel model = new WMS_Inv_AdjustModel()
            {
                InvAdjustBillNum = "TZ" + DateTime.Now.ToString("yyyyMMddHHmmssff"),

            };
            return View("~/Areas/WMS/Views/InvAdjust/Create.cshtml",model);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_Inv_AdjustModel model)
        {
            model.Id = 0;
            model.CreatePerson = GetUserTrueName();
            model.CreateTime = ResultHelper.NowTime;
            if (model.Lot == "" || model.Lot == null || !DateTimeHelper.CheckYearMonth(model.Lot))
            {
                return Json(JsonHandler.CreateMessage(0, "批次录入不符合规范"));
            }
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",InvAdjustBillNum" + model.InvAdjustBillNum, "成功", "创建", "WMS_Inv_Adjust");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",InvAdjustBillNum" + model.InvAdjustBillNum + "," + ErrorCol, "失败", "创建", "WMS_Inv_Adjust");
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
            WMS_Inv_AdjustModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_Inv_AdjustModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",InvAdjustBillNum" + model.InvAdjustBillNum, "成功", "修改", "WMS_Inv_Adjust");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",InvAdjustBillNum" + model.InvAdjustBillNum + "," + ErrorCol, "失败", "修改", "WMS_Inv_Adjust");
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
            WMS_Inv_AdjustModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_Inv_Adjust");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_Inv_Adjust");
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
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Inv_Adjust", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Inv_Adjust", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_Inv_AdjustModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
        public ActionResult Export(string invAdjustBillNum, string partCode, string partName, string adjustType, DateTime beginDate, DateTime endDate)
        {
            List<WMS_Inv_AdjustModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "InvAdjustBillNum.Contains(\"" + invAdjustBillNum + "\") && WMS_Part.PartName.Contains(\""
              + partName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& AdjustType.Contains(\"" + adjustType + "\")&& CreateTime>=(\""
              + beginDate + "\")&& CreateTime<=(\"" + endDate + "\")&& AdjustType.Contains(\"售\")");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    //jo.Add("Id", item.Id);
                    jo.Add("单据号", item.InvAdjustBillNum);
                    jo.Add("物料编码", item.PartCode);
                    jo.Add("物料名称", item.PartCode);
                    jo.Add("数量", item.AdjustQty);
                    jo.Add("类型", item.AdjustType);
                    jo.Add("库存", item.InvName);
                    //jo.Add("子库存", item.SubInvId);
                    jo.Add("备注", item.Remark);
                    //jo.Add("Attr1", item.Attr1);
                    //jo.Add("Attr2", item.Attr2);
                    //jo.Add("Attr3", item.Attr3);
                    //jo.Add("Attr4", item.Attr4);
                    //jo.Add("Attr5", item.Attr5);
                    jo.Add("操作人", item.CreatePerson);
                    jo.Add("操作时间", item.CreateTime);
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
              jo.Add("Id", "");
              jo.Add("调帐单据号", "");
              jo.Add("物料", "");
              jo.Add("调整数量", "");
              jo.Add("调整类型", "");
              jo.Add("库存", "");
              jo.Add("子库存", "");
              jo.Add("备注", "");
              jo.Add("Attr1", "");
              jo.Add("Attr2", "");
              jo.Add("Attr3", "");
              jo.Add("Attr4", "");
              jo.Add("Attr5", "");
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
    }
}

