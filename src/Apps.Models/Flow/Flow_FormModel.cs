using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
using System.Collections.Generic;
namespace Apps.Models.Flow
{
    public partial class Flow_FormModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "Id")]
        public override string Id { get; set; }

        [MaxWordsExpression(100)]
        [Display(Name = "表单名称")]
        public override string Name { get; set; }

        [MaxWordsExpression(500)]
        [Display(Name = "说明")]
        public override string Remark { get; set; }

        [MaxWordsExpression(2000)]
        [Display(Name = "使用部门")]
        public override string UsingDep { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "分类")]
        public override string TypeId { get; set; }

        public  string TypeName { get; set; }
        [Display(Name = "是否启用")]
        public override bool State { get; set; }

        [Display(Name = "创建时间")]
        public override DateTime? CreateTime { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrA")]
        public override string AttrA { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrB")]
        public override string AttrB { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrC")]
        public override string AttrC { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrD")]
        public override string AttrD { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrE")]
        public override string AttrE { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrF")]
        public override string AttrF { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrG")]
        public override string AttrG { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrH")]
        public override string AttrH { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrI")]
        public override string AttrI { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrJ")]
        public override string AttrJ { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrK")]
        public override string AttrK { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrL")]
        public override string AttrL { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrM")]
        public override string AttrM { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrN")]
        public override string AttrN { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrO")]
        public override string AttrO { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrP")]
        public override string AttrP { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrQ")]
        public override string AttrQ { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrR")]
        public override string AttrR { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrS")]
        public override string AttrS { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrT")]
        public override string AttrT { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrU")]
        public override string AttrU { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrV")]
        public override string AttrV { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrW")]
        public override string AttrW { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrX")]
        public override string AttrX { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrY")]
        public override string AttrY { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrZ")]
        public override string AttrZ { get; set; }

        public override string HtmlForm { get; set; }

        public List<Flow_FormAttrModel> attrList { get; set; }
        public List<Flow_StepModel> stepList { get; set; }

    }
}
