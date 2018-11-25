USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_Part]    Script Date: 2018/11/23 11:37:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Part](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartCode] [nvarchar](50) NOT NULL,
	[PartName] [nvarchar](100) NOT NULL,
	[PartType] [nvarchar](50) NULL,
	[CustomerCode] [nvarchar](50) NULL,
	[LogisticsCode] [nvarchar](50) NULL,
	[OtherCode] [nvarchar](50) NULL,
	[PCS] [decimal](10, 3) NULL,
	[StoreMan] [nvarchar](30) NULL,
	[Status] [nvarchar](10) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_Part] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'PartCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'PartName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'PartType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'CustomerCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物流号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'LogisticsCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'额外信息编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'OtherCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'每箱数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'PCS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保管员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'StoreMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Part', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO

USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_Supplier]    Script Date: 2018/11/24 12:48:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierCode] [nvarchar](50) NOT NULL,
	[SupplierShortName] [nvarchar](50) NULL,
	[SupplierName] [nvarchar](100) NOT NULL,
	[SupplierType] [nvarchar](10) NULL,
	[LinkMan] [nvarchar](20) NULL,
	[LinkManTel] [nvarchar](20) NULL,
	[LinkManAddress] [nvarchar](100) NULL,
	[Status] [nvarchar](10) NULL,
	[Remark] [nvarchar](500) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'SupplierCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商简称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'SupplierShortName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'SupplierName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'SupplierType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'LinkMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'LinkManTel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'LinkManAddress'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Supplier', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO


USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_Customer]    Script Date: 2018/11/24 12:48:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerCode] [nvarchar](20) NOT NULL,
	[CustomerShortName] [nvarchar](20) NULL,
	[CustomerName] [nvarchar](50) NOT NULL,
	[CustomerType] [nvarchar](20) NULL,
	[LinkMan] [nvarchar](20) NULL,
	[LinkManTel] [nvarchar](20) NULL,
	[LinkManAddress] [nvarchar](100) NULL,
	[Status] [nvarchar](10) NULL,
	[Remark] [nvarchar](200) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'CustomerCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户简称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'CustomerShortName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'CustomerType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'LinkMan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'LinkManTel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'LinkManAddress'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_Customer', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO


USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_InvInfo]    Script Date: 2018/11/24 12:49:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_InvInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvCode] [nvarchar](20) NOT NULL,
	[InvName] [nvarchar](50) NOT NULL,
	[Remark] [nchar](10) NULL,
	[Status] [nchar](10) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_InvInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'InvCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'InvName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvInfo', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO


USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_SubInvInfo]    Script Date: 2018/11/24 12:49:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_SubInvInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubInvCode] [nvarchar](20) NOT NULL,
	[SubInvName] [nvarchar](50) NOT NULL,
	[InvCode] [nvarchar](20) NOT NULL,
	[Status] [nvarchar](10) NULL,
	[Remark] [nvarchar](100) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_SubInvInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库位ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库位编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'SubInvCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库位名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'SubInvName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'InvCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_SubInvInfo', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO

USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_AI]    Script Date: 2018/11/25 18:43:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_AI](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArrivalBillNum] [nvarchar](50) NULL,
	[POId] [int] NULL,
	[BoxNum] [numeric](10, 3) NULL,
	[ArrivalNum] [decimal](10, 3) NULL,
	[ArrivalDate] [datetime] NULL,
	[ReceiveMan] [nvarchar](10) NULL,
	[ReceiveStatus] [nvarchar](10) NULL,
	[InspectBillNum] [nvarchar](50) NULL,
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


USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_PO]    Script Date: 2018/11/25 18:44:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_PO](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PO] [nvarchar](50) NOT NULL,
	[PODate] [nvarchar](50) NULL,
	[SupplierShortName] [nvarchar](50) NULL,
	[SupplierCode] [nvarchar](50) NULL,
	[PartCode] [nvarchar](50) NULL,
	[QTY] [decimal](10, 3) NULL,
	[PlanDate] [nvarchar](50) NULL,
	[POType] [nvarchar](10) NULL,
	[Status] [nvarchar](10) NULL,
	[Remark] [nvarchar](100) NULL,
	[Attr1] [nvarchar](50) NULL,
	[Attr2] [nvarchar](50) NULL,
	[Attr3] [nvarchar](50) NULL,
	[Attr4] [nvarchar](50) NULL,
	[Attr5] [nvarchar](50) NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_WMS_PO] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WMS_PO] ADD  CONSTRAINT [DF_WMS_PO_Status]  DEFAULT (N'正常') FOR [Status]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购订单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购订单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'PO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'PODate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商简称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'SupplierShortName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'SupplierCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'PartCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'QTY'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计划到货日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'PlanDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购订单类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'POType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_PO', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO


USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_ReInspect]    Script Date: 2018/11/25 18:44:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_ReInspect](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AIId] [int] NOT NULL,
	[OCheckOutResult] [nvarchar](50) NULL,
	[OQualifyNum] [decimal](10, 3) NULL,
	[ONoQualifyNum] [decimal](10, 3) NULL,
	[OCheckOutRemark] [nvarchar](100) NULL,
	[OCheckOutDate] [datetime] NULL,
	[NCheckOutResult] [nvarchar](50) NULL,
	[NQualifyNum] [decimal](10, 3) NULL,
	[NNoQualifyNum] [decimal](10, 3) NULL,
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OQualifyNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单不合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'ONoQualifyNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OCheckOutRemark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原送检单检验日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'OCheckOutDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NCheckOutResult'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NQualifyNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新送检单不合格数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_ReInspect', @level2type=N'COLUMN',@level2name=N'NNoQualifyNum'
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


USE [XDWMS]
GO

/****** Object:  Table [dbo].[WMS_InvDetail]    Script Date: 2018/11/25 18:44:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WMS_InvDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartCode] [nvarchar](50) NOT NULL,
	[QTY] [nchar](10) NOT NULL,
	[InvCode] [nvarchar](50) NULL,
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出入库明细ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'物料编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'PartCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'QTY'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库房编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'InvCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'BillId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据来源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'SourceBill'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'OperateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出入库类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'Type'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WMS_InvDetail', @level2type=N'COLUMN',@level2name=N'OperateMan'
GO














