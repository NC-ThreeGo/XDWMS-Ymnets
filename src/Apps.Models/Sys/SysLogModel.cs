using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace Apps.Models.Sys
{
    public partial class SysLogModel
    {
     

       [Display(Name = "ID")]
       public override string Id { get; set; }

       [Display(Name = "操作人")]
       public override string Operator { get; set; }

       [Display(Name = "信息")]
       public override string Message { get; set; }

       [Display(Name = "结果")]
       public override string Result { get; set; }

       [Display(Name = "类型")]
       public override string Type { get; set; }

       [Display(Name = "模块")]
       public override string Module { get; set; }

       [Display(Name = "创建时间")]
       public override DateTime? CreateTime { get; set; }
    }
}
