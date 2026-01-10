USE [PCIShield_TenantAXBXCX]
GO

/****** Object:  Table [dbo].[InvoiceDetailTaxSystemType]    Script Date: 21/3/2025 16:31:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseDetailTaxSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseDetailId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[PurchaseDetailTaxSystemTypeAmount] [decimal](18, 6) NOT NULL,
	[PurchaseDetailTaxSystemTypeRate] [decimal](18, 6) NOT NULL,
	[PurchaseDetailTaxSystemTypeSign] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseDetailTaxSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseDetailTaxSystemType] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[PurchaseDetailTaxSystemType] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[PurchaseDetailTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetailTaxSystemType_PurchaseDetail] FOREIGN KEY([PurchaseDetailId])
REFERENCES [dbo].[PurchaseDetail] ([PurchaseDetailId])
GO

ALTER TABLE [dbo].[PurchaseDetailTaxSystemType] CHECK CONSTRAINT [FK_PurchaseDetailTaxSystemType_PurchaseDetail]
GO

ALTER TABLE [dbo].[PurchaseDetailTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetailTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO

ALTER TABLE [dbo].[PurchaseDetailTaxSystemType] CHECK CONSTRAINT [FK_PurchaseDetailTaxSystemType_TaxSystemType]
GO

ALTER TABLE [PCIShield_TenantAXBXCX].[dbo].[BankAccountSystemType] ADD BankAccountSystemTypeVariant VARCHAR(5) 

ALTER TABLE [PCIShield_TenantAXBXCX].[dbo].[Contact] ADD JobTitle VARCHAR(50) 
