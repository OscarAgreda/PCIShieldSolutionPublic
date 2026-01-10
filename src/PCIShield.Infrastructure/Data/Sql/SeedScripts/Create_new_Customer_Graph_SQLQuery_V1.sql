


USE PCIShield_Core_Db
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Create GUIDs for primary entities
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @CustomerId UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceId UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceDetailId UNIQUEIDENTIFIER = NEWID();


	  DECLARE @EmployeeId UNIQUEIDENTIFIER = NEWID();




    DECLARE @CustomerUserTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @TaxpayerSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @CountryId UNIQUEIDENTIFIER = NEWID();
    DECLARE @StateId UNIQUEIDENTIFIER = NEWID();
    DECLARE @CityId UNIQUEIDENTIFIER = NEWID();
    DECLARE @CountyId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DistrictId UNIQUEIDENTIFIER = NEWID();
    DECLARE @AddressId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ContactId UNIQUEIDENTIFIER = NEWID();
    DECLARE @EmailAddressId UNIQUEIDENTIFIER = NEWID();
    DECLARE @PhoneNumberId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DocIdentificationId UNIQUEIDENTIFIER = NEWID();
    DECLARE @SalespersonId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ProductId UNIQUEIDENTIFIER = NEWID();
    DECLARE @EconomicActivityTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @TaxSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @AddressSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ContactSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @PhoneSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @EmailSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DocSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @MeasurementUnitTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ProductSystemTypeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ContinentId UNIQUEIDENTIFIER = NEWID();
    DECLARE @CurrencyId UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceARId UNIQUEIDENTIFIER = NEWID();
    DECLARE @BoundedContextId UNIQUEIDENTIFIER;
    DECLARE @ElectronicDocumentId UNIQUEIDENTIFIER = NEWID();
    DECLARE @InvoiceCustomerId UNIQUEIDENTIFIER = NEWID();
    DECLARE @SubcontinentId UNIQUEIDENTIFIER = NULL;
    DECLARE @ApplicationUserId UNIQUEIDENTIFIER;

    -- Find a valid UserId from an existing entity (like ApplicationUser table)
    SELECT  @UserId = '2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0';
    
   

    -- Find a valid ApplicationUserId for Salesperson
    SELECT  @ApplicationUserId ='2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0';
    
 

    -- Get an existing BoundedContext ID - CRITICAL FOR MANY DEPENDENCIES
    SELECT TOP 1 @BoundedContextId = BoundedContextId FROM BoundedContext WHERE IsActive = 1;
    
    IF @BoundedContextId IS NULL
    BEGIN
        RAISERROR('No active BoundedContext found. Cannot proceed.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Using BoundedContextId: ' + CAST(@BoundedContextId AS NVARCHAR(36));

    -- Check for existing ContinentId
    SELECT TOP 1 @ContinentId = ContinentId FROM Continent WHERE ContinentName = 'North America';
    
    IF @ContinentId IS NULL
    BEGIN
        -- Insert a new Continent
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
        -- Insert a new Currency
        SET @CurrencyId = NEWID();
        
        INSERT INTO Currency (CurrencyId, CurrencyName, CurrencySymbol, CurrencyCodeISO, CurrencyIdISO, CurrencyDecimalPlaces, CurrencyDecimalSeparator, IsDeleted)
        VALUES (@CurrencyId, 'US Dollar', '$', 'USD', 5, 2, ',', 0);
        
        PRINT 'Created new Currency with ID: ' + CAST(@CurrencyId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing Currency with ID: ' + CAST(@CurrencyId AS NVARCHAR(36));
    END

    -- Check if Country already exists to avoid duplicates
    DECLARE @ExistingCountryId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingCountryId = CountryId FROM Country WHERE CountryName = 'United States' OR CountryCodeISO2 = 'US';
    
    IF @ExistingCountryId IS NULL
    BEGIN
        -- Insert Country
        INSERT INTO Country (CountryId, CountryName, CountryCodeISO2, CountryCodeISO3, CountryIdISO, CountryAreaCode, 
                            ContinentId, SubcontinentId, CurrencyId, IsActive)
        VALUES (@CountryId, 'United States', 'US', 'USA', 840, '1', @ContinentId, @SubcontinentId, @CurrencyId, 1);
        
        PRINT 'Created new Country with ID: ' + CAST(@CountryId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @CountryId = @ExistingCountryId;
        PRINT 'Using existing Country with ID: ' + CAST(@CountryId AS NVARCHAR(36));
    END

    -- Check if State already exists
    DECLARE @ExistingStateId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingStateId = StateId FROM State WHERE StateCode = 'CA' AND CountryId = @CountryId;
    
    IF @ExistingStateId IS NULL
    BEGIN
        -- Insert State
        INSERT INTO State (StateId, StateCode, StateName, CountryId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@StateId, 'CA', 'California', @CountryId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new State with ID: ' + CAST(@StateId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @StateId = @ExistingStateId;
        PRINT 'Using existing State with ID: ' + CAST(@StateId AS NVARCHAR(36));
    END

    -- Check if City already exists
    DECLARE @ExistingCityId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingCityId = CityId FROM City WHERE CityName = 'San Francisco' AND StateId = @StateId;
    
    IF @ExistingCityId IS NULL
    BEGIN
        -- Insert City
        INSERT INTO City (CityId, CityName, StateId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CityId, 'San Francisco', @StateId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new City with ID: ' + CAST(@CityId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @CityId = @ExistingCityId;
        PRINT 'Using existing City with ID: ' + CAST(@CityId AS NVARCHAR(36));
    END

    -- Check if County already exists
    DECLARE @ExistingCountyId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingCountyId = CountyId FROM County WHERE CountyName = 'San Francisco County' AND StateId = @StateId;
    
    IF @ExistingCountyId IS NULL
    BEGIN
        -- Insert County
        INSERT INTO County (CountyId, CountyCode, CountyName, CountyPostalCode, StateId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CountyId, 'SFO', 'San Francisco County', '94105', @StateId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new County with ID: ' + CAST(@CountyId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @CountyId = @ExistingCountyId;
        PRINT 'Using existing County with ID: ' + CAST(@CountyId AS NVARCHAR(36));
    END

    -- Check if District already exists
    DECLARE @ExistingDistrictId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingDistrictId = DistrictId FROM District WHERE DistrictName = 'Financial District' AND CountyId = @CountyId;
    
    IF @ExistingDistrictId IS NULL
    BEGIN
        -- Insert District
        INSERT INTO District (DistrictId, DistrictCode, DistrictName, CountyId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@DistrictId, 'D01', 'Financial District', @CountyId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new District with ID: ' + CAST(@DistrictId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @DistrictId = @ExistingDistrictId;
        PRINT 'Using existing District with ID: ' + CAST(@DistrictId AS NVARCHAR(36));
    END

   
   
  -- MODIFIED: Replace your CustomerUserType check with this more robust version
-- Look for this section in your script where you check for existing CustomerUserType
SELECT TOP 1 @CustomerUserTypeId = CustomerUserTypeId 
FROM CustomerUserType 
WHERE CustomerUserTypeCode = 'STND' 
  ;  -- Added conditions

IF @CustomerUserTypeId IS NULL
BEGIN
    SET @CustomerUserTypeId = NEWID();
    
    INSERT INTO CustomerUserType (CustomerUserTypeId, CustomerUserTypeCode, CustomerUserTypeName, BoundedContextId, IsActive, IsDeleted)
    VALUES (@CustomerUserTypeId, 'STND', 'Standard Customer', @BoundedContextId, 1, 0);
    
    PRINT 'Created new CustomerUserType with ID: ' + CAST(@CustomerUserTypeId AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing CustomerUserType with ID: ' + CAST(@CustomerUserTypeId AS NVARCHAR(36));
END

-- NEW: Add this verification block after the CustomerUserType check
-- Make sure CustomerUserType really exists before trying to use it
IF NOT EXISTS (SELECT 1 FROM CustomerUserType WHERE CustomerUserTypeId = @CustomerUserTypeId)
BEGIN
    RAISERROR('CustomerUserType with ID %s does not exist in the database!', 16, 1 );
    ROLLBACK TRANSACTION;
    RETURN;
END


    -- Check for existing TaxpayerSystemType
    SELECT TOP 1 @TaxpayerSystemTypeId = TaxpayerSystemTypeId 
    FROM TaxpayerSystemType 
    WHERE TaxpayerSystemTypeCode = 'STD' ;
    
    IF @TaxpayerSystemTypeId IS NULL
    BEGIN
        -- Insert TaxpayerSystemType
        SET @TaxpayerSystemTypeId = NEWID();
        
        INSERT INTO TaxpayerSystemType (TaxpayerSystemTypeId, TaxpayerSystemTypeCode, TaxpayerSystemTypeName, TaxpayerSystemTypeValue, 
                                      CountryId, BoundedContextId, IsActive, IsDeleted)
        VALUES (@TaxpayerSystemTypeId, 'STD', 'Standard Taxpayer', 1, @CountryId, @BoundedContextId, 1, 0);
        
        PRINT 'Created new TaxpayerSystemType with ID: ' + CAST(@TaxpayerSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing TaxpayerSystemType with ID: ' + CAST(@TaxpayerSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing InvoiceSystemType
    SELECT TOP 1 @InvoiceSystemTypeId = InvoiceSystemTypeId 
    FROM InvoiceSystemType 
    WHERE InvoiceSystemTypeCode = 'STDINV';
    
    IF @InvoiceSystemTypeId IS NULL
    BEGIN
        -- Insert InvoiceSystemType
        SET @InvoiceSystemTypeId = NEWID();
        
        INSERT INTO InvoiceSystemType (InvoiceSystemTypeId, InvoiceSystemTypeCode, InvoiceSystemTypeName, InvoiceSystemTypeIsSale, 
                                    InvoiceSystemTypeSign, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@InvoiceSystemTypeId, 'STDINV', 'Standard Invoice', 1, 1, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new InvoiceSystemType with ID: ' + CAST(@InvoiceSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing InvoiceSystemType with ID: ' + CAST(@InvoiceSystemTypeId AS NVARCHAR(36));
    END

    -- Check for existing TaxSystemType
    SELECT TOP 1 @TaxSystemTypeId = TaxSystemTypeId 
    FROM TaxSystemType 
    WHERE TaxSystemTypeCode = 'EXEMPT';
    
    IF @TaxSystemTypeId IS NULL
    BEGIN
        -- Insert TaxSystemType
        SET @TaxSystemTypeId = NEWID();
        
        INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, 
                                TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@TaxSystemTypeId, 'NOT_SUBJ', 'NOT_SUBJ to Tax', 0.13, 1, 0, @BoundedContextId, @CountryId, 1, 0);
        
        PRINT 'Created new TaxSystemType with ID: ' + CAST(@TaxSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing TaxSystemType with ID: ' + CAST(@TaxSystemTypeId AS NVARCHAR(36));
    END


    -- Check for EXEMPT tax system type
DECLARE @TaxSystemTypeExemptId UNIQUEIDENTIFIER;
SELECT TOP 1 @TaxSystemTypeExemptId = TaxSystemTypeId 
FROM TaxSystemType 
WHERE TaxSystemTypeCode = 'EXEMPT';

IF @TaxSystemTypeExemptId IS NULL
BEGIN
    SET @TaxSystemTypeExemptId = NEWID();
    
    INSERT INTO TaxSystemType (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName, TaxSystemTypeRate, TaxSystemTypeSign, 
                            TaxSystemTypeMinimumTaxableValue, BoundedContextId, CountryId, IsActive, IsDeleted)
    VALUES (@TaxSystemTypeExemptId, 'EXEMPT', 'Tax Exempt', 0.00, 0, 0, @BoundedContextId, @CountryId, 1, 0);
    
    PRINT 'Created new TaxSystemType EXEMPT with ID: ' + CAST(@TaxSystemTypeExemptId AS NVARCHAR(36));

    -- Link to customer for IsExempt flag
    INSERT INTO CustomerTaxSystemType (CustomerId, TaxSystemTypeId, CustomerTaxSystemTypeComments, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@CustomerId, @TaxSystemTypeExemptId, 'Tax exempt customer', GETDATE(), @UserId, 0);
END





/* --- EXTRA TAX LOOKUPS (place after original TaxSystemType block) --- */
DECLARE @TaxSystemTypeId_EXEMPT UNIQUEIDENTIFIER
DECLARE @TaxSystemTypeId_NOTSUBJ UNIQUEIDENTIFIER

/* EXEMPT */
SELECT TOP 1 @TaxSystemTypeId_EXEMPT = TaxSystemTypeId
FROM   TaxSystemType
WHERE  TaxSystemTypeCode = 'EXEMPT'

IF @TaxSystemTypeId_EXEMPT IS NULL
BEGIN
    SET @TaxSystemTypeId_EXEMPT = NEWID()
    INSERT INTO TaxSystemType
           (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName,
            TaxSystemTypeRate, TaxSystemTypeSign,
            TaxSystemTypeMinimumTaxableValue, BoundedContextId,
            CountryId, IsActive, IsDeleted)
    VALUES (@TaxSystemTypeId_EXEMPT, 'EXEMPT', 'Exempt Entity', 0.00, 1, 0,
            @BoundedContextId, @CountryId, 1, 0)
END

/* NOT_SUBJ */
SELECT TOP 1 @TaxSystemTypeId_NOTSUBJ = TaxSystemTypeId
FROM   TaxSystemType
WHERE  TaxSystemTypeCode = 'NOT_SUBJ'

IF @TaxSystemTypeId_NOTSUBJ IS NULL
BEGIN
    SET @TaxSystemTypeId_NOTSUBJ = NEWID()
    INSERT INTO TaxSystemType
           (TaxSystemTypeId, TaxSystemTypeCode, TaxSystemTypeName,
            TaxSystemTypeRate, TaxSystemTypeSign,
            TaxSystemTypeMinimumTaxableValue, BoundedContextId,
            CountryId, IsActive, IsDeleted)
    VALUES (@TaxSystemTypeId_NOTSUBJ, 'NOT_SUBJ', 'Not Subject to VAT', 0.00, 1, 0,
            @BoundedContextId, @CountryId, 1, 0)
END
/* --- END EXTRA TAX LOOKUPS ---------------------------------------- */



















    -- Check for existing EconomicActivitySystemType
    SELECT TOP 1 @EconomicActivityTypeId = EconomicActivitySystemTypeId 
    FROM EconomicActivitySystemType 
    WHERE EconomicActivitySystemTypeCode = 'RETAIL' ;
    
    IF @EconomicActivityTypeId IS NULL
    BEGIN
        -- Insert EconomicActivitySystemType
        SET @EconomicActivityTypeId = NEWID();
        
        INSERT INTO EconomicActivitySystemType (EconomicActivitySystemTypeId, EconomicActivitySystemTypeCode, EconomicActivitySystemTypeName, 
                                            BoundedContextId, CountryId, IsActive, IsDeleted)
        VALUES (@EconomicActivityTypeId, 'RETAIL', 'Retail', @BoundedContextId, @CountryId, 1, 0);
        
        PRINT 'Created new EconomicActivitySystemType with ID: ' + CAST(@EconomicActivityTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing EconomicActivitySystemType with ID: ' + CAST(@EconomicActivityTypeId AS NVARCHAR(36));
    END

    -- Get or create AddressSystemType
    SELECT TOP 1 @AddressSystemTypeId = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'PRI' ;
    
    IF @AddressSystemTypeId IS NULL
    BEGIN
        SET @AddressSystemTypeId = NEWID();
        
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, IsActive, BoundedContextId)
        VALUES (@AddressSystemTypeId, 'PRI', 'Primary', 1, @BoundedContextId);
        
        PRINT 'Created new AddressSystemType with ID: ' + CAST(@AddressSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing AddressSystemType with ID: ' + CAST(@AddressSystemTypeId AS NVARCHAR(36));
    END

    -- Get or create ContactSystemType
    SELECT TOP 1 @ContactSystemTypeId = ContactSystemTypeId 
    FROM ContactSystemType 
    WHERE ContactSystemTypeCode = 'REPRESENT' AND IsActive = 1;
    
    IF @ContactSystemTypeId IS NULL
    BEGIN
        SET @ContactSystemTypeId = NEWID();
        
        INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, IsActive, IsDeleted)
        VALUES (@ContactSystemTypeId, 'REPRESENT', 'Primary', 1, 0);
        
        PRINT 'Created new ContactSystemType with ID: ' + CAST(@ContactSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing ContactSystemType with ID: ' + CAST(@ContactSystemTypeId AS NVARCHAR(36));
    END

    -- Get or create EmailAddressSystemType
    SELECT TOP 1 @EmailSystemTypeId = EmailAddressSystemTypeId 
    FROM EmailAddressSystemType 
    WHERE EmailAddressSystemTypeCode = 'BILLING' AND IsActive = 1;
    
    IF @EmailSystemTypeId IS NULL
    BEGIN
        SET @EmailSystemTypeId = NEWID();
        
        INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, IsActive, IsDeleted)
        VALUES (@EmailSystemTypeId, 'PRI', 'Primary', 1, 0);
        
        PRINT 'Created new EmailAddressSystemType with ID: ' + CAST(@EmailSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing EmailAddressSystemType with ID: ' + CAST(@EmailSystemTypeId AS NVARCHAR(36));
    END

    -- Get or create PhoneNumberSystemType
    SELECT TOP 1 @PhoneSystemTypeId = PhoneNumberSystemTypeId 
    FROM PhoneNumberSystemType 
    WHERE PhoneNumberSystemTypeCode = 'PRI' AND IsActive = 1;
    
    IF @PhoneSystemTypeId IS NULL
    BEGIN
        SET @PhoneSystemTypeId = NEWID();
        
        INSERT INTO PhoneNumberSystemType (PhoneNumberSystemTypeId, PhoneNumberSystemTypeCode, PhoneNumberSystemTypeName, IsActive, IsDeleted)
        VALUES (@PhoneSystemTypeId, 'PRI', 'Primary', 1, 0);
        
        PRINT 'Created new PhoneNumberSystemType with ID: ' + CAST(@PhoneSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing PhoneNumberSystemType with ID: ' + CAST(@PhoneSystemTypeId AS NVARCHAR(36));
    END

    -- Get or create DocumentIdentificationSystemType
    SELECT TOP 1 @DocSystemTypeId = DocumentIdentificationSystemTypeId 
    FROM DocumentIdentificationSystemType 
    WHERE DocumentIdentificationSystemTypeCode = 'NIT'       ;
    
    IF @DocSystemTypeId IS NULL
    BEGIN
        SET @DocSystemTypeId = NEWID();
        
        INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, 
                                                  DocumentIdentificationSystemTypeName, CountryId, BoundedContextId, IsActive, IsDeleted)
        VALUES (@DocSystemTypeId, 'NIT', 'Tax ID', @CountryId, @BoundedContextId, 1, 0);
        
        PRINT 'Created new DocumentIdentificationSystemType with ID: ' + CAST(@DocSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing DocumentIdentificationSystemType with ID: ' + CAST(@DocSystemTypeId AS NVARCHAR(36));
    END
























    -- Get or create ProductSystemType
    SELECT TOP 1 @ProductSystemTypeId = ProductSystemTypeId 
    FROM ProductSystemType 
    WHERE ProductSystemTypeCode = 'PROD' AND IsActive = 1;
    
    IF @ProductSystemTypeId IS NULL
    BEGIN
        SET @ProductSystemTypeId = NEWID();
        
        INSERT INTO ProductSystemType (ProductSystemTypeId, ProductSystemTypeCode, ProductSystemTypeName, IsActive, IsDeleted)
        VALUES (@ProductSystemTypeId, 'PROD', 'Product', 1, 0);
        
        PRINT 'Created new ProductSystemType with ID: ' + CAST(@ProductSystemTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing ProductSystemType with ID: ' + CAST(@ProductSystemTypeId AS NVARCHAR(36));
    END

    -- Get or create MeasurementUnitSystemType
    SELECT TOP 1 @MeasurementUnitTypeId = MeasurementUnitSystemTypeId 
    FROM MeasurementUnitSystemType 
    WHERE MeasurementUnitSystemTypeCode = 'EACH' ;
    
    IF @MeasurementUnitTypeId IS NULL
    BEGIN
        SET @MeasurementUnitTypeId = NEWID();
        
        INSERT INTO MeasurementUnitSystemType (MeasurementUnitSystemTypeId, MeasurementUnitSystemTypeCode, MeasurementUnitSystemTypeName, 
                                            MeasurementUnitSystemTypeAbbreviation, BoundedContextId, IsActive, IsDeleted)
        VALUES (@MeasurementUnitTypeId, 'EACH', 'Each', 'ea', @BoundedContextId, 1, 0);
        
        PRINT 'Created new MeasurementUnitSystemType with ID: ' + CAST(@MeasurementUnitTypeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        PRINT 'Using existing MeasurementUnitSystemType with ID: ' + CAST(@MeasurementUnitTypeId AS NVARCHAR(36));
    END

    -- Insert core entities
    -- Check if Address already exists
    DECLARE @ExistingAddressId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingAddressId = AddressId 
    FROM Address 
    WHERE AddressStreet = '123 Main St' 
        AND CityId = @CityId 
        AND StateId = @StateId 
        AND CountryId = @CountryId;
    
    IF @ExistingAddressId IS NULL
    BEGIN
        -- Insert Address
        INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, CityId, DistrictId, CountyId, StateId, CountryId, 
                          CreatedDate, CreatedBy, IsDeleted)
        VALUES (@AddressId, '123 Main St', 'Suite 100', '94105', @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
              GETDATE(), @UserId, 0);
        
        PRINT 'Created new Address with ID: ' + CAST(@AddressId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @AddressId = @ExistingAddressId;
        PRINT 'Using existing Address with ID: ' + CAST(@AddressId AS NVARCHAR(36));
    END

    -- Check if Contact already exists
    DECLARE @ExistingContactId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingContactId = ContactId 
    FROM Contact 
    WHERE FirstName = 'John' AND LastName = 'Smith' AND Email = 'john.smith@example.com';
    
    IF @ExistingContactId IS NULL
    BEGIN
        -- Insert Contact
        INSERT INTO Contact (ContactId, FirstName, LastName, Email, Phone, Mobile, IsPrimaryContact, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@ContactId, 'John', 'Smith', 'john.smith@example.com', '555-123-4567', '555-987-6543', 1, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Contact with ID: ' + CAST(@ContactId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @ContactId = @ExistingContactId;
        PRINT 'Using existing Contact with ID: ' + CAST(@ContactId AS NVARCHAR(36));
    END

    -- Check if EmailAddress already exists
    DECLARE @ExistingEmailAddressId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingEmailAddressId = EmailAddressId 
    FROM EmailAddress 
    WHERE EmailAddressString = 'info@acmecorp.com';
    
    IF @ExistingEmailAddressId IS NULL
    BEGIN
        -- Insert EmailAddress
        INSERT INTO EmailAddress (EmailAddressId, EmailAddressString, IsDeleted)
        VALUES (@EmailAddressId, 'info@acmecorp.com', 0);
        
        PRINT 'Created new EmailAddress with ID: ' + CAST(@EmailAddressId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @EmailAddressId = @ExistingEmailAddressId;
        PRINT 'Using existing EmailAddress with ID: ' + CAST(@EmailAddressId AS NVARCHAR(36));
    END

    -- Check if PhoneNumber already exists
    DECLARE @ExistingPhoneNumberId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingPhoneNumberId = PhoneNumberId 
    FROM PhoneNumber 
    WHERE PhoneNumberString = '555-789-1234';
    
    IF @ExistingPhoneNumberId IS NULL
    BEGIN
        -- Insert PhoneNumber
        INSERT INTO PhoneNumber (PhoneNumberId, PhoneNumberString, IsDeleted)
        VALUES (@PhoneNumberId, '555-789-1234', 0);
        
        PRINT 'Created new PhoneNumber with ID: ' + CAST(@PhoneNumberId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @PhoneNumberId = @ExistingPhoneNumberId;
        PRINT 'Using existing PhoneNumber with ID: ' + CAST(@PhoneNumberId AS NVARCHAR(36));
    END








    -- Create REP_DOC document type if it doesn't exist
DECLARE @DocSystemTypeREP_DOC UNIQUEIDENTIFIER;
SELECT TOP 1 @DocSystemTypeREP_DOC = DocumentIdentificationSystemTypeId 
FROM DocumentIdentificationSystemType 
WHERE DocumentIdentificationSystemTypeCode = 'REP_DOC';

IF @DocSystemTypeREP_DOC IS NULL
BEGIN
    SET @DocSystemTypeREP_DOC = NEWID();
    
    INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, 
                                          DocumentIdentificationSystemTypeName, CountryId, BoundedContextId, IsActive, IsDeleted)
    VALUES (@DocSystemTypeREP_DOC, 'REP_DOC', 'Representative Document', @CountryId, @BoundedContextId, 1, 0);
    
    PRINT 'Created new DocumentIdentificationSystemType REP_DOC with ID: ' + CAST(@DocSystemTypeREP_DOC AS NVARCHAR(36));

    -- Add representative document (with renamed variable)
    DECLARE @RepDocIdentification UNIQUEIDENTIFIER = NEWID();
    INSERT INTO DocumentIdentification (DocumentIdentificationId, DocumentIdentificationNumber, IsDeleted)
    VALUES (@RepDocIdentification, 'REP-123456', 0);

    INSERT INTO CustomerDocumentIdentification (CustomerId, DocumentIdentificationId, DocumentIdentificationSystemTypeId, 
                                           DocumentIdentificationVerified, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@CustomerId, @RepDocIdentification, @DocSystemTypeREP_DOC, GETDATE(), GETDATE(), @UserId, 0);
END











    -- Check if DocumentIdentification already exists
    DECLARE @ExistingDocIdentificationId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingDocIdentificationId = DocumentIdentificationId 
    FROM DocumentIdentification 
    WHERE DocumentIdentificationNumber = '123456789-0';
    
    IF @ExistingDocIdentificationId IS NULL
    BEGIN
        -- Insert DocumentIdentification
        INSERT INTO DocumentIdentification (DocumentIdentificationId, DocumentIdentificationNumber, IsDeleted)
        VALUES (@DocIdentificationId, '123456789-0', 0);
        
        PRINT 'Created new DocumentIdentification with ID: ' + CAST(@DocIdentificationId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @DocIdentificationId = @ExistingDocIdentificationId;
        PRINT 'Using existing DocumentIdentification with ID: ' + CAST(@DocIdentificationId AS NVARCHAR(36));
    END












	   	  

		   -- Check if Employee already exists for this ApplicationUserId
    DECLARE @ExistingEmployeeId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingEmployeeId = EmployeeId 
    FROM Employee 
    WHERE ApplicationUserId = @ApplicationUserId AND IsActive = 1 AND IsDeleted = 0;
    
    IF @ExistingEmployeeId IS NULL
    BEGIN
        -- Insert new Employee
        SET @EmployeeId = NEWID();
        
        INSERT INTO Employee (EmployeeId, EmployeeCode, EmployeeFirstName, EmployeeLastName, 
                            ApplicationUserId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@EmployeeId, 'EMP001', 'Michael', 'Johnson', 
               @ApplicationUserId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Employee with ID: ' + CAST(@EmployeeId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @EmployeeId = @ExistingEmployeeId;
        PRINT 'Using existing Employee with ID: ' + CAST(@EmployeeId AS NVARCHAR(36));
    END

    -- Check if Salesperson already exists
    DECLARE @ExistingSalespersonId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingSalespersonId = SalespersonId 
    FROM Salesperson 
    WHERE EmployeeId = @EmployeeId;
    
    IF @ExistingSalespersonId IS NULL
    BEGIN
        -- Insert Salesperson with valid EmployeeId
        INSERT INTO Salesperson (SalespersonId, SalespersonCode, SalespersonFirstName, SalespersonLastName, 
                               EmployeeId, ApplicationUserId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@SalespersonId, 'SP001', 'Michael', 'Johnson', 
              @EmployeeId, @ApplicationUserId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Salesperson with ID: ' + CAST(@SalespersonId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @SalespersonId = @ExistingSalespersonId;
        PRINT 'Using existing Salesperson with ID: ' + CAST(@SalespersonId AS NVARCHAR(36));
    END
















    -- Check if Product already exists
    DECLARE @ExistingProductId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingProductId = ProductId 
    FROM Product 
    WHERE ProductCode = 'PROD001' AND ProductName = 'Standard Widget';
    
    IF @ExistingProductId IS NULL
    BEGIN
        -- Insert Product
        INSERT INTO Product (ProductId, ProductCode, ProductName, Description, UnitPrice, ProductSystemTypeId, 
                          PrimaryMeasurementUnitSystemTypeId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@ProductId, 'PROD001', 'Standard Widget', 'High quality widget', 100.00, @ProductSystemTypeId, 
              @MeasurementUnitTypeId, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new Product with ID: ' + CAST(@ProductId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @ProductId = @ExistingProductId;
        PRINT 'Using existing Product with ID: ' + CAST(@ProductId AS NVARCHAR(36));
    END

    -- Check if Customer already exists
    DECLARE @ExistingCustomerId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingCustomerId = CustomerId 
    FROM Customer 
    WHERE CustomerCode = 'CUST001' AND CustomerCommercialName = 'Acme Corporation';
    
    IF @ExistingCustomerId IS NULL
    BEGIN
        -- Insert Customer ensuring foreign keys are valid
        INSERT INTO Customer (CustomerId, CustomerCode, CustomerIsPerson, CustomerFirstName, CustomerLastName, CustomerCommercialName,
                           TaxpayerSystemTypeId, CustomerCreditTermDays, CustomerCreditLimitAmount, CustomerIsReseller, CustomerStatus,
                           CustomerUserTypeId, CustomerIsForeign, CustomerPrimaryCountryId, CreatedDate, CreatedBy, IsActive, IsDeleted,ApplicationUserId)
        VALUES (@CustomerId, 'CUST001', 0, 'Oliver', 'James', 'PCIShield Corporation',
              @TaxpayerSystemTypeId, 30, 10000.00, 0, 1,
              @CustomerUserTypeId, 0, @CountryId, GETDATE(), @UserId, 1, 0,@UserId);
        
        PRINT 'Created new Customer with ID: ' + CAST(@CustomerId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @CustomerId = @ExistingCustomerId;
        PRINT 'Using existing Customer with ID: ' + CAST(@CustomerId AS NVARCHAR(36));
    END







    -- Check for existing document types for NRC and REP_DOC
DECLARE @DocSystemTypeNRC UNIQUEIDENTIFIER;
DECLARE @DocSystemTypeREP UNIQUEIDENTIFIER;

SELECT TOP 1 @DocSystemTypeNRC = DocumentIdentificationSystemTypeId 
FROM DocumentIdentificationSystemType 
WHERE DocumentIdentificationSystemTypeCode = 'NRC' AND IsActive = 1;

SELECT TOP 1 @DocSystemTypeREP = DocumentIdentificationSystemTypeId 
FROM DocumentIdentificationSystemType 
WHERE DocumentIdentificationSystemTypeCode = 'REP_DOC' AND IsActive = 1;

-- Create NRC document type if it doesn't exist
IF @DocSystemTypeNRC IS NULL
BEGIN
    SET @DocSystemTypeNRC = NEWID();
    
    INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, 
                                            DocumentIdentificationSystemTypeName, DocumentIdentificationSystemTypeRegex, IsRegexRequired, 
                                            CountryId, BoundedContextId, IsActive, IsDeleted)
    VALUES (@DocSystemTypeNRC, 'NRC', 'Business Registry ID', NULL, 0, @CountryId, @BoundedContextId, 1, 0);
    
    PRINT 'Created new DocumentIdentificationSystemType NRC with ID: ' + CAST(@DocSystemTypeNRC AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing DocumentIdentificationSystemType NRC with ID: ' + CAST(@DocSystemTypeNRC AS NVARCHAR(36));
END

-- Add necessary documents and relationships
DECLARE @DocIdentificationNRC UNIQUEIDENTIFIER = NEWID();
INSERT INTO DocumentIdentification (DocumentIdentificationId, DocumentIdentificationNumber, IsDeleted)
VALUES (@DocIdentificationNRC, 'NRC12345-6', 0);

INSERT INTO CustomerDocumentIdentification (CustomerId, DocumentIdentificationId, DocumentIdentificationSystemTypeId, 
                                         DocumentIdentificationVerified, CreatedDate, CreatedBy, IsDeleted)
VALUES (@CustomerId, @DocIdentificationNRC, @DocSystemTypeNRC, GETDATE(), GETDATE(), @UserId, 0);

-- Add REP_DOC document identification if needed
IF @DocSystemTypeREP IS NOT NULL
BEGIN
    DECLARE @DocIdentificationREP UNIQUEIDENTIFIER = NEWID();
    INSERT INTO DocumentIdentification (DocumentIdentificationId, DocumentIdentificationNumber, IsDeleted)
    VALUES (@DocIdentificationREP, 'REP789-A', 0);
    
    INSERT INTO CustomerDocumentIdentification (CustomerId, DocumentIdentificationId, DocumentIdentificationSystemTypeId, 
                                             DocumentIdentificationVerified, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@CustomerId, @DocIdentificationREP, @DocSystemTypeREP, GETDATE(), GETDATE(), @UserId, 0);
END

-- Add BILLING email type
DECLARE @EmailSystemTypeBilling UNIQUEIDENTIFIER;
SELECT TOP 1 @EmailSystemTypeBilling = EmailAddressSystemTypeId 
FROM EmailAddressSystemType 
WHERE EmailAddressSystemTypeCode = 'BILLING' AND IsActive = 1;

IF @EmailSystemTypeBilling IS NULL
BEGIN
    SET @EmailSystemTypeBilling = NEWID();
    INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, IsActive, IsDeleted)
    VALUES (@EmailSystemTypeBilling, 'BILLING', 'Billing Email', 1, 0);
END

-- Add billing email
DECLARE @EmailAddressBilling UNIQUEIDENTIFIER = NEWID();
INSERT INTO EmailAddress (EmailAddressId, EmailAddressString, IsDeleted)
VALUES (@EmailAddressBilling, 'billing@acmecorp.com', 0);

-- Link billing email to customer
INSERT INTO CustomerEmailAddress (CustomerId, EmailAddressId, EmailAddressSystemTypeId, IsPrimaryEmailAddress, CreatedDate, CreatedBy, IsDeleted)
VALUES (@CustomerId, @EmailAddressBilling, @EmailSystemTypeBilling, 0, GETDATE(), @UserId, 0);

-- Add BIL and DEL address types
DECLARE @AddressSystemTypeBIL UNIQUEIDENTIFIER;
DECLARE @AddressSystemTypeDEL UNIQUEIDENTIFIER;

SELECT TOP 1 @AddressSystemTypeBIL = AddressSystemTypeId 
FROM AddressSystemType 
WHERE AddressSystemTypeCode = 'BIL' AND IsActive = 1;

SELECT TOP 1 @AddressSystemTypeDEL = AddressSystemTypeId 
FROM AddressSystemType 
WHERE AddressSystemTypeCode = 'DEL' AND IsActive = 1;

IF @AddressSystemTypeBIL IS NULL
BEGIN
    SET @AddressSystemTypeBIL = NEWID();
    INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, IsActive, BoundedContextId)
    VALUES (@AddressSystemTypeBIL, 'BIL', 'Billing', 1, @BoundedContextId);
END

IF @AddressSystemTypeDEL IS NULL
BEGIN
    SET @AddressSystemTypeDEL = NEWID();
    INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, IsActive, BoundedContextId)
    VALUES (@AddressSystemTypeDEL, 'DEL', 'Delivery', 1, @BoundedContextId);
END

-- Link the same address for both billing and delivery purposes
INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
VALUES (@AddressId, @AddressSystemTypeBIL, @CustomerId, 0, GETDATE(), @UserId, 0);

INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
VALUES (@AddressId, @AddressSystemTypeDEL, @CustomerId, 0, GETDATE(), @UserId, 0);









    -- Now that Customer exists, check for relationships
    -- Check if CustomerAddress already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerAddress WHERE CustomerId = @CustomerId AND AddressId = @AddressId)
    BEGIN
        -- Insert CustomerAddress
        INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@AddressId, @AddressSystemTypeId, @CustomerId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerAddress relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerAddress relationship already exists';
    END






-- Create distinct billing address
DECLARE @BillingAddressId UNIQUEIDENTIFIER = NEWID();
INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, CityId, DistrictId, CountyId, StateId, CountryId, 
                  CreatedDate, CreatedBy, IsDeleted)
VALUES (@BillingAddressId, '789 Billing St', 'Suite 200', '94106', @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
      GETDATE(), @UserId, 0);

-- Create distinct delivery address
DECLARE @DeliveryAddressId UNIQUEIDENTIFIER = NEWID();
INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, CityId, DistrictId, CountyId, StateId, CountryId, 
                  CreatedDate, CreatedBy, IsDeleted)
VALUES (@DeliveryAddressId, '456 Shipping Ave', 'Floor 3', '94107', @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
      GETDATE(), @UserId, 0);

-- Link billing address to customer
INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
VALUES (@BillingAddressId, @AddressSystemTypeBIL, @CustomerId, 0, GETDATE(), @UserId, 0);

-- Link delivery address to customer
INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
VALUES (@DeliveryAddressId, @AddressSystemTypeDEL, @CustomerId, 0, GETDATE(), @UserId, 0);















    -- Check if CustomerContact already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerContact WHERE CustomerId = @CustomerId AND ContactId = @ContactId)
    BEGIN
        -- Insert CustomerContact
        INSERT INTO CustomerContact (CustomerId, ContactId, ContactSystemTypeId, IsPrimaryContact, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @ContactId, @ContactSystemTypeId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerContact relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerContact relationship already exists';
    END





    -- First, ensure ContactSystemType for REPRESENT exists
DECLARE @RepContactSystemTypeId UNIQUEIDENTIFIER;
SELECT TOP 1 @RepContactSystemTypeId = ContactSystemTypeId 
FROM ContactSystemType 
WHERE ContactSystemTypeCode = 'REPRESENT';

IF @RepContactSystemTypeId IS NULL
BEGIN
    SET @RepContactSystemTypeId = NEWID();
    INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, BoundedContextId, IsActive, IsDeleted)
    VALUES (@RepContactSystemTypeId, 'REPRESENT', 'Representative', @BoundedContextId, 1, 0);
END

-- Create representative contact
DECLARE @RepresentativeContactId UNIQUEIDENTIFIER = NEWID();
INSERT INTO Contact (ContactId, FirstName, LastName, Email, Phone, Mobile, IsPrimaryContact, CreatedDate, CreatedBy, IsActive, IsDeleted)
VALUES (@RepresentativeContactId, 'Robert', 'Johnson', 'robert.johnson@acmecorp.com', '555-222-3333', '555-444-5555', 0, GETDATE(), @UserId, 1, 0);

-- Link the contact to customer with proper ContactSystemType
INSERT INTO CustomerContact (CustomerId, ContactId, ContactSystemTypeId, IsPrimaryContact, CreatedDate, CreatedBy, IsDeleted)
VALUES (@CustomerId, @RepresentativeContactId, @RepContactSystemTypeId, 0, GETDATE(), @UserId, 0);










    -- Check if CustomerEmailAddress already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerEmailAddress WHERE CustomerId = @CustomerId AND EmailAddressId = @EmailAddressId)
    BEGIN
        -- Insert CustomerEmailAddress
        INSERT INTO CustomerEmailAddress (CustomerId, EmailAddressId, EmailAddressSystemTypeId, IsPrimaryEmailAddress, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @EmailAddressId, @EmailSystemTypeId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerEmailAddress relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerEmailAddress relationship already exists';
    END

    -- Check if CustomerPhoneNumber already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerPhoneNumber WHERE CustomerId = @CustomerId AND PhoneNumberId = @PhoneNumberId)
    BEGIN
        -- Insert CustomerPhoneNumber
        INSERT INTO CustomerPhoneNumber (CustomerId, PhoneNumberId, PhoneNumberSystemTypeId, IsPrimaryPhoneNumber, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @PhoneNumberId, @PhoneSystemTypeId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerPhoneNumber relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerPhoneNumber relationship already exists';
    END

    -- Check if CustomerDocumentIdentification already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerDocumentIdentification WHERE CustomerId = @CustomerId AND DocumentIdentificationId = @DocIdentificationId)
    BEGIN
        -- Insert CustomerDocumentIdentification
        INSERT INTO CustomerDocumentIdentification (CustomerId, DocumentIdentificationId, DocumentIdentificationSystemTypeId, 
                                                 DocumentIdentificationVerified, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @DocIdentificationId, @DocSystemTypeId, GETDATE(), GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerDocumentIdentification relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerDocumentIdentification relationship already exists';
    END

    -- Check if CustomerSalesperson already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerSalesperson WHERE CustomerId = @CustomerId AND SalespersonId = @SalespersonId)
    BEGIN
        -- Insert CustomerSalesperson
        INSERT INTO CustomerSalesperson (CustomerId, SalespersonId, IsPrimarySalesperson, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @SalespersonId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerSalesperson relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerSalesperson relationship already exists';
    END

    -- Check if CustomerEconomicActivitySystemType already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerEconomicActivitySystemType WHERE CustomerId = @CustomerId AND EconomicActivitySystemTypeId = @EconomicActivityTypeId)
    BEGIN
        -- Insert CustomerEconomicActivitySystemType
        INSERT INTO CustomerEconomicActivitySystemType (CustomerId, EconomicActivitySystemTypeId, IsPrimaryEconomicActivitySystemType,
                                                     CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @EconomicActivityTypeId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerEconomicActivitySystemType relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerEconomicActivitySystemType relationship already exists';
    END

    -- Check if CustomerTaxSystemType already exists
    IF NOT EXISTS (SELECT 1 FROM CustomerTaxSystemType WHERE CustomerId = @CustomerId AND TaxSystemTypeId = @TaxSystemTypeId)
    BEGIN
        -- Insert CustomerTaxSystemType
        INSERT INTO CustomerTaxSystemType (CustomerId, TaxSystemTypeId, CustomerTaxSystemTypeComments, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@CustomerId, @TaxSystemTypeId, 'Regular taxpayer', GETDATE(), @UserId, 0);
        
        PRINT 'Created new CustomerTaxSystemType relationship';
    END
    ELSE
    BEGIN
        PRINT 'CustomerTaxSystemType relationship already exists';
    END

    -- Check if Invoice already exists
    DECLARE @ExistingInvoiceId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingInvoiceId = InvoiceId 
    FROM Invoice 
    WHERE CustomerId = @CustomerId AND InvoiceSystemTypeId = @InvoiceSystemTypeId 
        AND CAST(CAST(InvoiceDate AS DATE) AS DATETIME) = CAST(CAST(GETDATE() AS DATE) AS DATETIME);
    
    IF @ExistingInvoiceId IS NULL
    BEGIN
        -- Insert Invoice
        INSERT INTO Invoice (InvoiceId, InvoiceSystemTypeId, InvoiceDate, CustomerId, IsDraft, IsVoided, InvoiceStatus,
                          InvoiceForeignCurrencyId, InvoiceForeignCurrencyRate, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceId, @InvoiceSystemTypeId, GETDATE(), @CustomerId, 0, 0, 1,
              NULL, NULL, GETDATE(), @UserId, 0);
        
        PRINT 'Created new Invoice with ID: ' + CAST(@InvoiceId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @InvoiceId = @ExistingInvoiceId;
        PRINT 'Using existing Invoice with ID: ' + CAST(@InvoiceId AS NVARCHAR(36));
    END

    -- Check if InvoiceDetail already exists
    DECLARE @ExistingInvoiceDetailId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingInvoiceDetailId = InvoiceDetailId 
    FROM InvoiceDetail 
    WHERE InvoiceId = @InvoiceId AND ProductId = @ProductId;
    
    IF @ExistingInvoiceDetailId IS NULL
    BEGIN
        -- Insert Invoice Detail
        INSERT INTO InvoiceDetail (InvoiceDetailId, InvoiceId, ProductId, ProductSystemTypeId, InvoiceDetailDescription,
                                InvoiceDetailQuantity, InvoiceDetailUnitPrice, InvoiceDetailDiscountPercentage, InvoiceDetailLineTotal,
                                MeasurementUnitSystemTypeId, InvoiceDetailMeasurementUnitSystemTypeQuantity,
                                InvoiceDetailMeasurementUnitSystemTypePrice, InvoiceDetailMeasurementUnitSystemTypeName,
                                InvoiceDetailMeasurementUnitSystemTypeAbbreviation, InvoiceDetailIsHiddenInGroup,
                                CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailId, @InvoiceId, @ProductId, @ProductSystemTypeId, 'Standard Widget',
              5, 100.00, 0, 500.00,
              @MeasurementUnitTypeId, 5,
              100.00, 'Each', 'ea', 0,
              GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceDetail with ID: ' + CAST(@InvoiceDetailId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @InvoiceDetailId = @ExistingInvoiceDetailId;
        PRINT 'Using existing InvoiceDetail with ID: ' + CAST(@InvoiceDetailId AS NVARCHAR(36));
    END

    -- Check if InvoiceTaxSystemType already exists
    IF NOT EXISTS (SELECT 1 FROM InvoiceTaxSystemType WHERE InvoiceId = @InvoiceId AND TaxSystemTypeId = @TaxSystemTypeId)
    BEGIN
        -- Insert Invoice Tax
        INSERT INTO InvoiceTaxSystemType (InvoiceId, TaxSystemTypeId, InvoiceTaxSystemTypeRate, InvoiceTaxSystemTypeAmount,
                                       InvoiceTaxSystemTypeSign, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceId, @TaxSystemTypeId, 0.13, 65.00, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceTaxSystemType relationship';
    END
    ELSE
    BEGIN
        PRINT 'InvoiceTaxSystemType relationship already exists';
    END

    -- Check if InvoiceAccountReceivable already exists
    DECLARE @ExistingInvoiceARId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingInvoiceARId = InvoiceAccountReceivableId 
    FROM InvoiceAccountReceivable 
    WHERE InvoiceId = @InvoiceId;
    
    IF @ExistingInvoiceARId IS NULL
    BEGIN
        -- Insert Invoice Account Receivable
        INSERT INTO InvoiceAccountReceivable (InvoiceAccountReceivableId, InvoiceId, OriginalBalance, CurrentBalance,
                                           DueDate, LastPaymentDate, IsVoided, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceARId, @InvoiceId, 565.00, 565.00, DATEADD(DAY, 30, GETDATE()), NULL, 0, GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceAccountReceivable with ID: ' + CAST(@InvoiceARId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @InvoiceARId = @ExistingInvoiceARId;
        PRINT 'Using existing InvoiceAccountReceivable with ID: ' + CAST(@InvoiceARId AS NVARCHAR(36));
    END

    -- Check if InvoiceContact already exists
    IF NOT EXISTS (SELECT 1 FROM InvoiceContact WHERE InvoiceId = @InvoiceId AND ContactId = @ContactId)
    BEGIN
        -- Insert InvoiceContact
        INSERT INTO InvoiceContact (InvoiceId, ContactId, ContactSystemTypeId, IsPrimaryContact, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceId, @ContactId, @ContactSystemTypeId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceContact relationship';
    END
    ELSE
    BEGIN
        PRINT 'InvoiceContact relationship already exists';
    END

    -- Check if InvoiceSalesperson already exists
    IF NOT EXISTS (SELECT 1 FROM InvoiceSalesperson WHERE InvoiceId = @InvoiceId AND SalespersonId = @SalespersonId)
    BEGIN
        -- Insert InvoiceSalesperson
        INSERT INTO InvoiceSalesperson (InvoiceId, SalespersonId, IsPrimarySalesperson, CommissionPercent, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceId, @SalespersonId, 1, 0.05, GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceSalesperson relationship';
    END
    ELSE
    BEGIN
        PRINT 'InvoiceSalesperson relationship already exists';
    END

    -- Check if ElectronicDocument table exists and create it if needed
    IF OBJECT_ID('ElectronicDocument', 'U') IS NOT NULL
    BEGIN
        -- Check if ElectronicDocument already exists
        DECLARE @ExistingElectronicDocumentId UNIQUEIDENTIFIER;
        SELECT TOP 1 @ExistingElectronicDocumentId = ed.ElectronicDocumentId 
        FROM ElectronicDocument ed
        JOIN ElectronicDocumentInvoice edi ON ed.ElectronicDocumentId = edi.ElectronicDocumentId
        WHERE edi.InvoiceId = @InvoiceId;
        
        IF @ExistingElectronicDocumentId IS NULL
        BEGIN
            -- Insert ElectronicDocument
            INSERT INTO ElectronicDocument (ElectronicDocumentId, CreatedDate, CreatedBy, IsDeleted)
            VALUES (@ElectronicDocumentId, GETDATE(), @UserId, 0);
            
            -- Insert ElectronicDocumentInvoice
            INSERT INTO ElectronicDocumentInvoice (ElectronicDocumentId, InvoiceId)
            VALUES (@ElectronicDocumentId, @InvoiceId);
            
            PRINT 'Created new ElectronicDocument with ID: ' + CAST(@ElectronicDocumentId AS NVARCHAR(36));
        END
        ELSE
        BEGIN
            SET @ElectronicDocumentId = @ExistingElectronicDocumentId;
            PRINT 'Using existing ElectronicDocument with ID: ' + CAST(@ElectronicDocumentId AS NVARCHAR(36));
        END
    END















    -- Check if InvoiceCustomer already exists
    DECLARE @ExistingInvoiceCustomerId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingInvoiceCustomerId = InvoiceCustomerId 
    FROM InvoiceCustomer 
    WHERE InvoiceId = @InvoiceId AND CustomerId = @CustomerId;
    
    IF @ExistingInvoiceCustomerId IS NULL
    BEGIN
        -- Insert InvoiceCustomer
        INSERT INTO InvoiceCustomer (InvoiceCustomerId, InvoiceId, CustomerId, InvoiceCustomerUserType, 
                                  InvoiceCustomerFirstName, InvoiceCustomerLastName, InvoiceCustomerCommercialName,
                                  CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceCustomerId, @InvoiceId, @CustomerId, 1, 
              '', '', 'Acme Corporation',
              GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceCustomer with ID: ' + CAST(@InvoiceCustomerId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @InvoiceCustomerId = @ExistingInvoiceCustomerId;
        PRINT 'Using existing InvoiceCustomer with ID: ' + CAST(@InvoiceCustomerId AS NVARCHAR(36));
    END

    -- Check if InvoiceCustomerAddress already exists
    IF NOT EXISTS (SELECT 1 FROM InvoiceCustomerAddress WHERE InvoiceCustomerId = @InvoiceCustomerId AND AddressId = @AddressId)
    BEGIN
        -- Insert InvoiceCustomerAddress
        INSERT INTO InvoiceCustomerAddress (InvoiceCustomerId, AddressId, AddressSystemTypeId, 
                                        InvoiceCustomerAddressSystemType, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceCustomerId, @AddressId, @AddressSystemTypeId, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceCustomerAddress relationship';
    END
    ELSE
    BEGIN
        PRINT 'InvoiceCustomerAddress relationship already exists';
    END





    -- Check for BILLING email type

SELECT TOP 1 @EmailSystemTypeBilling = EmailAddressSystemTypeId 
FROM EmailAddressSystemType 
WHERE EmailAddressSystemTypeCode = 'BILLING' AND IsActive = 1;

IF @EmailSystemTypeBilling IS NULL
BEGIN
    SET @EmailSystemTypeBilling = NEWID();
    INSERT INTO EmailAddressSystemType (EmailAddressSystemTypeId, EmailAddressSystemTypeCode, EmailAddressSystemTypeName, IsActive, IsDeleted)
    VALUES (@EmailSystemTypeBilling, 'BILLING', 'Billing Email', 1, 0);
    
    PRINT 'Created new EmailAddressSystemType BILLING with ID: ' + CAST(@EmailSystemTypeBilling AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing EmailAddressSystemType BILLING with ID: ' + CAST(@EmailSystemTypeBilling AS NVARCHAR(36));
END

-- Check if customer already has a billing email
IF NOT EXISTS (
    SELECT 1 
    FROM CustomerEmailAddress cea
    JOIN EmailAddressSystemType east ON cea.EmailAddressSystemTypeId = east.EmailAddressSystemTypeId
    WHERE cea.CustomerId = @CustomerId AND east.EmailAddressSystemTypeCode = 'BILLING'
)
BEGIN
    -- Check if billing email address already exists
    DECLARE @ExistingBillingEmailId UNIQUEIDENTIFIER;
    DECLARE @BillingEmailAddress NVARCHAR(100) = 'billing@pciShieldcorp.com';
    
    SELECT TOP 1 @ExistingBillingEmailId = EmailAddressId 
    FROM EmailAddress 
    WHERE EmailAddressString = @BillingEmailAddress;
    
   
    
    IF @ExistingBillingEmailId IS NULL
    BEGIN
        SET @EmailAddressBilling = NEWID();
        INSERT INTO EmailAddress (EmailAddressId, EmailAddressString, IsDeleted)
        VALUES (@EmailAddressBilling, @BillingEmailAddress, 0);
        
        PRINT 'Created new billing EmailAddress with ID: ' + CAST(@EmailAddressBilling AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @EmailAddressBilling = @ExistingBillingEmailId;
        PRINT 'Using existing billing EmailAddress with ID: ' + CAST(@EmailAddressBilling AS NVARCHAR(36));
    END
    
    -- Link billing email to customer
    INSERT INTO CustomerEmailAddress (CustomerId, EmailAddressId, EmailAddressSystemTypeId, IsPrimaryEmailAddress, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@CustomerId, @EmailAddressBilling, @EmailSystemTypeBilling, 0, GETDATE(), @UserId, 0);
    
    PRINT 'Added billing email to customer';
END
ELSE
BEGIN
    PRINT 'Customer already has billing email';
END



-- Add BIL and DEL address types if needed


SELECT TOP 1 @AddressSystemTypeBIL = AddressSystemTypeId 
FROM AddressSystemType 
WHERE AddressSystemTypeCode = 'BIL' AND IsActive = 1;

SELECT TOP 1 @AddressSystemTypeDEL = AddressSystemTypeId 
FROM AddressSystemType 
WHERE AddressSystemTypeCode = 'DEL' AND IsActive = 1;

IF @AddressSystemTypeBIL IS NULL
BEGIN
    SET @AddressSystemTypeBIL = NEWID();
    INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, IsActive, BoundedContextId)
    VALUES (@AddressSystemTypeBIL, 'BIL', 'Billing', 1, @BoundedContextId);
    
    PRINT 'Created new AddressSystemType BIL with ID: ' + CAST(@AddressSystemTypeBIL AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing AddressSystemType BIL with ID: ' + CAST(@AddressSystemTypeBIL AS NVARCHAR(36));
END

IF @AddressSystemTypeDEL IS NULL
BEGIN
    SET @AddressSystemTypeDEL = NEWID();
    INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, IsActive, BoundedContextId)
    VALUES (@AddressSystemTypeDEL, 'DEL', 'Delivery', 1, @BoundedContextId);
    
    PRINT 'Created new AddressSystemType DEL with ID: ' + CAST(@AddressSystemTypeDEL AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing AddressSystemType DEL with ID: ' + CAST(@AddressSystemTypeDEL AS NVARCHAR(36));
END

-- Check if customer already has billing address
IF NOT EXISTS (
    SELECT 1 
    FROM CustomerAddress ca
    JOIN AddressSystemType ast ON ca.AddressSystemTypeId = ast.AddressSystemTypeId
    WHERE ca.CustomerId = @CustomerId AND ast.AddressSystemTypeCode = 'BIL'
)
BEGIN
    -- Check if similar billing address exists
    DECLARE @ExistingBillingAddressId UNIQUEIDENTIFIER;
    DECLARE @BillingStreet NVARCHAR(100) = '789 Billing St';
    
    SELECT TOP 1 @ExistingBillingAddressId = AddressId 
    FROM Address 
    WHERE AddressStreet = @BillingStreet AND CityId = @CityId AND StateId = @StateId;
    
  
    
    IF @ExistingBillingAddressId IS NULL
    BEGIN
        SET @BillingAddressId = NEWID();
        INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, CityId, DistrictId, CountyId, StateId, CountryId, 
                          CreatedDate, CreatedBy, IsDeleted)
        VALUES (@BillingAddressId, @BillingStreet, 'Suite 200', '94106', @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
              GETDATE(), @UserId, 0);
        
        PRINT 'Created new billing Address with ID: ' + CAST(@BillingAddressId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @BillingAddressId = @ExistingBillingAddressId;
        PRINT 'Using existing billing Address with ID: ' + CAST(@BillingAddressId AS NVARCHAR(36));
    END
    
    -- Link billing address to customer
    INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@BillingAddressId, @AddressSystemTypeBIL, @CustomerId, 0, GETDATE(), @UserId, 0);
    
    PRINT 'Added billing address to customer';
END
ELSE
BEGIN
    PRINT 'Customer already has billing address';
END

-- Check if customer already has delivery address
IF NOT EXISTS (
    SELECT 1 
    FROM CustomerAddress ca
    JOIN AddressSystemType ast ON ca.AddressSystemTypeId = ast.AddressSystemTypeId
    WHERE ca.CustomerId = @CustomerId AND ast.AddressSystemTypeCode = 'DEL'
)
BEGIN
    -- Check if similar delivery address exists
    DECLARE @ExistingDeliveryAddressId UNIQUEIDENTIFIER;
    DECLARE @DeliveryStreet NVARCHAR(100) = '456 Shipping Ave';
    
    SELECT TOP 1 @ExistingDeliveryAddressId = AddressId 
    FROM Address 
    WHERE AddressStreet = @DeliveryStreet AND CityId = @CityId AND StateId = @StateId;
    
   
    
    IF @ExistingDeliveryAddressId IS NULL
    BEGIN
        SET @DeliveryAddressId = NEWID();
        INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, CityId, DistrictId, CountyId, StateId, CountryId, 
                          CreatedDate, CreatedBy, IsDeleted)
        VALUES (@DeliveryAddressId, @DeliveryStreet, 'Floor 3', '94107', @CityId, @DistrictId, @CountyId, @StateId, @CountryId, 
              GETDATE(), @UserId, 0);
        
        PRINT 'Created new delivery Address with ID: ' + CAST(@DeliveryAddressId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @DeliveryAddressId = @ExistingDeliveryAddressId;
        PRINT 'Using existing delivery Address with ID: ' + CAST(@DeliveryAddressId AS NVARCHAR(36));
    END
    
    -- Link delivery address to customer
    INSERT INTO CustomerAddress (AddressId, AddressSystemTypeId, CustomerId, IsPrimaryAddress, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@DeliveryAddressId, @AddressSystemTypeDEL, @CustomerId, 0, GETDATE(), @UserId, 0);
    
    PRINT 'Added delivery address to customer';
END
ELSE
BEGIN
    PRINT 'Customer already has delivery address';
END







-- Check if REPRESENT contact type exists

SELECT TOP 1 @RepContactSystemTypeId = ContactSystemTypeId 
FROM ContactSystemType 
WHERE ContactSystemTypeCode = 'REPRESENT' AND IsActive = 1;

IF @RepContactSystemTypeId IS NULL
BEGIN
    SET @RepContactSystemTypeId = NEWID();
    INSERT INTO ContactSystemType (ContactSystemTypeId, ContactSystemTypeCode, ContactSystemTypeName, BoundedContextId, IsActive, IsDeleted)
    VALUES (@RepContactSystemTypeId, 'REPRESENT', 'Representative', @BoundedContextId, 1, 0);
    
    PRINT 'Created new ContactSystemType REPRESENT with ID: ' + CAST(@RepContactSystemTypeId AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing ContactSystemType REPRESENT with ID: ' + CAST(@RepContactSystemTypeId AS NVARCHAR(36));
END

-- Check if customer already has representative contact
IF NOT EXISTS (
    SELECT 1 
    FROM CustomerContact cc
    JOIN ContactSystemType cst ON cc.ContactSystemTypeId = cst.ContactSystemTypeId
    WHERE cc.CustomerId = @CustomerId AND cst.ContactSystemTypeCode = 'REPRESENT'
)
BEGIN
    -- Check if similar representative contact exists
    DECLARE @ExistingRepContactId UNIQUEIDENTIFIER;
    
    SELECT TOP 1 @ExistingRepContactId = ContactId 
    FROM Contact 
    WHERE FirstName = 'Robert' AND LastName = 'Johnson' AND Email = 'robert.johnson@pciShieldcorp.com';
    
   
    
    IF @ExistingRepContactId IS NULL
    BEGIN
        SET @RepresentativeContactId = NEWID();
        INSERT INTO Contact (ContactId, FirstName, LastName, Email, Phone, Mobile, IsPrimaryContact, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@RepresentativeContactId, 'Robert', 'Johnson', 'robert.johnson@pciShieldcorp.com', '555-222-3333', '555-444-5555', 0, GETDATE(), @UserId, 1, 0);
        
        PRINT 'Created new representative Contact with ID: ' + CAST(@RepresentativeContactId AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @RepresentativeContactId = @ExistingRepContactId;
        PRINT 'Using existing representative Contact with ID: ' + CAST(@RepresentativeContactId AS NVARCHAR(36));
    END
    
    -- Link representative contact to customer
    INSERT INTO CustomerContact (CustomerId, ContactId, ContactSystemTypeId, IsPrimaryContact, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@CustomerId, @RepresentativeContactId, @RepContactSystemTypeId, 0, GETDATE(), @UserId, 0);
    
    PRINT 'Added representative contact to customer';
END
ELSE
BEGIN
    PRINT 'Customer already has representative contact';
END









-- Check if NIT document type exists
DECLARE @DocSystemTypeNIT UNIQUEIDENTIFIER;
SELECT TOP 1 @DocSystemTypeNIT = DocumentIdentificationSystemTypeId 
FROM DocumentIdentificationSystemType 
WHERE DocumentIdentificationSystemTypeCode = 'NIT' AND IsActive = 1;

IF @DocSystemTypeNIT IS NULL
BEGIN
    SET @DocSystemTypeNIT = NEWID();
    
    INSERT INTO DocumentIdentificationSystemType (DocumentIdentificationSystemTypeId, DocumentIdentificationSystemTypeCode, 
                                            DocumentIdentificationSystemTypeName, CountryId, BoundedContextId, IsActive, IsDeleted)
    VALUES (@DocSystemTypeNIT, 'NIT', 'Tax Identification Number', @CountryId, @BoundedContextId, 1, 0);
    
    PRINT 'Created new DocumentIdentificationSystemType NIT with ID: ' + CAST(@DocSystemTypeNIT AS NVARCHAR(36));
END
ELSE
BEGIN
    PRINT 'Using existing DocumentIdentificationSystemType NIT with ID: ' + CAST(@DocSystemTypeNIT AS NVARCHAR(36));
END

-- Check if customer already has NIT document identification
IF NOT EXISTS (
    SELECT 1 
    FROM CustomerDocumentIdentification cdi
    JOIN DocumentIdentification di ON cdi.DocumentIdentificationId = di.DocumentIdentificationId
    JOIN DocumentIdentificationSystemType dist ON cdi.DocumentIdentificationSystemTypeId = dist.DocumentIdentificationSystemTypeId
    WHERE cdi.CustomerId = @CustomerId AND dist.DocumentIdentificationSystemTypeCode = 'NIT'
)
BEGIN
    DECLARE @DocIdentificationNIT UNIQUEIDENTIFIER = NEWID();
    
    -- Check if document number already exists
    DECLARE @ExistingNITDocId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ExistingNITDocId = DocumentIdentificationId 
    FROM DocumentIdentification 
    WHERE DocumentIdentificationNumber = '123456-789-0';
    
    IF @ExistingNITDocId IS NULL
    BEGIN
        INSERT INTO DocumentIdentification (DocumentIdentificationId, DocumentIdentificationNumber, IsDeleted)
        VALUES (@DocIdentificationNIT, '123456-789-0', 0);
        
        PRINT 'Created new NIT DocumentIdentification with ID: ' + CAST(@DocIdentificationNIT AS NVARCHAR(36));
    END
    ELSE
    BEGIN
        SET @DocIdentificationNIT = @ExistingNITDocId;
        PRINT 'Using existing NIT DocumentIdentification with ID: ' + CAST(@DocIdentificationNIT AS NVARCHAR(36));
    END
    
    INSERT INTO CustomerDocumentIdentification (CustomerId, DocumentIdentificationId, DocumentIdentificationSystemTypeId, 
                                             DocumentIdentificationVerified, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@CustomerId, @DocIdentificationNIT, @DocSystemTypeNIT, GETDATE(), GETDATE(), @UserId, 0);
    
    PRINT 'Added NIT identification to customer';
END
ELSE
BEGIN
    PRINT 'Customer already has NIT identification';
END










    -- Check if InvoiceDetailTaxSystemType already exists
    IF NOT EXISTS (SELECT 1 FROM InvoiceDetailTaxSystemType WHERE InvoiceDetailId = @InvoiceDetailId AND TaxSystemTypeId = @TaxSystemTypeId)
    BEGIN
        -- Insert InvoiceDetailTaxSystemType
        INSERT INTO InvoiceDetailTaxSystemType (InvoiceDetailId, TaxSystemTypeId, InvoiceDetailTaxSystemTypeAmount,
                                            InvoiceDetailTaxSystemTypeRate, InvoiceDetailTaxSystemTypeSign,
                                            CreatedDate, CreatedBy, IsDeleted)
        VALUES (@InvoiceDetailId, @TaxSystemTypeId, 65.00, 0.13, 1, GETDATE(), @UserId, 0);
        
        PRINT 'Created new InvoiceDetailTaxSystemType relationship';
    END
    ELSE
    BEGIN
        PRINT 'InvoiceDetailTaxSystemType relationship already exists';
    END

    COMMIT TRANSACTION;
    SELECT 'Script executed successfully. Full customer and invoice graph created.' AS Result;
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