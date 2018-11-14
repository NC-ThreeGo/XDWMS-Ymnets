using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.DAL;
using System.Text.RegularExpressions;
using Apps.BLL.Core;
using Apps.IDAL;
using Apps.Common;
using Apps.Models;
using Apps.Locale;
using System.Transactions;
using Apps.Models.JOB;
using Apps.IDAL.JOB;
using Apps.IDAL.Sys;

namespace Apps.BLL.JOB
{
    public partial class JOB_TASKJOBSBLL
    {
        // 数据库访问对象
        [Dependency]
        public IJOB_TASKJOBS_LOGRepository taskJobLogRep { get; set; }

        [Dependency]
        public ISysUserRepository userRep { get; set; }

     
        /// <summary>
        /// 更新任务状态(0=准备，1=完成，2=关闭)
        /// </summary>
        /// <param name="error"></param>
        /// <param name="sno"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateState(ref ValidationErrors errors, string sno,int state,string result)
        {
            try
            {
                if (string.IsNullOrEmpty(sno))
                {
                    errors.Add("任务序号不能为空");
                    return false;
                }
                if (sno.Length < 51)
                {
                    errors.Add("任务序号不正确");
                    return false;
                }

                if (!Regex.IsMatch(sno, @"App([.0-9a-zA-Z]+_[.0-9a-zA-Z]+){1}"))
                {
                    errors.Add("任务序号不正确");
                    return false;
                }
                
                m_Rep.UpdateState(sno, state,result);

                return true;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                errors.Add("修改异常!");
                return false;
            }
        }
     

        public bool DeleteJob(ref ValidationErrors errors, string sno)
        {
            try
            {
                m_Rep.DeleteJob(sno);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHander.WriteException(ex);
                errors.Add("删除异常");
                return false;
            }

        }
    
     

        //根据主键获取模型
        public JOB_TASKJOBSModel GetModelById(string sno)
        {
            var entity = m_Rep.GetById(sno);
            if (entity == null)
            {
                return null;
            }
            JOB_TASKJOBSModel model = new JOB_TASKJOBSModel();

            //实现对象到模型转换
            model.sno = entity.sno;
            model.taskName = entity.taskName;
            model.Id = entity.Id;
            model.taskTitle = entity.taskTitle;
            model.taskCmd = entity.taskCmd;
            model.crtDt = entity.crtDt;
            model.state = entity.state;
            model.creator = entity.creator;

            return model;
        }

        //返回查询模型列表
        public List<JOB_TASKJOBSModel> GetListByCustom(ref GridPager pager, string querystr)
        {
            IQueryable<JOB_TASKJOBS> queryData;
            if (!string.IsNullOrEmpty(querystr))
            {
                queryData = m_Rep.GetList(a => a.sno.Contains(querystr) || a.taskTitle.Contains(querystr)).OrderByDescending(a => a.sno + a.taskName);

            }
            else
            {
                queryData = m_Rep.GetList().OrderByDescending(a => a.sno + a.taskName);
            }
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
            List<JOB_TASKJOBSModel> modelList = (from r in queryData
                                                 select new JOB_TASKJOBSModel
                                                 {
                                                     sno = r.sno,
                                                     taskName = r.taskName,
                                                     Id = r.Id,
                                                     taskTitle = r.taskTitle,
                                                     taskCmd = r.taskCmd,
                                                     crtDt = r.crtDt,
                                                     state = r.state,
                                                     creator = r.creator,
                                                     procName=r.procName,
                                                     procParams=r.procParams
                                                 }).ToList();
            foreach (var model in modelList)
            {
                model.creator = userRep.GetNameById(model.creator);
            }

            return modelList;
        }
        public List<P_JOB_GetUnTrackTask_Result> GetUnTrackTask()
        {
            return m_Rep.GetUnTrackTask();

        }
    }
}
