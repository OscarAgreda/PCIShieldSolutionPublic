USE PCIShield_TenantAXBXCX
GO

-- Create a temporary table to store our results with additional fields for different ID types
CREATE TABLE #InvoiceGraphIds (
    EntityType NVARCHAR(100),           -- Type of entity (Invoice, InvoiceDetail, Product, etc.)
    EntityName NVARCHAR(255),           -- Descriptive name including codes where available
    EntityId UNIQUEIDENTIFIER NULL,     -- Unique identifier of the entity (for UUID tables)
    RowId INT NULL,                     -- Row ID for junction tables with INT IDENTITY columns
    ParentEntityType NVARCHAR(100) NULL, -- Type of parent entity
    ParentEntityId UNIQUEIDENTIFIER NULL, -- ID of parent entity (for UUID tables)
    ParentRowId INT NULL,               -- Row ID of parent entity (for INT IDENTITY tables)
    CreatedDate DATETIME NULL,           -- When the entity was created
    OrderDisplay INT,                    -- For organizing output in logical order
    IsOrphaned BIT DEFAULT 0             -- Flag for problematic relationships
);

-- Main identifier variables
DECLARE @InvoiceId UNIQUEIDENTIFIER;
DECLARE @InvoiceCreatedDate DATETIME;
DECLARE @CustomerId UNIQUEIDENTIFIER;

-- Find the specific invoice we want to analyze
-- You can identify by customer code or other criteria
SELECT TOP 1 @InvoiceId = i.InvoiceId, @InvoiceCreatedDate = i.CreatedDate, @CustomerId = i.CustomerId
FROM Invoice i
JOIN Customer c ON i.CustomerId = c.CustomerId
WHERE c.CustomerCode = 'CUST002' -- Adjust this to match the customer code from your invoice creation script
AND i.IsDeleted = 0
ORDER BY i.CreatedDate DESC;

IF @InvoiceId IS NULL
BEGIN
    RAISERROR('Invoice for specified customer not found. Cannot proceed.', 16, 1);
    DROP TABLE #InvoiceGraphIds;
    RETURN;
END

-- Add the Invoice to our results
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'Invoice', 'Invoice #' + CAST(ROW_NUMBER() OVER(ORDER BY InvoiceId) AS NVARCHAR(10)) + ' - ' + 
             CONVERT(NVARCHAR(20), InvoiceDate, 101), 
       InvoiceId, NULL, NULL, NULL, NULL, CreatedDate, 100
FROM Invoice 
WHERE InvoiceId = @InvoiceId;

-- Define a very tight time window around invoice creation (+/- 3 seconds)
-- This ensures we only get entities created in the same batch
DECLARE @TimeWindowStart DATETIME = DATEADD(SECOND, -3, @InvoiceCreatedDate);
DECLARE @TimeWindowEnd DATETIME = DATEADD(SECOND, 3, @InvoiceCreatedDate);

PRINT 'Using time window: ' + CONVERT(VARCHAR, @TimeWindowStart, 121) + ' to ' + CONVERT(VARCHAR, @TimeWindowEnd, 121);

-- ========================== CUSTOMER SECTION ==========================
-- Add the Customer that this invoice is for
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'Customer', COALESCE(CustomerCommercialName, CustomerFirstName + ' ' + ISNULL(CustomerLastName, '')), 
       CustomerId, NULL, 'Invoice', @InvoiceId, NULL, CreatedDate, 200
FROM Customer 
WHERE CustomerId = @CustomerId
AND IsDeleted = 0;

-- ========================== INVOICE SYSTEM TYPE SECTION ==========================
-- Get InvoiceSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceSystemType', ist.InvoiceSystemTypeCode + ': ' + ist.InvoiceSystemTypeName, 
       ist.InvoiceSystemTypeId, NULL, 'Invoice', i.InvoiceId, NULL, NULL, 150
FROM Invoice i
JOIN InvoiceSystemType ist ON i.InvoiceSystemTypeId = ist.InvoiceSystemTypeId
WHERE i.InvoiceId = @InvoiceId;

-- ========================== INVOICE DETAIL SECTION ==========================
-- Get InvoiceDetail records
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceDetail', 'Line #' + CAST(ROW_NUMBER() OVER(ORDER BY id.InvoiceDetailId) AS NVARCHAR(10)) + ': ' + 
                       id.InvoiceDetailDescription, 
       id.InvoiceDetailId, NULL, 'Invoice', id.InvoiceId, NULL, id.CreatedDate, 300
FROM InvoiceDetail id
WHERE id.InvoiceId = @InvoiceId
AND id.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND id.IsDeleted = 0;

-- ========================== INVOICE DETAIL GROUP SECTION ==========================
-- Get InvoiceDetailGroup records
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceDetailGroup', idg.InvoiceDetailGroupCode + ': ' + COALESCE(idg.InvoiceDetailGroupDescription, 'Group'),
       idg.InvoiceDetailGroupId, NULL, 'Invoice', idg.InvoiceId, NULL, idg.CreatedDate, 250
FROM InvoiceDetailGroup idg
WHERE idg.InvoiceId = @InvoiceId
AND idg.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND idg.IsDeleted = 0;

-- Link InvoiceDetails to their groups when applicable
UPDATE id
SET id.ParentEntityType = 'InvoiceDetailGroup',
    id.ParentEntityId = idg.EntityId
FROM #InvoiceGraphIds id
JOIN InvoiceDetail d ON id.EntityId = d.InvoiceDetailId
JOIN #InvoiceGraphIds idg ON d.InvoiceDetailGroupId = idg.EntityId
WHERE id.EntityType = 'InvoiceDetail'
AND idg.EntityType = 'InvoiceDetailGroup'
AND d.InvoiceDetailGroupId IS NOT NULL;

-- ========================== PRODUCT SECTION ==========================
-- Get Products used in invoice details
DECLARE @InvoiceDetailIds TABLE (InvoiceDetailId UNIQUEIDENTIFIER);
INSERT INTO @InvoiceDetailIds
SELECT EntityId FROM #InvoiceGraphIds WHERE EntityType = 'InvoiceDetail';

INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'Product', p.ProductCode + ': ' + p.ProductName,
       p.ProductId, NULL, 'InvoiceDetail', id.InvoiceDetailId, NULL, p.CreatedDate, 350
FROM InvoiceDetail id
JOIN Product p ON id.ProductId = p.ProductId
WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds)
AND p.IsDeleted = 0;

-- Get ProductSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'ProductSystemType', pst.ProductSystemTypeCode + ': ' + pst.ProductSystemTypeName,
       pst.ProductSystemTypeId, NULL, 'Product', id.ProductId, NULL, NULL, 340
FROM InvoiceDetail id
JOIN Product p ON id.ProductId = p.ProductId
JOIN ProductSystemType pst ON id.ProductSystemTypeId = pst.ProductSystemTypeId
WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);

-- ========================== MEASUREMENT UNIT SECTION ==========================
-- Get MeasurementUnitSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'MeasurementUnitSystemType', must.MeasurementUnitSystemTypeCode + ': ' + must.MeasurementUnitSystemTypeName,
       must.MeasurementUnitSystemTypeId, NULL, 'InvoiceDetail', id.InvoiceDetailId, NULL, NULL, 360
FROM InvoiceDetail id
JOIN MeasurementUnitSystemType must ON id.MeasurementUnitSystemTypeId = must.MeasurementUnitSystemTypeId
WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);

-- ========================== PRICE USER TYPE SECTION ==========================
-- Get PriceUserType if it exists
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'PriceUserType', put.PriceUserTypeCode + ': ' + put.PriceUserTypeName,
       put.PriceUserTypeId, NULL, 'InvoiceDetail', id.InvoiceDetailId, NULL, NULL, 370
FROM InvoiceDetail id
JOIN PriceUserType put ON id.PriceUserTypeId = put.PriceUserTypeId
WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds)
AND id.PriceUserTypeId IS NOT NULL;

-- ========================== TAX SECTION ==========================
-- Get InvoiceTaxSystemType - note this uses RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'InvoiceTaxSystemType', tst.TaxSystemTypeCode + ': ' + tst.TaxSystemTypeName,
       NULL, itst.RowId, 'Invoice', itst.InvoiceId, NULL, itst.CreatedDate, 400
FROM InvoiceTaxSystemType itst
JOIN TaxSystemType tst ON itst.TaxSystemTypeId = tst.TaxSystemTypeId
WHERE itst.InvoiceId = @InvoiceId
AND itst.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND itst.IsDeleted = 0;

-- Get TaxSystemType for invoice taxes
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'TaxSystemType', tst.TaxSystemTypeCode + ': ' + tst.TaxSystemTypeName,
       tst.TaxSystemTypeId, NULL, 'InvoiceTaxSystemType', NULL, itst.RowId, NULL, 380
FROM InvoiceTaxSystemType itst
JOIN TaxSystemType tst ON itst.TaxSystemTypeId = tst.TaxSystemTypeId
WHERE itst.InvoiceId = @InvoiceId
AND itst.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND itst.IsDeleted = 0;

-- Get InvoiceDetailTaxSystemType - note this uses RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'InvoiceDetailTaxSystemType', tst.TaxSystemTypeCode + ': ' + tst.TaxSystemTypeName,
       NULL, idtst.RowId, 'InvoiceDetail', idtst.InvoiceDetailId, NULL, idtst.CreatedDate, 410
FROM InvoiceDetailTaxSystemType idtst
JOIN TaxSystemType tst ON idtst.TaxSystemTypeId = tst.TaxSystemTypeId
WHERE idtst.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds)
AND idtst.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND idtst.IsDeleted = 0;

-- ========================== INVOICE CUSTOMER SECTION ==========================
-- Get InvoiceCustomer
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceCustomer', COALESCE(ic.InvoiceCustomerCommercialName, 
                         ic.InvoiceCustomerFirstName + ' ' + ISNULL(ic.InvoiceCustomerLastName, '')),
       ic.InvoiceCustomerId, NULL, 'Invoice', ic.InvoiceId, NULL, ic.CreatedDate, 450
FROM InvoiceCustomer ic
WHERE ic.InvoiceId = @InvoiceId
AND ic.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND ic.IsDeleted = 0;

-- ========================== INVOICE CUSTOMER ADDRESS SECTION ==========================
-- Get InvoiceCustomerAddress relationships - note these use RowId (INT IDENTITY)
DECLARE @InvoiceCustomerIds TABLE (InvoiceCustomerId UNIQUEIDENTIFIER);
INSERT INTO @InvoiceCustomerIds
SELECT EntityId FROM #InvoiceGraphIds WHERE EntityType = 'InvoiceCustomer';

INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceCustomerAddress', 
       CASE 
           WHEN ast.AddressSystemTypeCode = 'BIL' THEN 'Billing Address' 
           WHEN ast.AddressSystemTypeCode = 'DEL' THEN 'Shipping Address'
           ELSE ast.AddressSystemTypeName
       END,
       NULL, ica.RowId, 'InvoiceCustomer', ica.InvoiceCustomerId, NULL, ica.CreatedDate, 460
FROM InvoiceCustomerAddress ica
JOIN AddressSystemType ast ON ica.AddressSystemTypeId = ast.AddressSystemTypeId
WHERE ica.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND ica.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND ica.IsDeleted = 0;

-- Get Address data
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'Address', a.AddressStreet + ISNULL(', ' + a.AddressStreetLine2, ''),
       a.AddressId, NULL, 'InvoiceCustomerAddress', NULL, ica.RowId, NULL, 470
FROM InvoiceCustomerAddress ica
JOIN Address a ON ica.AddressId = a.AddressId
WHERE ica.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND ica.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND ica.IsDeleted = 0;

-- ========================== INVOICE CUSTOMER DOCUMENT SECTION ==========================
-- Get InvoiceCustomerDocumentIdentification - note these use RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceCustomerDocumentIdentification',
       dist.DocumentIdentificationSystemTypeCode + ' Document',
       NULL, icdi.RowId, 'InvoiceCustomer', icdi.InvoiceCustomerId, NULL, icdi.CreatedDate, 480
FROM InvoiceCustomerDocumentIdentification icdi
JOIN DocumentIdentificationSystemType dist ON icdi.DocumentIdentificationSystemTypeId = dist.DocumentIdentificationSystemTypeId
WHERE icdi.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icdi.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND icdi.IsDeleted = 0;

-- Get DocumentIdentification
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'DocumentIdentification', di.DocumentIdentificationNumber,
       di.DocumentIdentificationId, NULL, 'InvoiceCustomerDocumentIdentification', NULL, icdi.RowId, NULL, 490
FROM InvoiceCustomerDocumentIdentification icdi
JOIN DocumentIdentification di ON icdi.DocumentIdentificationId = di.DocumentIdentificationId
WHERE icdi.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icdi.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND icdi.IsDeleted = 0;

-- ========================== INVOICE EMAIL SECTION ==========================
-- Get InvoiceCustomerEmailAddress - note these use RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceCustomerEmailAddress',
       east.EmailAddressSystemTypeName + ' Email',
       NULL, icea.RowId, 'InvoiceCustomer', icea.InvoiceCustomerId, NULL, icea.CreatedDate, 500
FROM InvoiceCustomerEmailAddress icea
JOIN EmailAddressSystemType east ON icea.EmailAddressSystemTypeId = east.EmailAddressSystemTypeId
WHERE icea.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icea.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND icea.IsDeleted = 0;

-- Get EmailAddress
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'EmailAddress', ea.EmailAddressString,
       ea.EmailAddressId, NULL, 'InvoiceCustomerEmailAddress', NULL, icea.RowId, NULL, 510
FROM InvoiceCustomerEmailAddress icea
JOIN EmailAddress ea ON icea.EmailAddressId = ea.EmailAddressId
WHERE icea.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icea.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND icea.IsDeleted = 0;

-- ========================== INVOICE PHONE SECTION ==========================
-- Get InvoiceCustomerPhoneNumber - note these use RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceCustomerPhoneNumber',
       pnst.PhoneNumberSystemTypeName + ' Phone',
       NULL, icpn.RowId, 'InvoiceCustomer', icpn.InvoiceCustomerId, NULL, icpn.CreatedDate, 520
FROM InvoiceCustomerPhoneNumber icpn
JOIN PhoneNumberSystemType pnst ON icpn.PhoneNumberSystemTypeId = pnst.PhoneNumberSystemTypeId
WHERE icpn.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icpn.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND icpn.IsDeleted = 0;

-- Get PhoneNumber
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'PhoneNumber', pn.PhoneNumberString,
       pn.PhoneNumberId, NULL, 'InvoiceCustomerPhoneNumber', NULL, icpn.RowId, NULL, 530
FROM InvoiceCustomerPhoneNumber icpn
JOIN PhoneNumber pn ON icpn.PhoneNumberId = pn.PhoneNumberId
WHERE icpn.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icpn.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND icpn.IsDeleted = 0;

-- ========================== CONTACT SECTION ==========================
-- Get InvoiceContact - note these use RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceContact',
       cst.ContactSystemTypeName + ' Contact',
       NULL, ic.RowId, 'Invoice', ic.InvoiceId, NULL, ic.CreatedDate, 540
FROM InvoiceContact ic
JOIN ContactSystemType cst ON ic.ContactSystemTypeId = cst.ContactSystemTypeId
WHERE ic.InvoiceId = @InvoiceId
AND ic.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND ic.IsDeleted = 0;

-- Get Contact
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'Contact', c.FirstName + ' ' + ISNULL(c.LastName, ''),
       c.ContactId, NULL, 'InvoiceContact', NULL, ic.RowId, c.CreatedDate, 550
FROM InvoiceContact ic
JOIN Contact c ON ic.ContactId = c.ContactId
WHERE ic.InvoiceId = @InvoiceId
AND ic.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND ic.IsDeleted = 0;

-- ========================== SALESPERSON SECTION ==========================
-- Get InvoiceSalesperson - note these use RowId (INT IDENTITY)
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceSalesperson',
       CASE WHEN is_p.IsPrimarySalesperson = 1 THEN 'Primary Salesperson' ELSE 'Salesperson' END,
       NULL, is_p.RowId, 'Invoice', is_p.InvoiceId, NULL, is_p.CreatedDate, 560
FROM InvoiceSalesperson is_p
WHERE is_p.InvoiceId = @InvoiceId
AND is_p.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND is_p.IsDeleted = 0;

-- Get Salesperson
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'Salesperson', s.SalespersonCode + ': ' + s.SalespersonFirstName + ' ' + ISNULL(s.SalespersonLastName, ''),
       s.SalespersonId, NULL, 'InvoiceSalesperson', NULL, is_p.RowId, s.CreatedDate, 570
FROM InvoiceSalesperson is_p
JOIN Salesperson s ON is_p.SalespersonId = s.SalespersonId
WHERE is_p.InvoiceId = @InvoiceId
AND is_p.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND is_p.IsDeleted = 0;

-- Get Employee linked to Salesperson
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'Employee', e.EmployeeCode + ': ' + e.EmployeeFirstName + ' ' + ISNULL(e.EmployeeLastName, ''),
       e.EmployeeId, NULL, 'Salesperson', s.SalespersonId, NULL, e.CreatedDate, 580
FROM #InvoiceGraphIds ig
JOIN Salesperson s ON ig.EntityId = s.SalespersonId
JOIN Employee e ON s.EmployeeId = e.EmployeeId
WHERE ig.EntityType = 'Salesperson'
AND e.IsDeleted = 0;

-- ========================== ACCOUNT RECEIVABLE SECTION ==========================
-- Get InvoiceAccountReceivable
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT 'InvoiceAccountReceivable', 'A/R: Due ' + CONVERT(VARCHAR, iar.DueDate, 101),
       iar.InvoiceAccountReceivableId, NULL, 'Invoice', iar.InvoiceId, NULL, iar.CreatedDate, 600
FROM InvoiceAccountReceivable iar
WHERE iar.InvoiceId = @InvoiceId
AND iar.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
AND iar.IsDeleted = 0;

-- ========================== ELECTRONIC DOCUMENT SECTION ==========================
-- Get ElectronicDocument if it exists
IF OBJECT_ID('ElectronicDocumentInvoice', 'U') IS NOT NULL
BEGIN
    INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
    SELECT 'ElectronicDocument', 'Electronic Document',
           ed.ElectronicDocumentId, NULL, 'Invoice', edi.InvoiceId, NULL, ed.CreatedDate, 620
    FROM ElectronicDocumentInvoice edi
    JOIN ElectronicDocument ed ON edi.ElectronicDocumentId = ed.ElectronicDocumentId
    WHERE edi.InvoiceId = @InvoiceId
    AND ed.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
    AND ed.IsDeleted = 0;
    
    -- Get the ElectronicDocumentInvoice row (may be RowId based)
    INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
    SELECT 'ElectronicDocumentInvoice', 'E-Doc Link',
           NULL, edi.RowId, 'ElectronicDocument', edi.ElectronicDocumentId, NULL, NULL, 625
    FROM ElectronicDocumentInvoice edi
    WHERE edi.InvoiceId = @InvoiceId;
END

-- ========================== SYSTEM TYPES SECTION ==========================
-- Get AddressSystemType for invoice customer addresses
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'AddressSystemType', ast.AddressSystemTypeCode + ': ' + ast.AddressSystemTypeName,
       ast.AddressSystemTypeId, NULL, NULL, NULL, NULL, NULL, 465
FROM InvoiceCustomerAddress ica
JOIN AddressSystemType ast ON ica.AddressSystemTypeId = ast.AddressSystemTypeId
WHERE ica.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND ica.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
GROUP BY ast.AddressSystemTypeId, ast.AddressSystemTypeCode, ast.AddressSystemTypeName;

-- Get DocumentIdentificationSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'DocumentIdentificationSystemType', dist.DocumentIdentificationSystemTypeCode + ': ' + dist.DocumentIdentificationSystemTypeName,
       dist.DocumentIdentificationSystemTypeId, NULL, NULL, NULL, NULL, NULL, 485
FROM InvoiceCustomerDocumentIdentification icdi
JOIN DocumentIdentificationSystemType dist ON icdi.DocumentIdentificationSystemTypeId = dist.DocumentIdentificationSystemTypeId
WHERE icdi.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icdi.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
GROUP BY dist.DocumentIdentificationSystemTypeId, dist.DocumentIdentificationSystemTypeCode, dist.DocumentIdentificationSystemTypeName;

-- Get EmailAddressSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'EmailAddressSystemType', east.EmailAddressSystemTypeCode + ': ' + east.EmailAddressSystemTypeName,
       east.EmailAddressSystemTypeId, NULL, NULL, NULL, NULL, NULL, 505
FROM InvoiceCustomerEmailAddress icea
JOIN EmailAddressSystemType east ON icea.EmailAddressSystemTypeId = east.EmailAddressSystemTypeId
WHERE icea.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icea.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
GROUP BY east.EmailAddressSystemTypeId, east.EmailAddressSystemTypeCode, east.EmailAddressSystemTypeName;

-- Get PhoneNumberSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'PhoneNumberSystemType', pnst.PhoneNumberSystemTypeCode + ': ' + pnst.PhoneNumberSystemTypeName,
       pnst.PhoneNumberSystemTypeId, NULL, NULL, NULL, NULL, NULL, 525
FROM InvoiceCustomerPhoneNumber icpn
JOIN PhoneNumberSystemType pnst ON icpn.PhoneNumberSystemTypeId = pnst.PhoneNumberSystemTypeId
WHERE icpn.InvoiceCustomerId IN (SELECT InvoiceCustomerId FROM @InvoiceCustomerIds)
AND icpn.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
GROUP BY pnst.PhoneNumberSystemTypeId, pnst.PhoneNumberSystemTypeCode, pnst.PhoneNumberSystemTypeName;

-- Get ContactSystemType
INSERT INTO #InvoiceGraphIds (EntityType, EntityName, EntityId, RowId, ParentEntityType, ParentEntityId, ParentRowId, CreatedDate, OrderDisplay)
SELECT DISTINCT 'ContactSystemType', cst.ContactSystemTypeCode + ': ' + cst.ContactSystemTypeName,
       cst.ContactSystemTypeId, NULL, NULL, NULL, NULL, NULL, 545
FROM InvoiceContact ic
JOIN ContactSystemType cst ON ic.ContactSystemTypeId = cst.ContactSystemTypeId
WHERE ic.InvoiceId = @InvoiceId
AND ic.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd
GROUP BY cst.ContactSystemTypeId, cst.ContactSystemTypeCode, cst.ContactSystemTypeName;

-- Mark potentially orphaned records
UPDATE #InvoiceGraphIds 
SET IsOrphaned = 1
WHERE ParentEntityId IS NOT NULL 
  AND ParentEntityId NOT IN (SELECT EntityId FROM #InvoiceGraphIds WHERE EntityId IS NOT NULL)
  AND (ParentRowId IS NULL OR ParentRowId NOT IN (SELECT RowId FROM #InvoiceGraphIds WHERE RowId IS NOT NULL));

-- ========================== SUMMARY VIEW ==========================
-- Output the results in a logical order with appropriate formatting for each ID type
SELECT 
    EntityType,
    EntityName,
    CASE 
        WHEN EntityId IS NOT NULL THEN CONVERT(NVARCHAR(36), EntityId)
        WHEN RowId IS NOT NULL THEN 'Row#' + CAST(RowId AS NVARCHAR(10))
        ELSE NULL
    END AS EntityIdentifier,
    ParentEntityType,
    CASE 
        WHEN ParentEntityId IS NOT NULL THEN CONVERT(NVARCHAR(36), ParentEntityId)
        WHEN ParentRowId IS NOT NULL THEN 'Row#' + CAST(ParentRowId AS NVARCHAR(10))
        ELSE NULL
    END AS ParentEntityIdentifier,
    CONVERT(NVARCHAR(25), CreatedDate, 120) AS CreatedDate,
    CASE WHEN IsOrphaned = 1 THEN 'Yes' ELSE 'No' END AS IsOrphaned 
FROM #InvoiceGraphIds
ORDER BY OrderDisplay, EntityType, EntityName;

-- Clean up
DROP TABLE #InvoiceGraphIds;