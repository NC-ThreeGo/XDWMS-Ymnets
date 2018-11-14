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
using Apps.Models.Enum;

namespace Apps.BLL.Flow
{
    public partial class Flow_StepBLL
    {
        public override List<Flow_StepModel> GetList(ref GridPager pager, string formId)
        {

            IQueryable<Flow_Step> queryData = null;

            queryData = m_Rep.GetList(a => a.FormId == formId).OrderBy(a=>a.Id);
        
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public override List<Flow_StepModel> CreateModelList(ref IQueryable<Flow_Step> queryData)
        {
            List<Flow_StepModel> modelList = (from r in queryData
                                              select new Flow_StepModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  Remark = r.Remark,
                                                  Sort = r.Sort,
                                                  FormId = r.FormId,
                                                  FlowRule = r.FlowRule,
                                                  IsCustom = r.IsCustom,
                                                  IsAllCheck = r.IsAllCheck,
                                                  Execution = r.Execution,
                                                  CompulsoryOver = r.CompulsoryOver,
                                                  IsEditAttr = r.IsEditAttr,

                                              }).ToList();

            DBContainer db = new DBContainer();
            List<string> users = new List<string>();
            foreach (var r in modelList)
            {
                if (!string.IsNullOrEmpty(r.Execution))
                {
                    string[] arr = r.Execution.Split(',');
                    users = db.SysUser.Where(a => arr.Contains(a.Id)).Select(a => a.TrueName).ToList();
                    r.Execution = string.Join(",", users.ToArray());
                }
                else
                {

                    r.Execution = r.FlowRule==(int)FlowRuleEnum.Lead?"按上级"
                        : r.FlowRule == (int)FlowRuleEnum.Person ? "按人员"
                        : r.FlowRule == (int)FlowRuleEnum.Customer ? "按申请人自选"
                        : r.FlowRule == (int)FlowRuleEnum.Position ? "按职位"
                        : r.FlowRule == (int)FlowRuleEnum.Department ? "按部门":"按规则";
                }
               
            }
            return modelList;
        }

    }
}
