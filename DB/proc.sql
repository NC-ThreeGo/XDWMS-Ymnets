ALTER   PROCEDURE [dbo].[P_WMS_ConfirmFeedList]
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

	--修改库存
	DECLARE cur_FeedList cursor for (select Id, SubAssemblyPartId, InvId, SubInvId, Lot, FeedQty * -1
											from WMS_Feed_List
											where ReleaseBillNum = @ReleaseBillNum
											  and ConfirmStatus = '未确认');
    --打开游标--
    open cur_FeedList;
    --开始循环游标变量--
    fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			BEGIN TRAN

			exec P_WMS_UpdateInvQty @UserId, @SubAssemblyPartId, @InvId, null, @Lot, 0, 1, @Qty, @now, '投料', @rowId, @ReleaseBillNum;

			--修改投料单行的确认状态
			update WMS_Feed_List set ConfirmStatus = '已确认', ConfirmMan = @UserId, ConfirmDate = @now,
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
        fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_FeedList  --关闭游标
    deallocate cur_FeedList   --释放游标

	IF @@TRANCOUNT > 0
		COMMIT TRAN ;

	IF (@countError > 0)
	BEGIN
		set @ReturnValue = '投料单确认成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
		RETURN;
	END
END

GO

ALTER   PROCEDURE [dbo].[P_WMS_ConfirmReturnOrder]
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

	--修改退货单状态
	update WMS_ReturnOrder set ConfirmStatus = '已确认', ConfirmMan = @UserId, ConfirmDate = @now,
                ModifyPerson = @UserId, ModifyTime = @now
          where ReturnOrderNum = @ReturnOrderNum;
	IF (@@ERROR <> 0)
	BEGIN
		;
		THROW 51000, '修改退货单状态时出错！', 1;
		RETURN
	END

	--修改库存：只对InvId不为空的记录修改库存（手工创建的库存退货单），根据不合格数量生成的退货单是没有进入库存的。
	DECLARE cur_ReturnOrder cursor for (select Id, PartId, InvId, SubInvId, Lot, AdjustQty * -1
											from WMS_ReturnOrder
											where ReturnOrderNum = @ReturnOrderNum
											  and InvId is not null);
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

	COMMIT TRAN
	RETURN
END
GO

ALTER   PROCEDURE [dbo].[P_WMS_CreateInspectBill]
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

ALTER   PROCEDURE [dbo].[P_WMS_CreateReInspect]
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
	DECLARE @PartId int;
	DECLARE @InvId int;
	DECLARE @SubInvId int;
	DECLARE @Lot varchar(50);
	DECLARE @OQualifyQty decimal(10, 3);
	DECLARE @qty decimal(10, 3);

	BEGIN TRAN

	SELECT @PartId = PartId,
			@InvId = InvId,
			@SubInvId = SubInvId,
			@Lot = Lot,
			@OQualifyQty = QualifyQty
		FROM WMS_AI
		WHERE Id = @AIID;

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
	exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, @SubInvId, @Lot, 0, 1, @qty, @now, '重新送检', @rowId, null

	COMMIT TRAN
	RETURN
END

GO

--手工创建退货单
ALTER PROCEDURE [dbo].[P_WMS_CreateReturnOrder]
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
								Remark,
								PrintStaus,
								CreatePerson,
								CreateTime) 
						VALUES	(--@ReturnOrderNum,
								@PartId,
								@Lot,
								@SupplierId,
								@InvId,
								null,
								@Qty,
								@Qty,
								@Remark,
								'未退货',
								@UserId,
								@now
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

ALTER   PROCEDURE [dbo].[P_WMS_InvAdjust]
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
	exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, null, @Lot, 0, 0,
		@AdjustQty, @now, '调账', @rowId, @InvAdjustBillNum

	COMMIT TRAN
	RETURN

END

GO

ALTER  PROCEDURE [dbo].[P_WMS_InvStock]	--库存备料
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
										Stock_InvId
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
										@rowId);


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

ALTER   PROCEDURE [dbo].[P_WMS_PrintFeedList]
	@UserId varchar(50),
	@FeedBillNum nvarchar(50),
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

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'TL', 'WMS_Feed_List', @now, @ReleaseBillNum output

	--进行库存备料
	DECLARE cur_FeedList cursor for (select Id, SubAssemblyPartId, InvId, SubInvId, Lot, FeedQty * -1
											from WMS_Feed_List
											where FeedBillNum = @FeedBillNum
											  and PrintStaus = '未打印');
    --打开游标--
    open cur_FeedList;
    --开始循环游标变量--
    fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			BEGIN TRAN

			exec P_WMS_InvStock @UserId, @SubAssemblyPartId, @InvId, null, @Lot, @Qty, @now, '投料', @rowId, @ReleaseBillNum;

			--修改投料单行的打印状态
			update WMS_Feed_List set ReleaseBillNum = @ReleaseBillNum,
					PrintStaus = '已打印', PrintMan = @UserId, PrintDate = @now,
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
        fetch next from cur_FeedList into @rowId, @SubAssemblyPartId, @InvId, @SubInvId, @Lot, @Qty;  
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

ALTER   PROCEDURE [dbo].[P_WMS_PrintReturnOrder]
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
					AdjustQty decimal(10, 3),
					Remark nvarchar(200)
				) 
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '临时保存检验信息时出错！'
		RETURN
	END

	BEGIN TRAN

	--修改表的BatchId，以解决并发问题
	SELECT @batchId = NEXT VALUE FOR S_WMS_BatchId;
	update WMS_ReturnOrder set BatchId = @batchId
			FROM WMS_ReturnOrder ro,
				 #ReturnOrder t
			WHERE ro.Id = t.Id
			  AND BatchId is null
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '修改BatchId时出错！'
		RETURN
	END

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'TH', 'WMS_ReturnOrder', @now, @ReturnOrderNum output

	update WMS_ReturnOrder set ReturnOrderNum = @ReturnOrderNum,
								--AdjustQty = t.AdjustQty,
								--Remark = t.Remark,
								PrintStaus = '已退货',
								PrintDate = @now,
								PrintMan = @UserId,
								ModifyPerson = @UserId,
								ModifyTime = @now
			FROM WMS_ReturnOrder ro,
				 #ReturnOrder t
			WHERE ro.Id = t.Id
				AND ro.BatchId = @batchId
	IF (@@ERROR <> 0)
	BEGIN
		set @ReturnValue = '保存退货记录时出错！'
		RETURN
	END


	COMMIT TRAN
	RETURN


END

GO

ALTER   PROCEDURE [dbo].[P_WMS_ProcessInspectBill]
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
									PrintStaus,
									CreatePerson,
									CreateTime
									)
				SELECT ib.Id,
						ib.PartId,
						ib.Lot,
						po.SupplierId,
						ib.NoQualifyQty,
						ib.NoQualifyQty,
						'未退货',
						@UserId,
						@now
					FROM #InspectBill ib,
							WMS_PO po
					WHERE ib.POId = po.Id
						AND ib.NoQualifyQty <> 0

	END


	COMMIT TRAN
	RETURN
END

GO

ALTER   PROCEDURE [dbo].[P_WMS_ProcessProductEntry]
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
	exec P_WMS_GetMaxNum 'RK', 'WMS_Product_Entry', @now, @EntryBillNum output

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
						'入库',
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

ALTER     PROCEDURE [dbo].[P_WMS_UpdateInvQty]
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
			--修改库存现有量
			UPDATE WMS_Inv SET Qty = inv.Qty - r.QTY, 
								StockQty = inv.StockQty - r.Qty
				FROM WMS_Inv inv,
					 WMS_InvRecord r
				WHERE r.Type = @type
				  AND r.BillId = @BillId
				  AND r.Stock_InvId = inv.Id;

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
										Stock_InvId
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
										null
									FROM WMS_InvRecord r
									WHERE r.Type = @type
										AND r.BillId = @BillId
										AND r.Stock_InvId is not null;
		END
		ELSE
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
											Stock_InvId
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
											null);

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
go



CREATE  OR ALTER    PROCEDURE [dbo].[P_WMS_ConfirmSaleOrder]
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

	--修改库存
	DECLARE cur_SaleOrder cursor for (select Id, PartId, InvId, SubInvId, Lot, Qty * -1
											from WMS_Sale_Order
											where SellBillNum = @SellBillNum
											  and ConfirmStatus = '未确认');
    --打开游标--
    open cur_SaleOrder;
    --开始循环游标变量--
    fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			BEGIN TRAN

			exec P_WMS_UpdateInvQty @UserId, @PartId, @InvId, null, @Lot, 0, 1, @Qty, @now, '销售', @rowId, @SellBillNum;

			--修改投料单行的确认状态
			update WMS_Sale_Order set ConfirmStatus = '已确认', ConfirmMan = @UserId, ConfirmDate = @now,
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
        fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;  
    end    
    close cur_SaleOrder  --关闭游标
    deallocate cur_SaleOrder   --释放游标

	IF @@TRANCOUNT > 0
		COMMIT TRAN ;

	IF (@countError > 0)
	BEGIN
		set @ReturnValue = '销售订单确认成功:' + CONVERT(varchar, @countOK) + '行，失败:' + CONVERT(varchar, @countError) + '行，具体请查看错误信息！';
		RETURN;
	END
END

GO


CREATE OR ALTER    PROCEDURE [dbo].[P_WMS_PrintSaleOrder]
	@UserId varchar(50),
	@SaleBillNum nvarchar(50),
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

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'XS', 'WMS_Sale_Order', @now, @SellBillNum output

	--进行库存备料
	DECLARE cur_SaleOrder cursor for (select Id, PartId, InvId, SubInvId, Lot, Qty * -1
											from WMS_Sale_Order
											where SaleBillNum = @SaleBillNum
											  and PrintStaus = '未打印');
    --打开游标--
    open cur_SaleOrder;
    --开始循环游标变量--
    fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;
    while @@FETCH_STATUS = 0    --返回被 FETCH语句执行的最后游标的状态--
    begin         
		BEGIN TRY   
			BEGIN TRAN

			exec P_WMS_InvStock @UserId, @PartId, @InvId, null, @Lot, @Qty, @now, '销售', @rowId, @SellBillNum;

			--修改投料单行的打印状态
			update WMS_Sale_Order set SellBillNum = @SellBillNum,
					PrintStaus = '已打印', PrintMan = @UserId, PrintDate = @now,
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
        fetch next from cur_SaleOrder into @rowId, @PartId, @InvId, @SubInvId, @Lot, @Qty;  
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

