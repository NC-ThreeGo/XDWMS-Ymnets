using System;

namespace Apps.Core.OnlineStat
{
    /// <summary>
    /// 在线用户记录器工厂类
    /// </summary>
    public sealed class OnlineUserRecorderFactory
    {
        // 用户超时分钟数
        private int m_userTimeOutMinute = 20;
        // 统计时间间隔
        private int m_statisticEventInterval = 60;

        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        public OnlineUserRecorderFactory()
        {
        }
        #endregion

        /// <summary>
        /// 设置或获取用户超时分钟数
        /// </summary>
        public int UserTimeOutMinute
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
        public int StatisticEventInterval
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
        /// 创建在线用户记录器
        /// </summary>
        /// <returns></returns>
        public OnlineUserRecorder Create()
        {
            OnlineUserRecorder recorder = null;

            // 创建在线用户记录器
            recorder = new OnlineUserRecorder();

            // 设置超时分钟数
            recorder.UserTimeOutMinute = this.UserTimeOutMinute;
            // 统计时间间隔
            recorder.StatisticEventInterval = this.StatisticEventInterval;

            return recorder;
        }
    }
}