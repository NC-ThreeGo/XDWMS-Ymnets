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
    public class FeedListController : BaseController
    {
        [Dependency]
        public IWMS_Feed_ListBLL m_BLL { get; set; }
        [Dependency]
        public IWMS_InvInfoBLL _InvInfoBll { get; set; }

        [Dependency]
        public IWMS_PartBLL m_PartBLL { get; set; }

        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            //定义打印状态下拉框的值
            List<ReportType> PrintTypes = new List<ReportType>();
            PrintTypes.Add(new ReportType() { Type = 0, Name = "" });
            PrintTypes.Add(new ReportType() { Type = 1, Name = "未打印" });
            PrintTypes.Add(new ReportType() { Type = 2, Name = "已打印" });
            PrintTypes.Add(new ReportType() { Type = 2, Name = "已失效" });
            ViewBag.PrintStaus = new SelectList(PrintTypes, "Name", "Name");

            //定义打印状态下拉框的值
            List<ReportType> ConfirmTypes = new List<ReportType>();
            ConfirmTypes.Add(new ReportType() { Type = 0, Name = "" });
            ConfirmTypes.Add(new ReportType() { Type = 1, Name = "未确认" });
            ConfirmTypes.Add(new ReportType() { Type = 2, Name = "已确认" });
            ViewBag.ConfirmStatus = new SelectList(ConfirmTypes, "Name", "Name");


            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string feedBillNum, string assemblyPartCode, string subAssemblyPartCode, string printStaus, string confirmStatus, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_Feed_ListModel> list = m_BLL.GetList(ref pager, queryStr);
            string query = " 1=1 ";
            if (printStaus == "已打印")
            {
               query += " && PrintDate>=(\"" + beginDate + "\")&& PrintDate<=(\"" + endDate + "\")";
            }
            //query += " && FeedBillNum.Contains(\"" + feedBillNum + "\")&&WMS_Part.PartCode.Contains(\"" + assemblyPartCode + "\")";
            query += " && FeedBillNum.Contains(\"" + feedBillNum + "\")";
            query += " && WMS_Part.PartCode.Contains(\"" + subAssemblyPartCode + "\")&& PrintStaus.Contains(\"" + printStaus + "\")&& ConfirmStatus.Contains(\"" + confirmStatus + "\")";

            //List<WMS_Feed_ListModel> list = m_BLL.GetListByWhere(ref pager, "FeedBillNum.Contains(\"" + feedBillNum + "\")&&WMS_Part.PartCode.Contains(\"" + assemblyPartCode + "\") && WMS_Part1.PartCode.Contains(\""
            //  + subAssemblyPartCode + "\")&& PrintStaus.Contains(\"" + printStaus + "\")&& ConfirmStatus.Contains(\"" + confirmStatus + "\")&& PrintDate>=(\""
            //  + beginDate + "\")&& PrintDate<=(\"" + endDate + "\")");

            List<WMS_Feed_ListModel> list = m_BLL.GetListByWhere(ref pager, query);
            GridRows<WMS_Feed_ListModel> grs = new GridRows<WMS_Feed_ListModel>();

            List<WMS_Feed_ListModel> footerList = new List<WMS_Feed_ListModel>();
            footerList.Add(new WMS_Feed_ListModel()
            {
                FeedBillNum = "<div style='text-align:right;color:#444'>合计：</div>",
                FeedQty = list.Sum(p => p.FeedQty),
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
            ViewBag.Inv = new SelectList(_InvInfoBll.GetList(ref setNoPagerAscById, ""), "Id", "InvName");
            WMS_Feed_ListModel model = new WMS_Feed_ListModel()
            {
                FeedBillNum = "TL" + DateTime.Now.ToString("yyyyMMddHHmmssff"),
                ReleaseBillNum = "TL" + DateTime.Now.ToString("yyyyMMddHHmmssff"),

            };
            return View(model);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_Feed_ListModel model)
        {
            model.Id = 0;
            model.CreatePerson = GetUserTrueName();
            model.CreateTime = ResultHelper.NowTime;
            model.PrintStaus = "未打印";
            model.ConfirmStatus = "未确认";
            if (model.Lot == "[空]")
                model.Lot = "";
            if (model.Lot == "" || model.Lot == null || !DateTimeHelper.CheckYearMonth(model.Lot))
            {
                return Json(JsonHandler.CreateMessage(0, "批次录入不符合规范"));
            }
            if (model != null && ModelState.IsValid )
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",FeedBillNum" + model.FeedBillNum, "成功", "创建", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",FeedBillNum" + model.FeedBillNum + "," + ErrorCol, "失败", "创建", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 打印/取消打印
        [SupportFilter(ActionName = "Create")]
        public ActionResult Print()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        [ValidateInput(false)]
        public JsonResult Print(string feedBillNum, int id = 0)
        {
            try
            {
                var releaseBillNum = m_BLL.PrintFeedList(ref errors, GetUserTrueName(), feedBillNum, id);
                if (!String.IsNullOrEmpty(releaseBillNum))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "打印投料单成功，id:" + id.ToString(), "成功", "打印", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, feedBillNum));
                    //return Redirect("~/Report/ReportManager/ShowBill?reportCode=ReturnOrder&billNum=" + returnOrderNum);
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), errors.Error + ",id:" + id.ToString(), "失败", "打印", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + errors.Error));
                }

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserTrueName(), ex.Message, "失败", "打印", "WMS_Feed_List");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
            }
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        [ValidateInput(false)]
        public JsonResult UnPrint(string releaseBillNum, int id = 0)
        {
            try
            {
                if (m_BLL.UnPrintFeedList(ref errors, GetUserTrueName(), releaseBillNum, id))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "取消打印投料单成功，id:" + id.ToString(), "成功", "取消打印", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, releaseBillNum));
                    //return Redirect("~/Report/ReportManager/ShowBill?reportCode=ReturnOrder&billNum=" + returnOrderNum);
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), errors.Error + ",id:" + id.ToString(), "失败", "取消打印", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + errors.Error));
                }

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserTrueName(), ex.Message, "失败", "取消打印", "WMS_Feed_List");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
            }
        }
        #endregion

        #region 确认
        [SupportFilter(ActionName = "Edit")]
        public ActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        [ValidateInput(false)]
        public JsonResult Confirm(string releaseBillNum)
        {
            try
            {
                if (m_BLL.ConfirmFeedList(ref errors, GetUserTrueName(), releaseBillNum))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "ReleaseBillNum" + releaseBillNum, "成功", "确认", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, releaseBillNum));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "ReleaseBillNum" + releaseBillNum + ", " + errors.Error, "失败", "确认", "WMS_Feed_List");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + errors.Error));
                }

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserTrueName(), "ReturnOrderNum" + releaseBillNum + ", " + ex.Message, "失败", "确认", "WMS_Feed_List");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(long id)
        {
            ViewBag.Inv = new SelectList(_InvInfoBll.GetList(ref setNoPagerAscById, ""), "Id", "InvName");
            WMS_Feed_ListModel entity = m_BLL.GetById(id);
            WMS_PartModel entity_p = m_PartBLL.GetById(entity.AssemblyPartId);
            entity.AssemblyPartCode = entity_p.PartCode;
            WMS_PartModel entity_p1 = m_PartBLL.GetById(entity.SubAssemblyPartId);
            entity.SubAssemblyPartCode = entity_p1.PartCode;

            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_Feed_ListModel model)
        {
            List<WMS_Feed_ListModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "Id = " + model.Id + " &&ConfirmStatus = \"已确认\"");
            if (list.Count() > 0)
            {
                return Json(JsonHandler.CreateMessage(0, "已确认单据不能修改"));
            }
            else
            {              
                model.ModifyTime = ResultHelper.NowTime;
                model.ModifyPerson = GetUserTrueName();
                if (model != null && ModelState.IsValid)
                {

                    if (m_BLL.Edit(ref errors, model))
                    {
                        LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",FeedBillNum" + model.FeedBillNum, "成功", "修改", "WMS_Feed_List");
                        return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                    }
                    else
                    {
                        string ErrorCol = errors.Error;
                        LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",FeedBillNum" + model.FeedBillNum + "," + ErrorCol, "失败", "修改", "WMS_Feed_List");
                        return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                    }
                }
                else
                {
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
                }
            }
                
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(long id)
        {
            WMS_Feed_ListModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(long id)
        {
            List<WMS_Feed_ListModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "Id = " + id + " &&ConfirmStatus = \"已确认\"");
            if (list.Count() > 0)
            {
                return Json(JsonHandler.CreateMessage(0, "已确认单据不能删除"));
            }
            else
            {
                if (id != 0)
                {
                    WMS_Feed_ListModel model = m_BLL.GetById(id);
                    model.ModifyTime = ResultHelper.NowTime;
                    model.ModifyPerson = GetUserTrueName();
                    model.PrintStaus = "已失效";

                    if (model != null && ModelState.IsValid)
                    {

                        if (m_BLL.Edit(ref errors, model))
                        {
                            LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",FeedBillNum" + model.FeedBillNum, "成功", "修改", "WMS_Feed_List");
                            return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                        }
                        else
                        {
                            string ErrorCol = errors.Error;
                            LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",FeedBillNum" + model.FeedBillNum + "," + ErrorCol, "失败", "修改", "WMS_Feed_List");
                            return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                        }
                    }
                    else
                    {
                        return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
                    }

                    //if (m_BLL.Delete(ref errors, id))
                    //{
                    //    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_Feed_List");
                    //    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                    //}
                    //else
                    //{
                    //    string ErrorCol = errors.Error;
                    //    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_Feed_List");
                    //    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                    //}
                }
                else
                {
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
                }
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
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Feed_List", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Feed_List", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_Feed_ListModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
        public ActionResult Export(string feedBillNum, string assemblyPartCode, string subAssemblyPartCode, string printStaus, string confirmStatus, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_Feed_ListModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            string query = " 1=1 ";
            if (printStaus == "已打印")
            {
                query += " && PrintDate>=(\"" + beginDate + "\")&& PrintDate<=(\"" + endDate + "\")";
            }
            query += " && FeedBillNum.Contains(\"" + feedBillNum + "\")&&WMS_Part.PartCode.Contains(\"" + assemblyPartCode + "\")";
            query += " && WMS_Part1.PartCode.Contains(\"" + subAssemblyPartCode + "\")&& PrintStaus.Contains(\"" + printStaus + "\")&& ConfirmStatus.Contains(\"" + confirmStatus + "\")";

            //List<WMS_Feed_ListModel> list = m_BLL.GetListByWhere(ref pager, "FeedBillNum.Contains(\"" + feedBillNum + "\")&&WMS_Part.PartCode.Contains(\"" + assemblyPartCode + "\") && WMS_Part1.PartCode.Contains(\""
            //  + subAssemblyPartCode + "\")&& PrintStaus.Contains(\"" + printStaus + "\")&& ConfirmStatus.Contains(\"" + confirmStatus + "\")&& PrintDate>=(\""
            //  + beginDate + "\")&& PrintDate<=(\"" + endDate + "\")");

            List<WMS_Feed_ListModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, query);
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    //jo.Add("Id", item.Id);
                    jo.Add("投料单号（业务）", item.FeedBillNum);
                    jo.Add("投料单号（系统）", item.ReleaseBillNum);
                    jo.Add("投料部门", item.Department);
                    jo.Add("总成物料", item.AssemblyPartId);
                    jo.Add("投料物料", item.SubAssemblyPartId);                
                jo.Add("投料数量", item.FeedQty);
                    jo.Add("箱数", item.BoxQty);
                    jo.Add("体积", item.Capacity);
                    jo.Add("库房", item.InvId);
                jo.Add("批次号", item.Lot);
                //jo.Add("子库存", item.SubInvId);
                    jo.Add("备注", item.Remark);
                    jo.Add("打印状态", item.PrintStaus);
                    jo.Add("打印时间", item.PrintDate);
                    jo.Add("打印人", item.PrintMan);
                    jo.Add("确认状态", item.ConfirmStatus);
                    jo.Add("确认人", item.ConfirmMan);
                    jo.Add("确认时间", item.ConfirmDate);
                jo.Add("确认信息", item.ConfirmMessage);
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
              //jo.Add("Id", "");
              jo.Add("投料单号（业务）(必输)", "");
              //jo.Add("投料单号（系统）", "");
              jo.Add("投料部门", "");
              jo.Add("总成物料", "");
              jo.Add("投料物料(必输)", "");            
            jo.Add("投料数量(必输)", "");
              jo.Add("箱数", "");
              jo.Add("体积", "");            
            jo.Add("库房", "");
            jo.Add("批次号(格式：YYYY-MM-DD)", "");
            //jo.Add("子库存", "");
            jo.Add("备注", "");
              //jo.Add("打印状态", "");
              //jo.Add("打印时间", "");
              //jo.Add("打印人", "");
              //jo.Add("确认状态", "");
              //jo.Add("确认人", "");
              //jo.Add("确认时间", "");
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
            var exportFileName = string.Concat("投料单导入模板",
                    ".xlsx");
                return new ExportExcelResult
                {
                    SheetName = "Sheet1",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
        #endregion

        #region 选择投料单
        /// <summary>
        /// 弹出选择送检单
        /// </summary>
        /// <param name="mulSelect">是否多选</param>
        /// <returns></returns>
        [SupportFilter(ActionName = "Create")]
        public ActionResult FeedListLookUp(string type, bool mulSelect = false)
        {
            ViewBag.Type = type;
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult FeedListGetList(GridPager pager, string type, string queryStr)
        {
            //TODO:显示未打印的投料单。
            List<WMS_Feed_ListModel> list;

            if (type == "print")
            {
                list = m_BLL.GetListByWhereAndGroupBy(ref pager, "PrintStaus == \"未打印\"");
            }
            else
            {
                list = m_BLL.GetListByWhereAndGroupBy(ref pager, "PrintStaus == \"已打印\" and ConfirmStatus == \"未确认\"");
            }
            GridRows<WMS_Feed_ListModel> grs = new GridRows<WMS_Feed_ListModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult GetFeedListByBillNum(GridPager pager, string type, string billNum)
        {
            List<WMS_Feed_ListModel> list;

            if (type == "print")
            {
                list  = m_BLL.GetListByWhere(ref pager, "FeedBillNum = \"" + billNum + "\"&& PrintStaus == \"未打印\"");
            }
            else
            {
                list = m_BLL.GetListByWhere(ref pager, "ReleaseBillNum = \"" + billNum + "\"&& PrintStaus == \"已打印\"");
            }
            GridRows<WMS_Feed_ListModel> grs = new GridRows<WMS_Feed_ListModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #endregion
    }
}

