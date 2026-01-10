USE PCIShield_Core_Db
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Declare variables to identify the customer to delete
    DECLARE @CustomerCode VARCHAR(50) = 'CUST001';
    DECLARE @CustomerCommercialName VARCHAR(100) = 'PCIShield Corporation';
    DECLARE @CustomerId UNIQUEIDENTIFIER;
    DECLARE @CustomerCreatedDate DATETIME;
    
    -- Get the CustomerId and creation date we want to delete
    SELECT @CustomerId = CustomerId, @CustomerCreatedDate = CreatedDate
    FROM Customer 
    WHERE CustomerCode = @CustomerCode AND CustomerCommercialName = @CustomerCommercialName;
    
    IF @CustomerId IS NULL
    BEGIN
        RAISERROR('Customer not found with code %s and name %s', 16, 1, @CustomerCode, @CustomerCommercialName);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Deleting customer with ID: ' + CAST(@CustomerId AS NVARCHAR(36));
    PRINT 'Customer created on: ' + CONVERT(VARCHAR, @CustomerCreatedDate, 121);
    
    -- Define a very tight time window around customer creation (+/- 3 seconds)
    -- This ensures we only delete entities created in the same batch
    DECLARE @TimeWindowStart DATETIME = DATEADD(SECOND, -3, @CustomerCreatedDate);
    DECLARE @TimeWindowEnd DATETIME = DATEADD(SECOND, 3, @CustomerCreatedDate);
    
    PRINT 'Using time window: ' + CONVERT(VARCHAR, @TimeWindowStart, 121) + ' to ' + CONVERT(VARCHAR, @TimeWindowEnd, 121);
    
    -- Create temporary tables to store IDs of entities to delete
    CREATE TABLE #EntitiesToDelete (
        EntityType NVARCHAR(100),
        EntityId UNIQUEIDENTIFIER,
        EntityName NVARCHAR(255),
        CreatedDate DATETIME NULL
    );
    
    -- Find all invoices for this customer created in the same time window
    DECLARE @InvoiceIds TABLE (InvoiceId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceIds
    SELECT InvoiceId FROM Invoice 
    WHERE CustomerId = @CustomerId
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoices to delete';
    
    -- Track these invoices for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'Invoice', InvoiceId, 'Invoice #' + CAST(ROW_NUMBER() OVER(ORDER BY InvoiceId) AS NVARCHAR(10)), CreatedDate
    FROM Invoice 
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
    
    -- Find all invoice customers for these invoices
    DECLARE @InvoiceCustomerIds TABLE (InvoiceCustomerId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceCustomerIds
    SELECT InvoiceCustomerId FROM InvoiceCustomer 
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds)
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoice customers to delete';
    
    -- Track these invoice customers for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'InvoiceCustomer', InvoiceCustomerId, 'Invoice Customer', CreatedDate
    FROM InvoiceCustomer 
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    
    -- Find all invoice details for these invoices
    DECLARE @InvoiceDetailIds TABLE (InvoiceDetailId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceDetailIds
    SELECT InvoiceDetailId FROM InvoiceDetail 
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds)
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoice details to delete';
    
    -- Track these invoice details for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'InvoiceDetail', InvoiceDetailId, 'Invoice Detail', CreatedDate
    FROM InvoiceDetail 
    WHERE InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);
    
    -- ========================== ADDRESS SECTION ==========================
    -- Find addresses created specifically for this customer
    DECLARE @AddressIds TABLE (AddressId UNIQUEIDENTIFIER);
    INSERT INTO @AddressIds
    SELECT DISTINCT a.AddressId
    FROM CustomerAddress ca
    JOIN Address a ON ca.AddressId = a.AddressId
    WHERE ca.CustomerId = @CustomerId
    AND ca.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    -- Only delete addresses that were created exclusively for this customer
    -- If an address is linked to multiple customers, don't delete it
    AND NOT EXISTS (
        SELECT 1 FROM CustomerAddress ca2 
        WHERE ca2.AddressId = a.AddressId 
        AND ca2.CustomerId != @CustomerId
    );
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' addresses to delete';
    
    -- Track these addresses for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'Address', AddressId, 'Address', NULL
    FROM Address 
    WHERE AddressId IN (SELECT AddressId FROM @AddressIds);
    
    -- ========================== CONTACT SECTION ==========================
    -- Find contacts created specifically for this customer
    DECLARE @ContactIds TABLE (ContactId UNIQUEIDENTIFIER);
    INSERT INTO @ContactIds
    SELECT DISTINCT c.ContactId
    FROM CustomerContact cc
    JOIN Contact c ON cc.ContactId = c.ContactId
    WHERE cc.CustomerId = @CustomerId
    AND cc.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    -- Only delete contacts that were created exclusively for this customer
    AND NOT EXISTS (
        SELECT 1 FROM CustomerContact cc2 
        WHERE cc2.ContactId = c.ContactId 
        AND cc2.CustomerId != @CustomerId
    );
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' contacts to delete';
    
    -- Track these contacts for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'Contact', ContactId, 'Contact', CreatedDate
    FROM Contact 
    WHERE ContactId IN (SELECT ContactId FROM @ContactIds);
    
    -- ========================== EMAIL SECTION ==========================
    -- Find email addresses created specifically for this customer
    DECLARE @EmailAddressIds TABLE (EmailAddressId UNIQUEIDENTIFIER);
    INSERT INTO @EmailAddressIds
    SELECT DISTINCT e.EmailAddressId
    FROM CustomerEmailAddress cea
    JOIN EmailAddress e ON cea.EmailAddressId = e.EmailAddressId
    WHERE cea.CustomerId = @CustomerId
    AND cea.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    -- Only delete email addresses that were created exclusively for this customer
    AND NOT EXISTS (
        SELECT 1 FROM CustomerEmailAddress cea2 
        WHERE cea2.EmailAddressId = e.EmailAddressId 
        AND cea2.CustomerId != @CustomerId
    );
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' email addresses to delete';
    
    -- Track these email addresses for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'EmailAddress', EmailAddressId, 'Email Address', NULL
    FROM EmailAddress 
    WHERE EmailAddressId IN (SELECT EmailAddressId FROM @EmailAddressIds);
    
    -- ========================== PHONE SECTION ==========================
    -- Find phone numbers created specifically for this customer
    DECLARE @PhoneNumberIds TABLE (PhoneNumberId UNIQUEIDENTIFIER);
    INSERT INTO @PhoneNumberIds
    SELECT DISTINCT p.PhoneNumberId
    FROM CustomerPhoneNumber cpn
    JOIN PhoneNumber p ON cpn.PhoneNumberId = p.PhoneNumberId
    WHERE cpn.CustomerId = @CustomerId
    AND cpn.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    -- Only delete phone numbers that were created exclusively for this customer
    AND NOT EXISTS (
        SELECT 1 FROM CustomerPhoneNumber cpn2 
        WHERE cpn2.PhoneNumberId = p.PhoneNumberId 
        AND cpn2.CustomerId != @CustomerId
    );
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' phone numbers to delete';
    
    -- Track these phone numbers for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'PhoneNumber', PhoneNumberId, 'Phone Number', NULL
    FROM PhoneNumber 
    WHERE PhoneNumberId IN (SELECT PhoneNumberId FROM @PhoneNumberIds);
    
    -- ========================== DOCUMENT SECTION ==========================
    -- Find document identifications created specifically for this customer
    DECLARE @DocIdentificationIds TABLE (DocumentIdentificationId UNIQUEIDENTIFIER);
    INSERT INTO @DocIdentificationIds
    SELECT DISTINCT di.DocumentIdentificationId
    FROM CustomerDocumentIdentification cdi
    JOIN DocumentIdentification di ON cdi.DocumentIdentificationId = di.DocumentIdentificationId
    WHERE cdi.CustomerId = @CustomerId
    AND cdi.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    -- Only delete documents that were created exclusively for this customer
    AND NOT EXISTS (
        SELECT 1 FROM CustomerDocumentIdentification cdi2 
        WHERE cdi2.DocumentIdentificationId = di.DocumentIdentificationId 
        AND cdi2.CustomerId != @CustomerId
    );
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' document identifications to delete';
    
    -- Track these document identifications for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'DocumentIdentification', DocumentIdentificationId, 'Document ID', NULL
    FROM DocumentIdentification 
    WHERE DocumentIdentificationId IN (SELECT DocumentIdentificationId FROM @DocIdentificationIds);
    
    -- ========================== PRODUCT SECTION ==========================
    -- For products, we'll be more careful - only delete if they were created in the time window
    -- and aren't used by other invoice details
    DECLARE @ProductIds TABLE (ProductId UNIQUEIDENTIFIER);
    INSERT INTO @ProductIds
    SELECT DISTINCT p.ProductId
    FROM InvoiceDetail id
    JOIN Product p ON id.ProductId = p.ProductId
    WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds)
    AND p.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    -- Only delete products that aren't used in other invoices
    AND NOT EXISTS (
        SELECT 1 FROM InvoiceDetail id2 
        WHERE id2.ProductId = p.ProductId 
        AND id2.InvoiceDetailId NOT IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds)
    );
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' products to delete';
    
    -- Track these products for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'Product', ProductId, 'Product', CreatedDate
    FROM Product 
    WHERE ProductId IN (SELECT ProductId FROM @ProductIds);
    
    -- ========================== ELECTRONIC DOCUMENT SECTION ==========================
    -- Find electronic documents created for these invoices
    DECLARE @ElectronicDocumentIds TABLE (ElectronicDocumentId UNIQUEIDENTIFIER);
    
    IF OBJECT_ID('ElectronicDocumentInvoice', 'U') IS NOT NULL
    BEGIN
        INSERT INTO @ElectronicDocumentIds
        SELECT DISTINCT ed.ElectronicDocumentId
        FROM ElectronicDocumentInvoice edi
        JOIN ElectronicDocument ed ON edi.ElectronicDocumentId = ed.ElectronicDocumentId
        WHERE edi.InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds)
        AND ed.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
        
        PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' electronic documents to delete';
        
        -- Track these electronic documents for deletion
        INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
        SELECT 'ElectronicDocument', ElectronicDocumentId, 'Electronic Document', CreatedDate
        FROM ElectronicDocument 
        WHERE ElectronicDocumentId IN (SELECT ElectronicDocumentId FROM @ElectronicDocumentIds);
    END
    
    -- ========================== DELETION PROCESS ==========================
    -- Print summary of entities marked for deletion
    SELECT EntityType, COUNT(*) AS Count 
    FROM #EntitiesToDelete 
    GROUP BY EntityType 
    ORDER BY EntityType;
    
    -- Now delete all relationships and entities in the correct order to maintain referential integrity
    
    -- 1. Delete InvoiceDetailTaxSystemType relationships
    DELETE FROM InvoiceDetailTaxSystemType
    WHERE InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);
    PRINT 'Deleted InvoiceDetailTaxSystemType relationships';
    
    -- 2. Delete InvoiceCustomerAddress relationships
    DELETE FROM InvoiceCustomerAddress
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    PRINT 'Deleted InvoiceCustomerAddress relationships';
    
    -- 3. Delete ElectronicDocumentInvoice relationships and ElectronicDocuments
    IF OBJECT_ID('ElectronicDocumentInvoice', 'U') IS NOT NULL
    BEGIN
        DELETE FROM ElectronicDocumentInvoice
        WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
        PRINT 'Deleted ElectronicDocumentInvoice relationships';
        
        DELETE FROM ElectronicDocument
        WHERE ElectronicDocumentId IN (SELECT ElectronicDocumentId FROM @ElectronicDocumentIds);
        PRINT 'Deleted ElectronicDocument records';
    END
    
    -- 4. Delete InvoiceDetail records
    DELETE FROM InvoiceDetail
    WHERE InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);
    PRINT 'Deleted InvoiceDetail records';
    
    -- 5. Delete InvoiceTaxSystemType relationships
    DELETE FROM InvoiceTaxSystemType
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
    PRINT 'Deleted InvoiceTaxSystemType relationships';
    
    -- 6. Delete InvoiceContact relationships
    DELETE FROM InvoiceContact
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
    PRINT 'Deleted InvoiceContact relationships';
    
    -- 7. Delete InvoiceSalesperson relationships
    DELETE FROM InvoiceSalesperson
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
    PRINT 'Deleted InvoiceSalesperson relationships';
    
    -- 8. Delete InvoiceAccountReceivable records
    DELETE FROM InvoiceAccountReceivable
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
    PRINT 'Deleted InvoiceAccountReceivable records';
    
    -- 9. Delete InvoiceCustomer records
    DELETE FROM InvoiceCustomer
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    PRINT 'Deleted InvoiceCustomer records';
    
    -- 10. Delete Invoice records
    DELETE FROM Invoice
    WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);
    PRINT 'Deleted Invoice records';
    
    -- 11. Delete CustomerAddress relationships
    DELETE FROM CustomerAddress
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerAddress relationships';
    
    -- 12. Delete CustomerContact relationships
    DELETE FROM CustomerContact
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerContact relationships';
    
    -- 13. Delete CustomerEmailAddress relationships
    DELETE FROM CustomerEmailAddress
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerEmailAddress relationships';
    
    -- 14. Delete CustomerPhoneNumber relationships
    DELETE FROM CustomerPhoneNumber
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerPhoneNumber relationships';
    
    -- 15. Delete CustomerDocumentIdentification relationships
    DELETE FROM CustomerDocumentIdentification
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerDocumentIdentification relationships';
    
    -- 16. Delete CustomerSalesperson relationships
    DELETE FROM CustomerSalesperson
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerSalesperson relationships';
    
    -- 17. Delete CustomerEconomicActivitySystemType relationships
    DELETE FROM CustomerEconomicActivitySystemType
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerEconomicActivitySystemType relationships';
    
    -- 18. Delete CustomerTaxSystemType relationships
    DELETE FROM CustomerTaxSystemType
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted CustomerTaxSystemType relationships';
    
    -- 19. Delete Customer record
    DELETE FROM Customer
    WHERE CustomerId = @CustomerId;
    PRINT 'Deleted Customer record';
    
    -- 20. Now delete the actual entity records, but only those that were created for this customer
    -- Delete Products
    DELETE FROM Product
    WHERE ProductId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'Product');
    PRINT 'Deleted Product records created for this customer';
    
    -- Delete Addresses
    DELETE FROM Address
    WHERE AddressId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'Address');
    PRINT 'Deleted Address records created for this customer';
    
    -- Delete Contacts
    DELETE FROM Contact
    WHERE ContactId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'Contact');
    PRINT 'Deleted Contact records created for this customer';
    
    -- Delete EmailAddresses
    DELETE FROM EmailAddress
    WHERE EmailAddressId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'EmailAddress');
    PRINT 'Deleted EmailAddress records created for this customer';
    
    -- Delete PhoneNumbers
    DELETE FROM PhoneNumber
    WHERE PhoneNumberId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'PhoneNumber');
    PRINT 'Deleted PhoneNumber records created for this customer';
    
    -- Delete DocumentIdentifications
    DELETE FROM DocumentIdentification
    WHERE DocumentIdentificationId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'DocumentIdentification');
    PRINT 'Deleted DocumentIdentification records created for this customer';
    
    -- NOTE: We do NOT delete system types like CustomerUserType, TaxpayerSystemType, etc.,
    -- as these are shared reference data
    
    -- Clean up
    DROP TABLE #EntitiesToDelete;
    
    COMMIT TRANSACTION;
    SELECT 'Customer and all related data deleted successfully.' AS Result;
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