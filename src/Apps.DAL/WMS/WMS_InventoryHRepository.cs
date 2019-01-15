using Apps.Models.WMS;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_Inventory_HRepository
    {
        public string CreateInventoryD(string opt, int headId, string invList)
        {
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));

            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            Context.P_WMS_CreateInventoryLine(opt, headId, invList, returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                return null;
            }
            else
                return (string)returnValue.Value;
        }
    }
}
