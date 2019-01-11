//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Apps.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WMS_InvInfo
    {
        public WMS_InvInfo()
        {
            this.WMS_AI = new HashSet<WMS_AI>();
            this.WMS_Product_Entry = new HashSet<WMS_Product_Entry>();
            this.WMS_SubInvInfo = new HashSet<WMS_SubInvInfo>();
            this.WMS_Inv = new HashSet<WMS_Inv>();
            this.WMS_Inv_Adjust = new HashSet<WMS_Inv_Adjust>();
            this.WMS_ReturnOrder = new HashSet<WMS_ReturnOrder>();
        }
    
        public int Id { get; set; }
        public string InvCode { get; set; }
        public string InvName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyPerson { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<bool> IsDefault { get; set; }
    
        public virtual ICollection<WMS_AI> WMS_AI { get; set; }
        public virtual ICollection<WMS_Product_Entry> WMS_Product_Entry { get; set; }
        public virtual ICollection<WMS_SubInvInfo> WMS_SubInvInfo { get; set; }
        public virtual ICollection<WMS_Inv> WMS_Inv { get; set; }
        public virtual ICollection<WMS_Inv_Adjust> WMS_Inv_Adjust { get; set; }
        public virtual ICollection<WMS_ReturnOrder> WMS_ReturnOrder { get; set; }
    }
}
