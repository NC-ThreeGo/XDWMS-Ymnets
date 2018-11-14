using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using Apps.Models;
using Apps.Common;
using System.Transactions;
using Apps.Models.Flow;
using Apps.IBLL.Flow;
using Apps.IDAL.Flow;
using Apps.BLL.Core;
using Apps.Models.Enum;
using Apps.Locale;

namespace Apps.BLL.Flow
{
    public partial class Flow_FormContentStepCheckBLL
    {
     
        public List<Flow_FormContentStepCheckModel> GetListByFormId(string formId, string contentId)
        {
            IQueryable<Flow_FormContentStepCheck> queryData = null;

            queryData = m_Rep.GetListByFormId(formId,contentId);

            return CreateModelList(ref queryData);
        }
       

        public void ResetCheckStateByFormCententId(string stepCheckId, string contentId, int checkState, int checkFlag)
        {
            m_Rep.ResetCheckStateByFormCententId(stepCheckId, contentId, checkState, checkFlag);
        }
    
    }
}
