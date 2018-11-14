using System;
using System.Linq;
using Apps.IDAL.Flow;
using Apps.Models;
using System.Data;

namespace Apps.DAL.Flow
{
    public partial class Flow_FormContentRepository
    {
        /// <summary>
        /// 获取审批人的审批列表
        /// </summary>
        /// <param name="userId">审批人工号</param>
        /// <returns></returns>
        public IQueryable<Flow_FormContent> GeExamineListByUserId(string userId)
        {
            IQueryable<Flow_FormContent> list = (from a in Context.Flow_FormContent
                                                 join c in Context.Flow_FormContentStepCheck
                                                 on a.Id equals c.ContentId
                                                 join d in Context.Flow_FormContentStepCheckState
                                                 on c.Id equals d.StepCheckId
                                                 where d.UserId == userId && !a.IsDelete
                                                 select a).Distinct();
            return list;
        }

        public IQueryable<Flow_FormContent> GeExamineList()
        {
            IQueryable<Flow_FormContent> list = (from a in Context.Flow_FormContent
                                                 join b in Context.Flow_Step
                                                 on a.FormId equals b.FormId
                                                 join c in Context.Flow_FormContentStepCheck
                                                 on b.Id equals c.StepId
                                                 join d in Context.Flow_FormContentStepCheckState
                                                 on c.Id equals d.StepCheckId
                                                 select a).Distinct();
            return list;
        }

     
    }
}
