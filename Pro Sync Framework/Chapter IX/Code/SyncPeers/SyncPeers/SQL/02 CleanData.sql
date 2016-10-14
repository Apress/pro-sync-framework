---------------------------------------------------------------------------------------------------------------------
--This script creates store procedure for inserting test data in the account table.It also resets the data back to orginal state
--This script needs to be run for each peer
--Scr 04
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