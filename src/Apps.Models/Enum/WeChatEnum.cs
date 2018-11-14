using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.Enum
{
    public enum WeChatSubscriberEnum
    {
        /// <summary>
        /// 个人订阅号
        /// </summary>
        PersonalSubscriber = 1,
        /// <summary>
        /// 媒体订阅号
        /// </summary>
        MediaSubscriber = 2,

        /// <summary>
        /// 企业订阅号
        /// </summary>
        EnterpriseSubscriber = 3,

        /// <summary>
        /// 测试号
        /// </summary>
        TestSubscriber = 4
    }
    public enum WeChatReplyCategory
    {
        //文本
        Text =1,
        //图文
        Image =2,
        //语音
        Voice =3,
        //相等，用于回复关键字
        Equal=4,
        //包含，用于回复关键字
        Contain = 5
    }

    public enum WeChatRequestRuleEnum
    {
        /// <summary>
        /// 默认回复，没有处理的
        /// </summary>
        Default =0,
        /// <summary>
        /// 关注回复
        /// </summary>
        Subscriber =1,
        /// <summary>
        /// 文本回复
        /// </summary>
        Text =2,
        /// <summary>
        /// 图片回复
        /// </summary>
        Image =3,
        /// <summary>
        /// 语音回复
        /// </summary>
        Voice =4,
        /// <summary>
        /// 视频回复
        /// </summary>
        Video =5,
        /// <summary>
        /// 超链接回复
        /// </summary>
        Link =6,
        /// <summary>
        /// LBS位置回复
        /// </summary>
        Location =7,
    }

    /// <summary>
    /// 用户请求类型
    /// </summary>
    public enum WeChatRequestType
    {
        Text =1,
        Image=2,
        Subscriber=3,
        UnSubscriber =4,
        Event =5,
        None=0
    }
    /// <summary>
    /// 系统响应类型
    /// </summary>
    public enum WeChatResponseType
    {
        Text =1,
        Image=2,
        Voice=3,
        Video=4,
        Link=5,
        Location=6,
        None=0
    }

}
