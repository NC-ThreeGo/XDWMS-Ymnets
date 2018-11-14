using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace Apps.Models.Sys
{
    /// <summary>
    /// 异常处理类
    /// </summary>
    public partial class SysExceptionModel
    {

        [Display(Name = "ID")]
        public override string Id { get; set; }

        [Display(Name = "帮助链接")]
        public override string HelpLink { get; set; }

        [Display(Name = "错误信息")]
        public override string Message { get; set; }

        [Display(Name = "来源")]
        public override string Source { get; set; }

        [Display(Name = "堆栈")]
        public override string StackTrace { get; set; }

        [Display(Name = "目标页")]
        public override string TargetSite { get; set; }

        [Display(Name = "程序集")]
        public override string Data { get; set; }

        [Display(Name = "发生时间")]
        public override DateTime? CreateTime { get; set; }
    }
}
