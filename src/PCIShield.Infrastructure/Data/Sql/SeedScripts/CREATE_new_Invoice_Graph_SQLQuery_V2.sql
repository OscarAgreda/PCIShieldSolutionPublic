USE PCIShield_Core_Db
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Create GUIDs for primary entities
    DECLARE @UserId UNIQUEIDENTIFIER = '2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0'; -- Using the same User ID from Customer script
    DECLARE @InvoiceId UNIQUEIDENTIFIER = NEWID();
    DECLARE @CustomerId UNIQUEIDENTIFIER; -- We'll look this up or create it
    DECLARE @InvoiceCustomerId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ElectronicDocumentId UNIQUEIDENTIFIER = NEWID();
    
    -- Invoice details
    DECLARE @InvoiceDetailId1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceDetailId2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceDetailGroupId UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceAccountReceivableId UNIQUEIDENTIFIER = NEWID();
    
    -- Products
    DECLARE @ProductId1 UNIQUEIDENTIFIER;
    DECLARE @ProductId2 UNIQUEIDENTIFIER;
    
    -- Customer related
    DECLARE @AddressId UNIQUEIDENTIFIER;
    DECLARE @BillingAddressId UNIQUEIDENTIFIER;
    DECLARE @ShippingAddressId UNIQUEIDENTIFIER;
    DECLARE @ContactId UNIQUEIDENTIFIER;
    DECLARE @EmailAddressId UNIQUEIDENTIFIER;
    DECLARE @PhoneNumberId UNIQUEIDENTIFIER;
    DECLARE @DocIdentificationId UNIQUEIDENTIFIER;
    
    -- System Types
    DECLARE @InvoiceSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @ProductSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @TaxSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @TaxSystemTypeVATId UNIQUEIDENTIFIER;
    DECLARE @AddressSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @AddressSystemTypeBillingId UNIQUEIDENTIFIER;
    DECLARE @AddressSystemTypeShippingId UNIQUEIDENTIFIER;
    DECLARE @ContactSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @EmailSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @PhoneSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @DocSystemTypeId UNIQUEIDENTIFIER;
    DECLARE @MeasurementUnitTypeId UNIQUEIDENTIFIER;
    DECLARE @PriceUserTypeId UNIQUEIDENTIFIER;

    -- Location and organization
    DECLARE @BoundedContextId UNIQUEIDENTIFIER;
    DECLARE @ContinentId UNIQUEIDENTIFIER;
    DECLARE @CountryId UNIQUEIDENTIFIER;
    DECLARE @StateId UNIQUEIDENTIFIER;
    DECLARE @CityId UNIQUEIDENTIFIER;
    DECLARE @CountyId UNIQUEIDENTIFIER;
    DECLARE @DistrictId UNIQUEIDENTIFIER;
    DECLARE @CurrencyId UNIQUEIDENTIFIER;
    DECLARE @SalespersonId UNIQUEIDENTIFIER;
    DECLARE @EmployeeId UNIQUEIDENTIFIER;
    DECLARE @ApplicationUserId UNIQUEIDENTIFIER = '2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0';

    -- Find a valid BoundedContext ID - CRITICAL FOR MANY DEPENDENCIES
    SELECT TOP 1 @BoundedContextId = BoundedContextId FROM BoundedContext WHERE IsActive = 1;
    
    IF @BoundedContextId IS NULL
    BEGIN
        RAISERROR('No active BoundedContext found. Cannot proceed.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Using BoundedContextId: ' + CAST(@BoundedContextId AS NVARCHAR(36));

    -- STEP 1: Retrieve or create location hierarchy (Continent, Country, State, City, etc.)
    -- Check for existing ContinentId
    SELECT TOP 1 @ContinentId = ContinentId FROM Continent WHERE ContinentName = 'North America';
    
    IF @ContinentId IS NULL
    BEGIN
        SET @ContinentId = NEWID();
        INSERT INTO Continent (ContinentId, ContinentName)
        VALUES (@ContinentId, 'North America');
        PRINT 'Created new Continent with ID: ' + CAST(@ContinentId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Continent with ID: ' + CAST(@ContinentId AS NVARCHAR(36));
    END

    -- Check for existing CurrencyId for USD
    SELECT TOP 1 @CurrencyId = CurrencyId FROM Currency WHERE CurrencyCodeISO = 'USD';
    
    IF @CurrencyId IS NULL
    BEGIN
        SET @CurrencyId = NEWID();
        INSERT INTO Currency (CurrencyId, CurrencyName, CurrencySymbol, CurrencyCodeISO, CurrencyIdISO, 
                             CurrencyDecimalPlaces, CurrencyDecimalSeparator, IsDeleted)
        VALUES (@CurrencyId, 'US Dollar', '$', 'USD', 840, 2, '.', 0);
        PRINT 'Created new Currency with ID: ' + CAST(@CurrencyId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Currency with ID: ' + CAST(@CurrencyId AS NVARCHAR(36));
    END

    -- Check if Country already exists
    SELECT TOP 1 @CountryId = CountryId FROM Country WHERE CountryCodeISO2 = 'US';
    
    IF @CountryId IS NULL
    BEGIN
        SET @CountryId = NEWID();
        INSERT INTO Country (CountryId, CountryName, CountryCodeISO2, CountryCodeISO3, CountryIdISO, 
                           CountryAreaCode, ContinentId, SubcontinentId, CurrencyId, IsActive)
        VALUES (@CountryId, 'United States', 'US', 'USA', 840, '1', @ContinentId, NULL, @CurrencyId, 1);
        PRINT 'Created new Country with ID: ' + CAST(@CountryId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Country with ID: ' + CAST(@CountryId AS NVARCHAR(36));
    END

    -- Check if State already exists
    SELECT TOP 1 @StateId = StateId FROM State WHERE StateCode = 'CA' AND CountryId = @CountryId;
    
    IF @StateId IS NULL
    BEGIN
        SET @StateId = NEWID();
        INSERT INTO State (StateId, StateCode, StateName, CountryId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@StateId, 'CA', 'California', @CountryId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new State with ID: ' + CAST(@StateId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing State with ID: ' + CAST(@StateId AS NVARCHAR(36));
    END

    -- Check if City already exists
    SELECT TOP 1 @CityId = CityId FROM City WHERE CityName = 'San Francisco' AND StateId = @StateId;
    
    IF @CityId IS NULL
    BEGIN
        SET @CityId = NEWID();
        INSERT INTO City (CityId, CityName, StateId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CityId, 'San Francisco', @StateId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new City with ID: ' + CAST(@CityId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing City with ID: ' + CAST(@CityId AS NVARCHAR(36));
    END

    -- Check if County already exists
    SELECT TOP 1 @CountyId = CountyId FROM County WHERE CountyName = 'San Francisco County' AND StateId = @StateId;
    
    IF @CountyId IS NULL
    BEGIN
        SET @CountyId = NEWID();
        INSERT INTO County (CountyId, CountyCode, CountyName, CountyPostalCode, StateId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CountyId, 'SFO', 'San Francisco County', '94105', @StateId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new County with ID: ' + CAST(@CountyId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing County with ID: ' + CAST(@CountyId AS NVARCHAR(36));
    END

    -- Check if District already exists
    SELECT TOP 1 @DistrictId = DistrictId FROM District WHERE DistrictName = 'Financial District' AND CountyId = @CountyId;
    
    IF @DistrictId IS NULL
    BEGIN
        SET @DistrictId = NEWID();
        INSERT INTO District (DistrictId, DistrictCode, DistrictName, CountyId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@DistrictId, 'FIN', 'Financial District', @CountyId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new District with ID: ' + CAST(@DistrictId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing District with ID: ' + CAST(@DistrictId AS NVARCHAR(36));
    END

    -- STEP 2: Retrieve or create system types
    -- Check for existing InvoiceSystemType
    SELECT TOP 1 @InvoiceSystemTypeId = InvoiceSystemTypeId 
    FROM InvoiceSystemType 
    WHERE InvoiceSystemTypeCode = 'STDINV';
    
    IF @InvoiceSystemTypeId IS NULL
    BEGIN
        SET @InvoiceSystemTypeId = NEWID();
        INSERT INTO InvoiceSystemType (InvoiceSystemTypeId, InvoiceSystemTypeCode, InvoiceSystemTypeName, 
                                      InvoiceSystemTypeIsSale, InvoiceSystemTypeSign, 
                                      CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@InvoiceSystemTypeId, 'STDINV', 'Standard Invoice', 1, 1, 
               GETDATE(), @UserId, 1, 0);
        PRINT 'Created new InvoiceSystemType with ID: ' + CAST(@InvoiceSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing InvoiceSystemType with ID: ' + CAST(@InvoiceSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing TaxSystemType for VAT
    SELECT TOP 1 @TaxSystemTypeVATId = TaxSystemTypeId 
    FROM TaxSystemType 
    WHERE TaxSystemTypeCode = 'VAT';
    
    IF @TaxSystemTypeVATId IS NULL
    BEGIN
        SET @TaxSystemTypeVATId = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, 
                                 TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue,
                                 BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSystemTypeVATId, 'VAT', 'Value Added Tax', 
               0.13, 1, 0, 
               @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Created new TaxSystemType VAT with ID: ' + CAST(@TaxSystemTypeVATId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing TaxSystemType VAT with ID: ' + CAST(@TaxSystemTypeVATId AS NVARCHAR(36));
    END
    
    -- Check for existing TaxSystemType for EXEMPT
    SELECT TOP 1 @TaxSystemTypeId = TaxSystemTypeId 
    FROM TaxSystemType 
    WHERE TaxSystemTypeCode = 'EXEMPT';
    
    IF @TaxSystemTypeId IS NULL
    BEGIN
        SET @TaxSystemTypeId = NEWID();
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, 
                                 TaxSystemTypeRate, TaxSystemTypeSign, TaxSystemTypeMinimumTaxableValue,
                                 BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSystemTypeId, 'EXEMPT', 'Tax Exempt', 
               0.00, 0, 0, 
               @BoundedContextId, @CountryId, 1, 0);
        PRINT 'Created new TaxSystemType EXEMPT with ID: ' + CAST(@TaxSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing TaxSystemType EXEMPT with ID: ' + CAST(@TaxSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing AddressSystemType types
    SELECT TOP 1 @AddressSystemTypeId = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'PRI';
    
    IF @AddressSystemTypeId IS NULL
    BEGIN
        SET @AddressSystemTypeId = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, 
                                      BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeId, 'PRI', 'Primary', 
               @BoundedContextId, 1);
        PRINT 'Created new AddressSystemType PRI with ID: ' + CAST(@AddressSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing AddressSystemType PRI with ID: ' + CAST(@AddressSystemTypeId AS NVARCHAR(36));
    END
    
    SELECT TOP 1 @AddressSystemTypeBillingId = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'BIL';
    
    IF @AddressSystemTypeBillingId IS NULL
    BEGIN
        SET @AddressSystemTypeBillingId = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, 
                                      BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeBillingId, 'BIL', 'Billing', 
               @BoundedContextId, 1);
        PRINT 'Created new AddressSystemType BIL with ID: ' + CAST(@AddressSystemTypeBillingId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing AddressSystemType BIL with ID: ' + CAST(@AddressSystemTypeBillingId AS NVARCHAR(36));
    END
    
    SELECT TOP 1 @AddressSystemTypeShippingId = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'DEL';
    
    IF @AddressSystemTypeShippingId IS NULL
    BEGIN
        SET @AddressSystemTypeShippingId = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, 
                                      BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeShippingId, 'DEL', 'Delivery', 
               @BoundedContextId, 1);
        PRINT 'Created new AddressSystemType DEL with ID: ' + CAST(@AddressSystemTypeShippingId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing AddressSystemType DEL with ID: ' + CAST(@AddressSystemTypeShippingId AS NVARCHAR(36));
    END

    -- Check for existing ContactSystemType
    SELECT TOP 1 @ContactSystemTypeId = ContactSystemTypeId 
    FROM ContactSystemType 
    WHERE ContactSystemTypeCode = 'BILLING';
    
    IF @ContactSystemTypeId IS NULL
    BEGIN
        SET @ContactSystemTypeId = NEWID();
        INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, 
                                      BoundedContextId, IsActive, IsDeleted)
        VALUES (@ContactSystemTypeId, 'BILLING', 'Billing Contact', 
               @BoundedContextId, 1, 0);
        PRINT 'Created new ContactSystemType with ID: ' + CAST(@ContactSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing ContactSystemType with ID: ' + CAST(@ContactSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing EmailAddressSystemType
    SELECT TOP 1 @EmailSystemTypeId = EmailAddressSystemTypeId 
    FROM EmailAddressSystemType 
    WHERE EmailAddressSystemTypeCode = 'BILLING';
    
    IF @EmailSystemTypeId IS NULL
    BEGIN
        SET @EmailSystemTypeId = NEWID();
        INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, 
                                          EmailAddressSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@EmailSystemTypeId, 'BILLING', 'Billing Email', 
               @BoundedContextId, 1, 0);
        PRINT 'Created new EmailAddressSystemType with ID: ' + CAST(@EmailSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing EmailAddressSystemType with ID: ' + CAST(@EmailSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing PhoneNumberSystemType
    SELECT TOP 1 @PhoneSystemTypeId = PhoneNumberSystemTypeId 
    FROM PhoneNumberSystemType 
    WHERE PhoneNumberSystemTypeCode = 'BUSINESS';
    
    IF @PhoneSystemTypeId IS NULL
    BEGIN
        SET @PhoneSystemTypeId = NEWID();
        INSERT INTO PhoneNumberSystemType (PhoneNumberSystemTypeId, PhoneNumberSystemTypeCode, 
                                         PhoneNumberSystemTypeName, BoundedContextId, IsActive, IsDeleted)
        VALUES (@PhoneSystemTypeId, 'BUS', 'Business Phone', 
               @BoundedContextId, 1, 0);
        PRINT 'Created new PhoneNumberSystemType with ID: ' + CAST(@PhoneSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing PhoneNumberSystemType with ID: ' + CAST(@PhoneSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing DocumentIdentificationSystemType
    SELECT TOP 1 @DocSystemTypeId = DocumentIdentificationSystemTypeId 
    FROM DocumentIdentificationSystemType 
    WHERE DocumentIdentificationSystemTypeCode = 'NIT';
    
    IF @DocSystemTypeId IS NULL
    BEGIN
        SET @DocSystemTypeId = NEWID();
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, 
                                                    DocumentIdentificationSystemTypeName, CountryId, BoundedContextId, 
                                                    IsActive, IsDeleted)
        VALUES (@DocSystemTypeId, 'NIT', 'Tax ID', 
               @CountryId, @BoundedContextId, 
               1, 0);
        PRINT 'Created new DocumentIdentificationSystemType with ID: ' + CAST(@DocSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing DocumentIdentificationSystemType with ID: ' + CAST(@DocSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing MeasurementUnitSystemType
    SELECT TOP 1 @MeasurementUnitTypeId = MeasurementUnitSystemTypeId 
    FROM MeasurementUnitSystemType 
    WHERE MeasurementUnitSystemTypeCode = 'EACH';
    
    IF @MeasurementUnitTypeId IS NULL
    BEGIN
        SET @MeasurementUnitTypeId = NEWID();
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, 
                                             MeasurementUnitSystemTypeName, MeasurementUnitSystemTypeAbbreviation, 
                                             BoundedContextId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@MeasurementUnitTypeId, 'EACH', 'Each', 'ea', 
               @BoundedContextId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new MeasurementUnitSystemType with ID: ' + CAST(@MeasurementUnitTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing MeasurementUnitSystemType with ID: ' + CAST(@MeasurementUnitTypeId AS NVARCHAR(36));
    END

    -- Check for existing ProductSystemType
    SELECT TOP 1 @ProductSystemTypeId = ProductSystemTypeId 
    FROM ProductSystemType 
    WHERE ProductSystemTypeCode = 'PROD';
    
    IF @ProductSystemTypeId IS NULL
    BEGIN
        SET @ProductSystemTypeId = NEWID();
        INSERT INTO ProductSystemType (ProductSystemTypeId, ProductSystemTypeCode, ProductSystemTypeName, 
                                     Description, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@ProductSystemTypeId, 'PROD', 'Product', 
               'Standard product', GETDATE(), @UserId, 1, 0);
        PRINT 'Created new ProductSystemType with ID: ' + CAST(@ProductSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing ProductSystemType with ID: ' + CAST(@ProductSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing PriceUserType
    SELECT TOP 1 @PriceUserTypeId = PriceUserTypeId 
    FROM PriceUserType 
    WHERE PriceUserTypeCode = 'RETAIL';
    
    IF @PriceUserTypeId IS NULL
    BEGIN
        SET @PriceUserTypeId = NEWID();
        INSERT INTO PriceUserType (PriceUserTypeId, PriceUserTypeCode, PriceUserTypeName, 
                                 Description, IsMinimumPrice, IsGlobalPrice, IsDiscountedInPrice, 
                                 CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@PriceUserTypeId, 'RETAIL', 'Retail Price', 
               'Standard retail price', 0, 1, 0, 
               GETDATE(), @UserId, 1, 0);
        PRINT 'Created new PriceUserType with ID: ' + CAST(@PriceUserTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing PriceUserType with ID: ' + CAST(@PriceUserTypeId AS NVARCHAR(36));
    END

    -- STEP 3: Create or retrieve a customer for the invoice
    -- Lookup existing customer or create a new one
    SELECT TOP 1 @CustomerId = CustomerId
    FROM Customer
    WHERE CustomerCode = 'CUST002' AND IsActive = 1 AND IsDeleted = 0;
    
    DECLARE @CustomerUserTypeId UNIQUEIDENTIFIER;
    DECLARE @TaxpayerSystemTypeId UNIQUEIDENTIFIER;
    
    -- Get needed IDs for customer creation if we need to create one
    IF @CustomerId IS NULL
    BEGIN
        -- Get CustomerUserType
        SELECT TOP 1 @CustomerUserTypeId = CustomerUserTypeId 
        FROM CustomerUserType 
        WHERE CustomerUserTypeCode = 'STND' AND IsActive = 1;
        
        IF @CustomerUserTypeId IS NULL
        BEGIN
            SET @CustomerUserTypeId = NEWID();
            INSERT INTO CustomerUserType (CustomerUserTypeId, CustomerUserTypeCode, CustomerUserTypeName, 
                                        BoundedContextId, IsActive, IsDeleted)
            VALUES (@CustomerUserTypeId, 'STND', 'Standard Customer', 
                   @BoundedContextId, 1, 0);
            PRINT 'Created new CustomerUserType with ID: ' + CAST(@CustomerUserTypeId AS NVARCHAR(36));
        END
        
        -- Get TaxpayerSystemType
        SELECT TOP 1 @TaxpayerSystemTypeId = TaxpayerSystemTypeId 
        FROM TaxpayerSystemType 
        WHERE TaxpayerSystemTypeCode = 'STD';
        
        IF @TaxpayerSystemTypeId IS NULL
        BEGIN
            SET @TaxpayerSystemTypeId = NEWID();
            INSERT INTO TaxpayerSystemType (TaxpayerSystemTypeId, TaxpayerSystemTypeCode, TaxpayerSystemTypeName, 
                                          TaxpayerSystemTypeValue, CountryId, BoundedContextId, IsActive, IsDeleted)
            VALUES (@TaxpayerSystemTypeId, 'STD', 'Standard Taxpayer', 
                   1, @CountryId, @BoundedContextId, 1, 0);
            PRINT 'Created new TaxpayerSystemType with ID: ' + CAST(@TaxpayerSystemTypeId AS NVARCHAR(36));
        END
        
        -- Create a new customer
        SET @CustomerId = NEWID();
        INSERT INTO Customer (CustomerId, CustomerCode, CustomerIsPerson, CustomerFirstName, CustomerLastName, 
                            CustomerCommercialName, TaxpayerSystemTypeId, CustomerCreditTermDays, 
                            CustomerCreditLimitAmount, CustomerIsReseller, CustomerStatus, CustomerUserTypeId, 
                            CustomerIsForeign, CustomerPrimaryCountryId, ApplicationUserId,
                            CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CustomerId, 'CUST002', 0, 'Sarah', 'Johnson', 
               'Johnson Enterprises', @TaxpayerSystemTypeId, 30, 
               15000.00, 0, 1, @CustomerUserTypeId, 
               0, @CountryId, @ApplicationUserId,
               GETDATE(), @UserId, 1, 0);
        PRINT 'Created new Customer with ID: ' + CAST(@CustomerId AS NVARCHAR(36));
        
        -- Create and link customer address
        SET @AddressId = NEWID();
        INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, 
                           CityId, DistrictId, CountyId, StateId, CountryId, 
                           CreatedDate, CreatedBy, IsDeleted)
        VALUES (@AddressId, '456 Market St', 'Suite 300', '94107', 
               @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
               GETDATE(), @UserId, 0);
        
        INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, 
                                   IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@AddressId, @AddressSystemTypeId, @CustomerId, 
               1, GETDATE(), @UserId, 0);
        
        -- Create separate billing address
        SET @BillingAddressId = NEWID();
        INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, 
                           CityId, DistrictId, CountyId, StateId, CountryId, 
                           CreatedDate, CreatedBy, IsDeleted)
        VALUES (@BillingAddressId, '789 Finance Ave', 'Floor 12', '94108', 
               @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
               GETDATE(), @UserId, 0);
        
        INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, 
                                   IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@BillingAddressId, @AddressSystemTypeBillingId, @CustomerId, 
               0, GETDATE(), @UserId, 0);
        
        -- Create separate shipping address
        SET @ShippingAddressId = NEWID();
        INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, 
                           CityId, DistrictId, CountyId, StateId, CountryId, 
                           CreatedDate, CreatedBy, IsDeleted)
        VALUES (@ShippingAddressId, '123 Warehouse Blvd', 'Dock 7', '94110', 
               @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
               GETDATE(), @UserId, 0);
        
        INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, 
                                   IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@ShippingAddressId, @AddressSystemTypeShippingId, @CustomerId, 
               0, GETDATE(), @UserId, 0);
    END
    ELSE
    BEGIN
        PRINT 'Using existing Customer with ID: ' + CAST(@CustomerId AS NVARCHAR(36));
        
        -- Get existing addresses for this customer
        SELECT TOP 1 @AddressId = a.AddressId
        FROM CustomerAddress ca
        JOIN Address a ON ca.AddressId = a.AddressId
        WHERE ca.CustomerId = @CustomerId AND ca.IsPrimaryAddress = 1;
        
        SELECT TOP 1 @BillingAddressId = a.AddressId
        FROM CustomerAddress ca
        JOIN Address a ON ca.AddressId = a.AddressId
        JOIN AddressSystemType ast ON ca.AddressSystemTypeId = ast.AddressSystemTypeId
        WHERE ca.CustomerId = @CustomerId AND ast.AddressSystemTypeCode = 'BIL';
        
        SELECT TOP 1 @ShippingAddressId = a.AddressId
        FROM CustomerAddress ca
        JOIN Address a ON ca.AddressId = a.AddressId
        JOIN AddressSystemType ast ON ca.AddressSystemTypeId = ast.AddressSystemTypeId
        WHERE ca.CustomerId = @CustomerId AND ast.AddressSystemTypeCode = 'DEL';
        
        -- If no billing address, create one
        IF @BillingAddressId IS NULL
        BEGIN
            SET @BillingAddressId = NEWID();
            INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, 
                               CityId, DistrictId, CountyId, StateId, CountryId, 
                               CreatedDate, CreatedBy, IsDeleted)
            VALUES (@BillingAddressId, '789 Finance Ave', 'Floor 12', '94108', 
                   @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
                   GETDATE(), @UserId, 0);
            
            INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, 
                                       IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@BillingAddressId, @AddressSystemTypeBillingId, @CustomerId, 
                   0, GETDATE(), @UserId, 0);
        END
        
        -- If no shipping address, create one
        IF @ShippingAddressId IS NULL
        BEGIN
            SET @ShippingAddressId = NEWID();
            INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, 
                               CityId, DistrictId, CountyId, StateId, CountryId, 
                               CreatedDate, CreatedBy, IsDeleted)
            VALUES (@ShippingAddressId, '123 Warehouse Blvd', 'Dock 7', '94110', 
                   @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
                   GETDATE(), @UserId, 0);
            
            INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, 
                                       IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@ShippingAddressId, @AddressSystemTypeShippingId, @CustomerId, 
                   0, GETDATE(), @UserId, 0);
        END
    END

    -- STEP 4: Create or retrieve contact information
    -- Create contact if needed
    SELECT TOP 1 @ContactId = ContactId 
    FROM Contact 
    WHERE FirstName = 'Sarah' AND LastName = 'Johnson' AND Email = 'sarah.johnson@example.com';
    
    IF @ContactId IS NULL
    BEGIN
        -- Insert Contact
        SET @ContactId = NEWID();
        INSERT INTO Contact (ContactId, FirstName, LastName, Email, Phone, Mobile, 
                          IsPrimaryContact, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@ContactId, 'Sarah', 'Johnson', 'sarah.johnson@example.com', '555-234-5678', '555-876-5432', 
              1, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Contact with ID: ' + CAST(@ContactId AS NVARCHAR(36));
        
        -- Link to customer if not already linked
        IF NOT EXISTS (SELECT 1 FROM CustomerContact WHERE CustomerId = @CustomerId AND ContactId = @ContactId)
        BEGIN
            INSERT INTO CustomerContact (CustomerId, ContactId, ContactSystemTypeId, 
                                      IsPrimaryContact, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@CustomerId, @ContactId, @ContactSystemTypeId, 
                  1, GETDATE(), @UserId, 0);
        END
    END
    ELSE
    BEGIN
        PRINT 'Using existing Contact with ID: ' + CAST(@ContactId AS NVARCHAR(36));
    END

    -- Create email address if needed
    SELECT TOP 1 @EmailAddressId = EmailAddressId 
    FROM EmailAddress 
    WHERE EmailAddressString = 'billing@johnsonenterprises.com';
    
    IF @EmailAddressId IS NULL
    BEGIN
        -- Insert EmailAddress
        SET @EmailAddressId = NEWID();
        INSERT INTO EmailAddress (EmailAddressId, EmailAddressString, IsDeleted)
        VALUES (@EmailAddressId, 'billing@johnsonenterprises.com', 0);
        
        PRINT 'Created new EmailAddress with ID: ' + CAST(@EmailAddressId AS NVARCHAR(36));
        
        -- Link to customer if not already linked
        IF NOT EXISTS (SELECT 1 FROM CustomerEmailAddress WHERE CustomerId = @CustomerId AND EmailAddressId = @EmailAddressId)
        BEGIN
            INSERT INTO CustomerEmailAddress (CustomerId, EmailAddressId, EmailAddressSystemTypeId, 
                                           IsPrimaryEmailAddress, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@CustomerId, @EmailAddressId, @EmailSystemTypeId, 
                  1, GETDATE(), @UserId, 0);
        END
    END
    ELSE
    BEGIN
        PRINT 'Using existing EmailAddress with ID: ' + CAST(@EmailAddressId AS NVARCHAR(36));
    END

    -- Create phone number if needed
    SELECT TOP 1 @PhoneNumberId = PhoneNumberId 
    FROM PhoneNumber 
    WHERE PhoneNumberString = '555-987-6543';
    
    IF @PhoneNumberId IS NULL
    BEGIN
        -- Insert PhoneNumber
        SET @PhoneNumberId = NEWID();
        INSERT INTO PhoneNumber (PhoneNumberId, PhoneNumberString, IsDeleted)
        VALUES (@PhoneNumberId, '555-987-6543', 0);
        
        PRINT 'Created new PhoneNumber with ID: ' + CAST(@PhoneNumberId AS NVARCHAR(36));
        
        -- Link to customer if not already linked
        IF NOT EXISTS (SELECT 1 FROM CustomerPhoneNumber WHERE CustomerId = @CustomerId AND PhoneNumberId = @PhoneNumberId)
        BEGIN
            INSERT INTO CustomerPhoneNumber (CustomerId, PhoneNumberId, PhoneNumberSystemTypeId, 
                                          IsPrimaryPhoneNumber, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@CustomerId, @PhoneNumberId, @PhoneSystemTypeId, 
                  1, GETDATE(), @UserId, 0);
        END
    END
    ELSE
    BEGIN
        PRINT 'Using existing PhoneNumber with ID: ' + CAST(@PhoneNumberId AS NVARCHAR(36));
    END

    -- Create document identification if needed
    SELECT TOP 1 @DocIdentificationId = DocumentIdentificationId 
    FROM DocumentIdentification 
    WHERE DocumentIdentificationNumber = '987654321-0';
    
    IF @DocIdentificationId IS NULL
    BEGIN
        -- Insert DocumentIdentification
        SET @DocIdentificationId = NEWID();
        INSERT INTO DocumentIdentification (DocumentIdentificationId, DocumentIdentificationNumber, IsDeleted)
        VALUES (@DocIdentificationId, '987654321-0', 0);
        
        PRINT 'Created new DocumentIdentification with ID: ' + CAST(@DocIdentificationId AS NVARCHAR(36));
        
        -- Link to customer if not already linked
        IF NOT EXISTS (SELECT 1 FROM CustomerDocumentIdentification WHERE CustomerId = @CustomerId AND DocumentIdentificationId = @DocIdentificationId)
        BEGIN
            INSERT INTO CustomerDocumentIdentification (CustomerId, DocumentIdentificationId, DocumentIdentificationSystemTypeId, 
                                                     DocumentIdentificationVerified, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@CustomerId, @DocIdentificationId, @DocSystemTypeId, 
                  GETDATE(), GETDATE(), @UserId, 0);
        END
    END
    ELSE
    BEGIN
        PRINT 'Using existing DocumentIdentification with ID: ' + CAST(@DocIdentificationId AS NVARCHAR(36));
    END

    -- STEP 5: Create or retrieve products
    -- Create first product if needed
    SELECT TOP 1 @ProductId1 = ProductId 
    FROM Product 
    WHERE ProductCode = 'PROD001';
    
    IF @ProductId1 IS NULL
    BEGIN
        -- Insert Product 1
        SET @ProductId1 = NEWID();
        INSERT INTO Product (ProductId, ProductCode, ProductName, Description, UnitPrice, 
                          ProductSystemTypeId, PrimaryMeasurementUnitSystemTypeId, 
                          CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@ProductId1, 'PROD001', 'Standard Widget', 'Standard widget model', 100.00, 
              @ProductSystemTypeId, @MeasurementUnitTypeId, 
              GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Product 1 with ID: ' + CAST(@ProductId1 AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Product 1 with ID: ' + CAST(@ProductId1 AS NVARCHAR(36));
    END
    
    -- Create second product if needed
    SELECT TOP 1 @ProductId2 = ProductId 
    FROM Product 
    WHERE ProductCode = 'PROD002';
    
    IF @ProductId2 IS NULL
    BEGIN
        -- Insert Product 2
        SET @ProductId2 = NEWID();
        INSERT INTO Product (ProductId, ProductCode, ProductName, Description, UnitPrice, 
                          ProductSystemTypeId, PrimaryMeasurementUnitSystemTypeId, 
                          CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@ProductId2, 'PROD002', 'Premium Widget', 'Premium widget model with extra features', 250.00, 
              @ProductSystemTypeId, @MeasurementUnitTypeId, 
              GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Product 2 with ID: ' + CAST(@ProductId2 AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Product 2 with ID: ' + CAST(@ProductId2 AS NVARCHAR(36));
    END

    -- STEP 6: Create or retrieve salesperson
    -- First get or create an employee for the salesperson
    SELECT TOP 1 @EmployeeId = EmployeeId 
    FROM Employee 
    WHERE ApplicationUserId = @ApplicationUserId;
    
    IF @EmployeeId IS NULL
    BEGIN
        -- Insert Employee
        SET @EmployeeId = NEWID();
        INSERT INTO Employee (EmployeeId, EmployeeCode, EmployeeFirstName, EmployeeLastName, 
                           ApplicationUserId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@EmployeeId, 'EMP001', 'Michael', 'Johnson', 
               @ApplicationUserId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Employee with ID: ' + CAST(@EmployeeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Employee with ID: ' + CAST(@EmployeeId AS NVARCHAR(36));
    END
    
    -- Now get or create the salesperson
    SELECT TOP 1 @SalespersonId = SalespersonId 
    FROM Salesperson 
    WHERE EmployeeId = @EmployeeId;
    
    IF @SalespersonId IS NULL
    BEGIN
        -- Insert Salesperson
        SET @SalespersonId = NEWID();
        INSERT INTO Salesperson (SalespersonId, SalespersonCode, SalespersonFirstName, SalespersonLastName, 
                              EmployeeId, ApplicationUserId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@SalespersonId, 'SP001', 'Michael', 'Johnson', 
              @EmployeeId, @ApplicationUserId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Salesperson with ID: ' + CAST(@SalespersonId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Salesperson with ID: ' + CAST(@SalespersonId AS NVARCHAR(36));
    END

    -- STEP 7: Create Invoice and related entities
    -- Check if this invoice already exists
    DECLARE @ExistingInvoiceId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingInvoiceId = InvoiceId 
    FROM Invoice 
    WHERE CustomerId = @CustomerId AND InvoiceSystemTypeId = @InvoiceSystemTypeId 
        AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE) 
        AND IsDraft = 0 AND IsVoided = 0;
    
    IF @ExistingInvoiceId IS NULL
    BEGIN
        -- Create a new invoice
        INSERT INTO Invoice (InvoiceId, InvoiceSystemTypeId, InvoiceDate, CustomerId, 
                          IsDraft, IsVoided, InvoiceStatus, InvoiceForeignCurrencyId, InvoiceForeignCurrencyRate, 
                          CreatedDate, CreatedBy, IsDeleted, InvoiceNumber)
        VALUES (@InvoiceId, @InvoiceSystemTypeId, GETDATE(), @CustomerId, 
              0, 0, 1, NULL, NULL, 
              GETDATE(), @UserId, 0,'0001');
        
        PRINT 'Created new Invoice with ID: ' + CAST(@InvoiceId AS NVARCHAR(36));
        
        -- Create invoice customer record (snapshot of customer at time of invoice)
        INSERT INTO InvoiceCustomer (InvoiceCustomerId, InvoiceId, CustomerId, InvoiceCustomerUserType, 
                                  InvoiceCustomerFirstName, InvoiceCustomerLastName, InvoiceCustomerCommercialName, 
                                  CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceCustomerId, @InvoiceId, @CustomerId, 1, 
              'Sarah', 'Johnson', 'Johnson Enterprises', 
              GETDATE(), @UserId, 0);
        
        -- Link invoice customer to addresses
        IF @BillingAddressId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceCustomerAddress (InvoiceCustomerId, AddressId, AddressSystemTypeId, 
                                            InvoiceCustomerAddressSystemType, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceCustomerId, @BillingAddressId, @AddressSystemTypeBillingId, 
                  1, GETDATE(), @UserId, 0);
        END
        
        IF @ShippingAddressId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceCustomerAddress (InvoiceCustomerId, AddressId, AddressSystemTypeId, 
                                            InvoiceCustomerAddressSystemType, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceCustomerId, @ShippingAddressId, @AddressSystemTypeShippingId, 
                  2, GETDATE(), @UserId, 0);
        END
        
        -- Link invoice customer to document identification
        IF @DocIdentificationId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceCustomerDocumentIdentification (InvoiceCustomerId, DocumentIdentificationId, 
                                                           DocumentIdentificationSystemTypeId, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceCustomerId, @DocIdentificationId, 
                  @DocSystemTypeId, GETDATE(), @UserId, 0);
        END
        
        -- Link invoice customer to email address
        IF @EmailAddressId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceCustomerEmailAddress (InvoiceCustomerId, EmailAddressId, EmailAddressSystemTypeId, 
                                                 IsPrimaryEmailAddress, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceCustomerId, @EmailAddressId, @EmailSystemTypeId, 
                  1, GETDATE(), @UserId, 0);
        END
        
        -- Link invoice customer to phone number
        IF @PhoneNumberId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceCustomerPhoneNumber (InvoiceCustomerId, PhoneNumberId, PhoneNumberSystemTypeId, 
                                                IsPrimaryPhoneNumber, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceCustomerId, @PhoneNumberId, @PhoneSystemTypeId, 
                  1, GETDATE(), @UserId, 0);
        END
        
        -- Link invoice to contact
        IF @ContactId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceContact (InvoiceId, ContactId, ContactSystemTypeId, 
                                     IsPrimaryContact, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceId, @ContactId, @ContactSystemTypeId, 
                  1, GETDATE(), @UserId, 0);
        END
        
        -- Link invoice to salesperson
        IF @SalespersonId IS NOT NULL
        BEGIN
            INSERT INTO InvoiceSalesperson (InvoiceId, SalespersonId, IsPrimarySalesperson, 
                                        CommissionPercent, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@InvoiceId, @SalespersonId, 1, 
                  0.05, GETDATE(), @UserId, 0);
        END
        
        -- Create invoice detail group
        INSERT INTO InvoiceDetailGroup (InvoiceDetailGroupId, InvoiceId, InvoiceDetailGroupCode, 
                                     InvoiceDetailGroupDescription, InvoiceDetailGroupQuantity, 
                                     InvoiceDetailGroupUnitPrice, InvoiceDetailGroupOrder, 
                                     InvoiceDetailGroupPrintDetails, InvoiceDetailGroupUnitPriceForeignCurrency, 
                                     CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailGroupId, @InvoiceId, 'GRP001', 
               'Premium Bundle', 1, 
               350.00, 1, 
               1, NULL, 
               GETDATE(), @UserId, 0);
        
        -- Create invoice details
        -- Invoice detail 1
        INSERT INTO InvoiceDetail (InvoiceDetailId, InvoiceId, InvoiceDetailGroupId, ProductId, 
                               ProductSystemTypeId, PriceUserTypeId, InvoiceDetailLineNumber, 
                               InvoiceDetailDescription, InvoiceDetailQuantity, InvoiceDetailUnitPrice, 
                               InvoiceDetailDiscountPercentage, InvoiceDetailLineTotal, 
                               MeasurementUnitSystemTypeId, InvoiceDetailMeasurementUnitSystemTypeQuantity, 
                               InvoiceDetailMeasurementUnitSystemTypePrice, InvoiceDetailMeasurementUnitSystemTypeName, 
                               InvoiceDetailMeasurementUnitSystemTypeAbbreviation, InvoiceDetailIsHiddenInGroup, 
                               InvoiceDetailUnitPriceForeignCurrency, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailId1, @InvoiceId, @InvoiceDetailGroupId, @ProductId1, 
              @ProductSystemTypeId, @PriceUserTypeId, 1, 
              'Standard Widget', 2, 100.00, 
              0, 200.00, 
              @MeasurementUnitTypeId, 2, 
              100.00, 'Each', 'ea', 0, 
              NULL, GETDATE(), @UserId, 0);
        
        -- Invoice detail 2
        INSERT INTO InvoiceDetail (InvoiceDetailId, InvoiceId, InvoiceDetailGroupId, ProductId, 
                               ProductSystemTypeId, PriceUserTypeId, InvoiceDetailLineNumber, 
                               InvoiceDetailDescription, InvoiceDetailQuantity, InvoiceDetailUnitPrice, 
                               InvoiceDetailDiscountPercentage, InvoiceDetailLineTotal, 
                               MeasurementUnitSystemTypeId, InvoiceDetailMeasurementUnitSystemTypeQuantity, 
                               InvoiceDetailMeasurementUnitSystemTypePrice, InvoiceDetailMeasurementUnitSystemTypeName, 
                               InvoiceDetailMeasurementUnitSystemTypeAbbreviation, InvoiceDetailIsHiddenInGroup, 
                               InvoiceDetailUnitPriceForeignCurrency, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailId2, @InvoiceId, NULL, @ProductId2, 
              @ProductSystemTypeId, @PriceUserTypeId, 2, 
              'Premium Widget', 1, 250.00, 
              0, 250.00, 
              @MeasurementUnitTypeId, 1, 
              250.00, 'Each', 'ea', 0, 
              NULL, GETDATE(), @UserId, 0);
        
        -- Add tax to invoice details
        -- For detail 1
        INSERT INTO InvoiceDetailTaxSystemType (InvoiceDetailId, TaxSystemTypeId, InvoiceDetailTaxSystemTypeAmount, 
                                             InvoiceDetailTaxSystemTypeRate, InvoiceDetailTaxSystemTypeSign, 
                                             CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailId1, @TaxSystemTypeVATId, 26.00, 
               0.13, 1, 
               GETDATE(), @UserId, 0);
        
        -- For detail 2
        INSERT INTO InvoiceDetailTaxSystemType (InvoiceDetailId, TaxSystemTypeId, InvoiceDetailTaxSystemTypeAmount, 
                                             InvoiceDetailTaxSystemTypeRate, InvoiceDetailTaxSystemTypeSign, 
                                             CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailId2, @TaxSystemTypeVATId, 32.50, 
               0.13, 1, 
               GETDATE(), @UserId, 0);
        
        -- Add invoice-level tax
        INSERT INTO InvoiceTaxSystemType (InvoiceId, TaxSystemTypeId, InvoiceTaxSystemTypeRate, 
                                      InvoiceTaxSystemTypeAmount, InvoiceTaxSystemTypeSign, 
                                      CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceId, @TaxSystemTypeVATId, 0.13, 
              58.50, 1, 
              GETDATE(), @UserId, 0);
        
        -- Create invoice account receivable
        INSERT INTO InvoiceAccountReceivable (InvoiceAccountReceivableId, InvoiceId, OriginalBalance, 
                                           CurrentBalance, DueDate, LastPaymentDate, 
                                           IsVoided, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceAccountReceivableId, @InvoiceId, 508.50, 
              508.50, DATEADD(DAY, 30, GETDATE()), NULL, 
              0, GETDATE(), @UserId, 0);
        
        -- Create electronic document
        INSERT INTO ElectronicDocument (ElectronicDocumentId, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@ElectronicDocumentId, GETDATE(), @UserId, 0);
        
        -- Link electronic document to invoice
        INSERT INTO ElectronicDocumentInvoice (ElectronicDocumentId, InvoiceId)
        VALUES (@ElectronicDocumentId, @InvoiceId);
        
        PRINT 'Created complete invoice graph with all related entities';
    END
    ELSE
    BEGIN
        SET @InvoiceId = @ExistingInvoiceId;
        PRINT 'Using existing Invoice with ID: ' + CAST(@InvoiceId AS NVARCHAR(36));
    END

    COMMIT TRANSACTION;
    SELECT 'Script executed successfully. Full invoice graph created.' AS Result;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    DECLARE @ErrorLine INT = ERROR_LINE();
    DECLARE @ErrorProc NVARCHAR(128) = ERROR_PROCEDURE();
    
    PRINT 'Error occurred at Line ' + CAST(@ErrorLine AS NVARCHAR) + ': ' + @ErrorMessage;
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH