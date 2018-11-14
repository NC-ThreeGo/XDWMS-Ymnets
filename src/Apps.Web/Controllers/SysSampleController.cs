using System.Collections.Generic;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Sys;
using Unity.Attributes;
using Apps.Web.Core;
using Apps.Locale;
using Apps.IBLL.Sys;
using System;
using System.Linq;
//using Microsoft.Reporting.WebForms;

namespace Apps.Web.Controllers
{
    public class SysSampleController : BaseController
    {

        /// <summary>
        /// 业务层注入
        /// </summary>
        [Dependency]
        public ISysSampleBLL m_BLL { get; set; }
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
            List<SysSampleModel> list = m_BLL.GetList(ref pager, queryStr);

            GridRows<SysSampleModel> grs = new GridRows<SysSampleModel>();
            grs.rows = permModel.SetDataTransparent(list, Request.FilePath);//启用数据过滤
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
        public JsonResult Create(SysSampleModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "SysSample");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "SysSample");
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
        public ActionResult Edit(string id)
        {

            SysSampleModel entity = m_BLL.GetById(id);
            //启用数据过滤
            entity = permModel.SetSingleDataTransparent(entity, Request.FilePath);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysSampleModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                //启用数据过滤
                model = permModel.SetSingleDataTransparent(model, m_BLL.GetById(model.Id), Request.FilePath);
                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.Name, "成功", "修改", "样例程序");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.Name + "," + ErrorCol, "失败", "修改", "样例程序");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":" + ErrorCol));
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
        public ActionResult Details(string id)
        {
            SysSampleModel entity = m_BLL.GetById(id);
            //启用数据过滤
            entity = permModel.SetSingleDataTransparent(entity, Request.FilePath);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(SysSampleModel model)
        {
            if (model != null)
            {
                if (m_BLL.Delete(ref errors, model.Id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Name:" + model.Name, "成功", "删除", "样例程序");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Name:" + model.Name + "," + ErrorCol, "失败", "删除", "样例程序");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion

        #region 导出到PDF EXCEL WORD
        //public ActionResult Reporting(string type = "PDF", string queryStr = "", int rows = 0, int page = 1)
        //{
        //    //选择了导出全部
        //    if (rows == 0 && page == 0)
        //    {
        //        rows = 9999999;
        //        page = 1;
        //    }
        //    GridPager pager = new GridPager()
        //    {
        //        rows = rows,
        //        page = page,
        //        sort="Id",
        //        order="desc"
        //    };
        //    List<SysSampleModel> ds = m_BLL.GetList(ref pager, queryStr);
        //    LocalReport localReport = new LocalReport();

        //    localReport.ReportPath = Server.MapPath("~/Reports/SysSampleReport.rdlc");

        //    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ds);
        //    localReport.DataSources.Add(reportDataSource);
        //    string reportType = type;
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =
        //        "<DeviceInfo>" +
        //        "<OutPutFormat>" + type + "</OutPutFormat>" +
        //        "<PageWidth>11in</PageWidth>" +
        //        "<PageHeight>11in</PageHeight>" +
        //        "<MarginTop>0.5in</MarginTop>" +
        //        "<MarginLeft>1in</MarginLeft>" +
        //        "<MarginRight>1in</MarginRight>" +
        //        "<MarginBottom>0.5in</MarginBottom>" +
        //        "</DeviceInfo>";
        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;

        //    renderedBytes = localReport.Render(
        //        reportType,
        //        deviceInfo,
        //        out mimeType,
        //        out encoding,
        //        out fileNameExtension,
        //        out streams,
        //        out warnings
        //        );
        //    return File(renderedBytes, mimeType);
        //}
        #endregion
    }
}
