using Apps.Models.Spl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.IBLL.Spl
{
    public partial interface ISpl_WareCategoryBLL
    {
       List<Spl_WareCategoryModel> GetList(string parentId);
    }
}
