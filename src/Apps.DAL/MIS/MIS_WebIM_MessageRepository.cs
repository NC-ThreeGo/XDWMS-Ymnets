using System;
using System.Collections.Generic;
using System.Linq;
using Apps.Common;
using System.Data;
using Apps.Models;
using Apps.Models.MIS;
using System.Data.Entity.Core.Objects;

namespace Apps.DAL.MIS
{
    public partial class MIS_WebIM_MessageRepository
    {

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        public void CreateMessage(string id, string message, string sender, string receiver,string receiverTitle)
        {
            Context.P_MIS_CreateMessage(id, message, sender, receiver, receiverTitle);
        }

        /// <summary>
        /// 删除发送者的一个消息(物理删除)
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMessageBySender(string id)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_DeleteMessageBySender(id);
            }
        }

        /// <summary>
        /// 删除发送者的所有消息（物理删除）
        /// </summary>
        /// <param name="sender"></param>
        public void DeleteMessageAllBySender(string sender)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_DeleteMessageAllBySender(sender);
            }
        }
        /// <summary>
        /// 删除发送者的多个消息(非物理删除)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="id"></param>
        public void DeleteMessageNotBySender(string sender,string[] ids)
        {
            using (DBContainer db = new DBContainer())
            {
                IQueryable< MIS_WebIM_Message> entityList=from r in db.MIS_WebIM_Message
                           where ids.Contains(r.Id)
                           where r.Sender==sender
                           select r;
                if (entityList == null)
                {
                    return;
                }
                foreach (var entity in entityList)
                {
                    entity.State = true;
                }
                db.SaveChanges();

            }
        }
        /// <summary>
        /// 删除发送者的全部消息(非物理删除)
        /// </summary>
        /// <param name="sender"></param>
        public void DeleteMessageAllNotBySender(string sender)
        {
            using (DBContainer db = new DBContainer())
            {
                IQueryable<MIS_WebIM_Message> entityList = from r in db.MIS_WebIM_Message
                                                     where r.Sender==sender
                                                     select r;
                if (entityList == null)
                {
                    return;
                }
                foreach (var entity in entityList)
                {
                    entity.State = true;
                }
                db.SaveChanges();

            }
        }
        /// <summary>
        /// 删除接收者的一个消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="?"></param>
        public void DeleteMessageByReceiver(string id, string receiver)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_DeleteMessageByReceiver(id, receiver);
            }
        }
        /// <summary>
        /// 删除接收者的所有消息
        /// </summary>
        /// <param name="receiver"></param>
        public void DeleteMessageAllByReceiver(string receiver)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_DeleteMessageAllByReceiver(receiver);
            }
        }
        /// <summary>
        /// 设置接收者的所有未阅读消息为已阅
        /// </summary>
        /// <param name="receiver"></param>
        public void SetMessageHasReadByReceiver(string receiver)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_SetMessageHasReadByReceiver(receiver);
            }
        }
        public void SetMessageHasReadFromSenderToReceiver(string sender,string receiver)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_SetMessageHasReadFromSenderToReceiver(sender,receiver);
            }
        }
        /// <summary>
        /// 设置接收者的一条未阅读消息为已阅
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="id"></param>
        public void SetMessageHasReadById(string receiver, string id)
        {
            using (DBContainer db = new DBContainer())
            {
                db.P_MIS_SetMessageHasReadById(receiver, id);
            }
        }
        /// <summary>
        /// 返回发送信息给当前用户的发送者列表及未阅读信息数
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public List<MIS_WebIM_SenderModel> GetSenderByReceiver(string receiver)
        {
            using (DBContainer db = new DBContainer())
            {
                ObjectResult<P_MIS_GetSenderByReceiver_Result> result= db.P_MIS_GetSenderByReceiver(receiver);
                var modelList = from r in result
                                select new MIS_WebIM_SenderModel { 
                                    Sender=r.Sender,
                                    SenderTitle=r.SenderTitle,
                                    MessageCount=r.MessageCount
                                };
                return modelList.ToList();
            }
        }
        /// <summary>
        /// 读取一个发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MIS_WebIM_Message GetMessageBySender(string id)
        {
            using (DBContainer db = new DBContainer())
            {
                return db.MIS_WebIM_Message.SingleOrDefault(a => a.Id == id);
            }
        }
        /// <summary>
        /// 返回用户的所有通信记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageno"></param>
        /// <param name="rows"></param>
        /// <param name="rowscount"></param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageModel> GetMessage(string userId, int pageno, int rows, ref int rowscount)
        {
            using (DBContainer db = new DBContainer())
            {
                ObjectParameter pRowscount = new ObjectParameter("rowscount", typeof(int));

                List<MIS_WebIM_Message> msgList = db.P_MIS_GetMessage(userId, pageno, rows, pRowscount).ToList();
                rowscount = (int)pRowscount.Value;

                var msgModelList = (from r in msgList
                                   select new MIS_WebIM_MessageModel {
                                       Id = r.Id,
                                       Message = r.Message,
                                       Sender = r.Sender,
                                       receiver = r.receiver,
                                       State = r.State,
                                       SendDt = r.SendDt,
                                       receiverTitle = r.receiverTitle,
                                   }).ToList();


                return msgModelList;
            }
        }

        /// <summary>
        /// 发送者发送的全部消息
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IQueryable<MIS_WebIM_Message> GetMesasgeAllBySender()
        {
            IQueryable<MIS_WebIM_Message> list = Context.MIS_WebIM_Message.AsQueryable();
            return list;
        }
        /// <summary>
        /// 返回接收者一个消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public MIS_WebIM_MessageRecModel GetMessageByReceiver(string receiver, string id)
        {
            using (DBContainer db = new DBContainer())
            {
                var model = (from m in db.MIS_WebIM_Message
                             join r in db.MIS_WebIM_Message_Rec on m.Id equals r.MessageId
                             where m.Id == id
                             where r.receiver == receiver
                             select new MIS_WebIM_MessageRecModel
                             {
                                 Id = m.Id,
                                 Message = m.Message,
                                 Sender = m.Sender,
                                 receiver = r.receiver,
                                 State = r.State,
                                 SendDt = m.SendDt,
                                 RecDt = r.RecDt
                             }).SingleOrDefault();
                return model;
            }
        }
       
        /// <summary>
        /// 返回接收者所有消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public IQueryable<MIS_WebIM_MessageRecModel> GetMessageAllByReceiver(string receiver)
        {
            var modelList = (from m in Context.MIS_WebIM_Message
                             join r in Context.MIS_WebIM_Message_Rec on m.Id equals r.MessageId
                             where r.receiver == receiver
                             select new MIS_WebIM_MessageRecModel
                             {
                                 Id = m.Id,
                                 Message = m.Message,
                                 Sender = m.Sender,
                                 receiver = r.receiver,
                                 State = r.State,
                                 SendDt = m.SendDt,
                                 RecDt = r.RecDt
                             }).AsQueryable();

            return modelList;

        }
    }
}
