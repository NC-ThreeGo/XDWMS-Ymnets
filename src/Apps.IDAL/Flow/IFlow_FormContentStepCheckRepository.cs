using Apps.Models;
using System.Linq;
namespace Apps.IDAL.Flow
{
    public partial interface IFlow_FormContentStepCheckRepository
    {
        IQueryable<Flow_FormContentStepCheck> GetListByFormId(string formId, string contentId);

        void ResetCheckStateByFormCententId(string stepCheckId, string contentId, int checkState, int checkFlag);
    }
}