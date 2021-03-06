
/****** Create Database ReplicaA  ******/
CREATE DATABASE [ReplicaA]
GO

/****** Create Customer Table in ReplicaA ******/
USE [ReplicaA]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
DROP TABLE [dbo].[Customer]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Designation] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Age] [int] NULL,
	[DateModified] [datetime] NOT NULL CONSTRAINT [DF_Customer_DateModified]  DEFAULT (getdate()),
	[DateCreated] [datetime] NOT NULL CONSTRAINT [DF_Customer_DateCreated]  DEFAULT (getdate())
) ON [PRIMARY]
Go
/****** Create Database ReplicaB  ******/
CREATE DATABASE [ReplicaB]
GO
/****** Create Customer Table in ReplicaB ******/
USE [ReplicaB]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
DROP TABLE [dbo].[Customer]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Designation] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Age] [int] NULL,
	[DateModified] [datetime] NOT NULL CONSTRAINT [DF_Customer_DateModified]  DEFAULT (getdate()),
	[DateCreated] [datetime] NOT NULL CONSTRAINT [DF_Customer_DateCreated]  DEFAULT (getdate())
) ON [PRIMARY]
Go
/****** Create Database ReplicaC  ******/
CREATE DATABASE [ReplicaC]
GO

/****** Create Customer Table in ReplicaA ******/
USE [ReplicaC]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
DROP TABLE [dbo].[Customer]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Designation] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Age] [int] NULL,
	[DateModified] [datetime] NOT NULL CONSTRAINT [DF_Customer_DateModified]  DEFAULT (getdate()),
	[DateCreated] [datetime] NOT NULL CONSTRAINT [DF_Customer_DateCreated]  DEFAULT (getdate())
) ON [PRIMARY]
Go