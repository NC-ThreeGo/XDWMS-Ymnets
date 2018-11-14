using System;
using System.Collections.Generic;
using System.Linq;
using Apps.Common;
using Unity.Attributes;
using Apps.Models;
using Apps.BLL.Core;
using Apps.IDAL;
using Apps.Models.MIS;
using Apps.IDAL.MIS;
using Apps.IDAL.Sys;

namespace Apps.BLL.MIS
{
    public partial class MIS_WebIM_MessageBLL
    {
        // 数据库访问对象
        [Dependency]
        public IMIS_WebIM_MessageRepository repository { get; set; }

        [Dependency]
        public ISysUserRepository userRep { get; set; }

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="message">消息内容</param>
        /// <param name="sender">发送者(当前用户ID)</param>
        /// <param name="receiver">接收者</param>
        /// <param name="receiverTitle">接收者标题名称</param>
        /// <returns></returns>
        public string CreateMessage(ref ValidationErrors validationErrors, string message, string sender, string receiver, string receiverTitle)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    validationErrors.Add("消息内容不能为空");
                    return null;
                }
                if (string.IsNullOrEmpty(receiver))
                {
                    validationErrors.Add("接者不能为空");
                    return null;
                }
                if (string.IsNullOrEmpty(receiverTitle))
                {
                    validationErrors.Add("接者不能为空");
                    return null;
                }

                string id = Guid.NewGuid().ToString();
                repository.CreateMessage(id, message, sender, receiver, receiverTitle);
                return id;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="id">消息ID</param>
        /// <param name="message">消息内容</param>
        /// <param name="sender">发送者(当前用户ID)</param>
        /// <param name="receiver">接收者</param>
        /// <param name="receiverTitle">接收者标题名称</param>
        /// <returns></returns>
        public bool CreateMessage(ref ValidationErrors validationErrors, string id, string message, string sender, string receiver, string receiverTitle)
        {
            try
            {
                repository.CreateMessage(id, message, sender, receiver,receiverTitle);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="model">消息模型</param>
        /// <returns></returns>
        public bool CreateMessage(ref ValidationErrors validationErrors, MIS_WebIM_MessageModel model)
        {
            try
            {
                repository.CreateMessage(model.Id, model.Message, model.Sender, model.receiver,model.receiverTitle);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 删除接收者的所有消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="receiver">接收者</param>
        /// <returns></returns>
        public bool DeleteMessageAllByReceiver(ref ValidationErrors validationErrors, string receiver)
        {
            try
            {
                repository.DeleteMessageAllByReceiver(receiver);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 删除用户消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="userId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteMessageByUserId(ref ValidationErrors validationErrors, string userId, string[] ids)
        {
            try
            {
                repository.DeleteMessageNotBySender(userId, ids);

                for (int i = 0; i < ids.Length; i++)
                {
                    repository.DeleteMessageByReceiver(ids[i], userId);
                }
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 删除用户所有消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteMessageByUserId(ref ValidationErrors validationErrors, string userId)
        {
            try
            {
                repository.DeleteMessageAllNotBySender(userId);

                repository.DeleteMessageAllByReceiver(userId);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 删除发送者的所有消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="sender">发送者(当前用户ID)</param>
        /// <returns></returns>
        public bool DeleteMessageAllBySender(ref ValidationErrors validationErrors, string sender)
        {
            try
            {
                repository.DeleteMessageAllBySender(sender);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 删除接收者的一个消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="id">消息ID</param>
        /// <param name="receiver">接收者</param>
        /// <returns></returns>
        public bool DeleteMessageByReceiver(ref ValidationErrors validationErrors, string id, string receiver)
        {
            try
            {
                repository.DeleteMessageByReceiver(id, receiver);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 删除发送者的一个消息
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="id">消息ID</param>
        /// <returns></returns>
        public bool DeleteMessageBySender(ref ValidationErrors validationErrors, string id)
        {
            try
            {
                repository.DeleteMessageBySender(id);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 返回用户的所有通信记录
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageModel> GetMessage(ref GridPager pager,string userId)
        {
            try
            {
                int rowscount = 0;
                List < MIS_WebIM_MessageModel > modelList= repository.GetMessage(userId, pager.page, pager.rows, ref rowscount);

                pager.totalRows = rowscount;
                
                foreach (var model in modelList)
                {
                    model.SenderTitle = userRep.GetNameById(model.Sender);
                }
                return modelList;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        
        }
        /// <summary>
        /// 发送者发送的全部消息
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="sender">发送者(当前用户ID)</param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageModel> GetMesasgeAllBySender(ref GridPager pager, string sender)
        {
            try
            {
                IQueryable<MIS_WebIM_Message> queryData = repository.GetMesasgeAllBySender().Where(a => a.Sender == sender).OrderByDescending(a => a.Id);

                pager.totalRows = queryData.Count();
                if (pager.totalRows > 0)
                {
                    if (pager.page <= 1)
                    {
                        queryData = queryData.Take(pager.rows);
                    }
                    else
                    {
                        queryData = queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
                    }
                }
                List<MIS_WebIM_MessageModel> modelList = (from r in queryData
                                                    select new MIS_WebIM_MessageModel
                                                       {
                                                           Id = r.Id,
                                                           Message = r.Message,
                                                           Sender = r.Sender,
                                                           receiver = r.receiver,
                                                           State = r.State,
                                                           SendDt = r.SendDt,
                                                       }).ToList();
                foreach (var model in modelList)
                {
                    model.SenderTitle = userRep.GetNameById(model.Sender);
                    model.receiverTitle = userRep.GetNameById(model.receiver);
                }
                return modelList;
            }
            catch (Exception ex)
            {

                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 返回接收者所有消息        
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="receiver">接收者</param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageRecModel> GetMessageAllByReceiver(ref GridPager pager, string receiver)
        {
            try
            {
                IQueryable<MIS_WebIM_MessageRecModel> queryData = repository.GetMessageAllByReceiver(receiver).Where(a => a.receiver == receiver).OrderByDescending(a => a.SendDt);

                pager.totalRows = queryData.Count();
                if (pager.totalRows > 0)
                {
                    if (pager.page <= 1)
                    {
                        queryData = queryData.Take(pager.rows);
                    }
                    else
                    {
                        queryData = queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
                    }
                }

                List<MIS_WebIM_MessageRecModel> modelList = (from r in queryData
                                                       select new MIS_WebIM_MessageRecModel
                                                    {
                                                        Id = r.Id,
                                                        Message = r.Message,
                                                        Sender = r.Sender,
                                                        receiver = r.receiver,
                                                        State = r.State,
                                                        SendDt = r.SendDt,
                                                        RecDt=r.RecDt,
                                                    }).ToList();
                foreach (var model in modelList)
                {
                    model.SenderTitle = userRep.GetNameById(model.Sender);
                    model.receiverTitle = userRep.GetNameById(model.receiver);
                }

                return modelList;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 返回接收者所有消息
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="receiver">接收者</param>
        /// <param name="state">阅读状态,false未读,true已读</param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageRecModel> GetMessageAllByReceiver(ref GridPager pager, string receiver,bool state)
        {
            try
            {
                IQueryable<MIS_WebIM_MessageRecModel> queryData = repository.GetMessageAllByReceiver(receiver).Where(a => a.receiver == receiver && a.State==state).OrderByDescending(a => a.Id);

                pager.totalRows = queryData.Count();
                if (pager.totalRows > 0)
                {
                    if (pager.page <= 1)
                    {
                        queryData = queryData.Take(pager.rows);
                    }
                    else
                    {
                        queryData = queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
                    }
                }

                List<MIS_WebIM_MessageRecModel> modelList = (from r in queryData
                                                       select new MIS_WebIM_MessageRecModel
                                                       {
                                                           Id = r.Id,
                                                           Message = r.Message,
                                                           Sender = r.Sender,
                                                           receiver = r.receiver,
                                                           State = r.State,
                                                           SendDt = r.SendDt,
                                                           RecDt = r.RecDt,
                                                       }).ToList();
                foreach (var model in modelList)
                {
                    model.SenderTitle = userRep.GetNameById(model.Sender);
                    model.receiverTitle = userRep.GetNameById(model.receiver);
                }

                return modelList;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 返回指定发送者到接收者(未读/已读)消息
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="sender">发送者</param>
        /// <param name="receiver">接收者</param>
        /// <param name="state">阅读状态,false未读,true已读</param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageRecModel> GetMessageFromSenderToReceiver(ref GridPager pager, string sender, string receiver, bool state)
        {
            try
            {
                IQueryable<MIS_WebIM_MessageRecModel> queryData = repository.GetMessageAllByReceiver(receiver).Where(a=>a.Sender==sender && a.State == state).OrderByDescending(a => a.SendDt);

                pager.totalRows = queryData.Count();
                if (pager.totalRows > 0)
                {
                    if (pager.page <= 1)
                    {
                        queryData = queryData.Take(pager.rows);
                    }
                    else
                    {
                        queryData = queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
                    }
                }

                List<MIS_WebIM_MessageRecModel> modelList = (from r in queryData
                                                       select new MIS_WebIM_MessageRecModel
                                                       {
                                                           Id = r.Id,
                                                           Message = r.Message,
                                                           Sender = r.Sender,
                                                           receiver = r.receiver,
                                                           State = r.State,
                                                           SendDt = r.SendDt,
                                                           RecDt = r.RecDt,
                                                       }).ToList();
                foreach (var model in modelList)
                {
                    model.SenderTitle = userRep.GetNameById(model.Sender);
                    model.receiverTitle = userRep.GetNameById(model.receiver);
                }

                return modelList;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 返回指定发送者到接收者所有消息
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="sender">发送者</param>
        /// <param name="receiver">接收者</param>
        /// <returns></returns>
        public List<MIS_WebIM_MessageRecModel> GetMessageFromSenderToReceiver(ref GridPager pager, string sender, string receiver)
        {
            try
            {
                IQueryable<MIS_WebIM_MessageRecModel> queryData = repository.GetMessageAllByReceiver(receiver).Where(a => a.receiver == receiver && a.Sender == sender).OrderBy(a => a.SendDt);

                pager.totalRows = queryData.Count();
                if (pager.totalRows > 0)
                {
                    if (pager.page <= 1)
                    {
                        queryData = queryData.Take(pager.rows);
                    }
                    else
                    {
                        queryData = queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
                    }
                }

                List<MIS_WebIM_MessageRecModel> modelList = (from r in queryData
                                                       select new MIS_WebIM_MessageRecModel
                                                       {
                                                           Id = r.Id,
                                                           Message = r.Message,
                                                           Sender = r.Sender,
                                                           receiver = r.receiver,
                                                           State = r.State,
                                                           SendDt = r.SendDt,
                                                           RecDt = r.RecDt,
                                                       }).ToList();
                foreach (var model in modelList)
                {
                    model.SenderTitle = userRep.GetNameById(model.Sender);
                    model.receiverTitle = userRep.GetNameById(model.receiver);
                }

                return modelList;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }        
        /// <summary>
        /// 返回发送信息给当前用户的发送者列表及未阅读信息数
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public List<MIS_WebIM_SenderModel> GetSenderByReceiver(string receiver)
        {
            try
            {
                return repository.GetSenderByReceiver(receiver);
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回接收者一个消息
        /// </summary>
        /// <param name="receiver">接收者</param>
        /// <param name="id">消息ID</param>
        /// <returns></returns>
        public MIS_WebIM_MessageRecModel GetMessageByReceiver(string receiver, string id)
        {
            try
            {
                return repository.GetMessageByReceiver(receiver, id);

            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 读取一个发送消息
        /// </summary>
        /// <param name="id">消息ID</param>
        /// <returns></returns>
        public MIS_WebIM_MessageModel GetMessageBySender(string id)
        {
            try
            {
                var entity = repository.GetMessageBySender(id);
                if (entity == null)
                {
                    return null;
                }
                MIS_WebIM_MessageModel model = new MIS_WebIM_MessageModel();
                model.Id = entity.Id;
                model.Message = entity.Message;
                model.Sender = entity.Sender;
                model.receiver = entity.receiver;
                model.State = entity.State;
                model.SendDt = entity.SendDt;
                model.SenderTitle = userRep.GetNameById(entity.Sender);
                model.receiverTitle = userRep.GetNameById(entity.receiver);

                return model;

            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 设置接收者的一条未阅读消息为已阅
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="receiver">接收者</param>
        /// <param name="id">消息ID</param>
        /// <returns></returns>
        public bool SetMessageHasReadById(ref ValidationErrors validationErrors, string receiver, string id)
        {
            try
            {
                repository.SetMessageHasReadById(receiver, id);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 设置接收者的所有未阅读消息为已阅
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="receiver">接收者</param>
        /// <returns></returns>
        public bool SetMessageHasReadByReceiver(ref ValidationErrors validationErrors, string receiver)
        {
            try
            {
                repository.SetMessageHasReadByReceiver(receiver);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// 修改一发送者到接收者的所有未阅读消息为已阅状态
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="sender">发送者</param>
        /// <param name="receiver">接收者</param>
        /// <returns></returns>
        public bool SetMessageHasReadFromSenderToReceiver(ref ValidationErrors validationErrors,string sender, string receiver)
        {
            try
            {
                repository.SetMessageHasReadFromSenderToReceiver(sender,receiver);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }
    }
}
