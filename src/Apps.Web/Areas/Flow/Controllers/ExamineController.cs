using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Sys;
using Unity.Attributes;
using Apps.IBLL.Flow;
using Apps.Models.Flow;
using System.Text;
using Apps.BLL.Flow;
using Apps.Web.Core;
using Apps.Models.Enum;
using Apps.Locale;
using Apps.IBLL.Sys;

namespace Apps.Web.Areas.Flow.Controllers
{
    public class ExamineController : BaseController
    {
        [Dependency]
        public ISysUserBLL userBLL { get; set; }
        [Dependency]
        public IFlow_TypeBLL m_BLL { get; set; }
        [Dependency]
        public IFlow_FormBLL formBLL { get; set; }
        [Dependency]
        public IFlow_FormAttrBLL formAttrBLL { get; set; }
        [Dependency]
        public IFlow_FormContentBLL formContentBLL { get; set; }
        [Dependency]
        public IFlow_StepBLL stepBLL { get; set; }
        [Dependency]
        public IFlow_StepRuleBLL stepRuleBLL { get; set; }
        [Dependency]
        public IFlow_FormContentStepCheckBLL stepCheckBLL { get; set; }
        [Dependency]
        public IFlow_FormContentStepCheckStateBLL stepCheckStateBLL { get; set; }
        [Dependency]
        public IFlow_ExternalBLL externalBLL { get; set; }



        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public JsonResult GetListByUserId(GridPager pager, string queryStr)
        {
            List<Flow_FormContentModel> list = formContentBLL.GeExaminetListByUserId(ref pager, queryStr, GetUserId());
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new Flow_FormContentModel()
                        {

                            Id = r.Id,
                            Title = r.Title,
                            UserId = r.UserId,
                            UserName = r.UserName,
                            FormId = r.FormId,
                            FormLevel = r.FormLevel,
                            CreateTime = r.CreateTime,
                            TimeOut = r.TimeOut,
                            CurrentStep = formContentBLL.GetCurrentFormStep(r),
                            CurrentState = formContentBLL.GetCurrentFormState(r),
                            IsExternal = r.IsExternal,
                            ExternalURL = r.ExternalURL
                        }).ToArray()

            };
            return Json(json);
        }



        #region 详细
        [SupportFilter(ActionName = "Details")]
        public ActionResult Details(string id, string contentId)
        {

            Flow_FormModel flowFormModel = formBLL.GetById(id);
            Flow_FormContentModel centent = formContentBLL.GetById(contentId);
            string currentStepName = formContentBLL.GetCurrentFormStep(centent);

            flowFormModel.stepList = new List<Flow_StepModel>();
            flowFormModel.stepList = stepBLL.GetList(ref setNoPagerAscById, flowFormModel.Id);
            for (int i = 0; i < flowFormModel.stepList.Count; i++)//获取步骤下面的步骤规则
            {
                flowFormModel.stepList[i].stepRuleList = new List<Flow_StepRuleModel>();
                flowFormModel.stepList[i].stepRuleList = GetStepRuleListByStepId(flowFormModel.stepList[i].Id);
                if (flowFormModel.stepList[i].Name == currentStepName)
                {
                    flowFormModel.stepList[i].Name = "<span class='label label-green'>" + flowFormModel.stepList[i].Name + "</span>";
                }
            }

            return View(flowFormModel);
        }
        //获取步骤下面的规则
        private List<Flow_StepRuleModel> GetStepRuleListByStepId(string stepId)
        {
            List<Flow_StepRuleModel> list = stepRuleBLL.GetList(stepId);
            return list;
        }
        #endregion

        [SupportFilter]
        public ActionResult Edit(string formId, string id)
        {
            //获得当前步骤ID
            string currentStepId = formContentBLL.GetCurrentStepCheckId(formId, id);
            
            Flow_FormModel formModel = formBLL.GetById(formId);
            //是否已经设置布局
            if (!string.IsNullOrEmpty(formModel.HtmlForm))
            {
                ViewBag.Html = formModel.HtmlForm;
            }
            else
            {
                ViewBag.Html = ExceHtmlJs(formId);
            }
            ViewBag.StepCheckMes = formContentBLL.GetCurrentStepCheckMes(ref setNoPagerAscById, formId, id, GetUserId());
            //获取可审核步骤的人
            List<Flow_FormContentStepCheckStateModel> stepCheckStateModel = stepCheckStateBLL.GetListByStepCheckId(ref setNoPagerAscById, currentStepId);
            //决定是否显示审核框
            string currentUser = GetUserId();
            if (stepCheckStateModel.Where(a => a.UserId == currentUser).Count() == 0)
            {
                ViewBag.StepCheckId = "";
            }
            else
            {
                ViewBag.StepCheckId = currentStepId;
            }


            ViewBag.IsCustomMember = false;

            if(!string.IsNullOrEmpty(currentStepId))
            { 
                List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(formId,id);
                int j = 0;//下一个步骤
                for (int i = 0; i < stepCheckModelList.Count(); i++)
                {
                    if (currentStepId == stepCheckModelList[i].Id)
                    {
                        j = i;
                    }
                }
                //获得下个步骤
                if (j + 1 < stepCheckModelList.Count())
                { 
                    //查询第二步是否是自选
                    Flow_FormContentStepCheckModel stepModel = stepCheckModelList[j + 1];
                    if (stepModel.IsCustom)
                    {
                        ViewBag.IsCustomMember = true;
                    }

                }
            }
            Flow_FormContentModel model = formContentBLL.GetById(id);
            return View(model);
        }
        //批阅
        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(string Remark, string TheSeal, string FormId, int Flag, string ContentId,string UserList)
        {
            string stepCheckId = formContentBLL.GetCurrentStepCheckId(FormId, ContentId);
            if (stepCheckId == "")
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
            string currentUser = GetUserId();
            List<Flow_FormContentStepCheckStateModel> stepCheckStateModelList = stepCheckStateBLL.GetListByStepCheckId(ref setNoPagerAscById, stepCheckId);
            Flow_FormContentStepCheckStateModel stepCheckStateModel = stepCheckStateModelList.SingleOrDefault(a => a.UserId == currentUser);
            //决定是否显示审核框
            if (stepCheckStateModel == null)
            {
                return Json(JsonHandler.CreateMessage(0, "越权操作！"));
            }
            stepCheckStateModel.Reamrk = Remark;
            stepCheckStateModel.TheSeal = TheSeal;
            stepCheckStateModel.CheckFlag = Flag;
            if (stepCheckStateBLL.Edit(ref errors, stepCheckStateModel))
            {
                //获取当前步骤
                Flow_FormContentStepCheckModel stepCheckModel = stepCheckBLL.GetById(stepCheckStateModel.StepCheckId);
                //获得当前的步骤模板
                Flow_StepModel currentStepModel = stepBLL.GetById(stepCheckModel.StepId);
                //驳回直接终止审核
                if(Flag==(int)FlowStateEnum.Reject)
                {
                    stepCheckModel.State = Flag;
                    stepCheckModel.StateFlag = false;
                    stepCheckBLL.Edit(ref errors, stepCheckModel);
                    //重置所有步骤的状态
                    stepCheckBLL.ResetCheckStateByFormCententId(stepCheckModel.Id, ContentId, (int)FlowStateEnum.Progress, (int)FlowStateEnum.Progress);
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk, "成功", "修改", "Flow_FormContentStepCheckState");
                    return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
                }
                else if (currentStepModel.IsAllCheck)
                {
                    //启用会签
                    //获得同步骤的同批审核人
                    List<Flow_FormContentStepCheckStateModel> stepCheckStateList = stepCheckStateBLL.GetListByStepCheckId(ref setNoPagerAscById, stepCheckStateModel.StepCheckId);
                    //查看自己是否是最后一个审核人
                    bool complete = stepCheckStateList.Where(a => a.CheckFlag == (int)FlowStateEnum.Progress).Count() == 1;
                    if (complete)
                    {
                        stepCheckModel.State = Flag;
                        stepCheckModel.StateFlag = true;
                        stepCheckBLL.Edit(ref errors, stepCheckModel);
                    }
                    else {
                        //让审核人继续执行这个步骤直到完成
                        LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk, "成功", "修改", "Flow_FormContentStepCheckState");
                        return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
                    }
                }
                else
                {
                    //不是会签，任何一个审批都通过
                    stepCheckModel.State = Flag;
                    stepCheckModel.StateFlag = true;
                    stepCheckBLL.Edit(ref errors, stepCheckModel);
                }
                //查看下一步是否为自创建
                if (!stepCheckModel.IsEnd && !string.IsNullOrEmpty(UserList))
                {
                    List<Flow_FormContentStepCheckModel> stepCheckList = stepCheckBLL.GetListByFormId(FormId, ContentId);
                    int j = 0;
                    for (int i = stepCheckList.Count() - 1; i >= 0; i--)
                    {
                        if (stepCheckId == stepCheckList[i].Id)
                        {
                            j = i;
                        }
                    }
                    //查看是否还有下一步步骤
                    if(j-1<=stepCheckList.Count())
                    {
                        //查有第二步骤，查看是否是自选
                        Flow_StepModel stepModel = stepBLL.GetById(stepCheckList[j + 1].StepId);
                        if (stepModel.FlowRule==(int)FlowRuleEnum.Customer)
                        {
                            foreach (string userId in UserList.Split(','))
                            {
                                //批量建立步骤审核人表
                                CreateCheckState(stepCheckList[j + 1].Id, userId);
                            }
                        }
                        else {
                            //批量建立审核人员表
                            foreach (string userId in GetStepCheckMemberList(stepCheckList[j + 1].StepId))
                            {
                                //批量建立步骤审核人表
                                CreateCheckState(stepCheckList[j + 1].Id, userId);
                            }
                        }

                    }
                    
                }


                LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk, "成功", "修改", "Flow_FormContentStepCheckState");
                return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk + "," + ErrorCol, "失败", "修改", "Flow_FormContentStepCheckState");
                return Json(JsonHandler.CreateMessage(0, Resource.CheckFail + ErrorCol));
            }

        }


        [SupportFilter(ActionName ="Edit")]
        public ActionResult EditExternal(string formId, string id)
        {
            //获得当前步骤ID
            string currentStepId = formContentBLL.GetCurrentStepCheckId(formId, id);

            Flow_FormModel formModel = formBLL.GetById(formId);
           
            ViewBag.Html = formModel.ExternalURL.Split(',')[2];
       
            ViewBag.StepCheckMes = formContentBLL.GetCurrentStepCheckMes(ref setNoPagerAscById, formId, id, GetUserId());
            //获取可审核步骤的人
            List<Flow_FormContentStepCheckStateModel> stepCheckStateModel = stepCheckStateBLL.GetListByStepCheckId(ref setNoPagerAscById, currentStepId);
            //决定是否显示审核框
            string currentUser = GetUserId();
            if (stepCheckStateModel.Where(a => a.UserId == currentUser).Count() == 0)
            {
                ViewBag.StepCheckId = "";
            }
            else
            {
                ViewBag.StepCheckId = currentStepId;
            }


            ViewBag.IsCustomMember = false;

            if (!string.IsNullOrEmpty(currentStepId))
            {
                List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(formId, id);
                int j = 0;//下一个步骤
                for (int i = 0; i < stepCheckModelList.Count(); i++)
                {
                    if (currentStepId == stepCheckModelList[i].Id)
                    {
                        j = i;
                    }
                }
                //获得下个步骤
                if (j + 1 < stepCheckModelList.Count())
                {
                    //查询第二步是否是自选
                    Flow_FormContentStepCheckModel stepModel = stepCheckModelList[j + 1];
                    if (stepModel.IsCustom)
                    {
                        ViewBag.IsCustomMember = true;
                    }

                }
            }
            Flow_FormContentModel model = formContentBLL.GetById(id);
            model.ExternalURL = formModel.ExternalURL;
            //来自外部，所有情况都需要枚举，每加一个，这里加一句，必须的，与前端相互对应
            if (formModel.ExternalURL.Contains("/External/"))
            {
                model.externalModel = externalBLL.GetById(id);
            }
            //////
            return View(model);
        }
        //批阅
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult EditExternal(string Remark, string TheSeal, string FormId, int Flag, string ContentId, string UserList)
        {
            string stepCheckId = formContentBLL.GetCurrentStepCheckId(FormId, ContentId);
            if (stepCheckId == "")
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
            string currentUser = GetUserId();
            List<Flow_FormContentStepCheckStateModel> stepCheckStateModelList = stepCheckStateBLL.GetListByStepCheckId(ref setNoPagerAscById, stepCheckId);
            Flow_FormContentStepCheckStateModel stepCheckStateModel = stepCheckStateModelList.SingleOrDefault(a => a.UserId == currentUser);
            //决定是否显示审核框
            if (stepCheckStateModel == null)
            {
                return Json(JsonHandler.CreateMessage(0, "越权操作！"));
            }
            stepCheckStateModel.Reamrk = Remark;
            stepCheckStateModel.TheSeal = TheSeal;
            stepCheckStateModel.CheckFlag = Flag;
            if (stepCheckStateBLL.Edit(ref errors, stepCheckStateModel))
            {
                //获取当前步骤
                Flow_FormContentStepCheckModel stepCheckModel = stepCheckBLL.GetById(stepCheckStateModel.StepCheckId);
                //获得当前的步骤模板
                Flow_StepModel currentStepModel = stepBLL.GetById(stepCheckModel.StepId);
                //驳回直接终止审核
                if (Flag == (int)FlowStateEnum.Reject)
                {
                    stepCheckModel.State = Flag;
                    stepCheckModel.StateFlag = false;
                    stepCheckBLL.Edit(ref errors, stepCheckModel);
                    //重置所有步骤的状态
                    stepCheckBLL.ResetCheckStateByFormCententId(stepCheckModel.Id, ContentId, (int)FlowStateEnum.Progress, (int)FlowStateEnum.Progress);
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk, "成功", "修改", "Flow_FormContentStepCheckState");
                    return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
                }
                else if (currentStepModel.IsAllCheck)
                {
                    //启用会签
                    //获得同步骤的同批审核人
                    List<Flow_FormContentStepCheckStateModel> stepCheckStateList = stepCheckStateBLL.GetListByStepCheckId(ref setNoPagerAscById, stepCheckStateModel.StepCheckId);
                    //查看自己是否是最后一个审核人
                    bool complete = stepCheckStateList.Where(a => a.CheckFlag == (int)FlowStateEnum.Progress).Count() == 1;
                    if (complete)
                    {
                        stepCheckModel.State = Flag;
                        stepCheckModel.StateFlag = true;
                        stepCheckBLL.Edit(ref errors, stepCheckModel);
                    }
                    else
                    {
                        //让审核人继续执行这个步骤直到完成
                        LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk, "成功", "修改", "Flow_FormContentStepCheckState");
                        return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
                    }
                }
                else
                {
                    //不是会签，任何一个审批都通过
                    stepCheckModel.State = Flag;
                    stepCheckModel.StateFlag = true;
                    stepCheckBLL.Edit(ref errors, stepCheckModel);
                }
                //查看下一步是否为自创建
                if (!stepCheckModel.IsEnd && !string.IsNullOrEmpty(UserList))
                {
                    List<Flow_FormContentStepCheckModel> stepCheckList = stepCheckBLL.GetListByFormId(FormId, ContentId);
                    int j = 0;
                    for (int i = stepCheckList.Count() - 1; i >= 0; i--)
                    {
                        if (stepCheckId == stepCheckList[i].Id)
                        {
                            j = i;
                        }
                    }
                    //查看是否还有下一步步骤
                    if (j - 1 <= stepCheckList.Count())
                    {
                        //查有第二步骤，查看是否是自选
                        Flow_StepModel stepModel = stepBLL.GetById(stepCheckList[j + 1].StepId);
                        if (stepModel.FlowRule == (int)FlowRuleEnum.Customer)
                        {
                            foreach (string userId in UserList.Split(','))
                            {
                                //批量建立步骤审核人表
                                CreateCheckState(stepCheckList[j + 1].Id, userId);
                            }
                        }
                        else
                        {
                            //批量建立审核人员表
                            foreach (string userId in GetStepCheckMemberList(stepCheckList[j + 1].StepId))
                            {
                                //批量建立步骤审核人表
                                CreateCheckState(stepCheckList[j + 1].Id, userId);
                            }
                        }

                    }

                }


                LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk, "成功", "修改", "Flow_FormContentStepCheckState");
                return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Id" + stepCheckStateModel.Id + ",StepCheckId" + stepCheckStateModel.Reamrk + "," + ErrorCol, "失败", "修改", "Flow_FormContentStepCheckState");
                return Json(JsonHandler.CreateMessage(0, Resource.CheckFail + ErrorCol));
            }

        }



        //创建审核人
        private void CreateCheckState(string stepCheckId, string userId)
        {
            Flow_FormContentStepCheckStateModel stepCheckModelState = new Flow_FormContentStepCheckStateModel();
            stepCheckModelState.Id = ResultHelper.NewId;
            stepCheckModelState.StepCheckId = stepCheckId;
            stepCheckModelState.UserId = userId;
            stepCheckModelState.CheckFlag = 2;
            stepCheckModelState.Reamrk = "";
            stepCheckModelState.TheSeal = "";
            stepCheckModelState.CreateTime = ResultHelper.NowTime;
            stepCheckStateBLL.Create(ref errors, stepCheckModelState);
        }
        //获得审核人列表
        private List<string> GetStepCheckMemberList(string stepId)
        {
            List<string> userModelList = new List<string>();
            Flow_StepModel model = stepBLL.GetById(stepId);
            if (model.FlowRule == (int)FlowRuleEnum.Lead)
            {
                SysUserModel userModel = userBLL.GetById(GetUserId());
                string[] array = userModel.Lead.Split(',');//获得领导，可能有多个领导
                foreach (string str in array)
                {
                    userModelList.Add(str);
                }
            }
            else if (model.FlowRule == (int)FlowRuleEnum.Position)
            {
                string[] array = model.Execution.Split(',');//获得领导，可能有多个领导
                foreach (string str in array)
                {
                    List<SysUserModel> userList = userBLL.GetListByPosId(str);
                    foreach (SysUserModel userModel in userList)
                    {
                        userModelList.Add(userModel.Id);
                    }
                }
            }
            else if (model.FlowRule == (int)FlowRuleEnum.Department)
            {
                GridPager pager = new GridPager()
                {
                    rows = 10000,
                    page = 1,
                    sort = "Id",
                    order = "desc"
                };
                string[] array = model.Execution.Split(',');//获得领导，可能有多个领导
                foreach (string str in array)
                {
                    List<SysUserModel> userList = userBLL.GetUserByDepId(ref pager, str, "");
                    foreach (SysUserModel userModel in userList)
                    {
                        userModelList.Add(userModel.Id);
                    }
                }
            }
            else if (model.FlowRule == (int)FlowRuleEnum.Person)
            {
                string[] array = model.Execution.Split(',');//获得领导，可能有多个领导
                foreach (string str in array)
                {
                    userModelList.Add(str);
                }
            }
          
            return userModelList;
        }

        #region 获得表单
        //根据设定公文，生成表单及控制条件
        private string ExceHtmlJs(string id)
        {
            //定义一个sb为生成HTML表单
            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("<script type='text/javascript'>function CheckForm(){");
            Flow_FormModel model = formBLL.GetById(id);

            #region 判断流程是否有字段,有就生成HTML表单
            sbHtml.Append(JuageExc(model.AttrA, "A", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrB, "B", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrC, "C", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrD, "D", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrE, "E", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrF, "F", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrG, "G", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrH, "H", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrI, "I", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrJ, "J", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrK, "K", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrL, "L", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrM, "M", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrN, "N", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrO, "O", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrP, "P", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrQ, "Q", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrR, "R", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrS, "S", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrT, "T", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrU, "U", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrV, "V", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrW, "W", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrX, "X", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrY, "Y", ref sbJS));
            sbHtml.Append(JuageExc(model.AttrZ, "Z", ref sbJS));
            #endregion
            sbJS.Append("return true}</script>");
            return sbJS.ToString()+sbHtml.ToString();
        }
        //对比
        private bool JudgeVal(string attrId, string rVal, string cVal, string lVal)
        {
            string attrType = formAttrBLL.GetById(attrId).AttrType;
            return new FlowHelper().Judge(attrType, rVal, cVal, lVal);
        }
        private string JuageExc(string attr, string no, ref StringBuilder sbJS)
        {

            if (!string.IsNullOrEmpty(attr))
            {
                return GetHtml(attr, no, ref sbJS);

            }
            return "";
        }
        //获取指定名称的HTML表单
        private string GetHtml(string id, string no, ref StringBuilder sbJS)
        {
            StringBuilder sb = new StringBuilder();
            Flow_FormAttrModel attrModel = formAttrBLL.GetById(id);
            sb.AppendFormat("<tr><td style='width:100px; text-align:right;'>{0} :</td>", attrModel.Title);
            //获取指定类型的HTML表单
            sb.AppendFormat("<td>{0}</td></tr>", new FlowHelper().GetInput(attrModel.AttrType, attrModel.Name, no, attrModel.OptionList,false));
            sbJS.Append(attrModel.CheckJS);
            return sb.ToString();
        }
        #endregion

    }
}
