using Apps.Common;
using Apps.BLL.WC;
using Apps.DAL.WC;
using Apps.IBLL.WC;
using Apps.Models;
using Apps.Models.JOB;
using Apps.Models.WC;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.IDAL.WC;
using Apps.IDAL;
using Apps.DAL;

namespace Apps.Jobs.WC
{
    public class WeChatGetTokenJob : ITaskJob
    {

        public string RunJob(ref JobDataMap dataMap, string jobName, string id, string taskName)
        {

            using (IWC_OfficalAccountsRepository m_Rep = new WC_OfficalAccountsRepository(new DBContainer()))
            {

                IQueryable<WC_OfficalAccounts> queryable = m_Rep.GetList();
                ValidationErrors validationErrors = new ValidationErrors();
                foreach (var entity in queryable)
                {
                    if (!string.IsNullOrEmpty(entity.AppId) && !string.IsNullOrEmpty(entity.AppSecret))
                    {
                        entity.AccessToken = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(entity.AppId, entity.AppSecret).access_token;
                        entity.ModifyTime = ResultHelper.NowTime;
                    }
                }
                if(queryable.Count()>0)
                {
                    TaskJob.UpdateState(ref validationErrors, jobName, 1, "成功");
                    m_Rep.SaveChanges();
                }

                return "批量更新Access_Token！";
            }
        }

        public string RunJobBefore(JobModel jobModel)
        {
            Log.Write("RunJobBefor", jobModel.taskName,"运行");
            ValidationErrors validationErrors = new ValidationErrors();

            using (IWC_OfficalAccountsRepository m_Rep = new WC_OfficalAccountsRepository(new DBContainer()))
            {
                IQueryable<WC_OfficalAccounts> queryable = m_Rep.GetList();
                int count = queryable.Count();
                if (count < 1)
                {
                    return "没有符合获取Access_Token的数据！";
                }
                return null;
            }
          
        }


        public string CloseJob(JobModel jobModel)
        {
            ValidationErrors validationErrors = new ValidationErrors();

            Log.Write("CloseJob", jobModel.taskName,"关闭");
            TaskJob.UpdateState(ref validationErrors, jobModel.id, 3, "挂起");
            return "关闭获取Access_Token任务";
           
        }
    }
}
