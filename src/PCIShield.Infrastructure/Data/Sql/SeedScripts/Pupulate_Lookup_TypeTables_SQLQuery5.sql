

USE PCIShield_Core_Db;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Declare common variables (mimicking your original script's setup)
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @BoundedContextId UNIQUEIDENTIFIER;
    DECLARE @CountryId UNIQUEIDENTIFIER;
    DECLARE @ContinentId UNIQUEIDENTIFIER;
    DECLARE @CurrencyId UNIQUEIDENTIFIER;
    DECLARE @SubcontinentId UNIQUEIDENTIFIER = NULL; -- Assuming NULL if not specifically used by these lookups

    -- Find a valid UserId (use the one from your script or select an active one)
    SET @UserId = '2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0'; -- Replace with a valid, non-deleted UserId from your ApplicationUser or User table
    IF NOT EXISTS (SELECT 1 FROM ApplicationUser WHERE ApplicationUserId = @UserId) -- Or your actual User table
    BEGIN
        -- Fallback if the hardcoded user doesn't exist, pick an admin or system user
        -- SELECT TOP 1 @UserId = UserId FROM Users WHERE IsAdmin = 1 AND IsActive = 1;
        -- For now, let's assume the hardcoded one is valid as per your previous script.
        PRINT 'Warning: Hardcoded @UserId ' + CAST(@UserId AS NVARCHAR(36)) + ' not verified against a user table in this setup script block.';
    END

    -- Get an existing BoundedContext ID - CRITICAL FOR MANY DEPENDENCIES
    SELECT TOP 1 @BoundedContextId = BoundedContextId FROM BoundedContext WHERE IsActive = 1;
    IF @BoundedContextId IS NULL
    BEGIN
        RAISERROR('No active BoundedContext found. This script requires an active BoundedContext to populate lookup tables.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    PRINT 'Using BoundedContextId: ' + CAST(@BoundedContextId AS NVARCHAR(36));

    -- Ensure essential Geo/Currency data exists for lookups that might need CountryId
    -- Continent (e.g., North America)
    SELECT TOP 1 @ContinentId = ContinentId FROM Continent WHERE ContinentName = 'North America';
    IF @ContinentId IS NULL
    BEGIN
        SET @ContinentId = NEWID();
        INSERT INTO Continent (ContinentId, ContinentName) VALUES (@ContinentId, 'North America');
        PRINT 'Created Continent: North America with ID ' + CAST(@ContinentId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Continent: North America with ID ' + CAST(@ContinentId AS NVARCHAR(36));
    END

    -- Currency (e.g., USD)
    SELECT TOP 1 @CurrencyId = CurrencyId FROM Currency WHERE CurrencyCodeISO = 'USD';
    IF @CurrencyId IS NULL
    BEGIN
        SET @CurrencyId = NEWID();
        INSERT INTO Currency (CurrencyId, CurrencyName, CurrencySymbol, CurrencyCodeISO, CurrencyIdISO, CurrencyDecimalPlaces, CurrencyDecimalSeparator, IsDeleted)
        VALUES (@CurrencyId, 'US Dollar', '$', 'USD', 840, 2, '.', 0); -- Assuming 840 for CurrencyIdISO and '.' for separator
        PRINT 'Created Currency: USD with ID ' + CAST(@CurrencyId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Currency: USD with ID ' + CAST(@CurrencyId AS NVARCHAR(36));
    END

    -- Country (e.g., United States) - needed for some Type tables
    SELECT TOP 1 @CountryId = CountryId FROM Country WHERE CountryCodeISO2 = 'US';
    IF @CountryId IS NULL
    BEGIN
        SET @CountryId = NEWID();
        INSERT INTO Country (CountryId, CountryName, CountryCodeISO2, CountryCodeISO3, CountryIdISO, CountryAreaCode, ContinentId, SubcontinentId, CurrencyId, IsActive)
        VALUES (@CountryId, 'United States', 'US', 'USA', 840, '1', @ContinentId, @SubcontinentId, @CurrencyId, 1);
        PRINT 'Created Country: United States with ID ' + CAST(@CountryId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Country: United States with ID ' + CAST(@CountryId AS NVARCHAR(36));
    END

    PRINT '--- Populating Lookup Tables ---';

    -- Table: AddressSystemType
    PRINT 'Populating AddressSystemType...';
    DECLARE @AddressSystemTypeId_PRI UNIQUEIDENTIFIER, @AddressSystemTypeId_BIL UNIQUEIDENTIFIER, @AddressSystemTypeId_DEL UNIQUEIDENTIFIER, @AddressSystemTypeId_SHIP UNIQUEIDENTIFIER, @AddressSystemTypeId_CORP UNIQUEIDENTIFIER;

    -- PRI (Primary)
    SELECT TOP 1 @AddressSystemTypeId_PRI = AddressSystemTypeId FROM AddressSystemType WHERE AddressSystemTypeCode = 'PRI' AND BoundedContextId = @BoundedContextId;
    IF @AddressSystemTypeId_PRI IS NULL
    BEGIN
        SET @AddressSystemTypeId_PRI = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeId_PRI, 'PRI', 'Primary Address', @BoundedContextId, 1);
        PRINT 'Inserted AddressSystemType: PRI';
    END ELSE PRINT 'AddressSystemType: PRI already exists.';

    -- BIL (Billing)
    SELECT TOP 1 @AddressSystemTypeId_BIL = AddressSystemTypeId FROM AddressSystemType WHERE AddressSystemTypeCode = 'BIL' AND BoundedContextId = @BoundedContextId;
    IF @AddressSystemTypeId_BIL IS NULL
    BEGIN
        SET @AddressSystemTypeId_BIL = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeId_BIL, 'BIL', 'Billing Address', @BoundedContextId, 1);
        PRINT 'Inserted AddressSystemType: BIL';
    END ELSE PRINT 'AddressSystemType: BIL already exists.';

    -- DEL (Delivery)
    SELECT TOP 1 @AddressSystemTypeId_DEL = AddressSystemTypeId FROM AddressSystemType WHERE AddressSystemTypeCode = 'DEL' AND BoundedContextId = @BoundedContextId;
    IF @AddressSystemTypeId_DEL IS NULL
    BEGIN
        SET @AddressSystemTypeId_DEL = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeId_DEL, 'DEL', 'Delivery Address', @BoundedContextId, 1);
        PRINT 'Inserted AddressSystemType: DEL';
    END ELSE PRINT 'AddressSystemType: DEL already exists.';

    -- SHIP (Shipping)
    SELECT TOP 1 @AddressSystemTypeId_SHIP = AddressSystemTypeId FROM AddressSystemType WHERE AddressSystemTypeCode = 'SHIP' AND BoundedContextId = @BoundedContextId;
    IF @AddressSystemTypeId_SHIP IS NULL
    BEGIN
        SET @AddressSystemTypeId_SHIP = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeId_SHIP, 'SHI', 'Shipping Address', @BoundedContextId, 1);
        PRINT 'Inserted AddressSystemType: SHIP';
    END ELSE PRINT 'AddressSystemType: SHIP already exists.';
    
    -- CORP (Corporate)
    SELECT TOP 1 @AddressSystemTypeId_CORP = AddressSystemTypeId FROM AddressSystemType WHERE AddressSystemTypeCode = 'CORP' AND BoundedContextId = @BoundedContextId;
    IF @AddressSystemTypeId_CORP IS NULL
    BEGIN
        SET @AddressSystemTypeId_CORP = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeId_CORP, 'COR', 'Corporate Headquarters', @BoundedContextId, 1);
        PRINT 'Inserted AddressSystemType: CORP';
    END ELSE PRINT 'AddressSystemType: CORP already exists.';


    -- Table: AttachmentType
    PRINT 'Populating AttachmentType...';
    DECLARE @AttachmentTypeId_INV UNIQUEIDENTIFIER, @AttachmentTypeId_PRODIMG UNIQUEIDENTIFIER, @AttachmentTypeId_CONTRACT UNIQUEIDENTIFIER;

    SELECT TOP 1 @AttachmentTypeId_INV = AttachmentTypeId FROM AttachmentType WHERE AttachmentTypeName = 'Invoice PDF';
    IF @AttachmentTypeId_INV IS NULL
    BEGIN
        SET @AttachmentTypeId_INV = NEWID();
        INSERT INTO AttachmentType (AttachmentTypeId, AttachmentTypeName, AttachmentTypeDescription)
        VALUES (@AttachmentTypeId_INV, 'Invoice PDF', 'PDF copy of the sales/purchase invoice');
        PRINT 'Inserted AttachmentType: Invoice PDF';
    END ELSE PRINT 'AttachmentType: Invoice PDF already exists.';

    SELECT TOP 1 @AttachmentTypeId_PRODIMG = AttachmentTypeId FROM AttachmentType WHERE AttachmentTypeName = 'Product Image';
    IF @AttachmentTypeId_PRODIMG IS NULL
    BEGIN
        SET @AttachmentTypeId_PRODIMG = NEWID();
        INSERT INTO AttachmentType (AttachmentTypeId, AttachmentTypeName, AttachmentTypeDescription)
        VALUES (@AttachmentTypeId_PRODIMG, 'Product Image', 'Image file of the product');
        PRINT 'Inserted AttachmentType: Product Image';
    END ELSE PRINT 'AttachmentType: Product Image already exists.';

    SELECT TOP 1 @AttachmentTypeId_CONTRACT = AttachmentTypeId FROM AttachmentType WHERE AttachmentTypeName = 'Signed Contract';
    IF @AttachmentTypeId_CONTRACT IS NULL
    BEGIN
        SET @AttachmentTypeId_CONTRACT = NEWID();
        INSERT INTO AttachmentType (AttachmentTypeId, AttachmentTypeName, AttachmentTypeDescription)
        VALUES (@AttachmentTypeId_CONTRACT, 'Signed Contract', 'Scanned copy of a signed contract');
        PRINT 'Inserted AttachmentType: Signed Contract';
    END ELSE PRINT 'AttachmentType: Signed Contract already exists.';


    -- Table: BankAccountSystemType
    PRINT 'Populating BankAccountSystemType...';
    DECLARE @BankAccountSystemTypeId_CHK UNIQUEIDENTIFIER, @BankAccountSystemTypeId_SAV UNIQUEIDENTIFIER, @BankAccountSystemTypeId_CC UNIQUEIDENTIFIER;

    SELECT TOP 1 @BankAccountSystemTypeId_CHK = BankAccountSystemTypeId FROM BankAccountSystemType WHERE BankAccountSystemTypeCode = 'CHK';
    IF @BankAccountSystemTypeId_CHK IS NULL
    BEGIN
        SET @BankAccountSystemTypeId_CHK = NEWID();
        INSERT INTO BankAccountSystemType (BankAccountSystemTypeId, BankAccountSystemTypeCode, BankAccountSystemTypeName, BankAccountSystemTypeVariant, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankAccountSystemTypeId_CHK, 'CHK', 'Checking Account', 'Per', 'Standard personal checking account', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankAccountSystemType: CHK';
    END ELSE PRINT 'BankAccountSystemType: CHK already exists.';

    SELECT TOP 1 @BankAccountSystemTypeId_SAV = BankAccountSystemTypeId FROM BankAccountSystemType WHERE BankAccountSystemTypeCode = 'SAV';
    IF @BankAccountSystemTypeId_SAV IS NULL
    BEGIN
        SET @BankAccountSystemTypeId_SAV = NEWID();
        INSERT INTO BankAccountSystemType (BankAccountSystemTypeId, BankAccountSystemTypeCode, BankAccountSystemTypeName, BankAccountSystemTypeVariant, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankAccountSystemTypeId_SAV, 'SAV', 'Savings Account', 'Bus', 'Business savings account for reserves', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankAccountSystemType: SAV';
    END ELSE PRINT 'BankAccountSystemType: SAV already exists.';
    
    SELECT TOP 1 @BankAccountSystemTypeId_CC = BankAccountSystemTypeId FROM BankAccountSystemType WHERE BankAccountSystemTypeCode = 'CCARD_PAY';
    IF @BankAccountSystemTypeId_CC IS NULL
    BEGIN
        SET @BankAccountSystemTypeId_CC = NEWID();
        INSERT INTO BankAccountSystemType (BankAccountSystemTypeId, BankAccountSystemTypeCode, BankAccountSystemTypeName, BankAccountSystemTypeVariant, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankAccountSystemTypeId_CC, 'CCARD_PAY', 'Credit Card Payable', 'Liab', 'Account for tracking credit card payables', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankAccountSystemType: CCARD_PAY';
    END ELSE PRINT 'BankAccountSystemType: CCARD_PAY already exists.';


    -- Table: BankTransactionSystemType
    PRINT 'Populating BankTransactionSystemType...';
    DECLARE @BankTransactionSystemTypeId_DEP UNIQUEIDENTIFIER, @BankTransactionSystemTypeId_WDL UNIQUEIDENTIFIER, @BankTransactionSystemTypeId_FEE UNIQUEIDENTIFIER, @BankTransactionSystemTypeId_XFER UNIQUEIDENTIFIER;

    SELECT TOP 1 @BankTransactionSystemTypeId_DEP = BankTransactionSystemTypeId FROM BankTransactionSystemType WHERE BankTransactionSystemTypeCode = 'DEP';
    IF @BankTransactionSystemTypeId_DEP IS NULL
    BEGIN
        SET @BankTransactionSystemTypeId_DEP = NEWID();
        INSERT INTO BankTransactionSystemType (BankTransactionSystemTypeId, BankTransactionSystemTypeCode, BankTransactionSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankTransactionSystemTypeId_DEP, 'DEP', 'Deposit', 'Funds deposited into the account', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankTransactionSystemType: DEP';
    END ELSE PRINT 'BankTransactionSystemType: DEP already exists.';

    SELECT TOP 1 @BankTransactionSystemTypeId_WDL = BankTransactionSystemTypeId FROM BankTransactionSystemType WHERE BankTransactionSystemTypeCode = 'WDL';
    IF @BankTransactionSystemTypeId_WDL IS NULL
    BEGIN
        SET @BankTransactionSystemTypeId_WDL = NEWID();
        INSERT INTO BankTransactionSystemType (BankTransactionSystemTypeId, BankTransactionSystemTypeCode, BankTransactionSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankTransactionSystemTypeId_WDL, 'WDL', 'Withdrawal', 'Funds withdrawn from the account', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankTransactionSystemType: WDL';
    END ELSE PRINT 'BankTransactionSystemType: WDL already exists.';

    SELECT TOP 1 @BankTransactionSystemTypeId_FEE = BankTransactionSystemTypeId FROM BankTransactionSystemType WHERE BankTransactionSystemTypeCode = 'FEE';
    IF @BankTransactionSystemTypeId_FEE IS NULL
    BEGIN
        SET @BankTransactionSystemTypeId_FEE = NEWID();
        INSERT INTO BankTransactionSystemType (BankTransactionSystemTypeId, BankTransactionSystemTypeCode, BankTransactionSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankTransactionSystemTypeId_FEE, 'FEE', 'Bank Fee', 'Fees charged by the bank', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankTransactionSystemType: FEE';
    END ELSE PRINT 'BankTransactionSystemType: FEE already exists.';

    SELECT TOP 1 @BankTransactionSystemTypeId_XFER = BankTransactionSystemTypeId FROM BankTransactionSystemType WHERE BankTransactionSystemTypeCode = 'XFER';
    IF @BankTransactionSystemTypeId_XFER IS NULL
    BEGIN
        SET @BankTransactionSystemTypeId_XFER = NEWID();
        INSERT INTO BankTransactionSystemType (BankTransactionSystemTypeId, BankTransactionSystemTypeCode, BankTransactionSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@BankTransactionSystemTypeId_XFER, 'XFER', 'Transfer', 'Funds transferred between accounts', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted BankTransactionSystemType: XFER';
    END ELSE PRINT 'BankTransactionSystemType: XFER already exists.';


    -- Table: ContactSystemType
    PRINT 'Populating ContactSystemType...';
    DECLARE @ContactSystemTypeId_PRIMARY UNIQUEIDENTIFIER, @ContactSystemTypeId_BILLING UNIQUEIDENTIFIER, @ContactSystemTypeId_TECH UNIQUEIDENTIFIER, @ContactSystemTypeId_REP UNIQUEIDENTIFIER;

    SELECT TOP 1 @ContactSystemTypeId_PRIMARY = ContactSystemTypeId FROM ContactSystemType WHERE ContactSystemTypeCode = 'PRIMARY_C' AND BoundedContextId = @BoundedContextId;
    IF @ContactSystemTypeId_PRIMARY IS NULL
    BEGIN
        SET @ContactSystemTypeId_PRIMARY = NEWID();
        INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@ContactSystemTypeId_PRIMARY, 'PRIMARY_C', 'Primary Contact', @BoundedContextId, 1, 0);
        PRINT 'Inserted ContactSystemType: PRIMARY_C';
    END ELSE PRINT 'ContactSystemType: PRIMARY_C already exists.';
    
    -- REPRESENT (Representative) - from your original script
    SELECT TOP 1 @ContactSystemTypeId_REP = ContactSystemTypeId FROM ContactSystemType WHERE ContactSystemTypeCode = 'REPRESENT' AND BoundedContextId = @BoundedContextId;
    IF @ContactSystemTypeId_REP IS NULL
    BEGIN
        SET @ContactSystemTypeId_REP = NEWID();
        INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@ContactSystemTypeId_REP, 'REPRESENT', 'Legal Representative', @BoundedContextId, 1, 0);
        PRINT 'Inserted ContactSystemType: REPRESENT';
    END ELSE PRINT 'ContactSystemType: REPRESENT already exists.';

    SELECT TOP 1 @ContactSystemTypeId_BILLING = ContactSystemTypeId FROM ContactSystemType WHERE ContactSystemTypeCode = 'BILLING_C' AND BoundedContextId = @BoundedContextId;
    IF @ContactSystemTypeId_BILLING IS NULL
    BEGIN
        SET @ContactSystemTypeId_BILLING = NEWID();
        INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@ContactSystemTypeId_BILLING, 'BILLING_C', 'Billing Contact', @BoundedContextId, 1, 0);
        PRINT 'Inserted ContactSystemType: BILLING_C';
    END ELSE PRINT 'ContactSystemType: BILLING_C already exists.';

    SELECT TOP 1 @ContactSystemTypeId_TECH = ContactSystemTypeId FROM ContactSystemType WHERE ContactSystemTypeCode = 'TECH_C' AND BoundedContextId = @BoundedContextId;
    IF @ContactSystemTypeId_TECH IS NULL
    BEGIN
        SET @ContactSystemTypeId_TECH = NEWID();
        INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@ContactSystemTypeId_TECH, 'TECH_C', 'Technical Contact', @BoundedContextId, 1, 0);
        PRINT 'Inserted ContactSystemType: TECH_C';
    END ELSE PRINT 'ContactSystemType: TECH_C already exists.';


    -- Table: CustomerUserType
    PRINT 'Populating CustomerUserType...';
    DECLARE @CustomerUserTypeId_STND UNIQUEIDENTIFIER, @CustomerUserTypeId_VIP UNIQUEIDENTIFIER, @CustomerUserTypeId_RESELLER UNIQUEIDENTIFIER, @CustomerUserTypeId_INTERNAL UNIQUEIDENTIFIER;

    -- STND (Standard) - from your original script
    SELECT TOP 1 @CustomerUserTypeId_STND = CustomerUserTypeId FROM CustomerUserType WHERE CustomerUserTypeCode = 'STND' AND BoundedContextId = @BoundedContextId;
    IF @CustomerUserTypeId_STND IS NULL
    BEGIN
        SET @CustomerUserTypeId_STND = NEWID();
        INSERT INTO CustomerUserType (CustomerUserTypeId, CustomerUserTypeCode, CustomerUserTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@CustomerUserTypeId_STND, 'STND', 'Standard Customer', @BoundedContextId, 1, 0);
        PRINT 'Inserted CustomerUserType: STND';
    END ELSE PRINT 'CustomerUserType: STND already exists.';

    SELECT TOP 1 @CustomerUserTypeId_VIP = CustomerUserTypeId FROM CustomerUserType WHERE CustomerUserTypeCode = 'VIP' AND BoundedContextId = @BoundedContextId;
    IF @CustomerUserTypeId_VIP IS NULL
    BEGIN
        SET @CustomerUserTypeId_VIP = NEWID();
        INSERT INTO CustomerUserType (CustomerUserTypeId, CustomerUserTypeCode, CustomerUserTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@CustomerUserTypeId_VIP, 'VIP', 'VIP Customer', @BoundedContextId, 1, 0);
        PRINT 'Inserted CustomerUserType: VIP';
    END ELSE PRINT 'CustomerUserType: VIP already exists.';

    SELECT TOP 1 @CustomerUserTypeId_RESELLER = CustomerUserTypeId FROM CustomerUserType WHERE CustomerUserTypeCode = 'RESELLER' AND BoundedContextId = @BoundedContextId;
    IF @CustomerUserTypeId_RESELLER IS NULL
    BEGIN
        SET @CustomerUserTypeId_RESELLER = NEWID();
        INSERT INTO CustomerUserType (CustomerUserTypeId, CustomerUserTypeCode, CustomerUserTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@CustomerUserTypeId_RESELLER, 'RESEL', 'Reseller/Partner', @BoundedContextId, 1, 0);
        PRINT 'Inserted CustomerUserType: RESELLER';
    END ELSE PRINT 'CustomerUserType: RESELLER already exists.';
    
    SELECT TOP 1 @CustomerUserTypeId_INTERNAL = CustomerUserTypeId FROM CustomerUserType WHERE CustomerUserTypeCode = 'INTERNAL' AND BoundedContextId = @BoundedContextId;
    IF @CustomerUserTypeId_INTERNAL IS NULL
    BEGIN
        SET @CustomerUserTypeId_INTERNAL = NEWID();
        INSERT INTO CustomerUserType (CustomerUserTypeId, CustomerUserTypeCode, CustomerUserTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@CustomerUserTypeId_INTERNAL, 'INTER', 'Internal / Inter-company', @BoundedContextId, 1, 0);
        PRINT 'Inserted CustomerUserType: INTERNAL';
    END ELSE PRINT 'CustomerUserType: INTERNAL already exists.';


    -- Table: DocumentIdentificationSystemType
    PRINT 'Populating DocumentIdentificationSystemType...';
    DECLARE @DocIdSysTypeId_NIT UNIQUEIDENTIFIER, @DocIdSysTypeId_NRC UNIQUEIDENTIFIER, @DocIdSysTypeId_PASSPORT UNIQUEIDENTIFIER, @DocIdSysTypeId_DRIVERLIC UNIQUEIDENTIFIER, @DocIdSysTypeId_REPDOC UNIQUEIDENTIFIER;

    -- NIT (Tax ID) - from your original script
    SELECT TOP 1 @DocIdSysTypeId_NIT = DocumentIdentificationSystemTypeId FROM DocumentIdentificationSystemType WHERE DocumentIdentificationSystemTypeCode = 'NIT' AND CountryId = @CountryId;
    IF @DocIdSysTypeId_NIT IS NULL
    BEGIN
        SET @DocIdSysTypeId_NIT = NEWID();
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, DocumentIdentificationSystemTypeName, DocumentIdentificationSystemTypeRegex, IsRegexRequired, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@DocIdSysTypeId_NIT, 'NIT', 'Tax Identification Number', '^\d{6}-\d{3}-\d{3}-\d{1}$', 0, @BoundedContextId, @CountryId, 1, 0); -- Example Regex for a format like 123456-123-123-1
        PRINT 'Inserted DocumentIdentificationSystemType: NIT';
    END ELSE PRINT 'DocumentIdentificationSystemType: NIT already exists.';

    -- NRC (Business Registry) - from your original script
    SELECT TOP 1 @DocIdSysTypeId_NRC = DocumentIdentificationSystemTypeId FROM DocumentIdentificationSystemType WHERE DocumentIdentificationSystemTypeCode = 'NRC' AND CountryId = @CountryId;
    IF @DocIdSysTypeId_NRC IS NULL
    BEGIN
        SET @DocIdSysTypeId_NRC = NEWID();
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, DocumentIdentificationSystemTypeName, DocumentIdentificationSystemTypeRegex, IsRegexRequired, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@DocIdSysTypeId_NRC, 'NRC', 'National Registry of Commerce', '^[A-Z0-9\-]+$', 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted DocumentIdentificationSystemType: NRC';
    END ELSE PRINT 'DocumentIdentificationSystemType: NRC already exists.';

    -- REP_DOC (Representative Document) - from your original script
    SELECT TOP 1 @DocIdSysTypeId_REPDOC = DocumentIdentificationSystemTypeId FROM DocumentIdentificationSystemType WHERE DocumentIdentificationSystemTypeCode = 'REP_DOC' AND CountryId = @CountryId;
    IF @DocIdSysTypeId_REPDOC IS NULL
    BEGIN
        SET @DocIdSysTypeId_REPDOC = NEWID();
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, DocumentIdentificationSystemTypeName, DocumentIdentificationSystemTypeRegex, IsRegexRequired, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@DocIdSysTypeId_REPDOC, 'REP_DOC', 'Representative Document ID', NULL, 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted DocumentIdentificationSystemType: REP_DOC';
    END ELSE PRINT 'DocumentIdentificationSystemType: REP_DOC already exists.';

    SELECT TOP 1 @DocIdSysTypeId_PASSPORT = DocumentIdentificationSystemTypeId FROM DocumentIdentificationSystemType WHERE DocumentIdentificationSystemTypeCode = 'PASSPORT'; -- CountryId can be NULL if it's a general type
    IF @DocIdSysTypeId_PASSPORT IS NULL
    BEGIN
        SET @DocIdSysTypeId_PASSPORT = NEWID();
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, DocumentIdentificationSystemTypeName, DocumentIdentificationSystemTypeRegex, IsRegexRequired, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@DocIdSysTypeId_PASSPORT, 'PASSPORT', 'Passport Number', '^[A-Z0-9]{6,20}$', 0, @BoundedContextId, NULL, 1, 0);
        PRINT 'Inserted DocumentIdentificationSystemType: PASSPORT';
    END ELSE PRINT 'DocumentIdentificationSystemType: PASSPORT already exists.';

    SELECT TOP 1 @DocIdSysTypeId_DRIVERLIC = DocumentIdentificationSystemTypeId FROM DocumentIdentificationSystemType WHERE DocumentIdentificationSystemTypeCode = 'DRIVER_LIC' AND CountryId = @CountryId;
    IF @DocIdSysTypeId_DRIVERLIC IS NULL
    BEGIN
        SET @DocIdSysTypeId_DRIVERLIC = NEWID();
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, DocumentIdentificationSystemTypeName, DocumentIdentificationSystemTypeRegex, IsRegexRequired, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@DocIdSysTypeId_DRIVERLIC, 'DRIVER_LIC', 'Driver''s License', NULL, 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted DocumentIdentificationSystemType: DRIVER_LIC';
    END ELSE PRINT 'DocumentIdentificationSystemType: DRIVER_LIC already exists.';


    -- Table: DocumentType
    PRINT 'Populating DocumentType...';
    DECLARE @DocumentTypeId_CONTRACT UNIQUEIDENTIFIER, @DocumentTypeId_IDPROOF UNIQUEIDENTIFIER, @DocumentTypeId_PO UNIQUEIDENTIFIER;

    SELECT TOP 1 @DocumentTypeId_CONTRACT = DocumentTypeId FROM DocumentType WHERE DocumentTypeName = 'Contract Document';
    IF @DocumentTypeId_CONTRACT IS NULL
    BEGIN
        SET @DocumentTypeId_CONTRACT = NEWID();
        INSERT INTO DocumentType (DocumentTypeId, DocumentTypeName, DocumentTypeDescription)
        VALUES (@DocumentTypeId_CONTRACT, 'Contract Document', 'Signed customer or supplier contract agreement');
        PRINT 'Inserted DocumentType: Contract Document';
    END ELSE PRINT 'DocumentType: Contract Document already exists.';

    SELECT TOP 1 @DocumentTypeId_IDPROOF = DocumentTypeId FROM DocumentType WHERE DocumentTypeName = 'Identity Proof';
    IF @DocumentTypeId_IDPROOF IS NULL
    BEGIN
        SET @DocumentTypeId_IDPROOF = NEWID();
        INSERT INTO DocumentType (DocumentTypeId, DocumentTypeName, DocumentTypeDescription)
        VALUES (@DocumentTypeId_IDPROOF, 'Identity Proof', 'Document used for proving identity (e.g., national ID card scan)');
        PRINT 'Inserted DocumentType: Identity Proof';
    END ELSE PRINT 'DocumentType: Identity Proof already exists.';
    
    SELECT TOP 1 @DocumentTypeId_PO = DocumentTypeId FROM DocumentType WHERE DocumentTypeName = 'Purchase Order Form';
    IF @DocumentTypeId_PO IS NULL
    BEGIN
        SET @DocumentTypeId_PO = NEWID();
        INSERT INTO DocumentType (DocumentTypeId, DocumentTypeName, DocumentTypeDescription)
        VALUES (@DocumentTypeId_PO, 'Purchase Order Form', 'Standard purchase order form document');
        PRINT 'Inserted DocumentType: Purchase Order Form';
    END ELSE PRINT 'DocumentType: Purchase Order Form already exists.';


    -- Table: EconomicActivitySystemType
    PRINT 'Populating EconomicActivitySystemType...';
    DECLARE @EconActTypeId_RETAIL UNIQUEIDENTIFIER, @EconActTypeId_WHOLESALE UNIQUEIDENTIFIER, @EconActTypeId_MANUF UNIQUEIDENTIFIER, @EconActTypeId_SVC UNIQUEIDENTIFIER;

    -- RETAIL - from your original script
    SELECT TOP 1 @EconActTypeId_RETAIL = EconomicActivitySystemTypeId FROM EconomicActivitySystemType WHERE EconomicActivitySystemTypeCode = 'RETAIL' AND CountryId = @CountryId;
    IF @EconActTypeId_RETAIL IS NULL
    BEGIN
        SET @EconActTypeId_RETAIL = NEWID();
        INSERT INTO EconomicActivitySystemType (EconomicActivitySystemTypeId, EconomicActivitySystemTypeCode, EconomicActivitySystemTypeName, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@EconActTypeId_RETAIL, 'RETAIL', 'Retail Trade', @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted EconomicActivitySystemType: RETAIL';
    END ELSE PRINT 'EconomicActivitySystemType: RETAIL already exists.';

    SELECT TOP 1 @EconActTypeId_WHOLESALE = EconomicActivitySystemTypeId FROM EconomicActivitySystemType WHERE EconomicActivitySystemTypeCode = 'WHOLESALE' AND CountryId = @CountryId;
    IF @EconActTypeId_WHOLESALE IS NULL
    BEGIN
        SET @EconActTypeId_WHOLESALE = NEWID();
        INSERT INTO EconomicActivitySystemType (EconomicActivitySystemTypeId, EconomicActivitySystemTypeCode, EconomicActivitySystemTypeName, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@EconActTypeId_WHOLESALE, 'WHOLESALE', 'Wholesale Trade', @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted EconomicActivitySystemType: WHOLESALE';
    END ELSE PRINT 'EconomicActivitySystemType: WHOLESALE already exists.';

    SELECT TOP 1 @EconActTypeId_MANUF = EconomicActivitySystemTypeId FROM EconomicActivitySystemType WHERE EconomicActivitySystemTypeCode = 'MANUF' AND CountryId = @CountryId;
    IF @EconActTypeId_MANUF IS NULL
    BEGIN
        SET @EconActTypeId_MANUF = NEWID();
        INSERT INTO EconomicActivitySystemType (EconomicActivitySystemTypeId, EconomicActivitySystemTypeCode, EconomicActivitySystemTypeName, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@EconActTypeId_MANUF, 'MANUF', 'Manufacturing', @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted EconomicActivitySystemType: MANUF';
    END ELSE PRINT 'EconomicActivitySystemType: MANUF already exists.';

    SELECT TOP 1 @EconActTypeId_SVC = EconomicActivitySystemTypeId FROM EconomicActivitySystemType WHERE EconomicActivitySystemTypeCode = 'SERVICES' AND CountryId = @CountryId;
    IF @EconActTypeId_SVC IS NULL
    BEGIN
        SET @EconActTypeId_SVC = NEWID();
        INSERT INTO EconomicActivitySystemType (EconomicActivitySystemTypeId, EconomicActivitySystemTypeCode, EconomicActivitySystemTypeName, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@EconActTypeId_SVC, 'SERVICES', 'Professional Services', @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted EconomicActivitySystemType: SERVICES';
    END ELSE PRINT 'EconomicActivitySystemType: SERVICES already exists.';


    -- Table: ElectronicDocumentAttributeSystemType
    PRINT 'Populating ElectronicDocumentAttributeSystemType...';
    DECLARE @EDASTId_InvNum UNIQUEIDENTIFIER, @EDASTId_AuthCode UNIQUEIDENTIFIER, @EDASTId_IssueDate UNIQUEIDENTIFIER;

    SELECT TOP 1 @EDASTId_InvNum = ElectronicDocumentAttributeSystemTypeId FROM ElectronicDocumentAttributeSystemType WHERE ElectronicDocumentAttributeSystemTypeCode = 'E_INV_NUMBER';
    IF @EDASTId_InvNum IS NULL
    BEGIN
        SET @EDASTId_InvNum = NEWID();
        INSERT INTO ElectronicDocumentAttributeSystemType (ElectronicDocumentAttributeSystemTypeId, ElectronicDocumentAttributeSystemTypeCode, ElectronicDocumentAttributeSystemTypeName, ElectronicDocumentAttributeSystemTypeDescription, ElectronicDocumentAttributeSystemTypeFormula, ElectronicDocumentAttributeSystemTypeJsonSchema, ElectronicDocumentAttributeDataType, ElectronicDocumentAttributeSystemTypeIsResponse, CountryId, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@EDASTId_InvNum, 'E_INV_NUMBER', 'E-Invoice Number', 'Official invoice number for electronic document.', NULL, '{"type": "string", "minLength": 1}', 'string', 0, @CountryId, @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ElectronicDocumentAttributeSystemType: E_INV_NUMBER';
    END ELSE PRINT 'ElectronicDocumentAttributeSystemType: E_INV_NUMBER already exists.';

    SELECT TOP 1 @EDASTId_AuthCode = ElectronicDocumentAttributeSystemTypeId FROM ElectronicDocumentAttributeSystemType WHERE ElectronicDocumentAttributeSystemTypeCode = 'E_AUTH_CODE';
    IF @EDASTId_AuthCode IS NULL
    BEGIN
        SET @EDASTId_AuthCode = NEWID();
        INSERT INTO ElectronicDocumentAttributeSystemType (ElectronicDocumentAttributeSystemTypeId, ElectronicDocumentAttributeSystemTypeCode, ElectronicDocumentAttributeSystemTypeName, ElectronicDocumentAttributeSystemTypeDescription, ElectronicDocumentAttributeSystemTypeFormula, ElectronicDocumentAttributeSystemTypeJsonSchema, ElectronicDocumentAttributeDataType, ElectronicDocumentAttributeSystemTypeIsResponse, CountryId, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@EDASTId_AuthCode, 'E_AUTH_CODE', 'E-Doc Authorization Code', 'Authorization code received from tax authority.', NULL, '{"type": "string"}', 'string', 1, @CountryId, @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ElectronicDocumentAttributeSystemType: E_AUTH_CODE';
    END ELSE PRINT 'ElectronicDocumentAttributeSystemType: E_AUTH_CODE already exists.';
    
    SELECT TOP 1 @EDASTId_IssueDate = ElectronicDocumentAttributeSystemTypeId FROM ElectronicDocumentAttributeSystemType WHERE ElectronicDocumentAttributeSystemTypeCode = 'E_ISSUE_DATE';
    IF @EDASTId_IssueDate IS NULL
    BEGIN
        SET @EDASTId_IssueDate = NEWID();
        INSERT INTO ElectronicDocumentAttributeSystemType (ElectronicDocumentAttributeSystemTypeId, ElectronicDocumentAttributeSystemTypeCode, ElectronicDocumentAttributeSystemTypeName, ElectronicDocumentAttributeSystemTypeDescription, ElectronicDocumentAttributeSystemTypeFormula, ElectronicDocumentAttributeSystemTypeJsonSchema, ElectronicDocumentAttributeDataType, ElectronicDocumentAttributeSystemTypeIsResponse, CountryId, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@EDASTId_IssueDate, 'E_ISSUE_DATE', 'E-Doc Issue Date', 'Date the electronic document was issued.', NULL, '{"type": "string", "format": "date"}', 'date', 0, @CountryId, @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ElectronicDocumentAttributeSystemType: E_ISSUE_DATE';
    END ELSE PRINT 'ElectronicDocumentAttributeSystemType: E_ISSUE_DATE already exists.';

    -- Table: InvoiceSystemType (Prerequisite for ElectronicDocumentTransmissionSystemType)
    PRINT 'Populating InvoiceSystemType (as prerequisite)...';
    DECLARE @InvoiceSystemTypeId_STDINV UNIQUEIDENTIFIER, @InvoiceSystemTypeId_CNOTE UNIQUEIDENTIFIER, @InvoiceSystemTypeId_DNOTE UNIQUEIDENTIFIER, @InvoiceSystemTypeId_PROFORMA UNIQUEIDENTIFIER;

    SELECT TOP 1 @InvoiceSystemTypeId_STDINV = InvoiceSystemTypeId FROM InvoiceSystemType WHERE InvoiceSystemTypeCode = 'STDINV';
    IF @InvoiceSystemTypeId_STDINV IS NULL
    BEGIN
        SET @InvoiceSystemTypeId_STDINV = NEWID();
        INSERT INTO InvoiceSystemType (InvoiceSystemTypeId, InvoiceSystemTypeCode, InvoiceSystemTypeName, InvoiceSystemTypeIsSale, InvoiceSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@InvoiceSystemTypeId_STDINV, 'STDINV', 'Standard Invoice', 1, 1, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted InvoiceSystemType: STDINV';
    END ELSE PRINT 'InvoiceSystemType: STDINV already exists.';

    SELECT TOP 1 @InvoiceSystemTypeId_CNOTE = InvoiceSystemTypeId FROM InvoiceSystemType WHERE InvoiceSystemTypeCode = 'CNOTE';
    IF @InvoiceSystemTypeId_CNOTE IS NULL
    BEGIN
        SET @InvoiceSystemTypeId_CNOTE = NEWID();
        INSERT INTO InvoiceSystemType (InvoiceSystemTypeId, InvoiceSystemTypeCode, InvoiceSystemTypeName, InvoiceSystemTypeIsSale, InvoiceSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@InvoiceSystemTypeId_CNOTE, 'NDC', 'Credit Note', 1, -1, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted InvoiceSystemType: NDC';
    END ELSE PRINT 'InvoiceSystemType: CNOTE already exists.';
    
    SELECT TOP 1 @InvoiceSystemTypeId_DNOTE = InvoiceSystemTypeId FROM InvoiceSystemType WHERE InvoiceSystemTypeCode = 'DNOTE';
    IF @InvoiceSystemTypeId_DNOTE IS NULL
    BEGIN
        SET @InvoiceSystemTypeId_DNOTE = NEWID();
        INSERT INTO InvoiceSystemType (InvoiceSystemTypeId, InvoiceSystemTypeCode, InvoiceSystemTypeName, InvoiceSystemTypeIsSale, InvoiceSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@InvoiceSystemTypeId_DNOTE, 'NDD', 'Debit Note', 1, 1, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted InvoiceSystemType: NDD';
    END ELSE PRINT 'InvoiceSystemType: DNOTE already exists.';

    SELECT TOP 1 @InvoiceSystemTypeId_PROFORMA = InvoiceSystemTypeId FROM InvoiceSystemType WHERE InvoiceSystemTypeCode = 'PROFORMA';
    IF @InvoiceSystemTypeId_PROFORMA IS NULL
    BEGIN
        SET @InvoiceSystemTypeId_PROFORMA = NEWID();
        INSERT INTO InvoiceSystemType (InvoiceSystemTypeId, InvoiceSystemTypeCode, InvoiceSystemTypeName, InvoiceSystemTypeIsSale, InvoiceSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@InvoiceSystemTypeId_PROFORMA, 'PROFORMA', 'Proforma Invoice', 1, 0, GETDATE(), @UserId, NULL, NULL, 1, 0); -- Sign 0 as it's not a fiscal doc usually
        PRINT 'Inserted InvoiceSystemType: PROFORMA';
    END ELSE PRINT 'InvoiceSystemType: PROFORMA already exists.';

    -- Table: ElectronicDocumentTransmissionSystemType
    PRINT 'Populating ElectronicDocumentTransmissionSystemType...';
    DECLARE @EDTSTId_TaxAuthAPI UNIQUEIDENTIFIER, @EDTSTId_Peppol UNIQUEIDENTIFIER;
    IF @InvoiceSystemTypeId_STDINV IS NOT NULL -- Only proceed if the prerequisite InvoiceSystemType exists
    BEGIN
        SELECT TOP 1 @EDTSTId_TaxAuthAPI = ElectronicDocumentTransmissionSystemTypeId FROM ElectronicDocumentTransmissionSystemType WHERE ElectronicDocumentTransmissionSystemTypeCode = 'TAX_AUTH_V1' AND CountryId = @CountryId;
        IF @EDTSTId_TaxAuthAPI IS NULL
        BEGIN
            SET @EDTSTId_TaxAuthAPI = NEWID();
            INSERT INTO ElectronicDocumentTransmissionSystemType (
                ElectronicDocumentTransmissionSystemTypeId, ElectronicDocumentTransmissionSystemTypeCode, ElectronicDocumentTransmissionSystemTypeName,
                ElectronicDocumentTransmissionSystemTypeDescription, ElectronicDocumentTransmissionSystemTypeURL, ElectronicDocumentTransmissionSystemTypeHTTPMethod,
                ElectronicDocumentTransmissionSystemTypePayloadType, ElectronicDocumentTransmissionSystemTypePayloadSchema, ElectronicDocumentTransmissionSystemTypeHeaders,
                ElectronicDocumentTransmissionSystemTypeSuccessStatusCodes, ElectronicDocumentTransmissionSystemTypeContentType, ElectronicDocumentTransmissionSystemTypeSuccessResponseRegex,
                CountryId, BoundedContextId, InvoiceSystemTypeId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted
            ) VALUES (
                @EDTSTId_TaxAuthAPI, 'TAX_AUTH_V1', 'Tax Authority API v1.0',
                'Direct API submission to national tax authority.', 'https://api.taxauthority.gov/v1/invoices', 'POST',
                'JSON', '{"type":"object", "properties": {"invoiceNumber": {"type":"string"}}}', '{"Authorization":"Bearer [TOKEN]", "X-API-Key":"[KEY]"}',
                '200,201,202', 'application/json', '"status":"(accepted|pending)"',
                @CountryId, @BoundedContextId, @InvoiceSystemTypeId_STDINV, GETDATE(), @UserId, NULL, NULL, 1, 0
            );
            PRINT 'Inserted ElectronicDocumentTransmissionSystemType: TAX_AUTH_V1';
        END ELSE PRINT 'ElectronicDocumentTransmissionSystemType: TAX_AUTH_V1 already exists.';

        SELECT TOP 1 @EDTSTId_Peppol = ElectronicDocumentTransmissionSystemTypeId FROM ElectronicDocumentTransmissionSystemType WHERE ElectronicDocumentTransmissionSystemTypeCode = 'PEPPOL_AS4' AND CountryId = @CountryId;
        IF @EDTSTId_Peppol IS NULL
        BEGIN
            SET @EDTSTId_Peppol = NEWID();
            INSERT INTO ElectronicDocumentTransmissionSystemType (
                ElectronicDocumentTransmissionSystemTypeId, ElectronicDocumentTransmissionSystemTypeCode, ElectronicDocumentTransmissionSystemTypeName,
                ElectronicDocumentTransmissionSystemTypeDescription, ElectronicDocumentTransmissionSystemTypeURL, ElectronicDocumentTransmissionSystemTypeHTTPMethod,
                ElectronicDocumentTransmissionSystemTypePayloadType, ElectronicDocumentTransmissionSystemTypePayloadSchema, ElectronicDocumentTransmissionSystemTypeHeaders,
                ElectronicDocumentTransmissionSystemTypeSuccessStatusCodes, ElectronicDocumentTransmissionSystemTypeContentType, ElectronicDocumentTransmissionSystemTypeSuccessResponseRegex,
                CountryId, BoundedContextId, InvoiceSystemTypeId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted
            ) VALUES (
                @EDTSTId_Peppol, 'PEPPOL_AS4', 'PEPPOL AS4 Profile',
                'Transmission via PEPPOL network using AS4.', 'https://peppol.accesspoint.example.com/as4', 'POST', -- URL is AP specific
                'XML', '<!-- UBL Invoice Schema -->', NULL, -- Headers specific to AS4
                '200', 'application/xml', '<SignalMessage>.+<MessageIdentifier>', -- Simplified regex
                @CountryId, @BoundedContextId, @InvoiceSystemTypeId_STDINV, GETDATE(), @UserId, NULL, NULL, 1, 0
            );
            PRINT 'Inserted ElectronicDocumentTransmissionSystemType: PEPPOL_AS4';
        END ELSE PRINT 'ElectronicDocumentTransmissionSystemType: PEPPOL_AS4 already exists.';
    END
    ELSE
    BEGIN
     PRINT 'Skipping ElectronicDocumentTransmissionSystemType population as prerequisite InvoiceSystemType STDINV is missing.';
    END


    -- Table: EmailAddressSystemType
    PRINT 'Populating EmailAddressSystemType...';
    DECLARE @EmailSysTypeId_PRI UNIQUEIDENTIFIER, @EmailSysTypeId_BILL UNIQUEIDENTIFIER, @EmailSysTypeId_SUPPORT UNIQUEIDENTIFIER, @EmailSysTypeId_SALES UNIQUEIDENTIFIER;

    -- PRI (Primary) - from your original script (or inferred for consistency)
    SELECT TOP 1 @EmailSysTypeId_PRI = EmailAddressSystemTypeId FROM EmailAddressSystemType WHERE EmailAddressSystemTypeCode = 'PRI' AND BoundedContextId = @BoundedContextId;
    IF @EmailSysTypeId_PRI IS NULL
    BEGIN
        SET @EmailSysTypeId_PRI = NEWID();
        INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@EmailSysTypeId_PRI, 'PRI', 'Primary Email', @BoundedContextId, 1, 0);
        PRINT 'Inserted EmailAddressSystemType: PRI';
    END ELSE PRINT 'EmailAddressSystemType: PRI already exists.';

    -- BILLING - from your original script
    SELECT TOP 1 @EmailSysTypeId_BILL = EmailAddressSystemTypeId FROM EmailAddressSystemType WHERE EmailAddressSystemTypeCode = 'BILLING' AND BoundedContextId = @BoundedContextId;
    IF @EmailSysTypeId_BILL IS NULL
    BEGIN
        SET @EmailSysTypeId_BILL = NEWID();
        INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@EmailSysTypeId_BILL, 'BILLING', 'Billing Email', @BoundedContextId, 1, 0);
        PRINT 'Inserted EmailAddressSystemType: BILLING';
    END ELSE PRINT 'EmailAddressSystemType: BILLING already exists.';

    SELECT TOP 1 @EmailSysTypeId_SUPPORT = EmailAddressSystemTypeId FROM EmailAddressSystemType WHERE EmailAddressSystemTypeCode = 'SUPPORT' AND BoundedContextId = @BoundedContextId;
    IF @EmailSysTypeId_SUPPORT IS NULL
    BEGIN
        SET @EmailSysTypeId_SUPPORT = NEWID();
        INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@EmailSysTypeId_SUPPORT, 'SUPPORT', 'Support Email', @BoundedContextId, 1, 0);
        PRINT 'Inserted EmailAddressSystemType: SUPPORT';
    END ELSE PRINT 'EmailAddressSystemType: SUPPORT already exists.';

    SELECT TOP 1 @EmailSysTypeId_SALES = EmailAddressSystemTypeId FROM EmailAddressSystemType WHERE EmailAddressSystemTypeCode = 'SALES' AND BoundedContextId = @BoundedContextId;
    IF @EmailSysTypeId_SALES IS NULL
    BEGIN
        SET @EmailSysTypeId_SALES = NEWID();
        INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@EmailSysTypeId_SALES, 'SALES', 'Sales Email', @BoundedContextId, 1, 0);
        PRINT 'Inserted EmailAddressSystemType: SALES';
    END ELSE PRINT 'EmailAddressSystemType: SALES already exists.';


    -- Table: InvoiceJournalSystemType
    PRINT 'Populating InvoiceJournalSystemType...';
    DECLARE @InvJournalSysTypeId_SALES UNIQUEIDENTIFIER, @InvJournalSysTypeId_PURCH UNIQUEIDENTIFIER, @InvJournalSysTypeId_GENERAL UNIQUEIDENTIFIER;

    SELECT TOP 1 @InvJournalSysTypeId_SALES = InvoiceJournalSystemTypeId FROM InvoiceJournalSystemType WHERE InvoiceJournalSystemTypeCode = 'SALES_JRNL';
    IF @InvJournalSysTypeId_SALES IS NULL
    BEGIN
        SET @InvJournalSysTypeId_SALES = NEWID();
        INSERT INTO InvoiceJournalSystemType (InvoiceJournalSystemTypeId, InvoiceJournalSystemTypeCode, InvoiceJournalSystemTypeName, IsActive)
        VALUES (@InvJournalSysTypeId_SALES, 'SALES_JRNL', 'Sales Journal', 1);
        PRINT 'Inserted InvoiceJournalSystemType: SALES_JRNL';
    END ELSE PRINT 'InvoiceJournalSystemType: SALES_JRNL already exists.';

    SELECT TOP 1 @InvJournalSysTypeId_PURCH = InvoiceJournalSystemTypeId FROM InvoiceJournalSystemType WHERE InvoiceJournalSystemTypeCode = 'PURCH_JRNL';
    IF @InvJournalSysTypeId_PURCH IS NULL
    BEGIN
        SET @InvJournalSysTypeId_PURCH = NEWID();
        INSERT INTO InvoiceJournalSystemType (InvoiceJournalSystemTypeId, InvoiceJournalSystemTypeCode, InvoiceJournalSystemTypeName, IsActive)
        VALUES (@InvJournalSysTypeId_PURCH, 'PURCH_JRNL', 'Purchases Journal', 1);
        PRINT 'Inserted InvoiceJournalSystemType: PURCH_JRNL';
    END ELSE PRINT 'InvoiceJournalSystemType: PURCH_JRNL already exists.';

    SELECT TOP 1 @InvJournalSysTypeId_GENERAL = InvoiceJournalSystemTypeId FROM InvoiceJournalSystemType WHERE InvoiceJournalSystemTypeCode = 'GEN_LEDGER';
    IF @InvJournalSysTypeId_GENERAL IS NULL
    BEGIN
        SET @InvJournalSysTypeId_GENERAL = NEWID();
        INSERT INTO InvoiceJournalSystemType (InvoiceJournalSystemTypeId, InvoiceJournalSystemTypeCode, InvoiceJournalSystemTypeName, IsActive)
        VALUES (@InvJournalSysTypeId_GENERAL, 'GEN_LEDGER', 'General Ledger Journal', 1);
        PRINT 'Inserted InvoiceJournalSystemType: GEN_LEDGER';
    END ELSE PRINT 'InvoiceJournalSystemType: GEN_LEDGER already exists.';


    -- Table: MeasurementUnitSystemType
    PRINT 'Populating MeasurementUnitSystemType...';
    DECLARE @MUId_EACH UNIQUEIDENTIFIER, @MUId_KG UNIQUEIDENTIFIER, @MUId_LBS UNIQUEIDENTIFIER, @MUId_MTR UNIQUEIDENTIFIER, @MUId_BOX UNIQUEIDENTIFIER, @MUId_HOUR UNIQUEIDENTIFIER;

    -- EACH - from your original script
    SELECT TOP 1 @MUId_EACH = MeasurementUnitSystemTypeId FROM MeasurementUnitSystemType WHERE MeasurementUnitSystemTypeCode = 'EACH';
    IF @MUId_EACH IS NULL
    BEGIN
        SET @MUId_EACH = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@MUId_EACH, 'EACH', 'Each', 'ea', @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted MeasurementUnitSystemType: EACH';
    END ELSE PRINT 'MeasurementUnitSystemType: EACH already exists.';

    SELECT TOP 1 @MUId_KG = MeasurementUnitSystemTypeId FROM MeasurementUnitSystemType WHERE MeasurementUnitSystemTypeCode = 'KG';
    IF @MUId_KG IS NULL
    BEGIN
        SET @MUId_KG = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@MUId_KG, 'KG', 'Kilogram', 'kg', @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted MeasurementUnitSystemType: KG';
    END ELSE PRINT 'MeasurementUnitSystemType: KG already exists.';

    SELECT TOP 1 @MUId_LBS = MeasurementUnitSystemTypeId FROM MeasurementUnitSystemType WHERE MeasurementUnitSystemTypeCode = 'LBS';
    IF @MUId_LBS IS NULL
    BEGIN
        SET @MUId_LBS = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@MUId_LBS, 'LBS', 'Pounds', 'lbs', @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted MeasurementUnitSystemType: LBS';
    END ELSE PRINT 'MeasurementUnitSystemType: LBS already exists.';

    SELECT TOP 1 @MUId_MTR = MeasurementUnitSystemTypeId FROM MeasurementUnitSystemType WHERE MeasurementUnitSystemTypeCode = 'MTR';
    IF @MUId_MTR IS NULL
    BEGIN
        SET @MUId_MTR = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@MUId_MTR, 'MTR', 'Meter', 'm', @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted MeasurementUnitSystemType: MTR';
    END ELSE PRINT 'MeasurementUnitSystemType: MTR already exists.';

    SELECT TOP 1 @MUId_BOX = MeasurementUnitSystemTypeId FROM MeasurementUnitSystemType WHERE MeasurementUnitSystemTypeCode = 'BOX';
    IF @MUId_BOX IS NULL
    BEGIN
        SET @MUId_BOX = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@MUId_BOX, 'BOX', 'Box', 'box', @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted MeasurementUnitSystemType: BOX';
    END ELSE PRINT 'MeasurementUnitSystemType: BOX already exists.';
    
    SELECT TOP 1 @MUId_HOUR = MeasurementUnitSystemTypeId FROM MeasurementUnitSystemType WHERE MeasurementUnitSystemTypeCode = 'HOUR';
    IF @MUId_HOUR IS NULL
    BEGIN
        SET @MUId_HOUR = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, BoundedContextId, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@MUId_HOUR, 'HOUR', 'Hour', 'hr', @BoundedContextId, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted MeasurementUnitSystemType: HOUR';
    END ELSE PRINT 'MeasurementUnitSystemType: HOUR already exists.';


    -- Table: MessageType
    PRINT 'Populating MessageType...';
    DECLARE @MessageTypeId_EMAIL UNIQUEIDENTIFIER, @MessageTypeId_SMS UNIQUEIDENTIFIER, @MessageTypeId_SYSTEM UNIQUEIDENTIFIER;

    SELECT TOP 1 @MessageTypeId_EMAIL = MessageTypeId FROM MessageType WHERE MessageTypeName = 'Email Notification';
    IF @MessageTypeId_EMAIL IS NULL
    BEGIN
        SET @MessageTypeId_EMAIL = NEWID();
        INSERT INTO MessageType (MessageTypeId, MessageTypeName, MessageTypeDescription)
        VALUES (@MessageTypeId_EMAIL, 'Email Notification', 'Standard notification sent via email');
        PRINT 'Inserted MessageType: Email Notification';
    END ELSE PRINT 'MessageType: Email Notification already exists.';

    SELECT TOP 1 @MessageTypeId_SMS = MessageTypeId FROM MessageType WHERE MessageTypeName = 'SMS Alert';
    IF @MessageTypeId_SMS IS NULL
    BEGIN
        SET @MessageTypeId_SMS = NEWID();
        INSERT INTO MessageType (MessageTypeId, MessageTypeName, MessageTypeDescription)
        VALUES (@MessageTypeId_SMS, 'SMS Alert', 'Short message alert sent via SMS');
        PRINT 'Inserted MessageType: SMS Alert';
    END ELSE PRINT 'MessageType: SMS Alert already exists.';
    
    SELECT TOP 1 @MessageTypeId_SYSTEM = MessageTypeId FROM MessageType WHERE MessageTypeName = 'In-App Notification';
    IF @MessageTypeId_SYSTEM IS NULL
    BEGIN
        SET @MessageTypeId_SYSTEM = NEWID();
        INSERT INTO MessageType (MessageTypeId, MessageTypeName, MessageTypeDescription)
        VALUES (@MessageTypeId_SYSTEM, 'In-App Notification', 'Notification displayed within the application');
        PRINT 'Inserted MessageType: In-App Notification';
    END ELSE PRINT 'MessageType: In-App Notification already exists.';


    -- Table: NotificationType
    PRINT 'Populating NotificationType...';
    DECLARE @NotificationTypeId_ORDERCONF UNIQUEIDENTIFIER, @NotificationTypeId_SHIPUPDATE UNIQUEIDENTIFIER, @NotificationTypeId_PAYREMIND UNIQUEIDENTIFIER;

    SELECT TOP 1 @NotificationTypeId_ORDERCONF = NotificationTypeId FROM NotificationType WHERE NotificationTypeName = 'Order Confirmation';
    IF @NotificationTypeId_ORDERCONF IS NULL
    BEGIN
        SET @NotificationTypeId_ORDERCONF = NEWID();
        INSERT INTO NotificationType (NotificationTypeId, NotificationTypeName, Description, NotificationThumbnail)
        VALUES (@NotificationTypeId_ORDERCONF, 'Order Confirmation', 'Notification for confirmed customer orders', NULL);
        PRINT 'Inserted NotificationType: Order Confirmation';
    END ELSE PRINT 'NotificationType: Order Confirmation already exists.';

    SELECT TOP 1 @NotificationTypeId_SHIPUPDATE = NotificationTypeId FROM NotificationType WHERE NotificationTypeName = 'Shipping Update';
    IF @NotificationTypeId_SHIPUPDATE IS NULL
    BEGIN
        SET @NotificationTypeId_SHIPUPDATE = NEWID();
        INSERT INTO NotificationType (NotificationTypeId, NotificationTypeName, Description, NotificationThumbnail)
        VALUES (@NotificationTypeId_SHIPUPDATE, 'Shipping Update', 'Notification for shipping status changes', NULL);
        PRINT 'Inserted NotificationType: Shipping Update';
    END ELSE PRINT 'NotificationType: Shipping Update already exists.';

    SELECT TOP 1 @NotificationTypeId_PAYREMIND = NotificationTypeId FROM NotificationType WHERE NotificationTypeName = 'Payment Reminder';
    IF @NotificationTypeId_PAYREMIND IS NULL
    BEGIN
        SET @NotificationTypeId_PAYREMIND = NEWID();
        INSERT INTO NotificationType (NotificationTypeId, NotificationTypeName, Description, NotificationThumbnail)
        VALUES (@NotificationTypeId_PAYREMIND, 'Payment Reminder', 'Reminder for overdue payments', NULL);
        PRINT 'Inserted NotificationType: Payment Reminder';
    END ELSE PRINT 'NotificationType: Payment Reminder already exists.';


    -- Table: PaymentSystemType
    PRINT 'Populating PaymentSystemType...';
    DECLARE @PaymentSysTypeId_CC UNIQUEIDENTIFIER, @PaymentSysTypeId_BANKXFER UNIQUEIDENTIFIER, @PaymentSysTypeId_CASH UNIQUEIDENTIFIER, @PaymentSysTypeId_CHECK UNIQUEIDENTIFIER;

    SELECT TOP 1 @PaymentSysTypeId_CC = PaymentSystemTypeId FROM PaymentSystemType WHERE PaymentSystemTypeCode = 'CREDIT_CARD';
    IF @PaymentSysTypeId_CC IS NULL
    BEGIN
        SET @PaymentSysTypeId_CC = NEWID();
        INSERT INTO PaymentSystemType (PaymentSystemTypeId, PaymentSystemTypeCode, PaymentSystemTypeName, Description, IsRemittable, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsRemovable, IsActive, IsDeleted)
        VALUES (@PaymentSysTypeId_CC, 'CREDIT_CARD', 'Credit Card', 'Payment via Visa, MasterCard, Amex, etc.', 1, GETDATE(), @UserId, NULL, NULL, 0, 1, 0);
        PRINT 'Inserted PaymentSystemType: CREDIT_CARD';
    END ELSE PRINT 'PaymentSystemType: CREDIT_CARD already exists.';

    SELECT TOP 1 @PaymentSysTypeId_BANKXFER = PaymentSystemTypeId FROM PaymentSystemType WHERE PaymentSystemTypeCode = 'BANK_XFER';
    IF @PaymentSysTypeId_BANKXFER IS NULL
    BEGIN
        SET @PaymentSysTypeId_BANKXFER = NEWID();
        INSERT INTO PaymentSystemType (PaymentSystemTypeId, PaymentSystemTypeCode, PaymentSystemTypeName, Description, IsRemittable, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsRemovable, IsActive, IsDeleted)
        VALUES (@PaymentSysTypeId_BANKXFER, 'BANK_XFER', 'Bank Transfer', 'Direct bank transfer (ACH, Wire, SEPA)', 1, GETDATE(), @UserId, NULL, NULL, 0, 1, 0);
        PRINT 'Inserted PaymentSystemType: BANK_XFER';
    END ELSE PRINT 'PaymentSystemType: BANK_XFER already exists.';

    SELECT TOP 1 @PaymentSysTypeId_CASH = PaymentSystemTypeId FROM PaymentSystemType WHERE PaymentSystemTypeCode = 'CASH';
    IF @PaymentSysTypeId_CASH IS NULL
    BEGIN
        SET @PaymentSysTypeId_CASH = NEWID();
        INSERT INTO PaymentSystemType (PaymentSystemTypeId, PaymentSystemTypeCode, PaymentSystemTypeName, Description, IsRemittable, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsRemovable, IsActive, IsDeleted)
        VALUES (@PaymentSysTypeId_CASH, 'CASH', 'Cash', 'Payment in physical currency', 0, GETDATE(), @UserId, NULL, NULL, 0, 1, 0);
        PRINT 'Inserted PaymentSystemType: CASH';
    END ELSE PRINT 'PaymentSystemType: CASH already exists.';
    
    SELECT TOP 1 @PaymentSysTypeId_CHECK = PaymentSystemTypeId FROM PaymentSystemType WHERE PaymentSystemTypeCode = 'CHECK';
    IF @PaymentSysTypeId_CHECK IS NULL
    BEGIN
        SET @PaymentSysTypeId_CHECK = NEWID();
        INSERT INTO PaymentSystemType (PaymentSystemTypeId, PaymentSystemTypeCode, PaymentSystemTypeName, Description, IsRemittable, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsRemovable, IsActive, IsDeleted)
        VALUES (@PaymentSysTypeId_CHECK, 'CHECK', 'Check', 'Payment by paper check', 1, GETDATE(), @UserId, NULL, NULL, 0, 1, 0);
        PRINT 'Inserted PaymentSystemType: CHECK';
    END ELSE PRINT 'PaymentSystemType: CHECK already exists.';


    -- Table: PhoneNumberSystemType
    PRINT 'Populating PhoneNumberSystemType...';
    DECLARE @PhoneSysTypeId_PRI UNIQUEIDENTIFIER, @PhoneSysTypeId_WORK UNIQUEIDENTIFIER, @PhoneSysTypeId_MOBILE UNIQUEIDENTIFIER, @PhoneSysTypeId_FAX UNIQUEIDENTIFIER;

    -- PRI (Primary) - from your original script
    SELECT TOP 1 @PhoneSysTypeId_PRI = PhoneNumberSystemTypeId FROM PhoneNumberSystemType WHERE PhoneNumberSystemTypeCode = 'PRI' AND BoundedContextId = @BoundedContextId;
    IF @PhoneSysTypeId_PRI IS NULL
    BEGIN
        SET @PhoneSysTypeId_PRI = NEWID();
        INSERT INTO PhoneNumberSystemType (PhoneNumberSystemTypeId, PhoneNumberSystemTypeCode, PhoneNumberSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@PhoneSysTypeId_PRI, 'PRI', 'Primary Phone', @BoundedContextId, 1, 0);
        PRINT 'Inserted PhoneNumberSystemType: PRI';
    END ELSE PRINT 'PhoneNumberSystemType: PRI already exists.';

    SELECT TOP 1 @PhoneSysTypeId_WORK = PhoneNumberSystemTypeId FROM PhoneNumberSystemType WHERE PhoneNumberSystemTypeCode = 'WORK' AND BoundedContextId = @BoundedContextId;
    IF @PhoneSysTypeId_WORK IS NULL
    BEGIN
        SET @PhoneSysTypeId_WORK = NEWID();
        INSERT INTO PhoneNumberSystemType (PhoneNumberSystemTypeId, PhoneNumberSystemTypeCode, PhoneNumberSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@PhoneSysTypeId_WORK, 'WORK', 'Work Phone', @BoundedContextId, 1, 0);
        PRINT 'Inserted PhoneNumberSystemType: WORK';
    END ELSE PRINT 'PhoneNumberSystemType: WORK already exists.';

    SELECT TOP 1 @PhoneSysTypeId_MOBILE = PhoneNumberSystemTypeId FROM PhoneNumberSystemType WHERE PhoneNumberSystemTypeCode = 'MOBILE' AND BoundedContextId = @BoundedContextId;
    IF @PhoneSysTypeId_MOBILE IS NULL
    BEGIN
        SET @PhoneSysTypeId_MOBILE = NEWID();
        INSERT INTO PhoneNumberSystemType (PhoneNumberSystemTypeId, PhoneNumberSystemTypeCode, PhoneNumberSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@PhoneSysTypeId_MOBILE, 'MOBIL', 'Mobile Phone', @BoundedContextId, 1, 0);
        PRINT 'Inserted PhoneNumberSystemType: MOBILE';
    END ELSE PRINT 'PhoneNumberSystemType: MOBILE already exists.';

    SELECT TOP 1 @PhoneSysTypeId_FAX = PhoneNumberSystemTypeId FROM PhoneNumberSystemType WHERE PhoneNumberSystemTypeCode = 'FAX' AND BoundedContextId = @BoundedContextId;
    IF @PhoneSysTypeId_FAX IS NULL
    BEGIN
        SET @PhoneSysTypeId_FAX = NEWID();
        INSERT INTO PhoneNumberSystemType (PhoneNumberSystemTypeId, PhoneNumberSystemTypeCode, PhoneNumberSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@PhoneSysTypeId_FAX, 'FAX', 'Fax Number', @BoundedContextId, 1, 0);
        PRINT 'Inserted PhoneNumberSystemType: FAX';
    END ELSE PRINT 'PhoneNumberSystemType: FAX already exists.';


    -- Table: PriceUserType
    PRINT 'Populating PriceUserType...';
    DECLARE @PriceUserTypeId_RETAIL UNIQUEIDENTIFIER, @PriceUserTypeId_WHOLESALE UNIQUEIDENTIFIER, @PriceUserTypeId_PROMO10 UNIQUEIDENTIFIER, @PriceUserTypeId_MINPRICE UNIQUEIDENTIFIER;

    SELECT TOP 1 @PriceUserTypeId_RETAIL = PriceUserTypeId FROM PriceUserType WHERE PriceUserTypeCode = 'RETAIL';
    IF @PriceUserTypeId_RETAIL IS NULL
    BEGIN
        SET @PriceUserTypeId_RETAIL = NEWID();
        INSERT INTO PriceUserType (PriceUserTypeId, PriceUserTypeCode, PriceUserTypeName, Description, IsMinimumPrice, IsGlobalPrice, IsDiscountedInPrice, PriceDiscountPercentage, PriceValidFrom, PriceValidUntil, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PriceUserTypeId_RETAIL, 'RETAIL', 'Retail Price', 'Standard consumer retail price list', 0, 1, 0, NULL, NULL, NULL, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted PriceUserType: RETAIL';
    END ELSE PRINT 'PriceUserType: RETAIL already exists.';

    SELECT TOP 1 @PriceUserTypeId_WHOLESALE = PriceUserTypeId FROM PriceUserType WHERE PriceUserTypeCode = 'WHOLESALE';
    IF @PriceUserTypeId_WHOLESALE IS NULL
    BEGIN
        SET @PriceUserTypeId_WHOLESALE = NEWID();
        INSERT INTO PriceUserType (PriceUserTypeId, PriceUserTypeCode, PriceUserTypeName, Description, IsMinimumPrice, IsGlobalPrice, IsDiscountedInPrice, PriceDiscountPercentage, PriceValidFrom, PriceValidUntil, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PriceUserTypeId_WHOLESALE, 'WHOLESALE', 'Wholesale Price', 'Price list for B2B resellers', 0, 0, 0, NULL, NULL, NULL, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted PriceUserType: WHOLESALE';
    END ELSE PRINT 'PriceUserType: WHOLESALE already exists.';

    SELECT TOP 1 @PriceUserTypeId_PROMO10 = PriceUserTypeId FROM PriceUserType WHERE PriceUserTypeCode = 'PROMO_10_OFF';
    IF @PriceUserTypeId_PROMO10 IS NULL
    BEGIN
        SET @PriceUserTypeId_PROMO10 = NEWID();
        INSERT INTO PriceUserType (PriceUserTypeId, PriceUserTypeCode, PriceUserTypeName, Description, IsMinimumPrice, IsGlobalPrice, IsDiscountedInPrice, PriceDiscountPercentage, PriceValidFrom, PriceValidUntil, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PriceUserTypeId_PROMO10, 'PROMO_10_OFF', 'Promotional 10% Discount', 'Time-limited 10% discount offer', 0, 0, 1, 0.10, GETDATE(), DATEADD(month, 1, GETDATE()), GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted PriceUserType: PROMO_10_OFF';
    END ELSE PRINT 'PriceUserType: PROMO_10_OFF already exists.';
    
    SELECT TOP 1 @PriceUserTypeId_MINPRICE = PriceUserTypeId FROM PriceUserType WHERE PriceUserTypeCode = 'MIN_CONTRACT';
    IF @PriceUserTypeId_MINPRICE IS NULL
    BEGIN
        SET @PriceUserTypeId_MINPRICE = NEWID();
        INSERT INTO PriceUserType (PriceUserTypeId, PriceUserTypeCode, PriceUserTypeName, Description, IsMinimumPrice, IsGlobalPrice, IsDiscountedInPrice, PriceDiscountPercentage, PriceValidFrom, PriceValidUntil, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PriceUserTypeId_MINPRICE, 'MIN_CONTRACT', 'Minimum Contract Price', 'Minimum price agreed in a contract', 1, 0, 0, NULL, '2023-01-01', '2024-12-31', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted PriceUserType: MIN_CONTRACT';
    END ELSE PRINT 'PriceUserType: MIN_CONTRACT already exists.';


    -- Table: ProductSystemType
    PRINT 'Populating ProductSystemType...';
    DECLARE @ProductSysTypeId_PROD UNIQUEIDENTIFIER, @ProductSysTypeId_SERVICE UNIQUEIDENTIFIER, @ProductSysTypeId_ASSET UNIQUEIDENTIFIER, @ProductSysTypeId_EXPENSE UNIQUEIDENTIFIER;

    -- PROD (Product) - from your original script
    SELECT TOP 1 @ProductSysTypeId_PROD = ProductSystemTypeId FROM ProductSystemType WHERE ProductSystemTypeCode = 'PROD';
    IF @ProductSysTypeId_PROD IS NULL
    BEGIN
        SET @ProductSysTypeId_PROD = NEWID();
        INSERT INTO ProductSystemType (ProductSystemTypeId, ProductSystemTypeCode, ProductSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@ProductSysTypeId_PROD, 'PROD', 'Finished Product', 'Standard sellable finished good', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ProductSystemType: PROD';
    END ELSE PRINT 'ProductSystemType: PROD already exists.';

    SELECT TOP 1 @ProductSysTypeId_SERVICE = ProductSystemTypeId FROM ProductSystemType WHERE ProductSystemTypeCode = 'SERVICE';
    IF @ProductSysTypeId_SERVICE IS NULL
    BEGIN
        SET @ProductSysTypeId_SERVICE = NEWID();
        INSERT INTO ProductSystemType (ProductSystemTypeId, ProductSystemTypeCode, ProductSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@ProductSysTypeId_SERVICE, 'SERVICE', 'Service Item', 'Billable service offering', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ProductSystemType: SERVICE';
    END ELSE PRINT 'ProductSystemType: SERVICE already exists.';

    SELECT TOP 1 @ProductSysTypeId_ASSET = ProductSystemTypeId FROM ProductSystemType WHERE ProductSystemTypeCode = 'ASSET_FIX';
    IF @ProductSysTypeId_ASSET IS NULL
    BEGIN
        SET @ProductSysTypeId_ASSET = NEWID();
        INSERT INTO ProductSystemType (ProductSystemTypeId, ProductSystemTypeCode, ProductSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@ProductSysTypeId_ASSET, 'ASSET_FIX', 'Fixed Asset', 'Capitalized fixed asset', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ProductSystemType: ASSET_FIX';
    END ELSE PRINT 'ProductSystemType: ASSET_FIX already exists.';

    SELECT TOP 1 @ProductSysTypeId_EXPENSE = ProductSystemTypeId FROM ProductSystemType WHERE ProductSystemTypeCode = 'EXPENSE_OP';
    IF @ProductSysTypeId_EXPENSE IS NULL
    BEGIN
        SET @ProductSysTypeId_EXPENSE = NEWID();
        INSERT INTO ProductSystemType (ProductSystemTypeId, ProductSystemTypeCode, ProductSystemTypeName, Description, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@ProductSysTypeId_EXPENSE, 'EXPENSE_OP', 'Operational Expense', 'Non-stock operational expense item', GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted ProductSystemType: EXPENSE_OP';
    END ELSE PRINT 'ProductSystemType: EXPENSE_OP already exists.';


    -- Table: PurchaseSystemType
    PRINT 'Populating PurchaseSystemType...';
    DECLARE @PurchaseSysTypeId_PO UNIQUEIDENTIFIER, @PurchaseSysTypeId_RETURN UNIQUEIDENTIFIER, @PurchaseSysTypeId_RFQ UNIQUEIDENTIFIER;

    SELECT TOP 1 @PurchaseSysTypeId_PO = PurchaseSystemTypeId FROM PurchaseSystemType WHERE PurchaseSystemTypeCode = 'STD_PO';
    IF @PurchaseSysTypeId_PO IS NULL
    BEGIN
        SET @PurchaseSysTypeId_PO = NEWID();
        INSERT INTO PurchaseSystemType (PurchaseSystemTypeId, PurchaseSystemTypeCode, PurchaseSystemTypeName, PurchaseSystemTypeIsPurchase, PurchaseSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PurchaseSysTypeId_PO, 'STD_PO', 'Standard Purchase Order', 1, 1, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted PurchaseSystemType: STD_PO';
    END ELSE PRINT 'PurchaseSystemType: STD_PO already exists.';

    SELECT TOP 1 @PurchaseSysTypeId_RETURN = PurchaseSystemTypeId FROM PurchaseSystemType WHERE PurchaseSystemTypeCode = 'RETURN_AUTH';
    IF @PurchaseSysTypeId_RETURN IS NULL
    BEGIN
        SET @PurchaseSysTypeId_RETURN = NEWID();
        INSERT INTO PurchaseSystemType (PurchaseSystemTypeId, PurchaseSystemTypeCode, PurchaseSystemTypeName, PurchaseSystemTypeIsPurchase, PurchaseSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PurchaseSysTypeId_RETURN, 'RETURN_AUTH', 'Supplier Return Authorization', 1, -1, GETDATE(), @UserId, NULL, NULL, 1, 0);
        PRINT 'Inserted PurchaseSystemType: RETURN_AUTH';
    END ELSE PRINT 'PurchaseSystemType: RETURN_AUTH already exists.';
    
    SELECT TOP 1 @PurchaseSysTypeId_RFQ = PurchaseSystemTypeId FROM PurchaseSystemType WHERE PurchaseSystemTypeCode = 'RFQ';
    IF @PurchaseSysTypeId_RFQ IS NULL
    BEGIN
        SET @PurchaseSysTypeId_RFQ = NEWID();
        INSERT INTO PurchaseSystemType (PurchaseSystemTypeId, PurchaseSystemTypeCode, PurchaseSystemTypeName, PurchaseSystemTypeIsPurchase, PurchaseSystemTypeSign, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, IsDeleted)
        VALUES (@PurchaseSysTypeId_RFQ, 'RFQ', 'Request for Quotation', 0, 0, GETDATE(), @UserId, NULL, NULL, 1, 0); -- Not a purchase itself, Sign 0
        PRINT 'Inserted PurchaseSystemType: RFQ';
    END ELSE PRINT 'PurchaseSystemType: RFQ already exists.';


    -- Table: RatingReasonType
    PRINT 'Populating RatingReasonType...';
    DECLARE @RatingReasonTypeId_QUALITY UNIQUEIDENTIFIER, @RatingReasonTypeId_SERVICE UNIQUEIDENTIFIER, @RatingReasonTypeId_DELIVERY UNIQUEIDENTIFIER, @RatingReasonTypeId_PRICE UNIQUEIDENTIFIER;

    SELECT TOP 1 @RatingReasonTypeId_QUALITY = RatingReasonTypeId FROM RatingReasonType WHERE RatingReasonDescription = 'Product Quality Issue';
    IF @RatingReasonTypeId_QUALITY IS NULL
    BEGIN
        SET @RatingReasonTypeId_QUALITY = NEWID();
        INSERT INTO RatingReasonType (RatingReasonTypeId, RatingReasonDescription)
        VALUES (@RatingReasonTypeId_QUALITY, 'Product Quality Issue');
        PRINT 'Inserted RatingReasonType: Product Quality Issue';
    END ELSE PRINT 'RatingReasonType: Product Quality Issue already exists.';

    SELECT TOP 1 @RatingReasonTypeId_SERVICE = RatingReasonTypeId FROM RatingReasonType WHERE RatingReasonDescription = 'Excellent Customer Service';
    IF @RatingReasonTypeId_SERVICE IS NULL
    BEGIN
        SET @RatingReasonTypeId_SERVICE = NEWID();
        INSERT INTO RatingReasonType (RatingReasonTypeId, RatingReasonDescription)
        VALUES (@RatingReasonTypeId_SERVICE, 'Excellent Customer Service');
        PRINT 'Inserted RatingReasonType: Excellent Customer Service';
    END ELSE PRINT 'RatingReasonType: Excellent Customer Service already exists.';

    SELECT TOP 1 @RatingReasonTypeId_DELIVERY = RatingReasonTypeId FROM RatingReasonType WHERE RatingReasonDescription = 'Delivery Time';
    IF @RatingReasonTypeId_DELIVERY IS NULL
    BEGIN
        SET @RatingReasonTypeId_DELIVERY = NEWID();
        INSERT INTO RatingReasonType (RatingReasonTypeId, RatingReasonDescription)
        VALUES (@RatingReasonTypeId_DELIVERY, 'Delivery Time');
        PRINT 'Inserted RatingReasonType: Delivery Time';
    END ELSE PRINT 'RatingReasonType: Delivery Time already exists.';
    
    SELECT TOP 1 @RatingReasonTypeId_PRICE = RatingReasonTypeId FROM RatingReasonType WHERE RatingReasonDescription = 'Price Competitiveness';
    IF @RatingReasonTypeId_PRICE IS NULL
    BEGIN
        SET @RatingReasonTypeId_PRICE = NEWID();
        INSERT INTO RatingReasonType (RatingReasonTypeId, RatingReasonDescription)
        VALUES (@RatingReasonTypeId_PRICE, 'Price Competitiveness');
        PRINT 'Inserted RatingReasonType: Price Competitiveness';
    END ELSE PRINT 'RatingReasonType: Price Competitiveness already exists.';


    -- Table: TaxpayerSystemType
    PRINT 'Populating TaxpayerSystemType...';
    DECLARE @TaxpayerSysTypeId_STD UNIQUEIDENTIFIER, @TaxpayerSysTypeId_INDIV UNIQUEIDENTIFIER, @TaxpayerSysTypeId_CORP UNIQUEIDENTIFIER, @TaxpayerSysTypeId_NONPROFIT UNIQUEIDENTIFIER;

    -- STD (Standard) - from your original script
    SELECT TOP 1 @TaxpayerSysTypeId_STD = TaxpayerSystemTypeId FROM TaxpayerSystemType WHERE TaxpayerSystemTypeCode = 'STD' AND CountryId = @CountryId;
    IF @TaxpayerSysTypeId_STD IS NULL
    BEGIN
        SET @TaxpayerSysTypeId_STD = NEWID();
        INSERT INTO TaxpayerSystemType (TaxpayerSystemTypeId, TaxpayerSystemTypeCode, TaxpayerSystemTypeName, TaxpayerSystemTypeValue, CountryId, BoundedContextId, IsActive, IsDeleted)
        VALUES (@TaxpayerSysTypeId_STD, 'STD', 'Standard Taxpayer', 1.0, @CountryId, @BoundedContextId, 1, 0); -- Value could be a multiplier or enum
        PRINT 'Inserted TaxpayerSystemType: STD';
    END ELSE PRINT 'TaxpayerSystemType: STD already exists.';

    SELECT TOP 1 @TaxpayerSysTypeId_INDIV = TaxpayerSystemTypeId FROM TaxpayerSystemType WHERE TaxpayerSystemTypeCode = 'INDIV' AND CountryId = @CountryId;
    IF @TaxpayerSysTypeId_INDIV IS NULL
    BEGIN
        SET @TaxpayerSysTypeId_INDIV = NEWID();
        INSERT INTO TaxpayerSystemType (TaxpayerSystemTypeId, TaxpayerSystemTypeCode, TaxpayerSystemTypeName, TaxpayerSystemTypeValue, CountryId, BoundedContextId, IsActive, IsDeleted)
        VALUES (@TaxpayerSysTypeId_INDIV, 'INDIV', 'Individual', 2.0, @CountryId, @BoundedContextId, 1, 0);
        PRINT 'Inserted TaxpayerSystemType: INDIV';
    END ELSE PRINT 'TaxpayerSystemType: INDIV already exists.';

    SELECT TOP 1 @TaxpayerSysTypeId_CORP = TaxpayerSystemTypeId FROM TaxpayerSystemType WHERE TaxpayerSystemTypeCode = 'CORP' AND CountryId = @CountryId;
    IF @TaxpayerSysTypeId_CORP IS NULL
    BEGIN
        SET @TaxpayerSysTypeId_CORP = NEWID();
        INSERT INTO TaxpayerSystemType (TaxpayerSystemTypeId, TaxpayerSystemTypeCode, TaxpayerSystemTypeName, TaxpayerSystemTypeValue, CountryId, BoundedContextId, IsActive, IsDeleted)
        VALUES (@TaxpayerSysTypeId_CORP, 'CORP', 'Corporation', 3.0, @CountryId, @BoundedContextId, 1, 0);
        PRINT 'Inserted TaxpayerSystemType: CORP';
    END ELSE PRINT 'TaxpayerSystemType: CORP already exists.';
    
    SELECT TOP 1 @TaxpayerSysTypeId_NONPROFIT = TaxpayerSystemTypeId FROM TaxpayerSystemType WHERE TaxpayerSystemTypeCode = 'NON_PROFIT' AND CountryId = @CountryId;
    IF @TaxpayerSysTypeId_NONPROFIT IS NULL
    BEGIN
        SET @TaxpayerSysTypeId_NONPROFIT = NEWID();
        INSERT INTO TaxpayerSystemType (TaxpayerSystemTypeId, TaxpayerSystemTypeCode, TaxpayerSystemTypeName, TaxpayerSystemTypeValue, CountryId, BoundedContextId, IsActive, IsDeleted)
        VALUES (@TaxpayerSysTypeId_NONPROFIT, 'NON_PROFIT', 'Non-Profit Organization', 4.0, @CountryId, @BoundedContextId, 1, 0);
        PRINT 'Inserted TaxpayerSystemType: NON_PROFIT';
    END ELSE PRINT 'TaxpayerSystemType: NON_PROFIT already exists.';


    -- Table: TaxSystemType
    PRINT 'Populating TaxSystemType...';
    DECLARE @TaxSysTypeId_EXEMPT UNIQUEIDENTIFIER, @TaxSysTypeId_NOTSUBJ UNIQUEIDENTIFIER, @TaxSysTypeId_VAT_STD UNIQUEIDENTIFIER, @TaxSysTypeId_VAT_REDUCED UNIQUEIDENTIFIER, @TaxSysTypeId_ZERO UNIQUEIDENTIFIER;

    -- EXEMPT - from your original script
    SELECT TOP 1 @TaxSysTypeId_EXEMPT = TaxSystemTypeId FROM TaxSystemType WHERE TaxSystemTypeCode = 'EXEMPT' AND CountryId = @CountryId;
    IF @TaxSysTypeId_EXEMPT IS NULL
    BEGIN
        SET @TaxSysTypeId_EXEMPT = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSysTypeId_EXEMPT, 'EXEMPT', 'Tax Exempt', 0.00, 0, 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted TaxSystemType: EXEMPT';
    END ELSE PRINT 'TaxSystemType: EXEMPT already exists.';

    -- NOT_SUBJ - from your original script
    SELECT TOP 1 @TaxSysTypeId_NOTSUBJ = TaxSystemTypeId FROM TaxSystemType WHERE TaxSystemTypeCode = 'NOT_SUBJ' AND CountryId = @CountryId;
    IF @TaxSysTypeId_NOTSUBJ IS NULL
    BEGIN
        SET @TaxSysTypeId_NOTSUBJ = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSysTypeId_NOTSUBJ, 'NOT_SUBJ', 'Not Subject to Tax', 0.00, 0, 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted TaxSystemType: NOT_SUBJ';
    END ELSE PRINT 'TaxSystemType: NOT_SUBJ already exists.';
    
    -- Standard VAT (example 20%)
    SELECT TOP 1 @TaxSysTypeId_VAT_STD = TaxSystemTypeId FROM TaxSystemType WHERE TaxSystemTypeCode = 'VAT_STD_20' AND CountryId = @CountryId;
    IF @TaxSysTypeId_VAT_STD IS NULL
    BEGIN
        SET @TaxSysTypeId_VAT_STD = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSysTypeId_VAT_STD, 'VAT_STD_20', 'VAT Standard Rate (20%)', 0.20, 1, 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted TaxSystemType: VAT_STD_20';
    END ELSE PRINT 'TaxSystemType: VAT_STD_20 already exists.';

    -- Reduced VAT (example 5%)
    SELECT TOP 1 @TaxSysTypeId_VAT_REDUCED = TaxSystemTypeId FROM TaxSystemType WHERE TaxSystemTypeCode = 'VAT_RED_5' AND CountryId = @CountryId;
    IF @TaxSysTypeId_VAT_REDUCED IS NULL
    BEGIN
        SET @TaxSysTypeId_VAT_REDUCED = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSysTypeId_VAT_REDUCED, 'VAT_RED_5', 'VAT Reduced Rate (5%)', 0.05, 1, 0, @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Inserted TaxSystemType: VAT_RED_5';
    END ELSE PRINT 'TaxSystemType: VAT_RED_5 already exists.';
    
    -- Zero-Rated VAT
    SELECT TOP 1 @TaxSysTypeId_ZERO = TaxSystemTypeId FROM TaxSystemType WHERE TaxSystemTypeCode = 'VAT_ZERO' AND CountryId = @CountryId;
    IF @TaxSysTypeId_ZERO IS NULL
    BEGIN
        SET @TaxSysTypeId_ZERO = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSysTypeId_ZERO, 'VAT_ZERO', 'VAT Zero Rated', 0.00, 1, 0, @BoundedContextId, @CountryId, 1, 0); -- Sign 1 because it's a type of VAT, just at 0%
        PRINT 'Inserted TaxSystemType: VAT_ZERO';
    END ELSE PRINT 'TaxSystemType: VAT_ZERO already exists.';

    ----------------------------------------------------------------------------------------------------
    -- Informational block about associative "Type" tables listed by the user
    ----------------------------------------------------------------------------------------------------
    PRINT '--- Notes on Associative "Type" Tables ---';
    PRINT 'The following tables are associative (junction) tables that link main entities (like Customer, Product, Invoice) to the "SystemType" lookup tables populated above.';
    PRINT 'Their population is typically handled when those main entities are created, as seen in your original customer graph script.';
    PRINT 'This script focuses on ensuring the *referenced* SystemType tables have the necessary data.';
    PRINT ' - CustomerEconomicActivitySystemType (links Customer to EconomicActivitySystemType)';
    PRINT ' - CustomerTaxSystemType (links Customer to TaxSystemType)';
    PRINT ' - InvoiceDetailTaxSystemType (links InvoiceDetail to TaxSystemType)';
    PRINT ' - InvoiceSystemTypeInvoiceJournalSystemType (links InvoiceSystemType to InvoiceJournalSystemType)';
    PRINT ' - InvoiceTaxSystemType (links Invoice to TaxSystemType)';
    PRINT ' - ProductMeasurementUnitSystemType (links Product to MeasurementUnitSystemType)';
    PRINT ' - ProductPriceUserType (links Product to PriceUserType)';
    PRINT ' - ProductTaxSystemType (links Product to TaxSystemType)';
    PRINT ' - PurchaseDetailTaxSystemType (links PurchaseDetail to TaxSystemType)';
    PRINT ' - SupplierEconomicActivitySystemType (links Supplier to EconomicActivitySystemType)';
    PRINT ' - SupplierTaxSystemType (links Supplier to TaxSystemType)';
    PRINT '--------------------------------------------';


    COMMIT TRANSACTION;
    SELECT 'Lookup tables population script executed successfully.' AS Result;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    DECLARE @ErrorLine INT = ERROR_LINE();
    DECLARE @ErrorProc NVARCHAR(128) = ERROR_PROCEDURE();

    PRINT 'Error occurred at Line ' + CAST(ISNULL(@ErrorLine, 0) AS NVARCHAR(10)) + ' in Procedure ' + ISNULL(@ErrorProc, 'N/A') + ': ' + @ErrorMessage;
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
