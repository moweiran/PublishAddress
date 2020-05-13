create database Address;
USE [Address]
GO

/****** Object:  Table [dbo].[Base_Provinces]    Script Date: 2020-05-13 23:50:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Base_Provinces](
	[Id] [varchar](50) NULL,
	[ProvinceId] [varchar](50) NULL,
	[ProvinceName] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[IsCompleted] [bit] NULL
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Provinces', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省份标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Provinces', @level2type=N'COLUMN',@level2name=N'ProvinceId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省份名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Provinces', @level2type=N'COLUMN',@level2name=N'ProvinceName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省份基础表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Provinces'
GO


USE [Address]
GO

/****** Object:  Table [dbo].[Base_Cities]    Script Date: 2020-05-13 23:48:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Base_Cities](
	[Id] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[CityId] [varchar](50) NULL,
	[CityName] [varchar](100) NULL,
	[ProvinceId] [varchar](50) NULL,
	[Province_Id] [varchar](50) NULL,
	[ProvinceName] [varchar](100) NULL,
	[IsCompleted] [bit] NULL
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Cities', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Cities', @level2type=N'COLUMN',@level2name=N'CityId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Cities', @level2type=N'COLUMN',@level2name=N'CityName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省份标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Cities', @level2type=N'COLUMN',@level2name=N'ProvinceId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市基础表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Cities'
GO
USE [Address]
GO

/****** Object:  Table [dbo].[Base_Counties]    Script Date: 2020-05-13 23:50:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Base_Counties](
	[Id] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[CountyId] [varchar](50) NULL,
	[CountyName] [varchar](100) NULL,
	[City_Id] [varchar](50) NULL,
	[CityId] [varchar](50) NULL,
	[ProvinceId] [varchar](50) NULL,
	[Province_Id] [varchar](50) NULL,
	[IsHasChildren] [bit] NOT NULL,
	[CityName] [varchar](100) NULL,
	[ProvinceName] [varchar](100) NULL,
	[IsCompleted] [bit] NULL
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Counties', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'县区标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Counties', @level2type=N'COLUMN',@level2name=N'CountyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'县区名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Counties', @level2type=N'COLUMN',@level2name=N'CountyName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Counties', @level2type=N'COLUMN',@level2name=N'CityId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'县区基础表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Counties'
GO

USE [Address]
GO

/****** Object:  Table [dbo].[Base_Towns]    Script Date: 2020-05-13 23:50:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Base_Towns](
	[Id] [varchar](50) NULL,
	[TownId] [varchar](50) NULL,
	[CountyId] [varchar](50) NULL,
	[TownName] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[County_Id] [varchar](50) NULL,
	[City_Id] [varchar](50) NULL,
	[CityId] [varchar](50) NULL,
	[ProvinceId] [varchar](50) NULL,
	[Province_Id] [varchar](50) NULL,
	[ProvinceName] [varchar](50) NULL,
	[CityName] [varchar](50) NULL,
	[CountyName] [varchar](50) NULL,
	[IsCompleted] [bit] NULL
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Towns', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'乡镇标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Towns', @level2type=N'COLUMN',@level2name=N'TownId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'县区标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Towns', @level2type=N'COLUMN',@level2name=N'CountyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'乡镇名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Towns', @level2type=N'COLUMN',@level2name=N'TownName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'乡镇基础表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Towns'
GO

USE [Address]
GO

/****** Object:  Table [dbo].[Base_Villages]    Script Date: 2020-05-13 23:50:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Base_Villages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProvinceId] [decimal](18, 0) NOT NULL,
	[ProvinceName] [varchar](50) NULL,
	[CityId] [decimal](18, 0) NOT NULL,
	[CityName] [varchar](50) NULL,
	[CountyId] [decimal](18, 0) NOT NULL,
	[CountyName] [varchar](50) NULL,
	[TownId] [decimal](18, 0) NOT NULL,
	[TownName] [varchar](50) NULL,
	[VillageId] [decimal](18, 0) NOT NULL,
	[VillageName] [varchar](500) NULL,
 CONSTRAINT [PK_BASE_VILLAGES] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省份标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'ProvinceId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省份名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'ProvinceName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'CityId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'CityName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'县区标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'CountyId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'县区名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'CountyName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'乡镇标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'TownId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'乡镇名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'TownName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'村级标识符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'VillageId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'村级名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages', @level2type=N'COLUMN',@level2name=N'VillageName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'村级基础表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Base_Villages'
GO


