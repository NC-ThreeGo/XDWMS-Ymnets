
using Apps.Models;
using Apps.Models.Spl;
using Apps.IDAL.Spl;
using System;
using System.Data.SqlClient;

namespace Apps.DAL.Spl
{
    public partial class Spl_WareCheckTotalRepository 
    {
        public void UpdateWareStockPileCheck(string Id)
        {
            SqlParameter[] para = new SqlParameter[]
           {
                new SqlParameter("@Id",Id),
           };
            Context.Database.ExecuteSqlCommand(@"
            declare 
            @WareDetailsId varchar(50), 
            @WarehouseId varchar(50),
            @DiffQuantity int,
            @Price money

            select @WareDetailsId=WareDetailsId,@WarehouseId=WarehouseId,@DiffQuantity=DiffQuantity,@Price=Price from dbo.Spl_WareCheckTotal where  Id=@Id

	        --不存在这条累计库存记录，那么新建
	        if(select COUNT(*) from Spl_WareStockPile where WareDetailsId=@WareDetailsId and WarehouseId=@WarehouseId)=0
	        begin
		        insert into Spl_WareStockPile(id,WareDetailsId,WarehouseId,firstenterdate,lastleavedate,Quantity,price,createtime) 
		        select NEWID(),@WareDetailsId,@WarehouseId,GETDATE(),null,@DiffQuantity,@Price,GETDATE()
	        end
	        else
	        begin
		        update Spl_WareStockPile set Quantity=@DiffQuantity,Price=@Price where WareDetailsId=@WareDetailsId and WarehouseId=@WarehouseId
	        end", para);
        }
    }
}
