
using System;
using System.Collections.Generic;
using System.Threading;

namespace Apps.Core.OnlineStat
{
	/// <summary>
	/// 在线用户记录器
	/// </summary>
    public class OnlineUserRecorder
    {
        // 在线用户数据库
        private OnlineUserDB m_db = null;
        // 命令队列 A, 用于接收命令
        private Queue<OnlineUserCmdBase> m_cmdQueueA = null;
        // 命令队列 X, 用于执行命令
        private Queue<OnlineUserCmdBase> m_cmdQueueX = null;
        // 繁忙标志
        private bool m_isBusy = false;
        // 上次统计时间
        private DateTime m_lastStatisticTime = new DateTime(0);
        // 用户超时分钟数
        private int m_userTimeOutMinute = 20;
        // 统计时间间隔
        private int m_statisticEventInterval = 60;

        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        internal OnlineUserRecorder()
        {
            this.m_db = new OnlineUserDB();

            // 初始化命令队列
            this.m_cmdQueueA = new Queue<OnlineUserCmdBase>();
            this.m_cmdQueueX = new Queue<OnlineUserCmdBase>();
        }
        #endregion

        /// <summary>
        /// 设置或获取用户超时分钟数
        /// </summary>
        internal int UserTimeOutMinute
        {
            set
            {
                this.m_userTimeOutMinute = value;
            }

            get
            {
                return this.m_userTimeOutMinute;
            }
        }

        /// <summary>
        /// 设置或获取统计时间间隔(单位秒)
        /// </summary>
        internal int StatisticEventInterval
        {
            set
            {
                this.m_statisticEventInterval = value;
            }

            get
            {
                return this.m_statisticEventInterval;
            }
        }

        /// <summary>
        /// 保存在线用户信息
        /// </summary>
        /// <param name="onlineUser"></param>
        public void Persist(OnlineUser onlineUser)
        {
            // 创建删除命令
            OnlineUserDeleteCmd delCmd = new OnlineUserDeleteCmd(this.m_db, onlineUser);
            // 创建插入命令
            OnlineUserInsertCmd insCmd = new OnlineUserInsertCmd(this.m_db, onlineUser);

            // 将命令添加到队列
            this.m_cmdQueueA.Enqueue(delCmd);
            this.m_cmdQueueA.Enqueue(insCmd);

            // 处理命令队列
            this.BeginProcessCmdQueue();
        }

        /// <summary>
        /// 删除在线用户信息
        /// </summary>
        /// <param name="onlineUser"></param>
        public void Delete(OnlineUser onlineUser)
        {
            // 创建删除命令
            OnlineUserDeleteCmd delCmd = new OnlineUserDeleteCmd(this.m_db, onlineUser);

            // 将命令添加到队列
            this.m_cmdQueueA.Enqueue(delCmd);

            // 处理命令队列
            this.BeginProcessCmdQueue();
        }

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <returns></returns>
        public IList<OnlineUser> GetUserList()
        {
            return this.m_db.Select();
        }

        /// <summary>
        /// 获取在线用户数量
        /// </summary>
        /// <returns></returns>
        public int GetUserCount()
        {
            return this.m_db.Count();
        }

        /// <summary>
        /// 异步方式处理命令队列
        /// </summary>
        private void BeginProcessCmdQueue()
        {
            if (this.m_isBusy)
                return;

            // 未到可以统计的时间
            if (DateTime.Now - this.m_lastStatisticTime < TimeSpan.FromSeconds(this.StatisticEventInterval))
                return;

            Thread t = null;

            t = new Thread(new ThreadStart(this.ProcessCmdQueue));
            t.Start();
        }

        /// <summary>
        /// 处理命令队列
        /// </summary>
        private void ProcessCmdQueue()
        {
            lock (this)
            {
                if (this.m_isBusy)
                    return;

                // 未到可以统计的时间
                if (DateTime.Now - this.m_lastStatisticTime < TimeSpan.FromSeconds(this.StatisticEventInterval))
                    return;

                this.m_isBusy = true;

                // 声明临时队列, 用于交换
                Queue<OnlineUserCmdBase> tempQ = null;

                // 交换两个命令队列
                tempQ = this.m_cmdQueueA;
                this.m_cmdQueueA = this.m_cmdQueueX;
                this.m_cmdQueueX = tempQ;
                tempQ = null;

                while (this.m_cmdQueueX.Count > 0)
                {
                    // 获取命令
                    OnlineUserCmdBase cmd = this.m_cmdQueueX.Peek();

                    if (cmd == null)
                        break;

                    // 执行命令
                    cmd.Execute();

                    // 从队列中移除命令
                    this.m_cmdQueueX.Dequeue();
                }

				// 清除超时用户
				this.m_db.ClearTimeOut(this.UserTimeOutMinute);
				// 排序
				this.m_db.Sort();

                this.m_lastStatisticTime = DateTime.Now;
                this.m_isBusy = false;
            }
        }
    }
}