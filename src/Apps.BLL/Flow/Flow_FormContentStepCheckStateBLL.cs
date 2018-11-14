using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.Models;
using Apps.Common;
using System.Transactions;
using Apps.Models.Flow;
using Apps.IBLL.Flow;
using Apps.IDAL.Flow;
using Apps.BLL.Core;
using Apps.Locale;

namespace Apps.BLL.Flow
{
    public partial class Flow_FormContentStepCheckStateBLL
    {
         public List<Flow_FormContentStepCheckStateModel> GetListByStepCheckId(ref GridPager pager, string stepCheckId)
        {

            IQueryable<Flow_FormContentStepCheckState> queryData = null;
            queryData = m_Rep.GetList(a => a.StepCheckId == stepCheckId);
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public override List<Flow_FormContentStepCheckStateModel> CreateModelList(ref IQueryable<Flow_FormContentStepCheckState> queryData)
        {

            List<Flow_FormContentStepCheckStateModel> modelList = (from r in queryData
                                                                   select new Flow_FormContentStepCheckStateModel
                                                                   {
                                                                       Id = r.Id,
                                                                       StepCheckId = r.StepCheckId,
                                                                       UserId = r.UserId,
                                                                       CheckFlag = r.CheckFlag,
                                                                       Reamrk = r.Reamrk,
                                                                       TheSeal = r.TheSeal,
                                                                       CreateTime = r.CreateTime,
                                                                       UserName = r.SysUser.TrueName

                                                                   }).ToList();

            return modelList;
        }

        public Flow_FormContentStepCheckStateModel GetByStepCheckId(string id)
        {
                Flow_FormContentStepCheckStateModel model = new Flow_FormContentStepCheckStateModel();
                Flow_FormContentStepCheckState entity = m_Rep.GetByStepCheckId(id);
                if (entity == null)
                {
                    return model;
                }
                
                model.Id = entity.Id;
                model.StepCheckId = entity.StepCheckId;
                model.UserId = entity.UserId;
                model.CheckFlag = entity.CheckFlag;
                model.Reamrk = entity.Reamrk;
                model.TheSeal = entity.TheSeal;
                model.CreateTime = entity.CreateTime;
                return model;
        }
     
    }
}
