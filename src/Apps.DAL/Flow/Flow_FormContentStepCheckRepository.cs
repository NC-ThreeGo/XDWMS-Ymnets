using System;
using System.Linq;
using Apps.IDAL.Flow;
using Apps.Models;
using System.Data;
using System.Data.SqlClient;

namespace Apps.DAL.Flow
{
    public partial class Flow_FormContentStepCheckRepository
    {

        public IQueryable<Flow_FormContentStepCheck> GetListByFormId(string formId,string contentId)
        {
            IQueryable<Flow_FormContentStepCheck> list = from a in Context.Flow_FormContentStepCheck
                                                         join b in Context.Flow_Step
                                                         on a.StepId equals b.Id
                                                         where b.FormId == formId & a.ContentId==contentId
                                                         select a;
            return list;
        }
        //驳回使用，重设状态
        public void ResetCheckStateByFormCententId(string stepCheckId, string contentId, int checkState, int checkFlag)
        {
            using (DBContainer db = new DBContainer())
            {
                string sql = @"update Flow_FormContentStepCheck set State=@CheckState where ContentId=@ContentId and id!=@stepCheckId
                        update Flow_FormContentStepCheckState set CheckFlag = @CheckFlag where Id in (select Id from Flow_FormContentStepCheckState where StepCheckId in
	                    (
                            select Id from Flow_FormContentStepCheck where ContentId = @ContentId and id!= @stepCheckId
	                    ))";
                SqlParameter[] para = new SqlParameter[]
                {
                new SqlParameter("@stepCheckId", stepCheckId),
                new SqlParameter("@ContentId", contentId),
                new SqlParameter("@CheckState", checkState),
                new SqlParameter("@CheckFlag", checkFlag)
                };
                ExecuteSqlCommand(sql, para);
            }
        }
    }
}
