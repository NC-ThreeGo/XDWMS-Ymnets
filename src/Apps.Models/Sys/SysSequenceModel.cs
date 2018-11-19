using System;
using System.ComponentModel.DataAnnotations;
using Apps.Common;
using Apps.Models;
namespace Apps.Models.Sys
{
    public partial class SysSequenceModel
    {
        [Display(Name = "类型1")]
        public SequenceType FirstTypeEnum { get; set; }
        [Display(Name = "类型1")]
        public String FirstTypeEnumText { get; set; }

        [Display(Name = "类型2")]
        public SequenceType SecondTypeEnum { get; set; }
        [Display(Name = "类型2")]
        public String SecondTypeEnumText { get; set; }
    }
}

