USE [XDWMS]
GO

/****** Object:  Table [dbo].[SysSequence]    Script Date: 2018/11/18 16:17:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SysSequence](
	[ID] [int] NOT NULL,
	[SN] [varchar](50) NULL,
	[TabName] [varchar](50) NOT NULL,
	[FirstType] [int] NOT NULL,
	[FirstRule] [nvarchar](100) NULL,
	[FirstLength] [int] NULL,
	[SecondType] [int] NULL,
	[SecondRule] [nvarchar](100) NULL,
	[SecondLength] [int] NULL,
	[ThirdType] [int] NULL,
	[ThirdRule] [nvarchar](100) NULL,
	[ThirdLength] [int] NULL,
	[FourType] [int] NULL,
	[FourRule] [nvarchar](100) NULL,
	[FourLength] [int] NULL,
	[JoinChar] [varchar](10) NULL,
	[Sample] [nvarchar](100) NULL,
	[CurrentValue] [nvarchar](100) NULL,
	[Remark] [nvarchar](200) NULL,
 CONSTRAINT [PK_SysSequence] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [XDWMS]
GO

/****** Object:  Table [dbo].[SysTNum]    Script Date: 2018/11/18 16:18:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SysTNum](
	[ID] [int] NOT NULL,
	[Num] [int] NOT NULL,
	[MinNum] [int] NOT NULL,
	[MaxNum] [int] NOT NULL,
	[Day] [varchar](20) NULL,
	[TabName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SysTNum] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

