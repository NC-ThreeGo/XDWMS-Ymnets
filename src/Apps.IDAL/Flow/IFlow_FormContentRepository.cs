using Apps.Models;
using System.Linq;
namespace Apps.IDAL.Flow
{
    public partial interface IFlow_FormContentRepository
    {
        IQueryable<Flow_FormContent> GeExamineListByUserId(string userId);
        IQueryable<Flow_FormContent> GeExamineList();
     
    }
}