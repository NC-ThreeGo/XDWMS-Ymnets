using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Apps.Models.MIS
{
    public partial class MIS_WebIM_MessageModel
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "*")]
        public override string Id { get; set; }
        [DisplayName("内容")]
        public override string Message { get; set; }
        [DisplayName("发送人")]
        public override string Sender { get; set; }
        [DisplayName("发送人")]
        public  string SenderTitle { get; set; }
        [DisplayName("接收人")]
        public override string receiver { get; set; }
        [DisplayName("接收人")]
        public override string receiverTitle { get; set; }
        [DisplayName("作业状态")]
        public override bool State { get; set; }
        [DisplayName("发送时间")]
        public override DateTime? SendDt { get; set; }
    }

    public partial class MIS_WebIM_MessageRecModel
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "*")]
        public  string Id { get; set; }
        [DisplayName("内容")]
        public  string Message { get; set; }
        [DisplayName("发送人")]
        public  string Sender { get; set; }
        [DisplayName("发送人")]
        public  string SenderTitle { get; set; }
        [DisplayName("接收人")]
        public  string receiver { get; set; }
        [DisplayName("接收人")]
        public  string receiverTitle { get; set; }
        [DisplayName("阅读状态")]
        public  bool State { get; set; }
        [DisplayName("发送时间")]
        public  DateTime? SendDt { get; set; }
        [DisplayName("阅读时间")]
        public  DateTime? RecDt { get; set; }
    }

    public partial class MIS_WebIM_SenderModel
    {
        [DisplayName("发送人")]
        public  string Sender { get; set; }
        [DisplayName("发送人")]
        public  string SenderTitle { get; set; }
        [DisplayName("发给我的未读信息记录数")]
        public  int MessageCount { get; set; }

    }
}
