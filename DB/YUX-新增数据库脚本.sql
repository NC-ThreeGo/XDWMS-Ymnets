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


