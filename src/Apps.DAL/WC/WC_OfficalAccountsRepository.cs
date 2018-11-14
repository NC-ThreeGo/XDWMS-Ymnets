using Apps.Common;
using Apps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.DAL.WC
{
    public partial class WC_OfficalAccountsRepository
    {
        public WC_OfficalAccounts GetCurrentAccount()
        {
            return Context.WC_OfficalAccounts.Where(p=>p.IsDefault).FirstOrDefault();
        }
        public bool SetDefault(string id)
        {
            //更新所有为不默认0
            ExecuteSqlCommand(@"update [dbo].[WC_OfficalAccounts] set IsDefault=0");
            //设置当前为默认1
           return ExecuteSqlCommand(@"update [dbo].[WC_OfficalAccounts] set IsDefault=1 where id='"+ ResultHelper.Formatstr(id) +"'")>0;
        }
    }
}
