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

        /// <summary>
        /// 输入检验结果，并把合格数量入库，不合格数量暂存到退货单上。
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="jsonReturnInspection"></param>
        /// <returns></returns>
        string ProcessReturnInspectBill(string opt, string jsonReturnInspection);
    }
}
