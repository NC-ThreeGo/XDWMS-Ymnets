using System.Collections.Generic;
using Apps.Common;
using Apps.Models.Flow;
namespace Apps.IBLL.Flow
{
    public partial interface IFlow_FormContentBLL
    {
        List<Flow_FormContentModel> GetListByUserId(ref GridPager pager, string queryStr,string userId);
        List<Flow_FormContentModel> GeExaminetListByUserId(ref GridPager pager, string queryStr, string userId);
        List<Flow_FormContentModel> GeExaminetList(ref GridPager pager, string queryStr);

        /// <summary>
        /// 获得申请表单的状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int GetCurrentFormState(Flow_FormContentModel model);
        /// <summary>
        /// 获取当前的步骤名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetCurrentFormStep(Flow_FormContentModel model);
        Flow_StepModel GetCurrentFormStepModel(Flow_FormContentModel model);
        string GetCurrentStepCheckMes(ref GridPager pager, string formId, string contentId, string currentUserId);
        string GetCurrentStepCheckId(string formId, string contentId);
    }
}
