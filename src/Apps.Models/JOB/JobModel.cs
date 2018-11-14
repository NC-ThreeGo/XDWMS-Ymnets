using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Apps.Models.JOB
{
    public partial class JobModel
    {
        [DisplayName("任务类型")]
        public int taskType { get; set; } //0=简单任务，1=复杂任务

        [DisplayName("任务名称")]
        public string taskName { get; set; }

        [DisplayName("任务标识")]
        public string id { get; set; }

        [DisplayName("任务标题")]
        public string taskTitle { get; set; }

        [DisplayName("创建人")]
        public string creator { get; set; }

        [DisplayName("任务参数集")]
        public string[,] param { get; set; }

        //用户过程
        [DisplayName("过程SQL")]
        public string procName { get; set; }
        [DisplayName("过程参数")]
        public string procParams { get; set; }

        //复杂任务
        [DisplayName("秒")]
        public string seconds { get; set; }
        [DisplayName("分")]
        public string minutes { get; set; }

        [DisplayName("小时")]
        public string hours { get; set; }

        [DisplayName("天/月")]
        public string dayOfMonth { get; set; }

        [DisplayName("月")]
        public string month { get; set; }

        [DisplayName("天/周")]
        public string dayOfWeek { get; set; }

        [DisplayName("年")]
        public string year { get; set; }

        [DisplayName("任务表达式")]
        public string cronExpression { get; set; }
        //简单任务
        [DisplayName("重复次数")]
        [Range(0, 1000000000,ErrorMessage="不能少于0")]
        [RegularExpression(@"[\d]{1,10}", ErrorMessage = "{0}为数字1－10位。")]
        public int repeatCount { get; set; }

        [DisplayName("无限次")]
        public bool repeatForever { get; set; }

        [DisplayName("间隔")]
        [Range(1,1000000000,ErrorMessage="不能少于1")]
        [RegularExpression(@"[\d]{1,10}", ErrorMessage = "{0}为数字1－10位。")]
        public int intervalCount { get; set; }

        [DisplayName("间隔类型")]
        public string intervalType { get; set; }//M月,d日,h小时,m分,s秒

        [DisplayName("开始日期")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        [DisplayName("结束日期")]
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }


    }
}
