using System;
using Apps.Models;
using System.Collections.Generic;
using Apps.Models.MIS;
namespace Apps.IDAL.MIS
{
    public partial interface IMIS_WebIM_MessageRepository
    {
        void DeleteMessageAllNotBySender(string sender);
        void DeleteMessageNotBySender(string sender, string []ids);
        void CreateMessage(string id, string message, string sender, string receiver, string receiverTitle);
        void DeleteMessageAllByReceiver(string receiver);
        void DeleteMessageAllBySender(string sender);
        void DeleteMessageByReceiver(string id, string receiver);
        void DeleteMessageBySender(string id);
        System.Linq.IQueryable<MIS_WebIM_Message> GetMesasgeAllBySender();
        System.Linq.IQueryable<MIS_WebIM_MessageRecModel> GetMessageAllByReceiver(string receiver);
        MIS_WebIM_MessageRecModel GetMessageByReceiver(string receiver, string id);
        MIS_WebIM_Message GetMessageBySender(string id);
        void SetMessageHasReadById(string receiver, string id);
        void SetMessageHasReadByReceiver(string receiver);
        System.Collections.Generic.List<MIS_WebIM_SenderModel> GetSenderByReceiver(string receiver);
        void SetMessageHasReadFromSenderToReceiver(string sender, string receiver);
        List<MIS_WebIM_MessageModel> GetMessage(string userId, int pageno, int rows, ref int rowscount);
    }
}
