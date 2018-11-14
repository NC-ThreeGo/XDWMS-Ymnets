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
using Apps.Models.Enum;
using Apps.Locale;

namespace Apps.BLL.Flow
{
    public partial class Flow_FormContentBLL
    {
        [Dependency]
        public IFlow_StepBLL stepBLL { get; set; }
        [Dependency]
        public IFlow_StepRuleBLL stepRuleBLL { get; set; }
        [Dependency]
        public IFlow_FormContentStepCheckBLL stepCheckBLL { get; set; }
        [Dependency]
        public IFlow_FormContentStepCheckStateBLL stepCheckStateBLL { get; set; }
      
        public List<Flow_FormContentModel> GetListByUserId(ref GridPager pager, string queryStr, string userId)
        {
            IQueryable<Flow_FormContent> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(a => a.Title.Contains(queryStr) && a.UserId==userId);
            }
            else
            {
                queryData = m_Rep.GetList(a=>a.UserId == userId);
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public List<Flow_FormContentModel> GeExaminetListByUserId(ref GridPager pager, string queryStr, string userId)
        {
            IQueryable<Flow_FormContent> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GeExamineListByUserId(userId).Where(a => a.Title.Contains(queryStr));
            }
            else
            {
                queryData = m_Rep.GeExamineListByUserId(userId);
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }


        public override List<Flow_FormContentModel> CreateModelList(ref IQueryable<Flow_FormContent> queryData)
        {

            List<Flow_FormContentModel> modelList = (from r in queryData
                                                     select new Flow_FormContentModel
                                                     {
                                                         Id = r.Id,
                                                         Title = r.Title,
                                                         UserId = r.UserId,
                                                         UserName = r.SysUser.TrueName,
                                                         FormId = r.FormId,
                                                         FormLevel = r.FormLevel,
                                                         CreateTime = r.CreateTime,
                                                         AttrA = r.AttrA,
                                                         AttrB = r.AttrB,
                                                         AttrC = r.AttrC,
                                                         AttrD = r.AttrD,
                                                         AttrE = r.AttrE,
                                                         AttrF = r.AttrF,
                                                         AttrG = r.AttrG,
                                                         AttrH = r.AttrH,
                                                         AttrI = r.AttrI,
                                                         AttrJ = r.AttrJ,
                                                         AttrK = r.AttrK,
                                                         AttrL = r.AttrL,
                                                         AttrM = r.AttrM,
                                                         AttrN = r.AttrN,
                                                         AttrO = r.AttrO,
                                                         AttrP = r.AttrP,
                                                         AttrQ = r.AttrQ,
                                                         AttrR = r.AttrR,
                                                         AttrS = r.AttrS,
                                                         AttrT = r.AttrT,
                                                         AttrU = r.AttrU,
                                                         AttrV = r.AttrV,
                                                         AttrW = r.AttrW,
                                                         AttrX = r.AttrX,
                                                         AttrY = r.AttrY,
                                                         AttrZ = r.AttrZ,
                                                         CustomMember = r.CustomMember,
                                                         TimeOut = r.TimeOut,
                                                         IsDelete = r.IsDelete,
                                                         ExternalURL =r.Flow_Form.ExternalURL,
                                                         IsExternal = r.Flow_Form.IsExternal
                                                     }).ToList();

            return modelList;
        }

        public List<Flow_FormContentModel> GeExaminetList(ref GridPager pager, string queryStr)
        {
            IQueryable<Flow_FormContent> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GeExamineList().Where(a => a.Title.Contains(queryStr));
            }
            else
            {
                queryData = m_Rep.GeExamineList();
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

    
        public int GetCurrentFormState(Flow_FormContentModel model)
        {
            if (model.TimeOut < ResultHelper.NowTime)
            {
                return (int)FlowStateEnum.Closed;
            }
            List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(model.FormId, model.Id);


            var passList = from r in stepCheckModelList where r.State == (int)FlowStateEnum.Pass select r;
            if (passList.Count() == stepCheckModelList.Count())
            {
                return (int)FlowStateEnum.Pass;
            }
            var rejectList = from r in stepCheckModelList where r.State == (int)FlowStateEnum.Reject select r;
            if (rejectList.Count() > 0)
            {
                return (int)FlowStateEnum.Reject;
            }
            return (int)FlowStateEnum.Progress;
        }
        public string GetCurrentFormStep(Flow_FormContentModel model)
        {
            string stepName = GetCurrentFormStepModel(model).Name;
            if(!string.IsNullOrEmpty(stepName))
            {
                return stepName;
            }
            return "结束";
        }

        public Flow_StepModel GetCurrentFormStepModel(Flow_FormContentModel model)
        {
            List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(model.FormId, model.Id);
            for (int i =0;i< stepCheckModelList.Count();i++)
            {
                if (stepCheckModelList[i].State == (int)FlowStateEnum.Progress)
                {
                    return stepBLL.GetById(stepCheckModelList[i].StepId);
                }
            }
            return new Flow_StepModel();
        }
       
      
        //获取环节所有信息
        public string GetCurrentStepCheckMes(ref GridPager pager, string formId, string contentId,string currentUserId)
        {
            string stepCheckMes = "";
            List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(formId, contentId);
            for (int i = 0; i < stepCheckModelList.Count; i++)
            {
                List<Flow_FormContentStepCheckStateModel> stepCheckStateList = stepCheckStateBLL.GetListByStepCheckId(ref pager, stepCheckModelList[i].Id);
                stepCheckMes = stepCheckMes + "<tr><th style='width:150px'>第" + (i + 1) + "步--->审核人：</th><td><table >";
                foreach (Flow_FormContentStepCheckStateModel checkStateModel in stepCheckStateList)
                {
                    stepCheckMes += "<tr class='" + (checkStateModel.UserId == currentUserId ? "color-green" : "") + "'><td>" + checkStateModel.UserName + " </td><th style='width:90px'> 审核意见：</th><td>" + checkStateModel.Reamrk + "</td><th style='width:90px'>审核结果：</th><td>" + (checkStateModel.CheckFlag == (int)FlowStateEnum.Pass ? "通过" : checkStateModel.CheckFlag == (int)FlowStateEnum.Reject ? "驳回" : "审核中") + "</td></tr>";
                }
                if (stepCheckStateList.Count == 0)
                {
                    stepCheckMes += "<tr><td>【等待上一步审核】</td><th> </th><td></td><th></th><td></td></tr>";
                }
                stepCheckMes += "</table></td></tr>";
            }
            return stepCheckMes;
        }
        //获取当前环节的审核条
        public string GetCurrentStepCheckId(string formId, string contentId)
        {
            List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(formId, contentId);
            return new FlowHelper().GetCurrentStepCheckIdByStepCheckModelList(stepCheckModelList);

        }

        public override Flow_FormContentModel GetById(object id)
        {
            if (IsExists(id))
            {
                Flow_FormContent entity = m_Rep.GetById(id);
                Flow_FormContentModel model = new Flow_FormContentModel();
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.UserId = entity.UserId;
                model.UserName = entity.SysUser.UserName;
                model.TrueName = entity.SysUser.TrueName;
                model.PosName = entity.SysUser.SysPosition.Name;
                model.DepName = entity.SysUser.SysStruct.Name;
                model.FormId = entity.FormId;
                model.FormLevel = entity.FormLevel;
                model.CreateTime = entity.CreateTime;
                model.AttrA = entity.AttrA;
                model.AttrB = entity.AttrB;
                model.AttrC = entity.AttrC;
                model.AttrD = entity.AttrD;
                model.AttrE = entity.AttrE;
                model.AttrF = entity.AttrF;
                model.AttrG = entity.AttrG;
                model.AttrH = entity.AttrH;
                model.AttrI = entity.AttrI;
                model.AttrJ = entity.AttrJ;
                model.AttrK = entity.AttrK;
                model.AttrL = entity.AttrL;
                model.AttrM = entity.AttrM;
                model.AttrN = entity.AttrN;
                model.AttrO = entity.AttrO;
                model.AttrP = entity.AttrP;
                model.AttrQ = entity.AttrQ;
                model.AttrR = entity.AttrR;
                model.AttrS = entity.AttrS;
                model.AttrT = entity.AttrT;
                model.AttrU = entity.AttrU;
                model.AttrV = entity.AttrV;
                model.AttrW = entity.AttrW;
                model.AttrX = entity.AttrX;
                model.AttrY = entity.AttrY;
                model.AttrZ = entity.AttrZ;
                model.CustomMember = entity.CustomMember;
                model.TimeOut = entity.TimeOut;
                model.IsDelete = entity.IsDelete;

                return model;
            }
            else
            {
                return null;
            }
        }

    }
}
