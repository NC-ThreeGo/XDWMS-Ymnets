using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Apps.Models.MIS
{
    public partial class MIS_WebIM_RecentContactModel
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "*")]
        public override int Id { get; set; }
        [DisplayName("联系人")]
        [Required(ErrorMessage = "*")]
        public override string ContactPersons { get; set; }
        [DisplayName("用户")]
        [Required(ErrorMessage = "*")]
        public override string UserId { get; set; }
        [DisplayName("最后通信时间")]
        [Required(ErrorMessage = "*")]
        public override DateTime InfoTime { get; set; }
        [DisplayName("联系人")]
        public override string ContactPersonsTitle { get; set; }
    }
}
