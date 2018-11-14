using Apps.Locale;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.WC
{
    public partial class WC_OfficalAccountsModel
    {
        [Display(Name = "TitleId", ResourceType = typeof(Resource))]
        public override string Id { get; set; }
        [NotNullExpression]
        [Display(Name = "WC_OfficalAccounts_OfficalName", ResourceType = typeof(Resource))]
        public override string OfficalName { get; set; }
        [NotNullExpression]
        [Display(Name = "WC_OfficalAccounts_OfficalCode", ResourceType = typeof(Resource))]
        public override string OfficalCode { get; set; }
        [Display(Name = "WC_OfficalAccounts_OfficalPhoto", ResourceType = typeof(Resource))]
        public override string OfficalPhoto { get; set; }
        [Display(Name = "WC_OfficalAccounts_ApiUrl", ResourceType = typeof(Resource))]
        public override string ApiUrl { get; set; }
        [Display(Name = "WC_OfficalAccounts_Token", ResourceType = typeof(Resource))]
        [NotNullExpression]
        public override string Token { get; set; }
        public override string AppId { get; set; }
        public override string AppSecret { get; set; }
        [Display(Name = "WC_OfficalAccounts_AccessToken", ResourceType = typeof(Resource))]
        public override string AccessToken { get; set; }
        [Display(Name = "TitleRemark", ResourceType = typeof(Resource))]
        public override string Remark { get; set; }
        [Display(Name = "TitleEnable", ResourceType = typeof(Resource))]
        public override bool Enable { get; set; }
        [Display(Name = "TitleIsDefault", ResourceType = typeof(Resource))]
        public override bool IsDefault { get; set; }
        [Display(Name = "TitleCategory", ResourceType = typeof(Resource))]
        public override int Category { get; set; }
        [Display(Name = "TitleCreateTime", ResourceType = typeof(Resource))]
        public override System.DateTime CreateTime { get; set; }
        [Display(Name = "TitleCreateBy", ResourceType = typeof(Resource))]
        public override string CreateBy { get; set; }
        [Display(Name = "TitleModifyTime", ResourceType = typeof(Resource))]
        public override System.DateTime ModifyTime { get; set; }
        [Display(Name = "TitleModifyBy", ResourceType = typeof(Resource))]
        public override string ModifyBy { get; set; }
    }
}
