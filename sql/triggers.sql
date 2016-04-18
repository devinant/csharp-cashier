--  Inserts a new transaction when balance is updated on a account
CREATE TRIGGER tUpdateAccountLog
ON Account
AFTER UPDATE
AS
BEGIN
  -- Trigger only if the balance is updated
  IF (UPDATE(balance))
  BEGIN
	SET NOCOUNT ON;

    DECLARE @id INT
    DECLARE @amount MONEY

    BEGIN TRANSACTION
	    SELECT @id = i.id, @amount = (i.balance - d.balance) FROM inserted i INNER JOIN deleted d ON i.id = d.id;
      INSERT INTO AccountLog VALUES(@id, GETDATE(), @amount);
    COMMIT
  END
END

-- DROP Helpers
-- DROP TRIGGER tUpdateAccountLog
