using Apps.Models;
using Apps.Models.Spl;
using Apps.IDAL.Spl;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Apps.DAL.Spl
{
	public partial class Spl_WareDetailsRepository
	{
        public int GetWareCountByCategoryId(string depId)
        {
            SqlParameter[] para = new SqlParameter[]
            {
                new SqlParameter("@DepId",depId),
            };
            return Context.Database.ExecuteSqlCommand(@"with CTE_Depart(Id ,Name ,ParentID )as
                                                      (
	                                                      select a.Id ,a.Name ,a.Id  ParentID
	                                                      from	SysStruct  a
	                                                      union	all
	                                                      select a.Id,a.Name ,b.ParentID 
	                                                      from SysStruct a
			                                              join CTE_Depart b on a.ParentID = b.Id 
                                                      )
                                                      select COUNT(*)
                                                      from CTE_Depart a
		                                              left join SysUser b  on a.id = b.DepId 
                                                      where	a.ParentID=@DepId and b.Id is not null");
        }

        public List<Spl_WareDetailsModel> GetListByWareHouse(string wareHouse)
        {
            List<Spl_WareDetailsModel> list = new List<Spl_WareDetailsModel>();
            var querData = from r in Context.Spl_WareDetails
                         join n in Context.Spl_WareStockPile
                         on r.Id equals n.WareDetailsId
                         where n.WarehouseId == wareHouse
                         select new {
                             Id = r.Id,
                             Name = r.Name,
                             Code = r.Code,
                             BarCode = r.BarCode,
                             WareCategoryId = r.WareCategoryId,
                             Unit = r.Unit,
                             Lable = r.Lable,
                             BuyPrice = r.BuyPrice,
                             SalePrice = r.SalePrice,
                             RetailPrice = r.RetailPrice,
                             Remark = r.Remark,
                             Vender = r.Vender,
                             Brand = r.Brand,
                             Color = r.Color,
                             Material = r.Material,
                             Size = r.Size,
                             Weight = r.Weight,
                             ComeFrom = r.ComeFrom,
                             UpperLimit = r.UpperLimit,
                             LowerLimit = r.LowerLimit,
                             PrimeCost = r.PrimeCost,
                             Price1 = r.Price1,
                             Price2 = r.Price2,
                             Price3 = r.Price3,
                             Price4 = r.Price4,
                             Price5 = r.Price5,
                             Photo1 = r.Photo1,
                             Photo2 = r.Photo2,
                             Photo3 = r.Photo3,
                             Photo4 = r.Photo4,
                             Photo5 = r.Photo5,
                             Enable = r.Enable,
                             CreateTime = r.CreateTime,
                             WareCategoryName = r.Spl_WareCategory.Name,
                             WareHouseName = n.Spl_Warehouse.Name,
                             Quantity = n.Quantity
                         };

            foreach (var r in querData) {
                list.Add(new Spl_WareDetailsModel() {
                    Id = r.Id,
                    Name = r.Name,
                    Code = r.Code,
                    BarCode = r.BarCode,
                    WareCategoryId = r.WareCategoryId,
                    Unit = r.Unit,
                    Lable = r.Lable,
                    BuyPrice = r.BuyPrice,
                    SalePrice = r.SalePrice,
                    RetailPrice = r.RetailPrice,
                    Remark = r.Remark,
                    Vender = r.Vender,
                    Brand = r.Brand,
                    Color = r.Color,
                    Material = r.Material,
                    Size = r.Size,
                    Weight = r.Weight,
                    ComeFrom = r.ComeFrom,
                    UpperLimit = r.UpperLimit,
                    LowerLimit = r.LowerLimit,
                    PrimeCost = r.PrimeCost,
                    Price1 = r.Price1,
                    Price2 = r.Price2,
                    Price3 = r.Price3,
                    Price4 = r.Price4,
                    Price5 = r.Price5,
                    Photo1 = r.Photo1,
                    Photo2 = r.Photo2,
                    Photo3 = r.Photo3,
                    Photo4 = r.Photo4,
                    Photo5 = r.Photo5,
                    Enable = r.Enable,
                    CreateTime = r.CreateTime,
                    WareCategoryName = r.WareCategoryName,
                    WareHouseName = r.WareHouseName,
                    Quantity = r.Quantity
                });
            }

            return list;
        }
    }
}
