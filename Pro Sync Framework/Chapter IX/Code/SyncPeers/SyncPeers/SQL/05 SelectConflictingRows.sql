---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for selecting conflicting rows from the data and metadata table
--This script needs to be run for each peer
--Scr 05
----------------------------------------------------------------------------------------------------------------------
GO
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