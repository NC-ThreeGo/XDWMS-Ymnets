using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Flow
{
    public partial class Flow_StepRuleModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "Id")]
        public override string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "StepId")]
        public override string StepId { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "AttrId")]
        public override string AttrId { get; set; }
        public  string AttrName { get; set; }
        [MaxWordsExpression(10)]
        [Display(Name = "Operator")]
        public override string Operator { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "Result")]
        public override string Result { get; set; }


         [MaxWordsExpression(50)]
        [Display(Name = "NextStep")]
        public override string NextStep { get; set; }
         public  string NextStepName { get; set; }
         public  string Mes { get; set; }
         public  string Action { get; set; }
    }
}
