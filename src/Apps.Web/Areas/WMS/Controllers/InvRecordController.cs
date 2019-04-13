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
    public class InvRecordController : BaseController
    {
        [Dependency]
        public IWMS_InvRecordBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            //定义打印状态下拉框的值
            List<ReportType> Type = new List<ReportType>();
            Type.Add(new ReportType() { Type = 0, Name = "" });
            Type.Add(new ReportType() { Type = 1, Name = "盘点调整" });
            Type.Add(new ReportType() { Type = 2, Name = "自制件入库" });
            Type.Add(new ReportType() { Type = 2, Name = "投料" });
            Type.Add(new ReportType() { Type = 2, Name = "销售" });
            Type.Add(new ReportType() { Type = 2, Name = "退货" });
            Type.Add(new ReportType() { Type = 2, Name = "调账" });
            Type.Add(new ReportType() { Type = 2, Name = "重新送检" });

            ViewBag.Types = new SelectList(Type, "Name", "Name");

            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string sourceBill, string partCode, string partName, string type,  DateTime beginDate, DateTime endDate)
        {
            //List<WMS_InvRecordModel> list = m_BLL.GetList(ref pager, queryStr);
            //GridRows<WMS_InvRecordModel> grs = new GridRows<WMS_InvRecordModel>();
            //grs.rows = list;
            //grs.total = pager.totalRows;
            //return Json(grs);
            string query = " 1=1 ";
            query += " && SourceBill.Contains(\"" + sourceBill + "\")&&WMS_Part.PartCode.Contains(\"" + partCode + "\")";
            query += " && WMS_Part.PartName.Contains(\"" + partName + "\")&& Type.Contains(\"" + type + "\")";
            query += " && OperateDate>=(\"" + beginDate + "\")&& OperateDate<=(\"" + endDate + "\")";
            List<WMS_InvRecordModel> list = m_BLL.GetListByWhere(ref pager, query);
            GridRows<WMS_InvRecordModel> grs = new GridRows<WMS_InvRecordModel>();

            List<WMS_InvRecordModel> footerList = new List<WMS_InvRecordModel>();
            footerList.Add(new WMS_InvRecordModel()
            {
                SourceBill = "<div style='text-align:right;color:#444'>合计：</div>",
                QTY = list.Sum(p => p.QTY),
            });

            grs.rows = list;
            grs.footer = footerList;
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
        public JsonResult Create(WMS_InvRecordModel model)
        {
            model.Id = 0;
            model.OperateDate = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",PartId" + model.PartId, "成功", "创建", "WMS_InvRecord");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",PartId" + model.PartId + "," + ErrorCol, "失败", "创建", "WMS_InvRecord");
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
            WMS_InvRecordModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_InvRecordModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",PartId" + model.PartId, "成功", "修改", "WMS_InvRecord");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",PartId" + model.PartId + "," + ErrorCol, "失败", "修改", "WMS_InvRecord");
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
            WMS_InvRecordModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "WMS_InvRecord");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_InvRecord");
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
                 LogHandler.WriteImportExcelLog(GetUserId(), "WMS_InvRecord", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserId(), "WMS_InvRecord", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_InvRecordModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
        public ActionResult Export(string sourceBill, string partCode, string partName, string type, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_InvRecordModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            string query = " 1=1 ";
            query += " && SourceBill.Contains(\"" + sourceBill + "\")&&WMS_Part.PartCode.Contains(\"" + partCode + "\")";
            query += " && WMS_Part.PartName.Contains(\"" + partName + "\")&& Type.Contains(\"" + type + "\")";
            query += " && OperateDate>=(\"" + beginDate + "\")&& OperateDate<=(\"" + endDate + "\")";

            //List<WMS_Sale_OrderModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            List<WMS_InvRecordModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, query);
            
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("单据编号", item.SourceBill);
                    jo.Add("物料编码", item.PartId);
                    jo.Add("物料名称", item.PartName);
                    jo.Add("数量", item.QTY);
                    jo.Add("操作时间", item.OperateDate);
                    jo.Add("批次", item.Lot);
                    jo.Add("出入库类型", item.Type);
                    jo.Add("操作人", item.OperateMan);
                //jo.Add("库房编码", item.InvId);
                //    jo.Add("SubInvId", item.SubInvId);
                //    jo.Add("单据ID", item.BillId);
                //    jo.Add("单据来源", item.SourceBill);
                //    jo.Add("备料库存", item.Stock_InvId);
                //    jo.Add("备料状态：1-不适用（直接修改库存现有量）；2-已备料；3-无效备料（取消备料后将2改成3）；4-取消备料（当前操作是取消备料）", item.StockStatus);
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
              jo.Add("出入库明细ID", "");
              jo.Add("物料编码", "");
              jo.Add("数量", "");
              jo.Add("库房编码", "");
              jo.Add("SubInvId", "");
              jo.Add("单据ID", "");
              jo.Add("单据来源", "");
              jo.Add("操作时间", "");
              jo.Add("Lot", "");
              jo.Add("出入库类型", "");
              jo.Add("操作人", "");
              jo.Add("备料库存", "");
              jo.Add("备料状态：1-不适用（直接修改库存现有量）；2-已备料；3-无效备料（取消备料后将2改成3）；4-取消备料（当前操作是取消备料）", "");
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

