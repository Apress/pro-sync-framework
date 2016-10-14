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