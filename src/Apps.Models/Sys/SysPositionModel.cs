using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Sys
{
    public partial class SysPositionEditModel
    {
        public  string id { get; set; }
        public  string text { get; set; }
        public  string state { get; set; }
    }
    public partial class SysPositionModel
    {
        [MaxWordsExpression(50)]
        [Display(Name = "ID")]
        public override string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "职位名称")]
        public override string Name { get; set; }

        [MaxWordsExpression(500)]
        [Display(Name = "职位说明")]
        public override string Remark { get; set; }

        [Display(Name = "排序")]
        public override int Sort { get; set; }

        [Display(Name = "创建时间")]
        public override DateTime CreateTime { get; set; }

        [Display(Name = "状态")]
        public override bool Enable { get; set; }

        [Display(Name = "职位允许人数")]
        public override int MemberCount { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "DepId")]
        public override string DepId { get; set; }
        public  string DepName { get; set; }

        public  string Flag { get; set; }
    }
}
