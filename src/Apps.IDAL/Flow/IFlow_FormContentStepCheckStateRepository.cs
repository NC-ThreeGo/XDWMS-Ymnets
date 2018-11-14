using Apps.Models;
using System.Linq;
namespace Apps.IDAL.Flow
{
    public partial interface IFlow_FormContentStepCheckStateRepository
    {
        Flow_FormContentStepCheckState GetByStepCheckId(string id);
    }
}