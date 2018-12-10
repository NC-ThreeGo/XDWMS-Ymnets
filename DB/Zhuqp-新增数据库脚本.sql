ALTER   PROCEDURE [dbo].[P_WMS_ProcessInspectBill]
	@userId varchar(50),
	@jsonInspectBill NVARCHAR(MAX) --检验结果
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	--库房和子库存
	DECLARE @InvId int
	DECLARE @SubInvId int = -1

	DECLARE @InspectBillNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @count int
	DECLARE @InStoreBillNum varchar(50)
	DECLARE @ReturnOrderNum varchar(50)

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'RK', 'WMS_AI', @now

	BEGIN TRAN

	--将检验结果保存到临时表
	SELECT ib.*, po.PartId  
		INTO #InspectBill
		FROM OPENJSON(@jsonInspectBill)  
			WITH (	Id int,
					POId int,
					InspectBillNum nvarchar(50),
					CheckOutDate date,
					CheckOutResult nvarchar(50),
					QualifyQty decimal(10, 3),
					InvCode varchar(20),
					NoQualifyQty decimal(10, 3),
					CheckOutRemark nvarchar(50)
				) ib,
			WMS_PO po
		WHERE ib.POId = po.Id

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
						WMS_AI.InStoreMan = @userId,
						WMS_AI.InvCode = t.InvCode
					FROM #InspectBill t,
						WMS_AI
					WHERE WMS_AI.Id = t.Id
							AND WMS_AI.InspectBillNum = @InspectBillNum

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
							@SubInvId,	--哪个子库存？？？
							ib.Id,
							@InStoreBillNum,
							@now,
							'入库',
							@userId
							FROM #InspectBill ib
		--修改库存现有量
		----1.库房+子库存+物料记录如果不存在，则增加（数量=0）
		BEGIN TRAN
			INSERT INTO WMS_Inv (InvId,
								SubInvId,
								PartId,
								Qty
								)
					SELECT	ib.InvId,	
							@SubInvId,	--哪个子库存？？？
							ib.PartId,
							0
							FROM #InspectBill ib
							WHERE not exists (
												SELECT 1 FROM WMS_Inv inv
													WHERE inv.InvId = ib.InvId
														AND inv.SubInvId = @SubInvId
														AND inv.PartId = ib.PartId
												)

		COMMIT TRAN

		----2.修改现有量
		UPDATE WMS_Inv SET Qty = Qty + ib.QualifyQty
			FROM WMS_Inv inv,
				#InspectBill ib
			WHERE inv.InvId = @InvId
				AND inv.SubInvId = @SubInvId
				AND inv.PartId = ib.PartId
	END


	--对不合格数量进行退库处理
	SELECT @count = count(*) FROM #InspectBill
		WHERE NoQualifyQty <> 0
	IF (@count > 0)
	BEGIN
		--插入退货记录
		select 1
	END


	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'SJ', 'WMS_AI', @now, @InspectBillNum output


	COMMIT TRAN
END
go



ALTER   PROCEDURE [dbo].[P_WMS_CreateInspectBill]
	-- Add the parameters for the stored procedure here
	@userId varchar(50),
	@arrivalBillNum varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	DECLARE @billNum varchar(50)
	DECLARE @now date = getdate()
	DECLARE @defaultInvCode varchar(20)

	--先初始化当前日期、当前type的Num（要在事务开始之前执行）
	exec P_WMS_InitNumForDay 'SJ', 'WMS_AI', @now

	BEGIN TRAN

	--获取当前的单据编号
	exec P_WMS_GetMaxNum 'SJ', 'WMS_AI', @now, @billNum output

	SELECT top 1 @defaultInvCode = InvCode from WMS_InvInfo
		WHERE Status = '有效' AND IsDefault = 1

	update WMS_AI set InspectBillNum = @billNum,
					  InspectMan = @userId,
					  InspectDate = @now,
					  InspectStatus = '已检验',
					  CheckOutDate = @now,
					  InvCode = @defaultInvCode
			where ArrivalBillNum = @arrivalBillNum


	COMMIT TRAN
END
go




ALTER PROCEDURE [dbo].[P_WMS_GetMaxNum]
	@type varchar(50),
	@tabname varchar(50),
	@now date,
	@result nvarchar(50) output 
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int
	declare @maxNum int

	--获取单据编号
	select @count = count(*) from WMS_Num t
		where t.Type = @type and t.TabName = @tabname  and t.Day = @now
	IF (@count = 0)
	BEGIN
		set @maxNum = 0
		begin tran
		insert into WMS_Num (Num, Day, Type, TabName, MinNum, MaxNum)
			values (@maxNum, @now, @type, @tabname, 0, 9999)
		commit tran
	END

	select @maxNum = t.Num + 1 from WMS_Num t WITH (UPDLOCK)
		where t.Type = @type and t.Day = @now

	--修改当前的单据编号
	update WMS_Num set Num = @maxNum 
		where Type = @type and DATEDIFF(day, Day, @now) = 0

	set @result = @type + CONVERT(varchar(8), @now, 112) + replace(right(str(@maxNum), 4),' ','0')

END
go



ALTER PROCEDURE [dbo].[P_WMS_InitNumForDay]
	@type varchar(50),
	@tabname varchar(50),
	@now date
AS
BEGIN
	SET NOCOUNT ON;

	declare @count int
	declare @maxNum int

	--获取单据编号
	select @count = count(*) from WMS_Num t
		where t.Type = @type and t.TabName = @tabname  and t.Day = @now
	IF (@count = 0)
	BEGIN
		set @maxNum = 0
		insert into WMS_Num (Num, Day, Type, TabName, MinNum, MaxNum)
			values (@maxNum, @now, @type, @tabname, 0, 9999)
	END
END
go



