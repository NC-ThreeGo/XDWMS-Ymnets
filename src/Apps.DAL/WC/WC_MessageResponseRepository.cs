using Apps.Common;
using Apps.Models;
using Apps.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.DAL.WC
{
    public partial class WC_MessageResponseRepository
    {
        public bool PostData(WC_MessageResponse model)
        {
            //如果所有开关都关掉，证明不启用回复
            if (model.Category == null)
            {
                return true;
            }
            //全部设置为不默认
            ExecuteSqlCommand(string.Format("update [dbo].[WC_MessageResponse] set IsDefault=0 where OfficalAccountId ='{0}' and MessageRule={1}", ResultHelper.Formatstr(model.OfficalAccountId), model.MessageRule));
            //默认回复和订阅回复,且不是图文另外处理，因为他们有3种模式，但是只有一个是默认的
            if (model.Category!= (int)WeChatReplyCategory.Image && (model.MessageRule == (int)WeChatRequestRuleEnum.Default || model.MessageRule == (int)WeChatRequestRuleEnum.Subscriber))
            {
                //查看数据库是否存在数据
                var entity = Context.WC_MessageResponse.Where(p => p.OfficalAccountId == model.OfficalAccountId && p.MessageRule == model.MessageRule && p.Category == model.Category).FirstOrDefault();
                if (entity != null)
                {
                    //删除原来的
                    Context.WC_MessageResponse.Remove(entity);
                }
            }
            //全部设置为默认
            ExecuteSqlCommand(string.Format("update [dbo].[WC_MessageResponse] set IsDefault=1 where OfficalAccountId ='{0}' and MessageRule={1} and Category={2}", ResultHelper.Formatstr(model.OfficalAccountId), model.MessageRule,model.Category));
            //修改
            if(IsExist(model.Id))
            {
               Context.Entry<WC_MessageResponse>(model).State = EntityState.Modified;
               return Edit(model);
            }
            else { 
               return Create(model);
            }
        }


        public List<P_WC_GetResponseContent_Result> GetResponseContent(string officalAccountId, string matchKey)
        {
            return Context.P_WC_GetResponseContent(officalAccountId, matchKey).ToList();
        }

        /// <summary>
        /// 获得订阅时候回复的内容
        /// </summary>
        /// <param name="officalAccountId"></param>
        /// <returns></returns>
        public List<WC_MessageResponse> GetSubscribeResponseContent(string officalAccountId)
        {
            return Context.WC_MessageResponse.Where(a=>a.OfficalAccountId==officalAccountId && a.MessageRule==(int)WeChatRequestRuleEnum.Subscriber && a.IsDefault).ToList();
        }
    }
}
