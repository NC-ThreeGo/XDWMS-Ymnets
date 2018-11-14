using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Apps.Models.Sys;
using Newtonsoft.Json;
using Apps.IBLL;
using Apps.BLL;
using Apps.DAL;
using Apps.Models;
using Apps.IBLL.Sys;
using Apps.BLL.Sys;
using Apps.DAL.Sys;

namespace Apps.Web.Core.Signalr
{
    [HubName("WebIMHub")]
    public class WebIM : Hub
    {
        
        //public static List<RoomInfo> RoomList = new List<RoomInfo>();
        ISysUserBLL m_BLL = new SysUserBLL()
        {
            m_Rep = new SysUserRepository(new DBContainer())
        };

        public static List<SysOnlineUserModel> list = new List<SysOnlineUserModel>();
        OnlineUsersManage mg = new OnlineUsersManage();
        /// <summary>
        /// 重写链接事件
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            if (list.Count()==0)
            {
                list = m_BLL.GetAllUsers();
            }
            return base.OnConnected();
        }
        /// <summary>
        /// 更新用户状态为在线
        /// </summary>
        /// <param name="userId"></param>
        public void RegisterUser(string userId)
        {
            if (list.Count() == 0)
            {
                list = m_BLL.GetAllUsers();
            }
            //更新用户为上线
            UpdateUserStatus(userId, Context.ConnectionId,1);
            GetCurrentInfo();
            //更新所有窗口的列表状态
            UpdateUserList();
        }

        /// <summary>
        /// 获取用户名和自己的唯一编码
        /// </summary>
        /// <param name="name"></param>
        public void GetCurrentInfo()
        {
            // 查询用户。
            var user = list.SingleOrDefault(u => u.ContextId == Context.ConnectionId);
            if (user != null)
            {
                //把信息返回到前端作显示
                string jsondata = JsonConvert.SerializeObject(user);
                Clients.Client(Context.ConnectionId).showInfo(jsondata);
            }

           
        }
        /// <summary>
        /// 用户断开
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //更新用户为下线
            UpdateUserStatus("", Context.ConnectionId, 0);
            //更新所有用户的列表
            UpdateUserList();
            return base.OnDisconnected(true);
        }

        /// <summary>
        /// 更新所有用户的在线列表
        /// </summary>
        private void UpdateUserList()
        {
            string jsondata = JsonConvert.SerializeObject(list);
            Clients.All.getUserlist(jsondata);

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="contextID"></param>
        /// <param name="Message"></param>
        public void SendMessage(string contextID, string Message)
        {

            var user = list.Where(u => u.ContextId == contextID).FirstOrDefault();
            //判断用户是否存在,存在则发送
            if (user != null)
            {
                //给用户发送一条信息
                Clients.Client(contextID).addMessage(Message + " " + DateTime.Now, Context.ConnectionId);
                //给自己也发送一条信息才能看到自己发的短信
                Clients.Client(Context.ConnectionId).addMessage(Message + " " + DateTime.Now, contextID);
            }
            else
            {
                //用户突然断开了
                Clients.Client(Context.ConnectionId).showMessage("该用户已离线");
            }
        }


        /// <summary>
        /// 更新用户的在线状态，userid,contextid同时为空不做处理
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="ContextId">ContextId</param>
        /// <param name="Status">状态：0离线状态1在线2忙碌3离开</param>
        public void UpdateUserStatus(string UserId, string ContextId, int Status)
        {
            //获取缓存中的用户列表
            SysOnlineUserModel model = new SysOnlineUserModel();
            //用户链接时调用
            if (!string.IsNullOrEmpty(UserId))
            {
                model = list.Where(a => a.UserId == UserId).SingleOrDefault();

            }//用户断开时候用
            else if (!string.IsNullOrEmpty(ContextId))
            {
                model = list.Where(a => a.ContextId == ContextId).SingleOrDefault();
            }
            //model为空一般是刷新太快无法处理，忽略不会导致信息错误
            if (model != null)
            {
                //删除列表中的用户，稍后添加
                list.Remove(model);
                //改变状态
                model.Status = Status;
                //给出ContextId
                model.ContextId = ContextId;
                //加回用户
                list.Add(model);
            }
        }
      
    }
}