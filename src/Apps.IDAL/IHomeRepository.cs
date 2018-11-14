using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;

namespace Apps.IDAL
{
    public interface IHomeRepository
    {
        IQueryable<SysModule> GetMenuByPersonId(string personId, string moduleId);
       
        
    }
}
