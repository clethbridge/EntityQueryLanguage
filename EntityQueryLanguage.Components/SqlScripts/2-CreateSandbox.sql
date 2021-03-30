CREATE SCHEMA [shop]

	CREATE TABLE [shop].[Customers] (
		 [Id] INT IDENTITY
		,[Name] VARCHAR(128) NOT NULL
		,[CreatedOn] DATETIME DEFAULT GETDATE()
		PRIMARY KEY (Id)
	)

	CREATE TABLE [shop].[Products] (
		 [Id] INT IDENTITY
		,[Name] VARCHAR(128) NOT NULL
		,[Summary] VARCHAR(256)
		,[Price] DECIMAL(14,2) NOT NULL
		,[ProductType] INT NOT NULL
		PRIMARY KEY (Id)
	)

	CREATE TABLE [shop].[Orders] (
		 [Id] INT IDENTITY
		,[CreatedOn] DATETIME DEFAULT GETDATE()
		,[CustomerId] INT FOREIGN KEY REFERENCES [shop].[Customers]([Id])
		,[ProductId] INT FOREIGN KEY REFERENCES [shop].[Products]([Id])
		,[Quantity] INT NOT NULL
		PRIMARY KEY (Id)
	)
GO

/*Customers Term Mapping*/
EXEC EQL.AddEntityKey 'ek-0001', 'shop', 'Customers'
EXEC EQL.CanInsertEntity 'shop', 'Customers'
EXEC EQL.CanUpdateEntity 'shop', 'Customers'
EXEC EQL.CanDeleteEntity 'shop', 'Customers'
EXEC EQL.AddTermKey 'ek-0001|pk', 'shop', 'Customers', 'Id'
EXEC EQL.AddTermKey 't-0001', 'shop', 'Customers', 'Name'
EXEC EQL.AddTermKey 't-0002', 'shop', 'Customers', 'CreatedOn'

/*Products Term Mapping*/
EXEC EQL.AddEntityKey 'ek-0002', 'shop', 'Products'
EXEC EQL.CanInsertEntity 'shop', 'Products'
EXEC EQL.CanUpdateEntity 'shop', 'Products'
EXEC EQL.CanDeleteEntity 'shop', 'Products'
EXEC EQL.AddTermKey 'ek-0002|pk', 'shop', 'Products', 'Id'
EXEC EQL.AddTermKey 't-0003', 'shop', 'Products', 'Name'
EXEC EQL.AddTermKey 't-0004', 'shop', 'Products', 'Summary'
EXEC EQL.AddTermKey 't-0005', 'shop', 'Products', 'Price'
EXEC EQL.AddTermKey 't-0006', 'shop', 'Products', 'ProductType'

/*Orders Term Mapping*/
EXEC EQL.AddEntityKey 'ek-0003', 'shop', 'Orders'
EXEC EQL.CanInsertEntity 'shop', 'Orders'
EXEC EQL.CanUpdateEntity 'shop', 'Orders'
EXEC EQL.CanDeleteEntity 'shop', 'Orders'
EXEC EQL.AddTermKey 'ek-0003|pk', 'shop', 'Orders', 'Id'
EXEC EQL.AddTermKey 't-0007', 'shop', 'Orders', 'CreatedOn'
EXEC EQL.AddTermKey 'ek-0001|pk', 'shop', 'Orders', 'CustomerId'
EXEC EQL.AddTermKey 'ek-0002|pk', 'shop', 'Orders', 'ProductId'
EXEC EQL.AddTermKey 't-0008', 'shop', 'Orders', 'Quantity'

/*Define 'ProductType'*/
INSERT INTO EQL.Lookups (TermKey, Code, Value, DisplayOrder) VALUES 
 ('t-0006', 1, 'Bed', 1)
,('t-0006', 2, 'Bathroom', 2)
,('t-0006', 3, 'Beyond', 3)