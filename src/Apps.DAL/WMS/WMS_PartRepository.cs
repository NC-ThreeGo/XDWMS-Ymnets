using Apps.Common;
using Apps.Models.WMS;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_PartRepository
    {
        public bool UpdateStoreMan(string opt, string o_StoreMan, string n_StoreMan)
        {
            string s = string.Format("update [dbo].[WMS_Part_bk1211] set StoreMan='{1}' where StoreMan ='{0}'", o_StoreMan, n_StoreMan);
            ExecuteSqlCommand(string.Format("update [dbo].[WMS_Part_bk1211] set StoreMan='{1}' where StoreMan ='{0}'", o_StoreMan, n_StoreMan));
            return true;
        }
    }
}