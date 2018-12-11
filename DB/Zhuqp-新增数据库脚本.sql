
/****** Object:  StoredProcedure [dbo].[P_WMS_CreateInspectBill]    Script Date: 2018/12/11 15:17:50 ******/
DROP PROCEDURE [dbo].[P_WMS_CreateInspectBill]
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_CreateInspectBill]    Script Date: 2018/12/11 15:17:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[P_WMS_CreateInspectBill]
	-- Add the parameters for the stored procedure here
	@UserId varchar(50),
	@ArrivalBillNum varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @billNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @defaultInvId int

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'SJ', 'WMS_AI', @now

	BEGIN TRAN

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'SJ', 'WMS_AI', @now, @billNum output

	SELECT top 1 @defaultInvId = Id from WMS_InvInfo
		WHERE Status = '有效' AND IsDefault = 1

	update WMS_AI set InspectBillNum = @billNum,
					  InspectMan = @UserId,
					  InspectDate = @now,
					  InspectStatus = '已检验',
					  CheckOutDate = @now,
					  InvId = @defaultInvId
			where ArrivalBillNum = @ArrivalBillNum


	COMMIT TRAN
END

GO


CREATE PROCEDURE [dbo].[p_wms_deleteTestData]
AS
BEGIN
delete [WMS_AI]
delete wms_po
delete wms_inv
delete WMS_InvRecord
delete WMS_ReturnOrderEND
end

GO



/****** Object:  StoredProcedure [dbo].[P_WMS_GetMaxNum]    Script Date: 2018/12/11 15:18:21 ******/
DROP PROCEDURE [dbo].[P_WMS_GetMaxNum]
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_GetMaxNum]    Script Date: 2018/12/11 15:18:21 ******/
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
		begin tran
		insert into WMS_Num (Num, Day, Type, TabName, MinNum, MaxNum)
			values (@maxNum, @Now, @Type, @Tabname, 0, 9999)
		commit tran
	END

	--修改当前的单据编号
	update WMS_Num set Num = Num + 1 
		where Type = @type and Day = @now

	select @maxNum = Num + 1 from WMS_Num t
		where t.Type = @Type and t.Day = @Now


	set @Result = @Type + CONVERT(varchar(8), @Now, 112) + replace(right(str(@maxNum), 4),' ','0')

END

GO

/****** Object:  StoredProcedure [dbo].[P_WMS_InitNumForDay]    Script Date: 2018/12/11 15:18:34 ******/
DROP PROCEDURE [dbo].[P_WMS_InitNumForDay]
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_InitNumForDay]    Script Date: 2018/12/11 15:18:34 ******/
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



/****** Object:  StoredProcedure [dbo].[P_WMS_ProcessInspectBill]    Script Date: 2018/12/11 15:21:40 ******/
DROP PROCEDURE [dbo].[P_WMS_ProcessInspectBill]
GO

/****** Object:  StoredProcedure [dbo].[P_WMS_ProcessInspectBill]    Script Date: 2018/12/11 15:21:40 ******/
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
						Qty
						)
			SELECT	ib.InvId,	
					ib.SubInvId,	
					ib.PartId,
					0
					FROM #InspectBill ib
					WHERE ib.QualifyQty <> 0
						AND not exists (
										SELECT 1 FROM WMS_Inv inv
											WHERE inv.InvId = ib.InvId
												AND Isnull(inv.SubInvId, 0) = Isnull(ib.SubInvId, 0)
												AND inv.PartId = ib.PartId
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
	update WMS_AI SET	WMS_AI.InspectStatus = '已检验',
						WMS_AI.CheckOutDate = t.CheckOutDate,
						WMS_AI.CheckOutResult = t.CheckOutResult,
						WMS_AI.QualifyQty = t.QualifyQty,
						WMS_AI.NoQualifyQty = t.NoQualifyQty,
						WMS_AI.CheckOutRemark = t.CheckOutRemark,
						WMS_AI.InStoreBillNum = @InStoreBillNum,
						WMS_AI.InStoreStatus = '已入库',
						WMS_AI.InStoreMan = @UserId,
						WMS_AI.InvId = t.InvId,
						WMS_AI.SubInvId = t.SubInvId
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
									SupplierId,
									ReturnQty,
									AdjustQty,
									CreatePerson,
									CreateTime
									)
				SELECT ib.Id,
						ib.PartId,
						po.SupplierId,
						ib.NoQualifyQty,
						ib.NoQualifyQty,
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