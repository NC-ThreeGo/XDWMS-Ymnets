using Apps.Common;
using Apps.BLL.MIS;
using Apps.DAL.MIS;
using Apps.IBLL.MIS;
using Apps.Models;
using Apps.Models.JOB;
using Apps.Models.MIS;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Jobs.MIS
{
    public class DiscussPostNewTopicsJob : ITaskJob
    {

        public string RunJob(ref JobDataMap dataMap, string jobName, string id, string taskName)
        {

            IMIS_ArticleBLL discussArticleBLL = new MIS_ArticleBLL()
            {
                m_Rep = new MIS_ArticleRepository(new DBContainer())
            };

            MIS_ArticleModel model = discussArticleBLL.GetById(id);
            string retResult = "";
            if (model == null)
            {
                retResult = "文章不存在";
                return retResult;
            }
            model.CheckFlag =0;

            ValidationErrors validationErrors = new ValidationErrors();

            discussArticleBLL.Edit(ref validationErrors, model);

            if (validationErrors.Count > 0)
            {
                return validationErrors.Error;
            }
            retResult = "修改成功";
            return retResult;
        }

        public string RunJobBefore(JobModel jobModel)
        {
            Log.Write("RunJobBefor", jobModel.taskName,"运行");
            ValidationErrors validationErrors = new ValidationErrors();
            IMIS_ArticleBLL discussArticleBLL = new MIS_ArticleBLL()
            {
                m_Rep = new MIS_ArticleRepository(new DBContainer())
            };
            var model = discussArticleBLL.GetById(jobModel.id);
            if (model == null)
            {

                return "参数不能为空!";
            }

            model.CheckFlag = 1;
            if (discussArticleBLL.Edit(ref validationErrors, model))
            {
                return null;
            }
            else
            {
                return validationErrors.Error;
            }


        }


        public string CloseJob(JobModel jobModel)
        {
            Log.Write("CloseJob", jobModel.taskName,"关闭");
            ValidationErrors validationErrors = new ValidationErrors();
            IMIS_ArticleBLL discussArticleBLL = new MIS_ArticleBLL()
            {
                m_Rep = new MIS_ArticleRepository(new DBContainer())
            };
            var model = discussArticleBLL.GetById(jobModel.id);
            if (model == null)
            {

                return "参数不能为空!";
            }

            model.CheckFlag = 1;
            if (discussArticleBLL.Edit(ref validationErrors, model))
            {
                return null;
            }
            else
            {
                return validationErrors.Error;
            }

        }
    }
}
