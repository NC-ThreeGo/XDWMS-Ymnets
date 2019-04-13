using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_ReturnInspectionRepository
    {
        /// <summary>
        /// 批量保存退货检验单
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="jsonReturnInspection"></param>
        /// <returns></returns>
        string CreateBatchReturnInspection(string opt, string jsonReturnInspection);
    }
}
