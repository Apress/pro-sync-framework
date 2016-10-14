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
Use Collaboration_Peer1

GO

CREATE TABLE Account(
	AccountId uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
	[Name] nvarchar(100) NOT NULL)

-------------------------------------------------------------------------------------------------------------------------
--Create Data and Metadata table
--This script needs to be run for each peer
--Scr 01
-------------------------------------------------------------------------------------------------------------------------


CREATE TABLE AccountScope(    	
    scope_id uniqueidentifier DEFAULT NEWID(), 	
    scope_name nvarchar(100) NULL,
    scope_sync_knowledge varbinary(max) NULL,
	scope_tombstone_cleanup_knowledge varbinary(max) NULL,
	scope_timestamp timestamp)

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

INSERT INTO AccountScope(scope_name) VALUES ('AccountScope')
GO
------------------------------------
-- Create tracking tables for each base table.
--
CREATE TABLE Account_Tracking(
    AccountId uniqueidentifier NOT NULL PRIMARY KEY,          
    sync_row_is_tombstone int DEFAULT 0,
    sync_row_timestamp timestamp, 
    sync_update_peer_key int DEFAULT 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int DEFAULT 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime DEFAULT GETDATE())

GO
---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for inserting test data in the account table.It also resets the data back to orginal state
--This script needs to be run for each peer
--Scr 02
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_ResetData

AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	DELETE FROM Account_Tracking
	DELETE FROM Account

	--INSERT INTO Account.
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000001','Account1')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000002','Account2')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000003','Account3')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000004','Account4')
	
GO
Exec  ('sp_ResetData')
GO


---------------------------------------------------------------------------------------------------------------------
--Create Insert/update/Delete triggers for storing metadata in Metadata tracking tables.
--This script needs to be run for each peer
--Scr 03
----------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER Account_InsertTrigger ON Account FOR INSERT
AS    
	INSERT INTO Account_Tracking(AccountId, sync_update_peer_key, 
                                sync_update_peer_timestamp, sync_create_peer_key, 
                                sync_create_peer_timestamp) 
	SELECT AccountId, 0, @@DBTS + 1, 0, @@DBTS + 1
	FROM inserted		

-- Update triggers
GO
CREATE TRIGGER Account_UpdateTrigger ON Account FOR UPDATE
AS    
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,		
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN inserted i ON a.AccountId = i.AccountId     	

-- Delete triggers
GO
CREATE TRIGGER Account_DeleteTrigger ON Account FOR DELETE
AS
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,
		sync_row_is_tombstone = 1,
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN deleted d ON a.AccountId = d.AccountId  
GO
---------------------------------------------------------------------------------------------------------------------
--This script creates store procedures for
--1. Selecting changes using metadata information
--2.Apply Incremental Changes[Insert/update/delete] to data tables
--3.Apply Incremental Changes[Insert/update/delete] to Metadata tables
--This script needs to be run for each peer
--Scr 04
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectChanges (				
		@sync_min_timestamp bigint,		
		@sync_metadata_only int,
		@sync_initialize int)
AS  	
	SELECT  at.AccountId, 
			a.Name,	                
			at.sync_row_is_tombstone, 
			at.sync_row_timestamp, 	                       
			at.sync_update_peer_key, 
			at.sync_update_peer_timestamp, 
			at.sync_create_peer_key, 
			at.sync_create_peer_timestamp 
	FROM Account a RIGHT JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE at.sync_row_timestamp > @sync_min_timestamp		
	ORDER BY at.AccountId ASC

GO


CREATE PROCEDURE sp_Account_ApplyInsert (						
        @AccountId uniqueidentifier,
		@Name nvarchar(100),
		@sync_row_count int OUT)        
AS

    IF NOT EXISTS (SELECT AccountId FROM Account_Tracking WHERE AccountId = @AccountId)
	    INSERT INTO Account (AccountId, Name) 
	    VALUES (@AccountId, @Name)
	    SET @sync_row_count = @@rowcount

GO

CREATE PROCEDURE sp_Account_InsertMetadata (		
		@AccountId uniqueidentifier,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp bigint,
		@sync_row_is_tombstone int,		
		@sync_row_count int OUT)        
AS	
	
	INSERT INTO Account_Tracking (AccountId, sync_update_peer_key, sync_update_peer_timestamp, 
                                   sync_create_peer_key, sync_create_peer_timestamp, sync_row_is_tombstone)
	VALUES (@AccountId, @sync_update_peer_key, @sync_update_peer_timestamp, 
            @sync_create_peer_key, @sync_create_peer_timestamp, @sync_row_is_tombstone)        			
	SET @sync_row_count = @@rowcount           
GO	


CREATE PROCEDURE sp_Account_ApplyUpdate (									
        @AccountId uniqueidentifier,
		@Name nvarchar(100),
		@sync_min_timestamp bigint , 								
		@sync_row_count int OUT,
		@sync_force_write int)        
AS		
	UPDATE a
	SET a.Name = @Name   
	FROM Account a JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE ((at.sync_row_timestamp <= @sync_min_timestamp) OR @sync_force_write = 1)
		AND at.AccountId = @AccountId  
	SET @sync_row_count = @@rowcount
GO


CREATE PROCEDURE sp_Account_UpdateMetadata (
		@AccountId uniqueidentifier,
		@sync_create_peer_key int,
		@sync_create_peer_timestamp bigint,					
		@sync_update_peer_key int,
		@sync_update_peer_timestamp timestamp,						
		@sync_row_timestamp timestamp,
		@sync_check_concurrency int,
		@sync_row_count int OUT)        
AS			
	UPDATE Account_Tracking SET
		sync_create_peer_key = @sync_create_peer_key, 
		sync_create_peer_timestamp =  @sync_create_peer_timestamp,
		sync_update_peer_key = @sync_update_peer_key, 
		sync_update_peer_timestamp =  @sync_update_peer_timestamp 
	WHERE AccountId = @AccountId AND 
		(@sync_check_concurrency = 0 OR sync_row_timestamp = @sync_row_timestamp)
	SET @sync_row_count = @@rowcount

GO

CREATE PROCEDURE sp_Account_ApplyDelete(
	@AccountId uniqueidentifier ,	
	@sync_min_timestamp bigint , 	     	
	@sync_row_count int OUT)	 
AS  
	DELETE a
	FROM Account a JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE at.sync_row_timestamp <= @sync_min_timestamp         
		AND at.AccountId = @AccountId            
	SET @sync_row_count = @@rowcount              

GO

CREATE PROCEDURE sp_Account_DeleteMetadata(
    @AccountId uniqueidentifier,			
	@sync_row_timestamp timestamp,	
	@sync_check_concurrency int,	
	@sync_row_count int OUT) 	
AS    
	DELETE at
	FROM Account_Tracking at 
	WHERE at.AccountId = @AccountId 
		AND (@sync_check_concurrency = 0 OR at.sync_row_timestamp = @sync_row_timestamp)
	SET @sync_row_count = @@rowcount           	

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting conflicting rows from the data and metadata table
--This script needs to be run for each peer
--Scr 05
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectRow
        @AccountId uniqueidentifier
AS
   SELECT  at.AccountId, 
           a.Name, 	    	       
		   at.sync_row_timestamp, 
	       at.sync_row_is_tombstone,
	       at.sync_update_peer_key, 
	       at.sync_update_peer_timestamp, 
	       at.sync_create_peer_key, 
	       at.sync_create_peer_timestamp 
	FROM Account a RIGHT JOIN Account_Tracking at ON a.AccountId = at.AccountId 
	WHERE at.AccountId = @AccountId 

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting the rows from the metadata table which can be cleaned up
--This script needs to be run for each peer
--Scr 06
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectMetadata     
	@metadata_aging_in_hours int
AS
	IF @metadata_aging_in_hours = -1
		BEGIN
			SELECT AccountId,
				   sync_row_timestamp, 	       
				   sync_update_peer_key, 
				   sync_update_peer_timestamp, 
				   sync_create_peer_key, 
				   sync_create_peer_timestamp 
			FROM Account_Tracking
			WHERE sync_row_is_tombstone = 1
		END
	
	ELSE
		BEGIN
			SELECT AccountId,
				   sync_row_timestamp, 	       
				   sync_update_peer_key, 
				   sync_update_peer_timestamp, 
				   sync_create_peer_key, 
				   sync_create_peer_timestamp 
			FROM Account_Tracking
			WHERE sync_row_is_tombstone = 1 AND
			DATEDIFF(hh, last_change_datetime, GETDATE()) > @metadata_aging_in_hours
		END
GO


Use Collaboration_Peer2
GO

CREATE TABLE Account(
	AccountId uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
	[Name] nvarchar(100) NOT NULL)

-------------------------------------------------------------------------------------------------------------------------
--Create Data and Metadata table
--This script needs to be run for each peer
--Scr 01
-------------------------------------------------------------------------------------------------------------------------


CREATE TABLE AccountScope(    	
    scope_id uniqueidentifier DEFAULT NEWID(), 	
    scope_name nvarchar(100) NULL,
    scope_sync_knowledge varbinary(max) NULL,
	scope_tombstone_cleanup_knowledge varbinary(max) NULL,
	scope_timestamp timestamp)

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

INSERT INTO AccountScope(scope_name) VALUES ('AccountScope')
GO
------------------------------------
-- Create tracking tables for each base table.
--
CREATE TABLE Account_Tracking(
    AccountId uniqueidentifier NOT NULL PRIMARY KEY,          
    sync_row_is_tombstone int DEFAULT 0,
    sync_row_timestamp timestamp, 
    sync_update_peer_key int DEFAULT 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int DEFAULT 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime DEFAULT GETDATE())

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for inserting test data in the account table.It also resets the data back to orginal state
--This script needs to be run for each peer
--Scr 02
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_ResetData

AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	DELETE FROM Account_Tracking
	DELETE FROM Account

	--INSERT INTO Account.
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000001','Account1')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000002','Account2')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000003','Account3')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000004','Account4')
	
	
GO
Exec  ('sp_ResetData')
GO
---------------------------------------------------------------------------------------------------------------------
--Create Insert/update/Delete triggers for storing metadata in Metadata tracking tables.
--This script needs to be run for each peer
--Scr 03
----------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER Account_InsertTrigger ON Account FOR INSERT
AS    
	INSERT INTO Account_Tracking(AccountId, sync_update_peer_key, 
                                sync_update_peer_timestamp, sync_create_peer_key, 
                                sync_create_peer_timestamp) 
	SELECT AccountId, 0, @@DBTS + 1, 0, @@DBTS + 1
	FROM inserted		

-- Update triggers
GO
CREATE TRIGGER Account_UpdateTrigger ON Account FOR UPDATE
AS    
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,		
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN inserted i ON a.AccountId = i.AccountId     	

-- Delete triggers
GO
CREATE TRIGGER Account_DeleteTrigger ON Account FOR DELETE
AS
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,
		sync_row_is_tombstone = 1,
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN deleted d ON a.AccountId = d.AccountId  
GO
---------------------------------------------------------------------------------------------------------------------
--This script creates store procedures for
--1. Selecting changes using metadata information
--2.Apply Incremental Changes[Insert/update/delete] to data tables
--3.Apply Incremental Changes[Insert/update/delete] to Metadata tables
--This script needs to be run for each peer
--Scr 04
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectChanges (				
		@sync_min_timestamp bigint,		
		@sync_metadata_only int,
		@sync_initialize int)
AS  	
	SELECT  at.AccountId, 
			a.Name,	                
			at.sync_row_is_tombstone, 
			at.sync_row_timestamp, 	                       
			at.sync_update_peer_key, 
			at.sync_update_peer_timestamp, 
			at.sync_create_peer_key, 
			at.sync_create_peer_timestamp 
	FROM Account a RIGHT JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE at.sync_row_timestamp > @sync_min_timestamp		
	ORDER BY at.AccountId ASC

GO


CREATE PROCEDURE sp_Account_ApplyInsert (						
        @AccountId uniqueidentifier,
		@Name nvarchar(100),
		@sync_row_count int OUT)        
AS

    IF NOT EXISTS (SELECT AccountId FROM Account_Tracking WHERE AccountId = @AccountId)
	    INSERT INTO Account (AccountId, Name) 
	    VALUES (@AccountId, @Name)
	    SET @sync_row_count = @@rowcount

GO

CREATE PROCEDURE sp_Account_InsertMetadata (		
		@AccountId uniqueidentifier,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp bigint,
		@sync_row_is_tombstone int,		
		@sync_row_count int OUT)        
AS	
	
	INSERT INTO Account_Tracking (AccountId, sync_update_peer_key, sync_update_peer_timestamp, 
                                   sync_create_peer_key, sync_create_peer_timestamp, sync_row_is_tombstone)
	VALUES (@AccountId, @sync_update_peer_key, @sync_update_peer_timestamp, 
            @sync_create_peer_key, @sync_create_peer_timestamp, @sync_row_is_tombstone)        			
	SET @sync_row_count = @@rowcount           
GO	


CREATE PROCEDURE sp_Account_ApplyUpdate (									
        @AccountId uniqueidentifier,
		@Name nvarchar(100),
		@sync_min_timestamp bigint , 								
		@sync_row_count int OUT,
		@sync_force_write int)        
AS		
	UPDATE a
	SET a.Name = @Name   
	FROM Account a JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE ((at.sync_row_timestamp <= @sync_min_timestamp) OR @sync_force_write = 1)
		AND at.AccountId = @AccountId  
	SET @sync_row_count = @@rowcount
GO


CREATE PROCEDURE sp_Account_UpdateMetadata (
		@AccountId uniqueidentifier,
		@sync_create_peer_key int,
		@sync_create_peer_timestamp bigint,					
		@sync_update_peer_key int,
		@sync_update_peer_timestamp timestamp,						
		@sync_row_timestamp timestamp,
		@sync_check_concurrency int,
		@sync_row_count int OUT)        
AS			
	UPDATE Account_Tracking SET
		sync_create_peer_key = @sync_create_peer_key, 
		sync_create_peer_timestamp =  @sync_create_peer_timestamp,
		sync_update_peer_key = @sync_update_peer_key, 
		sync_update_peer_timestamp =  @sync_update_peer_timestamp 
	WHERE AccountId = @AccountId AND 
		(@sync_check_concurrency = 0 OR sync_row_timestamp = @sync_row_timestamp)
	SET @sync_row_count = @@rowcount

GO

CREATE PROCEDURE sp_Account_ApplyDelete(
	@AccountId uniqueidentifier ,	
	@sync_min_timestamp bigint , 	     	
	@sync_row_count int OUT)	 
AS  
	DELETE a
	FROM Account a JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE at.sync_row_timestamp <= @sync_min_timestamp         
		AND at.AccountId = @AccountId            
	SET @sync_row_count = @@rowcount              

GO

CREATE PROCEDURE sp_Account_DeleteMetadata(
    @AccountId uniqueidentifier,			
	@sync_row_timestamp timestamp,	
	@sync_check_concurrency int,	
	@sync_row_count int OUT) 	
AS    
	DELETE at
	FROM Account_Tracking at 
	WHERE at.AccountId = @AccountId 
		AND (@sync_check_concurrency = 0 OR at.sync_row_timestamp = @sync_row_timestamp)
	SET @sync_row_count = @@rowcount           	

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting conflicting rows from the data and metadata table
--This script needs to be run for each peer
--Scr 05
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectRow
        @AccountId uniqueidentifier
AS
   SELECT  at.AccountId, 
           a.Name, 	    	       
		   at.sync_row_timestamp, 
	       at.sync_row_is_tombstone,
	       at.sync_update_peer_key, 
	       at.sync_update_peer_timestamp, 
	       at.sync_create_peer_key, 
	       at.sync_create_peer_timestamp 
	FROM Account a RIGHT JOIN Account_Tracking at ON a.AccountId = at.AccountId 
	WHERE at.AccountId = @AccountId 

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting the rows from the metadata table which can be cleaned up
--This script needs to be run for each peer
--Scr 06
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectMetadata     
	@metadata_aging_in_hours int
AS
	IF @metadata_aging_in_hours = -1
		BEGIN
			SELECT AccountId,
				   sync_row_timestamp, 	       
				   sync_update_peer_key, 
				   sync_update_peer_timestamp, 
				   sync_create_peer_key, 
				   sync_create_peer_timestamp 
			FROM Account_Tracking
			WHERE sync_row_is_tombstone = 1
		END
	
	ELSE
		BEGIN
			SELECT AccountId,
				   sync_row_timestamp, 	       
				   sync_update_peer_key, 
				   sync_update_peer_timestamp, 
				   sync_create_peer_key, 
				   sync_create_peer_timestamp 
			FROM Account_Tracking
			WHERE sync_row_is_tombstone = 1 AND
			DATEDIFF(hh, last_change_datetime, GETDATE()) > @metadata_aging_in_hours
		END
GO
Use Collaboration_Peer3

GO

CREATE TABLE Account(
	AccountId uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
	[Name] nvarchar(100) NOT NULL)

-------------------------------------------------------------------------------------------------------------------------
--Create Data and Metadata table
--This script needs to be run for each peer
--Scr 01
-------------------------------------------------------------------------------------------------------------------------


CREATE TABLE AccountScope(    	
    scope_id uniqueidentifier DEFAULT NEWID(), 	
    scope_name nvarchar(100) NULL,
    scope_sync_knowledge varbinary(max) NULL,
	scope_tombstone_cleanup_knowledge varbinary(max) NULL,
	scope_timestamp timestamp)

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

INSERT INTO AccountScope(scope_name) VALUES ('AccountScope')
GO
------------------------------------
-- Create tracking tables for each base table.
--
CREATE TABLE Account_Tracking(
    AccountId uniqueidentifier NOT NULL PRIMARY KEY,          
    sync_row_is_tombstone int DEFAULT 0,
    sync_row_timestamp timestamp, 
    sync_update_peer_key int DEFAULT 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int DEFAULT 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime DEFAULT GETDATE())

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for inserting test data in the account table.It also resets the data back to orginal state
--This script needs to be run for each peer
--Scr 02
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_ResetData

AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	DELETE FROM Account_Tracking
	DELETE FROM Account

	--INSERT INTO Account.
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000001','Account1')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000002','Account2')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000003','Account3')
	INSERT INTO Account(AccountId,Name) VALUES ('00000000-0000-0000-0000-000000000004','Account4')
	
	
GO
Exec  ('sp_ResetData')
GO
---------------------------------------------------------------------------------------------------------------------
--Create Insert/update/Delete triggers for storing metadata in Metadata tracking tables.
--This script needs to be run for each peer
--Scr 03
----------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER Account_InsertTrigger ON Account FOR INSERT
AS    
	INSERT INTO Account_Tracking(AccountId, sync_update_peer_key, 
                                sync_update_peer_timestamp, sync_create_peer_key, 
                                sync_create_peer_timestamp) 
	SELECT AccountId, 0, @@DBTS + 1, 0, @@DBTS + 1
	FROM inserted		

-- Update triggers
GO
CREATE TRIGGER Account_UpdateTrigger ON Account FOR UPDATE
AS    
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,		
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN inserted i ON a.AccountId = i.AccountId     	

-- Delete triggers
GO
CREATE TRIGGER Account_DeleteTrigger ON Account FOR DELETE
AS
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,
		sync_row_is_tombstone = 1,
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN deleted d ON a.AccountId = d.AccountId  
GO
---------------------------------------------------------------------------------------------------------------------
--This script creates store procedures for
--1. Selecting changes using metadata information
--2.Apply Incremental Changes[Insert/update/delete] to data tables
--3.Apply Incremental Changes[Insert/update/delete] to Metadata tables
--This script needs to be run for each peer
--Scr 04
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectChanges (				
		@sync_min_timestamp bigint,		
		@sync_metadata_only int,
		@sync_initialize int)
AS  	
	SELECT  at.AccountId, 
			a.Name,	                
			at.sync_row_is_tombstone, 
			at.sync_row_timestamp, 	                       
			at.sync_update_peer_key, 
			at.sync_update_peer_timestamp, 
			at.sync_create_peer_key, 
			at.sync_create_peer_timestamp 
	FROM Account a RIGHT JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE at.sync_row_timestamp > @sync_min_timestamp		
	ORDER BY at.AccountId ASC

GO


CREATE PROCEDURE sp_Account_ApplyInsert (						
        @AccountId uniqueidentifier,
		@Name nvarchar(100),
		@sync_row_count int OUT)        
AS

    IF NOT EXISTS (SELECT AccountId FROM Account_Tracking WHERE AccountId = @AccountId)
	    INSERT INTO Account (AccountId, Name) 
	    VALUES (@AccountId, @Name)
	    SET @sync_row_count = @@rowcount

GO

CREATE PROCEDURE sp_Account_InsertMetadata (		
		@AccountId uniqueidentifier,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp bigint,
		@sync_row_is_tombstone int,		
		@sync_row_count int OUT)        
AS	
	
	INSERT INTO Account_Tracking (AccountId, sync_update_peer_key, sync_update_peer_timestamp, 
                                   sync_create_peer_key, sync_create_peer_timestamp, sync_row_is_tombstone)
	VALUES (@AccountId, @sync_update_peer_key, @sync_update_peer_timestamp, 
            @sync_create_peer_key, @sync_create_peer_timestamp, @sync_row_is_tombstone)        			
	SET @sync_row_count = @@rowcount           
GO	


CREATE PROCEDURE sp_Account_ApplyUpdate (									
        @AccountId uniqueidentifier,
		@Name nvarchar(100),
		@sync_min_timestamp bigint , 								
		@sync_row_count int OUT,
		@sync_force_write int)        
AS		
	UPDATE a
	SET a.Name = @Name   
	FROM Account a JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE ((at.sync_row_timestamp <= @sync_min_timestamp) OR @sync_force_write = 1)
		AND at.AccountId = @AccountId  
	SET @sync_row_count = @@rowcount
GO


CREATE PROCEDURE sp_Account_UpdateMetadata (
		@AccountId uniqueidentifier,
		@sync_create_peer_key int,
		@sync_create_peer_timestamp bigint,					
		@sync_update_peer_key int,
		@sync_update_peer_timestamp timestamp,						
		@sync_row_timestamp timestamp,
		@sync_check_concurrency int,
		@sync_row_count int OUT)        
AS			
	UPDATE Account_Tracking SET
		sync_create_peer_key = @sync_create_peer_key, 
		sync_create_peer_timestamp =  @sync_create_peer_timestamp,
		sync_update_peer_key = @sync_update_peer_key, 
		sync_update_peer_timestamp =  @sync_update_peer_timestamp 
	WHERE AccountId = @AccountId AND 
		(@sync_check_concurrency = 0 OR sync_row_timestamp = @sync_row_timestamp)
	SET @sync_row_count = @@rowcount

GO

CREATE PROCEDURE sp_Account_ApplyDelete(
	@AccountId uniqueidentifier ,	
	@sync_min_timestamp bigint , 	     	
	@sync_row_count int OUT)	 
AS  
	DELETE a
	FROM Account a JOIN Account_Tracking at ON a.AccountId = at.AccountId
	WHERE at.sync_row_timestamp <= @sync_min_timestamp         
		AND at.AccountId = @AccountId            
	SET @sync_row_count = @@rowcount              

GO

CREATE PROCEDURE sp_Account_DeleteMetadata(
    @AccountId uniqueidentifier,			
	@sync_row_timestamp timestamp,	
	@sync_check_concurrency int,	
	@sync_row_count int OUT) 	
AS    
	DELETE at
	FROM Account_Tracking at 
	WHERE at.AccountId = @AccountId 
		AND (@sync_check_concurrency = 0 OR at.sync_row_timestamp = @sync_row_timestamp)
	SET @sync_row_count = @@rowcount           	

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting conflicting rows from the data and metadata table
--This script needs to be run for each peer
--Scr 05
----------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE sp_Account_SelectRow
        @AccountId uniqueidentifier
AS
   SELECT  at.AccountId, 
           a.Name, 	    	       
		   at.sync_row_timestamp, 
	       at.sync_row_is_tombstone,
	       at.sync_update_peer_key, 
	       at.sync_update_peer_timestamp, 
	       at.sync_create_peer_key, 
	       at.sync_create_peer_timestamp 
	FROM Account a RIGHT JOIN Account_Tracking at ON a.AccountId = at.AccountId 
	WHERE at.AccountId = @AccountId 

GO

---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting the rows from the metadata table which can be cleaned up
--This script needs to be run for each peer
--Scr 06
----------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE sp_Account_SelectMetadata     
	@metadata_aging_in_hours int
AS
	IF @metadata_aging_in_hours = -1
		BEGIN
			SELECT AccountId,
				   sync_row_timestamp, 	       
				   sync_update_peer_key, 
				   sync_update_peer_timestamp, 
				   sync_create_peer_key, 
				   sync_create_peer_timestamp 
			FROM Account_Tracking
			WHERE sync_row_is_tombstone = 1
		END
	
	ELSE
		BEGIN
			SELECT AccountId,
				   sync_row_timestamp, 	       
				   sync_update_peer_key, 
				   sync_update_peer_timestamp, 
				   sync_create_peer_key, 
				   sync_create_peer_timestamp 
			FROM Account_Tracking
			WHERE sync_row_is_tombstone = 1 AND
			DATEDIFF(hh, last_change_datetime, GETDATE()) > @metadata_aging_in_hours
		END
GO
