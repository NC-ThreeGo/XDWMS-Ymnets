using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_Inventory_HRepository
    {
        /// <summary>
        /// 创建指定库房的盘点表
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="headId"></param>
        /// <param name="invList"></param>
        string CreateInventoryD(string opt, int headId, string invList);
    }
}
