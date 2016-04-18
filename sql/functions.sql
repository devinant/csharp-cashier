-- Checks if a login was successful. Returns 1 on true, 0 on false
CREATE FUNCTION fLogin
(
  @accountHolderId INT,
  @pinCode INT
)
RETURNS INT
AS
BEGIN
  DECLARE @correctPin AS INT

  SELECT @correctPin=(SELECT AccountHolder.pinCode FROM AccountHolder
    WHERE AccountHolder.id=@AccountHolderId)

  IF (@pinCode=@correctPin)
  BEGIN
    RETURN 1
  END

  RETURN 0
END

-- Returns table with user's accounts
-- Example usage
-- SELECT * FROM fGetUserAccounts(2)
CREATE FUNCTION fGetUserAccounts
(
  @accountHolderId  INT
)
RETURNS @UserAccounts TABLE
(
  AccountId   INT,
  AccountName   VARCHAR(40),
  AccountBalance  MONEY
)
AS
BEGIN
  INSERT @UserAccounts
    SELECT Account.id, Account.name, Account.balance FROM
    Account INNER JOIN AccountOwnership ON Account.id=AccountOwnership.account_id
    WHERE AccountOwnership.accountHolder_id=@accountHolderId
    ORDER BY Account.id
  RETURN
END


