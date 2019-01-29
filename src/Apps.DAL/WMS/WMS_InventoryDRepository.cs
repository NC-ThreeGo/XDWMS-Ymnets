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
    public partial class WMS_Inventory_DRepository
    {
        public bool ClearInventoryQty(string opt, int headId)
        {
            ExecuteSqlCommand(string.Format("update [dbo].[WMS_Inventory_D] set InventoryQty=0 where HeadId ='{0}'", headId));
            return true;
        }
         
    }
}
