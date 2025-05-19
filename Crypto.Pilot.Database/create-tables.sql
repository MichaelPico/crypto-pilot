IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'crypto_pilot')
    EXEC('CREATE SCHEMA crypto_pilot');

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'users' AND s.name = 'crypto_pilot')
BEGIN
    CREATE TABLE [crypto_pilot].[users] (
        [id] INT NOT NULL IDENTITY(1, 1),
        [name] NVARCHAR(100) NOT NULL,
        [email] NVARCHAR(100) NOT NULL UNIQUE,
        [phone_number] NVARCHAR(20) NOT NULL,
        CONSTRAINT [PK_users] PRIMARY KEY ([id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'cryptocurrencies' AND s.name = 'crypto_pilot')
BEGIN
    CREATE TABLE [crypto_pilot].[cryptocurrencies] (
        [id] INT NOT NULL IDENTITY(1, 1),
        [name] NVARCHAR(100) NOT NULL,
        [current_price] FLOAT NOT NULL,
        CONSTRAINT [PK_cryptocurrencies] PRIMARY KEY ([id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'alerts' AND s.name = 'crypto_pilot')
BEGIN
    CREATE TABLE [crypto_pilot].[alerts] (
        [id] INT NOT NULL IDENTITY(1, 1),
        [user_id] INT NOT NULL,
        [cryptocurrency_id] INT NOT NULL,
        [target_price] FLOAT NOT NULL,
        [notified] BIT NOT NULL DEFAULT 0,
        [over_the_price] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_alerts] PRIMARY KEY ([id]),
        CONSTRAINT [FK_alerts_users] FOREIGN KEY ([user_id]) REFERENCES [crypto_pilot].[users]([id]),
        CONSTRAINT [FK_alerts_cryptocurrencies] FOREIGN KEY ([cryptocurrency_id]) REFERENCES [crypto_pilot].[cryptocurrencies]([id])
    );
END
GO
