------------------------------------------------------------------------------------------------------------------------
--Create Database for each of the three Peers
------------------------------------------------------------------------------------------------------------------------
USE master
GO

IF EXISTS (SELECT [name] FROM [master].[sys].[databases] 
			   WHERE [name] = N'Collaboration_Peer1')
	BEGIN                       
		DROP DATABASE Collaboration_Peer1
	END

CREATE DATABASE Collaboration_Peer1
GO
ALTER DATABASE Collaboration_Peer1 SET ALLOW_SNAPSHOT_ISOLATION ON
GO

IF EXISTS (SELECT [name] FROM [master].[sys].[databases] 
			   WHERE [name] = N'Collaboration_Peer2')
	BEGIN
		DROP DATABASE Collaboration_Peer2
	END

CREATE DATABASE Collaboration_Peer2
GO
ALTER DATABASE Collaboration_Peer2 SET ALLOW_SNAPSHOT_ISOLATION ON
GO

IF EXISTS (SELECT [name] FROM [master].[sys].[databases] 
			   WHERE [name] = N'Collaboration_Peer3')
	BEGIN
		DROP DATABASE Collaboration_Peer3
	END

CREATE DATABASE Collaboration_Peer3
GO
ALTER DATABASE Collaboration_Peer3 SET ALLOW_SNAPSHOT_ISOLATION ON
GO
