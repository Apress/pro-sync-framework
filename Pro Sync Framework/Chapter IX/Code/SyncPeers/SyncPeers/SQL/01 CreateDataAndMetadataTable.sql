-------------------------------------------------------------------------------------------------------------------------
--Create Data and Metadata table
--This script needs to be run for each peer
--Scr 01
-------------------------------------------------------------------------------------------------------------------------
GO
--Create Data Table[Account]
CREATE TABLE Account(
	AccountId uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
	[Name] nvarchar(100) NOT NULL)
GO

--Create Metadata tables..
--Create Scope table
CREATE TABLE AccountScope(    	
    scope_id uniqueidentifier DEFAULT NEWID(), 	
    scope_name nvarchar(100) NULL,
    scope_sync_knowledge varbinary(max) NULL,
	scope_tombstone_cleanup_knowledge varbinary(max) NULL,
	scope_timestamp timestamp)

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

--Insert AccountScope in Scope table
INSERT INTO AccountScope(scope_name) VALUES ('AccountScope')
GO

--Create metadata table
-- Create metadata tracking tables for each base table in Peers
--This is an example of Decouled tracking
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