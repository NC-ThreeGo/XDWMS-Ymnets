using Apps.Web.Core;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Apps.Web.Areas.Report.Controllers
{
    public class ReportManagerController : Controller
    {
        // GET: Report/ReportManager
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 报表设计
        /// </summary>
        /// <returns></returns>
        public ActionResult Designer()
        {
            string ReportNum = string.Empty;
            WebReport webReport = new WebReport();
            webReport.Width = Unit.Percentage(100);
            webReport.Height = 600;
            webReport.ToolbarIconsStyle = ToolbarIconsStyle.Black;
            webReport.ToolbarIconsStyle = ToolbarIconsStyle.Black;
            webReport.PrintInBrowser = true;
            webReport.PrintInPdf = true;
            webReport.ShowExports = true;
            webReport.ShowPrint = true;
            webReport.SinglePage = true;
            DataSet ds = null;
            //ds = new ReportProvider().GetDataSource(entity, list, orderType, "");
            string path = Server.MapPath("~/ReportFiles/" + "入库单打印模板.frx");
            //if (!FileManager.FileExists(path))
            //{
            //    string template = Server.MapPath("~/ReportFiles/Temp/Report.frx");
            //    System.IO.File.Copy(template, path, true);
            //}
            webReport.Report.Load(path);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                webReport.Report.RegisterData(ds);
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    webReport.Report.GetDataSource(ds.Tables[i].TableName).Enabled = true;
                }
            }
            webReport.DesignerPath = "~/WebReportDesigner/index.html";
            webReport.DesignReport = true;
            webReport.DesignScriptCode = true;
            webReport.DesignerSavePath = "~/ReportFiles/Temp/";
            webReport.DesignerSaveCallBack = "~/Report/ReportManager/SaveDesignedReport";
            webReport.ID = ReportNum;

            ViewBag.WebReport = webReport;
            return View();
        }

        /// <summary>
        /// 保存报表设计回调函数
        /// </summary>
        /// <param name="reportID"></param>
        /// <param name="reportUUID"></param>
        /// <returns></returns>
        public ActionResult SaveDesignedReport(string reportID, string reportUUID)
        {
            //ReportProvider provider = new ReportProvider();
            //if (reportID.IsEmpty())
            //{
            //    return Redirect("/Report/Manager/List");
            //}
            //ReportsEntity entity = provider.GetReport(reportID);
            //if (entity.IsNull())
            //{
            //    return Redirect("/Report/Manager/List");
            //}
            //string FileRealPath = Server.MapPath("~" + entity.FileName);
            //string FileTempPath = Server.MapPath("~/Theme/content/report/temp/" + reportUUID);
            //FileManager.DeleteFile(FileRealPath);
            //System.IO.File.Copy(FileTempPath, FileRealPath, true);
            return Content("");
        }
    }
}