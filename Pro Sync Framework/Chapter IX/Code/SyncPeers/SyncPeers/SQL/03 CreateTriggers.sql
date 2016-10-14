---------------------------------------------------------------------------------------------------------------------
--Create Insert/update/Delete triggers for storing metadata in Metadata tracking tables.
--This script needs to be run for each peer
--Scr 02
----------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER Account_InsertTrigger ON Account FOR INSERT
AS    
	INSERT INTO Account_Tracking(AccountId, sync_update_peer_key, 
                                sync_update_peer_timestamp, sync_create_peer_key, 
                                sync_create_peer_timestamp) 
	SELECT AccountId, 0, @@DBTS + 1, 0, @@DBTS + 1
	FROM inserted		


GO
CREATE TRIGGER Account_UpdateTrigger ON Account FOR UPDATE
AS    
    UPDATE a    
	SET sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS + 1,		
	    last_change_datetime = GETDATE()
	FROM Account_Tracking a JOIN inserted i ON a.AccountId = i.AccountId     	


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