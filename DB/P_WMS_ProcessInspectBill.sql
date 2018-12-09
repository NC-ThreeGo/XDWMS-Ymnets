CREATE OR ALTER PROCEDURE P_WMS_ProcessInspectBill
	@userId varchar(50),
	@jsonInspectBill NVARCHAR(MAX) --检验结果
AS
BEGIN
	SET NOCOUNT ON;
	set xact_abort on   

	--库房和子库存
	DECLARE @InvId int
	DECLARE @SubInvId int

	DECLARE @InspectBillNum varchar(50)
	DECLARE @now datetime = getdate()
	DECLARE @count int
	DECLARE @InStoreBillNum varchar(50)
	DECLARE @ReturnOrderNum varchar(50)

	BEGIN TRAN

	--将检验结果保存到临时表
	SELECT *  
		INTO #InspectBill
		FROM OPENJSON(@jsonInspectBill)  
			WITH (	Id int,
					POId int,
					InspectBillNum nvarchar(50),
					CheckOutResult nvarchar(50),
					QualifyQty decimal(10, 3),
					NoQualifyQty decimal(10, 3),
					CheckOutRemark nvarchar(50)
				)

	SELECT top 1 @InspectBillNum = InspectBillNum FROM #InspectBill

	--保存检验结果
	update WMS_AI SET	WMS_AI.InspectStatus = '已检验',
						WMS_AI.CheckOutDate = @now,
						WMS_AI.CheckOutResult = t.CheckOutResult,
						WMS_AI.QualifyQty = t.QualifyQty,
						WMS_AI.NoQualifyQty = t.NoQualifyQty,
						WMS_AI.CheckOutRemark = t.CheckOutRemark,
						WMS_AI.InStoreBillNum = ''
					FROM #InspectBill t,
						WMS_AI
					WHERE WMS_AI.Id = t.Id
							AND WMS_AI.InspectBillNum = @InspectBillNum

	--对合格数量进行入库处理
	SELECT @count = count(*) FROM #InspectBill
		WHERE QualifyQty <> 0
	IF (@count > 0)
	BEGIN
		--获取入库单号
		exec P_WMS_GetMaxNum 'RK', 'WMS_AI', @now, @InStoreBillNum output
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
					SELECT po.PartId,
							ib.QualifyQty,
							@InvId,	--哪个库房？？？
							@SubInvId,	--哪个子库存？？？
							ib.Id,
							@InStoreBillNum,
							@now,
							'入库',
							@userId
							FROM #InspectBill ib,
								WMS_PO po
							WHERE ib.POId = po.Id
		--修改库存现有量
		----1.库房+子库存+物料记录如果不存在，则增加（数量=0）
		BEGIN TRAN
			INSERT INTO WMS_Inv (InvId,
								SubInvId,
								PartId,
								Qty
								)
					SELECT	@InvId,	--哪个库房？？？
							@SubInvId,	--哪个子库存？？？
							po.PartId,
							0
							FROM #InspectBill ib,
								WMS_PO po
							WHERE ib.POId = po.Id
								AND not exists (
												SELECT 1 FROM WMS_Inv inv
													WHERE inv.InvId = @InvId
														AND inv.SubInvId = @SubInvId
														AND inv.PartId = po.PartId
												)

		COMMIT TRAN

		----2.修改现有量
		UPDATE WMS_Inv SET Qty = Qty + QualifyQty
			FROM WMS_Inv inv,
				#InspectBill ib,
				WMS_PO po
			WHERE ib.POId = po.Id
				AND inv.InvId = @InvId
				AND inv.SubInvId = @SubInvId
				AND inv.PartId = po.PartId
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
GO
