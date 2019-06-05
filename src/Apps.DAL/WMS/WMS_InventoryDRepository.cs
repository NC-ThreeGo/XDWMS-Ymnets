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
            ExecuteSqlCommand(string.Format("update [dbo].[WMS_Inventory_D] set InventoryQty=0,Attr1='' where HeadId ='{0}'", headId));
            return true;
        }
        public string SpecialInventory(string opt, int headId)
        {
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));

            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            Context.P_WMS_SpecialInventory(opt, headId,returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                return null;
            }
            else
                return (string)returnValue.Value;
        }

    }
}
