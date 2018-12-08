using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.IDAL.WMS
{
    public partial interface IWMS_AIRepository
    {
        IQueryable<WMS_POForAIModel> GetPOListForAI(string poNo);
    }
}
