USE [PCIShield_TenantAXBXCX]
GO

/****** Object:  Table [dbo].[ProductTaxSystemType]    Script Date: 8/4/2025 16:58:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductTaxSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[ProductTaxSystemTypeAmount] [decimal](18, 6) NOT NULL,
	[ProductTaxSystemTypeRate] [decimal](18, 6) NOT NULL,
	[ProductTaxSystemTypeSign] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductTaxSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductTaxSystemType] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[ProductTaxSystemType] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[ProductTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ProductTaxSystemType_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO

ALTER TABLE [dbo].[ProductTaxSystemType] CHECK CONSTRAINT [FK_ProductTaxSystemType_Product]
GO

ALTER TABLE [dbo].[ProductTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ProductTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO

ALTER TABLE [dbo].[ProductTaxSystemType] CHECK CONSTRAINT [FK_ProductTaxSystemType_TaxSystemType]
GO


