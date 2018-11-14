using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.DAL;
using Apps.BLL.Core;
using Apps.Common;
using Apps.Models;
using Apps.IDAL;
using Apps.Locale;
using Apps.Models.JOB;
namespace Apps.BLL.JOB
{
    public partial class JOB_TASKJOBS_LOGBLL
    {
      
        public bool DeleteBySno(ref ValidationErrors errors, string sno)
        {
            try
            {

                if (m_Rep.DeleteBySno(sno) <1)
                {
                    errors.Add("删除出错!");
                    return false;
                }
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
        public JOB_TASKJOBS_LOGModel GetModelById(int itemId)
        {
            var entity = m_Rep.GetById(itemId);
            if (entity == null)
            {
                return null;
            }
            JOB_TASKJOBS_LOGModel model = new JOB_TASKJOBS_LOGModel();

            //实现对象到模型转换
            model.itemID = entity.itemID;
            model.sno = entity.sno;
            model.taskName = entity.taskName;
            model.Id = entity.Id;
            model.executeDt = entity.executeDt;
            model.executeStep = entity.executeStep;
            model.result = entity.result;

            return model;
        }

        //返回查询模型列表
        public List<JOB_TASKJOBS_LOGModel> GetListBySno(ref GridPager pager, string sno)
        {

            if (string.IsNullOrEmpty(sno))
            {
                return null;
            }
            IQueryable<JOB_TASKJOBS_LOG> queryData;
            queryData = m_Rep.GetList(a => a.sno == sno).OrderByDescending(a => a.itemID);
            if (queryData == null)
            {
                return null;
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
            List<JOB_TASKJOBS_LOGModel> modelList = (from r in queryData
                                                     select new JOB_TASKJOBS_LOGModel
                                                     {
                                                         itemID = r.itemID,
                                                         sno = r.sno,
                                                         taskName = r.taskName,
                                                         Id = r.Id,
                                                         executeDt = r.executeDt,
                                                         executeStep = r.executeStep,
                                                         result = r.result,
                                                     }).ToList();

            return modelList;
        }


    }
}
