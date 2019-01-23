using Apps.Models.WMS;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_Sale_OrderRepository
    {
        public string PrintSaleOrder(string opt, string saleBillNum, int id, ref string sellBillNum)
        {
            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            ObjectParameter paramrSellBillNum = new ObjectParameter("SellBillNum", typeof(string));
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_PrintSaleOrder(opt, saleBillNum, id, paramrSellBillNum, returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                sellBillNum = (string)paramrSellBillNum.Value;
                return null;
            }
            else
                return (string)returnValue.Value;
        }

        public string UnPrintSaleOrder(string opt, string sellBillNum, int id)
        {
            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));
            Context.P_WMS_UnPrintSaleOrder(opt, sellBillNum, id, returnValue);

            if (returnValue.Value == DBNull.Value)
            {
                return null;
            }
            else
                return (string)returnValue.Value;
        }

        public void ConfirmSaleOrder(string opt, string sellBillNum)
        {
            ObjectParameter returnValue = new ObjectParameter("ReturnValue", typeof(string));

            //由于EF的默认调用会启用事务，导致和存储过程中的事务冲突，所以设置为不启用事务。
            Context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
            Context.P_WMS_ConfirmSaleOrder(opt, sellBillNum, returnValue);
        }
    }
}
