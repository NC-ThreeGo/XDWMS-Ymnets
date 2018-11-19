using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.Sys;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models.Sys;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Apps.Web.Controllers
{
    public class SysSequenceController : BaseController
    {
        [Dependency]
        public ISysSequenceBLL m_BLL { get; set; }
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
            List<SysSequenceModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<SysSequenceModel> grs = new GridRows<SysSequenceModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            IEnumerable<SequenceType> SequenceTypes = (IEnumerable<SequenceType>)Enum.GetValues(typeof(SequenceType));
            ViewBag.SequenceTypes = new SelectList(SequenceTypes);
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysSequenceModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.FirstType = (int)model.FirstTypeEnum;
                model.SecondType = (int)model.SecondTypeEnum;

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",SN" + model.SN, "成功", "创建", "SysSequence");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",SN" + model.SN + "," + ErrorCol, "失败", "创建", "SysSequence");
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
            IEnumerable<SequenceType> SequenceTypes = (IEnumerable<SequenceType>)Enum.GetValues(typeof(SequenceType));
            ViewBag.SequenceTypes = new SelectList(SequenceTypes);
            SysSequenceModel entity = m_BLL.GetById(id);
            entity.FirstTypeEnum = (SequenceType)entity.FirstType;
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysSequenceModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.FirstType = (int)model.FirstTypeEnum;
                model.SecondType = (int)model.SecondTypeEnum;

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",SN" + model.SN, "成功", "修改", "SysSequence");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",SN" + model.SN + "," + ErrorCol, "失败", "修改", "SysSequence");
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
            SysSequenceModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "SysSequence");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "SysSequence");
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
            var list = new List<SysSequenceModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "SysSequence");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "SysSequence");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<SysSequenceModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
            List<SysSequenceModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
                foreach (var item in list)
                {
                    var jo = new JObject();
                    jo.Add("Id", item.Id);
                    jo.Add("SN", item.SN);
                    jo.Add("TabName", item.TabName);
                    jo.Add("FirstType", item.FirstType);
                    jo.Add("FirstRule", item.FirstRule);
                    jo.Add("FirstLength", item.FirstLength);
                    jo.Add("SecondType", item.SecondType);
                    jo.Add("SecondRule", item.SecondRule);
                    jo.Add("SecondLength", item.SecondLength);
                    jo.Add("ThirdType", item.ThirdType);
                    jo.Add("ThirdRule", item.ThirdRule);
                    jo.Add("ThirdLength", item.ThirdLength);
                    jo.Add("FourType", item.FourType);
                    jo.Add("FourRule", item.FourRule);
                    jo.Add("FourLength", item.FourLength);
                    jo.Add("JoinChar", item.JoinChar);
                    jo.Add("Sample", item.Sample);
                    jo.Add("CurrentValue", item.CurrentValue);
                    jo.Add("Remark", item.Remark);
                    jObjects.Add(jo);
                }
                var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
                var exportFileName = string.Concat(
                    "File",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
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

