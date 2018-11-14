using System.Collections.Generic;
using Apps.Common;
using Apps.Models.Flow;
namespace Apps.IBLL.Flow
{
    public partial interface IFlow_FormContentStepCheckBLL
    {
        List<Flow_FormContentStepCheckModel> GetListByFormId(string formId, string contentId);
        void ResetCheckStateByFormCententId(string stepCheckId, string contentId, int checkState, int checkFlag);
        
    }
}
