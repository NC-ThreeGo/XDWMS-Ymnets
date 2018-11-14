using System;
using System.Linq;
using Apps.IDAL.Flow;
using Apps.Models;
using System.Data;

namespace Apps.DAL.Flow
{
    public partial class Flow_StepRepository
    {
        public int Delete(string id)
        {

            Flow_Step entity = Context.Flow_Step.SingleOrDefault(a => a.Id == id);
                if (entity != null)
                {
                    IQueryable<Flow_StepRule> collection = from f in Context.Flow_StepRule
                                                           where f.StepId==id
                                                           select f;
                    foreach (var deleteItem in collection)
                    {
                        Context.Flow_StepRule.Remove(deleteItem);
                    }
                    Context.Flow_Step.Remove(entity);
                }
                return this.SaveChanges();
        }

 

    }
}
