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
    public partial class Flow_StepRuleBLL
    {
        [Dependency]
        public IFlow_FormAttrRepository attrRep { get; set; }
        [Dependency]
        public IFlow_StepRepository stepRep { get; set; }
        public List<Flow_StepRuleModel> GetList(string stepId)
        {

            IQueryable<Flow_StepRule> queryData = null;

                queryData = m_Rep.GetList(a => a.StepId==stepId);

           
            return CreateModelList(ref queryData);
        }
        public override List<Flow_StepRuleModel> CreateModelList(ref IQueryable<Flow_StepRule> queryData)
        {

            List<Flow_StepRuleModel> modelList = (from r in queryData
                                                  select new Flow_StepRuleModel
                                                  {
                                                      Id = r.Id,
                                                      StepId = r.StepId,
                                                      AttrId = r.AttrId,
                                                      Operator = r.Operator,
                                                      Result = r.Result,
                                                      NextStep = r.NextStep
                                                  }).ToList();
            foreach (var r in modelList)
            {
                r.AttrName = attrRep.GetById(r.AttrId).Title;
                r.NextStepName = r.NextStep == "0" ? "结束流程" : stepRep.GetById(r.NextStep).Name;
            }
            return modelList;
        }


        public bool ValidAttr(string attrId,string Result)
        {
            Flow_FormAttr stepModel = attrRep.GetById(attrId);
            if (stepModel.AttrType == "数字")
            {
                try
                {
                    Convert.ToInt32(Result);
                    return true;
                }
                catch {
                    return false;
                }
            }
            if (stepModel.AttrType == "日期")
            {
                try
                {
                    Convert.ToDateTime(Result);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Create(ref ValidationErrors errors, Flow_StepRuleModel model)
        {
            try
            {
                if (!ValidAttr(model.AttrId,model.Result))
                {
                    errors.Add("条件验证失败,类型和值不相符，如数字不能和字符串,日期类型必须是:2014-1-1格式");
                    return false;
                }
                //判断条件是否可行
               

                Flow_StepRule entity = m_Rep.GetById(model.Id);
                if (entity != null)
                {
                    errors.Add(Resource.PrimaryRepeat);
                    return false;
                }
                entity = new Flow_StepRule();
                entity.Id = model.Id;
                entity.StepId = model.StepId;
                entity.AttrId = model.AttrId;
                entity.Operator = model.Operator;
                entity.Result = model.Result;
                entity.NextStep = model.NextStep;
                if (m_Rep.Create(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.InsertFail);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }

    }
}
