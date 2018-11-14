using Apps.Common;
using Apps.Locale;
using Apps.Models;
using Apps.Models.Enum;
using Apps.Models.WC;
using Apps.IBLL.WC;
using Apps.Web.Core;
using Unity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apps.Web.Areas.WC.Controllers
{
    public class MessageResponseController : BaseController
    {
        [Dependency]
        public IWC_OfficalAccountsBLL account_BLL { get; set; }
        [Dependency]
        public IWC_MessageResponseBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();
        // GET: WC/MessageResponse
        [SupportFilter]
        public ActionResult Index()
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            ViewBag.CurrentOfficalAcount = model.OfficalName;
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult PostData(WC_MessageResponseModel model)
        {
            WC_OfficalAccountsModel accountModel = account_BLL.GetCurrentAccount();
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = ResultHelper.NewId;
            }
            
            model.CreateBy = GetUserId();
            model.CreateTime = ResultHelper.NowTime;
            model.ModifyBy = GetUserId();
            model.ModifyTime = ResultHelper.NowTime;
            model.OfficalAccountId = accountModel.Id;
            model.Enable = true;
            model.IsDefault = true;
            if (m_BLL.PostData(ref errors, model))
            {
                LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OfficalAccountId" + model.OfficalAccountId, "成功", "保存", "WC_MessageResponse");
                return Json(JsonHandler.CreateMessage(1, Resource.SaveSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OfficalAccountId" + model.OfficalAccountId + "," + ErrorCol, "失败", "保存", "WC_MessageResponse");
                return Json(JsonHandler.CreateMessage(0, Resource.SaveFail + ErrorCol));
            }

        }
        /// <summary>
        /// 获得自动回复的列表
        /// </summary>
        /// <param name="pager">分页</param>
        /// <param name="queryStr">查询条件</param>
        /// <param name="messageRule">类型</param>
        /// <param name="matchKey">关键字</param>
        /// <param name="category">规则</param>
        /// <returns></returns>
        public JsonResult GetList(GridPager pager, string queryStr, int messageRule,string matchKey,int? category=0)
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            //组合查询条件
            var predicate = PredicateBuilder.True<WC_MessageResponse>();
            predicate = predicate.And(x => string.Equals(x.OfficalAccountId, model.Id, StringComparison.OrdinalIgnoreCase));
            predicate = predicate.And(x => x.MessageRule == messageRule);

            if (category != 0)
            {
                predicate = predicate.And(x => x.Category == category);
            }

            if (!string.IsNullOrEmpty(matchKey))
            {
                predicate = predicate.And(x => x.MatchKey == matchKey);
            }

            List<WC_MessageResponseModel> list = m_BLL.GetList(ref pager, predicate, queryStr);
            GridRows<WC_MessageResponseModel> grs = new GridRows<WC_MessageResponseModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }


        public JsonResult GetListProperty(GridPager pager, int messageRule)
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            //组合查询条件
            var predicate = PredicateBuilder.True<WC_MessageResponse>();
            predicate = predicate.And(x => string.Equals(x.OfficalAccountId, model.Id, StringComparison.OrdinalIgnoreCase));
            predicate = predicate.And(x => x.MessageRule == messageRule);

  
            List<WC_MessageResponseModel> list = m_BLL.GetListProperty(ref pager, predicate);
            GridRows<WC_MessageResponseModel> grs = new GridRows<WC_MessageResponseModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }


        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "WC_MessageResponse");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "WC_MessageResponse");
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