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
using Senparc.Weixin.MP.AdvancedAPIs.User;
using System.Threading.Tasks;

namespace Apps.Web.Areas.WC.Controllers
{
    public class UserController : BaseController
    {
        [Dependency]
        public IWC_UserBLL m_BLL { get; set; }
        [Dependency]
        public IWC_OfficalAccountsBLL account_BLL { get; set; }
        [Dependency]
        public IWC_GroupBLL group_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            ViewBag.CurrentOfficalAcount = model.OfficalName;
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<WC_UserModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<WC_UserModel> grs = new GridRows<WC_UserModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(WC_UserModel model)
        {
            model.Id = ResultHelper.NewId;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OpenId" + model.OpenId, "成功", "创建", "WC_User");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OpenId" + model.OpenId + "," + ErrorCol, "失败", "创建", "WC_User");
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
            ViewBag.Perm = GetPermission();
            WC_UserModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(WC_UserModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OpenId" + model.OpenId, "成功", "修改", "WC_User");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OpenId" + model.OpenId + "," + ErrorCol, "失败", "修改", "WC_User");
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
            ViewBag.Perm = GetPermission();
            WC_UserModel entity = m_BLL.GetById(id);
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
                string[] arrs = id.Split(',');
                if (m_BLL.Delete(ref errors, arrs))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Ids:" + id, "成功", "删除", "WC_User");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Ids" + id + "," + ErrorCol, "失败", "删除", "WC_User");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion

        #region 同步
        [HttpPost]
        [SupportFilter]
        public JsonResult SyncUser(string id,string officeId)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                //填充数据
                string[] arrs = id.Split(',');
                List<BatchGetUserInfoData> list = new List<BatchGetUserInfoData>();
                foreach (var m in arrs)
                {
                    list.Add(new BatchGetUserInfoData() {
                        openid = m
                    });
                }

                //批量同步数据
                WC_OfficalAccountsModel accountModel =  account_BLL.GetById(officeId);
                var batchList =  Senparc.Weixin.MP.AdvancedAPIs.UserApi.BatchGetUserInfo(accountModel.AccessToken, list);
                foreach (var info in batchList.user_info_list)
                {
                    WC_UserModel userModel = m_BLL.GetById(info.openid);
                    if (userModel != null)
                    {
                        userModel.City = info.city;
                        userModel.OpenId = info.openid;
                        userModel.Id = info.openid;
                        userModel.HeadImgUrl = info.headimgurl;
                        userModel.Language = info.language;
                        userModel.NickName = info.nickname;
                        userModel.Province = info.province;
                        userModel.Sex = info.sex;
                        m_BLL.Edit(ref errors, userModel);
                    }
                }

                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + id, "成功", "删除", "WC_User");
                return Json(JsonHandler.CreateMessage(1, Resource.SaveSucceed));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.SaveFail));
            }


        }
        #endregion

        #region 批量移动用户分组 本地

        [SupportFilter(ActionName = "Edit")]
        public ActionResult MoveUser(string id)
        {
            ViewBag.UserIds = id.ToString();
            WC_OfficalAccountsModel model = account_BLL.GetCurrentAccount();
            List<WC_GroupModel> list = group_BLL.GetList(model.Id);
            return View(list);
        }
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public ActionResult MoveUser(string userids, string groupid)
        {
            WC_OfficalAccountsModel wcmodel = account_BLL.GetCurrentAccount();
            List<string> itemstr = userids.Split(',').ToList();
            foreach (var item in itemstr)
            {
                ViewBag.Perm = GetPermission();
                WC_UserModel model = m_BLL.GetById(item);
                model.GroupId = groupid;
                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OpenId" + model.OpenId, "成功", "修改", "WC_User");

                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",OpenId" + model.OpenId + "," + ErrorCol, "失败", "修改", "WC_User");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));

                }
            }
            return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
        }

        #endregion
    }
}
