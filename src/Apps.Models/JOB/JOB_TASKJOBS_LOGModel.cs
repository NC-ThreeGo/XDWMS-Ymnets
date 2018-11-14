using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Apps.Models.JOB
{
    public partial class JOB_TASKJOBS_LOGModel
   {
       [DisplayName("日志序号")]
       [Required(ErrorMessage = "*")]
       public override int itemID { get; set; }
       [DisplayName("任务序号")]
       [Required(ErrorMessage = "*")]
       public override string sno { get; set; }
       [DisplayName("任务名称")]
       [Required(ErrorMessage = "*")]
       public override string taskName { get; set; }
       [DisplayName("任务ID")]
       [Required(ErrorMessage = "*")]
       public override string Id { get; set; }
       [DisplayName("执行时间")]
       public override DateTime? executeDt { get; set; }
       [DisplayName("执行步骤")]
       public override string executeStep { get; set; }
       [DisplayName("结果")]
       public override string result { get; set; }
    }
}
