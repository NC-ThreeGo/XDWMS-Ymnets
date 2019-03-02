using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_Inv_History_HRepository
    {
        /// <summary>
        /// 创建历史库存
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        string Create(string oper, string title, string status, string remark);
    }
}
