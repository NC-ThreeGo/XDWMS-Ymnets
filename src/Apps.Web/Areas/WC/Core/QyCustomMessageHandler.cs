/*----------------------------------------------------------------
    Copyright (C) 2016 Senparc
    
    文件名：QyCustomMessageHandler.cs
    文件功能描述：自定义QyMessageHandler
    
    
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Senparc.Weixin.QY.Entities;
using Senparc.Weixin.QY.MessageHandlers;
using Apps.Models;
using Apps.Web.Core;
using Apps.Models.Enum;
using Apps.Common;
using Apps.IBLL.WC;
using Apps.DAL.WC;
using Apps.BLL.WC;
using Apps.Models.WC;
using Senparc.Weixin.QY.Helpers;

namespace Apps.Web.Areas.WC.Core
{
    public partial class QyCustomMessageHandler : QyMessageHandler<QyCustomMessageContext>
    {
        string Id = "";

        public QyCustomMessageHandler(Stream inputStream, PostModel postModel, string id, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessage = true;
            this.Id = id;
        }

        public override IResponseMessageBase OnTextRequest(Senparc.Weixin.QY.Entities.RequestMessageText requestMessage)
        {
            IWC_OfficalAccountsBLL account_BLL = new WC_OfficalAccountsBLL()
            {
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



            if (messageList.Count() > 0)
            {
                //文本方式
                if (messageList[0].MessageRule == (int)WeChatRequestRuleEnum.Text)
                {
                    var responseMessage = base.CreateResponseMessage<Senparc.Weixin.QY.Entities.ResponseMessageText>();
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
                        responseMessage.Articles.Add(new Senparc.Weixin.QY.Entities.Article()
                        {
                            Title = model.TextContent,
                            Description = model.ImgTextContext,
                            PicUrl = WebConfigPara.SiteConfig.WeChatSiteUrl + model.ImgTextUrl,
                            Url = model.ImgTextLink
                        });
                    }
                    return responseMessage;
                }//一般很少用到
               //默认回复
                else if (messageList[0].MessageRule == (int)WeChatRequestRuleEnum.Default)
                {
                    if (messageList[0].Category == (int)WeChatReplyCategory.Text)
                    {
                        var responseMessage = base.CreateResponseMessage<Senparc.Weixin.QY.Entities.ResponseMessageText>();
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
                            responseMessage.Articles.Add(new Senparc.Weixin.QY.Entities.Article()
                            {
                                Title = model.TextContent,
                                Description = model.ImgTextContext,
                                PicUrl = WebConfigPara.SiteConfig.WeChatSiteUrl + model.ImgTextUrl,
                                Url = model.ImgTextLink
                            });
                        }
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

        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageImage>();
            responseMessage.Image.MediaId = requestMessage.MediaId;
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您刚发送的图片如下：";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format("位置坐标 {0} - {1}", requestMessage.Latitude, requestMessage.Longitude);
            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(Senparc.Weixin.QY.Entities.IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这是一条没有找到合适回复信息的默认消息。";
            return responseMessage;
        }
    }
}
