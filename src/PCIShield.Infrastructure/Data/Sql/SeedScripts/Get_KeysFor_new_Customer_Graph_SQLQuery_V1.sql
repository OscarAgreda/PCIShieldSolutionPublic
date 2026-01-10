USE PCIShield_TenantAXBXCX
GO

-- DROP TABLE #CustomerGraphIds ;

-- Create a temporary table to store our results
CREATE TABLE #CustomerGraphIds (
    EntityType NVARCHAR(100),
    EntityName NVARCHAR(255),
    EntityId UNIQUEIDENTIFIER,
    ParentEntityType NVARCHAR(100) NULL,
    ParentEntityId UNIQUEIDENTIFIER NULL,
    CreatedDate DATETIME NULL,
    OrderDisplay INT,
    IsOrphaned BIT DEFAULT 0
);

DECLARE @CustomerId UNIQUEIDENTIFIER;
DECLARE @CustomerCreatedDate DATETIME;
DECLARE @UserId UNIQUEIDENTIFIER = '2CD63ACA-28F4-52A5-D27D-0B2557B8ADF0';

-- First, get the specific customer with CustomerCode 'CUST001'
SELECT @CustomerId = CustomerId, @CustomerCreatedDate = CreatedDate
FROM Customer 
WHERE CustomerCode = 'CUST001' AND CustomerCommercialName = 'PCIShield Corporation' AND IsDeleted = 0;

IF @CustomerId IS NULL
BEGIN
    RAISERROR('Customer with code CUST001 and name PCIShield Corporation not found. Cannot proceed.', 16, 1);
    DROP TABLE #CustomerGraphIds;
    RETURN;
END

-- Add the Customer to our results
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 'Customer', CustomerCommercialName, CustomerId, NULL, NULL, CreatedDate, 100
FROM Customer 
WHERE CustomerId = @CustomerId;

-- Only consider entities created around the same time as the customer (+/- 1 hour)
DECLARE @TimeWindowStart DATETIME = DATEADD(HOUR, -1, @CustomerCreatedDate);
DECLARE @TimeWindowEnd DATETIME = DATEADD(HOUR, 1, @CustomerCreatedDate);

-- Get BoundedContext
DECLARE @BoundedContextId UNIQUEIDENTIFIER;
SELECT @BoundedContextId = BoundedContextId 
FROM Customer c
JOIN CustomerUserType cut ON c.CustomerUserTypeId = cut.CustomerUserTypeId
WHERE c.CustomerId = @CustomerId;

IF @BoundedContextId IS NOT NULL
BEGIN
    INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
    SELECT 'BoundedContext', 'Active Bounded Context', BoundedContextId, 'Customer', @CustomerId, NULL, 10
    FROM BoundedContext
    WHERE BoundedContextId = @BoundedContextId;
END

-- Get Address relationships and actual addresses
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'Address', 
    CASE 
        WHEN ast.AddressSystemTypeCode = 'PRI' THEN 'Primary: ' 
        WHEN ast.AddressSystemTypeCode = 'BIL' THEN 'Billing: ' 
        WHEN ast.AddressSystemTypeCode = 'DEL' THEN 'Delivery: ' 
        ELSE ast.AddressSystemTypeCode + ': ' 
    END + a.AddressStreet,
    a.AddressId, 
    'Customer', 
    ca.CustomerId,
    ca.CreatedDate,
    300
FROM CustomerAddress ca
JOIN Address a ON ca.AddressId = a.AddressId
JOIN AddressSystemType ast ON ca.AddressSystemTypeId = ast.AddressSystemTypeId
WHERE ca.CustomerId = @CustomerId 
  AND ca.IsDeleted = 0
  AND ca.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Get Address System Types used with this customer
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT 
    'AddressSystemType', 
    ast.AddressSystemTypeName, 
    ast.AddressSystemTypeId, 
    'Address', 
    NULL,
    NULL,
    290
FROM CustomerAddress ca
JOIN AddressSystemType ast ON ca.AddressSystemTypeId = ast.AddressSystemTypeId
WHERE ca.CustomerId = @CustomerId 
  AND ca.IsDeleted = 0
  AND ca.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Get Contact relationships and actual contacts
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'Contact', 
    CASE 
        WHEN cst.ContactSystemTypeCode = 'REPRESENT' THEN 'Representative: ' 
        ELSE '' 
    END + c.FirstName + ' ' + c.LastName, 
    c.ContactId, 
    'Customer', 
    cc.CustomerId,
    cc.CreatedDate,
    310
FROM CustomerContact cc
JOIN Contact c ON cc.ContactId = c.ContactId
JOIN ContactSystemType cst ON cc.ContactSystemTypeId = cst.ContactSystemTypeId
WHERE cc.CustomerId = @CustomerId 
  AND cc.IsDeleted = 0
  AND cc.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Get Contact System Types
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT 
    'ContactSystemType', 
    cst.ContactSystemTypeName, 
    cst.ContactSystemTypeId, 
    'Contact', 
    NULL,
    NULL,
    305
FROM CustomerContact cc
JOIN ContactSystemType cst ON cc.ContactSystemTypeId = cst.ContactSystemTypeId
WHERE cc.CustomerId = @CustomerId 
  AND cc.IsDeleted = 0
  AND cc.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Email Addresses
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'EmailAddress', 
    e.EmailAddressString, 
    e.EmailAddressId, 
    'Customer', 
    cea.CustomerId,
    cea.CreatedDate,
    320
FROM CustomerEmailAddress cea
JOIN EmailAddress e ON cea.EmailAddressId = e.EmailAddressId
WHERE cea.CustomerId = @CustomerId 
  AND cea.IsDeleted = 0
  AND cea.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Email Address System Types
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT 
    'EmailAddressSystemType', 
    east.EmailAddressSystemTypeName, 
    east.EmailAddressSystemTypeId, 
    'EmailAddress', 
    NULL,
    NULL,
    315
FROM CustomerEmailAddress cea
JOIN EmailAddressSystemType east ON cea.EmailAddressSystemTypeId = east.EmailAddressSystemTypeId
WHERE cea.CustomerId = @CustomerId 
  AND cea.IsDeleted = 0
  AND cea.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Phone Numbers
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'PhoneNumber', 
    p.PhoneNumberString, 
    p.PhoneNumberId, 
    'Customer', 
    cpn.CustomerId,
    cpn.CreatedDate,
    330
FROM CustomerPhoneNumber cpn
JOIN PhoneNumber p ON cpn.PhoneNumberId = p.PhoneNumberId
WHERE cpn.CustomerId = @CustomerId 
  AND cpn.IsDeleted = 0
  AND cpn.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Phone Number System Types
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT 
    'PhoneNumberSystemType', 
    pnst.PhoneNumberSystemTypeName, 
    pnst.PhoneNumberSystemTypeId, 
    'PhoneNumber', 
    NULL,
    NULL,
    325
FROM CustomerPhoneNumber cpn
JOIN PhoneNumberSystemType pnst ON cpn.PhoneNumberSystemTypeId = pnst.PhoneNumberSystemTypeId
WHERE cpn.CustomerId = @CustomerId 
  AND cpn.IsDeleted = 0
  AND cpn.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Document Identifications
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'DocumentIdentification', 
    CASE 
        WHEN dist.DocumentIdentificationSystemTypeCode = 'NIT' THEN 'Tax ID: ' 
        WHEN dist.DocumentIdentificationSystemTypeCode = 'NRC' THEN 'Business Registry: ' 
        WHEN dist.DocumentIdentificationSystemTypeCode = 'REP_DOC' THEN 'Representative Doc: ' 
        ELSE 'Document: ' 
    END + di.DocumentIdentificationNumber,
    di.DocumentIdentificationId, 
    'Customer', 
    cdi.CustomerId,
    cdi.CreatedDate,
    340
FROM CustomerDocumentIdentification cdi
JOIN DocumentIdentification di ON cdi.DocumentIdentificationId = di.DocumentIdentificationId
JOIN DocumentIdentificationSystemType dist ON cdi.DocumentIdentificationSystemTypeId = dist.DocumentIdentificationSystemTypeId
WHERE cdi.CustomerId = @CustomerId 
  AND cdi.IsDeleted = 0
  AND cdi.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Document Identification System Types
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT 
    'DocumentIdentificationSystemType', 
    dist.DocumentIdentificationSystemTypeName, 
    dist.DocumentIdentificationSystemTypeId, 
    'DocumentIdentification', 
    NULL,
    NULL,
    335
FROM CustomerDocumentIdentification cdi
JOIN DocumentIdentificationSystemType dist ON cdi.DocumentIdentificationSystemTypeId = dist.DocumentIdentificationSystemTypeId
WHERE cdi.CustomerId = @CustomerId 
  AND cdi.IsDeleted = 0
  AND cdi.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Location data linked to addresses
DECLARE @AddressIds TABLE (AddressId UNIQUEIDENTIFIER);
INSERT INTO @AddressIds
SELECT EntityId FROM #CustomerGraphIds WHERE EntityType = 'Address'; -- FIXED: Changed AddressId to EntityId

-- Country
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'Country', 
    c.CountryName, 
    c.CountryId, 
    'Address', 
    a.AddressId,
    NULL,
    200
FROM Address a
JOIN Country c ON a.CountryId = c.CountryId
WHERE a.AddressId IN (SELECT AddressId FROM @AddressIds);

-- Now get other location info
DECLARE @CountryIds TABLE (CountryId UNIQUEIDENTIFIER);
INSERT INTO @CountryIds
SELECT EntityId FROM #CustomerGraphIds WHERE EntityType = 'Country';

-- Continent
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'Continent', 
    c.ContinentName, 
    c.ContinentId, 
    'Country', 
    ctry.CountryId,
    NULL,
    190
FROM Country ctry
JOIN Continent c ON ctry.ContinentId = c.ContinentId
WHERE ctry.CountryId IN (SELECT CountryId FROM @CountryIds);

-- Currency
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'Currency', 
    c.CurrencyName, 
    c.CurrencyId, 
    'Country', 
    ctry.CountryId,
    NULL,
    195
FROM Country ctry
JOIN Currency c ON ctry.CurrencyId = c.CurrencyId
WHERE ctry.CountryId IN (SELECT CountryId FROM @CountryIds);

-- State
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'State', 
    s.StateName, 
    s.StateId, 
    'Country', 
    s.CountryId,
    s.CreatedDate,
    210
FROM Address a
JOIN State s ON a.StateId = s.StateId
WHERE a.AddressId IN (SELECT AddressId FROM @AddressIds);

-- City
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'City', 
    c.CityName, 
    c.CityId, 
    'State', 
    c.StateId,
    c.CreatedDate,
    220
FROM Address a
JOIN City c ON a.CityId = c.CityId
WHERE a.AddressId IN (SELECT AddressId FROM @AddressIds);

-- County
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'County', 
    c.CountyName, 
    c.CountyId, 
    'State', 
    c.StateId,
    c.CreatedDate,
    215
FROM Address a
JOIN County c ON a.CountyId = c.CountyId
WHERE a.AddressId IN (SELECT AddressId FROM @AddressIds);

-- District
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'District', 
    d.DistrictName, 
    d.DistrictId, 
    'County', 
    d.CountyId,
    d.CreatedDate,
    225
FROM Address a
JOIN District d ON a.DistrictId = d.DistrictId
WHERE a.AddressId IN (SELECT AddressId FROM @AddressIds);

-- Customer System Types
-- CustomerUserType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'CustomerUserType', 
    cut.CustomerUserTypeName, 
    cut.CustomerUserTypeId, 
    'Customer', 
    @CustomerId,
    NULL,
    350
FROM Customer c
JOIN CustomerUserType cut ON c.CustomerUserTypeId = cut.CustomerUserTypeId
WHERE c.CustomerId = @CustomerId;

-- TaxpayerSystemType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'TaxpayerSystemType', 
    tpst.TaxpayerSystemTypeName, 
    tpst.TaxpayerSystemTypeId, 
    'Customer', 
    @CustomerId,
    NULL,
    355
FROM Customer c
JOIN TaxpayerSystemType tpst ON c.TaxpayerSystemTypeId = tpst.TaxpayerSystemTypeId
WHERE c.CustomerId = @CustomerId;

-- TaxSystemTypes for this customer
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'TaxSystemType', 
    tst.TaxSystemTypeName, 
    tst.TaxSystemTypeId, 
    'Customer', 
    ctst.CustomerId,
    ctst.CreatedDate,
    360
FROM CustomerTaxSystemType ctst
JOIN TaxSystemType tst ON ctst.TaxSystemTypeId = tst.TaxSystemTypeId
WHERE ctst.CustomerId = @CustomerId 
  AND ctst.IsDeleted = 0
  AND ctst.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- EconomicActivitySystemType for this customer
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'EconomicActivitySystemType', 
    east.EconomicActivitySystemTypeName, 
    east.EconomicActivitySystemTypeId, 
    'Customer', 
    ceast.CustomerId,
    ceast.CreatedDate,
    365
FROM CustomerEconomicActivitySystemType ceast
JOIN EconomicActivitySystemType east ON ceast.EconomicActivitySystemTypeId = east.EconomicActivitySystemTypeId
WHERE ceast.CustomerId = @CustomerId 
  AND ceast.IsDeleted = 0
  AND ceast.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Salesperson for this customer
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'Salesperson', 
    s.SalespersonFirstName + ' ' + s.SalespersonLastName, 
    s.SalespersonId, 
    'Customer', 
    cs.CustomerId,
    cs.CreatedDate,
    370
FROM CustomerSalesperson cs
JOIN Salesperson s ON cs.SalespersonId = s.SalespersonId
WHERE cs.CustomerId = @CustomerId 
  AND cs.IsDeleted = 0
  AND cs.CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Get Employee connected to Salesperson
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'Employee', 
    e.EmployeeFirstName + ' ' + e.EmployeeLastName, 
    e.EmployeeId, 
    'Salesperson', 
    s.SalespersonId,
    e.CreatedDate,
    375
FROM #CustomerGraphIds cg
JOIN Salesperson s ON cg.EntityId = s.SalespersonId
JOIN Employee e ON s.EmployeeId = e.EmployeeId
WHERE cg.EntityType = 'Salesperson' AND e.IsDeleted = 0;

-- Get Invoice for this customer
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'Invoice', 
    'Invoice #' + CAST(ROW_NUMBER() OVER(ORDER BY InvoiceId) AS NVARCHAR(10)) + ' - ' + 
             CONVERT(NVARCHAR(20), InvoiceDate, 101), 
    InvoiceId, 
    'Customer', 
    CustomerId,
    CreatedDate,
    400
FROM Invoice
WHERE CustomerId = @CustomerId 
  AND IsDeleted = 0
  AND CreatedDate BETWEEN @TimeWindowStart AND @TimeWindowEnd;

-- Now get Invoice-related entities
DECLARE @InvoiceIds TABLE (InvoiceId UNIQUEIDENTIFIER);
INSERT INTO @InvoiceIds
SELECT EntityId FROM #CustomerGraphIds WHERE EntityType = 'Invoice';

-- InvoiceSystemType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'InvoiceSystemType', 
    ist.InvoiceSystemTypeName, 
    ist.InvoiceSystemTypeId, 
    'Invoice', 
    i.InvoiceId,
    i.CreatedDate,
    395
FROM Invoice i
JOIN InvoiceSystemType ist ON i.InvoiceSystemTypeId = ist.InvoiceSystemTypeId
WHERE i.InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds);

-- InvoiceDetail
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'InvoiceDetail', 
    'Line Item: ' + InvoiceDetailDescription, 
    InvoiceDetailId, 
    'Invoice', 
    InvoiceId,
    CreatedDate,
    410
FROM InvoiceDetail
WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds) 
  AND IsDeleted = 0;

-- Get InvoiceDetail-related entities
DECLARE @InvoiceDetailIds TABLE (InvoiceDetailId UNIQUEIDENTIFIER);
INSERT INTO @InvoiceDetailIds
SELECT EntityId FROM #CustomerGraphIds WHERE EntityType = 'InvoiceDetail';

-- Products
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'Product', 
    p.ProductName, 
    p.ProductId, 
    'InvoiceDetail', 
    id.InvoiceDetailId,
    p.CreatedDate,
    430
FROM InvoiceDetail id
JOIN Product p ON id.ProductId = p.ProductId
WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds) 
  AND p.IsDeleted = 0;

-- ProductSystemType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'ProductSystemType', 
    pst.ProductSystemTypeName, 
    pst.ProductSystemTypeId, 
    'Product', 
    p.ProductId,
    NULL,
    425
FROM #CustomerGraphIds cg
JOIN Product p ON cg.EntityId = p.ProductId
JOIN ProductSystemType pst ON p.ProductSystemTypeId = pst.ProductSystemTypeId
WHERE cg.EntityType = 'Product';

-- MeasurementUnitSystemType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT DISTINCT
    'MeasurementUnitSystemType', 
    must.MeasurementUnitSystemTypeName, 
    must.MeasurementUnitSystemTypeId, 
    'InvoiceDetail', 
    id.InvoiceDetailId,
    NULL,
    435
FROM InvoiceDetail id
JOIN MeasurementUnitSystemType must ON id.MeasurementUnitSystemTypeId = must.MeasurementUnitSystemTypeId
WHERE id.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds);

-- InvoiceTaxSystemType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'InvoiceTaxSystemType', 
    tst.TaxSystemTypeName, 
    itst.TaxSystemTypeId, 
    'Invoice', 
    itst.InvoiceId,
    itst.CreatedDate,
    415
FROM InvoiceTaxSystemType itst
JOIN TaxSystemType tst ON itst.TaxSystemTypeId = tst.TaxSystemTypeId
WHERE itst.InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds) 
  AND itst.IsDeleted = 0;

-- InvoiceDetailTaxSystemType
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'InvoiceDetailTaxSystemType', 
    tst.TaxSystemTypeName, 
    idtst.TaxSystemTypeId, 
    'InvoiceDetail', 
    idtst.InvoiceDetailId,
    idtst.CreatedDate,
    420
FROM InvoiceDetailTaxSystemType idtst
JOIN TaxSystemType tst ON idtst.TaxSystemTypeId = tst.TaxSystemTypeId
WHERE idtst.InvoiceDetailId IN (SELECT InvoiceDetailId FROM @InvoiceDetailIds) 
  AND idtst.IsDeleted = 0;

-- InvoiceAccountReceivable
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'InvoiceAccountReceivable', 
    'AR for Invoice', 
    InvoiceAccountReceivableId, 
    'Invoice', 
    InvoiceId,
    CreatedDate,
    440
FROM InvoiceAccountReceivable
WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds) 
  AND IsDeleted = 0;

-- InvoiceCustomer
INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
SELECT 
    'InvoiceCustomer', 
    COALESCE(InvoiceCustomerCommercialName, 'Invoice Customer Record'), 
    InvoiceCustomerId, 
    'Invoice', 
    InvoiceId,
    CreatedDate,
    450
FROM InvoiceCustomer
WHERE InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds) 
  AND IsDeleted = 0;

-- ElectronicDocument
IF OBJECT_ID('ElectronicDocument', 'U') IS NOT NULL
BEGIN
    INSERT INTO #CustomerGraphIds (EntityType, EntityName, EntityId, ParentEntityType, ParentEntityId, CreatedDate, OrderDisplay)
    SELECT 
        'ElectronicDocument', 
        'Electronic Document', 
        ed.ElectronicDocumentId, 
        'Invoice', 
        edi.InvoiceId,
        ed.CreatedDate,
        460
    FROM ElectronicDocument ed
    JOIN ElectronicDocumentInvoice edi ON ed.ElectronicDocumentId = edi.ElectronicDocumentId
    WHERE edi.InvoiceId IN (SELECT InvoiceId FROM @InvoiceIds) 
      AND ed.IsDeleted = 0;
END

-- Mark potentially orphaned records
UPDATE #CustomerGraphIds 
SET IsOrphaned = 1
WHERE ParentEntityId IS NOT NULL 
  AND ParentEntityId NOT IN (SELECT EntityId FROM #CustomerGraphIds);

-- Output the results in a logical order
SELECT 
    EntityType,
    EntityName,
    CONVERT(NVARCHAR(36), EntityId) AS EntityId,
    ParentEntityType,
    CASE WHEN ParentEntityId IS NOT NULL 
         THEN CONVERT(NVARCHAR(36), ParentEntityId)
         ELSE NULL
    END AS ParentEntityId,
    CONVERT(NVARCHAR(25), CreatedDate, 120) AS CreatedDate,
    CASE WHEN IsOrphaned = 1 THEN 'Yes' ELSE 'No' END AS IsOrphaned 
FROM #CustomerGraphIds
ORDER BY OrderDisplay, EntityType, EntityName;

-- Clean up
DROP TABLE #CustomerGraphIds;