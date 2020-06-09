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
    public class CustomerController : BaseController
    {
        [Dependency]
        public IWMS_CustomerBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            if (!String.IsNullOrEmpty(queryStr))
            {
                queryStr = queryStr.Trim();
            }
                List<WMS_CustomerModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<WMS_CustomerModel> grs = new GridRows<WMS_CustomerModel>();
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
        public JsonResult Create(WMS_CustomerModel model)
        {
            model.Id = 0;
            model.CreateTime = ResultHelper.NowTime;            
            model.CreatePerson = GetUserTrueName();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",CustomerCode" + model.CustomerCode, "成功", "创建", "WMS_Customer");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    //string ErrorCol = errors.Error;
                    string ErrorCol = " ：输入错误数据";
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",CustomerCode" + model.CustomerCode + "," + ErrorCol, "失败", "创建", "WMS_Customer");
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
            WMS_CustomerModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WMS_CustomerModel model)
        {
            model.ModifyTime = ResultHelper.NowTime;
            model.ModifyPerson = GetUserTrueName();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",CustomerCode" + model.CustomerCode, "成功", "修改", "WMS_Customer");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + model.Id + ",CustomerCode" + model.CustomerCode + "," + ErrorCol, "失败", "修改", "WMS_Customer");
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
            WMS_CustomerModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id:" + id, "成功", "删除", "WMS_Customer");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserTrueName(), "Id" + id + "," + ErrorCol, "失败", "删除", "WMS_Customer");
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
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Customer", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入成功");
                return Json(JsonHandler.CreateMessage(1,
                   Resource.InsertSucceed + "，记录数：" + Utils.GetRowCount(Utils.GetMapPath(filePath)).ToString(),
                   filePath));
            }
            else
            {
                 LogHandler.WriteImportExcelLog(GetUserTrueName(), "WMS_Customer", filePath.Substring(filePath.LastIndexOf('/') + 1), filePath, "导入失败");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail, filePath));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData(string queryStr)
        {
            List<WMS_CustomerModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
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
            List<WMS_CustomerModel> list = m_BLL.GetList(ref setNoPagerAscById, queryStr);
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    //jo.Add("客户ID", item.Id);
                    jo.Add("客户编码", item.CustomerCode);
                    jo.Add("客户简称", item.CustomerShortName);
                    jo.Add("客户名称", item.CustomerName);
                    jo.Add("客户类型", item.CustomerType);
                    jo.Add("联系人", item.LinkMan);
                    jo.Add("联系电话", item.LinkManTel);
                    jo.Add("联系地址", item.LinkManAddress);
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
              //jo.Add("客户ID", "");
              jo.Add("客户编码(必输)", "");
              jo.Add("客户简称(必输)", "");
              jo.Add("客户名称(必输)", "");
              jo.Add("客户类型", "");
              jo.Add("联系人", "");
              jo.Add("联系电话", "");
              jo.Add("联系地址", "");
              //jo.Add("状态", "");
              jo.Add("说明", "");
            //jo.Add("创建人", "");
            //jo.Add("创建时间", "");
            //jo.Add("修改人", "");
            //jo.Add("修改时间", "");
               jo.Add("导入的错误信息", "");
            jObjects.Add(jo);
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            var exportFileName = string.Concat("客户导入模板",
                    ".xlsx");
                return new ExportExcelResult
                {
                    SheetName = "Sheet1",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
        #endregion

        #region 选择客户
        /// <summary>
        /// 弹出选择客户
        /// </summary>
        /// <param name="mulSelect">是否多选</param>
        /// <returns></returns>
        [SupportFilter(ActionName = "Index")]
        public ActionResult CustomerLookUp(bool mulSelect = false)
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult CustomerGetList(GridPager pager, string customerCode, string customerShortName)
        {
            List<WMS_CustomerModel> list = m_BLL.GetListByWhere(ref pager, "Status == \"有效\" && CustomerCode.Contains(\""
                + customerCode + "\") && CustomerShortName.Contains(\"" + customerShortName + "\")");
            GridRows<WMS_CustomerModel> grs = new GridRows<WMS_CustomerModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetCustomerByCode(string customerCode)
        {
            List<WMS_CustomerModel> list = m_BLL.GetListByWhere(ref setNoPagerAscById, "Status == \"有效\" && CustomerCode == \""
                + customerCode + "\"");
            if (list.Count() == 0)
            {
                return Json(JsonHandler.CreateMessage(0, "客户商编码不存在！"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed, JsonHandler.SerializeObject(list.First())));
            }
        }
        #endregion

        #region 根据物料的所属客户，返回客户列表
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetCustomerByBelong(string codes)
        {
            List<WMS_CustomerModel> list = m_BLL.GetListByBelong(ref setNoPagerAscById, codes);
            return Json(list);
        }
        #endregion
    }
}

