using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
using Apps.Locale;
namespace Apps.Models.Sys
{
    public partial class SysSampleModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "SysSample_Id", ResourceType = typeof(Resource))]
        public override string Id { get ; set; }
        [NotNullExpression]
        [MaxWordsExpression(50)]
        [Display(Name = "SysSample_Name", ResourceType = typeof(Resource))]
        public override string Name { get; set; }

        [Display(Name = "Age")]
        public override int? Age { get; set; }

        [Display(Name = "Bir")]
        public override DateTime? Bir { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "Photo")]
        public override string Photo { get; set; }


        [Display(Name = "Note")]
        public override string Note { get; set; }

        [Display(Name = "CreateTime")]
        public override DateTime? CreateTime { get; set; }

    }
}
