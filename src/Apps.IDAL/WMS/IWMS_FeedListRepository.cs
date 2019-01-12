using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_Feed_ListRepository
    {
        /// <summary>
        /// 打印投料单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="feedBillNum"></param>
        /// <param name="releaseBillNum">投料单号</param>
        /// <returns>错误信息</returns>
        string PrintFeedList(string opt, string feedBillNum, ref string releaseBillNum);

        /// <summary>
        /// 确认投料单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="releaseBillNum"></param>
        void ConfirmFeedList(string opt, string releaseBillNum);
    }
}
