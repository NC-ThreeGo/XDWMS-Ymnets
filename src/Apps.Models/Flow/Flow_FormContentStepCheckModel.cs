using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Flow
{
    public partial class Flow_FormContentStepCheckModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "ID")]
        public override string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "所属公文")]
        public override string ContentId { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "StepId")]
        public override string StepId { get; set; }

        [Display(Name = "0不通过1通过2审核中")]
        public override int State { get; set; }

        [Display(Name = "true此步骤审核完成")]
        public override bool StateFlag { get; set; }

        [Display(Name = "创建时间")]
        public override DateTime CreateTime { get; set; }

        [Display(Name = "IsEnd")]
        public override bool IsEnd { get; set; }


        //等于 1 时候为自选
        public override bool IsCustom { get; set; }
    }
}
