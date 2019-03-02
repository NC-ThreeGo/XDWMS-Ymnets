using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_Inventory_DRepository
    {
        /// <summary>
        /// 清空指定的盘点数量
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="headId"></param>
        /// <param name="invList"></param>
        bool ClearInventoryQty(string opt, int headId);

        string SpecialInventory(string opt, int headId);

    }
}
