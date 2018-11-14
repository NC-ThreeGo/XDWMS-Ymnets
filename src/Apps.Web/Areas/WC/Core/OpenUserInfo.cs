using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apps.Web.Areas.WC.Core
{
    public class OpenUserInfo
    {
        /// <summary>
        /// 授权用户的OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 授权用户的姓名
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 授权用户的头像
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// Token开始时间
        /// </summary>
        public DateTime AccessTokenStartTime { get; set; }//两个小时后会过期
        /// <summary>
        /// 用户的Token
        /// </summary>
        public string AccessToken { get; set; }//每个人都会不一样的

    }
}