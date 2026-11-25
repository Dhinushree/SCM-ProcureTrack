IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [Items] (
        [ItemId] int NOT NULL IDENTITY,
        [ItemCode] nvarchar(30) NOT NULL,
        [ItemName] nvarchar(150) NOT NULL,
        [Category] nvarchar(100) NULL,
        [UnitOfMeasure] nvarchar(20) NOT NULL,
        [CurrentStock] decimal(18,3) NOT NULL,
        [ReorderLevel] decimal(18,3) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Items] PRIMARY KEY ([ItemId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [Vendors] (
        [VendorId] int NOT NULL IDENTITY,
        [VendorCode] nvarchar(20) NOT NULL,
        [CompanyName] nvarchar(150) NOT NULL,
        [ContactPerson] nvarchar(100) NULL,
        [Email] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [TaxGSTId] nvarchar(20) NULL,
        [PaymentTerms] nvarchar(50) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Vendors] PRIMARY KEY ([VendorId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [PurchaseOrders] (
        [PurchaseOrderId] int NOT NULL IDENTITY,
        [PONumber] nvarchar(30) NOT NULL,
        [VendorId] int NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [ExpectedDeliveryDate] datetime2 NOT NULL,
        [Remarks] nvarchar(500) NULL,
        [Status] nvarchar(30) NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [GrandTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY ([PurchaseOrderId]),
        CONSTRAINT [FK_PurchaseOrders_Vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [Vendors] ([VendorId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [GoodsReceiptNotes] (
        [GoodsReceiptNoteId] int NOT NULL IDENTITY,
        [GRNNumber] nvarchar(30) NOT NULL,
        [PurchaseOrderId] int NOT NULL,
        [ReceiptDate] datetime2 NOT NULL,
        [Remarks] nvarchar(500) NULL,
        [Status] nvarchar(30) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_GoodsReceiptNotes] PRIMARY KEY ([GoodsReceiptNoteId]),
        CONSTRAINT [FK_GoodsReceiptNotes_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([PurchaseOrderId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [PurchaseOrderDetails] (
        [PurchaseOrderDetailId] int NOT NULL IDENTITY,
        [PurchaseOrderId] int NOT NULL,
        [ItemId] int NOT NULL,
        [OrderedQuantity] decimal(18,3) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [TaxPercentage] decimal(5,2) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PurchaseOrderDetails] PRIMARY KEY ([PurchaseOrderDetailId]),
        CONSTRAINT [FK_PurchaseOrderDetails_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([ItemId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([PurchaseOrderId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [GoodsReceiptDetails] (
        [GoodsReceiptDetailId] int NOT NULL IDENTITY,
        [GoodsReceiptNoteId] int NOT NULL,
        [PurchaseOrderDetailId] int NOT NULL,
        [ReceivedQuantity] decimal(18,3) NOT NULL,
        [AcceptedQuantity] decimal(18,3) NOT NULL,
        [RejectedQuantity] decimal(18,3) NOT NULL,
        [InspectionRemarks] nvarchar(500) NULL,
        CONSTRAINT [PK_GoodsReceiptDetails] PRIMARY KEY ([GoodsReceiptDetailId]),
        CONSTRAINT [FK_GoodsReceiptDetails_GoodsReceiptNotes_GoodsReceiptNoteId] FOREIGN KEY ([GoodsReceiptNoteId]) REFERENCES [GoodsReceiptNotes] ([GoodsReceiptNoteId]),
        CONSTRAINT [FK_GoodsReceiptDetails_PurchaseOrderDetails_PurchaseOrderDetailId] FOREIGN KEY ([PurchaseOrderDetailId]) REFERENCES [PurchaseOrderDetails] ([PurchaseOrderDetailId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE TABLE [InventoryLedgers] (
        [InventoryLedgerId] int NOT NULL IDENTITY,
        [ItemId] int NOT NULL,
        [GoodsReceiptDetailId] int NULL,
        [TransactionType] nvarchar(20) NOT NULL,
        [Quantity] decimal(18,3) NOT NULL,
        [BalanceAfter] decimal(18,3) NOT NULL,
        [ReferenceNumber] nvarchar(50) NULL,
        [TransactionDate] datetime2 NOT NULL,
        [Remarks] nvarchar(500) NULL,
        CONSTRAINT [PK_InventoryLedgers] PRIMARY KEY ([InventoryLedgerId]),
        CONSTRAINT [FK_InventoryLedgers_GoodsReceiptDetails_GoodsReceiptDetailId] FOREIGN KEY ([GoodsReceiptDetailId]) REFERENCES [GoodsReceiptDetails] ([GoodsReceiptDetailId]),
        CONSTRAINT [FK_InventoryLedgers_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([ItemId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GoodsReceiptDetails_GoodsReceiptNoteId] ON [GoodsReceiptDetails] ([GoodsReceiptNoteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GoodsReceiptDetails_PurchaseOrderDetailId] ON [GoodsReceiptDetails] ([PurchaseOrderDetailId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GoodsReceiptNotes_PurchaseOrderId] ON [GoodsReceiptNotes] ([PurchaseOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InventoryLedgers_GoodsReceiptDetailId] ON [InventoryLedgers] ([GoodsReceiptDetailId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InventoryLedgers_ItemId] ON [InventoryLedgers] ([ItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderDetails_ItemId] ON [PurchaseOrderDetails] ([ItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderDetails_PurchaseOrderId] ON [PurchaseOrderDetails] ([PurchaseOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrders_VendorId] ON [PurchaseOrders] ([VendorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123185038_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261123185038_InitialCreate', N'11.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261124121306_AddIdentityTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261124121306_AddIdentityTables', N'11.0.0');
END;

COMMIT;
GO

