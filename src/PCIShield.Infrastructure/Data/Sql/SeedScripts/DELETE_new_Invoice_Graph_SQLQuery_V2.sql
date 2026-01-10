USE PCIShield_TenantAXBXCX
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Declare variables to identify the invoice to delete
    DECLARE @InvoiceId UNIQUEIDENTIFIER;
    DECLARE @CustomerId UNIQUEIDENTIFIER;
    DECLARE @InvoiceCreatedDate DATETIME;
    
    -- You can identify the invoice by ID if you know it
    -- SET @InvoiceId = 'YOUR-INVOICE-GUID-HERE';
    
    -- Or find it by CustomerID and being the most recent one
    -- (This will find the invoice we created in the previous script)
    SELECT TOP 1 @InvoiceId = InvoiceId, @CustomerId = CustomerId, @InvoiceCreatedDate = CreatedDate
    FROM Invoice
    WHERE CustomerId IN (SELECT CustomerId FROM Customer WHERE CustomerCode = 'CUST002')
    ORDER BY CreatedDate DESC;
    
    IF @InvoiceId IS NULL
    BEGIN
        RAISERROR('Invoice not found with the specified criteria', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Deleting invoice with ID: ' + CAST(@InvoiceId AS NVARCHAR(36));
    PRINT 'For customer ID: ' + CAST(@CustomerId AS NVARCHAR(36));
    PRINT 'Invoice created on: ' + CONVERT(VARCHAR, @InvoiceCreatedDate, 121);
    
    -- Define a very tight time window around invoice creation (+/- 3 seconds)
    -- This ensures we only delete entities created in the same batch
    DECLARE @TimeWindowStart DATETIME = DATEADD(SECOND, -3, @InvoiceCreatedDate);
    DECLARE @TimeWindowEnd DATETIME = DATEADD(SECOND, 3, @InvoiceCreatedDate);
    
    PRINT 'Using time window: ' + CONVERT(VARCHAR, @TimeWindowStart, 121) + ' to ' + CONVERT(VARCHAR, @TimeWindowEnd, 121);
    
    -- Create temporary table to store IDs of entities to delete
    CREATE TABLE #EntitiesToDelete (
        EntityType NVARCHAR(100),
        EntityId UNIQUEIDENTIFIER,
        EntityName NVARCHAR(255),
        CreatedDate DATETIME NULL
    );
    
    -- Track the invoice for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'Invoice', InvoiceId, 'Invoice #' + CAST(ROW_NUMBER() OVER(ORDER BY InvoiceId) AS NVARCHAR(10)), CreatedDate
    FROM Invoice 
    WHERE InvoiceId = @InvoiceId;
    
    -- ========================== INVOICE DETAIL SECTION ==========================
    -- Find all invoice details for this invoice
    DECLARE @InvoiceDetailIds TABLE (InvoiceDetailId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceDetailIds
    SELECT InvoiceDetailId FROM InvoiceDetail 
    WHERE InvoiceId = @InvoiceId
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoice details to delete';
    
    -- Track these invoice details for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'InvoiceDetail', InvoiceDetailId, 'Invoice Detail', CreatedDate
    FROM InvoiceDetail 
    WHERE InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);
    
    -- ========================== INVOICE DETAIL GROUP SECTION ==========================
    -- Find invoice detail groups for this invoice
    DECLARE @InvoiceDetailGroupIds TABLE (InvoiceDetailGroupId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceDetailGroupIds
    SELECT InvoiceDetailGroupId FROM InvoiceDetailGroup 
    WHERE InvoiceId = @InvoiceId
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoice detail groups to delete';
    
    -- Track these invoice detail groups for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'InvoiceDetailGroup', InvoiceDetailGroupId, 'Invoice Detail Group', CreatedDate
    FROM InvoiceDetailGroup 
    WHERE InvoiceDetailGroupId IN (SELECT InvoiceDetailGroupId FROM @InvoiceDetailGroupIds);
    
    -- ========================== INVOICE CUSTOMER SECTION ==========================
    -- Find invoice customers for this invoice
    DECLARE @InvoiceCustomerIds TABLE (InvoiceCustomerId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceCustomerIds
    SELECT InvoiceCustomerId FROM InvoiceCustomer 
    WHERE InvoiceId = @InvoiceId
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoice customers to delete';
    
    -- Track these invoice customers for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'InvoiceCustomer', InvoiceCustomerId, 'Invoice Customer', CreatedDate
    FROM InvoiceCustomer 
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    
    -- ========================== INVOICE ACCOUNT RECEIVABLE SECTION ==========================
    -- Find invoice account receivables for this invoice
    DECLARE @InvoiceARIds TABLE (InvoiceAccountReceivableId UNIQUEIDENTIFIER);
    INSERT INTO @InvoiceARIds
    SELECT InvoiceAccountReceivableId FROM InvoiceAccountReceivable 
    WHERE InvoiceId = @InvoiceId
    AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
    
    PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' invoice account receivables to delete';
    
    -- Track these invoice account receivables for deletion
    INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
    SELECT 'InvoiceAccountReceivable', InvoiceAccountReceivableId, 'Invoice Account Receivable', CreatedDate
    FROM InvoiceAccountReceivable 
    WHERE InvoiceAccountReceivableId IN (SELECT InvoiceAccountReceivableId FROM @InvoiceARIds);
    
    -- ========================== ELECTRONIC DOCUMENT SECTION ==========================
    -- Find electronic documents created for this invoice
    DECLARE @ElectronicDocumentIds TABLE (ElectronicDocumentId UNIQUEIDENTIFIER);
    
    IF OBJECT_ID('ElectronicDocumentInvoice', 'U') IS NOT NULL
    BEGIN
        INSERT INTO @ElectronicDocumentIds
        SELECT DISTINCT ed.ElectronicDocumentId
        FROM ElectronicDocumentInvoice edi
        JOIN ElectronicDocument ed ON edi.ElectronicDocumentId = ed.ElectronicDocumentId
        WHERE edi.InvoiceId = @InvoiceId
        AND ed.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;
        
        PRINT 'Found ' + CAST(@@ROWCOUNT AS VARCHAR) + ' electronic documents to delete';
        
        -- Track these electronic documents for deletion
        INSERT INTO #EntitiesToDelete (EntityType, EntityId, EntityName, CreatedDate)
        SELECT 'ElectronicDocument', ElectronicDocumentId, 'Electronic Document', CreatedDate
        FROM ElectronicDocument 
        WHERE ElectronicDocumentId IN (SELECT ElectronicDocumentId FROM @ElectronicDocumentIds);
    END
    
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
    
    -- 3. Delete InvoiceCustomerDocumentIdentification relationships
    DELETE FROM InvoiceCustomerDocumentIdentification
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    PRINT 'Deleted InvoiceCustomerDocumentIdentification relationships';
    
    -- 4. Delete InvoiceCustomerEmailAddress relationships
    DELETE FROM InvoiceCustomerEmailAddress
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    PRINT 'Deleted InvoiceCustomerEmailAddress relationships';
    
    -- 5. Delete InvoiceCustomerPhoneNumber relationships
    DELETE FROM InvoiceCustomerPhoneNumber
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    PRINT 'Deleted InvoiceCustomerPhoneNumber relationships';
    
    -- 6. Delete ElectronicDocumentInvoice relationships
    IF OBJECT_ID('ElectronicDocumentInvoice', 'U') IS NOT NULL
    BEGIN
        DELETE FROM ElectronicDocumentInvoice
        WHERE InvoiceId = @InvoiceId;
        PRINT 'Deleted ElectronicDocumentInvoice relationships';
        
        -- Delete ElectronicDocument records
        DELETE FROM ElectronicDocument
        WHERE ElectronicDocumentId IN (SELECT ElectronicDocumentId FROM @ElectronicDocumentIds);
        PRINT 'Deleted ElectronicDocument records';
    END
    
    -- 7. Delete InvoiceDetailJunction relationships if any exist
    IF OBJECT_ID('InvoiceDetailJunction', 'U') IS NOT NULL
    BEGIN
        DELETE FROM InvoiceDetailJunction
        WHERE InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);
        PRINT 'Deleted InvoiceDetailJunction relationships';
    END
    
    -- 8. Delete InvoiceDetail records
    DELETE FROM InvoiceDetail
    WHERE InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);
    PRINT 'Deleted InvoiceDetail records';
    
    -- 9. Delete InvoiceDetailGroup records
    DELETE FROM InvoiceDetailGroup
    WHERE InvoiceDetailGroupId IN (SELECT InvoiceDetailGroupId FROM @InvoiceDetailGroupIds);
    PRINT 'Deleted InvoiceDetailGroup records';
    
    -- 10. Delete InvoiceTaxSystemType relationships
    DELETE FROM InvoiceTaxSystemType
    WHERE InvoiceId = @InvoiceId;
    PRINT 'Deleted InvoiceTaxSystemType relationships';
    
    -- 11. Delete InvoiceContact relationships
    DELETE FROM InvoiceContact
    WHERE InvoiceId = @InvoiceId;
    PRINT 'Deleted InvoiceContact relationships';
    
    -- 12. Delete InvoiceSalesperson relationships
    DELETE FROM InvoiceSalesperson
    WHERE InvoiceId = @InvoiceId;
    PRINT 'Deleted InvoiceSalesperson relationships';
    
    -- 13. Delete InvoiceAccountReceivablePayment relationships if any
    DELETE FROM InvoiceAccountReceivablePayment
    WHERE InvoiceAccountReceivableId IN (SELECT InvoiceAccountReceivableId FROM @InvoiceARIds);
    PRINT 'Deleted InvoiceAccountReceivablePayment relationships';
    
    -- 14. Delete InvoiceAccountReceivable records
    DELETE FROM InvoiceAccountReceivable
    WHERE InvoiceAccountReceivableId IN (SELECT InvoiceAccountReceivableId FROM @InvoiceARIds);
    PRINT 'Deleted InvoiceAccountReceivable records';
    
    -- 15. Delete InvoiceCustomer records
    DELETE FROM InvoiceCustomer
    WHERE InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds);
    PRINT 'Deleted InvoiceCustomer records';
    
    -- 16. Delete Invoice record
    DELETE FROM Invoice
    WHERE InvoiceId = @InvoiceId;
    PRINT 'Deleted Invoice record';
    
    -- 17. Now delete the actual entity records, but only those that were created for this invoice
    -- Delete Products (only those we're sure were created just for this invoice)
    DELETE FROM Product
    WHERE ProductId IN (SELECT EntityId FROM #EntitiesToDelete WHERE EntityType = 'Product');
    PRINT 'Deleted Product records created for this invoice';
    
    -- Clean up
    DROP TABLE #EntitiesToDelete;
    
    COMMIT TRANSACTION;
    SELECT 'Invoice and all related data deleted successfully.' AS Result;
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