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
using System;
using Apps.Web.Core;
using Apps.Models.Enum;
using Apps.IBLL.Sys;

namespace Apps.Web.Areas.Flow.Controllers
{
    public class ApplyController : BaseController
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
            
            
            List<Flow_FormContentModel> list = formContentBLL.GeExaminetListByUserId(ref setNoPagerAscById,"",GetUserId());
            foreach (var model in list)
            {
                List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(model.FormId, model.Id);
                model.CurrentState = formContentBLL.GetCurrentFormState(model);
            }
            FlowStateCountModel stateModel = new FlowStateCountModel();
            stateModel.requestCount = list.Count();
            stateModel.passCount = list.Where(a => a.CurrentState == (int)FlowStateEnum.Pass).Count();
            stateModel.rejectCount = list.Where(a => a.CurrentState == (int)FlowStateEnum.Reject).Count();
            stateModel.processCount = list.Where(a => a.CurrentState == (int)FlowStateEnum.Progress).Count();
            stateModel.closedCount = list.Where(a => a.TimeOut < DateTime.Now).Count();
            return View(stateModel);
        }
        [HttpPost]
        public JsonResult GetListByUserId(GridPager pager, string queryStr)
        {
            List<Flow_FormContentModel> list = formContentBLL.GetListByUserId(ref pager, queryStr, GetUserId());
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new Flow_FormContentModel()
                        {

                            Id = r.Id,
                            Title = r.Title,
                            UserId = r.UserId,
                            FormId = r.FormId,
                            FormLevel = r.FormLevel,
                            CreateTime = r.CreateTime,
                            TimeOut = r.TimeOut,
                            CurrentStep = formContentBLL.GetCurrentFormStep(r),
                            CurrentState = formContentBLL.GetCurrentFormState(r),
                            IsExternal = r.IsExternal,
                            ExternalURL=r.ExternalURL

                        }).ToArray()

            };
            return Json(json);
        }
        public string GetCurrentStep(Flow_FormContentModel model)
        {
            string str = "结束";
            List<Flow_FormContentStepCheckModel> stepCheckModelList = stepCheckBLL.GetListByFormId(model.FormId,model.Id);
            for (int i = stepCheckModelList.Count()-1;i>=0;i--)
            {
                if (stepCheckModelList[i].State == 2)
                {
                    str = stepBLL.GetById(stepCheckModelList[i].StepId).Name;
                }
            }
            return str;
        }
       

        #region 详细
        [SupportFilter(ActionName = "Details")]
        public ActionResult Details(string id,string contentId)
        {
            
            Flow_FormModel flowFormModel = formBLL.GetById(id);
            Flow_FormContentModel centent = formContentBLL.GetById(contentId);
            string currentStepName= formContentBLL.GetCurrentFormStep(centent);
  
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
        

        [SupportFilter(ActionName = "Details")]
        public ActionResult Edit(string formId,string id)
        {

            
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
            ViewBag.StepCheckMes = formContentBLL.GetCurrentStepCheckMes(ref setNoPagerAscById, formId, id,GetUserId());
            Flow_FormContentModel model = formContentBLL.GetById(id);

            //Type formType = formModel.GetType();
            //string[] arrStr = { "AttrA", "AttrB", "AttrC", "AttrD", "AttrE", "AttrF", "AttrG", "AttrH", "AttrI", "AttrJ", "AttrK"
            //                      , "AttrL", "AttrM", "AttrN", "AttrO", "AttrP", "AttrQ", "AttrR", "AttrS", "AttrT", "AttrU"
            //                      , "AttrV", "AttrW", "AttrX", "AttrY", "AttrZ"};
            //foreach (string str in arrStr)
            //{
            //    object o = formType.GetProperty(str).GetValue(formModel, null);
            //    if (o != null)
            //    {
            //        Flow_FormAttrModel attrModel = formAttrBLL.GetById(o.ToString());
            //        if (attrModel.AttrType == "人员弹出框")
            //        {

            //        }
            //    }
            //}

            return View(model);
        }



        [SupportFilter(ActionName = "Details")]
        public ActionResult EditExternal(string formId, string id)
        {


            Flow_FormModel formModel = formBLL.GetById(formId);
            //是否已经设置布局获得详细页面
            ViewBag.Html = formModel.ExternalURL.Split(',')[2];

            ViewBag.StepCheckMes = formContentBLL.GetCurrentStepCheckMes(ref setNoPagerAscById, formId, id, GetUserId());
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



        //根据设定公文，生成表单及控制条件
        private string ExceHtmlJs(string id)
        {
            //定义一个sb为生成HTML表单
            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("<script type='text/javascript'>function CheckForm(){");
            Flow_FormModel model = formBLL.GetById(id);
            #region 判断流程是否有字段,有就生成HTML表单
            Type formType = model.GetType();
            //查找名称为"A-Z"的属性
            string[] arrStr = { "AttrA", "AttrB", "AttrC", "AttrD", "AttrE", "AttrF", "AttrG", "AttrH", "AttrI", "AttrJ", "AttrK"
                                  , "AttrL", "AttrM", "AttrN", "AttrO", "AttrP", "AttrQ", "AttrR", "AttrS", "AttrT", "AttrU"
                                  , "AttrV", "AttrW", "AttrX", "AttrY", "AttrZ"};
            foreach (string str in arrStr)
            {
                object o = formType.GetProperty(str).GetValue(model, null);
                if (o != null)
                {
                    //查找model类的Class对象的"str"属性的值
                    sbHtml.Append(GetHtml(o.ToString(), str, ref sbJS));
                }
            }
            
     
            #endregion
            sbJS.Append("return true}</script>");
            ViewBag.HtmlJS = sbJS.ToString();
            return sbHtml.ToString();
        }
     
        //对比
        private bool JudgeVal(string attrId, string rVal, string cVal, string lVal)
        {
            string attrType = formAttrBLL.GetById(attrId).AttrType;
            return new FlowHelper().Judge(attrType, rVal, cVal, lVal);
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

        
    }
}
