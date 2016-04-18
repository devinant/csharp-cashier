-- Creates a view that returns values required for ComboBoxPopulator.cs (Account Holder)
CREATE VIEW VCashierAccountHolderPopulator AS
	SELECT fName + ' ' + lName AS 'Value', id AS 'Key' FROM AccountHolder

-- Creates a view that returns values required for ComboBoxPopulator.cs (Account Linking)
-- Excludes the Master Account
CREATE VIEW VCashierAccountOwnershipPopulator AS
  SELECT id AS 'Key', name AS 'Value' FROM Account WHERE id != 1

-- Creates a view that returns 'id' and 'Name' from all AccountHolders
CREATE VIEW vAccountHolderIdAndName
AS
(
  SELECT id, fName + ' ' + lName AS name FROM AccountHolder
)

-- EXCEL --
--Views Accountholders and their accounts
CREATE VIEW vShowAccountHolderAccounts
AS
(
	SELECT AccountHolder.id AS UserId, AccountHolder.fName + ' ' + AccountHolder.lName AS 'User Name',
	Account.id AS AccountID, Account.name, Account.balance
	FROM Account
	INNER JOIN AccountOwnership
	ON Account.id=AccountOwnership.account_id
	INNER JOIN AccountHolder ON AccountOwnership.accountHolder_id = AccountHolder.id
)

--Views Master Account
CREATE VIEW vMasterAccount
AS
(
	SELECT id AS AccountId, Balance FROM
	Account WHERE id = 1
)

--Views accounts and their respective interest
CREATE VIEW vAccountInterest
AS
(
	SELECT Account.id, SUM(calculatedInterest) AS 'Calculated Interest' FROM 
	Account INNER JOIN Interest ON Account.id = Interest.account_id
	GROUP BY Account.id
)

-- SELECT Helpers
-- SELECT * FROM VCashierAccountHolderPopulator
-- SELECT * FROM VCashierAccountOwnershipPopulator
-- SELECT * FROM vAccountHolderIdAndName
-- SELECT * FROM vShowAccountHolderAccounts
-- SELECT * FROM vMasterAccount
-- SELECT * FROM vAccountInterest

-- DROP Helpers
-- DROP VIEW VCashierAccountHolderPopulator
-- DROP VIEW VCashierAccountOwnershipPopulator
-- DROP VIEW vAccountHolderIdAndName
-- DROP VIEW vShowAccountHolderAccounts
-- DROP VIEW vMasterAccount
-- DROP VIEW vAccountInterest
