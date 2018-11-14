using System;

namespace Apps.Core.OnlineStat
{
    /// <summary>
    /// 在线用户命令基础类
    /// </summary>
    internal abstract class OnlineUserCmdBase
    {
        // 当前用户对象
        private OnlineUser m_currUser = null;
        // 在线用户数据库
        private OnlineUserDB m_db = null;

        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        public OnlineUserCmdBase()
        {
        }

        /// <summary>
        /// 类参数构造器
        /// </summary>
        /// <param name="db">在线用户数据库</param>
        /// <param name="currUser">当前用户</param>
        public OnlineUserCmdBase(OnlineUserDB db, OnlineUser currUser)
        {
            this.OnlineUserDB = db;
            this.CurrentUser = currUser;
        }
        #endregion

        /// <summary>
        /// 设置或获取当前用户
        /// </summary>
        public OnlineUser CurrentUser
        {
            set
            {
                this.m_currUser = value;
            }

            get
            {
                return this.m_currUser;
            }
        }

        /// <summary>
        /// 设置或获取在线用户数据库
        /// </summary>
        public OnlineUserDB OnlineUserDB
        {
            set
            {
                this.m_db = value;
            }

            get
            {
                return this.m_db;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public abstract void Execute();
    }
}