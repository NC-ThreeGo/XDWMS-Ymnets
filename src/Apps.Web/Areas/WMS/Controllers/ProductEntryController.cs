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
    public class ProductEntryController : BaseController
    {
        
        [Dependency]
        public IWMS_Product_EntryBLL m_BLL { get; set; }
        [Dependency]
        public IWMS_InvInfoBLL _InvInfoBll { get; set; }
        [Dependency]
        public IWMS_ReturnInspectionBLL _ReturnInspectionBLL { get; set; }

        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string partName, string partCode, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_Product_EntryModel> list = m_BLL.GetList(ref pager, queryStr);
            //GridRows<WMS_Product_EntryModel> grs = new GridRows<WMS_Product_EntryModel>();
            //grs.rows = list;
            //grs.total = pager.totalRows;
            //return Json(grs);
            List<WMS_Product_EntryModel> list = m_BLL.GetListByWhere(ref pager, "WMS_Part.PartName.Contains(\""
                + partName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& CreateTime>=(\""
                + beginDate + "\")&& CreateTime<=(\"" + endDate.AddDays(1) + "\")");
            GridRows<WMS_Product_EntryModel> grs = new GridRows<WMS_Product_EntryModel>();
            
            //增加退货检验单据
            //List<WMS_ReturnInspectionModel> listRI = _ReturnInspectionBLL.GetListByWhere(ref pager, "WMS_Part.PartName.Contains(\""
            //    + partName + "\")&& WMS_Part.PartType == \"自制件\" && WMS_Part.PartCode.Contains(\"" + partCode + "\")&& CreateTime>=(\""
            //    + beginDate + "\")&& CreateTime<=(\"" + endDate.AddDays(1) + "\")");

            List<WMS_Product_EntryModel> footerList = new List<WMS_Product_EntryModel>();
            //自制件入库数
            decimal productQty = list.Sum(p => p.ProductQty);
            //自制件退库数
            //decimal returnProductQty = listRI.Sum(p => p.Qty).Value;

            footerList.Add(new WMS_Product_EntryModel()
            {
                PartName = "<div style='text-align:right;color:#444'>合计：</div>",

                ProductQty = productQty,

                Lot = "<div style='text-align:right;color:#444'>退货合计：</div>",

                //InvName = returnProductQty.ToString(),

                //Remark = "<div style='text-align:right;color:#444'>退货率：</div>",

                //CreatePerson = productQty == 0 ? "0": (returnProductQty / productQty).ToString(),
             
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
            WMS_Product_EntryModel model = new WMS_Product_EntryModel()
            {
                ProductBillNum = "RK" + DateTime.Now.ToString("yyyyMMddHHmmssff"),

            };
            return View(model);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WMS_Product_EntryModel model)
        {            
            model.Id = 0;
            model.CreatePerson = GetUserTrueName();
            model.CreateTime = ResultHelper.NowTime;
            if(model.Lot == null || !DateTimeHelper.CheckYearMonth(model.Lot))
            {
                return Json(JsonHandler.CreateMessage(0, "批次录入不符合规范"));
            }
            if (model != null && ModelState.IsValid)
            {
                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ProductBillNum" + model.ProductBillNum, "成功", "创建", "WMS_Product_Entry");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ProductBillNum" + model.ProductBillNum + "," + ErrorCol, "失败", "创建", "WMS_Product_Entry");
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
            WMS_Product_EntryModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_Product_EntryModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ProductBillNum" + model.ProductBillNum, "成功", "修改", "WMS_Product_Entry");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",ProductBillNum" + model.ProductBillNum + "," + ErrorCol, "失败", "修改", "WMS_Product_Entry");
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
            WMS_Product_EntryModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_Product_Entry");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_Product_Entry");
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
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Product_Entry", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Product_Entry", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_Product_EntryModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
        public ActionResult Export(string partName, string partCode, DateTime beginDate, DateTime endDate)
        {
            //List<WMS_Product_EntryModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            List<WMS_Product_EntryModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "WMS_Part.PartName.Contains(\""
                + partName + "\")&& WMS_Part.PartCode.Contains(\"" + partCode + "\")&& CreateTime>=(\""
                + beginDate + "\")&& CreateTime<=(\"" + endDate.AddDays(1) + "\")");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    //jo.Add("Id", item.Id);
                    jo.Add("入库单号（业务）", item.ProductBillNum);
                    jo.Add("入库单号（系统）", item.EntryBillNum);
                    jo.Add("本货部门", item.Department);
                    jo.Add("物料编码", item.Partid);
                    jo.Add("物料名称", item.PartName);
                    jo.Add("数量", item.ProductQty);
                    jo.Add("库房", item.InvName);
                    //jo.Add("子库存", item.SubInvId);
                    jo.Add("备注", item.Remark);
                    //jo.Add("Attr1", item.Attr1);
                    //jo.Add("Attr2", item.Attr2);
                    //jo.Add("Attr3", item.Attr3);
                    //jo.Add("Attr4", item.Attr4);
                    //jo.Add("Attr5", item.Attr5);
                    jo.Add("创建人", item.CreatePerson);
                    jo.Add("创建时间", item.CreateTime);
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
              jo.Add("入库单号（业务）(必输)", "");
              //jo.Add("入库单号（系统）", "");
              jo.Add("本货部门", "");
              jo.Add("物料编码(必输)", "");
              jo.Add("数量(必输)", "");
              jo.Add("批次(格式：YYYY-MM-DD)", "");
              jo.Add("库房(必输)", "");
              //jo.Add("子库存", "");
              jo.Add("备注", "");
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
            var exportFileName = string.Concat("自制件导入模板",
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

