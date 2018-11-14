using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.Spl;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Spl;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System;
using ClosedXML.Excel;
using System.IO;

namespace Apps.Web.Areas.Spl.Controllers
{
    public class PersonController : BaseController
    {
        [Dependency]
        public ISpl_PersonBLL m_BLL { get; set; }
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
            List<Spl_PersonModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<Spl_PersonModel> grs = new GridRows<Spl_PersonModel>();
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
        public JsonResult Create(Spl_PersonModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "Spl_Person");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "Spl_Person");
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
            
            Spl_PersonModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Spl_PersonModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "Spl_Person");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "Spl_Person");
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
        public ActionResult Details(string id)
        {
            
            Spl_PersonModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_Person");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_Person");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion

        #region 导入

        [HttpPost]
        [SupportFilter]
        public ActionResult Import(string filePath,int flag)
        {
            var personList = new List<Spl_PersonModel>();
            bool checkResult = false;
            //校验数据
            if (flag == 0)
            {
                checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), personList, ref errors);
            }
            else
            {
                checkResult = m_BLL.CheckImportBatchData(Utils.GetMapPath(filePath), personList, ref errors);
            }
             //校验通过直接保存
             if (checkResult)
             {
                 m_BLL.SaveImportData(personList);
                 LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "Spl_Person");
                 return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
             }
             else
             {
                 string ErrorCol = errors.Error;
                 LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "Spl_Person");
                 return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
              }
        
        }

        [HttpPost]
        [SupportFilter(ActionName = "Import")]
        public JsonResult ImportBefor(string filePath, int flag)
        {
            var personList = new List<Spl_PersonModel>();
            bool checkResult = false;
            //校验数据
            if (flag == 0)
            {
                checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), personList, ref errors);
            }
            else
            {
                checkResult = m_BLL.CheckImportBatchData(Utils.GetMapPath(filePath), personList, ref errors);
            }
            //校验通过返回数据
            if (checkResult)
            {
                GridRows<Spl_PersonModel> grs = new GridRows<Spl_PersonModel>();
                grs.rows = personList;
                grs.total = personList.Count();
                return Json(grs);
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "Spl_Person");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }

        }
        [HttpPost]
        [ValidateInput(false)]
        [SupportFilter(ActionName = "Import")]
        public JsonResult SaveBefor(string inserted)
        {
            var personList = JsonHandler.DeserializeJsonToList<Spl_PersonModel>(inserted);
            var personResultList = new List<Spl_PersonModel>();
            //新加
            if (personList != null && personList.Count != 0)
            {
                foreach (var model in personList)
                {
                    Spl_PersonModel entity = new Spl_PersonModel();
                    entity.Id = ResultHelper.NewId;
                    entity.Name = model.Name;
                    entity.Sex = model.Sex;
                    entity.Age = model.Age;
                    entity.IDCard = model.IDCard;
                    entity.Phone = model.Phone;
                    entity.Email = model.Email;
                    entity.Address = model.Address;
                    entity.CreateTime = ResultHelper.NowTime;
                    entity.Region = model.Region;
                    entity.Category = model.Category;
                    personResultList.Add(entity);
                }

            }
            try
            {
                m_BLL.SaveImportData(personResultList);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "Spl_Person");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            catch (Exception ex){
                LogHandler.WriteServiceLog(GetUserId(), ex.Message, "失败", "导入", "Spl_Person");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail+ ex.Message));
            }
          
        }


        [HttpPost]
        [SupportFilter(ActionName="Export")]
        public JsonResult CheckExportData()
        {
            List<Spl_PersonModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
            var exportSpource = this.GetExportData();
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

            var exportFileName = string.Concat(
                "Person",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xlsx");

            return new ExportExcelResult
            {
                SheetName = "人员列表",
                FileName = exportFileName,
                ExportData = dt
            };
        }
        public ActionResult ExportHard()
        {
            //模拟数据库赋值，一个班级对应多个学生
            SchoolClass model = new SchoolClass();
            model.Id = "CLS0001";
            model.Name = "三年二班";
            model.Manager = "黄SIR";
            model.PhoneNumber = "13800138000";
            model.Manager2 = "李SIR";
            model.PhoneNumber2 = "13888138666";
            model.Remark = "这是一段有很多个字的班级说明，只有足够长的字，才能证明这段文字很长，如果100个字还不够长，那么就再来100个字！";
            model.stuList = new List<Students>();
            model.stuList.Add(new Students() { Id = "STU0001", Name = "牛掰掰", Sex = "男", Age = "23", Point = "80", PhoneNumber = "13545678547" });
            model.stuList.Add(new Students() { Id = "STU0002", Name = "张三", Sex = "女", Age = "23", Point = "70", PhoneNumber = "13545654874" });
            model.stuList.Add(new Students() { Id = "STU0003", Name = "李四", Sex = "女", Age = "25", Point = "50", PhoneNumber = "13545633552" });
            model.stuList.Add(new Students() { Id = "STU0004", Name = "王五", Sex = "男", Age = "22", Point = "66", PhoneNumber = "13566885541" });
            model.stuList.Add(new Students() { Id = "STU0005", Name = "林蛋大", Sex = "男", Age = "26", Point = "95", PhoneNumber = "13821298458" });
            model.stuList.Add(new Students() { Id = "STU0006", Name = "刘丽丽", Sex = "女", Age = "19", Point = "95", PhoneNumber = "13821298458" });



            var wb = new XLWorkbook();
            //--Adding a worksheet
            var ws = wb.Worksheets.Add("班级");
            //--Adding text
            //表头
            ws.Cell("A1").Value = model.Name + "班级信息表";

            //标题
            ws.Cell("A2").Value = "班级代号";
            ws.Cell("B2").Value = "班级名称";
            ws.Cell("C2").Value = "班主任";
            ws.Cell("D2").Value = "联系电话";
            ws.Cell("E2").Value = "副班主任";
            ws.Cell("F2").Value = "联系电话";
            //主表内容
            ws.Cell("A3").Value = model.Id;
            ws.Cell("B3").Value = model.Name;
            ws.Cell("C3").Value = model.Manager;
            ws.Cell("D3").Value = model.PhoneNumber;
            ws.Cell("E3").Value = model.Manager2;
            ws.Cell("F3").Value = model.PhoneNumber2;
            ws.Cell("A4").Value = model.Remark;//说明

            //明细表标题
            ws.Cell("A5").Value = "学号";
            ws.Cell("B5").Value = "姓名";
            ws.Cell("C5").Value = "性别";
            ws.Cell("D5").Value = "年龄";
            ws.Cell("E5").Value = "得分";
            ws.Cell("F5").Value = "电话号码";



            for (int i = 0; i < model.stuList.Count(); i++)
            {
                ws.Cell(i + 6, 1).Value = model.stuList[i].Id;
                ws.Cell(i + 6, 2).Value = model.stuList[i].Name;
                ws.Cell(i + 6, 3).Value = model.stuList[i].Sex;
                ws.Cell(i + 6, 4).Value = model.stuList[i].Age;
                ws.Cell(i + 6, 5).Value = model.stuList[i].PhoneNumber;
                ws.Cell(i + 6, 6).Value = model.stuList[i].PhoneNumber;
            }

            var rngTable = ws.Range("A1:F" + (model.stuList.Count() + 5));

            //合并表头
            var rngHeaders = rngTable.Range("A1:F1");
            ws.Row(1).Height = 20;
            rngHeaders.FirstCell().Style
             .Font.SetBold()
             .Fill.SetBackgroundColor(XLColor.Buff)
             .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //--Merge title cells
            rngHeaders.FirstRow().Merge();

            //第二行表头样式
            rngHeaders = rngTable.Range("A2:F2"); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            //rngHeaders.Style.Font.FontColor = XLColor.DarkBlue;
            rngHeaders.Style.Fill.BackgroundColor = XLColor.Aqua;


            //合并说明
            var rngRemark = rngTable.Range("A4:F4");
            ws.Row(4).Height = 30;
            rngRemark.Style.Alignment.WrapText = true;
            rngRemark.FirstCell().Comment.Style.Size.SetAutomaticSize();

            rngRemark.FirstRow().Merge();

            rngTable = ws.Range("A5:F" + (model.stuList.Count() + 5));
            var excelTable = rngTable.CreateTable();

            //--Adjust column widths to their content
            ws.Columns().AdjustToContents(); // You can also specify the range of columns to adjust, e.g.
                                             // ws.Columns(2, 6).AdjustToContents(); or ws.Columns("2-6").AdjustToContents();
                                             //wb.SaveAs("ExcelSample.xlsx");
                                             //如果文件太大，建议保存后通知用户下载，这是通过保存的形式，如果很小就直接用流的方式输出到下载就可以了

            //Console.WriteLine("Total bytes = " + memoryStream.ToArray().Length);
            var exportFileName = string.Concat(
                    "ExcelSample",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx");

            return new ExportExcelResult(wb)
            {
                SheetName = "人员列表",
                FileName = exportFileName,
                Workbook = wb
            };

        }

        private JArray GetExportData()
        {
            List<Spl_PersonModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();

            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("Name", item.Name);
                jo.Add("Sex", item.Sex);
                jo.Add("Age", item.Age);
                jo.Add("IDCard", item.IDCard);
                jo.Add("Phone", item.Phone);
                jo.Add("Email", item.Email);
                jo.Add("Address", item.Address);
                jo.Add("CreateTime", item.CreateTime);
                jo.Add("Region", item.Region);
                jo.Add("Category", item.Category);
                jObjects.Add(jo);
            }
            return jObjects;
        }
        #endregion
    }


    public class SchoolClass
    {

        public string Id { get; set; }//班级标示
        public string Name { get; set; }//班级名称
        public string Manager { get; set; }//班主任姓名
        public string Manager2 { get; set; }//副班主任姓名
        public string PhoneNumber { get; set; }//班主任联系电话
        public string PhoneNumber2 { get; set; }//副主任联系电话
        public string Remark { get; set; } //班级说明
        public List<Students> stuList { get; set; }//一个班级对应多个学生
    }

    public class Students
    {
        public string Id { get; set; }//学号
        public string Name { get; set; }//姓名
        public string Sex { get; set; }//性别
        public string Age { get; set; }//年龄
        public string Point { get; set; }//年度得分
        public string PhoneNumber { get; set; }//电话
    }
}
