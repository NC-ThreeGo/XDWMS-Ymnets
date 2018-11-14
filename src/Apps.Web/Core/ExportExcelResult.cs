using ClosedXML.Excel;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Apps.Web.Core
{
    public class ExportExcelResult : ActionResult
    {
        public string SheetName { get; set; }
        public string FileName { get; set; }
        public DataTable ExportData { get; set; }

        public XLWorkbook Workbook { get;set;}

        public ExportExcelResult()
        {

        }

        public ExportExcelResult(XLWorkbook wb)
        {
            this.Workbook = wb;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (ExportData == null && Workbook==null)
            {
                throw new InvalidDataException("ExportData");
            }
            if (string.IsNullOrWhiteSpace(this.SheetName))
            {
                this.SheetName = "Sheet1";
            }
            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                this.FileName = string.Concat(
                    "ExportData_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx");
            }

            this.ExportExcelEventHandler(context);
        }

        /// <summary>
        /// Exports the excel event handler.
        /// </summary>
        /// <param name="context">The context.</param>
        private void ExportExcelEventHandler(ControllerContext context)
        {
            try
            {
                
                var workbook = new XLWorkbook();
                if (Workbook != null)
                {
                    workbook = Workbook;
                }
                if (this.ExportData != null || this.Workbook!=null)
                {
                    context.HttpContext.Response.Clear();

                    // 编码
                    context.HttpContext.Response.ContentEncoding = Encoding.UTF8;

                    // 设置网页ContentType
                    context.HttpContext.Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // 导出名字
                    var browser = context.HttpContext.Request.Browser.Browser;
                    var exportFileName = browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase)
                        ? this.FileName
                        : HttpUtility.UrlEncode(this.FileName, Encoding.UTF8);

                    context.HttpContext.Response.AddHeader(
                        "Content-Disposition",
                        string.Format("attachment;filename={0}", exportFileName));

                    // Add all DataTables in the DataSet as a worksheets
                    if (ExportData != null)
                        workbook.Worksheets.Add(this.ExportData, this.SheetName);

                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(context.HttpContext.Response.OutputStream);
                        memoryStream.Close();
                    }
                }
                workbook.Dispose();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}