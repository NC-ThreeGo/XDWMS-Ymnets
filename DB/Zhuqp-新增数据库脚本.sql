/****** Object:  Table [dbo].[SysImportExcelLog]    Script Date: 2018/11/25 21:53:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SysImportExcelLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImportTime] [datetime] NOT NULL,
	[ImportTable] [varchar](50) NOT NULL,
	[ImportFileName] [varchar](50) NULL,
	[ImportFilePathUrl] [varchar](200) NULL,
	[ImportStatus] [varchar](10) NULL,
	[CreateBy] [varchar](50) NULL,
 CONSTRAINT [PK_WMS_ImportExcelLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'ImportTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入的表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'ImportTable'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入的文件名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'ImportFileName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入的文件Url' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'ImportFilePathUrl'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'ImportStatus'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysImportExcelLog', @level2type=N'COLUMN',@level2name=N'CreateBy'
GO





/****** Object:  Table [dbo].[SysParam]    Script Date: 2018/12/6 10:06:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SysParam](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeCode] [nvarchar](20) NULL,
	[TypeName] [nvarchar](50) NULL,
	[ParamCode] [nvarchar](50) NULL,
	[ParamName] [nvarchar](50) NULL,
	[Sort] [int] NULL,
	[Enable] [bit] NOT NULL,
	[CreatePerson] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[ModifyPerson] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
 CONSTRAINT [PK_SysParam] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SysParam] ADD  CONSTRAINT [DF_SysParam_Enable]  DEFAULT ((1)) FOR [Enable]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数类别编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'TypeCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数类别名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'TypeName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数值编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'ParamCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数值名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'ParamName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'Sort'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否启用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'Enable'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'CreatePerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'ModifyPerson'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SysParam', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO






