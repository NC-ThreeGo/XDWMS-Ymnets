using Apps.Models.WMS;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_AIRepository
    {
        public IQueryable<WMS_POForAIModel> GetPOListForAI(string poNo)
        {
            var aiQty = from ai in Context.WMS_AI
                        group ai by ai.POId into g
                        select new { g.Key, sumqty = g.Sum(p => p.ArrivalNum) };
            //ai.ToArray();

            var queryData = from po in Context.WMS_PO
                            where po.Status == "有效" && po.PO == poNo
                            join ai in aiQty on po.Id equals ai.Key into poai
                            from t in poai.DefaultIfEmpty()
                            select new WMS_POForAIModel()
                            {
                                Id = po.Id,
                                PO = po.PO,
                                PODate = po.PODate,
                                SupplierId = po.SupplierId,
                                SupplierShortName = po.WMS_Supplier.SupplierShortName,
                                PartId = po.PartId,
                                PartCode = po.WMS_Part.PartCode,
                                QTY = po.QTY,
                                PlanDate = po.PlanDate,
                                POType = po.POType,
                                SumAIQty = t.sumqty,
                            };

            return queryData;
        }
    }
}
