using Apps.Core.OnlineStat;
using Apps.Models.Sys;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Apps.Web.Core
{

    public class OnlineHttpModule
    {
        // 缓存键
        public static readonly string g_onlineUserRecorderCacheKey = "__OnlineUserRecorder";
        #region IHttpHandler 成员
        public static void ProcessRequest()
        {
            // 获取在线用户记录器
            OnlineUserRecorder recorder = HttpContext.Current.Cache[g_onlineUserRecorderCacheKey] as OnlineUserRecorder;

            if (recorder == null)
            {
                // 创建记录器工厂
                OnlineUserRecorderFactory factory = new OnlineUserRecorderFactory();

                // 设置用户超时时间
                factory.UserTimeOutMinute = 2;
                // 统计时间间隔
                factory.StatisticEventInterval = 20;

                // 创建记录器
                recorder = factory.Create();

                // 缓存记录器
                HttpContext.Current.Cache.Insert(g_onlineUserRecorderCacheKey, recorder);
            }

            OnlineUser user = new OnlineUser();


            AccountModel model = (AccountModel)HttpContext.Current.Session["Account"];//注意session的名称是和登录保存的名称一致
            // 用户名称
            user.UserName = Convert.ToString(model.Id);
            // SessionID
            user.SessionID = HttpContext.Current.Session.SessionID;
            // IP 地址
            user.ClientIP = HttpContext.Current.Request.UserHostAddress;
            // 最后活动时间
            user.ActiveTime = DateTime.Now;
            // 最后请求地址
            user.RequestURL = HttpContext.Current.Request.RawUrl;

            // 保存用户信息
            recorder.Persist(user);
        }
        #endregion
    }



}
