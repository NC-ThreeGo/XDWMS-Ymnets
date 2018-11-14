using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Unity.Attributes;
using Apps.Common;
using Apps.Web.Core;
using Apps.Models;
using Apps.IBLL;
using System.Text;
using Apps.IBLL.MIS;
using Apps.BLL.MIS;
using Apps.Models.MIS;
using Apps.Models.Sys;
using Apps.Core.OnlineStat;
using Apps.BLL;
using Apps.Locale;
using Apps.IBLL.Sys;
using Apps.BLL.Sys;

namespace Apps.Web.Areas.Mis.Controllers
{
    public class WebIMController : BaseController
    {
        //
        // GET: /Mis/MIS_WebIM_Message/
        [Dependency]
        public ISysStructBLL structBLL { get; set; }
        [Dependency]
        public ISysUserBLL sysUserBLL { get; set; }
        [Dependency]
        public IMIS_WebIM_MessageBLL messageBLL { get; set; }
        [Dependency]
        public IMIS_WebIM_RecentContactBLL recentContactBLL { get; set; }

        [Dependency]
        public IMIS_WebIM_CommonTalkBLL commonTalkBLL { get; set; }

        private SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));


        ValidationErrors errors = new ValidationErrors();
        /// <summary>
        /// IM首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //获取真实姓名
            ViewBag.UserName = GetUserTrueName();

            ViewBag.MIS0002 = siteConfig.refreshcurrentwin;
            ViewBag.MIS0003 = siteConfig.refreshrecentcontact;
            ViewBag.MIS0004 = siteConfig.refreshnewmessage;
            return View();
        }

        public ActionResult ChatLog()
        {
            return View();
        }
        /// <summary>
        /// 获取聊天记录
        /// </summary>
        [HttpPost]
        public JsonResult GetChatLogList(GridPager pager, string queryStr)
        {
            var list = messageBLL.GetMessage(ref pager, GetUserId());
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new MIS_WebIM_MessageModel()
                        {

                            Id = r.Id,
                            Message = r.Message,
                            Sender = r.Sender,
                            receiver = r.receiver,
                            State = r.State,
                            SendDt = r.SendDt,
                            receiverTitle = r.receiverTitle

                        }).ToArray()

            };

            return Json(json);
        }
        // 删除 
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            string returnValue = string.Empty;
            string[] deleteId = ids.Split(',');
            if (deleteId != null && deleteId.Length > 0)
            {


                if (messageBLL.DeleteMessageByUserId(ref errors, GetUserId(), deleteId))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Ids:" + ids, "成功", "删除", "WebIM消息记录");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + ids + "," + ErrorCol, "失败", "删除", "WebIM消息记录");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(returnValue);
        }
        /// <summary>
        /// 获取全部人员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="checkState"></param>
        /// <returns></returns>
        public JsonResult GetTreeByEasyui(string id)
        {
            OnlineUserRecorder recorder = HttpContext.Cache[OnlineHttpModule.g_onlineUserRecorderCacheKey] as OnlineUserRecorder;
            List<SysStructModel> list = structBLL.GetList(id);
            List<SysUser> userList = sysUserBLL.GetListByDepId(id);
            if (userList.Count > 0)
            {
                foreach (var user in userList)
                {
                    SysStructModel addUserToStruct = new SysStructModel();
                    addUserToStruct.Id = user.Id;
                    addUserToStruct.Name = user.TrueName;
                    addUserToStruct.Type = "member";
                    list.Insert(0, addUserToStruct);//把人员加进组
                }
            }
            var json = from m in list
                       select new SysStructUserModel()
                       {
                           id = m.Id,
                           text = "<input id=\"" + m.Id + "\" class=\"" + (m.Type == "group" ? "CBGroup" : "CBMember") + "\" type=\"checkbox\" ref=\"" + id + "\" value=\"" + m.Id + "\" />" + m.Name,     //text
                           attributes = m.Name,
                           iconCls = m.Type == "group" ? "fa fa-users color-green" : SetOnlineIcon(m.Id, recorder),
                           state = m.Type == "group" ? "closed" : "open"

                       };
            return Json(json);

        }
        //判断用户是否在线
        public string SetOnlineIcon(string id, OnlineUserRecorder recorder)
        {
            if (recorder == null)
                return "fa fa-user color-gray";
            // 绑定在线用户列表
            IList<OnlineUser> userList = recorder.GetUserList();
            bool b = false;
            foreach (var OnlineUser in userList)
            {
                if (id == OnlineUser.UserName)
                {
                    b = true;
                }
            }
            if (b)
            {
                return "fa fa-user color-green";
            }
            else
            {
                return "fa fa-user color-gray";
            }
        }


    

        /// <summary>
        /// 获取常用语
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommonTalk()
        {
            GridPager pager = new GridPager()
            {
                rows = 1000,
                page = 1,
                order = "desc",
                sort = "Id"

            };
            List<MIS_WebIM_CommonTalkModel> list = commonTalkBLL.GetList(ref pager, "");
            StringBuilder sb = new StringBuilder();
            foreach (MIS_WebIM_CommonTalkModel m in list)
            {
                sb.AppendFormat(" <li><a href=\"javascript:SetCommmonTalk('{0}')\">{1}</a></li>", m.Talk, m.Talk);
            }
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送新信息
        /// </summary>
        /// <param name="mes">信息内容</param>
        /// <param name="receiver">接收人列表</param>
        /// <param name="receiverName">接收人名称列表</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SendMessage(string mes, string receiver, string receiverName, bool state)
        {
            if (GetUserId() == null)
                return Json(false);
            MIS_WebIM_MessageModel model = new MIS_WebIM_MessageModel()
            {
                Id = ResultHelper.NewId,
                Message = mes,
                Sender = GetUserId(),
                SenderTitle = GetUserTrueName(),
                receiver = receiver,
                receiverTitle = receiverName,
                State = state,
                SendDt = DateTime.Now
            };
            string ErrorCol = string.Empty;
            if (messageBLL.CreateMessage(ref errors, model))
            {
                //开关，从config配置此功能
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id, "成功", "创建", "WebIM发送消息");
                return Json(true);
            }
            else
            {
                ErrorCol = errors.Error;
                if (!string.IsNullOrEmpty(ErrorCol))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ErrorCol, "失败", "创建", "WebIM发送消息");//写入日志                      

                }
                return Json(false);//提示插入失败
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SendMessageGetId(string mes, string receiver, string receiverName, bool state)
        {
            if (GetUserId() == null)
                return Json(false);
            string NewId = ResultHelper.NewId;
            MIS_WebIM_MessageModel model = new MIS_WebIM_MessageModel()
            {
                Id = NewId,
                Message = mes,
                Sender = GetUserId(),
                SenderTitle = GetUserTrueName(),
                receiver = receiver,
                receiverTitle = receiverName,
                State = state,
                SendDt = DateTime.Now
            };
            string ErrorCol = string.Empty;
            if (messageBLL.CreateMessage(ref errors, model))
            {
                //开关，从config配置此功能
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id, "成功", "创建", "WebIM发送消息");
                return Json(NewId);
            }
            else
            {
                ErrorCol = errors.Error;
                if (!string.IsNullOrEmpty(ErrorCol))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ErrorCol, "失败", "创建", "WebIM发送消息");//写入日志                      

                }
                return Json("");//提示插入失败
            }
        }



        /// <summary>
        /// 获取当前聊天人发给我的的最新信息
        /// </summary>
        /// <param name="receiver">对话人</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChatContent(string memberId)
        {
            StringBuilder sb = new StringBuilder("");
            GridPager gridModel = new GridPager()
            {
                rows = 1000,
                page = 1,
                totalRows = 0
            };
            List<MIS_WebIM_MessageRecModel> recList = messageBLL.GetMessageFromSenderToReceiver(ref gridModel, memberId, GetUserId(), false);
            foreach (MIS_WebIM_MessageRecModel model in recList)
            {
                sb.AppendFormat("<p class='tit'><strong>[{0}]</strong> <span>{1}</span>：</p><p>{2}</p>", model.SenderTitle, model.SendDt, model.Message);
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 如果没有数据就获取最后那几条
        /// </summary>
        /// <param name="receiver">对话人</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChatContentIsReaded(string memberId)
        {
            StringBuilder sb = new StringBuilder("");
            GridPager gridModel = new GridPager()
            {
                rows = 3,
                page = 1,
                totalRows = 0
            };
            List<MIS_WebIM_MessageRecModel> recList = messageBLL.GetMessageFromSenderToReceiver(ref gridModel, memberId, GetUserId(), true);
            //在次排序
            for (int i = recList.Count() - 1; i > -1; i--)
            {
                MIS_WebIM_MessageRecModel model = recList[i];
                sb.AppendFormat("<p class='tit'><strong>[{0}]</strong> <span>{1}</span>：</p><p>{2}</p>", model.SenderTitle, model.SendDt, model.Message);
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置消息为已读
        /// </summary>
        /// <param name="memberId"></param>
        [HttpPost]
        public void SetMessageHasReadByReceiver(string memberId)
        {
            messageBLL.SetMessageHasReadFromSenderToReceiver(ref errors, memberId, GetUserId());

        }
        /// <summary>
        /// 获取我还有什么未读信息并更新在线用户列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetNewMessages()
        {
            if (Session["Account"] == null)
                return Json("overtime", JsonRequestBehavior.AllowGet);
            StringBuilder sbid = new StringBuilder();
            StringBuilder sbname = new StringBuilder();
            List<MIS_WebIM_SenderModel> senderList = messageBLL.GetSenderByReceiver(GetUserId());
            if (senderList!=null)
            foreach (MIS_WebIM_SenderModel model in senderList)
            {
                sbid.Append(model.Sender + ",");
                sbname.Append(model.SenderTitle + ",");
            }
            //获取消息总数
            int mesCount = senderList.Sum(a=>a.MessageCount);
            OnlineHttpModule.ProcessRequest();
            //格式
            //在线人数
            OnlineUserRecorder recorder = HttpContext.Cache[OnlineHttpModule.g_onlineUserRecorderCacheKey] as OnlineUserRecorder;

            return Json(JsonHandler.CreateMessage(recorder.GetUserList().Count, sbid.ToString() + "|" + sbname.ToString(), mesCount.ToString()), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取最近联系人
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRecentContact()
        {
            StringBuilder sb = new StringBuilder();
            GridPager gridModel = new GridPager()
            {
                rows = 20,
                page = 1,
                totalRows = 0
            };
            List<MIS_WebIM_RecentContactModel> list = recentContactBLL.GetList(ref gridModel, GetUserId());
            sb.Append("<ul id=\"RecentContactUL\">");
            foreach (MIS_WebIM_RecentContactModel model in list)
            {
                sb.AppendFormat("<li ><a class=\"a-default\" href=\"javascript:SetWaitListByRecentContact('{0}','{1}')\">{1}</a></li>", model.ContactPersons, model.ContactPersonsTitle, model.ContactPersonsTitle);
            }
            sb.Append("</ul>");

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSelMemberList(string whereStr)
        {
            List<SysUser> list = sysUserBLL.GetListBySelName(whereStr);
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (SysUser en in list)
                {
                    sb.AppendFormat("<li><a href=\"javascript:void(0)\"><lable id=\"{0}\">[{1}]</lable></a></li>", en.Id, en.TrueName);
                }
                return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //快速发送
        public ActionResult CeleritySend()
        {
            if (GetUserId() != null)
            {
                //获取真实姓名
                ViewBag.UserName = GetUserTrueName();
                ViewBag.MIS0002 = siteConfig.refreshcurrentwin;
                ViewBag.MIS0003 = siteConfig.refreshrecentcontact;
                ViewBag.MIS0004 = siteConfig.refreshnewmessage;

                return View();
            }
            else
            {
                return Redirect("/Account/Index");
            }
        }

        public ActionResult SelMember()
        {
            if (GetUserId() != null)
            {
                //获取真实姓名
                ViewBag.UserName = GetUserTrueName();
                //获取系统配置的初始值
                ViewBag.MIS0002 = siteConfig.refreshcurrentwin;
                ViewBag.MIS0003 = siteConfig.refreshrecentcontact;
                ViewBag.MIS0004 = siteConfig.refreshnewmessage;
                return View();
            }
            else
            {
                return Redirect("/Account/Index");
            }
        }
    }
}
