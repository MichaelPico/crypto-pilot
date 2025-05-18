CREATE TABLE [crypto_pilot].[users] (
    [id] INT NOT NULL IDENTITY(1, 1),
    [name] NVARCHAR(100) NOT NULL,
    [email] NVARCHAR(100) NOT NULL UNIQUE,
    [phone_number] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_users] PRIMARY KEY ([id])
);
GO

CREATE TABLE [pilot].[cryptocurrencies] (
    [id] INT NOT NULL IDENTITY(1, 1),
    [name] NVARCHAR(100) NOT NULL,
    [current_price] FLOAT NOT NULL,
    CONSTRAINT [PK_cryptocurrencies] PRIMARY KEY ([id])
);
GO

CREATE TABLE [pilot].[alerts] (
    [id] INT NOT NULL IDENTITY(1, 1),
    [user_id] INT NOT NULL,
    [cryptocurrency_id] INT NOT NULL,
    [target_price] FLOAT NOT NULL,
    [notified] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_alerts] PRIMARY KEY ([id]),
    CONSTRAINT [FK_alerts_users] FOREIGN KEY ([user_id]) REFERENCES [crypto].[users]([id]),
    CONSTRAINT [FK_alerts_cryptocurrencies] FOREIGN KEY ([cryptocurrency_id]) REFERENCES [crypto].[cryptocurrencies]([id])
);
GO
