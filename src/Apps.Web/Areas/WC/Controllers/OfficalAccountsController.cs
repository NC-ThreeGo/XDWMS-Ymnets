using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.WC;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.WC;
using Unity.Attributes;
using Apps.Models.Enum;
using System;

namespace Apps.Web.Areas.WC.Controllers
{
    public class OfficalAccountsController : BaseController
    {
        [Dependency]
        public IWC_OfficalAccountsBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            WC_OfficalAccountsModel model = m_BLL.GetCurrentAccount();
            ViewBag.CurrentOfficalAcount = model.OfficalName;
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<WC_OfficalAccountsModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<WC_OfficalAccountsModel> grs = new GridRows<WC_OfficalAccountsModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult SetDefault(string id)
        {
            if (id != null)
            {
                if (m_BLL.SetDefault(id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id, "成功", "设置默认", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id, "失败", "设置默认", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WC_OfficalAccountsModel model)
        {
            model.Id = ResultHelper.NewId;
            //企业号的URL不与普通的一致
            if (model.Category == (int)WeChatSubscriberEnum.EnterpriseSubscriber)
            {
                model.ApiUrl = WebConfigPara.SiteConfig.WeChatQYApiUrl + model.Id;
            }
            else
            {
                model.ApiUrl = WebConfigPara.SiteConfig.WeChatApiUrl + model.Id;
            }
            model.CreateTime = ResultHelper.NowTime;
            model.CreateBy = GetUserId();
            model.ModifyTime = ResultHelper.NowTime;
            model.ModifyBy = GetUserId();
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    if (model.IsDefault)
                    {
                        m_BLL.SetDefault(model.Id);
                    }
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OfficalName" + model.OfficalName, "成功", "创建", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OfficalName" + model.OfficalName + "," + ErrorCol, "失败", "创建", "WC_OfficalAccounts");
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
            
            WC_OfficalAccountsModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WC_OfficalAccountsModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.ModifyTime = ResultHelper.NowTime;
                model.ModifyBy = GetUserId();
                model.ApiUrl = WebConfigPara.SiteConfig.WeChatApiUrl + model.Id;
                if (m_BLL.Edit(ref errors, model))
                {
                    if (model.IsDefault)
                    {
                        m_BLL.SetDefault(model.Id);
                    }
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OfficalName" + model.OfficalName, "成功", "修改", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OfficalName" + model.OfficalName + "," + ErrorCol, "失败", "修改", "WC_OfficalAccounts");
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
            
            WC_OfficalAccountsModel entity = m_BLL.GetById(id);
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
                WC_OfficalAccountsModel model = m_BLL.GetById(id);
                if (model.IsDefault)
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "删除失败，因为当前公众号为默认公众号", "失败", "删除", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ",因为当前公众号为默认公众号"));
                }

                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "WC_OfficalAccounts");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion
        [HttpPost]
        public JsonResult GetToken()
        {

            List<WC_OfficalAccountsModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            foreach (var model in list)
            {
                try
                {
                    if (model.Category == (int)WeChatSubscriberEnum.EnterpriseSubscriber)
                    {
                        if (!string.IsNullOrEmpty(model.AppId) && !string.IsNullOrEmpty(model.AppSecret))
                        {
                            model.AccessToken = Senparc.Weixin.QY.CommonAPIs.CommonApi.GetToken(model.AppId, model.AppSecret).access_token;
                            model.ModifyTime = ResultHelper.NowTime;
                            m_BLL.Edit(ref errors, model);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.AppId) && !string.IsNullOrEmpty(model.AppSecret))
                        {
                            model.AccessToken = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(model.AppId, model.AppSecret).access_token;
                            model.ModifyTime = ResultHelper.NowTime;
                            m_BLL.Edit(ref errors, model);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return Json(JsonHandler.CreateMessage(1, "成批更新成功"));
        }
    }
}
