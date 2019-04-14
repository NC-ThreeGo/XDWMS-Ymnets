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
    
    public partial class WMS_ReturnInspection
    {
        public int Id { get; set; }
        public string ReturnInspectionNum { get; set; }
        public string PartCustomerCode { get; set; }
        public string PartCustomerCodeName { get; set; }
        public int PartID { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<decimal> PCS { get; set; }
        public Nullable<decimal> Volume { get; set; }
        public Nullable<int> InvId { get; set; }
        public Nullable<int> SubInvId { get; set; }
        public string PrintStatus { get; set; }
        public Nullable<System.DateTime> PrintDate { get; set; }
        public string PrintMan { get; set; }
        public string Remark { get; set; }
        public string InspectMan { get; set; }
        public Nullable<System.DateTime> InspectDate { get; set; }
        public string InspectStatus { get; set; }
        public string CheckOutResult { get; set; }
        public Nullable<decimal> QualifyQty { get; set; }
        public Nullable<decimal> NoQualifyQty { get; set; }
        public string Lot { get; set; }
        public string ConfirmStatus { get; set; }
        public string ConfirmMan { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public string ConfirmRemark { get; set; }
        public string Attr1 { get; set; }
        public string Attr2 { get; set; }
        public string Attr3 { get; set; }
        public string Attr4 { get; set; }
        public string Attr5 { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyPerson { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public string InStoreBillNum { get; set; }
    
        public virtual WMS_Customer WMS_Customer { get; set; }
        public virtual WMS_InvInfo WMS_InvInfo { get; set; }
        public virtual WMS_Part WMS_Part { get; set; }
        public virtual WMS_SubInvInfo WMS_SubInvInfo { get; set; }
        public virtual WMS_Supplier WMS_Supplier { get; set; }
    }
}
