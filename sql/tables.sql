--- Account
CREATE TABLE Account (
	id INT IDENTITY(1, 1) NOT NULL,
	balance MONEY DEFAULT(0) NOT NULL,
	name VARCHAR(20) NOT NULL

	CONSTRAINT pk_Account PRIMARY KEY(id)
);

--- AccountLog
CREATE TABLE AccountLog (
	id INT IDENTITY(1, 1) NOT NULL,
	account_id INT NOT NULL,
	dateOfTransaction DATE,
	amount MONEY DEFAULT(0) NOT NULL

	CONSTRAINT fk_Account FOREIGN KEY(account_id) REFERENCES Account(id) ON DELETE CASCADE,
	CONSTRAINT pk_AccountLog PRIMARY KEY(id)
);

CREATE TABLE AccountHolder
(
	id INT IDENTITY(1,1) NOT NULL,
	fName VARCHAR(20) NOT NULL,
	lName VARCHAR(20) NOT NULL,
	pinCode INT NOT NULL,
	street VARCHAR(50) NOT NULL,
	city VARCHAR(50) NOT NULL,
	birthDate DATE NOT NULL
	
	CONSTRAINT pk_User PRIMARY KEY(id)
)
CREATE TABLE Interest
(
	dateOfCalculation DATE NOT NULL,
	calculatedInterest MONEY NOT NULL,
	account_id INT NOT NULL
	
	CONSTRAINT pk_Interest PRIMARY KEY(dateOfCalculation, account_id)
	CONSTRAINT fk_InterestAccount FOREIGN KEY(account_id)
	REFERENCES Account(id) ON DELETE CASCADE
)

CREATE TABLE AccountOwnership
(
	accountHolder_id INT,
	account_id INT
	
	CONSTRAINT fkAccountOwnershipHolder FOREIGN KEY(accountHolder_id)
	REFERENCES AccountHolder(id) ON DELETE CASCADE,
	CONSTRAINT fkAccountOwnershipAccount FOREIGN KEY(account_id)
	REFERENCES Account(id) ON DELETE CASCADE,
	
	CONSTRAINT pk_AccountOwnership PRIMARY KEY(accountHolder_id, account_id)
)

-- DROP Helpers
-- DROP TABLE Account;
-- DROP TABLE AccountLog;
-- DROP TABLE AccountOwnership
-- DROP TABLE AccountHolder
-- DROP TABLE Interest


-- DELETE Helpers
-- DELETE FROM Account;
-- DELETE FROM AccountLog;
-- DELETE FROM AccountOwnership
-- DELETE FROM AccountHolder
-- DELETE FROM Interest
