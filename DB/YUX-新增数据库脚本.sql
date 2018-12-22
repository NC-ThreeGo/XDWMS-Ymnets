/****** Object:  Table [dbo].[WMS_Feed_List]    Script Date: 2018/12/22 18:47:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Feed_List](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FeedBillNum] [nvarchar](50) NULL,
	[ReleaseBillNum] [nvarchar](50) NOT NULL,
	[Department] [nvarchar](50) NULL,
	[AssemblyPartId] [int] NOT NULL,
	[SubAssemblyPartId] [int] NOT NULL,
	[FeedQty] [decimal](10, 3) NOT NULL,
	[BoxQty] [decimal](10, 3) NULL,
	[Capacity] [nvarchar](50) NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[Remark] [nvarchar](100) NULL,
	[PrintStaus] [nvarchar](10) NULL,
	[PrintDate] [date] NULL,
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
	[ModifyTime] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_Feed_List]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Feed_List_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Feed_List] CHECK CONSTRAINT [FK_WMS_Feed_List_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_Feed_List]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Feed_List_WMS_Part] FOREIGN KEY([AssemblyPartId])
REFERENCES [dbo].[WMS_Part] ([Id])
GO

ALTER TABLE [dbo].[WMS_Feed_List] CHECK CONSTRAINT [FK_WMS_Feed_List_WMS_Part]
GO

ALTER TABLE [dbo].[WMS_Feed_List]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Feed_List_WMS_Part1] FOREIGN KEY([SubAssemblyPartId])
REFERENCES [dbo].[WMS_Part] ([Id])
GO

ALTER TABLE [dbo].[WMS_Feed_List] CHECK CONSTRAINT [FK_WMS_Feed_List_WMS_Part1]
GO

ALTER TABLE [dbo].[WMS_Feed_List]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Feed_List_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Feed_List] CHECK CONSTRAINT [FK_WMS_Feed_List_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投料单号（业务）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'FeedBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投料单号（系统）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'ReleaseBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投料部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'Department'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总成物料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'AssemblyPartId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投料物料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'SubAssemblyPartId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投料数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'FeedQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'箱数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'BoxQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'体积' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'Capacity'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'SubInvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'PrintStaus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'PrintDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'PrintMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'ConfirmStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'ConfirmMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'ConfirmDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Feed_List', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO


/****** Object:  Table [dbo].[WMS_Inv_Adjust]    Script Date: 2018/12/22 18:48:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Inv_Adjust](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvAdjustBillNum] [nvarchar](50) NOT NULL,
	[PartId] [int] NOT NULL,
	[AdjustQty] [decimal](10, 3) NOT NULL,
	[AdjustType] [nvarchar](50) NOT NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[Remark] [nvarchar](100) NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_Inv_Adjust]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Inv_Adjust_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inv_Adjust] CHECK CONSTRAINT [FK_WMS_Inv_Adjust_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_Inv_Adjust]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Inv_Adjust_WMS_Part] FOREIGN KEY([PartId])
REFERENCES [dbo].[WMS_Part] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inv_Adjust] CHECK CONSTRAINT [FK_WMS_Inv_Adjust_WMS_Part]
GO

ALTER TABLE [dbo].[WMS_Inv_Adjust]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Inv_Adjust_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inv_Adjust] CHECK CONSTRAINT [FK_WMS_Inv_Adjust_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调帐单据号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'InvAdjustBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'PartId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调整数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'AdjustQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调整类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'AdjustType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'SubInvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inv_Adjust', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO



/****** Object:  Table [dbo].[WMS_Inventory]    Script Date: 2018/12/22 18:49:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Inventory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InventoryBillNum] [nvarchar](50) NOT NULL,
	[PartId] [int] NOT NULL,
	[InventoryQty] [decimal](10, 3) NOT NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[Remark] [nvarchar](100) NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_Inventory]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Inventory_WMS_Inv] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inventory] CHECK CONSTRAINT [FK_WMS_Inventory_WMS_Inv]
GO

ALTER TABLE [dbo].[WMS_Inventory]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Inventory_WMS_Part] FOREIGN KEY([PartId])
REFERENCES [dbo].[WMS_Part] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inventory] CHECK CONSTRAINT [FK_WMS_Inventory_WMS_Part]
GO

ALTER TABLE [dbo].[WMS_Inventory]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Inventory_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Inventory] CHECK CONSTRAINT [FK_WMS_Inventory_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点单据号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'InventoryBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'PartId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'InventoryQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'SubInvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Inventory', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO



/****** Object:  Table [dbo].[WMS_Product_Entry]    Script Date: 2018/12/22 18:49:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Product_Entry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductBillNum] [nvarchar](100) NULL,
	[EntryBillNum] [nvarchar](50) NOT NULL,
	[Department] [nvarchar](50) NULL,
	[Partid] [int] NOT NULL,
	[ProductQty] [decimal](10, 3) NOT NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[Remark] [nvarchar](100) NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_Product_Entry]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Product_Entry_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Product_Entry] CHECK CONSTRAINT [FK_WMS_Product_Entry_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_Product_Entry]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Product_Entry_WMS_Part] FOREIGN KEY([Partid])
REFERENCES [dbo].[WMS_Part] ([Id])
GO

ALTER TABLE [dbo].[WMS_Product_Entry] CHECK CONSTRAINT [FK_WMS_Product_Entry_WMS_Part]
GO

ALTER TABLE [dbo].[WMS_Product_Entry]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Product_Entry_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Product_Entry] CHECK CONSTRAINT [FK_WMS_Product_Entry_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库单号（业务）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'ProductBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'入库单号（系统）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'EntryBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'本货部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'Department'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'Partid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'ProductQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'SubInvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Product_Entry', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO



/****** Object:  Table [dbo].[WMS_Sale_Order]    Script Date: 2018/12/22 18:50:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Sale_Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleBillNum] [nvarchar](50) NULL,
	[SellBillNum] [nvarchar](50) NOT NULL,
	[PlanDeliveryDate] [datetime] NULL,
	[CustomerId] [int] NOT NULL,
	[Qty] [decimal](10, 3) NOT NULL,
	[BoxQty] [decimal](10, 3) NULL,
	[InvId] [int] NULL,
	[SubInvId] [int] NULL,
	[Remark] [nvarchar](100) NULL,
	[PrintStaus] [nvarchar](10) NULL,
	[PrintDate] [datetime] NULL,
	[PrintMan] [nvarchar](10) NULL,
	[ConfirmStatus] [nvarchar](10) NULL,
	[ConfirmMan] [nvarchar](10) NULL,
	[ConfirmDate] [date] NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [date] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_Sale_Order]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Sale_Order_WMS_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[WMS_Customer] ([Id])
GO

ALTER TABLE [dbo].[WMS_Sale_Order] CHECK CONSTRAINT [FK_WMS_Sale_Order_WMS_Customer]
GO

ALTER TABLE [dbo].[WMS_Sale_Order]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Sale_Order_WMS_InvInfo] FOREIGN KEY([InvId])
REFERENCES [dbo].[WMS_InvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Sale_Order] CHECK CONSTRAINT [FK_WMS_Sale_Order_WMS_InvInfo]
GO

ALTER TABLE [dbo].[WMS_Sale_Order]  WITH CHECK ADD  CONSTRAINT [FK_WMS_Sale_Order_WMS_SubInvInfo] FOREIGN KEY([SubInvId])
REFERENCES [dbo].[WMS_SubInvInfo] ([Id])
GO

ALTER TABLE [dbo].[WMS_Sale_Order] CHECK CONSTRAINT [FK_WMS_Sale_Order_WMS_SubInvInfo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售单号（业务）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'SaleBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售单号（系统）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'SellBillNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计划发货日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'PlanDeliveryDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'Qty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'箱数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'BoxQty'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'InvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'SubInvId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'PrintStaus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'PrintDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打印人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'PrintMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'ConfirmStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'ConfirmMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'ConfirmDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Sale_Order', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO






