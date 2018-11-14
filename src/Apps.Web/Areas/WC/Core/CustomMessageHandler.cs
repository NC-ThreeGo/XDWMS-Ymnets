
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using Apps.IBLL.WC;
using Apps.BLL.WC;
using Apps.DAL.WC;
using Apps.Models.WC;
using Apps.Common;
using Apps.Models;
using System.Collections.Generic;
using Apps.Models.Enum;
using Apps.Web.Core;

namespace Apps.Web.Areas.WC.Core
{
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
        * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
        * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
        * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
        */
        string Id = "";

        public CustomMessageHandler(Stream inputStream, PostModel postModel,string id, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                this.Id = id;
                return true;
            };
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
          

            IWC_OfficalAccountsBLL account_BLL = new WC_OfficalAccountsBLL() {
                m_Rep = new WC_OfficalAccountsRepository(new Models.DBContainer())
            };

            //获得当前公众号
            WC_OfficalAccountsModel account = account_BLL.GetById(Id);

            IWC_MessageResponseBLL message_BLL = new WC_MessageResponseBLL()
            {
                m_Rep = new WC_MessageResponseRepository(new Models.DBContainer())
            };
            
           
            //只获取第一条匹配的条件作为信息
            List<P_WC_GetResponseContent_Result> messageList = message_BLL.GetResponseContent(Id, requestMessage.Content);

            

            if (messageList.Count()>0)
            {
                //文本方式
                if (messageList[0].MessageRule == (int)WeChatRequestRuleEnum.Text)
                {
                    var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                    responseMessage.CreateTime = ResultHelper.NowTime;
                    responseMessage.ToUserName = requestMessage.FromUserName;
                    responseMessage.FromUserName = account.OfficalId;
                    responseMessage.Content = messageList[0].TextContent;
                    return responseMessage;
                }
                //图文方式
                else if (messageList[0].MessageRule == (int)WeChatRequestRuleEnum.Image)
                {
                    var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                    foreach (var model in messageList)
                    {
                        responseMessage.Articles.Add(new Article()
                        {
                            Title = model.TextContent,
                            Description = model.ImgTextContext,
                            PicUrl = WebConfigPara.SiteConfig.WeChatSiteUrl + model.ImgTextUrl,
                            Url = model.ImgTextLink
                        });
                    }
                    return responseMessage;
                }//一般很少用到
                else if (messageList[0].MessageRule == (int)WeChatRequestRuleEnum.Voice)
                {
                    var responseMessage = base.CreateResponseMessage<ResponseMessageMusic>();
                    responseMessage.Music.MusicUrl = WebConfigPara.SiteConfig.WeChatSiteUrl + messageList[0].MeidaUrl;
                    responseMessage.Music.Title = messageList[0].TextContent;
                    responseMessage.Music.Description = messageList[0].Remark;
                    return responseMessage;
                }//默认回复
                else if(messageList[0].MessageRule == (int)WeChatRequestRuleEnum.Default)
                {
                    if (messageList[0].Category == (int)WeChatReplyCategory.Text)
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                        responseMessage.CreateTime = ResultHelper.NowTime;
                        responseMessage.ToUserName = requestMessage.FromUserName;
                        responseMessage.FromUserName = account.OfficalId;
                        responseMessage.Content = messageList[0].TextContent;
                        return responseMessage;
                    }
                    //图文方式
                    else if (messageList[0].Category == (int)WeChatReplyCategory.Image)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        foreach (var model in messageList)
                        {
                            responseMessage.Articles.Add(new Article()
                            {
                                Title = model.TextContent,
                                Description = model.ImgTextContext,
                                PicUrl = WebConfigPara.SiteConfig.WeChatSiteUrl + model.ImgTextUrl,
                                Url = model.ImgTextLink
                            });
                        }
                        return responseMessage;
                    }//一般很少用到
                    else if (messageList[0].Category == (int)WeChatReplyCategory.Voice)
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageMusic>();
                        responseMessage.Music.MusicUrl = WebConfigPara.SiteConfig.WeChatSiteUrl + messageList[0].MeidaUrl;
                        responseMessage.Music.Title = messageList[0].TextContent;
                        responseMessage.Music.Description = messageList[0].Remark;
                        return responseMessage;
                    }

                }
                //下面方式用到才启用
                //视频方式
                //位置
            }
            var errorResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            //因为没有设置errorResponseMessage.Content，所以这小消息将无法正确返回。
            return errorResponseMessage;


        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var locationService = new LocationService();
            var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
            return responseMessage;
        }

        public override IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您刚才发送的是小视频";
            return responseMessage;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();
            responseMessage.Articles.Add(new Article()
            {
                Title = "您刚才发送了图片信息",
                Description = "您发送的图片将会显示在边上",
                PicUrl = requestMessage.PicUrl,
                Url = "http://sdk.weixin.senparc.com"
            });
            responseMessage.Articles.Add(new Article()
            {
                Title = "第二条",
                Description = "第二条带连接的内容",
                PicUrl = requestMessage.PicUrl,
                Url = "http://sdk.weixin.senparc.com"
            });

            return responseMessage;
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            IWC_OfficalAccountsBLL account_BLL = new WC_OfficalAccountsBLL()
            {
                m_Rep = new WC_OfficalAccountsRepository(new Models.DBContainer())
            };

            //获得当前公众号
            WC_OfficalAccountsModel account = account_BLL.GetById(Id);
            var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            //上传缩略图
            var uploadResult = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(account.AccessToken, UploadMediaFileType.image,
                                                         Server.GetMapPath("~/Images/Logo.jpg"));

            //设置音乐信息
            responseMessage.Music.Title = "天籁之音";
            responseMessage.Music.Description = "播放您上传的语音";
            responseMessage.Music.MusicUrl = "http://sdk.weixin.senparc.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            responseMessage.Music.HQMusicUrl = "http://sdk.weixin.senparc.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            responseMessage.Music.ThumbMediaId = uploadResult.media_id;
            return responseMessage;
        }
        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了一条视频信息，ID：" + requestMessage.MediaId;
            return responseMessage;
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = string.Format(@"您发送了一条连接信息：
Title：{0}
Description:{1}
Url:{2}", requestMessage.Title, requestMessage.Description, requestMessage.Url);
            return responseMessage;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }
    }

   
}