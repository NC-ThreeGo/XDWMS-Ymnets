using System;
using System.Collections.Generic;

namespace Apps.Core.OnlineStat
{
    /// <summary>
    /// 在线用户类
    /// </summary>
    public class OnlineUser
    {
        // 用户 ID
        private int m_uniqueID;
        // 名称
        private string m_userName;
        // 身份
        private int m_userDegree;
        // 最后活动时间
        private DateTime m_activeTime;
        // 最后请求地址
        private string m_requestURL;
        // SessionID
        private string m_sessionID;
        // IP 地址
        private string m_clientIP;

        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        public OnlineUser()
        {
        }

        /// <summary>
        /// 类参数构造器
        /// </summary>
        /// <param name="uniqueID">用户 ID</param>
        /// <param name="userName">用户名称</param>
        public OnlineUser(int uniqueID, string userName)
        {
            this.UniqueID = uniqueID;
            this.UserName = userName;
        }
        #endregion

        /// <summary>
        /// 设置或获取用户 ID
        /// </summary>
        public int UniqueID
        {
            set
            {
                this.m_uniqueID = value;
            }

            get
            {
                return this.m_uniqueID;
            }
        }

        /// <summary>
        /// 设置或获取用户昵称
        /// </summary>
        public string UserName
        {
            set
            {
                this.m_userName = value;
            }

            get
            {
                return this.m_userName;
            }
        }

        /// <summary>
        /// 设置或获取用户身份
        /// </summary>
        public int UserDegree
        {
            set
            {
                this.m_userDegree = value;
            }

            get
            {
                return this.m_userDegree;
            }
        }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime ActiveTime
        {
            set
            {
                this.m_activeTime = value;
            }

            get
            {
                return this.m_activeTime;
            }
        }

        /// <summary>
        /// 最后请求地址
        /// </summary>
        public string RequestURL
        {
            set
            {
                this.m_requestURL = value;
            }

            get
            {
                return this.m_requestURL;
            }
        }

        /// <summary>
        /// 设置或获取 SessionID
        /// </summary>
        public string SessionID
        {
            set
            {
                this.m_sessionID = value;
            }

            get
            {
                return this.m_sessionID;
            }
        }

        /// <summary>
        /// 设置或获取 IP 地址
        /// </summary>
        public string ClientIP
        {
            set
            {
                this.m_clientIP = value;
            }

            get
            {
                return this.m_clientIP;
            }
        }
    }
}