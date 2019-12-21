using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_PartRepository
    {
        /// <summary>
        /// 替换制定指定的保管员
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="o_StoreMan"></param>
        /// <param name="n_StoreMan"></param>
        bool UpdateStoreMan(string opt, string o_StoreMan, string n_StoreMan);
    }
}