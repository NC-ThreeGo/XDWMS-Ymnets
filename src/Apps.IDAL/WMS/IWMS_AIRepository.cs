using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_AIRepository
    {
        IQueryable<WMS_POForAIModel> GetPOListForAI(string poNo);

        void CreateInspectBill(string opt, string arrivalBillNum);

        /// <summary>
        /// 输入检验结果，并把合格数量入库，不合格数量暂存到退货单上。
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="jsonInspectBill"></param>
        /// <returns>错误信息，如果为空则表示成功。</returns>
        string ProcessInspectBill(string opt, string jsonInspectBill);
    }
}
