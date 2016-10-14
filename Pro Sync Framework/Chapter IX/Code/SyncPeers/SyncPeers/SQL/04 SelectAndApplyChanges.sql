
---------------------------------------------------------------------------------------------------------------------
--This script creates store procedures for
--1. Selecting changes using metadata information
--2.Apply Incremental Changes[Insert/update/delete] to data tables
--3.Apply Incremental Changes[Insert/update/delete] to Metadata tables
--This script needs to be run for each peer
--Scr 03
----------------------------------------------------------------------------------------------------------------------

-- Selecting changes using metadata information
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

--Apply Incremental Changes[Insert/update/delete] to data tables
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

--Apply Incremental Changes[Insert/update/delete] to Metadata tables
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
