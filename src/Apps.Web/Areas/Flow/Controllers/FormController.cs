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
using Apps.Models;
using Apps.Core.OnlineStat;
using Apps.Web.Core;
using System;
using System.Reflection;
using Apps.Locale;
using Apps.BLL.Flow;

namespace Apps.Web.Areas.Flow.Controllers
{
    public class FormController : BaseController
    {
        [Dependency]
        public IFlow_FormBLL m_BLL { get; set; }
        [Dependency]
        public IFlow_FormAttrBLL attrBLL { get; set; }
        [Dependency]
        public IFlow_TypeBLL typeBLL { get; set; }
        [Dependency]
        public IFlow_StepBLL stepBLL { get; set; }
        [Dependency]
        public IFlow_StepRuleBLL stepRuleBLL { get; set; }
        [Dependency]
        public IFlow_FormAttrBLL formAttrBLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        public JsonResult GetFormAttrList(GridPager pager, string queryStr)
        {
            List<Flow_FormAttrModel> list = attrBLL.GetList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new Flow_FormAttrModel()
                        {
                            Id = r.Id,
                            Title = r.Title,
                            Name = r.Name,
                            AttrType = r.AttrType,
                            CheckJS = r.CheckJS,
                            TypeId = r.TypeId,
                            CreateTime = r.CreateTime

                        }).ToArray()

            };

            return Json(json);
        }

        [HttpPost]
        public JsonResult GetList(GridPager pager, string queryStr)
        {

            List<Flow_FormModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<Flow_FormModel> grs = new GridRows<Flow_FormModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            
            List<Flow_TypeModel> list = typeBLL.GetList(ref setNoPagerAscBySort, "");
            ViewBag.FlowType = new SelectList(list, "Id", "Name");
            
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(Flow_FormModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "Flow_Form");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "Flow_Form");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            
            Flow_FormModel model = m_BLL.GetById(id);
            //得到已经选择的字段
            StringBuilder sb = new StringBuilder();
            //获得对象的类型，model
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
                    sb.Append(GetAttr(o.ToString(), str));
                }
            }
            ViewBag.AttrList = sb.ToString();
            List<Flow_TypeModel> list = typeBLL.GetList(ref setNoPagerAscBySort, "");
            ViewBag.FlowType = new SelectList(list, "Id", "Name",model.TypeId);
            ViewBag.FlowTypeName = new SelectList(list, "Id", "Name");
            return View(model);
        }
        //获取已经添加的字段
        private string GetAttr(string id,string str)
        {
           
                Flow_FormAttrModel model = attrBLL.GetById(id);
                return "<tr id='tr" + str + "'><td style='text-align:right;width:150px;border-bottom:1px solid #ccc;padding:3px;'>" + model.Title + "：</td>" +
                "<td style='border-bottom:1px solid #ccc;padding:3px;'>" + getExample(model.AttrType) + "<input id='" + str + "' name='" + str + "' type='hidden' value='" + model.Id + "' /></td>" +
                "<td style='border-bottom:1px solid #ccc;padding:3px;'><a href=\"javascript:deleteCurrentTR('tr" + str + "');\">[删除]</a></td></tr>";
          
        }

       private string getExample(string v)
        {
            switch (v)
            {
                case "文本": return "<input type='text' />"; 
                case "多行文本": return "<textarea></textarea>"; 
                case "数字": return "<input type='text' />"; 
                case "日期": return "<input type='text' />";
                case "附件": return "<input type=\'text\' maxlength=\'255\' class=\'txtInput normal left\'><a onclick=\'$(\'#FileUpload\').trigger(\'click\')\' class=\'files\'>浏览</a><input class=\'displaynone\' type=\'file\' id=\'FileUpload\' name=\'FileUpload\' onchange=\'Upload(\'SingleFile\', \'fujian\', \'FileUpload\');\'><span class=\'uploading\'>上传中</span>";
                case "下拉框": return "<select><option value ='下拉框设置'>下拉框设置</option></select>";
                case "单选按钮": return "<input type='radio' value='' />单选按钮";
                case "复选框": return "<input type='checkbox' value='' />复选框";
                case "人员弹出框": return "<input  readonly='readonly' type='text' style='width: 90px; display: inline; background: #dedede;'><a class='fa fa-plus-square color-gray fa-lg' id='selExc' href='javascript:void(0)'></a>";
                default: return "";
            }
        }
        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Flow_FormModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "Flow_Form");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "Flow_Form");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":"+ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 设计表单布局
        [SupportFilter(ActionName = "Edit")]
        public ActionResult FormLayout(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            Flow_FormModel formModel = m_BLL.GetById(id);
            //是否已经设置布局
            if (!string.IsNullOrEmpty(formModel.HtmlForm))
            {
                ViewBag.Html = formModel.HtmlForm;
            }
            else
            {
                ViewBag.Html = ExceHtmlJs(id);
            }
            ViewBag.FormId = id;
            
            
            
            return View();
        }
        private string ExceHtmlJs(string id)
        {
            //定义一个sb为生成HTML表单
            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("<script type='text/javascript'>function CheckForm(){");
            Flow_FormModel model = m_BLL.GetById(id);
            #region 判断流程是否有字段,有就生成HTML表单
            Type formType = model.GetType();
            //查找名称为"A-Z"的属性
            string[] arrStr = { "AttrA", "AttrB", "AttrC", "AttrD", "AttrE", "AttrF", "AttrG", "AttrH", "AttrI", "AttrJ", "AttrK"
                                  , "AttrL", "AttrM", "AttrN", "AttrO", "AttrP", "AttrQ", "AttrR", "AttrS", "AttrT", "AttrU"
                                  , "AttrV", "AttrW", "AttrX", "AttrY", "AttrZ"};
            sbHtml.AppendFormat("<div class='easyui-draggable' data-option='onDrag:onDrag'><table class='inputtable'><tr><td style='text-align:center'>{0}</td></tr></table></div>", model.Name);
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
            return sbHtml.ToString()+sbJS.ToString();
        }

        private string GetHtml(string id, string no, ref StringBuilder sbJS)
        {
            StringBuilder sb = new StringBuilder();
            Flow_FormAttrModel attrModel = formAttrBLL.GetById(id);
            sb.AppendFormat("<div class='easyui-draggable' data-option='onDrag:onDrag'><table class='inputtable'><tr><td style='vertical-align:middle' class='inputtitle'>{0}</td>", attrModel.Title);
            //获取指定类型的HTML表单
            sb.AppendFormat("<td class='inputcontent'>{0}</td></tr></table></div>", new FlowHelper().GetInput(attrModel.AttrType, attrModel.Name, no, attrModel.OptionList, false));
            sbJS.Append(attrModel.CheckJS);
            return sb.ToString();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        [ValidateInput(false)]
        public JsonResult SaveLayout(string  html,string formId)
        {

            Flow_FormModel model = m_BLL.GetById(formId);
            model.HtmlForm = html;
            if (m_BLL.Edit(ref errors, model))
            {
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.Name, "成功", "修改", "表单布局");
                return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), "Id:" + model.Id + ",Name:" + model.Name + "," + ErrorCol, "失败", "修改", "表单布局");
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ":"+ErrorCol));
            }
            
        }
        #endregion


        #region 设计步骤
        [SupportFilter(ActionName = "Edit")]
        public ActionResult EditStep(string id)
        {
            
            Flow_FormModel flowFormModel = m_BLL.GetById(id);
             List<Flow_StepModel> stepList = stepBLL.GetList(ref setNoPagerAscById, flowFormModel.Id);//获得全部步骤
            foreach (var r in stepList)//获取步骤下面的步骤规则
            {
                r.stepRuleList = GetStepRuleListByStepId(r.Id);
            }
            flowFormModel.stepList = stepList;//获取表单关联的步骤
            ViewBag.Form = flowFormModel;
            Flow_StepModel model = new Flow_StepModel();
            model.FormId = flowFormModel.Id;
            model.IsEditAttr = true;
            return View(model);
        }

     

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult EditStep(Flow_StepModel model)
        {
            model.Id = ResultHelper.NewId;
            if (model != null && ModelState.IsValid)
            {

                if (stepBLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "Flow_Step");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, model.Id));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "Flow_Step");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult DeleteStep(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (stepBLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Flow_Step");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Flow_Step");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }



        #endregion 
        //获取已经添加的字段
        private List<Flow_FormAttrModel> GetAttrList(Flow_FormModel model)
        {
            List<Flow_FormAttrModel> list = new List<Flow_FormAttrModel>();
            Flow_FormAttrModel attrModel = new Flow_FormAttrModel();
            #region 处理字段
            //获得对象的类型，model
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
                    attrModel = attrBLL.GetById(o.ToString()); 
                    list.Add(attrModel);
                }
            }
            #endregion
            return list;
        }

        //获取步骤下面的规则
        private List<Flow_StepRuleModel> GetStepRuleListByStepId(string stepId)
        {
            List<Flow_StepRuleModel> list = stepRuleBLL.GetList(stepId);
            return list;
        }
        #region 详细
       [SupportFilter(ActionName = "Edit")]
        public ActionResult Details(string id)
        {
            
            Flow_FormModel flowFormModel = m_BLL.GetById(id);
            //获取现有的步骤
            GridPager pager = new GridPager()
            {
                rows = 1000,
                page = 1,
                sort = "Id",
                order = "asc"
            };
            flowFormModel.stepList = new List<Flow_StepModel>();
            flowFormModel.stepList = stepBLL.GetList(ref pager, flowFormModel.Id);
            for (int i = 0; i < flowFormModel.stepList.Count; i++)//获取步骤下面的步骤规则
            {
                flowFormModel.stepList[i].stepRuleList = new List<Flow_StepRuleModel>();
                flowFormModel.stepList[i].stepRuleList = GetStepRuleListByStepId(flowFormModel.stepList[i].Id);
            }

            return View(flowFormModel);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Flow_Form");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Flow_Form");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion

        #region 设计分支
        [SupportFilter(ActionName = "Edit")]
        public ActionResult StepList(string id)
        {
            ViewBag.FormId = id;
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult GetStepList(GridPager pager, string id)
        {
            List<Flow_StepModel> stepList = stepBLL.GetList(ref pager, id);
            int i = 1;
            var json = new
            {
                total = pager.totalRows,
                rows = (from r in stepList
                        select new Flow_StepModel()
                        {
                            StepNo = "第 "+(i++)+" 步",
                            Id = r.Id,
                            Name = r.Name,
                            Remark = r.Remark,
                            Sort = r.Sort,
                            FormId = r.FormId,
                            FlowRule = r.FlowRule,
                            Action = "<a href='javascript:SetRule(\"" + r.Id + "\")'>分支(" + GetStepRuleListByStepId(r.Id).Count() + ")</a></a>"
                        }).ToArray()

            };
            return Json(json);
        }
        [SupportFilter(ActionName = "Edit")]
        public ActionResult StepRuleList(string stepId,string formId)
        {
            //获取现有的步骤
            GridPager pager = new GridPager()
            {
                rows = 1000,
                page = 1,
                sort = "Id",
                order = "desc"
            };
            
            Flow_FormModel flowFormModel = m_BLL.GetById(formId);
            List<Flow_FormAttrModel>  attrList = new List<Flow_FormAttrModel>();//获取表单关联的字段
            attrList = GetAttrList(flowFormModel);
            List<Flow_StepModel> stepList = stepBLL.GetList(ref pager, formId);

            ViewBag.StepId = stepId;
            ViewBag.AttrList = attrList;
            ViewBag.StepList = stepList;
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult GetStepRuleList(string stepId)
        {
            List<Flow_StepRuleModel> stepList = stepRuleBLL.GetList(stepId);
            int i =1;
            var json = new
            {
                rows = (from r in stepList
                        select new Flow_StepRuleModel()
                        {
                            Mes="分支"+(i++),
                            Id = r.Id,
                            StepId = r.StepId,
                            AttrId = r.AttrId,
                            AttrName = r.AttrName,
                            Operator = r.Operator,
                            Result = r.Result,
                            NextStep = r.NextStep,
                            NextStepName = r.NextStepName
                        }).ToArray()

            };

            return Json(json);
        }


        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult CreateStepEvent(Flow_StepRuleModel model)
        {
            model.Id = ResultHelper.NewId;
            if (model != null && ModelState.IsValid)
            {

                if (stepRuleBLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",StepId" + model.Id, "成功", "创建", "Flow_StepRule");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed, model.Id));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",StepId" + model.Id + "," + ErrorCol, "失败", "创建", "Flow_StepRule");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }


        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult DeleteStepRule(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (stepRuleBLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Flow_StepRule");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Flow_StepRule");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion 
    }
}
