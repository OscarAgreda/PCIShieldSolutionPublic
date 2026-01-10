USE PCIShield_Core_Db
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Get reference IDs from existing database records
    DECLARE @ContinentId UNIQUEIDENTIFIER;
    DECLARE @SubcontinentId UNIQUEIDENTIFIER;
    DECLARE @CurrencyId UNIQUEIDENTIFIER;
    DECLARE @BoundedContextId UNIQUEIDENTIFIER;
    DECLARE @UserId UNIQUEIDENTIFIER;

    -- Get user ID for audit
    SELECT TOP 1 @UserId = '2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0';

    -- Get any available Continent ID
    SELECT TOP 1 @ContinentId = ContinentId FROM Continent;
    
    IF @ContinentId IS NULL
    BEGIN
        RAISERROR('No Continent records found in database. Cannot proceed.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Using ContinentId: ' + CAST(@ContinentId AS NVARCHAR(36));

    -- Get any available Subcontinent ID or use NULL if allowed
    SELECT TOP 1 @SubcontinentId = SubcontinentId FROM Subcontinent;
    PRINT 'Using SubcontinentId: ' + ISNULL(CAST(@SubcontinentId AS NVARCHAR(36)), 'NULL');

    -- Get any available Currency ID
    SELECT TOP 1 @CurrencyId = CurrencyId FROM Currency;
    
    IF @CurrencyId IS NULL
    BEGIN
        RAISERROR('No Currency records found in database. Cannot proceed.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Using CurrencyId: ' + CAST(@CurrencyId AS NVARCHAR(36));

    -- Get active BoundedContext ID
    SELECT TOP 1 @BoundedContextId = BoundedContextId FROM BoundedContext WHERE IsActive = 1;
    
    IF @BoundedContextId IS NULL
    BEGIN
        RAISERROR('No active BoundedContext found. Cannot proceed.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Using BoundedContextId: ' + CAST(@BoundedContextId AS NVARCHAR(36));

    -- Create or get El Salvador country
    DECLARE @CountryId UNIQUEIDENTIFIER;
    SELECT TOP 1 @CountryId = CountryId 
    FROM Country 
    WHERE CountryName = 'El Salvador' OR CountryCodeISO2 = 'SV';

    IF @CountryId IS NULL
    BEGIN
        SET @CountryId = NEWID();
        INSERT INTO Country (CountryId, CountryName, CountryCodeISO2, CountryCodeISO3, CountryIdISO, 
                           CountryAreaCode, ContinentId, SubcontinentId, CurrencyId, IsActive)
        VALUES (@CountryId, 'El Salvador', 'SV', 'SLV', 222, '503', 
               @ContinentId, @SubcontinentId, @CurrencyId, 1);
        PRINT 'Created new Country: El Salvador';
    END
    ELSE
    BEGIN
        PRINT 'Using existing Country: El Salvador';
    END

    -- Create or get San Salvador department (state)
    DECLARE @StateId UNIQUEIDENTIFIER;
    SELECT TOP 1 @StateId = StateId 
    FROM State 
    WHERE StateName = 'San Salvador' AND CountryId = @CountryId;

    IF @StateId IS NULL
    BEGIN
        SET @StateId = NEWID();
        INSERT INTO State (StateId, StateCode, StateName, CountryId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@StateId, 'SS', 'San Salvador', @CountryId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new State: San Salvador';
    END
    ELSE
    BEGIN
        PRINT 'Using existing State: San Salvador';
    END

    -- Create or get San Salvador municipality (county)
    DECLARE @CountyId UNIQUEIDENTIFIER;
    SELECT TOP 1 @CountyId = CountyId 
    FROM County 
    WHERE CountyName = 'San Salvador Municipality' AND StateId = @StateId;

    IF @CountyId IS NULL
    BEGIN
        SET @CountyId = NEWID();
        INSERT INTO County (CountyId, CountyCode, CountyName, CountyPostalCode, StateId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CountyId, 'SSM', 'San Salvador Municipality', '01101', @StateId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new County: San Salvador Municipality';
    END
    ELSE
    BEGIN
        PRINT 'Using existing County: San Salvador Municipality';
    END

    -- Create or get Historic Center district
    DECLARE @DistrictId UNIQUEIDENTIFIER;
    SELECT TOP 1 @DistrictId = DistrictId 
    FROM District 
    WHERE DistrictName = 'Centro Histórico' AND CountyId = @CountyId;

    IF @DistrictId IS NULL
    BEGIN
        SET @DistrictId = NEWID();
        INSERT INTO District (DistrictId, DistrictCode, DistrictName, CountyId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@DistrictId, 'CHD', 'Centro Histórico', @CountyId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new District: Centro Histórico';
    END
    ELSE
    BEGIN
        PRINT 'Using existing District: Centro Histórico';
    END

    -- Create or get San Salvador city
    DECLARE @CityId UNIQUEIDENTIFIER;
    SELECT TOP 1 @CityId = CityId 
    FROM City 
    WHERE CityName = 'San Salvador' AND StateId = @StateId;

    IF @CityId IS NULL
    BEGIN
        SET @CityId = NEWID();
        INSERT INTO City (CityId, CityName, StateId, CreatedDate, CreatedBy, IsActive, IsDeleted)
        VALUES (@CityId, 'San Salvador', @StateId, GETDATE(), @UserId, 1, 0);
        PRINT 'Created new City: San Salvador';
    END
    ELSE
    BEGIN
        PRINT 'Using existing City: San Salvador';
    END

    -- Check for needed AddressSystemTypes based on specification
    DECLARE @AddressSystemTypeBIL UNIQUEIDENTIFIER;
    DECLARE @AddressSystemTypeDEL UNIQUEIDENTIFIER;
    DECLARE @AddressSystemTypePRI UNIQUEIDENTIFIER;

    SELECT TOP 1 @AddressSystemTypeBIL = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'BIL' AND IsActive = 1;

    SELECT TOP 1 @AddressSystemTypeDEL = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'DEL' AND IsActive = 1;

    SELECT TOP 1 @AddressSystemTypePRI = AddressSystemTypeId 
    FROM AddressSystemType 
    WHERE AddressSystemTypeCode = 'PRI' AND IsActive = 1;

    IF @AddressSystemTypeBIL IS NULL
    BEGIN
        SET @AddressSystemTypeBIL = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeBIL, 'BIL', 'Billing', @BoundedContextId, 1);
        PRINT 'Created new AddressSystemType: BIL';
    END

    IF @AddressSystemTypeDEL IS NULL
    BEGIN
        SET @AddressSystemTypeDEL = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypeDEL, 'DEL', 'Delivery', @BoundedContextId, 1);
        PRINT 'Created new AddressSystemType: DEL';
    END

    IF @AddressSystemTypePRI IS NULL
    BEGIN
        SET @AddressSystemTypePRI = NEWID();
        INSERT INTO AddressSystemType (AddressSystemTypeId, AddressSystemTypeCode, AddressSystemTypeName, BoundedContextId, IsActive)
        VALUES (@AddressSystemTypePRI, 'PRI', 'Primary', @BoundedContextId, 1);
        PRINT 'Created new AddressSystemType: PRI';
    END

    -- Create new address in San Salvador
    DECLARE @AddressId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO Address (AddressId, AddressStreet, AddressStreetLine2, ZipCode, CityId, DistrictId, 
                      CountyId, StateId, CountryId, CreatedDate, CreatedBy, IsDeleted)
    VALUES (@AddressId, 'Avenida España 1332', 'Edificio Comercial Plaza', '01101', @CityId, @DistrictId, 
          @CountyId, @StateId, @CountryId, GETDATE(), @UserId, 0);
    
    PRINT 'Created new Address in San Salvador with ID: ' + CAST(@AddressId AS NVARCHAR(36));

    COMMIT TRANSACTION;
    SELECT 'Script executed successfully. Created address hierarchy for El Salvador.' AS Result;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT 'Error occurred at Line ' + CAST(@ErrorLine AS NVARCHAR) + ': ' + @ErrorMessage;
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH