using System;
using Apps.Models;
using System.Collections.Generic;
using Apps.Common;
using Apps.Models.MIS;
using Apps.IDAL.MIS;
using Apps.IDAL;
using Apps.IDAL.Sys;

namespace Apps.IBLL.MIS
{
    public partial interface IMIS_WebIM_MessageBLL
    {
        bool DeleteMessageByUserId(ref ValidationErrors validationErrors, string userId, string[] ids);
        bool DeleteMessageByUserId(ref ValidationErrors validationErrors, string userId);
        List<MIS_WebIM_MessageModel> GetMessage(ref GridPager pager, string userId);
        string CreateMessage(ref ValidationErrors validationErrors, string message, string sender, string receiver, string receiverTitle);
        bool CreateMessage(ref ValidationErrors validationErrors, MIS_WebIM_MessageModel model);
        bool CreateMessage(ref ValidationErrors validationErrors, string id, string message, string sender, string receiver, string receiverTitle);
        bool DeleteMessageAllByReceiver(ref ValidationErrors validationErrors, string receiver);
        bool DeleteMessageAllBySender(ref ValidationErrors validationErrors, string sender);
        bool DeleteMessageByReceiver(ref ValidationErrors validationErrors, string id, string receiver);
        bool DeleteMessageBySender(ref ValidationErrors validationErrors, string id);
        System.Collections.Generic.List<MIS_WebIM_MessageModel> GetMesasgeAllBySender(ref GridPager pager, string sender);
        System.Collections.Generic.List<MIS_WebIM_MessageRecModel> GetMessageAllByReceiver(ref GridPager pager, string receiver);
        MIS_WebIM_MessageRecModel GetMessageByReceiver(string receiver, string id);
        MIS_WebIM_MessageModel GetMessageBySender(string id);
        IMIS_WebIM_MessageRepository repository { get; set; }
        bool SetMessageHasReadById(ref ValidationErrors validationErrors, string receiver, string id);
        bool SetMessageHasReadByReceiver(ref ValidationErrors validationErrors, string receiver);
        ISysUserRepository userRep { get; set; }
        List<MIS_WebIM_SenderModel> GetSenderByReceiver(string receiver);
        List<MIS_WebIM_MessageRecModel> GetMessageAllByReceiver(ref GridPager pager, string receiver, bool state);
        List<MIS_WebIM_MessageRecModel> GetMessageFromSenderToReceiver(ref GridPager pager, string sender, string receiver, bool state);
        List<MIS_WebIM_MessageRecModel> GetMessageFromSenderToReceiver(ref GridPager pager, string sender, string receiver);
        bool SetMessageHasReadFromSenderToReceiver(ref ValidationErrors validationErrors, string sender, string receiver);
    }
}
