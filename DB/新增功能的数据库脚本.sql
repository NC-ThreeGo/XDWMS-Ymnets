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



USE [XDWMS]
GO

/****** Object:  StoredProcedure [dbo].[P_Sys_SwiftNum]    Script Date: 2018/11/18 16:20:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[P_Sys_SwiftNum](@Day varchar(20),@TabName varchar(20),@Num int OUTPUT)
AS
BEGIN
	--开始事务
	BEGIN TRANSACTION
		
	--使用错误计数器
	DECLARE @ERRORSUM INT
	SET @ERRORSUM=0

	DECLARE @COUNT INT
	--判断是否是每日流水号
	IF(@Day='' OR @Day IS NULL)
	BEGIN
		--非每日流水号情况
		SET @COUNT=(SELECT COUNT(*) FROM [dbo].[TNum] WHERE TabName=@TabName)
		SET @ERRORSUM=@ERRORSUM+@@ERROR
		IF(@COUNT=0)
		BEGIN
			INSERT INTO [dbo].[TNum]([Num],[MinNum],[MaxNum],[Day],[TabName])VALUES(1,1,99999,@Day,@TabName)
			SET @ERRORSUM=@ERRORSUM+@@ERROR
		END
		SELECT TOP 1 @Num=[Num] FROM [dbo].[TNum] WHERE [TabName]=@TabName
		UPDATE [dbo].[TNum]  SET [Num]=[Num]+1 WHERE [TabName]=@TabName
		SET @ERRORSUM=@ERRORSUM+@@ERROR
	END
	ELSE
	BEGIN
		--每日流水号情况
		SET @COUNT=(SELECT COUNT(*) FROM [dbo].[TNum] WHERE TabName=@TabName AND [Day]=@Day)
		SET @ERRORSUM=@ERRORSUM+@@ERROR
		IF(@COUNT=0)
		BEGIN
			INSERT INTO [dbo].[TNum]([Num],[MinNum],[MaxNum],[Day],[TabName])VALUES(1,1,99999,@Day,@TabName)
			SET @ERRORSUM=@ERRORSUM+@@ERROR
		END
		SELECT TOP 1 @Num=[Num] FROM [dbo].[TNum] WHERE TabName=@TabName AND [Day]=@Day
		UPDATE [dbo].[TNum]  SET [Num]=[Num]+1 WHERE TabName=@TabName AND [Day]=@Day
		SET @ERRORSUM=@ERRORSUM+@@ERROR
	END

	IF @ERRORSUM<>0
	BEGIN
		--回滚事务
		ROLLBACK TRANSACTION
	END
	ELSE
	BEGIN
		--提交事务
		COMMIT TRANSACTION
	END
END


GO

