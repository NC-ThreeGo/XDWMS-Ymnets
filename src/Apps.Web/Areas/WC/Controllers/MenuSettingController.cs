using Apps.Common;
using Apps.Locale;
using Apps.Models.WC;
using Apps.IBLL.WC;
using Apps.Web.Core;
using Unity.Attributes;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Apps.Models.Enum;

namespace Apps.Web.Areas.WC.Controllers
{
    public class MenuSettingController : BaseController
    {
        
        [Dependency]
        public IWC_OfficalAccountsBLL account_BLL { get; set; }

        [SupportFilter]
        // GET: WC/MenuSetting
        public ActionResult Index()
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            ViewBag.CurrentOfficalAcount = model;
            GetMenuResult result = new GetMenuResult(new ButtonGroup());

            //初始化
            for (int i = 0; i < 3; i++)
            {
                var subButton = new SubButton();
                for (int j = 0; j < 5; j++)
                {
                    var singleButton = new SingleClickButton();
                    subButton.sub_button.Add(singleButton);
                }
            }

            return View(result);
        }
        [SupportFilter(ActionName ="Index")]
        [HttpGet]
        public JsonResult GetCurrentAcountState()
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            if (string.IsNullOrEmpty(model.AppId) || string.IsNullOrEmpty(model.AppSecret) || string.IsNullOrEmpty(model.AccessToken))
            {
                return Json(JsonHandler.CreateMessage(0, "当前公众号没有编辑按钮功能！"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, model.AccessToken));
            }
           
        }

        [SupportFilter(ActionName = "Edit")]
        [HttpPost]
        public ActionResult CreateMenuQY(Senparc.Weixin.QY.GetMenuResultFull resultFull)
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            string token = model.AccessToken;
            try
            {
                QyJsonResult result = null;
                //普通接口
                Senparc.Weixin.QY.Entities.Menu.ButtonGroup buttonGroup = Senparc.Weixin.QY.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull).menu;
                result = Senparc.Weixin.QY.CommonAPIs.CommonApi.CreateMenu(token,Convert.ToInt32(model.OfficalId), buttonGroup);
              
                var json = new
                {
                    Success = result.errmsg == "ok",
                    Message = "菜单更新成功。普通自定义菜单接口"
                };
                return Json(json);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = string.Format("更新失败：{0}。普通自定义菜单接口", ex.Message) };
                return Json(json);
            }
        }

        [SupportFilter(ActionName = "Edit")]
        [HttpPost]
        public ActionResult CreateMenu( GetMenuResultFull resultFull, MenuMatchRule menuMatchRule)
        {

            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            string token = model.AccessToken;
            var useAddCondidionalApi = menuMatchRule != null && !menuMatchRule.CheckAllNull();
            var apiName = string.Format("使用接口：{0}。", (useAddCondidionalApi ? "个性化菜单接口" : "普通自定义菜单接口"));
            try
            {
                //重新整理按钮信息
                WxJsonResult result = null;
                IButtonGroupBase buttonGroup = null;
                if (useAddCondidionalApi)
                {
                    //个性化接口
                    buttonGroup = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull, new ConditionalButtonGroup()).menu;

                    var addConditionalButtonGroup = buttonGroup as ConditionalButtonGroup;
                    addConditionalButtonGroup.matchrule = menuMatchRule;
                    result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenuConditional(token, addConditionalButtonGroup);
                    apiName += string.Format("menuid：{0}。", (result as CreateMenuConditionalResult).menuid);
                }
                else
                {
                    //普通接口
                    buttonGroup = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
                    result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenu(token, buttonGroup);
                }

                var json = new
                {
                    Success = result.errmsg == "ok",
                    Message = "菜单更新成功。" + apiName
                };
                return Json(json);
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = string.Format("更新失败：{0}。{1}", ex.Message, apiName) };
                return Json(json);
            }
        }
        [SupportFilter(ActionName = "Edit")]
        public ActionResult GetMenu()
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            string token = model.AccessToken;
           
            if (model.Category != (int)WeChatSubscriberEnum.EnterpriseSubscriber)
            {

                var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenu(token);
                if (result == null)
                {
                    return Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = Senparc.Weixin.QY.CommonAPIs.CommonApi.GetMenu(token, Convert.ToInt32(model.OfficalId));
                if (result == null)
                {
                    return Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
           
        }
        [SupportFilter(ActionName = "Delete")]
        public ActionResult DeleteMenu()
        {
            try
            {
                WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
                string token = model.AccessToken;
                if (model.Category != (int)WeChatSubscriberEnum.EnterpriseSubscriber)
                {


                    var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.DeleteMenu(token);
                    var json = new
                    {
                        Success = result.errmsg == "ok",
                        Message = result.errmsg
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = Senparc.Weixin.QY.CommonAPIs.CommonApi.DeleteMenu(token, Convert.ToInt32(model.OfficalId));
                    var json = new
                    {
                        Success = result.errmsg == "ok",
                        Message = result.errmsg
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}