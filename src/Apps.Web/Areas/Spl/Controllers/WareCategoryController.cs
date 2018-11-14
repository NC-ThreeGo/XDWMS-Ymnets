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
using System.Text;
using Apps.Models.Sys;

namespace Apps.Web.Areas.Spl.Controllers
{
    public class WareCategoryController : BaseController
    {
        [Dependency]
        public ISpl_WareCategoryBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        
        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName="Index")]
        public JsonResult GetList(string id)
        {
            if (id == null)
                id = "0";
            List<Spl_WareCategoryModel> list = m_BLL.GetList(id);
            var json = from r in list
                       select new Spl_WareCategoryModel()
                       {
                           Id = r.Id,
                           Name = r.Name,
                           ParentId = r.ParentId,
                           Code = r.Code,
                           Enable = r.Enable,
                           Remark = r.Remark,
                           CreateTime = r.CreateTime,
                           state = (m_BLL.GetList(r.Id).Count > 0) ? "closed" : "open"
                       };


            return Json(json);
        }


        [HttpPost]
        public JsonResult GetListByComTree(string id)
        {
            List<Spl_WareCategoryModel> list = m_BLL.GetList(id);
            var json = from r in list
                       select new SysTreeModel()
                       {
                           id = r.Id,
                           text = r.Name,
                           state = (m_BLL.GetList(r.Id).Count > 0) ? "closed" : "open"
                       };


            return Json(json);
        }

        [HttpPost]
        public JsonResult GetListByParentId(string id)
        {
            if (id == null)
                id = "0";
            List<Spl_WareCategoryModel> list = m_BLL.GetList(id);
            StringBuilder sb = new StringBuilder("");
            foreach (var i in list)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", i.Id, i.Name);
            }

            return Json(sb.ToString());
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create(string id)
        {
            Spl_WareCategoryModel entity = new Spl_WareCategoryModel()
            {
                ParentId = id,
                Enable = true
            };
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(Spl_WareCategoryModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "Spl_WareCategory");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "Spl_WareCategory");
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
            Spl_WareCategoryModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Spl_WareCategoryModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "Spl_WareCategory");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "Spl_WareCategory");
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
            Spl_WareCategoryModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_WareCategory");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_WareCategory");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion
    }
}

