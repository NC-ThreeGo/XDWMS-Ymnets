USE [XDWMS]
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ConfirmFeedList]    Script Date: 2019/5/11 21:00:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_ConfirmFeedList]
	@UserId varchar(50),
	@ReleaseBillNum	varchar(50),
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @SubAssemblyPartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @Qty decimal(10, 3);
	DECLARE @rowId int;
	DECLARE @countOK int = 0;
	DECLARE @countError int = 0;

	BEGIN TRAN

	--修改库存
	DECLARE cur_FeedList cursor for (select Id, SubAssemblyPartId, InvId, SubInvId, Lot, FeedQty * -1
											from WMS_Feed_List
											where ReleaseBillNum = @ReleaseBillNum
											  and PrintStaus = '已打印'
											  and ConfirmStatus = '未确认');
    --打开游标--
    open cur_FeedList;
    --开始循环游标变量--
    fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			--BEGIN TRAN

			exec P_WMS_UpdateInvQty @UserId, @SubAssemblyPartId, @InvId, null, @Lot, 0, 1, @Qty, @now, '投料', @rowId, @ReleaseBillNum;

			--修改投料单行的确认状态
			update WMS_Feed_List set ConfirmStatus = '已确认', ConfirmMan = @UserId, ConfirmDate = @now,
					ConfirmMessage = '',
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;

			set @countOK = @countOK + 1;
			--COMMIT TRAN;
 		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRAN ;

			--保存错误信息
			BEGIN TRAN SaveError
			set @countError = @countError + 1;
			update WMS_Feed_List set ConfirmMessage = ERROR_MESSAGE(),
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;
			COMMIT TRAN SaveError

			--跳出循环
			BREAK;
		END CATCH

		--转到下一个游标，没有会死循环
        fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_FeedList  --关闭游标
    deallocate cur_FeedList   --释放游标

	IF (@countError = 0)
	BEGIN
		IF @@TRANCOUNT > 0
			COMMIT TRAN ;
		RETURN;
	END
	ELSE
	BEGIN
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;
		set @ReturnValue = '投料单确认存在错误，具体请查看错误信息！';
		RETURN;
	END;

	--IF (@countError > 0)
	--BEGIN
	--	set @ReturnValue = '投料单确认成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
	--	RETURN;
	--END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ConfirmInventory]    Script Date: 2019/5/11 21:00:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[P_WMS_ConfirmInventory] --盘点调整
	@UserId varchar(50),
	@HeadId int,	--盘点头表的ID
	@ReturnValue	varchar(50) OUTPUT

AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @Count int;
	DECLARE @InventoryBillNum varchar(50);

	--查询该盘点表的状态是否为“已生成”
	select @Count = count(*) from WMS_Inventory_H
							where id = @HeadId
							  and InventoryStatus = '已生成';
	IF (@Count <> 1)
	BEGIN
		;
		THROW 51000, '当前盘点表状态不是“已生成”，请确认！', 1;
		RETURN;
	END;


	--查询是否存在实盘数小于0的数据
	select @Count = count(*) from WMS_Inventory_D id
							where id.HeadId = @HeadId
							  and id.InventoryQty < 0;
	IF (@Count > 0)
	BEGIN
		update WMS_Inventory_D set ConfirmMessage = '实盘数小于0，无法进行盘点调整！'
				from WMS_Inventory_D id
				where id.HeadId = @HeadId
					and id.InventoryQty < 0;
		THROW 51000, '存在实盘数小于0，请确认！', 1;
		RETURN;
	END;

	--判断盘点表中物料的批次是否冲突：空批次和非空批次同时存在
	select InvId, SubInvId, PartId
		INTO #id_lot
		from
		(
			select InvId, SubInvId, PartId,
					case when Isnull(Lot, '') = '' then 0
							 else 1
							 end LotType
				--INTO #id_lot
				from WMS_Inventory_D
				where HeadId = @HeadId
				group by InvId, SubInvId, PartId,
						case when Isnull(Lot, '') = '' then 0
							 else 1
							 end
		) lot
		group by lot.InvId, lot.SubInvId, lot.PartId
		having count(*) > 1;
	select count(*) from #id_lot;
	IF (@Count > 0)
	BEGIN
		update WMS_Inventory_D set ConfirmMessage = '盘点物料的批次存在冲突【空批次和非空批次同时存在】，无法进行盘点调整！'
				from WMS_Inventory_D id,
					 #id_lot id_lot
				where id.InvId = id_lot.InvId
					and isnull(id.SubInvId, 0) = isnull(id_lot.SubInvId, 0)
					and id.PartId = id_lot.PartId
					and id.HeadId = @HeadId;
		THROW 51000, '盘点物料的批次存在冲突【空批次和非空批次同时存在】，请确认！', 1;
		RETURN;
	END;

	--查询是否存在实盘数<备料数的数据，如果存在写入错误信息后报错
	select @Count = count(*) from WMS_Inventory_D id,
								  WMS_Inv inv
							where id.InvId = inv.InvId
							  and isnull(id.SubInvId, 0) = isnull(inv.SubInvId, 0)
							  and id.PartId = inv.PartId
							  and isnull(id.Lot, 0) = isnull(inv.Lot, 0)
							  and id.HeadId = @HeadId
							  and id.InventoryQty < inv.StockQty;
	IF (@Count > 0)
	BEGIN
		update WMS_Inventory_D set ConfirmMessage = '实盘数小于备料数，无法进行盘点调整！'
				from WMS_Inventory_D id,
					 WMS_Inv inv
				where id.InvId = inv.InvId
					and isnull(id.SubInvId, 0) = isnull(inv.SubInvId, 0)
					and id.PartId = inv.PartId
					and isnull(id.Lot, 0) = isnull(inv.Lot, 0)
					and id.HeadId = @HeadId
					and id.InventoryQty < inv.StockQty;
		THROW 51000, '存在实盘数小于备料数的数据，请确认！', 1;
		RETURN;
	END;

	BEGIN TRAN

	--获取盘点表头的单据编号
	select @InventoryBillNum = InventoryBillNum
		from WMS_Inventory_H
		 where Id = @HeadId;

	--插入库存记录表
	INSERT INTO WMS_InvRecord (PartId,
								Lot,
								QTY,
								InvId,
								SubInvId,
								BillId,
								SourceBill,
								OperateDate,
								Type,
								OperateMan
								)
				SELECT id.PartId,
						id.Lot,
						id.InventoryQty - id.SnapshootQty,
						id.InvId,
						id.SubInvId,	
						id.Id,
						@InventoryBillNum,
						@now,
						'盘点调整',
						@UserId
						FROM WMS_Inventory_D id
						WHERE id.HeadId = @HeadId
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存库存记录时出错！'
		ROLLBACK TRAN
		RETURN
	END

	--修改现有量
	MERGE INTO WMS_Inv AS inv
		USING (SELECT * FROM WMS_Inventory_D WHERE HeadId = @HeadId)  AS id
		ON (id.InvId = inv.InvId
			and isnull(id.SubInvId, 0) = isnull(inv.SubInvId, 0)
			and id.PartId = inv.PartId
			and isnull(id.Lot, 0) = isnull(inv.Lot, 0)
			)
		WHEN MATCHED
			THEN UPDATE SET inv.Qty = inv.Qty + (id.InventoryQty - id.SnapshootQty)
		WHEN NOT MATCHED
			THEN INSERT (InvId,
						SubInvId,
						PartId,
						Lot,
						Qty,
						StockQty) 
					VALUES(id.InvId,
							id.SubInvId,
							id.PartId,
							id.Lot,
							id.InventoryQty,
							0);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '修改库存现有量时出错！'
		ROLLBACK TRAN
		RETURN
	END

	--修改盘点表状态
	update WMS_Inventory_H set InventoryStatus = '已确认',
								ModifyPerson = @UserId,
								ModifyTime = @now
			where Id = @HeadId;


	COMMIT TRAN
	RETURN

END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ConfirmReturnOrder]    Script Date: 2019/5/11 21:00:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_ConfirmReturnOrder]
	@UserId varchar(50),
	@ReturnOrderNum	varchar(50),
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @PartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @Qty decimal(10, 3);
	DECLARE @rowId int;


	BEGIN TRAN

	--修改库存：只对InvId不为空的记录修改库存（手工创建的库存退货单），根据不合格数量生成的退货单是没有进入库存的。
	DECLARE cur_ReturnOrder cursor for (select rod.Id, ro.PartId, ro.InvId, ro.SubInvId, ro.Lot, rod.ReturnQty * -1
											from WMS_ReturnOrder_D rod,
											     WMS_ReturnOrder ro
											where rod.ReturnOrderDNum = @ReturnOrderNum
											  and rod.PrintStaus = '已退货'
											  and rod.ConfirmStatus = '未确认'
											  and rod.HeadId = ro.Id
											  and ro.InvId is not null);
    --打开游标--
    open cur_ReturnOrder;
    --开始循环游标变量--
    fetch next from cur_ReturnOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin            
		exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, null, @Lot, 0, 0, @Qty, @now, '退货', @rowId, @ReturnOrderNum
		--转到下一个游标，没有会死循环
        fetch next from cur_ReturnOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_ReturnOrder  --关闭游标
    deallocate cur_ReturnOrder   --释放游标


	--修改实际退货单状态
	update WMS_ReturnOrder_D set ConfirmStatus = '已确认', ConfirmMan = @UserId, ConfirmDate = @now,
                ModifyPerson = @UserId, ModifyTime = @now
          where ReturnOrderDNum = @ReturnOrderNum
		    and PrintStaus = '已退货'
			and ConfirmStatus = '未确认';
	IF (@@ERROR <> 0)
	BEGIN
		;
		THROW 51000, '修改实际退货单状态时出错！', 1;
		RETURN
	END

	COMMIT TRAN
	RETURN
END
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ConfirmSaleOrder]    Script Date: 2019/5/11 21:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[P_WMS_ConfirmSaleOrder]
	@UserId varchar(50),
	@SellBillNum	varchar(50),
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @PartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @Qty decimal(10, 3);
	DECLARE @rowId int;
	DECLARE @countOK int = 0;
	DECLARE @countError int = 0;

	BEGIN TRAN

	--修改库存
	DECLARE cur_SaleOrder cursor for (select Id, PartId, InvId, SubInvId, Lot, Qty * -1
											from WMS_Sale_Order
											where SellBillNum = @SellBillNum
											  and PrintStaus = '已打印'
											  and ConfirmStatus = '未确认');
    --打开游标--
    open cur_SaleOrder;
    --开始循环游标变量--
    fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			--BEGIN TRAN

			exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, null, @Lot, 0, 1, @Qty, @now, '销售', @rowId, @SellBillNum;

			--修改投料单行的确认状态
			update WMS_Sale_Order set ConfirmStatus = '已确认', ConfirmMan = @UserId, ConfirmDate = @now,
					ConfirmMessage = '',
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;

			set @countOK = @countOK + 1;
			--COMMIT TRAN;
 		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRAN ;

			--保存错误信息
			BEGIN TRAN SaveError
			set @countError = @countError + 1;
			update WMS_Sale_Order set ConfirmMessage = ERROR_MESSAGE(),
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;
			COMMIT TRAN SaveError

			--跳出循环
			BREAK;
		END CATCH

		--转到下一个游标，没有会死循环
        fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_SaleOrder  --关闭游标
    deallocate cur_SaleOrder   --释放游标

	IF (@countError = 0)
	BEGIN
		IF @@TRANCOUNT > 0
			COMMIT TRAN ;
		RETURN;
	END
	ELSE
	BEGIN
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;
		set @ReturnValue = '销售订单确认存在错误，具体请查看错误信息！';
		RETURN;
	END;

	--IF (@countError > 0)
	--BEGIN
	--	set @ReturnValue = '销售订单确认成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
	--	RETURN;
	--END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateBatchReturnInspection]    Script Date: 2019/5/11 21:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE     PROCEDURE [dbo].[P_WMS_CreateBatchReturnInspection]
	@UserId varchar(50),
	@JsonReturnInspection NVARCHAR(MAX), --所选择的退货入库记录
	@ReturnInspectionNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now date = getdate();
	DECLARE @count int;
	DECLARE @InvId int;
	DECLARE @CheckOutResult varchar(50);

	--保存到临时表
	SELECT *
		INTO #ReturnInspection
		FROM OPENJSON(@JsonReturnInspection)  
			WITH (	PartCustomerCode nvarchar(50),
					PartId int,
					CustomerId int,
					SupplierId int,
					Qty decimal(10, 3),
					PCS nvarchar(50),
					Volume nvarchar(50),
					Remark nvarchar(200)
				) 
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存信息时出错！'
		RETURN
	END

	select @count = count(*) from #ReturnInspection
		where PartId is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在物料Id为空的记录，请确认！', 1;
		RETURN;
	END;

	select @count = count(*) from #ReturnInspection
		where CustomerId is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在客户Id为空的记录，请确认！', 1;
		RETURN;
	END;

	select @count = count(*) from #ReturnInspection ri, WMS_Part p
		where ri.PartId = p.Id
		  and p.PartType = '外购件'
		  and SupplierId is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在供应商Id为空的外购件记录，请确认！', 1;
		RETURN;
	END;

	select @count = count(*) from #ReturnInspection
		where Qty <= 0;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在退货数量小于0的记录，请确认！', 1;
		RETURN;
	END;

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'THJ', 'WMS_ReturnInspection', @now

	BEGIN TRAN

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'THJ', 'WMS_ReturnInspection', @now, @ReturnInspectionNum output

	--获取默认库房
	select top 1 @InvId = Id from WMS_InvInfo
		where Status = '有效'
		  and IsDefault = 1;

	--赋值：默认的检验结果
	set @CheckOutResult = '合格';

	INSERT INTO WMS_ReturnInspection (ReturnInspectionNum,
								PartCustomerCode,
								PartCustomerCodeName,
								PartID,
								CustomerId,
								SupplierId,
								Qty,
								PCS,
								Volume,
								Remark,
								QualifyQty,
								NoQualifyQty,
								CheckOutResult,
								InvId,

								PrintStatus,
								PrintDate,
								PrintMan,
								InspectStatus,
								ConfirmStatus,

								CreatePerson,
								CreateTime
								) 
						SELECT	@ReturnInspectionNum,
								PartCustomerCode,
								'',
								PartId,
								CustomerId,
								SupplierId,
								Qty,
								case when isnull(PCS, '') = '' then null else convert(decimal(19,2), PCS) end,
								case when isnull(Volume, '') = '' then null else convert(decimal(19,2), Volume) end,
								Remark,
								Qty,	--默认合格
								0,
								@CheckOutResult,
								@InvId,

								'已打印',
								@now,
								@UserId,
								'未检验',
								'未确认',

								@UserId,
								@now
							FROM #ReturnInspection;
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存退货入库记录时出错！'
		RETURN
	END


	COMMIT TRAN
	RETURN
END
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateBatchReturnOrder]    Script Date: 2019/5/11 21:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[P_WMS_CreateBatchReturnOrder]
	@UserId varchar(50),
	@JsonReturnOrder NVARCHAR(MAX), --所选择要退货的退货记录
	--@ReturnOrderNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now date = getdate();
	DECLARE @count int;

	--保存到临时表
	SELECT *
		INTO #ReturnOrder
		FROM OPENJSON(@JsonReturnOrder)  
			WITH (	PartId int,
					SupplierId int,
					InvId int,
					Lot varchar(50),
					AdjustQty decimal(10, 3),
					Remark nvarchar(200)
				) 
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存信息时出错！'
		RETURN
	END

	select @count = count(*) from #ReturnOrder
		where PartId is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在物料Id为空的记录，请确认！', 1;
		RETURN;
	END;

	select @count = count(*) from #ReturnOrder
		where SupplierId is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在供应商Id为空的记录，请确认！', 1;
		RETURN;
	END;

	select @count = count(*) from #ReturnOrder
		where InvId is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在库房Id为空的记录，请确认！', 1;
		RETURN;
	END;

	select @count = count(*) from #ReturnOrder
		where Lot is null;
	IF (@count > 0)
	BEGIN
		;
		THROW 51000, '存在批次为空的记录，请确认！', 1;
		RETURN;
	END;

	BEGIN TRAN

	INSERT INTO WMS_ReturnOrder (
								PartID,
								Lot,
								SupplierId,
								InvId,
								SubInvId,
								ReturnQty,
								AdjustQty,
								Status,
								Remark,
								CreatePerson,
								CreateTime
								) 
						SELECT	
								PartId,
								Lot,
								SupplierId,
								InvId,
								null,
								AdjustQty,
								0,
								'有效',
								Remark,
								@UserId,
								@now
							FROM #ReturnOrder;
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存退货记录时出错！'
		RETURN
	END


	COMMIT TRAN
	RETURN
END
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateInspectBill]    Script Date: 2019/5/11 21:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_CreateInspectBill]
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@ArrivalBillNum varchar(50),
	@InspectBillNum varchar(50) output
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	--DECLARE @billNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @defaultInvId int

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'SJ', 'WMS_AI', @now

	BEGIN TRAN

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'SJ', 'WMS_AI', @now, @InspectBillNum output

	SELECT top 1 @defaultInvId = Id from WMS_InvInfo
		WHERE Status = '有效' AND IsDefault = 1

	update WMS_AI set InspectBillNum = @InspectBillNum,
					  InspectMan = @UserId,
					  InspectDate = @now,
					  InspectStatus = '已送检',
					  CheckOutDate = @now,
					  InvId = @defaultInvId,
					  InStoreStatus = '未入库'
			where ArrivalBillNum = @ArrivalBillNum


	COMMIT TRAN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateInventoryLine]    Script Date: 2019/5/11 21:00:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[P_WMS_CreateInventoryLine]
	@UserId varchar(50),
	@HeadId int,
	@JsonInvList NVARCHAR(MAX), --盘点的库房
	@ReturnValue	varchar(500) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @InspectBillNum varchar(50)
	DECLARE @now date = getdate();
	DECLARE @InventoryStatus nvarchar(10);

	select @InventoryStatus = InventoryStatus
		from WMS_Inventory_H
		where id = @HeadId;
	IF (@InventoryStatus <> '未生成')
	BEGIN
		;
		THROW 51000, '盘点表已生成，请确认！', 1;
		RETURN
	END;


	--将盘点的库房保存到临时表
	SELECT *
		INTO #InvList
		FROM OPENJSON(@JsonInvList)  
			WITH (	Id int
				);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存盘点库房时出错！'
		RETURN
	END

	BEGIN TRAN

	--插入盘点行表
	insert into WMS_Inventory_D (HeadId,
								PartId,
								InvId,
								SubInvId,
								Lot,
								SnapshootQty,
								InventoryQty,
								CreatePerson,
								CreateTime
								)
					select		@HeadId,
								PartId,
								InvId,
								SubInvId,
								Lot,
								Qty,
								0,
								@UserId,
								@now
						from WMS_Inv inv
						where qty+stockqty>0 and inv.InvId in (select id from #InvList);

	--修改盘点头表状态
	update WMS_Inventory_H set InventoryStatus = '已生成',
								ModifyPerson = @UserId,
								ModifyTime = @now
			where Id = @HeadId;

	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateInvHistory]    Script Date: 2019/5/11 21:00:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--创建历史库存
CREATE PROCEDURE [dbo].[P_WMS_CreateInvHistory]
	@UserId varchar(50),
	@InvHistoryTitle nvarchar(100),
	@InvHistoryStatus nvarchar(10),
	@Remark nvarchar(200),
	--@ReturnOrderNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @rowId int

	BEGIN TRAN

	--插入退货记录（退货单号为空，打印时再生成退货单号）
	INSERT INTO WMS_Inv_History_H (
								InvHistoryTitle,
								InvHistoryStatus,
								Remark,
								CreatePerson,
								CreateTime
								) 
						VALUES	(
								@InvHistoryTitle,
								@InvHistoryStatus,
								@Remark,
								@UserId,
								@now
								);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存历史库存记录时出错！'
		RETURN
	END
	set @rowId = @@IDENTITY

	--保存历史库存的行表
	insert into WMS_Inv_History_D (HeadId,
								PartId,
								InvId,
								SubInvId,
								SnapshootQty,
								CreatePerson,
								CreateTime
								)
					select		@rowId,
								PartId,
								InvId,
								SubInvId,
								sum(Qty),
								@UserId,
								@now
						from WMS_Inv inv
						group by PartId,
								InvId,
								SubInvId;

	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存历史库存记录时出错！'
		RETURN
	END

	COMMIT TRAN
	RETURN
END
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateReInspect]    Script Date: 2019/5/11 21:00:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_CreateReInspect]
	@UserId varchar(50),
	@AIID int,
	@NCheckOutResult nvarchar(50),
	@NQualifyQty decimal(10, 3),
	@NNoQualifyQty decimal(10, 3),
	@NCheckOutRemark nvarchar(100),
	@NCheckOutDate datetime,
	@Remark nvarchar(100),
	@ReturnValue	varchar(50) OUTPUT

AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate();
	DECLARE @rowId int
	DECLARE @SupplierId int;
	DECLARE @PartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @OQualifyQty decimal(10, 3);
	DECLARE @ONoQualifyQty decimal(10, 3);
	DECLARE @qty decimal(10, 3);
	DECLARE @NoQualifyQty decimal(10, 3);

	BEGIN TRAN

	SELECT @PartId = ai.PartId,
			@SupplierId = po.SupplierId,
			@InvId = ai.InvId,
			@SubInvId = ai.SubInvId,
			@Lot = ai.Lot,
			@OQualifyQty = ai.QualifyQty,
			@ONoQualifyQty = ai.NoQualifyQty
		FROM WMS_AI ai,
			WMS_PO po
		WHERE ai.POId = po.Id
		  and ai.Id = @AIID;

	--插入ReInspect表
	INSERT INTO WMS_ReInspect (AIId,
							OCheckOutResult,
							OQualifyQty,
							ONoQualifyQty,
							OCheckOutRemark,
							OCheckOutDate,
							NCheckOutResult,
							NQualifyQty,
							NNoQualifyQty,
							NCheckOutRemark,
							NCheckOutDate,
							Remark,
							AdjustMan,
							AdjustDate,
							CreatePerson,
							CreateTime
							)
			SELECT @AIID,
					CheckOutResult,
					QualifyQty,
					NoQualifyQty,
					CheckOutRemark,
					CheckOutDate,
					@NCheckOutResult,
					@NQualifyQty,
					@NNoQualifyQty,
					@NCheckOutRemark,
					@NCheckOutDate,
					@Remark,
					@UserId,
					@now,
					@UserId,
					@now
				FROM WMS_AI
				WHERE Id = @AIID;
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存重新送检记录时出错！'
		ROLLBACK TRAN
		RETURN
	END
	set @rowId = @@IDENTITY

	--修改AI表
	UPDATE WMS_AI set CheckOutDate = @NCheckOutDate,
						CheckOutResult = @NCheckOutResult,
						QualifyQty = @NQualifyQty,
						NoQualifyQty = @NNoQualifyQty,
						CheckOutRemark = @NCheckOutRemark,
						ModifyPerson = @UserId,
						ModifyTime = @now
				WHERE Id = @AIID;
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '修改检验记录时出错！'
		ROLLBACK TRAN
		RETURN
	END

	--修改库存：调整数量=新数量-旧数量
	set @qty = @NQualifyQty - @OQualifyQty;
	exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, @SubInvId, @Lot, 0, 0, @qty, @now, '重新送检', @rowId, null

	--插入退货记录
	set @NoQualifyQty = @NNoQualifyQty - @ONoQualifyQty;
	IF (@NoQualifyQty <> 0)
	BEGIN
		INSERT INTO WMS_ReturnOrder (AIID,
									ReInspectID,
									PartID,
									Lot,
									SupplierId,
									ReturnQty,
									AdjustQty,
									Status,
									--PrintStaus,
									CreatePerson,
									CreateTime
									--ConfirmStatus
									)
				VALUES (@AIID,
						@rowId,
						@PartId,
						@Lot,
						@SupplierId,
						@NoQualifyQty,
						0,
						'有效',
						--'未退货',
						@UserId,
						@now
						--'未确认'
						);
	END;

	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateReturnOrder]    Script Date: 2019/5/11 21:00:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--手工创建退货单
CREATE PROCEDURE [dbo].[P_WMS_CreateReturnOrder]
	@UserId varchar(50),
	@PartId int,
	@SupplierId int,
	@InvId int,
	@Lot varchar(50),
	@Qty decimal(10, 3),
	@Remark nvarchar(200),
	--@ReturnOrderNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @rowId int

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	--exec P_WMS_InitNumForDay 'TH', 'WMS_ReturnOrder', @now

	BEGIN TRAN

	--获取当前的单据编号
	--exec P_WMS_GetMaxNum 'TH', 'WMS_ReturnOrder', @now, @ReturnOrderNum output

	--插入退货记录（退货单号为空，打印时再生成退货单号）
	INSERT INTO WMS_ReturnOrder (--ReturnOrderNum,
								PartID,
								Lot,
								SupplierId,
								InvId,
								SubInvId,
								ReturnQty,
								AdjustQty,
								Status,
								Remark,
								--PrintStaus,
								CreatePerson,
								CreateTime
								--ConfirmStatus
								) 
						VALUES	(--@ReturnOrderNum,
								@PartId,
								@Lot,
								@SupplierId,
								@InvId,
								null,
								@Qty,
								0,
								'有效',
								@Remark,
								--'未退货',
								@UserId,
								@now
								--'未确认'
								);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存退货记录时出错！'
		RETURN
	END
	set @rowId = @@IDENTITY

	--修改库存：只对AIID为空的记录修改库存（手工创建的退货单），根据不合格数量生成的退货单是没有进入库存的。
	--在确认退货单时再修改库存
	--exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, null, @Qty, @now, '调账', @rowId, null

	--插入库存记录表
	--INSERT INTO WMS_InvRecord (PartId,
	--							QTY,
	--							InvId,
	--							SubInvId,
	--							BillId,
	--							SourceBill,
	--							OperateDate,
	--							Type,
	--							OperateMan
	--							)
	--			SELECT ro.PartId,
	--					ro.AdjustQty,
	--					ro.InvId,
	--					ro.SubInvId,	
	--					ro.Id,
	--					null,
	--					@now,
	--					'退库',
	--					@UserId
	--					FROM WMS_ReturnOrder ro
	--					WHERE ro.Id = @rowId
	--					 AND  ro.AIID IS NULL
	--IF (@@ERROR <> 0)
	--BEGIN
	--	set @ReturnValue = '保存库存记录时出错！'
	--	ROLLBACK TRAN
	--	RETURN
	--END

	----修改现有量
	--UPDATE WMS_Inv SET Qty = Qty - ro.AdjustQty
	--	FROM WMS_Inv inv,
	--		WMS_ReturnOrder ro
	--	WHERE inv.InvId = ro.InvId
	--		AND Isnull(inv.SubInvId, 0) = Isnull(ro.SubInvId, 0)
	--		AND inv.PartId = ro.PartId
	--		AND ro.Id = @rowId
	--		AND  ro.AIID IS NULL
	--IF (@@ERROR <> 0)
	--BEGIN
	--	set @ReturnValue = '修改库存现有量时出错！'
	--	ROLLBACK TRAN
	--	RETURN
	--END


	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[p_wms_deleteTestData]    Script Date: 2019/5/11 21:00:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[p_wms_deleteTestData]
AS
BEGIN
delete wms_inv
delete WMS_InvRecord
delete WMS_Feed_List;
delete WMS_Sale_Order;
delete WMS_ReturnOrder_d;
delete WMS_ReturnOrder;
delete WMS_Inv_Adjust;
delete WMS_Product_Entry;
delete WMS_ReInspect;
delete WMS_Inventory_D;
delete WMS_Inventory_H;
delete [WMS_AI]
delete wms_po
delete WMS_Inv_History_D;
delete WMS_Inv_History_H;
end

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_GetMaxNum]    Script Date: 2019/5/11 21:00:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[P_WMS_GetMaxNum]
	@Type varchar(50),
	@Tabname varchar(50),
	@Now date,
	@Result nvarchar(50) output 
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int
	declare @maxNum int

	--获取单据编号
	select @count = count(*) from WMS_Num t
		where t.Type = @Type and t.TabName = @Tabname  and t.Day = @Now
	IF (@count = 0)
	BEGIN
		set @maxNum = 0
		--begin tran
		insert into WMS_Num (Num, Day, Type, TabName, MinNum, MaxNum)
			values (@maxNum, @Now, @Type, @Tabname, 0, 9999)
		--commit tran
	END

	--修改当前的单据编号
	update WMS_Num set Num = Num + 1 
		where Type = @type and Day = @now

	select @maxNum = Num from WMS_Num t
		where t.Type = @Type and t.Day = @Now


	set @Result = @Type + CONVERT(varchar(8), @Now, 112) + replace(right(str(@maxNum), 4),' ','0')

END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_InitNumForDay]    Script Date: 2019/5/11 21:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[P_WMS_InitNumForDay]
	@Type varchar(50),
	@Tabname varchar(50),
	@Now date
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int
	declare @maxNum int

	--获取单据编号
	select @count = count(*) from WMS_Num t
		where t.Type = @Type and t.TabName = @Tabname  and t.Day = @Now
	IF (@count = 0)
	BEGIN
		set @maxNum = 0
		insert into WMS_Num (Num, Day, Type, TabName, MinNum, MaxNum)
			values (@maxNum, @Now, @Type, @Tabname, 0, 9999)
	END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_InvAdjust]    Script Date: 2019/5/11 21:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [dbo].[P_WMS_InvAdjust]
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@PartId int,
	@InvId int,
	@Lot nvarchar(50),
	@AdjustQty decimal(10, 3),
	@AdjustType nvarchar(50),
	@Remark nvarchar(200),
	@InvAdjustBillNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(500) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   
	
	DECLARE @now datetime = getdate()
	DECLARE @rowId int
	DECLARE @count int

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'TZ', 'WMS_Inv_Adjust', @now

	BEGIN TRAN 

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'TZ', 'WMS_Inv_Adjust', @now, @InvAdjustBillNum output

	--插入调账记录
	INSERT INTO WMS_Inv_Adjust (InvAdjustBillNum,
								PartID,
								InvId,
								SubInvId,
								AdjustQty,
								AdjustType,
								Remark,
								CreatePerson,
								CreateTime) 
						VALUES	(@InvAdjustBillNum,
								@PartId,
								@InvId,
								null,
								@AdjustQty,
								@AdjustType,
								@Remark,
								@UserId,
								@now
								);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存调账记录时出错！'
		ROLLBACK TRAN
		RETURN
	END
	set @rowId = @@IDENTITY

	--修改库存：
	exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, null, @Lot, 1, 0,
		@AdjustQty, @now, '调账', @rowId, @InvAdjustBillNum

	COMMIT TRAN
	RETURN

END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_InvStock]    Script Date: 2019/5/11 21:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE       PROCEDURE [dbo].[P_WMS_InvStock]	--库存备料
	@UserId varchar(50),
	@PartId int,
	@InvId int,
	@SubInvId int,
	@Lot varchar(50),
	@Qty decimal(10, 3),
	@now datetime,
	@type varchar(50),
	@BillId int,
	@SourceBill varchar(50)
AS
BEGIN
	DECLARE @AllowNegativeInv bit = 0; --是否允许负库存，默认否
	DECLARE @Count int;
	DECLARE @rowId int;
	DECLARE @InvQty decimal(10, 3) = 0;
	DECLARE @StockQty decimal(10, 3) = 0;
	DECLARE @CurrentQty decimal(10, 3) = 0;	--当前扣除数量
	DECLARE @ResidueQty decimal(10, 3) = 0; --剩余数量

	IF (@Qty = 0)
	BEGIN
		;
		THROW 51000, '库存备料数量为0，请确认！', 1;
		RETURN;
	END;
	
	--修改库存备料数
	IF (@Qty > 0)
	BEGIN
		;
		THROW 51000, '入库业务不能进行备料操作，请确认！', 1;
		RETURN;
	END


	--减少库存：当批次为空，则按先进先出的原则进行备料；当批次非空时，只对指定批次进行备料
	IF (@Qty < 0)
	BEGIN
		IF (@Lot IS NOT NULL) --批次不为空
		BEGIN
			SELECT @Count = count(*), @InvQty = SUM(Qty - Isnull(StockQty, 0)) FROM WMS_Inv
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId
					AND Isnull(Lot, 0) = Isnull(@Lot, 0);
			IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
			BEGIN
				;
				THROW 51000, '当前批次的库存现有量不足，请确认！', 1;
				RETURN;
			END
		END

		IF (@Lot IS NULL) --批次为空
		BEGIN
			SELECT @Count = count(*), @InvQty = SUM(Qty - Isnull(StockQty, 0)) FROM WMS_Inv
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId;
			IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
			BEGIN
				;
				THROW 51000, '库存现有量不足，请确认！', 1;
				RETURN;
			END
		END

		--使用游标，按先进先出的原则备料
		DECLARE cur_Inv cursor for select Id, Qty, Isnull(StockQty, 0)
												from WMS_Inv
												where InvId = @InvId
													AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
													AND PartId = @PartId
													AND Isnull(Lot, 0) = Isnull(@Lot, Isnull(Lot, 0))
													AND Qty - Isnull(StockQty, 0) > 0
											Order By Lot;
		set @ResidueQty = ABS(@Qty);
		--打开游标--
		open cur_Inv;
		--开始循环游标变量--
		fetch next from cur_Inv into @rowId, @InvQty, @StockQty;
		while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
		begin         
			IF (@InvQty - @StockQty < @ResidueQty)
			BEGIN
				set @CurrentQty = @InvQty - @StockQty;
			END
			ELSE
			BEGIN
				set @CurrentQty = @ResidueQty;
			END;
			set @ResidueQty = @ResidueQty - @CurrentQty;

			--修改库存备料数
			UPDATE WMS_Inv SET StockQty = Isnull(StockQty, 0) + @CurrentQty
				WHERE Id = @rowId;
			--插入库存记录表
			INSERT INTO WMS_InvRecord (PartId,
										Lot,
										QTY,
										InvId,
										SubInvId,
										BillId,
										SourceBill,
										OperateDate,
										Type,
										OperateMan,
										Stock_InvId,
										StockStatus
										)
								VALUES (@PartId,
										@Lot,
										@CurrentQty,
										@InvId,
										@SubInvId,	
										@BillId,
										@SourceBill,
										@now,
										@type,
										@UserId,
										@rowId,
										2);


			IF (@ResidueQty > 0)
			BEGIN
				--转到下一个游标，没有会死循环
				fetch next from cur_Inv into @rowId, @InvQty, @StockQty; 
			END
			ELSE
			BEGIN
				BREAK;
			END;
		end    
		close cur_Inv  --关闭游标
		deallocate cur_Inv   --释放游标
	END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_InvStock_BatchUpdate]    Script Date: 2019/5/11 21:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[P_WMS_InvStock_BatchUpdate]	--库存备料
	@UserId varchar(50),
	@PartId int,
	@InvId int,
	@SubInvId int,
	@Lot varchar(50),
	@Qty decimal(10, 3)
AS
BEGIN
	DECLARE @AllowNegativeInv bit = 0; --是否允许负库存，默认否
	DECLARE @Count int;
	DECLARE @InvQty decimal(10, 3);

	IF (@Qty = 0)
	BEGIN
		;
		THROW 51000, '库存备料数量为0，请确认！', 1;
		RETURN;
	END;
	
	--修改库存备料数
	IF (@Qty > 0)
	BEGIN
		;
		THROW 51000, '入库业务不能进行备料操作，请确认！', 1;
		RETURN;
	END


	--减少库存：当批次为空，则按先进先出的原则进行备料；当批次非空时，只对指定批次进行备料
	IF (@Qty < 0)
	BEGIN
		IF (@Lot IS NOT NULL) --批次不为空
		BEGIN
			SELECT @Count = count(*), @InvQty = SUM(Qty - Isnull(StockQty, 0)) FROM WMS_Inv
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId
					AND Isnull(Lot, 0) = Isnull(@Lot, 0);
			IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
			BEGIN
				;
				THROW 51000, '当前批次的库存现有量不足，请确认！', 1;
				RETURN;
			END

			--增加备料数
			UPDATE WMS_Inv SET StockQty = Isnull(StockQty, 0) + ABS(@Qty)
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId
					AND Isnull(Lot, 0) = Isnull(@Lot, 0);
		END

		IF (@Lot IS NULL) --批次为空
		BEGIN
			SELECT @Count = count(*), @InvQty = SUM(Qty - Isnull(StockQty, 0)) FROM WMS_Inv
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId;
			IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
			BEGIN
				;
				THROW 51000, '库存现有量不足，请确认！', 1;
				RETURN;
			END

			--扣减库存：先进先出
			UPDATE WMS_Inv SET StockQty = StockQty +
					CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty - Isnull(t.StockQty, 0)), 0) 
								FROM WMS_Inv t
								WHERE t.InvId = @InvId
									AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
									AND t.PartId = @PartId
									AND Isnull(t.Lot, 0) <= Isnull(inv.Lot, 0)
									AND t.Qty > 0) >= 0
						THEN Qty - Isnull(StockQty, 0)
						ELSE CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty - Isnull(t.StockQty, 0)), 0) 
										FROM WMS_Inv t
										WHERE t.InvId = @InvId
											AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
											AND t.PartId = @PartId
											AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
											AND t.Qty > 0) < 0
								THEN 0
								ELSE (SELECT ABS(@Qty) - Isnull(SUM(t.Qty - Isnull(t.StockQty, 0)), 0) 
										FROM WMS_Inv t
										WHERE t.InvId = @InvId
											AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
											AND t.PartId = @PartId
											AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
											AND t.Qty > 0)
								END
						END,
					OutQty = 
					CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty - Isnull(t.StockQty, 0)), 0) 
								FROM WMS_Inv t
								WHERE t.InvId = @InvId
									AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
									AND t.PartId = @PartId
									AND Isnull(t.Lot, 0) <= Isnull(inv.Lot, 0)
									AND t.Qty > 0) >= 0
						THEN Qty - Isnull(StockQty, 0)
						ELSE CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty - Isnull(t.StockQty, 0)), 0) 
										FROM WMS_Inv t
										WHERE t.InvId = @InvId
											AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
											AND t.PartId = @PartId
											AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
											AND t.Qty > 0) < 0
								THEN 0
								ELSE (SELECT ABS(@Qty) - Isnull(SUM(t.Qty - Isnull(t.StockQty, 0)), 0) 
										FROM WMS_Inv t
										WHERE t.InvId = @InvId
											AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
											AND t.PartId = @PartId
											AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
											AND t.Qty > 0)
								END
						END
				FROM WMS_Inv inv
				WHERE inv.InvId = @InvId
					AND Isnull(inv.SubInvId, 0) = Isnull(@SubInvId, 0)
					AND inv.PartId = @PartId;
		END
	END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_PrintFeedList]    Script Date: 2019/5/11 21:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_PrintFeedList]
	@UserId varchar(50),
	@FeedBillNum nvarchar(50),
	@Id int,	--如果@Id = 0，表示是第一次打印整个单据，需要生成单据号；如果<>0，则表示第二次打印备料。
	@ReleaseBillNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now date = getdate()
	DECLARE @SubAssemblyPartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @Qty decimal(10, 3);
	DECLARE @rowId int;
	DECLARE @countOK int = 0;
	DECLARE @countError int = 0;

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	--exec P_WMS_InitNumForDay 'TL', 'WMS_Feed_List', @now

	BEGIN TRAN

	--获取当前的单据编号：如果@Id = 0，表示是第一次打印整个单据，需要生成单据号
	IF (@Id = 0)
	BEGIN
		exec P_WMS_GetMaxNum 'TL', 'WMS_Feed_List', @now, @ReleaseBillNum output
	END
	ELSE
	BEGIN
		select @ReleaseBillNum = ReleaseBillNum
			from WMS_Feed_List
			where Id = @Id;
	END;

	--进行库存备料
	DECLARE cur_FeedList cursor for (select Id, SubAssemblyPartId, InvId, SubInvId, Lot, FeedQty * -1
											from WMS_Feed_List
											where IIF(@Id = 0, FeedBillNum, CONVERT(VARCHAR, Id)) = IIF(@Id = 0, @FeedBillNum, CONVERT(VARCHAR, @Id))
											  and PrintStaus = '未打印');
    --打开游标--
    open cur_FeedList;
    --开始循环游标变量--
    fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			--判断投料数如果大于0，报错
			IF (@Qty >= 0)
			BEGIN
				;
				THROW 51000, '当前投料数为负数或零，请确认！', 1;
			END;

			exec P_WMS_InvStock @UserId, @SubAssemblyPartId, @InvId, null, @Lot, @Qty, @now, '投料', @rowId, @ReleaseBillNum;

			--修改投料单行的打印状态
			update WMS_Feed_List set ReleaseBillNum = @ReleaseBillNum,
					PrintStaus = '已打印', PrintMan = @UserId, PrintDate = @now,
					ConfirmMessage = '',
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;

			set @countOK = @countOK + 1;
 		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRAN;

			--保存错误信息
			BEGIN TRAN SaveError
			set @countError = @countError + 1;
			update WMS_Feed_List set ConfirmMessage = ERROR_MESSAGE(),
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;
			COMMIT TRAN SaveError

			--跳出循环
			BREAK;
		END CATCH

		--转到下一个游标，没有会死循环
        fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_FeedList  --关闭游标
    deallocate cur_FeedList   --释放游标

	IF (@countError = 0)
	BEGIN
		IF @@TRANCOUNT > 0
			COMMIT TRAN;
		RETURN;
	END
	ELSE
	BEGIN
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;
		set @ReturnValue = '投料单备料存在错误，具体请查看错误信息！';
		RETURN;
	END;

	--IF (@countError > 0)
	--BEGIN
	--	set @ReturnValue = '投料单备料成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
	--	RETURN;
	--END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_PrintReturnOrder]    Script Date: 2019/5/11 21:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_PrintReturnOrder]
	@UserId varchar(50),
	@JsonReturnOrder NVARCHAR(MAX), --所选择要打印的退货记录
	@ReturnOrderNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now date = getdate()
	DECLARE @batchId int

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'TH', 'WMS_ReturnOrder', @now

	--将检验结果保存到临时表
	SELECT *
		INTO #ReturnOrder
		FROM OPENJSON(@JsonReturnOrder)  
			WITH (	Id int,
					Qty decimal(10, 3),
					Remark nvarchar(200)
				) 
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存检验信息时出错！'
		RETURN
	END

	BEGIN TRAN

	--修改表的BatchId，以解决并发问题（不用了）
	--SELECT @batchId = NEXT VALUE FOR S_WMS_BatchId;
	--update WMS_ReturnOrder set BatchId = @batchId
	--		FROM WMS_ReturnOrder ro,
	--			 #ReturnOrder t
	--		WHERE ro.Id = t.Id
	--		  AND BatchId is null
	--IF (@@ERROR <> 0)
	--BEGIN
	--	set @ReturnValue = '修改BatchId时出错！'
	--	RETURN
	--END

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'TH', 'WMS_ReturnOrder', @now, @ReturnOrderNum output

	--插入实际退货表：WMS_ReturnOrder_D
	insert into WMS_ReturnOrder_D (
								ReturnOrderDNum,
								HeadId,
								ReturnQty,
								Remark,
								PrintStaus,
								PrintDate,
								PrintMan,
								ConfirmStatus,
								CreatePerson,
								CreateTime
								)
					select @ReturnOrderNum,
							t.Id,
							t.Qty,
							t.Remark,
							'已退货',
							@now,
							@UserId,
							'未确认',
							@UserId,
							@now
						from #ReturnOrder t
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存实际退货记录时出错！'
		RETURN
	END

	--修改待退货表的实际退货数量：WMS_ReturnOrder
	update WMS_ReturnOrder set --ReturnOrderNum = @ReturnOrderNum,
								AdjustQty = ro.AdjustQty + t.Qty,
								--Remark = t.Remark,
								--PrintStaus = '已退货',
								--PrintDate = @now,
								--PrintMan = @UserId,
								ModifyPerson = @UserId,
								ModifyTime = @now
			FROM WMS_ReturnOrder ro,
				 #ReturnOrder t
			WHERE ro.Id = t.Id
				--AND ro.BatchId = @batchId
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存待退货记录时出错！'
		RETURN
	END


	COMMIT TRAN
	RETURN


END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_PrintReturnOrder1]    Script Date: 2019/5/11 21:00:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[P_WMS_PrintReturnOrder1]
	@UserId varchar(50),
	@ReturnOrderNum	varchar(50),
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now date = getdate()

	BEGIN TRAN

	--修改退货单状态
	UPDATE WMS_ReturnOrder set PrintStaus = '已退货',
								PrintDate = @now,
								PrintMan = @UserId,
								ModifyPerson = @UserId,
								ModifyTime = @now
			WHERE ReturnOrderNum = @ReturnOrderNum
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存退货记录时出错！'
		RETURN
	END

	--修改库存：只对AIID为空的记录修改库存（手工创建的退货单），根据不合格数量生成的退货单是没有进入库存的。
	--插入库存记录表
	INSERT INTO WMS_InvRecord (PartId,
								QTY,
								InvId,
								SubInvId,
								BillId,
								SourceBill,
								OperateDate,
								Type,
								OperateMan
								)
				SELECT ro.PartId,
						ro.AdjustQty,
						ro.InvId,
						ro.SubInvId,	
						ro.Id,
						@ReturnOrderNum,
						@now,
						'退库',
						@UserId
						FROM WMS_ReturnOrder ro
						WHERE ro.ReturnOrderNum = @ReturnOrderNum
						 AND  ro.AIID IS NULL
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存库存记录时出错！'
		ROLLBACK TRAN
		RETURN
	END

	--修改现有量
	UPDATE WMS_Inv SET Qty = Qty - ro.AdjustQty
		FROM WMS_Inv inv,
			WMS_ReturnOrder ro
		WHERE inv.InvId = ro.InvId
			AND Isnull(inv.SubInvId, 0) = Isnull(ro.SubInvId, 0)
			AND inv.PartId = ro.PartId
			AND ro.ReturnOrderNum = @ReturnOrderNum
			AND  ro.AIID IS NULL
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '修改库存现有量时出错！'
		ROLLBACK TRAN
		RETURN
	END


	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_PrintSaleOrder]    Script Date: 2019/5/11 21:00:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[P_WMS_PrintSaleOrder]
	@UserId varchar(50),
	@SaleBillNum nvarchar(50),
	@Id int,	--如果@Id = 0，表示是第一次打印整个单据，需要生成单据号；如果 <> 0，则表示第二次打印备料。
	@SellBillNum	varchar(50) OUTPUT,
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now date = getdate()
	DECLARE @PartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @Qty decimal(10, 3);
	DECLARE @rowId int;
	DECLARE @countOK int = 0;
	DECLARE @countError int = 0;

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	--exec P_WMS_InitNumForDay 'TL', 'WMS_Feed_List', @now

	BEGIN TRAN

	--获取当前的单据编号：如果@Id = 0，表示是第一次打印整个单据，需要生成单据号
	IF (@Id = 0)
	BEGIN
		exec P_WMS_GetMaxNum 'XS', 'WMS_Sale_Order', @now, @SellBillNum output;
	END
	ELSE
	BEGIN
		select @SellBillNum = SellBillNum
			from WMS_Sale_Order
			where Id = @Id;
	END;

	--进行库存备料
	DECLARE cur_SaleOrder cursor for (select Id, PartId, InvId, SubInvId, Lot, Qty * -1
											from WMS_Sale_Order
											where IIF(@Id = 0, SaleBillNum, CONVERT(VARCHAR, Id)) = IIF(@Id = 0, @SaleBillNum, CONVERT(VARCHAR, @Id))
											  and PrintStaus = '未打印');
    --打开游标--
    open cur_SaleOrder;
    --开始循环游标变量--
    fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			--判断销售订单数如果大于0，报错
			IF (@Qty >= 0)
			BEGIN
				;
				THROW 51000, '当前销售订单数为负数或零，请确认！', 1;
			END;

			--BEGIN TRAN

			exec P_WMS_InvStock @UserId, @PartId, @InvId, null, @Lot, @Qty, @now, '销售', @rowId, @SellBillNum;

			--修改投料单行的打印状态
			update WMS_Sale_Order set SellBillNum = @SellBillNum,
					PrintStaus = '已打印', PrintMan = @UserId, PrintDate = @now,
					ConfirmMessage = '',
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;

			set @countOK = @countOK + 1;
			--COMMIT TRAN;
 		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRAN ;

			--保存错误信息
			BEGIN TRAN SaveError
			set @countError = @countError + 1;
			update WMS_Sale_Order set ConfirmMessage = ERROR_MESSAGE(),
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;
			COMMIT TRAN SaveError

			--跳出循环
			BREAK;
		END CATCH

		--转到下一个游标，没有会死循环
        fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_SaleOrder  --关闭游标
    deallocate cur_SaleOrder   --释放游标

	IF (@countError = 0)
	BEGIN
		IF @@TRANCOUNT > 0
			COMMIT TRAN ;
		RETURN;
	END
	ELSE
	BEGIN
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;
		set @ReturnValue = '销售订单备料存在错误，具体请查看错误信息！';
		RETURN;
	END;

	--IF (@countError > 0)
	--BEGIN
	--	set @ReturnValue = '销售订单备料成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
	--	RETURN;
	--END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ProcessInspectBill]    Script Date: 2019/5/11 21:00:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_ProcessInspectBill]
	@UserId varchar(50),
	@JsonInspectBill NVARCHAR(MAX), --检验结果
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @InspectBillNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @count int
	DECLARE @InStoreBillNum varchar(50)
	DECLARE @ReturnOrderNum varchar(50)

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'RK', 'WMS_AI', @now

	--将检验结果保存到临时表
	SELECT *
		INTO #InspectBill
		FROM OPENJSON(@JsonInspectBill)  
			WITH (	Id int,
					POId int,
					PartId int,
					Lot nvarchar(50),
					InspectBillNum nvarchar(50),
					CheckOutDate date,
					CheckOutResult nvarchar(50),
					QualifyQty decimal(10, 3),
					InvId int,
					SubInvId int,
					NoQualifyQty decimal(10, 3),
					CheckOutRemark nvarchar(50)
				) 
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存检验信息时出错！'
		RETURN
	END

	--根据检验结果初始化库存现有量，目的是处理并发。如果放在下面的事务中，会导致初始化语句只有在事务完成后才能提交，不利于并发处理。而放在事务之前，是立即提交，这样可以同时对库存现有量进行修改。
	INSERT INTO WMS_Inv (InvId,
						SubInvId,
						PartId,
						Lot,
						Qty,
						StockQty
						)
			SELECT	ib.InvId,	
					ib.SubInvId,	
					ib.PartId,
					ib.Lot,
					0,
					0
					FROM #InspectBill ib
					WHERE ib.QualifyQty <> 0
						AND not exists (
										SELECT 1 FROM WMS_Inv inv
											WHERE inv.InvId = ib.InvId
												AND Isnull(inv.SubInvId, 0) = Isnull(ib.SubInvId, 0)
												AND inv.PartId = ib.PartId
												AND Isnull(inv.Lot, 0) = Isnull(ib.Lot, 0)
										)
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '初始化库存数据时出错！'
		RETURN
	END


	BEGIN TRAN


	SELECT top 1 @InspectBillNum = InspectBillNum FROM #InspectBill

	--获取入库单号
	exec P_WMS_GetMaxNum 'RK', 'WMS_AI', @now, @InStoreBillNum output

	--保存检验结果
	update WMS_AI SET	--WMS_AI.InspectStatus = '已送检',
						WMS_AI.CheckOutDate = t.CheckOutDate,
						WMS_AI.CheckOutResult = t.CheckOutResult,
						WMS_AI.QualifyQty = t.QualifyQty,
						WMS_AI.NoQualifyQty = t.NoQualifyQty,
						WMS_AI.CheckOutRemark = t.CheckOutRemark,
						WMS_AI.InStoreBillNum = @InStoreBillNum,
						WMS_AI.InStoreStatus = '已入库',
						WMS_AI.InStoreMan = @UserId,
						WMS_AI.InvId = t.InvId,
						WMS_AI.SubInvId = t.SubInvId,
						WMS_AI.ModifyPerson = @UserId,
						WMS_AI.ModifyTime = @now
					FROM #InspectBill t,
						WMS_AI
					WHERE WMS_AI.Id = t.Id
							AND WMS_AI.InspectBillNum = @InspectBillNum
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存检验信息时出错！'
		ROLLBACK TRAN
		RETURN
	END


	--对合格数量进行入库处理
	SELECT @count = count(*) FROM #InspectBill
		WHERE QualifyQty <> 0
	IF (@count > 0)
	BEGIN
		--插入库存记录表
		INSERT INTO WMS_InvRecord (PartId,
									Lot,
									QTY,
									InvId,
									SubInvId,
									BillId,
									SourceBill,
									OperateDate,
									Type,
									OperateMan
									)
					SELECT ib.PartId,
							ib.Lot,
							ib.QualifyQty,
							ib.InvId,
							ib.SubInvId,	
							ib.Id,
							@InStoreBillNum,
							@now,
							'入库',
							@UserId
							FROM #InspectBill ib
							WHERE ib.QualifyQty <> 0
		IF (@@ERROR <> 0)
		BEGIN
			set @ReturnValue = '保存库存记录时出错！'
			ROLLBACK TRAN
			RETURN
		END

		--修改现有量
		UPDATE WMS_Inv SET Qty = Qty + ib.QualifyQty
			FROM WMS_Inv inv,
				#InspectBill ib
			WHERE inv.InvId = ib.InvId
				AND Isnull(inv.SubInvId, 0) = Isnull(ib.SubInvId, 0)
				AND inv.PartId = ib.PartId
				AND Isnull(inv.Lot, 0) = Isnull(ib.Lot, 0)
				AND ib.QualifyQty <> 0
		IF (@@ERROR <> 0)
		BEGIN
			set @ReturnValue = '修改库存现有量时出错！'
			ROLLBACK TRAN
			RETURN
		END
	END


	--对不合格数量进行退库处理
	SELECT @count = count(*) FROM #InspectBill
		WHERE NoQualifyQty <> 0
	IF (@count > 0)
	BEGIN
		--插入退货记录
		INSERT INTO WMS_ReturnOrder (AIID,
									PartID,
									Lot,
									SupplierId,
									ReturnQty,
									AdjustQty,
									Status,
									--PrintStaus,
									CreatePerson,
									CreateTime
									--ConfirmStatus
									)
				SELECT ib.Id,
						ib.PartId,
						null,
						po.SupplierId,
						ib.NoQualifyQty,
						0,
						'有效',
						--'未退货',
						@UserId,
						@now
						--'未确认'
					FROM #InspectBill ib,
							WMS_PO po
					WHERE ib.POId = po.Id
						AND ib.NoQualifyQty <> 0

	END


	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ProcessProductEntry]    Script Date: 2019/5/11 21:00:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_ProcessProductEntry]
	@UserId varchar(50),
	@ProductBillNum nvarchar(100), --自制件入库单号（业务）
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @EntryBillNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @count int

	SELECT @count = count(*) FROM WMS_Product_Entry pe
					WHERE pe.ProductQty <> 0
						AND pe.ProductBillNum = @ProductBillNum
						AND pe.EntryBillNum is null
	IF (@count = 0)
	BEGIN
		set @ReturnValue = '没有找到指定的入库单！'
		RETURN
	END

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'RK', 'WMS_Product_Entry', @now

	--根据检验结果初始化库存现有量，目的是处理并发。如果放在下面的事务中，会导致初始化语句只有在事务完成后才能提交，不利于并发处理。而放在事务之前，是立即提交，这样可以同时对库存现有量进行修改。
	INSERT INTO WMS_Inv (InvId,
						SubInvId,
						PartId,
						Lot,
						Qty,
						StockQty
						)
			SELECT	pe.InvId,	
					pe.SubInvId,	
					pe.PartId,
					pe.Lot,
					0,
					0
					FROM WMS_Product_Entry pe
					WHERE pe.ProductQty <> 0
						AND pe.ProductBillNum = @ProductBillNum
						AND pe.EntryBillNum is null
						AND not exists (
										SELECT 1 FROM WMS_Inv inv
											WHERE inv.InvId = pe.InvId
												AND Isnull(inv.SubInvId, 0) = Isnull(pe.SubInvId, 0)
												AND inv.PartId = pe.PartId
												AND Isnull(inv.Lot, 0) = Isnull(pe.Lot, 0)
										);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '初始化库存数据时出错！'
		RETURN
	END

	BEGIN TRAN

	--获取入库单号
	exec P_WMS_GetMaxNum 'Z', 'WMS_Product_Entry', @now, @EntryBillNum output

	--保存检验结果
	update WMS_Product_Entry SET EntryBillNum = @EntryBillNum,
								ModifyPerson = @UserId,
								ModifyTime = @now
					WHERE ProductBillNum = @ProductBillNum
						AND EntryBillNum is null
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存检验信息时出错！'
		ROLLBACK TRAN
		RETURN
	END

	--插入库存记录表
	INSERT INTO WMS_InvRecord (PartId,
								Lot,
								QTY,
								InvId,
								SubInvId,
								BillId,
								SourceBill,
								OperateDate,
								Type,
								OperateMan
								)
				SELECT pe.PartId,
						pe.Lot,
						pe.ProductQty,
						pe.InvId,
						pe.SubInvId,	
						pe.Id,
						@EntryBillNum,
						@now,
						'自制件入库',
						@UserId
						FROM WMS_Product_Entry pe
						WHERE pe.ProductBillNum = @ProductBillNum
							AND pe.EntryBillNum = @EntryBillNum
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存库存记录时出错！'
		ROLLBACK TRAN
		RETURN
	END

	--修改现有量
	UPDATE WMS_Inv SET Qty = Qty + pe.ProductQty
		FROM WMS_Inv inv,
			WMS_Product_Entry pe
		WHERE inv.InvId = pe.InvId
			AND Isnull(inv.SubInvId, 0) = Isnull(pe.SubInvId, 0)
			AND inv.PartId = pe.PartId
			AND Isnull(inv.Lot, 0) = Isnull(pe.Lot, 0)
			AND pe.ProductBillNum = @ProductBillNum
			AND pe.EntryBillNum = @EntryBillNum
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '修改库存现有量时出错！'
		ROLLBACK TRAN
		RETURN
	END

	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ProcessReturnInspectBill]    Script Date: 2019/5/11 21:00:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE     PROCEDURE [dbo].[P_WMS_ProcessReturnInspectBill]
	@UserId varchar(50),
	@JsonReturnInspectBill NVARCHAR(MAX), --检验结果
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @ReturnInspectBillNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @lot varchar(6) = dbo.F_GetLot(@now);
	DECLARE @count int
	DECLARE @InStoreBillNum varchar(50)
	DECLARE @ReturnOrderNum varchar(50)

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'RK', 'WMS_ReturnInspection', @now

	--将检验结果保存到临时表
	SELECT *
		INTO #ReturnInspectBill
		FROM OPENJSON(@JsonReturnInspectBill)  
			WITH (	Id int,
					PartID int,
					ReturnInspectionNum nvarchar(50),
					SupplierId int,
					InspectDate date,
					CheckOutResult nvarchar(50),
					QualifyQty decimal(10, 3),
					InvId int,
					SubInvId int,
					NoQualifyQty decimal(10, 3)
				) 
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存检验信息时出错！'
		RETURN
	END

	--根据检验结果初始化库存现有量，目的是处理并发。如果放在下面的事务中，会导致初始化语句只有在事务完成后才能提交，不利于并发处理。而放在事务之前，是立即提交，这样可以同时对库存现有量进行修改。
	INSERT INTO WMS_Inv (InvId,
						SubInvId,
						PartId,
						Lot,
						Qty,
						StockQty
						)
			SELECT	ib.InvId,	
					ib.SubInvId,	
					ib.PartID,
					@lot,
					0,
					0
					FROM #ReturnInspectBill ib
					WHERE ib.QualifyQty <> 0
						AND not exists (
										SELECT 1 FROM WMS_Inv inv
											WHERE inv.InvId = ib.InvId
												AND Isnull(inv.SubInvId, 0) = Isnull(ib.SubInvId, 0)
												AND inv.PartId = ib.PartID
												AND Isnull(inv.Lot, 0) = @lot
										)
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '初始化库存数据时出错！'
		RETURN
	END


	BEGIN TRAN


	SELECT top 1 @ReturnInspectBillNum = ReturnInspectionNum FROM #ReturnInspectBill

	--获取入库单号
	exec P_WMS_GetMaxNum 'RK', 'WMS_ReturnInspection', @now, @InStoreBillNum output

	--保存检验结果
	update WMS_ReturnInspection SET	WMS_ReturnInspection.InspectStatus = '已检验',
						WMS_ReturnInspection.InspectDate = t.InspectDate,
						WMS_ReturnInspection.CheckOutResult = t.CheckOutResult,
						WMS_ReturnInspection.QualifyQty = t.QualifyQty,
						WMS_ReturnInspection.NoQualifyQty = t.NoQualifyQty,
						WMS_ReturnInspection.InStoreBillNum = @InStoreBillNum,
						WMS_ReturnInspection.InvId = t.InvId,
						WMS_ReturnInspection.SubInvId = t.SubInvId,
						WMS_ReturnInspection.Lot = @lot,
						WMS_ReturnInspection.ModifyPerson = @UserId,
						WMS_ReturnInspection.ModifyTime = @now
					FROM #ReturnInspectBill t,
						WMS_ReturnInspection
					WHERE WMS_ReturnInspection.Id = t.Id
							AND WMS_ReturnInspection.ReturnInspectionNum = @ReturnInspectBillNum
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存检验信息时出错！'
		ROLLBACK TRAN
		RETURN
	END


	--对合格数量进行入库处理
	SELECT @count = count(*) FROM #ReturnInspectBill
		WHERE QualifyQty <> 0
	IF (@count > 0)
	BEGIN
		--插入库存记录表
		INSERT INTO WMS_InvRecord (PartId,
									Lot,
									QTY,
									InvId,
									SubInvId,
									BillId,
									SourceBill,
									OperateDate,
									Type,
									OperateMan
									)
					SELECT ib.PartId,
							@lot,
							ib.QualifyQty,
							ib.InvId,
							ib.SubInvId,	
							ib.Id,
							@InStoreBillNum,
							@now,
							'入库',
							@UserId
							FROM #ReturnInspectBill ib
							WHERE ib.QualifyQty <> 0
		IF (@@ERROR <> 0)
		BEGIN
			set @ReturnValue = '保存库存记录时出错！'
			ROLLBACK TRAN
			RETURN
		END

		--修改现有量
		UPDATE WMS_Inv SET Qty = Qty + ib.QualifyQty
			FROM WMS_Inv inv,
				#ReturnInspectBill ib
			WHERE inv.InvId = ib.InvId
				AND Isnull(inv.SubInvId, 0) = Isnull(ib.SubInvId, 0)
				AND inv.PartId = ib.PartId
				AND Isnull(inv.Lot, 0) = @lot
				AND ib.QualifyQty <> 0
		IF (@@ERROR <> 0)
		BEGIN
			set @ReturnValue = '修改库存现有量时出错！'
			ROLLBACK TRAN
			RETURN
		END
	END


	--对不合格数量进行退库处理（必须是外购件，即有供应商ID的退货记录）
	SELECT @count = count(*) FROM #ReturnInspectBill
		WHERE NoQualifyQty <> 0
		  and SupplierId is not null
	IF (@count > 0)
	BEGIN
		--插入退货记录
		INSERT INTO WMS_ReturnOrder (
									PartID,
									Lot,
									SupplierId,
									ReturnQty,
									AdjustQty,
									Status,
									--PrintStaus,
									CreatePerson,
									CreateTime
									--ConfirmStatus
									)
				SELECT 
						ib.PartId,
						null,
						ib.SupplierId,
						ib.NoQualifyQty,
						0,
						'有效',
						--'未退货',
						@UserId,
						@now
						--'未确认'
					FROM #ReturnInspectBill ib
					WHERE ib.NoQualifyQty <> 0
					  and ib.SupplierId is not null

	END


	COMMIT TRAN
	RETURN
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_SpecialInventory]    Script Date: 2019/5/11 21:00:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[P_WMS_SpecialInventory] --盘点调整
	@UserName varchar(50),
	@HeadId int,	--盘点头表的ID
	@ReturnValue	varchar(50) OUTPUT

AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @now datetime = getdate()
	DECLARE @Count int;
	BEGIN TRAN
	--更改头表状态为已生成
	update WMS_Inventory_H set InventoryStatus='已生成' where Id = @HeadId;
	--抽检物料导入后，如果存在物料批次，那么更改快照数，不存在新增行	
	MERGE INTO WMS_Inventory_D AS inventory
		USING (SELECT * FROM WMS_Inv where PartId in (select PartId from WMS_Inventory_D where HeadId = @HeadId))  AS inv
		ON (inventory.InvId = inv.InvId
			and isnull(inventory.SubInvId, 0) = isnull(inv.SubInvId, 0)
			and inventory.PartId = inv.PartId
			and isnull(inventory.Lot, 0) = isnull(inv.Lot, 0)
			and inventory.HeadId = @HeadId
			)
		WHEN MATCHED
			THEN UPDATE SET inventory.SnapshootQty = inv.Qty
		WHEN NOT MATCHED and inv.qty+inv.stockqty>0
			THEN INSERT (InvId,
						SubInvId,
						PartId,
						Lot,
						HeadId,
						SnapshootQty,
						InventoryQty,
						CreatePerson,
						CreateTime) 
					VALUES(inv.InvId,
							inv.SubInvId,
							inv.PartId,
							inv.Lot,
							@HeadId,
							inv.Qty,
							0,
							@UserName,
							@now);
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '抽检导入时出错！'
		ROLLBACK TRAN
		RETURN
	END
	COMMIT TRAN
	RETURN

END
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_UnInvStock]    Script Date: 2019/5/11 21:00:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE         PROCEDURE [dbo].[P_WMS_UnInvStock]	--取消库存备料
	@UserId varchar(50),
	@now datetime,
	@type varchar(50),
	@BillId int
AS
BEGIN
	DECLARE @Count int;
	DECLARE @rowId int;
	DECLARE @InvQty decimal(10, 3) = 0;
	DECLARE @StockQty decimal(10, 3) = 0;

	--修改库存现有量的备料数
	UPDATE WMS_Inv SET StockQty = inv.StockQty - r.Qty
		FROM WMS_Inv inv,
			WMS_InvRecord r
		WHERE r.Type = @type
			AND r.BillId = @BillId
			AND r.Stock_InvId = inv.Id
			AND r.StockStatus = 2;

	--插入库存记录表：取消备料操作记录
	INSERT INTO WMS_InvRecord (PartId,
								Lot,
								QTY,
								InvId,
								SubInvId,
								BillId,
								SourceBill,
								OperateDate,
								Type,
								OperateMan,
								Stock_InvId,
								StockStatus
								)
						SELECT	r.PartId,
								r.Lot,
								r.Qty * -1,
								r.InvId,
								r.SubInvId,	
								r.BillId,
								r.SourceBill,
								@now,
								r.type,
								@UserId,
								Stock_InvId,
								4
							FROM WMS_InvRecord r
							WHERE r.Type = @type
								AND r.BillId = @BillId
								AND r.Stock_InvId is not null
								AND r.StockStatus = 2;

	--修改库存记录表中原有备料记录的状态为3（3-无效备料（取消备料后将2改成3））
	UPDATE WMS_InvRecord SET StockStatus = 3
		WHERE Type = @type
			AND BillId = @BillId
			AND Stock_InvId is not null
			AND StockStatus = 2;

END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_UnPrintFeedList]    Script Date: 2019/5/11 21:00:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[P_WMS_UnPrintFeedList]
	@UserId varchar(50),
	@ReleaseBillNum nvarchar(50),
	@Id int,	
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	DECLARE @now date = getdate()
	DECLARE @rowId int;
	DECLARE @countOK int = 0;
	DECLARE @countError int = 0;

	SET NOCOUNT ON;
	set xact_abort on   

	--进行库存取消备料
	DECLARE cur_FeedList cursor for (select Id
											from WMS_Feed_List
											where IIF(@Id = 0, ReleaseBillNum, CONVERT(VARCHAR, Id)) = IIF(@Id = 0, @ReleaseBillNum, CONVERT(VARCHAR, @Id))
											  and PrintStaus = '已打印');
    --打开游标--
    open cur_FeedList;
    --开始循环游标变量--
    fetch next from cur_FeedList into @rowId;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			BEGIN TRAN

			--取消备料
			exec P_WMS_UnInvStock @UserId, @now, '投料', @rowId;

			--修改投料单行的打印状态
			update WMS_Feed_List set PrintStaus = '未打印', PrintMan = @UserId, PrintDate = @now,
					ConfirmMessage = '',
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;

			set @countOK = @countOK + 1;
			COMMIT TRAN;
 		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRAN ;

			--报错确认的错误信息
			set @countError = @countError + 1;
			update WMS_Feed_List set ConfirmMessage = ERROR_MESSAGE(),
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;
		END CATCH

		--转到下一个游标，没有会死循环
        fetch next from cur_FeedList into @rowId;  
    end    
    close cur_FeedList  --关闭游标
    deallocate cur_FeedList   --释放游标

	IF @@TRANCOUNT > 0
		COMMIT TRAN ;

	IF (@countError > 0)
	BEGIN
		set @ReturnValue = '投料单备料成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
		RETURN;
	END

END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_UnPrintSaleOrder]    Script Date: 2019/5/11 21:00:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[P_WMS_UnPrintSaleOrder]
	@UserId varchar(50),
	@SellBillNum	varchar(50),
	@Id int,	
	@ReturnValue	varchar(50) OUTPUT
AS
BEGIN
	DECLARE @now date = getdate()
	DECLARE @rowId int;
	DECLARE @countOK int = 0;
	DECLARE @countError int = 0;

	SET NOCOUNT ON;
	set xact_abort on   

		--进行库存备料
	DECLARE cur_SaleOrder cursor for (select Id
											from WMS_Sale_Order
											where IIF(@Id = 0, SellBillNum, CONVERT(VARCHAR, Id)) = IIF(@Id = 0, @SellBillNum, CONVERT(VARCHAR, @Id))
											  and PrintStaus = '已打印');
    --打开游标--
    open cur_SaleOrder;
    --开始循环游标变量--
    fetch next from cur_SaleOrder into @rowId;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			BEGIN TRAN

			--取消备料
			exec P_WMS_UnInvStock @UserId, @now, '销售', @rowId;

			--修改销售订单行的打印状态
			update WMS_Sale_Order set PrintStaus = '未打印', PrintMan = @UserId, PrintDate = @now,
					ConfirmMessage = '',
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;

			set @countOK = @countOK + 1;
			COMMIT TRAN;
 		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRAN ;

			--报错确认的错误信息
			set @countError = @countError + 1;
			update WMS_Sale_Order set ConfirmMessage = ERROR_MESSAGE(),
					ModifyPerson = @UserId, ModifyTime = @now
					where Id = @rowId;
		END CATCH

		--转到下一个游标，没有会死循环
        fetch next from cur_SaleOrder into @rowId;  
    end    
    close cur_SaleOrder  --关闭游标
    deallocate cur_SaleOrder   --释放游标

	IF @@TRANCOUNT > 0
		COMMIT TRAN ;

	IF (@countError > 0)
	BEGIN
		set @ReturnValue = '销售订单备料成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
		RETURN;
	END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_UpdateInvQty]    Script Date: 2019/5/11 21:00:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[P_WMS_UpdateInvQty]
	@UserId varchar(50),
	@PartId int,
	@InvId int,
	@SubInvId int,
	@Lot varchar(50),
	@AllowAddLot bit,	--在增加库存时，是否允许新增批次
	@HasStockQty bit,	--在减少库存时，是否已进行过备料
	@Qty decimal(10, 3),
	@now datetime,
	@type varchar(50),
	@BillId int,
	@SourceBill varchar(50),
	@AllowNegativeInv bit = 0 --是否允许负库存，默认否
AS
BEGIN
	--DECLARE @AllowNegativeInv bit = 0; --是否允许负库存，默认否
	DECLARE @Count int;
	DECLARE @rowId int;
	DECLARE @InvQty decimal(10, 3);
	DECLARE @StockQty decimal(10, 3) = 0;	--备料数
	DECLARE @CurrentQty decimal(10, 3) = 0;	--当前扣除数量
	DECLARE @ResidueQty decimal(10, 3) = 0; --剩余数量

	IF (@Qty = 0)
	BEGIN
		;
		THROW 51000, '库存修改数量为0，请确认！', 1;
		RETURN;
	END;
	
	IF (@Lot = '')
	BEGIN
		set @Lot = null;
	END;

	--修改库存现有量
	--增加库存
	IF (@Qty > 0)
	BEGIN
		--入库时必须要有批次号
		IF (Isnull(@Lot, 0) = 0)
		BEGIN
			;
			THROW 51000, '入库时批次不能为空，请确认！', 1;
			RETURN;
		END

		--查找是否存在同批次的库存
		SELECT @Count = count(*) FROM WMS_Inv
			WHERE InvId = @InvId
				AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
				AND PartId = @PartId
				AND Isnull(Lot, 0) = Isnull(@Lot, 0);
		IF (@Count = 1)	--如果找到，则修改库存现有量
		BEGIN
			UPDATE WMS_Inv SET Qty = Qty + @Qty
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId
					AND Isnull(Lot, 0) = Isnull(@Lot, 0);
		END
		ELSE IF (@AllowAddLot = 1)	--新增批次
		BEGIN
			--如果批次不为空，则判断已有的库存现有量是否存在空批次（系统不允许存在空批次和非空批次同时存在的情况）
			IF (Isnull(@Lot, '') <> '')
			BEGIN
				SELECT @Count = count(*) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Isnull(Lot, '') = '';
				IF (@Count > 0)
				BEGIN
					;
					THROW 51000, '入库批次存在问题：当前批次不为空，但库存存在为空的批次，请确认！', 1;
					RETURN;
				END
			END
			--如果批次为空，则判断已有的库存现有量是否存在不为空批次（系统不允许存在空批次和非空批次同时存在的情况）
			IF (Isnull(@Lot, '') = '')
			BEGIN
				SELECT @Count = count(*) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Isnull(Lot, '') <> '';
				IF (@Count > 0)
				BEGIN
					;
					THROW 51000, '入库批次存在问题：当前批次为空，但库存存在不为空的批次，请确认！', 1;
					RETURN;
				END
			END
		
			--插入库存现有量
			INSERT INTO WMS_Inv (InvId,
								SubInvId,
								PartId,
								Lot,
								Qty,
								StockQty)
						VALUES (@InvId,
								@SubInvId,
								@PartId,
								@Lot,
								@Qty,
								0
								);

		END
		ELSE  --增加库存时发生无效批次
		BEGIN
			;
			THROW 51000, '入库批次存在问题：当前批次库存不存在且该操作不允许新增批次，请确认！', 1;
			RETURN;
		END

		--插入库存记录表
		INSERT INTO WMS_InvRecord (PartId,
									Lot,
									QTY,
									InvId,
									SubInvId,
									BillId,
									SourceBill,
									OperateDate,
									Type,
									OperateMan,
									Stock_InvId,
									StockStatus
									)
							VALUES (@PartId,
									@Lot,
									@Qty,
									@InvId,
									@SubInvId,	
									@BillId,
									@SourceBill,
									@now,
									@type,
									@UserId,
									null,
									1);
	END


	--减少库存：当批次为空，则按先进先出的原则扣减库存；当批次非空时，只扣减指定批次的库存
	IF (@Qty < 0)
	BEGIN
		IF (@HasStockQty = 1)	--已经备料过，直接扣减库存，不用判断库存现有量
		BEGIN
			--修改库存现有量
			UPDATE WMS_Inv SET Qty = inv.Qty - r.QTY, 
								StockQty = inv.StockQty - r.Qty
				FROM WMS_Inv inv,
					 WMS_InvRecord r
				WHERE r.Type = @type
				  AND r.BillId = @BillId
				  AND r.Stock_InvId = inv.Id
				  AND r.StockStatus = 2;

			--插入库存记录表
			INSERT INTO WMS_InvRecord (PartId,
										Lot,
										QTY,
										InvId,
										SubInvId,
										BillId,
										SourceBill,
										OperateDate,
										Type,
										OperateMan,
										Stock_InvId,
										StockStatus
										)
								SELECT	r.PartId,
										r.Lot,
										r.Qty,
										r.InvId,
										r.SubInvId,	
										r.BillId,
										r.SourceBill,
										@now,
										r.type,
										@UserId,
										null,
										1
									FROM WMS_InvRecord r
									WHERE r.Type = @type
										AND r.BillId = @BillId
										AND r.Stock_InvId is not null;
		END
		ELSE
		BEGIN
			IF (Isnull(@Lot, '') <> '') --批次不为空
			BEGIN
				SELECT @Count = count(*), @InvQty = SUM(Qty - Isnull(StockQty, 0)) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Isnull(Lot, 0) = Isnull(@Lot, 0);
				IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
				BEGIN
					;
					THROW 51000, '当前批次的库存现有量不足，请确认！', 1;
					RETURN;
				END
			END

			IF (Isnull(@Lot, '') = '') --批次为空
			BEGIN
				SELECT @Count = count(*), @InvQty = SUM(Qty - Isnull(StockQty, 0)) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId;
				IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
				BEGIN
					;
					THROW 51000, '库存现有量不足，请确认！', 1;
					RETURN;
				END
			END

			--使用游标，按先进先出的原则出库
			DECLARE cur_Inv cursor for select Id, Qty, Isnull(StockQty, 0)
											from WMS_Inv
											where InvId = @InvId
												AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
												AND PartId = @PartId
												AND Isnull(Lot, 0) = Isnull(@Lot, Isnull(Lot, 0))
												AND Qty - Isnull(StockQty, 0) > 0
											Order By Lot;
			set @ResidueQty = ABS(@Qty);
			--打开游标--
			open cur_Inv;
			--开始循环游标变量--
			fetch next from cur_Inv into @rowId, @InvQty, @StockQty;
			while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
			begin         
				IF (@InvQty - @StockQty < @ResidueQty)
				BEGIN
					set @CurrentQty = @InvQty - @StockQty;
				END
				ELSE
				BEGIN
					set @CurrentQty = @ResidueQty;
				END;
				set @ResidueQty = @ResidueQty - @CurrentQty;

				--修改库存现有量
				UPDATE WMS_Inv SET Qty = Qty - @CurrentQty
					WHERE Id = @rowId;
				--插入库存记录表
				INSERT INTO WMS_InvRecord (PartId,
											Lot,
											QTY,
											InvId,
											SubInvId,
											BillId,
											SourceBill,
											OperateDate,
											Type,
											OperateMan,
											Stock_InvId,
											StockStatus
											)
									VALUES (@PartId,
											@Lot,
											@CurrentQty,
											@InvId,
											@SubInvId,	
											@BillId,
											@SourceBill,
											@now,
											@type,
											@UserId,
											null,
											1);

				IF (@ResidueQty > 0)
				BEGIN
					--转到下一个游标，没有会死循环
					fetch next from cur_Inv into @rowId, @InvQty, @StockQty; 
				END
				ELSE
				BEGIN
					BREAK;
				END;
			end    
			close cur_Inv  --关闭游标
			deallocate cur_Inv   --释放游标

		END
	END
END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_UpdateInvQty_BatchUpdate]    Script Date: 2019/5/11 21:00:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[P_WMS_UpdateInvQty_BatchUpdate]
	@UserId varchar(50),
	@PartId int,
	@InvId int,
	@SubInvId int,
	@Lot varchar(50),
	@AllowAddLot bit,	--在增加库存时，是否允许新增批次
	@HasStockQty bit,	--在减少库存时，是否已进行过备料
	@Qty decimal(10, 3),
	@now datetime,
	@type varchar(50),
	@BillId int,
	@SourceBill varchar(50)
AS
BEGIN
	DECLARE @AllowNegativeInv bit = 0; --是否允许负库存，默认否
	DECLARE @Count int;
	DECLARE @InvQty decimal(10, 3);

	IF (@Qty = 0)
	BEGIN
		;
		THROW 51000, '库存修改数量为0，请确认！', 1;
		RETURN;
	END;
	
	--修改库存现有量
	--增加库存
	IF (@Qty > 0)
	BEGIN
		--查找是否存在同批次的库存
		SELECT @Count = count(*) FROM WMS_Inv
			WHERE InvId = @InvId
				AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
				AND PartId = @PartId
				AND Isnull(Lot, 0) = Isnull(@Lot, 0);
		IF (@Count = 1)	--如果找到，则修改库存现有量
		BEGIN
			UPDATE WMS_Inv SET Qty = Qty + @Qty
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId
					AND Isnull(Lot, 0) = Isnull(@Lot, 0);
		END
		ELSE IF (@AllowAddLot = 1)	--新增批次
		BEGIN
			--如果批次不为空，则判断已有的库存现有量是否存在空批次（系统不允许存在空批次和非空批次同时存在的情况）
			IF (@Lot IS NOT NULL)
			BEGIN
				SELECT @Count = count(*) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Lot IS NULL;
				IF (@Count > 0)
				BEGIN
					;
					THROW 51000, '入库批次存在问题：当前批次不为空，但库存存在为空的批次，请确认！', 1;
					RETURN;
				END
			END
			--如果批次为空，则判断已有的库存现有量是否存在不为空批次（系统不允许存在空批次和非空批次同时存在的情况）
			IF (@Lot IS NULL)
			BEGIN
				SELECT @Count = count(*) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Lot IS NOT NULL;
				IF (@Count > 0)
				BEGIN
					;
					THROW 51000, '入库批次存在问题：当前批次为空，但库存存在不为空的批次，请确认！', 1;
					RETURN;
				END
			END
		
			--插入库存现有量
			INSERT INTO WMS_Inv (InvId,
								SubInvId,
								PartId,
								Lot,
								Qty)
						VALUES (@InvId,
								@SubInvId,
								@PartId,
								@Lot,
								@Qty
								);

		END
		ELSE  --增加库存时发生无效批次
		BEGIN
			;
			THROW 51000, '入库批次存在问题：当前批次库存不存在且该操作不允许新增批次，请确认！', 1;
			RETURN;
		END
	END


	--减少库存：当批次为空，则按先进先出的原则扣减库存；当批次非空时，只扣减指定批次的库存
	IF (@Qty < 0)
	BEGIN
		IF (@HasStockQty = 1)	--已经备料过，直接扣减库存，不用判断库存现有量
		BEGIN
			UPDATE WMS_Inv SET Qty = Qty + @Qty, 
								StockQty = StockQty + @Qty
				WHERE InvId = @InvId
					AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
					AND PartId = @PartId
					AND Isnull(Lot, 0) = Isnull(@Lot, 0);
		END
		ELSE
		BEGIN
			IF (@Lot IS NOT NULL) --批次不为空
			BEGIN
				SELECT @Count = count(*), @InvQty = SUM(Qty - StockQty) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Isnull(Lot, 0) = Isnull(@Lot, 0);
				IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
				BEGIN
					;
					THROW 51000, '当前批次的库存现有量不足，请确认！', 1;
					RETURN;
				END

				--扣减库存
				UPDATE WMS_Inv SET Qty = Qty + @Qty
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId
						AND Isnull(Lot, 0) = Isnull(@Lot, 0);
			END

			IF (@Lot IS NULL) --批次为空
			BEGIN
				SELECT @Count = count(*), @InvQty = SUM(Qty - StockQty) FROM WMS_Inv
					WHERE InvId = @InvId
						AND Isnull(SubInvId, 0) = Isnull(@SubInvId, 0)
						AND PartId = @PartId;
				IF (@Qty < 0 AND IsNull(@InvQty, 0) < ABS(@Qty) AND @AllowNegativeInv = 0)	--当减少库存、且不允许负库存、且库存现有量不足时，抛出异常
				BEGIN
					;
					THROW 51000, '库存现有量不足，请确认！', 1;
					RETURN;
				END

				--扣减库存：先进先出
				UPDATE WMS_Inv SET Qty = Qty -
						CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty), 0) 
									FROM WMS_Inv t
									WHERE t.InvId = @InvId
										AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
										AND t.PartId = @PartId
										AND Isnull(t.Lot, 0) <= Isnull(inv.Lot, 0)
										AND t.Qty > 0) >= 0
							THEN Qty
							ELSE CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty), 0) 
											FROM WMS_Inv t
											WHERE t.InvId = @InvId
												AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
												AND t.PartId = @PartId
												AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
												AND t.Qty > 0) < 0
									THEN 0
									ELSE (SELECT ABS(@Qty) - Isnull(SUM(t.Qty), 0) 
											FROM WMS_Inv t
											WHERE t.InvId = @InvId
												AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
												AND t.PartId = @PartId
												AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
												AND t.Qty > 0)
									END
							END,
						OutQty = 
						CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty), 0) 
									FROM WMS_Inv t
									WHERE t.InvId = @InvId
										AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
										AND t.PartId = @PartId
										AND Isnull(t.Lot, 0) <= Isnull(inv.Lot, 0)
										AND t.Qty > 0) >= 0
							THEN Qty
							ELSE CASE WHEN (SELECT ABS(@Qty) - Isnull(SUM(t.Qty), 0) 
											FROM WMS_Inv t
											WHERE t.InvId = @InvId
												AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
												AND t.PartId = @PartId
												AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
												AND t.Qty > 0) < 0
									THEN 0
									ELSE (SELECT ABS(@Qty) - Isnull(SUM(t.Qty), 0) 
											FROM WMS_Inv t
											WHERE t.InvId = @InvId
												AND Isnull(t.SubInvId, 0) = Isnull(@SubInvId, 0)
												AND t.PartId = @PartId
												AND Isnull(t.Lot, 0) < Isnull(inv.Lot, 0)
												AND t.Qty > 0)
									END
							END
					FROM WMS_Inv inv
					WHERE inv.InvId = @InvId
						AND Isnull(inv.SubInvId, 0) = Isnull(@SubInvId, 0)
						AND inv.PartId = @PartId;
			END
		END
	END

	--插入库存记录表
	INSERT INTO WMS_InvRecord (PartId,
								Lot,
								QTY,
								InvId,
								SubInvId,
								BillId,
								SourceBill,
								OperateDate,
								Type,
								OperateMan
								)
			VALUES (@PartId,
						@Lot,
						@Qty,
						@InvId,
						@SubInvId,	
						@BillId,
						@SourceBill,
						@now,
						@type,
						@UserId);
	IF (@@ERROR <> 0)
	BEGIN
		;
		THROW 51000, '保存库存记录时出错！', 1;
		RETURN
	END
END

GO

