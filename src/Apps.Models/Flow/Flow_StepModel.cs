using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
using System.Collections.Generic;
namespace Apps.Models.Flow
{
    public partial class Flow_StepModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "Id")]
        public override string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "步骤名称")]
        [NotNullExpression]
        public override string Name { get; set; }

        [MaxWordsExpression(500)]
        [Display(Name = "步骤说明")]
        public override string Remark { get; set; }

        [Display(Name = "排序")]
        public override int Sort { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "所属表单")]
        public override string FormId { get; set; }

        [Display(Name = "流转规则")]
        public override int FlowRule { get; set; }

        [Display(Name = "自选审批人")]
        public override bool IsCustom { get; set; }

        [Display(Name = "是否全审核")]
        public override bool IsAllCheck { get; set; }

        [MaxWordsExpression(4000)]
        [Display(Name = "审批人")]
        public override string Execution { get; set; }

        [Display(Name = "强制完成")]
        public override bool CompulsoryOver { get; set; }

        [Display(Name = "编辑附件")]
        public override bool IsEditAttr { get; set; }

        //public Flow_FormModel formModel { get; set; }
        public List<Flow_StepRuleModel> stepRuleList { get; set; }

        public  string Action { get; set; }
        public  string StepNo { get; set; }
    }
}
