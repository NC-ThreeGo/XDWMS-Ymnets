/****** Object:  Table [dbo].[WMS_AI]    Script Date: 2018/12/6 16:45:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_AI](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArrivalBillNum] [nvarchar](50) NULL,
	[POId] [int] NOT NULL,
	[BoxNum] [numeric](10, 3) NULL,
	[ArrivalNum] [decimal](10, 3) NULL,
	[ArrivalDate] [datetime] NULL,
	[ReceiveMan] [nvarchar](10) NULL,
	[ReceiveStatus] [nvarchar](10) NULL,
	[InspectBillNum] [nvarchar](50) NOT NULL,
	[InspectMan] [nvarchar](20) NULL,
	[InspectDate] [datetime] NULL,
	[InspectStatus] [nvarchar](10) NULL,
	[CheckOutDate] [datetime] NULL,
	[CheckOutResult] [nvarchar](50) NULL,
	[QualifyNum] [numeric](10, 3) NULL,
	[NoQualifyNum] [numeric](10, 3) NULL,
	[CheckOutRemark] [nvarchar](100) NULL,
	[ReInspectBillNum] [nvarchar](100) NULL,
	[InStoreBillNum] [nvarchar](50) NULL,
	[InStoreMan] [nvarchar](10) NULL,
	[InvCode] [nvarchar](50) NULL,
	[InStoreStatus] [nvarchar](10) NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_AI] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_AI] ADD  CONSTRAINT [DF_WMS_AI_ReceiveStatus]  DEFAULT (N'未到货') FOR [ReceiveStatus]
GO

ALTER TABLE [dbo].[WMS_AI] ADD  CONSTRAINT [DF_WMS_AI_InspectStatus]  DEFAULT (N'未送检') FOR [InspectStatus]
GO

ALTER TABLE [dbo].[WMS_AI] ADD  CONSTRAINT [DF_WMS_AI_InStoreStatus]  DEFAULT (N'未入库') FOR [InStoreStatus]
GO

ALTER TABLE [dbo].[WMS_AI]  WITH CHECK ADD  CONSTRAINT [FK_WMS_AI_WMS_PO] FOREIGN KEY([POId])
REFERENCES [dbo].[WMS_PO] ([Id])
GO

ALTER TABLE [dbo].[WMS_AI] CHECK CONSTRAINT [FK_WMS_AI_WMS_PO]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货单据号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ArrivalBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购订单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'POId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货箱数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'BoxNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ArrivalNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ArrivalDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ReceiveMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ReceiveStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'送检单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InspectBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'送检人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InspectMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'送检日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InspectDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'送检状体' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InspectStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'检验日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'CheckOutDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'检验结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'CheckOutResult'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'QualifyNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'不合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'NoQualifyNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'检验说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'CheckOutRemark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'重新送检单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ReInspectBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InStoreBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InStoreMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库仓库' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InvCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'InStoreStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_AI', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO

/****** Object:  Table [dbo].[WMS_InvRecord]    Script Date: 2018/12/6 16:45:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_InvRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartId] [int] NOT NULL,
	[QTY] [nchar](10) NOT NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[BillId] [int] NULL,
	[SourceBill] [nvarchar](50) NULL,
	[OperateDate] [datetime] NULL,
	[Type] [nvarchar](10) NULL,
	[OperateMan] [nvarchar](10) NULL,
 CONSTRAINT [PK_WMS_InvDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_InvRecord]  WITH CHECK ADD  CONSTRAINT [FK_WMS_InvRecord_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_InvRecord] CHECK CONSTRAINT [FK_WMS_InvRecord_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_InvRecord]  WITH CHECK ADD  CONSTRAINT [FK_WMS_InvRecord_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_InvRecord] CHECK CONSTRAINT [FK_WMS_InvRecord_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出入库明细ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'PartId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'QTY'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'BillId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据来源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'SourceBill'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'OperateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出入库类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'Type'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvRecord', @level2type=N'COLUMN',@level2name=N'OperateMan'
GO


/****** Object:  Table [dbo].[WMS_ReturnOrder]    Script Date: 2018/12/6 16:46:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_ReturnOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReturnOrderNum] [nvarchar](50) NULL,
	[AIID] [int] NULL,
	[PartID] [int] NULL,
	[SupplierId] [int] NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[ReturnNum] [decimal](10, 3) NOT NULL,
	[AdjustNum] [decimal](10, 3) NOT NULL,
	[Remark] [nchar](10) NULL,
	[PrintStaus] [nvarchar](10) NULL,
	[PrintDate] [datetime] NULL,
	[PrintMan] [nvarchar](10) NULL,
	[ConfirmStatus] [nvarchar](10) NULL,
	[ConfirmMan] [nvarchar](10) NULL,
	[ConfirmDate] [datetime] NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_ReturnOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_ReturnOrder] ADD  CONSTRAINT [DF_WMS_ReturnOrder_ConfirmStatus]  DEFAULT (N'未审核') FOR [ConfirmStatus]
GO

ALTER TABLE [dbo].[WMS_ReturnOrder]  WITH CHECK ADD  CONSTRAINT [FK_WMS_ReturnOrder_WMS_AI] FOREIGN KEY([AIID])
REFERENCES [dbo].[WMS_AI] ([Id])
GO

ALTER TABLE [dbo].[WMS_ReturnOrder] CHECK CONSTRAINT [FK_WMS_ReturnOrder_WMS_AI]
GO

ALTER TABLE [dbo].[WMS_ReturnOrder]  WITH CHECK ADD  CONSTRAINT [FK_WMS_ReturnOrder_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_ReturnOrder] CHECK CONSTRAINT [FK_WMS_ReturnOrder_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_ReturnOrder]  WITH CHECK ADD  CONSTRAINT [FK_WMS_ReturnOrder_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_ReturnOrder] CHECK CONSTRAINT [FK_WMS_ReturnOrder_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退货单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退货单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ReturnOrderNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货检验单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'AIID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'PartID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'代理商编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'SupplierId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应退货数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ReturnNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实际退货数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'AdjustNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退货说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'PrintStaus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'PrintDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'PrintMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ConfirmStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ConfirmMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ConfirmDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO


/****** Object:  Table [dbo].[WMS_Inv]    Script Date: 2018/12/6 16:49:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Inv](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvId] [int] NOT NULL,
	[SubInvId] [int] NOT NULL,
	[PartId] [int] NOT NULL,
	[Qty] [numeric](10, 3) NOT NULL,
 CONSTRAINT [PK_Inv] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_Inv]  WITH CHECK ADD  CONSTRAINT [FK_Inv_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inv] CHECK CONSTRAINT [FK_Inv_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_Inv]  WITH CHECK ADD  CONSTRAINT [FK_Inv_WMS_Part] FOREIGN KEY([PartId])
REFERENCES [dbo].[WMS_Part] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inv] CHECK CONSTRAINT [FK_Inv_WMS_Part]
GO

ALTER TABLE [dbo].[WMS_Inv]  WITH CHECK ADD  CONSTRAINT [FK_Inv_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inv] CHECK CONSTRAINT [FK_Inv_WMS_SubInvInfo]
GO






