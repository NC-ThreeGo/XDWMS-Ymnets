using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Flow
{
    public partial class Flow_FormContentStepCheckStateModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "Id")]
        public override string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "StepCheckId")]
        public override string StepCheckId { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "UserId")]
        public override string UserId { get; set; }

        public string UserName { get; set; }

        [Display(Name = "CheckFlag")]
        public override int CheckFlag { get; set; }

        [MaxWordsExpression(2000)]
        [Display(Name = "Reamrk")]
        public override string Reamrk { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "TheSeal")]
        public override string TheSeal { get; set; }

        [Display(Name = "CreateTime")]
        public override DateTime CreateTime { get; set; }

    }
}
