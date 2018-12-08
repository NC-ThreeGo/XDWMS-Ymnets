/****** Object:  Table [dbo].[WMS_ReInspect]    Script Date: 2018/12/8 12:15:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_ReInspect](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AIId] [int] NOT NULL,
	[OCheckOutResult] [nvarchar](50) NULL,
	[OQualifyQty] [decimal](10, 3) NULL,
	[ONoQualifyQty] [decimal](10, 3) NULL,
	[OCheckOutRemark] [nvarchar](100) NULL,
	[OCheckOutDate] [datetime] NULL,
	[NCheckOutResult] [nvarchar](50) NULL,
	[NQualifyQty] [decimal](10, 3) NULL,
	[NNoQualifyQty] [decimal](10, 3) NULL,
	[NCheckOutRemark] [nvarchar](100) NULL,
	[NCheckOutDate] [datetime] NULL,
	[Remark] [nvarchar](100) NULL,
	[AdjustMan] [nvarchar](10) NULL,
	[AdjustDate] [datetime] NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](10) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_ReInspect] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'重新送检单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到货送检单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'AIId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OCheckOutResult'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OQualifyQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单不合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'ONoQualifyQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OCheckOutRemark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单检验日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OCheckOutDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NCheckOutResult'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NQualifyQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单不合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NNoQualifyQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单检验结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NCheckOutRemark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单检验日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NCheckOutDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调整说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调整人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'AdjustMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调整时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'AdjustDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO

/****** Object:  Table [dbo].[WMS_ReturnOrder]    Script Date: 2018/12/8 12:17:23 ******/
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
	[ReturnQty] [decimal](10, 3) NOT NULL,
	[AdjustQty] [decimal](10, 3) NOT NULL,
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应退货数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'ReturnQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实际退货数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReturnOrder', @level2type=N'COLUMN',@level2name=N'AdjustQty'
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



