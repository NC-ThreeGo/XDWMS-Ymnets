using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Sys
{
    public partial class SysRightOperateModel
    {
        public override string Id { get; set; }
        public override string RightId { get; set; }
        public override string KeyCode { get; set; }
        public override bool IsValid { get; set; }

    }
}
