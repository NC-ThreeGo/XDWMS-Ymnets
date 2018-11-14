using Apps.Common;
using Apps.Models;
using Apps.Models.Spl;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Apps.BLL.Spl
{
    public partial class Spl_ProductBLL
    {
        public override List<Spl_ProductModel> GetList(ref GridPager pager, string queryStr)
        {

            IQueryable<Spl_Product> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
								a=>a.Id.Contains(queryStr)
								|| a.Name.Contains(queryStr)
								|| a.Code.Contains(queryStr)
								
								|| a.Color.Contains(queryStr)
								
								|| a.CategoryId.Contains(queryStr)
								
								|| a.CreateBy.Contains(queryStr)
								
								);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
        
            //启用通用列头过滤
            if (!string.IsNullOrWhiteSpace(pager.filterRules))
            {
                List<DataFilterModel> dataFilterList = JsonHandler.Deserialize<List<DataFilterModel>>(pager.filterRules).Where(f => !string.IsNullOrWhiteSpace(f.value)).ToList();
                queryData = LinqHelper.DataFilter<Spl_Product>(queryData, dataFilterList);
            }

            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<Spl_ProductModel> CreateModelList(ref IQueryable<Spl_Product> queryData)
        {

            List<Spl_ProductModel> modelList = (from r in queryData
                                              select new Spl_ProductModel
                                              {
													Id = r.Id,
													Name = r.Name,
													Code = r.Code,
													Price = r.Price,
													Color = r.Color,
													Number = r.Number,
													CategoryId = r.CategoryId,
													CreateTime = r.CreateTime,
													CreateBy = r.CreateBy,
													CostPrice = r.CostPrice,
                                                    ProductCategory = r.Spl_ProductCategory.Name
                                              }).ToList();

            return modelList;
        }


        public List<ProductPillarModel> GetListByPillar(ref GridPager pager, string queryStr)
        {
            using (DBContainer db = new DBContainer())
            {
                DbRawSqlQuery<ProductPillarModel> DbQuery = db.Database.SqlQuery<ProductPillarModel>(@"
SELECT  dbo.Spl_Product.Color, dbo.Spl_ProductCategory.Name, dbo.Spl_Product.Price,  SUM(dbo.Spl_Product.Number) 
                AS Number
FROM      dbo.Spl_Product INNER JOIN
                dbo.Spl_ProductCategory ON dbo.Spl_Product.CategoryId = dbo.Spl_ProductCategory.Id
GROUP BY dbo.Spl_Product.Price, dbo.Spl_Product.Color, dbo.Spl_ProductCategory.Name
");
                //启用通用列头过滤
                pager.totalRows = DbQuery.Count();
                //排序
                IQueryable<ProductPillarModel> queryData = LinqHelper.SortingAndPaging(DbQuery.AsQueryable(), pager.sort, pager.order, pager.page, pager.rows);
                return CreateModelListByPillar(ref queryData);
            }
        }

        public  List<ProductPillarModel> CreateModelListByPillar(ref IQueryable<ProductPillarModel> queryData)
        {

            List<ProductPillarModel> modelList = (from r in queryData
                                                select new ProductPillarModel
                                                {
                                                    Color = r.Color,
                                                    Name = r.Name,
                                                    Price = r.Price,
                                                    Number = r.Number
                                                }).ToList();

            return modelList;
        }


    }

}
