-- Creates a account using a name
CREATE PROCEDURE spAddAccount
(
  @name VARCHAR(20),
  @holder_id INT,
  @account_id INT OUTPUT
) AS
BEGIN
  -- Create a new account with '0' balance
  INSERT INTO Account VALUES(0, @name)
  SELECT @account_id = SCOPE_IDENTITY()

  -- Declare the inserted value: it will never
  -- be used. If a account is newly created, it will never
  -- "clash" with this user. @inserted in this case is a dummy
  -- value since spLinkAccountHolderToAccount requires it
  DECLARE @inserted INT

  -- If the holder is set, link the accounts
  IF (@holder_id > 0)
    EXEC spLinkAccountHolderToAccount @holder_id, @account_id, @inserted

  RETURN
END

CREATE PROCEDURE spAddAccountHolder
(
  @first_name VARCHAR(20),
  @last_name VARCHAR(20),
  @pin_code INT,
  @street VARCHAR(50),
  @city VARCHAR(50),
  @birth_date DATE
) AS
BEGIN
  INSERT INTO AccountHolder VALUES(@first_name, @last_name, @pin_code, @street, @city, @birth_date);
END

-- Inserts a owner/user to a account.
CREATE PROCEDURE spLinkAccountHolderToAccount
(
  @holder_id INT,
  @account_id INT,
  @inserted INT OUTPUT
) AS
BEGIN
  -- Initially, set the @inserted to 1
  SET @inserted = 1

  -- Try to insert a row, if it fails set the @inserted variable to 0
  BEGIN TRY
    INSERT INTO AccountOwnership VALUES(@holder_id, @account_id)
  END TRY
  BEGIN CATCH
    SET @inserted = 0 -- It failed (keys already exist)
  END CATCH
END

--Calculates interest for every account
CREATE PROCEDURE spCalculateInterest
(
  @interestRate INT
)
AS
BEGIN
  INSERT INTO Interest(calculatedInterest, dateOfCalculation, account_id)
  SELECT calculatedInterest = (balance /365) * @interestRate, getDate(), id FROM Account;
  UPDATE Account SET balance = balance + ((balance /365) * @interestRate);
END


-- currently take a 3 % cut of the transfer amount (10$ to tranfer, 0.3$ got to account 1 and 9.7$ is transferred)
-- Transfers money from one account to another
CREATE PROCEDURE spTransferMoney
(
  @fromAccountNr INT,
  @toAccountNr INT,
  @amount FLOAT
)
AS
BEGIN
	DECLARE @transferFee FLOAT
	SET @transferFee = (@amount * 0.03)
	SET @amount = (@amount - @transferFee)

	BEGIN TRANSACTION
    UPDATE Account SET balance = (balance - ABS(@amount)) WHERE id = @fromAccountNr
    UPDATE Account SET balance = (balance + ABS(@amount)) WHERE id = @toAccountNr
	UPDATE Account SET balance = (balance + ABS(@transferFee)) WHERE id = 1

    IF (((SELECT balance FROM Account WHERE id = @fromAccountNr) >= 0) AND
		(@fromAccountNr != @toAccountNr))
    BEGIN
      COMMIT
    END
    ELSE
    BEGIN
      ROLLBACK
    END
END



-- Gets the names of other accountholder of an account
CREATE PROCEDURE spNamesOfOtherAccountHoldersOfAccount
(
  @AccountHolder INT,
  @Account INT
)
AS
BEGIN
  SELECT (AccountHolder.fName + ' ' + AccountHolder.lName) AS 'name' FROM AccountHolder
        INNER JOIN AccountOwnership ON AccountHolder.id = AccountOwnership.accountHolder_id WHERE
        AccountOwnership.account_id = @account AND AccountOwnership.accountHolder_id != @accountHolder ORDER BY name;
END

-- Withdraws money from an account
CREATE PROCEDURE spWithdrawMoney
(
  @accountId    INT,
  @amount       FLOAT,
  @success      INT OUTPUT
)
AS
BEGIN
  DECLARE @accountBalance AS MONEY
  SELECT @accountBalance=(SELECT Account.balance FROM Account WHERE Account.id = @accountId)

  IF (@accountBalance >= ABS(@amount))
  BEGIN
    BEGIN TRANSACTION
      UPDATE Account
      SET balance = balance - ABS(@amount) - (@amount*0.02)
      WHERE Account.id=@accountId
      
      UPDATE Account
	  SET balance = balance + (@amount*0.02)
	  WHERE Account.id = 1
	  
      SET @success = 1
    COMMIT
  END
  ELSE
  BEGIN
    SET @success = 0
  END
END


-- Inserts money to an account using the Account.id and amount and returns the result
CREATE PROCEDURE spDepositMoney
(
  @account_id INT,
  @amount MONEY,
  @result INT OUTPUT
)
AS
BEGIN
  SET @result = 0

  BEGIN TRANSACTION
    BEGIN TRY
      -- ABS always ensures positive value
      UPDATE Account SET balance = (balance + ABS(@amount)) WHERE id = @account_id
      COMMIT
      SET @result = 1
    END TRY
    BEGIN CATCH
      ROLLBACK
    END CATCH
END

-- DROP Helpers
-- DROP PROCEDURE spAddAccount
-- DROP PROCEDURE spAddAccountHolder
-- DROP PROCEDURE spLinkAccountHolderToAccount
-- DROP PROCEDURE spTransferMoney
-- DROP PROCEDURE spNamesOfOtherAccountHoldersOfAccount
-- DROP PROCEDURE spWithdrawMoney
-- DROP PROCEDURE spDepositMoney
