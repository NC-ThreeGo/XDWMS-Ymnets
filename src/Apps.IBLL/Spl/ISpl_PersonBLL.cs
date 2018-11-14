using System.Collections.Generic;
using Apps.Common;
using Apps.Models.Spl;
namespace Apps.IBLL.Spl
{
    public partial interface ISpl_PersonBLL
    {
         bool CheckImportData(string fileName, List<Spl_PersonModel> personList, ref ValidationErrors errors);
        bool CheckImportBatchData(string fileName, List<Spl_PersonModel> personList, ref ValidationErrors errors);
        void SaveImportData(IEnumerable<Spl_PersonModel> personList);
    }
}
