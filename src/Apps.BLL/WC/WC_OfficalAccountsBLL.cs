using Apps.Models;
using Apps.Models.WC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.BLL.WC
{
    public partial class WC_OfficalAccountsBLL
    {
        public WC_OfficalAccountsModel GetCurrentAccount()
        {
            WC_OfficalAccounts entity =  m_Rep.GetCurrentAccount();
            if (entity == null)
            {
                return new WC_OfficalAccountsModel();
            }
            WC_OfficalAccountsModel model = new WC_OfficalAccountsModel();
            model.Id = entity.Id;
            model.OfficalName = entity.OfficalName;
            model.OfficalCode = entity.OfficalCode;
            model.OfficalPhoto = entity.OfficalPhoto;
            model.ApiUrl = entity.ApiUrl;
            model.Token = entity.Token;
            model.AppId = entity.AppId;
            model.AppSecret = entity.AppSecret;
            model.AccessToken = entity.AccessToken;
            model.Remark = entity.Remark;
            model.Enable = entity.Enable;
            model.IsDefault = entity.IsDefault;
            model.Category = entity.Category;
            model.CreateTime = entity.CreateTime;
            model.CreateBy = entity.CreateBy;
            model.ModifyTime = entity.ModifyTime;
            model.ModifyBy = entity.ModifyBy;
            return model;
        }
        public bool SetDefault(string id)
        {
            return m_Rep.SetDefault(id);
        }
    }
}
