using System;
using System.Collections.Generic;

namespace Apps.Core.OnlineStat
{
    /// <summary>
    /// 在线用户数据库
    /// </summary>
    internal class OnlineUserDB
    {
        // 在线用户集合
        private List<OnlineUser> m_onlineUserList = null;

        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        public OnlineUserDB()
        {
            this.m_onlineUserList = new List<OnlineUser>();
        }
        #endregion

        /// <summary>
        /// 插入新用户
        /// </summary>
        /// <param name="newUser"></param>
        public void Insert(OnlineUser newUser)
        {
            lock (this)
            {
                this.m_onlineUserList.Add(newUser);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="delUser"></param>
        public void Delete(OnlineUser delUser)
        {
            lock (this)
            {
                this.m_onlineUserList.RemoveAll((new PredicateDelete(delUser)).Predicate);
            }
        }

        /// <summary>
        /// 清除超时用户
        /// </summary>
        /// <param name="timeOutMinute">超时分钟数</param>
        public void ClearTimeOut(int timeOutMinute)
        {
            lock (this)
            {
                this.m_onlineUserList.RemoveAll((new PredicateTimeOut(timeOutMinute)).Predicate);
            }
        }

		/// <summary>
		/// 排序在线用户列表
		/// </summary>
		public void Sort()
		{
			// 按活动时间进行排序
			this.m_onlineUserList.Sort(CompareByActiveTime);
		}

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public IList<OnlineUser> Select()
        {
            return this.m_onlineUserList.ToArray();
        }

        /// <summary>
        /// 获取在线用户数量
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.m_onlineUserList.Count;
        }

        #region 用户删除条件断言
        private class PredicateDelete
        {
            // 被删除的用户
            private OnlineUser m_delUser = null;
            // 是否为空条件
            private bool m_isNullCondation = true;

            #region 类构造器
            /// <summary>
            /// 类参数构造器
            /// </summary>
            /// <param name="delUser"></param>
            public PredicateDelete(OnlineUser delUser)
            {
                this.m_delUser = delUser;

                if (this.m_delUser == null)
                    return;

                // 用户 ID
                this.m_isNullCondation &= this.m_delUser.UniqueID <= 0;
                // 名称
                this.m_isNullCondation &= String.IsNullOrEmpty(this.m_delUser.UserName);
                // SessionID
                this.m_isNullCondation &= String.IsNullOrEmpty(this.m_delUser.SessionID);
            }
            #endregion

            /// <summary>
            /// 判断用户 ID 是否等于指定值
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public bool Predicate(OnlineUser user)
            {
                if (this.m_isNullCondation)
                    return false;

                if (user == null)
                    return false;

                // 用户 ID 相同, ID > 0
                if (user.UniqueID > 0 && user.UniqueID == this.m_delUser.UniqueID)
                    return true;

                // 用户名称相同, 并且不是空字符串
                if (!String.IsNullOrEmpty(user.UserName) && user.UserName == this.m_delUser.UserName)
                    return true;

                // SessionID 相同, 并且不是空字符串
                if (user.SessionID == this.m_delUser.SessionID)
                    return true;

                return false;
            }
        }
        #endregion

        #region 用户超时条件断言
        private class PredicateTimeOut
        {
            // 超时分钟数
            private int m_timeOutMinute;

            #region 类构造器
            /// <summary>
            /// 类参数构造器
            /// </summary>
            /// <param name="minute">超时分钟数</param>
            public PredicateTimeOut(int minute)
            {
                this.m_timeOutMinute = minute;
            }
            #endregion

            /// <summary>
            /// 判断用户活动时间是否小于指定值
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public bool Predicate(OnlineUser user)
            {
                if (user == null)
                    return false;

                return user.ActiveTime < DateTime.Now.AddMinutes(-this.m_timeOutMinute);
            }
        }
        #endregion

        /// <summary>
        /// 比较两个用户的活动时间
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int CompareByActiveTime(OnlineUser x, OnlineUser y)
        {
            if (x == null)
                throw new NullReferenceException("X 值为空 ( X Is Null )");

            if (y == null)
                throw new NullReferenceException("Y 值为空 ( Y Is Null )");

            if (x.ActiveTime > y.ActiveTime)
                return -1;

            if (x.ActiveTime < y.ActiveTime)
                return +1;

            return 0;
        }
    }
}