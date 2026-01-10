
USE [PCIShield_Core_Db]
GO

ALTER DATABASE [PCIShield_Core_Db] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PCIShield_Core_Db].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PCIShield_Core_Db] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET ARITHABORT OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PCIShield_Core_Db] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PCIShield_Core_Db] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PCIShield_Core_Db] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PCIShield_Core_Db] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PCIShield_Core_Db] SET  MULTI_USER 
GO
ALTER DATABASE [PCIShield_Core_Db] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PCIShield_Core_Db] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PCIShield_Core_Db] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PCIShield_Core_Db] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PCIShield_Core_Db] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PCIShield_Core_Db] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'PCIShield_Core_Db', N'ON'
GO
ALTER DATABASE [PCIShield_Core_Db] SET QUERY_STORE = ON
GO
ALTER DATABASE [PCIShield_Core_Db] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PCIShield_Core_Db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[AddressId] [uniqueidentifier] NOT NULL,
	[AddressStreet] [nvarchar](255) NOT NULL,
	[AddressStreetLine2] [nvarchar](255) NULL,
	[AddressStreetLine3] [nvarchar](255) NULL,
	[ZipCode] [nvarchar](255) NULL,
	[CityId] [uniqueidentifier] NULL,
	[DistrictId] [uniqueidentifier] NULL,
	[CountyId] [uniqueidentifier] NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddressSystemType](
	[AddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[AddressSystemTypeCode] [nvarchar](3) NOT NULL,
	[AddressSystemTypeName] [nvarchar](255) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AddressSystemType] PRIMARY KEY CLUSTERED 
(
	[AddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Aggregate](
	[AggregateId] [uniqueidentifier] NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[AggregateName] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Aggregate] PRIMARY KEY CLUSTERED 
(
	[AggregateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationSetting](
	[ApplicationSettingId] [uniqueidentifier] NOT NULL,
	[SettingId] [uniqueidentifier] NOT NULL,
	[StringValue] [nvarchar](max) NULL,
	[NumericValue] [decimal](28, 12) NULL,
	[BooleanValue] [bit] NULL,
	[DateValue] [datetime2](7) NULL,
	[JsonValue] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ApplicationSetting] PRIMARY KEY CLUSTERED 
(
	[ApplicationSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
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




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationUser](
	[ApplicationUserId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsLoginAllowed] [bit] NOT NULL,
	[LastLogin] [datetime2](7) NULL,
	[LogoutTime] [datetime2](7) NULL,
	[LastFailedLogin] [datetime2](7) NULL,
	[FailedLoginCount] [int] NOT NULL,
	[Email] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[IsUserApproved] [bit] NOT NULL,
	[IsPhoneVerified] [bit] NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[ConfirmationEmail] [nvarchar](255) NULL,
	[LastPasswordChange] [datetime2](7) NULL,
	[IsLocked] [bit] NOT NULL,
	[LockedUntil] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsUserFullyRegistered] [bit] NOT NULL,
	[AvailabilityStatus] [nvarchar](50) NULL,
	[IsBanned] [bit] NOT NULL,
	[IsFullyRegistered] [bit] NOT NULL,
	[LastLoginIP] [nvarchar](50) NULL,
	[LastActiveAt] [datetime2](7) NULL,
	[IsOnline] [bit] NULL,
	[IsConnectedToSignalr] [bit] NULL,
	[TimeLastSignalrPing] [datetime2](7) NULL,
	[IsLoggedIntoApp] [bit] NULL,
	[TimeLastLoggedToApp] [datetime2](7) NULL,
	[AverageResponseTime] [int] NULL,
	[UserIconUrl] [nvarchar](255) NULL,
	[UserProfileImagePath] [nvarchar](255) NULL,
	[UserBirthDate] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[TenantId] [uniqueidentifier] NOT NULL,
	[IsEmployee] [bit] NULL,
	[IsErpOwner] [bit] NULL,
	[IsCustomer] [bit] NULL,
 CONSTRAINT [PK_ApplicationUser] PRIMARY KEY CLUSTERED 
(
	[ApplicationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationUserSetting](
	[ApplicationUserSettingId] [uniqueidentifier] NOT NULL,
	[ApplicationUserId] [uniqueidentifier] NOT NULL,
	[SettingId] [uniqueidentifier] NOT NULL,
	[StringValue] [nvarchar](max) NULL,
	[NumericValue] [decimal](28, 12) NULL,
	[BooleanValue] [bit] NULL,
	[DateValue] [datetime2](7) NULL,
	[JsonValue] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ApplicationUserSetting] PRIMARY KEY CLUSTERED 
(
	[ApplicationUserSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Area](
	[AreaId] [uniqueidentifier] NOT NULL,
	[AreaName] [nvarchar](255) NOT NULL,
	[AreaDescription] [nvarchar](255) NOT NULL,
	[AreaColor] [nvarchar](255) NOT NULL,
	[AreaIconUrl] [nvarchar](255) NULL,
	[AreaImagePath] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsCountryRequired] [bit] NOT NULL,
	[IsStateRequired] [bit] NOT NULL,
	[IsCityRequired] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,

	[CountryId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[AreaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentDetail](
	[AttachmentDetailId] [uniqueidentifier] NOT NULL,
	[AttachmentIconUrl] [nvarchar](255) NULL,
	[AttachmentPath] [nvarchar](255) NULL,
	[AttachmentSize] [decimal](18, 2) NULL,
	[AttachmentTypeId] [uniqueidentifier] NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AttachmentDetail] PRIMARY KEY CLUSTERED 
(
	[AttachmentDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentType](
	[AttachmentTypeId] [uniqueidentifier] NOT NULL,
	[AttachmentTypeName] [nvarchar](255) NOT NULL,
	[AttachmentTypeDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_AttachmentType] PRIMARY KEY CLUSTERED 
(
	[AttachmentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bank](
	[BankId] [uniqueidentifier] NOT NULL,
	[BankCode] [nvarchar](50) NOT NULL,
	[BankName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Bank] PRIMARY KEY CLUSTERED 
(
	[BankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankAccount](
	[BankAccountId] [uniqueidentifier] NOT NULL,
	[BankId] [uniqueidentifier] NOT NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountName] [nvarchar](100) NOT NULL,
	[BankAccountSystemTypeId] [uniqueidentifier] NOT NULL,
	[CurrencyId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_BankAccount] PRIMARY KEY CLUSTERED 
(
	[BankAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankAccountSystemType](
	[BankAccountSystemTypeId] [uniqueidentifier] NOT NULL,
	[BankAccountSystemTypeCode] [nvarchar](50) NOT NULL,
	[BankAccountSystemTypeName] [nvarchar](100) NOT NULL,
    	[BankAccountSystemTypeVariant] [nvarchar](5) NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_BankAccountSystemType] PRIMARY KEY CLUSTERED 
(
	[BankAccountSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankTransaction](
	[BankTransactionId] [uniqueidentifier] NOT NULL,
	[BankAccountId] [uniqueidentifier] NOT NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountName] [nvarchar](100) NOT NULL,
	[BankTransactionSystemTypeId] [uniqueidentifier] NOT NULL,
	[TransactionDate] [datetime2](7) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[Reference] [nvarchar](100) NULL,
	[CurrencyId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_BankTransaction] PRIMARY KEY CLUSTERED 
(
	[BankTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankTransactionSystemType](
	[BankTransactionSystemTypeId] [uniqueidentifier] NOT NULL,
	[BankTransactionSystemTypeCode] [nvarchar](50) NOT NULL,
	[BankTransactionSystemTypeName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_BankTransactionSystemType] PRIMARY KEY CLUSTERED 
(
	[BankTransactionSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BoundedContext](
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[BoundedContextCode] [nvarchar](10) NOT NULL,
	[BoundedContextName] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BoundedContext] PRIMARY KEY CLUSTERED 
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [uniqueidentifier] NOT NULL,
	[CategoryCode] [nvarchar](50) NOT NULL,
	[CategoryName] [nvarchar](255) NOT NULL,
	[CategoryDescription] [nvarchar](500) NULL,
	[ParentCategoryId] [uniqueidentifier] NULL,
	[ClassificationId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[AreaId] [uniqueidentifier] NULL
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[City](
	[CityId] [uniqueidentifier] NOT NULL,
	[CityName] [nvarchar](255) NOT NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classification](
	[ClassificationId] [uniqueidentifier] NOT NULL,
	[ClassificationCode] [nvarchar](50) NOT NULL,
	[ClassificationName] [nvarchar](255) NOT NULL,
	[ClassificationDescription] [nvarchar](500) NULL,
	[TableEntityId] [uniqueidentifier] NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Classification] PRIMARY KEY CLUSTERED 
(
	[ClassificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[ContactId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](150) NOT NULL,
	[LastName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](30) NULL,
	[Mobile] [nvarchar](30) NULL,
	[IsPrimaryContact] [bit] NULL,
	[JobTitle] [nvarchar](50) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContactSystemType](
	[ContactSystemTypeId] [uniqueidentifier] NOT NULL,
	[ContactSystemTypeCode] [nvarchar](10) NOT NULL,
	[ContactSystemTypeName] [nvarchar](255) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ContactSystemType] PRIMARY KEY CLUSTERED 
(
	[ContactSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Continent](
	[ContinentId] [uniqueidentifier] NOT NULL,
	[ContinentName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Continent] PRIMARY KEY CLUSTERED 
(
	[ContinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContinentLocale](
	[ContinentLocaleId] [uniqueidentifier] NOT NULL,
	[ContinentId] [uniqueidentifier] NOT NULL,
	[ContinentLocaleName] [nvarchar](255) NOT NULL,
	[ISOLanguageCode] [nvarchar](2) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ContinentLocale] PRIMARY KEY CLUSTERED 
(
	[ContinentLocaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversation](
	[ConversationId] [uniqueidentifier] NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[ApplicationUserId] [uniqueidentifier] NOT NULL,
	[AreaId] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[ConversationMoreInfoId] [uniqueidentifier] NOT NULL,
	[ConversationSumSpent] [decimal](18, 2) NOT NULL,
	[ConversationSumTimeInSecs] [int] NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[InProgress] [bit] NOT NULL,
	[CustomerIsConnected] [bit] NOT NULL,
	[EmployeeIsConnected] [bit] NOT NULL,
	[CustomerIsPurchasingBalance] [bit] NOT NULL,
	[CustomerPurchasingSpentTime] [datetime2](7) NULL,
	[LastMessageAt] [datetime2](7) NULL,
	[OrigUnansweredConversationId] [uniqueidentifier] NULL,
	[ReconnectConversationId] [uniqueidentifier] NULL,
	[Tags] [nvarchar](255) NULL,

	[Title] [nvarchar](255) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[ConversationStageId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Conversation] PRIMARY KEY CLUSTERED 
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConversationMoreInfo](
	[ConversationMoreInfoId] [uniqueidentifier] NOT NULL,
	[EmployeeNeverResponded] [bit] NOT NULL,
	[CanceledByEmployee] [bit] NOT NULL,
	[CanceledByCustomer] [bit] NOT NULL,
	[CustomerNeverResponded] [bit] NOT NULL,
	[Ended] [bit] NOT NULL,
	[EndedByNoBalance] [bit] NOT NULL,
	[LostSignalEmployee] [bit] NOT NULL,
	[LostSignalCustomer] [bit] NOT NULL,
	[Other] [bit] NOT NULL,
	[StageDescription] [nvarchar](255) NOT NULL,
	[StageIconUrl] [nvarchar](255) NULL,
	[StageName] [nvarchar](255) NULL,
	[Started] [bit] NOT NULL,
	[StillActiveWaitingOnEmployee] [bit] NOT NULL,
	[StillActiveWaitingOnCustomer] [bit] NOT NULL,

 CONSTRAINT [PK_ConversationMoreInfo] PRIMARY KEY CLUSTERED 
(
	[ConversationMoreInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConversationReconnection](
	[ReconnectionId] [uniqueidentifier] NOT NULL,
	[ReconnectionTimestamp] [datetime2](7) NOT NULL,
	[DurationSinceLastConnection] [int] NULL,
	[ReasonForReconnection] [nvarchar](255) NULL,
	[ConnectionStatusBefore] [nvarchar](255) NULL,
	[ConnectionStatusAfter] [nvarchar](255) NULL,
	[CustomerIsConnected] [bit] NOT NULL,
	[EmployeeIsConnected] [bit] NOT NULL,
	[StageDescription] [nvarchar](255) NULL,
	[StillActiveWaitingOnCustomer] [bit] NOT NULL,
	[StillActiveWaitingOnEmployee] [bit] NOT NULL,
	[ReconConversationSumTimeInSecs] [int] NOT NULL,
	[ReconConversationSumSpent] [decimal](18, 2) NOT NULL,
	[ReconEmployeeNeverResponded] [bit] NOT NULL,
	[ReconCustomerNeverResponded] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,

	[ConversationId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ConversationReconnection] PRIMARY KEY CLUSTERED 
(
	[ReconnectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConversationStage](
	[ConversationStageId] [uniqueidentifier] NOT NULL,
	[ConversationStageName] [nvarchar](255) NULL,
	[ConversationStageThumbnail] [varbinary](max) NULL,
	[ConversationDescription] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ConversationStage] PRIMARY KEY CLUSTERED 
(
	[ConversationStageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[CountryId] [uniqueidentifier] NOT NULL,
	[CountryName] [nvarchar](255) NOT NULL,
	[CountryCodeISO2] [nvarchar](2) NOT NULL,
	[CountryCodeISO3] [nvarchar](3) NOT NULL,
	[CountryIdISO] [int] NOT NULL,
	[CountryAreaCode] [nvarchar](5) NOT NULL,
	[ContinentId] [uniqueidentifier] NOT NULL,
	[SubcontinentId] [uniqueidentifier] NULL,
	[CurrencyId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryLocale](
	[CountryLocaleId] [uniqueidentifier] NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[CountryLocaleName] [nvarchar](255) NOT NULL,
	[ISOLanguageCode] [nvarchar](2) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CountryLocale] PRIMARY KEY CLUSTERED 
(
	[CountryLocaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[County](
	[CountyId] [uniqueidentifier] NOT NULL,
	[CountyCode] [nvarchar](50) NOT NULL,
	[CountyName] [nvarchar](255) NOT NULL,
	[CountyPostalCode] [nvarchar](25) NOT NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_County] PRIMARY KEY CLUSTERED 
(
	[CountyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currency](
	[CurrencyId] [uniqueidentifier] NOT NULL,
	[CurrencyName] [nvarchar](255) NOT NULL,
	[CurrencySymbol] [nvarchar](4) NOT NULL,
	[CurrencyCodeISO] [nvarchar](3) NOT NULL,
	[CurrencyIdISO] [int] NOT NULL,
	[CurrencyDecimalPlaces] [int] NOT NULL,
	[CurrencyDecimalSeparator] [char](1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrencyLocale](
	[CurrencyLocaleId] [uniqueidentifier] NOT NULL,
	[CurrencyLocaleName] [nvarchar](255) NOT NULL,
	[ISOLanguageCode] [nvarchar](2) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CurrencyLocale] PRIMARY KEY CLUSTERED 
(
	[CurrencyLocaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerId] [uniqueidentifier] NOT NULL,
	[CustomerCode] [nvarchar](50) NOT NULL,
	[CustomerIsPerson] [bit] NOT NULL,
	[CustomerFirstName] [nvarchar](150) NOT NULL,
	[CustomerLastName] [nvarchar](100) NULL,
	[CustomerCommercialName] [nvarchar](150) NULL,
	[TaxpayerSystemTypeId] [uniqueidentifier] NOT NULL,
	[CustomerCreditTermDays] [int] NOT NULL,
	[CustomerCreditLimitAmount] [decimal](18, 6) NOT NULL,
	[CustomerIsReseller] [bit] NOT NULL,
	[CustomerStatus] [int] NOT NULL,
	[CustomerUserTypeId] [uniqueidentifier] NOT NULL,
	[CustomerIsForeign] [bit] NOT NULL,
	[CustomerPrimaryCountryId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 [Website] [nvarchar](200) NULL,
	[ApplicationUserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerAddress](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[AddressId] [uniqueidentifier] NOT NULL,
	[AddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[IsPrimaryAddress] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerContact](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[ContactId] [uniqueidentifier] NOT NULL,
	[ContactSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryContact] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerContact] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerDocument](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [uniqueidentifier] NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CustomerDocument] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerDocumentIdentification](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationVerified] [datetime2](7) NULL,
	[DocumentIdentificationSystemTypeId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerDocumentIdentification] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerEconomicActivitySystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[EconomicActivitySystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryEconomicActivitySystemType] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerEconomicActivitySystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerEmailAddress](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[EmailAddressId] [uniqueidentifier] NOT NULL,
	[EmailAddressDateVerified] [datetime2](7) NULL,
	[EmailAddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryEmailAddress] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerEmailAddress] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerFeedback](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[FeedbackDate] [datetime2](7) NOT NULL,
	[FeedbackContent] [nvarchar](255) NOT NULL,
	[EmployeeRatingScore] [int] NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[ConversationId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
	[RatingReasonTypeId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CustomerFeedback] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerPayment](
	[CustomerPaymentId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[PaymentSystemTypeId] [uniqueidentifier] NOT NULL,
	[PaymentAmount] [decimal](18, 6) NOT NULL,
	[PaymentNumber] [nvarchar](100) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[PaymentReference] [nvarchar](100) NULL,
	[PaymentReferenceDate] [datetime2](7) NULL,
	[IsVoided] [bit] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[ReferenceBankId] [uniqueidentifier] NULL,
	[DepositBankAccountId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerPayment] PRIMARY KEY CLUSTERED 
(
	[CustomerPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerPhoneNumber](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[PhoneNumberId] [uniqueidentifier] NOT NULL,
	[PhoneNumberDateVerified] [datetime2](7) NULL,
	[PhoneNumberSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryPhoneNumber] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerPhoneNumber] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerSalesperson](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[SalespersonId] [uniqueidentifier] NOT NULL,
	[IsPrimarySalesperson] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerSalesperson] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerTaxSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[CustomerTaxSystemTypeComments] [nvarchar](255) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerTaxSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerUserType](
	[CustomerUserTypeId] [uniqueidentifier] NOT NULL,
	[CustomerUserTypeCode] [nvarchar](5) NOT NULL,
	[CustomerUserTypeName] [nvarchar](255) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerUserType] PRIMARY KEY CLUSTERED 
(
	[CustomerUserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[District](
	[DistrictId] [uniqueidentifier] NOT NULL,
	[DistrictCode] [nvarchar](50) NOT NULL,
	[DistrictName] [nvarchar](255) NOT NULL,
	[CountyId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_District] PRIMARY KEY CLUSTERED 
(
	[DistrictId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document](
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DocumentUri] [nvarchar](255) NOT NULL,
	[DocumentToken] [nvarchar](255) NOT NULL,
	[DocumentMetadata] [nvarchar](255) NOT NULL,
	[DocumentIconUrl] [nvarchar](255) NULL,
	[DocumentSecuredUrl] [nvarchar](255) NOT NULL,
	[DocumentTitle] [nvarchar](255) NOT NULL,
	[DocumentDescription] [nvarchar](255) NOT NULL,

 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentIdentification](
	[DocumentIdentificationId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationNumber] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentIdentification] PRIMARY KEY CLUSTERED 
(
	[DocumentIdentificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentIdentificationSystemType](
	[DocumentIdentificationSystemTypeId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationSystemTypeCode] [nvarchar](10) NOT NULL,
	[DocumentIdentificationSystemTypeName] [nvarchar](255) NOT NULL,
	[DocumentIdentificationSystemTypeRegex] [nvarchar](255) NULL,
	[IsRegexRequired] [bit] NOT NULL,
	[BoundedContextId] [uniqueidentifier] NULL,
	[CountryId] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentIdentificationSystemType] PRIMARY KEY CLUSTERED 
(
	[DocumentIdentificationSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentType](
	[DocumentTypeId] [uniqueidentifier] NOT NULL,
	[DocumentTypeName] [nvarchar](255) NOT NULL,
	[DocumentTypeDescription] [nvarchar](255) NULL,

 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EconomicActivitySystemType](
	[EconomicActivitySystemTypeId] [uniqueidentifier] NOT NULL,
	[EconomicActivitySystemTypeCode] [nvarchar](10) NOT NULL,
	[EconomicActivitySystemTypeName] [nvarchar](255) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_EconomicActivitySystemType] PRIMARY KEY CLUSTERED 
(
	[EconomicActivitySystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocument](
	[ElectronicDocumentId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ElectronicDocument] PRIMARY KEY CLUSTERED 
(
	[ElectronicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentAttribute](
	[ElectronicDocumentAttributeId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentAttributeSystemTypeId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentAttributeSystemTypeStringValue] [nvarchar](max) NULL,
	[ElectronicDocumentAttributeSystemTypeIntegerValue] [bigint] NULL,
	[ElectronicDocumentAttributeSystemTypeDecimalValue] [decimal](28, 12) NULL,
	[ElectronicDocumentAttributeSystemTypeBooleanValue] [bit] NULL,
	[ElectronicDocumentAttributeSystemTypeDateTimeValue] [datetime2](7) NULL,
	[ElectronicDocumentAttributeSystemTypeJsonValue] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentAttribute] PRIMARY KEY CLUSTERED 
(
	[ElectronicDocumentAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentAttributeSystemType](
	[ElectronicDocumentAttributeSystemTypeId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentAttributeSystemTypeCode] [nvarchar](50) NOT NULL,
	[ElectronicDocumentAttributeSystemTypeName] [nvarchar](255) NOT NULL,
	[ElectronicDocumentAttributeSystemTypeDescription] [nvarchar](500) NULL,
	[ElectronicDocumentAttributeSystemTypeFormula] [nvarchar](max) NULL,
	[ElectronicDocumentAttributeSystemTypeJsonSchema] [nvarchar](max) NULL,
	[ElectronicDocumentAttributeDataType] [nvarchar](20) NOT NULL,
	[ElectronicDocumentAttributeSystemTypeIsResponse] [bit] NOT NULL,
	[CountryId] [uniqueidentifier] NULL,
	[BoundedContextId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentAttributeSystemType] PRIMARY KEY CLUSTERED 
(
	[ElectronicDocumentAttributeSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentInvoice](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ElectronicDocumentId] [uniqueidentifier] NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentInvoice] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentPurchase](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ElectronicDocumentId] [uniqueidentifier] NOT NULL,
	[PurchaseId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentPurchase] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentTransmission](
	[ElectronicDocumentTransmissionId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentTransmissionSystemTypeId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentTransmissionStatus] [nvarchar](50) NOT NULL,
	[LastTransmissionAttemptDate] [datetime2](7) NULL,
	[LastTransmissionAttemptStatus] [nvarchar](50) NULL,
	[LastTransmissionAttemptErrorMessage] [nvarchar](max) NULL,
	[LastTransmissionAttemptResponsePayload] [nvarchar](max) NULL,
	[LastTransmissionAttemptResponseStatusCode] [int] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentTransmission] PRIMARY KEY CLUSTERED 
(
	[ElectronicDocumentTransmissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentTransmissionAttempt](
	[ElectronicDocumentTransmissionAttemptId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentTransmissionId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentTransmissionAttemptNumber] [int] NOT NULL,
	[ElectronicDocumentTransmissionAttemptURL] [nvarchar](max) NOT NULL,
	[ElectronicDocumentTransmissionAttemptHTTPMethod] [nvarchar](10) NOT NULL,
	[ElectronicDocumentTransmissionAttemptDate] [datetime2](7) NOT NULL,
	[ElectronicDocumentTransmissionAttemptStatus] [nvarchar](50) NOT NULL,
	[ElectronicDocumentTransmissionAttemptRequestPayload] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionAttemptRequestHeaders] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionAttemptResponsePayload] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionAttemptResponseHeaders] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionAttemptErrorMessage] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionAttemptResponseStatusCode] [int] NULL,
	[ElectronicDocumentTransmissionAttemptResponseStatusMessage] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentTransmissionAttempt] PRIMARY KEY CLUSTERED 
(
	[ElectronicDocumentTransmissionAttemptId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectronicDocumentTransmissionSystemType](
	[ElectronicDocumentTransmissionSystemTypeId] [uniqueidentifier] NOT NULL,
	[ElectronicDocumentTransmissionSystemTypeCode] [nvarchar](50) NOT NULL,
	[ElectronicDocumentTransmissionSystemTypeName] [nvarchar](255) NOT NULL,
	[ElectronicDocumentTransmissionSystemTypeDescription] [nvarchar](500) NULL,
	[ElectronicDocumentTransmissionSystemTypeURL] [nvarchar](max) NOT NULL,
	[ElectronicDocumentTransmissionSystemTypeHTTPMethod] [nvarchar](10) NOT NULL,
	[ElectronicDocumentTransmissionSystemTypePayloadType] [nvarchar](50) NOT NULL,
	[ElectronicDocumentTransmissionSystemTypePayloadSchema] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionSystemTypeHeaders] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionSystemTypeSuccessStatusCodes] [nvarchar](max) NULL,
	[ElectronicDocumentTransmissionSystemTypeContentType] [nvarchar](100) NOT NULL,
	[ElectronicDocumentTransmissionSystemTypeSuccessResponseRegex] [nvarchar](max) NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[InvoiceSystemTypeId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ElectronicDocumentTransmissionSystemType] PRIMARY KEY CLUSTERED 
(
	[ElectronicDocumentTransmissionSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailAddress](
	[EmailAddressId] [uniqueidentifier] NOT NULL,
	[EmailAddressString] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_EmailAddress] PRIMARY KEY CLUSTERED 
(
	[EmailAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailAddressSystemType](
	[EmailAddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[EmailAddressSystemTypeCode] [nvarchar](10) NOT NULL,
	[EmailAddressSystemTypeName] [nvarchar](255) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_EmailAddressSystemType] PRIMARY KEY CLUSTERED 
(
	[EmailAddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[EmployeeCode] [nvarchar](10) NOT NULL,
	[EmployeeFirstName] [nvarchar](150) NOT NULL,
	[EmployeeLastName] [nvarchar](100) NULL,
	[ApplicationUserId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeArea](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[AreaId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EmployeeArea] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeDocument](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EmployeeDocument] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeIdentityDocument](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [uniqueidentifier] NOT NULL,

 CONSTRAINT [PK_EmployeeIdentityDocument] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice](
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[InvoiceSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceDate] [datetime2](7) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[IsDraft] [bit] NOT NULL,
	[IsVoided] [bit] NOT NULL,
	[InvoiceStatus] [int] NOT NULL,
	[InvoiceForeignCurrencyId] [uniqueidentifier] NULL,
	[InvoiceForeignCurrencyRate] [decimal](18, 6) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
    	[InvoiceCreditTermDays] [int] NULL,

        	[InvoiceNumber] VARCHAR(50) NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceAccountReceivable](
	[InvoiceAccountReceivableId] [uniqueidentifier] NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[OriginalBalance] [decimal](18, 6) NOT NULL,
	[CurrentBalance] [decimal](18, 6) NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
	[LastPaymentDate] [datetime2](7) NULL,
	[IsVoided] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceAccountReceivable] PRIMARY KEY CLUSTERED 
(
	[InvoiceAccountReceivableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceAccountReceivablePayment](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceAccountReceivableId] [uniqueidentifier] NOT NULL,
	[CustomerPaymentId] [uniqueidentifier] NOT NULL,
	[AppliedAmount] [decimal](18, 6) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceAccountReceivablePayment] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceContact](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[ContactId] [uniqueidentifier] NOT NULL,
	[ContactSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryContact] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceContact] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCustomer](
	[InvoiceCustomerId] [uniqueidentifier] NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[InvoiceCustomerUserType] [int] NOT NULL,
	[InvoiceCustomerFirstName] [nvarchar](150) NOT NULL,
	[InvoiceCustomerLastName] [nvarchar](100) NULL,
	[InvoiceCustomerCommercialName] [nvarchar](150) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceCustomer] PRIMARY KEY CLUSTERED 
(
	[InvoiceCustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCustomerAddress](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceCustomerId] [uniqueidentifier] NOT NULL,
	[AddressId] [uniqueidentifier] NOT NULL,
	[AddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceCustomerAddressSystemType] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceCustomerAddress] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCustomerDocumentIdentification](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceCustomerId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationSystemTypeId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceCustomerDocumentIdentification] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCustomerEmailAddress](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceCustomerId] [uniqueidentifier] NOT NULL,
	[EmailAddressId] [uniqueidentifier] NOT NULL,
	[EmailAddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryEmailAddress] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceCustomerEmailAddress] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCustomerPhoneNumber](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceCustomerId] [uniqueidentifier] NOT NULL,
	[PhoneNumberId] [uniqueidentifier] NOT NULL,
	[PhoneNumberSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryPhoneNumber] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceCustomerPhoneNumber] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceDetail](
	[InvoiceDetailId] [uniqueidentifier] NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[InvoiceDetailGroupId] [uniqueidentifier] NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductSystemTypeId] [uniqueidentifier] NOT NULL,
	[PriceUserTypeId] [uniqueidentifier] NULL,
	[InvoiceDetailLineNumber] [decimal](18, 6) NULL,
	[InvoiceDetailDescription] [nvarchar](max) NOT NULL,
	[InvoiceDetailQuantity] [decimal](18, 6) NOT NULL,
	[InvoiceDetailUnitPrice] [decimal](28, 12) NOT NULL,
	[InvoiceDetailDiscountPercentage] [decimal](18, 6) NOT NULL,
	[InvoiceDetailLineTotal] [decimal](28, 12) NOT NULL,
	[MeasurementUnitSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceDetailMeasurementUnitSystemTypeQuantity] [decimal](18, 6) NOT NULL,
	[InvoiceDetailMeasurementUnitSystemTypePrice] [decimal](18, 6) NOT NULL,
	[InvoiceDetailMeasurementUnitSystemTypeName] [nvarchar](100) NOT NULL,
	[InvoiceDetailMeasurementUnitSystemTypeAbbreviation] [nvarchar](10) NOT NULL,
	[InvoiceDetailIsHiddenInGroup] [bit] NOT NULL,
	[InvoiceDetailUnitPriceForeignCurrency] [decimal](28, 12) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceDetail] PRIMARY KEY CLUSTERED 
(
	[InvoiceDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceDetailGroup](
	[InvoiceDetailGroupId] [uniqueidentifier] NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[InvoiceDetailGroupCode] [nvarchar](50) NOT NULL,
	[InvoiceDetailGroupDescription] [nvarchar](max) NULL,
	[InvoiceDetailGroupQuantity] [decimal](18, 6) NOT NULL,
	[InvoiceDetailGroupUnitPrice] [decimal](28, 12) NOT NULL,
	[InvoiceDetailGroupOrder] [decimal](18, 6) NOT NULL,
	[InvoiceDetailGroupPrintDetails] [bit] NOT NULL,
	[InvoiceDetailGroupUnitPriceForeignCurrency] [decimal](28, 12) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceDetailGroup] PRIMARY KEY CLUSTERED 
(
	[InvoiceDetailGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceDetailJunction](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[TableEntityId] [uniqueidentifier] NOT NULL,
	[RecordId] [uniqueidentifier] NOT NULL,
	[InvoiceDetailId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceDetailJunction] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceDetailTaxSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceDetailId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceDetailTaxSystemTypeAmount] [decimal](18, 6) NOT NULL,
	[InvoiceDetailTaxSystemTypeRate] [decimal](18, 6) NOT NULL,
	[InvoiceDetailTaxSystemTypeSign] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceDetailTaxSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceJournalSystemType](
	[InvoiceJournalSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceJournalSystemTypeCode] [nvarchar](50) NOT NULL,
	[InvoiceJournalSystemTypeName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceJournalSystemType] PRIMARY KEY CLUSTERED 
(
	[InvoiceJournalSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceSalesperson](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[SalespersonId] [uniqueidentifier] NOT NULL,
	[IsPrimarySalesperson] [bit] NOT NULL,
	[CommissionPercent] [decimal](18, 12) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceSalesperson] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceSystemType](
	[InvoiceSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceSystemTypeCode] [nvarchar](50) NOT NULL,
	[InvoiceSystemTypeName] [nvarchar](100) NOT NULL,
	[InvoiceSystemTypeIsSale] [bit] NOT NULL,
	[InvoiceSystemTypeSign] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceSystemType] PRIMARY KEY CLUSTERED 
(
	[InvoiceSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceSystemTypeInvoiceJournalSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceJournalSystemTypeId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_InvoiceSystemTypeInvoiceJournalSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceTaxSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[InvoiceTaxSystemTypeRate] [decimal](18, 6) NOT NULL,
	[InvoiceTaxSystemTypeAmount] [decimal](18, 6) NOT NULL,
	[InvoiceTaxSystemTypeSign] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_InvoiceTaxSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Localization](
	[LocalizationId] [uniqueidentifier] NOT NULL,
	[TableEntityId] [uniqueidentifier] NOT NULL,
	[TablePropertyId] [uniqueidentifier] NOT NULL,
	[LocalizationValue] [nvarchar](255) NOT NULL,
	[ISOLanguageCode] [nvarchar](2) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Localization] PRIMARY KEY CLUSTERED 
(
	[LocalizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeasurementUnitSystemType](
	[MeasurementUnitSystemTypeId] [uniqueidentifier] NOT NULL,
	[MeasurementUnitSystemTypeCode] [nvarchar](10) NOT NULL,
	[MeasurementUnitSystemTypeName] [nvarchar](100) NOT NULL,
	[MeasurementUnitSystemTypeAbbreviation] [nvarchar](10) NULL,
	[BoundedContextId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_MeasurementUnitSystemType] PRIMARY KEY CLUSTERED 
(
	[MeasurementUnitSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeasurementUnitTypeConversion](
	[MeasurementUnitTypeConversionId] [uniqueidentifier] NOT NULL,
	[FromMeasurementUnitSystemTypeId] [uniqueidentifier] NOT NULL,
	[ToMeasurementUnitSystemTypeId] [uniqueidentifier] NOT NULL,
	[ConversionFactor] [decimal](28, 12) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_MeasurementUnitTypeConversion] PRIMARY KEY CLUSTERED 
(
	[MeasurementUnitTypeConversionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Message](
	[MessageId] [uniqueidentifier] NOT NULL,
	[AiRobot] [bit] NOT NULL,
	[ApplicationUserId] [uniqueidentifier] NOT NULL,
	[ConversationId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[DeliveredToEmployeeAt] [datetime2](7) NULL,
	[DeliveredToCustomerAt] [datetime2](7) NULL,
	[HasAttachments] [bit] NOT NULL,
	[HasBeenDeliveredToEmployee] [bit] NOT NULL,
	[HasBeenDeliveredToCustomer] [bit] NOT NULL,
	[HasBeenReadByEmployee] [bit] NOT NULL,
	[HasBeenReadByCustomer] [bit] NOT NULL,
	[IsChat] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSensitive] [bit] NULL,
	[IsReplyToMessage] [bit] NULL,
	[IsVideoCall] [bit] NOT NULL,
	[IsVoiceCall] [bit] NOT NULL,
	[IsVoiceNote] [bit] NOT NULL,
	[LowBalance] [bit] NULL,
	[MessageContent] [nvarchar](255) NOT NULL,
	[ReplyToMessageContent] [nvarchar](255) NULL,
	[MessageDetailSpent] [decimal](18, 2) NOT NULL,
	[MessageDetailTimeInSecs] [int] NOT NULL,
	[MessageTypeId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ReadByEmployeeAt] [datetime2](7) NULL,
	[ReadByCustomerAt] [datetime2](7) NULL,
	[ReplyToMessageId] [uniqueidentifier] NOT NULL,
	[SentByEmployee] [bit] NOT NULL,
	[SentByEmployeeAt] [datetime2](7) NULL,
	[SentByCustomer] [bit] NOT NULL,
	[SentByCustomerAt] [datetime2](7) NULL,

	
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[VoiceNoteApproved] [bit] NULL,
	[VoiceNoteSize] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageDocument](
	[RowId] [int] IDENTITY(1,1) NOT NULL,

	[DocumentId] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [uniqueidentifier] NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_MessageDocument] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageType](
	[MessageTypeId] [uniqueidentifier] NOT NULL,
	[MessageTypeName] [nvarchar](255) NOT NULL,
	[MessageTypeDescription] [nvarchar](255) NULL,
 CONSTRAINT [PK_MessageType] PRIMARY KEY CLUSTERED 
(
	[MessageTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[NotificationId] [uniqueidentifier] NOT NULL,
	[ApplicationUserId] [uniqueidentifier] NOT NULL,
	[NotificationText] [nvarchar](255) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[NotificationTime] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,

	[MessageId] [uniqueidentifier] NOT NULL,
	[NotificationTypeId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationType](
	[NotificationTypeId] [uniqueidentifier] NOT NULL,
	[NotificationTypeName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[NotificationThumbnail] [varbinary](max) NULL,
 CONSTRAINT [PK_NotificationType] PRIMARY KEY CLUSTERED 
(
	[NotificationTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentSystemType](
	[PaymentSystemTypeId] [uniqueidentifier] NOT NULL,
	[PaymentSystemTypeCode] [nvarchar](50) NOT NULL,
	[PaymentSystemTypeName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsRemittable] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsRemovable] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PaymentSystemType] PRIMARY KEY CLUSTERED 
(
	[PaymentSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhoneNumber](
	[PhoneNumberId] [uniqueidentifier] NOT NULL,
	[PhoneNumberString] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PhoneNumber] PRIMARY KEY CLUSTERED 
(
	[PhoneNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhoneNumberSystemType](
	[PhoneNumberSystemTypeId] [uniqueidentifier] NOT NULL,
	[PhoneNumberSystemTypeCode] [nvarchar](5) NOT NULL,
	[PhoneNumberSystemTypeName] [nvarchar](255) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PhoneNumberSystemType] PRIMARY KEY CLUSTERED 
(
	[PhoneNumberSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceUserType](
	[PriceUserTypeId] [uniqueidentifier] NOT NULL,
	[PriceUserTypeCode] [nvarchar](50) NOT NULL,
	[PriceUserTypeName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsMinimumPrice] [bit] NOT NULL,
	[IsGlobalPrice] [bit] NOT NULL,
	[IsDiscountedInPrice] [bit] NOT NULL,
	[PriceDiscountPercentage] [decimal](18, 12) NULL,
	[PriceValidFrom] [datetime2](7) NULL,
	[PriceValidUntil] [datetime2](7) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PriceUserType] PRIMARY KEY CLUSTERED 
(
	[PriceUserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[ProductName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[UnitPrice] [decimal](28, 12) NOT NULL,
	[ProductSystemTypeId] [uniqueidentifier] NOT NULL,
	[PrimaryMeasurementUnitSystemTypeId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductBoundedContext](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductBoundedContext] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
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



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductMeasurementUnitSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[MeasurementUnitSystemTypeId] [uniqueidentifier] NOT NULL,
	[ProductMeasurementUnitSystemTypeQuantity] [decimal](18, 6) NOT NULL,
	[ProductMeasurementUnitSystemTypePrice] [decimal](18, 6) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductMeasurementUnitSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPriceUserType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[PriceUserTypeId] [uniqueidentifier] NOT NULL,
	[ProductPriceUserTypeAmount] [decimal](18, 6) NOT NULL,
	[ProductPriceUserTypeQuantityFrom] [decimal](18, 6) NULL,
	[ProductPriceUserTypeQuantityTo] [decimal](18, 6) NULL,
	[ProductPriceUserTypeValidFrom] [datetime2](7) NULL,
	[ProductPriceUserTypeValidUntil] [datetime2](7) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductPriceUserType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSystemType](
	[ProductSystemTypeId] [uniqueidentifier] NOT NULL,
	[ProductSystemTypeCode] [nvarchar](25) NOT NULL,
	[ProductSystemTypeName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProductSystemType] PRIMARY KEY CLUSTERED 
(
	[ProductSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchase](
	[PurchaseId] [uniqueidentifier] NOT NULL,
	[PurchaseSystemTypeId] [uniqueidentifier] NOT NULL,
	[PurchaseDate] [datetime2](7) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[IsDraft] [bit] NOT NULL,
	[IsVoided] [bit] NOT NULL,
	[PurchaseStatus] [int] NOT NULL,
	[PurchaseForeignCurrencyId] [uniqueidentifier] NULL,
	[PurchaseForeignCurrencyRate] [decimal](18, 6) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED 
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseAccountPayable](
	[PurchaseAccountPayableId] [uniqueidentifier] NOT NULL,
	[PurchaseId] [uniqueidentifier] NOT NULL,
	[OriginalBalance] [decimal](18, 6) NOT NULL,
	[CurrentBalance] [decimal](18, 6) NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
	[LastPaymentDate] [datetime2](7) NULL,
	[IsVoided] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseAccountPayable] PRIMARY KEY CLUSTERED 
(
	[PurchaseAccountPayableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseAccountPayablePayment](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseAccountPayableId] [uniqueidentifier] NOT NULL,
	[SupplierPaymentId] [uniqueidentifier] NOT NULL,
	[AppliedAmount] [decimal](18, 6) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseAccountPayablePayment] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseDetail](
	[PurchaseDetailId] [uniqueidentifier] NOT NULL,
	[PurchaseId] [uniqueidentifier] NOT NULL,
	[PurchaseDetailGroupId] [uniqueidentifier] NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductSystemTypeId] [uniqueidentifier] NOT NULL,
	[PurchaseDetailLineNumber] [decimal](18, 6) NULL,
	[PurchaseDetailDescription] [nvarchar](max) NOT NULL,
	[PurchaseDetailQuantity] [decimal](18, 6) NOT NULL,
	[PurchaseDetailUnitCost] [decimal](28, 12) NOT NULL,
	[PurchaseDetailDiscountPercentage] [decimal](18, 6) NOT NULL,
	[PurchaseDetailLineTotal] [decimal](28, 12) NOT NULL,
	[MeasurementUnitSystemTypeId] [uniqueidentifier] NOT NULL,
	[PurchaseDetailMeasurementUnitSystemTypeQuantity] [decimal](18, 6) NOT NULL,
	[PurchaseDetailMeasurementUnitSystemTypeCost] [decimal](18, 6) NOT NULL,
	[PurchaseDetailMeasurementUnitSystemTypeName] [nvarchar](100) NOT NULL,
	[PurchaseDetailMeasurementUnitSystemTypeAbbreviation] [nvarchar](10) NOT NULL,
	[PurchaseDetailIsHiddenInGroup] [bit] NOT NULL,
	[PurchaseDetailUnitCostForeignCurrency] [decimal](28, 12) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseDetail] PRIMARY KEY CLUSTERED 
(
	[PurchaseDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseSystemType](
	[PurchaseSystemTypeId] [uniqueidentifier] NOT NULL,
	[PurchaseSystemTypeCode] [nvarchar](50) NOT NULL,
	[PurchaseSystemTypeName] [nvarchar](100) NOT NULL,
	[PurchaseSystemTypeIsPurchase] [bit] NOT NULL,
	[PurchaseSystemTypeSign] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseSystemType] PRIMARY KEY CLUSTERED 
(
	[PurchaseSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RatingReasonType](
	[RatingReasonTypeId] [uniqueidentifier] NOT NULL,
	[RatingReasonDescription] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_RatingReasonType] PRIMARY KEY CLUSTERED 
(
	[RatingReasonTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salesperson](
	[SalespersonId] [uniqueidentifier] NOT NULL,
	[SalespersonCode] [nvarchar](10) NULL,
	[SalespersonFirstName] [nvarchar](150) NULL,
	[SalespersonLastName] [nvarchar](100) NULL,
	[EmployeeId] [uniqueidentifier] NULL,
	[ApplicationUserId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Salesperson] PRIMARY KEY CLUSTERED 
(
	[SalespersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Setting](
	[SettingId] [uniqueidentifier] NOT NULL,
	[SettingGroupId] [uniqueidentifier] NOT NULL,
	[SettingCode] [nvarchar](255) NOT NULL,
	[SettingName] [nvarchar](255) NOT NULL,
	[SettingValueType] [char](1) NOT NULL,
	[SettingDescription] [nvarchar](max) NULL,
	[BoundedContextId] [uniqueidentifier] NULL,
	[CountryId] [uniqueidentifier] NULL,
	[StringDefaultValue] [nvarchar](max) NULL,
	[NumericDefaultValue] [decimal](28, 12) NULL,
	[BooleanDefaultValue] [bit] NULL,
	[DateDefaultValue] [datetime2](7) NULL,
	[JsonDefaultValue] [nvarchar](max) NULL,
	[AppliesToApplicationSetting] [bit] NOT NULL,
	[AppliesToApplicationUserSetting] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SettingGroup](
	[SettingGroupId] [uniqueidentifier] NOT NULL,
	[SettingGroupName] [nvarchar](255) NOT NULL,
	[SettingGroupDescription] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SettingGroup] PRIMARY KEY CLUSTERED 
(
	[SettingGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[State](
	[StateId] [uniqueidentifier] NOT NULL,
	[StateCode] [nvarchar](5) NOT NULL,
	[StateName] [nvarchar](255) NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StateCountryGovCode](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[StateGovCode] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_StateCountryGovCode] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subcontinent](
	[SubcontinentId] [uniqueidentifier] NOT NULL,
	[ContinentId] [uniqueidentifier] NOT NULL,
	[SubcontinentName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Subcontinent] PRIMARY KEY CLUSTERED 
(
	[SubcontinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubcontinentLocale](
	[SubcontinentLocaleId] [uniqueidentifier] NOT NULL,
	[SubcontinentId] [uniqueidentifier] NOT NULL,
	[SubcontinentLocaleName] [nvarchar](255) NOT NULL,
	[ISOLanguageCode] [nvarchar](2) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SubcontinentLocale] PRIMARY KEY CLUSTERED 
(
	[SubcontinentLocaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[SupplierId] [uniqueidentifier] NOT NULL,
	[SupplierCode] [nvarchar](50) NOT NULL,
	[SupplierIsPerson] [bit] NOT NULL,
	[SupplierFirstName] [nvarchar](150) NOT NULL,
	[SupplierLastName] [nvarchar](100) NULL,
	[SupplierCommercialName] [nvarchar](150) NULL,
	[TaxpayerSystemTypeId] [uniqueidentifier] NOT NULL,
	[SupplierCreditTermDays] [int] NOT NULL,
	[SupplierCreditLimitAmount] [decimal](18, 6) NOT NULL,
	[SupplierStatus] [int] NOT NULL,
	[SupplierIsForeign] [bit] NOT NULL,
	[SupplierPrimaryCountryId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierAddress](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[AddressId] [uniqueidentifier] NOT NULL,
	[AddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryAddress] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierAddress] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierContact](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[ContactId] [uniqueidentifier] NOT NULL,
	[ContactSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryContact] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierContact] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierDocumentIdentification](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationSystemTypeId] [uniqueidentifier] NOT NULL,
	[DocumentIdentificationVerified] [datetime2](7) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierDocumentIdentification] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierEconomicActivitySystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[EconomicActivitySystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryEconomicActivitySystemType] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierEconomicActivitySystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierEmailAddress](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[EmailAddressId] [uniqueidentifier] NOT NULL,
	[EmailAddressSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryEmailAddress] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierEmailAddress] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierPayment](
	[SupplierPaymentId] [uniqueidentifier] NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[PaymentSystemTypeId] [uniqueidentifier] NOT NULL,
	[PaymentAmount] [decimal](18, 6) NOT NULL,
	[PaymentNumber] [nvarchar](100) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[PaymentReference] [nvarchar](100) NULL,
	[PaymentReferenceDate] [datetime2](7) NULL,
	[IsVoided] [bit] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[ReferenceBankId] [uniqueidentifier] NULL,
	[SourceBankAccountId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierPayment] PRIMARY KEY CLUSTERED 
(
	[SupplierPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierPhoneNumber](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[PhoneNumberId] [uniqueidentifier] NOT NULL,
	[PhoneNumberSystemTypeId] [uniqueidentifier] NOT NULL,
	[IsPrimaryPhoneNumber] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierPhoneNumber] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierTaxSystemType](
	[RowId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[SupplierTaxSystemTypeComments] [nvarchar](255) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SupplierTaxSystemType] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableEntity](
	[TableEntityId] [uniqueidentifier] NOT NULL,
	[TableEntityName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[TableEntityAlias] [nvarchar](15) NULL,
	[AggregateId] [uniqueidentifier] NULL,
	[IsAggregateRoot] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_TableEntity] PRIMARY KEY CLUSTERED 
(
	[TableEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableMetadata](
	[TableMetadataId] [uniqueidentifier] NOT NULL,
	[StrategyName] [nvarchar](50) NOT NULL,
	[EntityName] [nvarchar](50) NOT NULL,
	[UseRelatedEntitiesFromSPecificaionClass] [bit] NOT NULL,
	[StrategyAiPromtInstruction] [nvarchar](3999) NOT NULL,
	[IncludeUpdateCommand] [bit] NOT NULL,
	[IncludeCreateCommand] [bit] NOT NULL,
	[IncludeDeleteCommand] [bit] NOT NULL,
	[IncludeFetchDataByIdWithSpecificationCommand] [bit] NOT NULL,
	[IncludeCreateFactoryInsideTheCommand] [bit] NOT NULL,
	[UseBuilderInsideTheCreateFactory] [bit] NOT NULL,
	[IncludeRabbitMqQueue] [bit] NOT NULL,
	[RabbitMqQueueRoutingMappingKey] [nvarchar](50) NOT NULL,
	[RabbitMqQueueRoutingMappingValue] [nvarchar](50) NOT NULL,
	[IncludeMediatRHandler] [bit] NOT NULL,
	[IncludeSignalRNotification] [bit] NOT NULL,
	[SignalRHubMethodName] [nvarchar](50) NOT NULL,
	[IncludeAiDecisionSupport] [bit] NOT NULL,
	[UseStrategyPattern] [bit] NOT NULL,
	[UseFactoryPattern] [bit] NOT NULL,
	[RelatedEntities] [nvarchar](255) NOT NULL,
	[IncludeConcurrencyControl] [bit] NOT NULL,
	[IncludeTransactionScope] [bit] NOT NULL,
	[IncludeErrorHandling] [bit] NOT NULL,
	[CachingStrategy] [int] NOT NULL,
	[IncludeEventSourcing] [bit] NOT NULL,
	[IncludeValidation] [bit] NOT NULL,
	[IncludeThreadSafety] [bit] NOT NULL,
	[IncludeRoleBasedAccess] [bit] NOT NULL,
	[UseDecoratorPattern] [bit] NOT NULL,
	[IncludeLogging] [bit] NOT NULL,
	[IncludeAuthorization] [bit] NOT NULL,
 CONSTRAINT [PK_TableMetadata] PRIMARY KEY CLUSTERED 
(
	[TableMetadataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableProperty](
	[TablePropertyId] [uniqueidentifier] NOT NULL,
	[TableEntityId] [uniqueidentifier] NOT NULL,
	[TablePropertyName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[TablePropertySQLType] [int] NOT NULL,
	[TablePropertyDataType] [int] NOT NULL,
	[TablePropertyIsPK] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_TableProperty] PRIMARY KEY CLUSTERED 
(
	[TablePropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxpayerSystemType](
	[TaxpayerSystemTypeId] [uniqueidentifier] NOT NULL,
	[TaxpayerSystemTypeCode] [nvarchar](10) NOT NULL,
	[TaxpayerSystemTypeName] [nvarchar](255) NOT NULL,
	[TaxpayerSystemTypeValue] [decimal](18, 4) NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[BoundedContextId] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_TaxpayerSystemType] PRIMARY KEY CLUSTERED 
(
	[TaxpayerSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxSystemType](
	[TaxSystemTypeId] [uniqueidentifier] NOT NULL,
	[TaxSystemTypeCode] [nvarchar](10) NOT NULL,
	[TaxSystemTypeName] [nvarchar](255) NOT NULL,
	[TaxSystemTypeRate] [decimal](18, 6) NOT NULL,
	[TaxSystemTypeSign] [int] NOT NULL,
	[TaxSystemTypeMinimumTaxableValue] [decimal](18, 6) NOT NULL,
	[BoundedContextId] [uniqueidentifier] NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_TaxSystemType] PRIMARY KEY CLUSTERED 
(
	[TaxSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnansweredConversation](
	[UnansweredConversationId] [uniqueidentifier] NOT NULL,
	[Question] [nvarchar](255) NOT NULL,
	[AnsweredTime] [datetime2](7) NULL,
	[Answered] [bit] NULL,
	[Canceled] [bit] NULL,
	[Unanswered] [bit] NULL,
	[EmergencyIamInDanger] [bit] NULL,
	[AiRobot] [bit] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsChat] [bit] NULL,
	[IsVoiceCall] [bit] NULL,
	[IsVoiceNote] [bit] NULL,
	[IsVideoCall] [bit] NULL,

	[ProductId] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[AvailableEmployees] [nvarchar](max) NULL,
	[ConversationDescription] [nvarchar](255) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UnansweredConversation] PRIMARY KEY CLUSTERED 
(
	[UnansweredConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoiceNoteDocument](
	[RowId] [int] IDENTITY(1,1) NOT NULL,

	[DocumentId] [uniqueidentifier] NOT NULL,
	[DocumentTypeId] [uniqueidentifier] NOT NULL,
	[MessageId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_VoiceNoteDocument] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



















































ALTER TABLE [dbo].[CustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerAddress_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([AddressId])
GO
ALTER TABLE [dbo].[CustomerAddress] CHECK CONSTRAINT [FK_CustomerAddress_Address]
GO
ALTER TABLE [dbo].[InvoiceCustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerAddress_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([AddressId])
GO
ALTER TABLE [dbo].[InvoiceCustomerAddress] CHECK CONSTRAINT [FK_InvoiceCustomerAddress_Address]
GO
ALTER TABLE [dbo].[SupplierAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAddress_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([AddressId])
GO
ALTER TABLE [dbo].[SupplierAddress] CHECK CONSTRAINT [FK_SupplierAddress_Address]
GO
ALTER TABLE [dbo].[CustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerAddress_AddressSystemType] FOREIGN KEY([AddressSystemTypeId])
REFERENCES [dbo].[AddressSystemType] ([AddressSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerAddress] CHECK CONSTRAINT [FK_CustomerAddress_AddressSystemType]
GO
ALTER TABLE [dbo].[InvoiceCustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerAddress_AddressSystemType] FOREIGN KEY([AddressSystemTypeId])
REFERENCES [dbo].[AddressSystemType] ([AddressSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceCustomerAddress] CHECK CONSTRAINT [FK_InvoiceCustomerAddress_AddressSystemType]
GO
ALTER TABLE [dbo].[SupplierAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAddress_AddressSystemType] FOREIGN KEY([AddressSystemTypeId])
REFERENCES [dbo].[AddressSystemType] ([AddressSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierAddress] CHECK CONSTRAINT [FK_SupplierAddress_AddressSystemType]
GO
ALTER TABLE [dbo].[TableEntity]  WITH CHECK ADD  CONSTRAINT [FK_TableEntity_Aggregate] FOREIGN KEY([AggregateId])
REFERENCES [dbo].[Aggregate] ([AggregateId])
GO
ALTER TABLE [dbo].[TableEntity] CHECK CONSTRAINT [FK_TableEntity_Aggregate]
GO
ALTER TABLE [dbo].[ApplicationUserSetting]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationUserSetting_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[ApplicationUserSetting] CHECK CONSTRAINT [FK_ApplicationUserSetting_ApplicationUser]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_ApplicationUser]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_ApplicationUser]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_ApplicationUser]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_ApplicationUser]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_ApplicationUser]
GO
ALTER TABLE [dbo].[Salesperson]  WITH CHECK ADD  CONSTRAINT [FK_Salesperson_ApplicationUser] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[ApplicationUser] ([ApplicationUserId])
GO
ALTER TABLE [dbo].[Salesperson] CHECK CONSTRAINT [FK_Salesperson_ApplicationUser]
GO
ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO
ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_Area]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_Area]
GO
ALTER TABLE [dbo].[EmployeeArea]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeArea_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO
ALTER TABLE [dbo].[EmployeeArea] CHECK CONSTRAINT [FK_EmployeeArea_Area]
GO
ALTER TABLE [dbo].[AttachmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentDetail_AttachmentType] FOREIGN KEY([AttachmentTypeId])
REFERENCES [dbo].[AttachmentType] ([AttachmentTypeId])
GO
ALTER TABLE [dbo].[AttachmentDetail] CHECK CONSTRAINT [FK_AttachmentDetail_AttachmentType]
GO
ALTER TABLE [dbo].[BankAccount]  WITH CHECK ADD  CONSTRAINT [FK_BankAccount_Bank] FOREIGN KEY([BankId])
REFERENCES [dbo].[Bank] ([BankId])
GO
ALTER TABLE [dbo].[BankAccount] CHECK CONSTRAINT [FK_BankAccount_Bank]
GO
ALTER TABLE [dbo].[BankTransaction]  WITH CHECK ADD  CONSTRAINT [FK_BankTransaction_BankAccount] FOREIGN KEY([BankAccountId])
REFERENCES [dbo].[BankAccount] ([BankAccountId])
GO
ALTER TABLE [dbo].[BankTransaction] CHECK CONSTRAINT [FK_BankTransaction_BankAccount]
GO
ALTER TABLE [dbo].[BankAccount]  WITH CHECK ADD  CONSTRAINT [FK_BankAccount_BankAccountSystemType] FOREIGN KEY([BankAccountSystemTypeId])
REFERENCES [dbo].[BankAccountSystemType] ([BankAccountSystemTypeId])
GO
ALTER TABLE [dbo].[BankAccount] CHECK CONSTRAINT [FK_BankAccount_BankAccountSystemType]
GO
ALTER TABLE [dbo].[BankTransaction]  WITH CHECK ADD  CONSTRAINT [FK_BankTransaction_BankTransactionSystemType] FOREIGN KEY([BankTransactionSystemTypeId])
REFERENCES [dbo].[BankTransactionSystemType] ([BankTransactionSystemTypeId])
GO
ALTER TABLE [dbo].[BankTransaction] CHECK CONSTRAINT [FK_BankTransaction_BankTransactionSystemType]
GO
ALTER TABLE [dbo].[AddressSystemType]  WITH CHECK ADD  CONSTRAINT [FK_AddressSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[AddressSystemType] CHECK CONSTRAINT [FK_AddressSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[Aggregate]  WITH CHECK ADD  CONSTRAINT [FK_Aggregate_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[Aggregate] CHECK CONSTRAINT [FK_Aggregate_BoundedContext]
GO
ALTER TABLE [dbo].[Classification]  WITH CHECK ADD  CONSTRAINT [FK_Classification_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[Classification] CHECK CONSTRAINT [FK_Classification_BoundedContext]
GO
ALTER TABLE [dbo].[ContactSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ContactSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[ContactSystemType] CHECK CONSTRAINT [FK_ContactSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[CustomerUserType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerUserType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[CustomerUserType] CHECK CONSTRAINT [FK_CustomerUserType_BoundedContext]
GO
ALTER TABLE [dbo].[DocumentIdentificationSystemType]  WITH CHECK ADD  CONSTRAINT [FK_DocumentIdentificationSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[DocumentIdentificationSystemType] CHECK CONSTRAINT [FK_DocumentIdentificationSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[EconomicActivitySystemType]  WITH CHECK ADD  CONSTRAINT [FK_EconomicActivitySystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[EconomicActivitySystemType] CHECK CONSTRAINT [FK_EconomicActivitySystemType_BoundedContext]
GO
ALTER TABLE [dbo].[ElectronicDocumentAttributeSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentAttributeSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[ElectronicDocumentAttributeSystemType] CHECK CONSTRAINT [FK_ElectronicDocumentAttributeSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentTransmissionSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionSystemType] CHECK CONSTRAINT [FK_ElectronicDocumentTransmissionSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[EmailAddressSystemType]  WITH CHECK ADD  CONSTRAINT [FK_EmailAddressSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[EmailAddressSystemType] CHECK CONSTRAINT [FK_EmailAddressSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[MeasurementUnitSystemType]  WITH CHECK ADD  CONSTRAINT [FK_MeasurementUnitSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[MeasurementUnitSystemType] CHECK CONSTRAINT [FK_MeasurementUnitSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[PhoneNumberSystemType]  WITH CHECK ADD  CONSTRAINT [FK_PhoneNumberSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[PhoneNumberSystemType] CHECK CONSTRAINT [FK_PhoneNumberSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[ProductBoundedContext]  WITH CHECK ADD  CONSTRAINT [FK_ProductBoundedContext_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[ProductBoundedContext] CHECK CONSTRAINT [FK_ProductBoundedContext_BoundedContext]
GO
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_BoundedContext]
GO
ALTER TABLE [dbo].[TaxpayerSystemType]  WITH CHECK ADD  CONSTRAINT [FK_TaxpayerSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[TaxpayerSystemType] CHECK CONSTRAINT [FK_TaxpayerSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[TaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_TaxSystemType_BoundedContext] FOREIGN KEY([BoundedContextId])
REFERENCES [dbo].[BoundedContext] ([BoundedContextId])
GO
ALTER TABLE [dbo].[TaxSystemType] CHECK CONSTRAINT [FK_TaxSystemType_BoundedContext]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_Category]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategory_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_ProductCategory_Category]
GO
ALTER TABLE [dbo].[UnansweredConversation]  WITH CHECK ADD  CONSTRAINT [FK_UnansweredConversation_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[UnansweredConversation] CHECK CONSTRAINT [FK_UnansweredConversation_Category]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_City] FOREIGN KEY([CityId])
REFERENCES [dbo].[City] ([CityId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_City]
GO
ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_Classification] FOREIGN KEY([ClassificationId])
REFERENCES [dbo].[Classification] ([ClassificationId])
GO
ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_Classification]
GO
ALTER TABLE [dbo].[CustomerContact]  WITH CHECK ADD  CONSTRAINT [FK_CustomerContact_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[CustomerContact] CHECK CONSTRAINT [FK_CustomerContact_Contact]
GO
ALTER TABLE [dbo].[InvoiceContact]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceContact_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[InvoiceContact] CHECK CONSTRAINT [FK_InvoiceContact_Contact]
GO
ALTER TABLE [dbo].[SupplierContact]  WITH CHECK ADD  CONSTRAINT [FK_SupplierContact_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[SupplierContact] CHECK CONSTRAINT [FK_SupplierContact_Contact]
GO
ALTER TABLE [dbo].[CustomerContact]  WITH CHECK ADD  CONSTRAINT [FK_CustomerContact_ContactSystemType] FOREIGN KEY([ContactSystemTypeId])
REFERENCES [dbo].[ContactSystemType] ([ContactSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerContact] CHECK CONSTRAINT [FK_CustomerContact_ContactSystemType]
GO
ALTER TABLE [dbo].[InvoiceContact]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceContact_ContactSystemType] FOREIGN KEY([ContactSystemTypeId])
REFERENCES [dbo].[ContactSystemType] ([ContactSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceContact] CHECK CONSTRAINT [FK_InvoiceContact_ContactSystemType]
GO
ALTER TABLE [dbo].[SupplierContact]  WITH CHECK ADD  CONSTRAINT [FK_SupplierContact_ContactSystemType] FOREIGN KEY([ContactSystemTypeId])
REFERENCES [dbo].[ContactSystemType] ([ContactSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierContact] CHECK CONSTRAINT [FK_SupplierContact_ContactSystemType]
GO
ALTER TABLE [dbo].[ContinentLocale]  WITH CHECK ADD  CONSTRAINT [FK_ContinentLocale_Continent] FOREIGN KEY([ContinentId])
REFERENCES [dbo].[Continent] ([ContinentId])
GO
ALTER TABLE [dbo].[ContinentLocale] CHECK CONSTRAINT [FK_ContinentLocale_Continent]
GO
ALTER TABLE [dbo].[Country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Continent] FOREIGN KEY([ContinentId])
REFERENCES [dbo].[Continent] ([ContinentId])
GO
ALTER TABLE [dbo].[Country] CHECK CONSTRAINT [FK_Country_Continent]
GO
ALTER TABLE [dbo].[Subcontinent]  WITH CHECK ADD  CONSTRAINT [FK_Subcontinent_Continent] FOREIGN KEY([ContinentId])
REFERENCES [dbo].[Continent] ([ContinentId])
GO
ALTER TABLE [dbo].[Subcontinent] CHECK CONSTRAINT [FK_Subcontinent_Continent]
GO
ALTER TABLE [dbo].[ConversationReconnection]  WITH CHECK ADD  CONSTRAINT [FK_ConversationReconnection_Conversation] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversation] ([ConversationId])
GO
ALTER TABLE [dbo].[ConversationReconnection] CHECK CONSTRAINT [FK_ConversationReconnection_Conversation]
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFeedback_Conversation] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversation] ([ConversationId])
GO
ALTER TABLE [dbo].[CustomerFeedback] CHECK CONSTRAINT [FK_CustomerFeedback_Conversation]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Conversation] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversation] ([ConversationId])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_Conversation]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_ConversationMoreInfo] FOREIGN KEY([ConversationMoreInfoId])
REFERENCES [dbo].[ConversationMoreInfo] ([ConversationMoreInfoId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_ConversationMoreInfo]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_ConversationStage] FOREIGN KEY([ConversationStageId])
REFERENCES [dbo].[ConversationStage] ([ConversationStageId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_ConversationStage]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Country]
GO
ALTER TABLE [dbo].[Area]  WITH CHECK ADD  CONSTRAINT [FK_Area_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[Area] CHECK CONSTRAINT [FK_Area_Country]
GO
ALTER TABLE [dbo].[Bank]  WITH CHECK ADD  CONSTRAINT [FK_Bank_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[Bank] CHECK CONSTRAINT [FK_Bank_Country]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_Country]
GO
ALTER TABLE [dbo].[CountryLocale]  WITH CHECK ADD  CONSTRAINT [FK_CountryLocale_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[CountryLocale] CHECK CONSTRAINT [FK_CountryLocale_Country]
GO
ALTER TABLE [dbo].[DocumentIdentificationSystemType]  WITH CHECK ADD  CONSTRAINT [FK_DocumentIdentificationSystemType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[DocumentIdentificationSystemType] CHECK CONSTRAINT [FK_DocumentIdentificationSystemType_Country]
GO
ALTER TABLE [dbo].[EconomicActivitySystemType]  WITH CHECK ADD  CONSTRAINT [FK_EconomicActivitySystemType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[EconomicActivitySystemType] CHECK CONSTRAINT [FK_EconomicActivitySystemType_Country]
GO
ALTER TABLE [dbo].[ElectronicDocumentAttributeSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentAttributeSystemType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[ElectronicDocumentAttributeSystemType] CHECK CONSTRAINT [FK_ElectronicDocumentAttributeSystemType_Country]
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentTransmissionSystemType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionSystemType] CHECK CONSTRAINT [FK_ElectronicDocumentTransmissionSystemType_Country]
GO
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_Country]
GO
ALTER TABLE [dbo].[State]  WITH CHECK ADD  CONSTRAINT [FK_State_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[State] CHECK CONSTRAINT [FK_State_Country]
GO
ALTER TABLE [dbo].[StateCountryGovCode]  WITH CHECK ADD  CONSTRAINT [FK_StateCountryGovCode_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[StateCountryGovCode] CHECK CONSTRAINT [FK_StateCountryGovCode_Country]
GO
ALTER TABLE [dbo].[TaxpayerSystemType]  WITH CHECK ADD  CONSTRAINT [FK_TaxpayerSystemType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[TaxpayerSystemType] CHECK CONSTRAINT [FK_TaxpayerSystemType_Country]
GO
ALTER TABLE [dbo].[TaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_TaxSystemType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO
ALTER TABLE [dbo].[TaxSystemType] CHECK CONSTRAINT [FK_TaxSystemType_Country]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_County] FOREIGN KEY([CountyId])
REFERENCES [dbo].[County] ([CountyId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_County]
GO
ALTER TABLE [dbo].[District]  WITH CHECK ADD  CONSTRAINT [FK_District_County] FOREIGN KEY([CountyId])
REFERENCES [dbo].[County] ([CountyId])
GO
ALTER TABLE [dbo].[District] CHECK CONSTRAINT [FK_District_County]
GO
ALTER TABLE [dbo].[BankAccount]  WITH CHECK ADD  CONSTRAINT [FK_BankAccount_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])
GO
ALTER TABLE [dbo].[BankAccount] CHECK CONSTRAINT [FK_BankAccount_Currency]
GO
ALTER TABLE [dbo].[BankTransaction]  WITH CHECK ADD  CONSTRAINT [FK_BankTransaction_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])
GO
ALTER TABLE [dbo].[BankTransaction] CHECK CONSTRAINT [FK_BankTransaction_Currency]
GO
ALTER TABLE [dbo].[Country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([CurrencyId])
GO
ALTER TABLE [dbo].[Country] CHECK CONSTRAINT [FK_Country_Currency]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_Customer]
GO
ALTER TABLE [dbo].[CustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerAddress_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerAddress] CHECK CONSTRAINT [FK_CustomerAddress_Customer]
GO
ALTER TABLE [dbo].[CustomerContact]  WITH CHECK ADD  CONSTRAINT [FK_CustomerContact_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerContact] CHECK CONSTRAINT [FK_CustomerContact_Customer]
GO
ALTER TABLE [dbo].[CustomerDocument]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocument_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerDocument] CHECK CONSTRAINT [FK_CustomerDocument_Customer]
GO
ALTER TABLE [dbo].[CustomerDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocumentIdentification_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerDocumentIdentification] CHECK CONSTRAINT [FK_CustomerDocumentIdentification_Customer]
GO
ALTER TABLE [dbo].[CustomerEconomicActivitySystemType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerEconomicActivitySystemType_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerEconomicActivitySystemType] CHECK CONSTRAINT [FK_CustomerEconomicActivitySystemType_Customer]
GO
ALTER TABLE [dbo].[CustomerEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerEmailAddress_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerEmailAddress] CHECK CONSTRAINT [FK_CustomerEmailAddress_Customer]
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFeedback_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerFeedback] CHECK CONSTRAINT [FK_CustomerFeedback_Customer]
GO
ALTER TABLE [dbo].[CustomerPayment]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPayment_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerPayment] CHECK CONSTRAINT [FK_CustomerPayment_Customer]
GO
ALTER TABLE [dbo].[CustomerPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPhoneNumber_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerPhoneNumber] CHECK CONSTRAINT [FK_CustomerPhoneNumber_Customer]
GO
ALTER TABLE [dbo].[CustomerSalesperson]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSalesperson_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerSalesperson] CHECK CONSTRAINT [FK_CustomerSalesperson_Customer]
GO
ALTER TABLE [dbo].[CustomerTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerTaxSystemType_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[CustomerTaxSystemType] CHECK CONSTRAINT [FK_CustomerTaxSystemType_Customer]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_Customer]
GO
ALTER TABLE [dbo].[InvoiceCustomer]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[InvoiceCustomer] CHECK CONSTRAINT [FK_InvoiceCustomer_Customer]
GO
ALTER TABLE [dbo].[UnansweredConversation]  WITH CHECK ADD  CONSTRAINT [FK_UnansweredConversation_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[UnansweredConversation] CHECK CONSTRAINT [FK_UnansweredConversation_Customer]
GO
ALTER TABLE [dbo].[InvoiceAccountReceivablePayment]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceAccountReceivablePayment_CustomerPayment] FOREIGN KEY([CustomerPaymentId])
REFERENCES [dbo].[CustomerPayment] ([CustomerPaymentId])
GO
ALTER TABLE [dbo].[InvoiceAccountReceivablePayment] CHECK CONSTRAINT [FK_InvoiceAccountReceivablePayment_CustomerPayment]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_CustomerUserType] FOREIGN KEY([CustomerUserTypeId])
REFERENCES [dbo].[CustomerUserType] ([CustomerUserTypeId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_CustomerUserType]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_District] FOREIGN KEY([DistrictId])
REFERENCES [dbo].[District] ([DistrictId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_District]
GO
ALTER TABLE [dbo].[CustomerDocument]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([DocumentId])
GO
ALTER TABLE [dbo].[CustomerDocument] CHECK CONSTRAINT [FK_CustomerDocument_Document]
GO
ALTER TABLE [dbo].[EmployeeDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([DocumentId])
GO
ALTER TABLE [dbo].[EmployeeDocument] CHECK CONSTRAINT [FK_EmployeeDocument_Document]
GO
ALTER TABLE [dbo].[EmployeeIdentityDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeIdentityDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([DocumentId])
GO
ALTER TABLE [dbo].[EmployeeIdentityDocument] CHECK CONSTRAINT [FK_EmployeeIdentityDocument_Document]
GO
ALTER TABLE [dbo].[MessageDocument]  WITH CHECK ADD  CONSTRAINT [FK_MessageDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([DocumentId])
GO
ALTER TABLE [dbo].[MessageDocument] CHECK CONSTRAINT [FK_MessageDocument_Document]
GO
ALTER TABLE [dbo].[VoiceNoteDocument]  WITH CHECK ADD  CONSTRAINT [FK_VoiceNoteDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([DocumentId])
GO
ALTER TABLE [dbo].[VoiceNoteDocument] CHECK CONSTRAINT [FK_VoiceNoteDocument_Document]
GO
ALTER TABLE [dbo].[CustomerDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocumentIdentification_DocumentIdentification] FOREIGN KEY([DocumentIdentificationId])
REFERENCES [dbo].[DocumentIdentification] ([DocumentIdentificationId])
GO
ALTER TABLE [dbo].[CustomerDocumentIdentification] CHECK CONSTRAINT [FK_CustomerDocumentIdentification_DocumentIdentification]
GO
ALTER TABLE [dbo].[InvoiceCustomerDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerDocumentIdentification_DocumentIdentification] FOREIGN KEY([DocumentIdentificationId])
REFERENCES [dbo].[DocumentIdentification] ([DocumentIdentificationId])
GO
ALTER TABLE [dbo].[InvoiceCustomerDocumentIdentification] CHECK CONSTRAINT [FK_InvoiceCustomerDocumentIdentification_DocumentIdentification]
GO
ALTER TABLE [dbo].[SupplierDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_SupplierDocumentIdentification_DocumentIdentification] FOREIGN KEY([DocumentIdentificationId])
REFERENCES [dbo].[DocumentIdentification] ([DocumentIdentificationId])
GO
ALTER TABLE [dbo].[SupplierDocumentIdentification] CHECK CONSTRAINT [FK_SupplierDocumentIdentification_DocumentIdentification]
GO
ALTER TABLE [dbo].[CustomerDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocumentIdentification_DocumentIdentificationSystemType] FOREIGN KEY([DocumentIdentificationSystemTypeId])
REFERENCES [dbo].[DocumentIdentificationSystemType] ([DocumentIdentificationSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerDocumentIdentification] CHECK CONSTRAINT [FK_CustomerDocumentIdentification_DocumentIdentificationSystemType]
GO
ALTER TABLE [dbo].[InvoiceCustomerDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerDocumentIdentification_DocumentIdentificationSystemType] FOREIGN KEY([DocumentIdentificationSystemTypeId])
REFERENCES [dbo].[DocumentIdentificationSystemType] ([DocumentIdentificationSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceCustomerDocumentIdentification] CHECK CONSTRAINT [FK_InvoiceCustomerDocumentIdentification_DocumentIdentificationSystemType]
GO
ALTER TABLE [dbo].[SupplierDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_SupplierDocumentIdentification_DocumentIdentificationSystemType] FOREIGN KEY([DocumentIdentificationSystemTypeId])
REFERENCES [dbo].[DocumentIdentificationSystemType] ([DocumentIdentificationSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierDocumentIdentification] CHECK CONSTRAINT [FK_SupplierDocumentIdentification_DocumentIdentificationSystemType]
GO
ALTER TABLE [dbo].[CustomerDocument]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocument_DocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[CustomerDocument] CHECK CONSTRAINT [FK_CustomerDocument_DocumentType]
GO
ALTER TABLE [dbo].[EmployeeDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocument_DocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[EmployeeDocument] CHECK CONSTRAINT [FK_EmployeeDocument_DocumentType]
GO
ALTER TABLE [dbo].[EmployeeIdentityDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeIdentityDocument_DocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[EmployeeIdentityDocument] CHECK CONSTRAINT [FK_EmployeeIdentityDocument_DocumentType]
GO
ALTER TABLE [dbo].[MessageDocument]  WITH CHECK ADD  CONSTRAINT [FK_MessageDocument_DocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[MessageDocument] CHECK CONSTRAINT [FK_MessageDocument_DocumentType]
GO
ALTER TABLE [dbo].[VoiceNoteDocument]  WITH CHECK ADD  CONSTRAINT [FK_VoiceNoteDocument_DocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[VoiceNoteDocument] CHECK CONSTRAINT [FK_VoiceNoteDocument_DocumentType]
GO
ALTER TABLE [dbo].[CustomerEconomicActivitySystemType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerEconomicActivitySystemType_EconomicActivitySystemType] FOREIGN KEY([EconomicActivitySystemTypeId])
REFERENCES [dbo].[EconomicActivitySystemType] ([EconomicActivitySystemTypeId])
GO
ALTER TABLE [dbo].[CustomerEconomicActivitySystemType] CHECK CONSTRAINT [FK_CustomerEconomicActivitySystemType_EconomicActivitySystemType]
GO
ALTER TABLE [dbo].[SupplierEconomicActivitySystemType]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEconomicActivitySystemType_EconomicActivitySystemType] FOREIGN KEY([EconomicActivitySystemTypeId])
REFERENCES [dbo].[EconomicActivitySystemType] ([EconomicActivitySystemTypeId])
GO
ALTER TABLE [dbo].[SupplierEconomicActivitySystemType] CHECK CONSTRAINT [FK_SupplierEconomicActivitySystemType_EconomicActivitySystemType]
GO
ALTER TABLE [dbo].[ElectronicDocumentAttribute]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentAttribute_ElectronicDocument] FOREIGN KEY([ElectronicDocumentId])
REFERENCES [dbo].[ElectronicDocument] ([ElectronicDocumentId])
GO
ALTER TABLE [dbo].[ElectronicDocumentAttribute] CHECK CONSTRAINT [FK_ElectronicDocumentAttribute_ElectronicDocument]
GO
ALTER TABLE [dbo].[ElectronicDocumentInvoice]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentInvoice_ElectronicDocument] FOREIGN KEY([ElectronicDocumentId])
REFERENCES [dbo].[ElectronicDocument] ([ElectronicDocumentId])
GO
ALTER TABLE [dbo].[ElectronicDocumentInvoice] CHECK CONSTRAINT [FK_ElectronicDocumentInvoice_ElectronicDocument]
GO
ALTER TABLE [dbo].[ElectronicDocumentPurchase]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentPurchase_ElectronicDocument] FOREIGN KEY([ElectronicDocumentId])
REFERENCES [dbo].[ElectronicDocument] ([ElectronicDocumentId])
GO
ALTER TABLE [dbo].[ElectronicDocumentPurchase] CHECK CONSTRAINT [FK_ElectronicDocumentPurchase_ElectronicDocument]
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmission]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentTransmission_ElectronicDocument] FOREIGN KEY([ElectronicDocumentId])
REFERENCES [dbo].[ElectronicDocument] ([ElectronicDocumentId])
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmission] CHECK CONSTRAINT [FK_ElectronicDocumentTransmission_ElectronicDocument]
GO
ALTER TABLE [dbo].[ElectronicDocumentAttribute]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentAttribute_ElectronicDocumentAttributeSystemType] FOREIGN KEY([ElectronicDocumentAttributeSystemTypeId])
REFERENCES [dbo].[ElectronicDocumentAttributeSystemType] ([ElectronicDocumentAttributeSystemTypeId])
GO
ALTER TABLE [dbo].[ElectronicDocumentAttribute] CHECK CONSTRAINT [FK_ElectronicDocumentAttribute_ElectronicDocumentAttributeSystemType]
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionAttempt]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentTransmissionAttempt_ElectronicDocumentTransmission] FOREIGN KEY([ElectronicDocumentTransmissionId])
REFERENCES [dbo].[ElectronicDocumentTransmission] ([ElectronicDocumentTransmissionId])
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionAttempt] CHECK CONSTRAINT [FK_ElectronicDocumentTransmissionAttempt_ElectronicDocumentTransmission]
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmission]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentTransmission_ElectronicDocumentTransmissionSystemType] FOREIGN KEY([ElectronicDocumentTransmissionSystemTypeId])
REFERENCES [dbo].[ElectronicDocumentTransmissionSystemType] ([ElectronicDocumentTransmissionSystemTypeId])
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmission] CHECK CONSTRAINT [FK_ElectronicDocumentTransmission_ElectronicDocumentTransmissionSystemType]
GO
ALTER TABLE [dbo].[CustomerEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerEmailAddress_EmailAddress] FOREIGN KEY([EmailAddressId])
REFERENCES [dbo].[EmailAddress] ([EmailAddressId])
GO
ALTER TABLE [dbo].[CustomerEmailAddress] CHECK CONSTRAINT [FK_CustomerEmailAddress_EmailAddress]
GO
ALTER TABLE [dbo].[InvoiceCustomerEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerEmailAddress_EmailAddress] FOREIGN KEY([EmailAddressId])
REFERENCES [dbo].[EmailAddress] ([EmailAddressId])
GO
ALTER TABLE [dbo].[InvoiceCustomerEmailAddress] CHECK CONSTRAINT [FK_InvoiceCustomerEmailAddress_EmailAddress]
GO
ALTER TABLE [dbo].[SupplierEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEmailAddress_EmailAddress] FOREIGN KEY([EmailAddressId])
REFERENCES [dbo].[EmailAddress] ([EmailAddressId])
GO
ALTER TABLE [dbo].[SupplierEmailAddress] CHECK CONSTRAINT [FK_SupplierEmailAddress_EmailAddress]
GO
ALTER TABLE [dbo].[CustomerEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerEmailAddress_EmailAddressSystemType] FOREIGN KEY([EmailAddressSystemTypeId])
REFERENCES [dbo].[EmailAddressSystemType] ([EmailAddressSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerEmailAddress] CHECK CONSTRAINT [FK_CustomerEmailAddress_EmailAddressSystemType]
GO
ALTER TABLE [dbo].[InvoiceCustomerEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerEmailAddress_EmailAddressSystemType] FOREIGN KEY([EmailAddressSystemTypeId])
REFERENCES [dbo].[EmailAddressSystemType] ([EmailAddressSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceCustomerEmailAddress] CHECK CONSTRAINT [FK_InvoiceCustomerEmailAddress_EmailAddressSystemType]
GO
ALTER TABLE [dbo].[SupplierEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEmailAddress_EmailAddressSystemType] FOREIGN KEY([EmailAddressSystemTypeId])
REFERENCES [dbo].[EmailAddressSystemType] ([EmailAddressSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierEmailAddress] CHECK CONSTRAINT [FK_SupplierEmailAddress_EmailAddressSystemType]
GO
ALTER TABLE [dbo].[Conversation]  WITH CHECK ADD  CONSTRAINT [FK_Conversation_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[Conversation] CHECK CONSTRAINT [FK_Conversation_Employee]
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFeedback_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[CustomerFeedback] CHECK CONSTRAINT [FK_CustomerFeedback_Employee]
GO
ALTER TABLE [dbo].[EmployeeArea]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeArea_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeArea] CHECK CONSTRAINT [FK_EmployeeArea_Employee]
GO
ALTER TABLE [dbo].[EmployeeDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocument_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeDocument] CHECK CONSTRAINT [FK_EmployeeDocument_Employee]
GO
ALTER TABLE [dbo].[EmployeeIdentityDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeIdentityDocument_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeIdentityDocument] CHECK CONSTRAINT [FK_EmployeeIdentityDocument_Employee]
GO
ALTER TABLE [dbo].[Salesperson]  WITH CHECK ADD  CONSTRAINT [FK_Salesperson_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[Salesperson] CHECK CONSTRAINT [FK_Salesperson_Employee]
GO
ALTER TABLE [dbo].[ElectronicDocumentInvoice]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentInvoice_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[ElectronicDocumentInvoice] CHECK CONSTRAINT [FK_ElectronicDocumentInvoice_Invoice]
GO
ALTER TABLE [dbo].[InvoiceAccountReceivable]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceAccountReceivable_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceAccountReceivable] CHECK CONSTRAINT [FK_InvoiceAccountReceivable_Invoice]
GO
ALTER TABLE [dbo].[InvoiceContact]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceContact_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceContact] CHECK CONSTRAINT [FK_InvoiceContact_Invoice]
GO
ALTER TABLE [dbo].[InvoiceCustomer]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomer_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceCustomer] CHECK CONSTRAINT [FK_InvoiceCustomer_Invoice]
GO
ALTER TABLE [dbo].[InvoiceDetail]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetail_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceDetail] CHECK CONSTRAINT [FK_InvoiceDetail_Invoice]
GO
ALTER TABLE [dbo].[InvoiceDetailGroup]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetailGroup_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceDetailGroup] CHECK CONSTRAINT [FK_InvoiceDetailGroup_Invoice]
GO
ALTER TABLE [dbo].[InvoiceSalesperson]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceSalesperson_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceSalesperson] CHECK CONSTRAINT [FK_InvoiceSalesperson_Invoice]
GO
ALTER TABLE [dbo].[InvoiceTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceTaxSystemType_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceTaxSystemType] CHECK CONSTRAINT [FK_InvoiceTaxSystemType_Invoice]
GO
ALTER TABLE [dbo].[InvoiceAccountReceivablePayment]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceAccountReceivablePayment_InvoiceAccountReceivable] FOREIGN KEY([InvoiceAccountReceivableId])
REFERENCES [dbo].[InvoiceAccountReceivable] ([InvoiceAccountReceivableId])
GO
ALTER TABLE [dbo].[InvoiceAccountReceivablePayment] CHECK CONSTRAINT [FK_InvoiceAccountReceivablePayment_InvoiceAccountReceivable]
GO
ALTER TABLE [dbo].[InvoiceCustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerAddress_InvoiceCustomer] FOREIGN KEY([InvoiceCustomerId])
REFERENCES [dbo].[InvoiceCustomer] ([InvoiceCustomerId])
GO
ALTER TABLE [dbo].[InvoiceCustomerAddress] CHECK CONSTRAINT [FK_InvoiceCustomerAddress_InvoiceCustomer]
GO
ALTER TABLE [dbo].[InvoiceCustomerDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerDocumentIdentification_InvoiceCustomer] FOREIGN KEY([InvoiceCustomerId])
REFERENCES [dbo].[InvoiceCustomer] ([InvoiceCustomerId])
GO
ALTER TABLE [dbo].[InvoiceCustomerDocumentIdentification] CHECK CONSTRAINT [FK_InvoiceCustomerDocumentIdentification_InvoiceCustomer]
GO
ALTER TABLE [dbo].[InvoiceCustomerEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerEmailAddress_InvoiceCustomer] FOREIGN KEY([InvoiceCustomerId])
REFERENCES [dbo].[InvoiceCustomer] ([InvoiceCustomerId])
GO
ALTER TABLE [dbo].[InvoiceCustomerEmailAddress] CHECK CONSTRAINT [FK_InvoiceCustomerEmailAddress_InvoiceCustomer]
GO
ALTER TABLE [dbo].[InvoiceCustomerPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerPhoneNumber_InvoiceCustomer] FOREIGN KEY([InvoiceCustomerId])
REFERENCES [dbo].[InvoiceCustomer] ([InvoiceCustomerId])
GO
ALTER TABLE [dbo].[InvoiceCustomerPhoneNumber] CHECK CONSTRAINT [FK_InvoiceCustomerPhoneNumber_InvoiceCustomer]
GO
ALTER TABLE [dbo].[InvoiceDetailJunction]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetailJunction_InvoiceDetail] FOREIGN KEY([InvoiceDetailId])
REFERENCES [dbo].[InvoiceDetail] ([InvoiceDetailId])
GO
ALTER TABLE [dbo].[InvoiceDetailJunction] CHECK CONSTRAINT [FK_InvoiceDetailJunction_InvoiceDetail]
GO
ALTER TABLE [dbo].[InvoiceDetailTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetailTaxSystemType_InvoiceDetail] FOREIGN KEY([InvoiceDetailId])
REFERENCES [dbo].[InvoiceDetail] ([InvoiceDetailId])
GO
ALTER TABLE [dbo].[InvoiceDetailTaxSystemType] CHECK CONSTRAINT [FK_InvoiceDetailTaxSystemType_InvoiceDetail]
GO
ALTER TABLE [dbo].[InvoiceDetail]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetail_InvoiceDetailGroup] FOREIGN KEY([InvoiceDetailGroupId])
REFERENCES [dbo].[InvoiceDetailGroup] ([InvoiceDetailGroupId])
GO
ALTER TABLE [dbo].[InvoiceDetail] CHECK CONSTRAINT [FK_InvoiceDetail_InvoiceDetailGroup]
GO
ALTER TABLE [dbo].[InvoiceSystemTypeInvoiceJournalSystemType]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceSystemTypeInvoiceJournalSystemType_InvoiceJournalSystemType] FOREIGN KEY([InvoiceJournalSystemTypeId])
REFERENCES [dbo].[InvoiceJournalSystemType] ([InvoiceJournalSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceSystemTypeInvoiceJournalSystemType] CHECK CONSTRAINT [FK_InvoiceSystemTypeInvoiceJournalSystemType_InvoiceJournalSystemType]
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentTransmissionSystemType_InvoiceSystemType] FOREIGN KEY([InvoiceSystemTypeId])
REFERENCES [dbo].[InvoiceSystemType] ([InvoiceSystemTypeId])
GO
ALTER TABLE [dbo].[ElectronicDocumentTransmissionSystemType] CHECK CONSTRAINT [FK_ElectronicDocumentTransmissionSystemType_InvoiceSystemType]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_InvoiceSystemType] FOREIGN KEY([InvoiceSystemTypeId])
REFERENCES [dbo].[InvoiceSystemType] ([InvoiceSystemTypeId])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_InvoiceSystemType]
GO
ALTER TABLE [dbo].[InvoiceSystemTypeInvoiceJournalSystemType]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceSystemTypeInvoiceJournalSystemType_InvoiceSystemType] FOREIGN KEY([InvoiceSystemTypeId])
REFERENCES [dbo].[InvoiceSystemType] ([InvoiceSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceSystemTypeInvoiceJournalSystemType] CHECK CONSTRAINT [FK_InvoiceSystemTypeInvoiceJournalSystemType_InvoiceSystemType]
GO
ALTER TABLE [dbo].[InvoiceDetail]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetail_MeasurementUnitSystemType] FOREIGN KEY([MeasurementUnitSystemTypeId])
REFERENCES [dbo].[MeasurementUnitSystemType] ([MeasurementUnitSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceDetail] CHECK CONSTRAINT [FK_InvoiceDetail_MeasurementUnitSystemType]
GO
ALTER TABLE [dbo].[ProductMeasurementUnitSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ProductMeasurementUnitSystemType_MeasurementUnitSystemType] FOREIGN KEY([MeasurementUnitSystemTypeId])
REFERENCES [dbo].[MeasurementUnitSystemType] ([MeasurementUnitSystemTypeId])
GO
ALTER TABLE [dbo].[ProductMeasurementUnitSystemType] CHECK CONSTRAINT [FK_ProductMeasurementUnitSystemType_MeasurementUnitSystemType]
GO
ALTER TABLE [dbo].[PurchaseDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetail_MeasurementUnitSystemType] FOREIGN KEY([MeasurementUnitSystemTypeId])
REFERENCES [dbo].[MeasurementUnitSystemType] ([MeasurementUnitSystemTypeId])
GO
ALTER TABLE [dbo].[PurchaseDetail] CHECK CONSTRAINT [FK_PurchaseDetail_MeasurementUnitSystemType]
GO
ALTER TABLE [dbo].[AttachmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentDetail_Message] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
GO
ALTER TABLE [dbo].[AttachmentDetail] CHECK CONSTRAINT [FK_AttachmentDetail_Message]
GO
ALTER TABLE [dbo].[CustomerDocument]  WITH CHECK ADD  CONSTRAINT [FK_CustomerDocument_Message] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
GO
ALTER TABLE [dbo].[CustomerDocument] CHECK CONSTRAINT [FK_CustomerDocument_Message]
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFeedback_Message] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
GO
ALTER TABLE [dbo].[CustomerFeedback] CHECK CONSTRAINT [FK_CustomerFeedback_Message]
GO
ALTER TABLE [dbo].[MessageDocument]  WITH CHECK ADD  CONSTRAINT [FK_MessageDocument_Message] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
GO
ALTER TABLE [dbo].[MessageDocument] CHECK CONSTRAINT [FK_MessageDocument_Message]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Message] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Message]
GO
ALTER TABLE [dbo].[VoiceNoteDocument]  WITH CHECK ADD  CONSTRAINT [FK_VoiceNoteDocument_Message] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
GO
ALTER TABLE [dbo].[VoiceNoteDocument] CHECK CONSTRAINT [FK_VoiceNoteDocument_Message]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_MessageType] FOREIGN KEY([MessageTypeId])
REFERENCES [dbo].[MessageType] ([MessageTypeId])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_MessageType]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_NotificationType] FOREIGN KEY([NotificationTypeId])
REFERENCES [dbo].[NotificationType] ([NotificationTypeId])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_NotificationType]
GO
ALTER TABLE [dbo].[CustomerPayment]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPayment_PaymentSystemType] FOREIGN KEY([PaymentSystemTypeId])
REFERENCES [dbo].[PaymentSystemType] ([PaymentSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerPayment] CHECK CONSTRAINT [FK_CustomerPayment_PaymentSystemType]
GO
ALTER TABLE [dbo].[SupplierPayment]  WITH CHECK ADD  CONSTRAINT [FK_SupplierPayment_PaymentSystemType] FOREIGN KEY([PaymentSystemTypeId])
REFERENCES [dbo].[PaymentSystemType] ([PaymentSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierPayment] CHECK CONSTRAINT [FK_SupplierPayment_PaymentSystemType]
GO
ALTER TABLE [dbo].[CustomerPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPhoneNumber_PhoneNumber] FOREIGN KEY([PhoneNumberId])
REFERENCES [dbo].[PhoneNumber] ([PhoneNumberId])
GO
ALTER TABLE [dbo].[CustomerPhoneNumber] CHECK CONSTRAINT [FK_CustomerPhoneNumber_PhoneNumber]
GO
ALTER TABLE [dbo].[InvoiceCustomerPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerPhoneNumber_PhoneNumber] FOREIGN KEY([PhoneNumberId])
REFERENCES [dbo].[PhoneNumber] ([PhoneNumberId])
GO
ALTER TABLE [dbo].[InvoiceCustomerPhoneNumber] CHECK CONSTRAINT [FK_InvoiceCustomerPhoneNumber_PhoneNumber]
GO
ALTER TABLE [dbo].[SupplierPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_SupplierPhoneNumber_PhoneNumber] FOREIGN KEY([PhoneNumberId])
REFERENCES [dbo].[PhoneNumber] ([PhoneNumberId])
GO
ALTER TABLE [dbo].[SupplierPhoneNumber] CHECK CONSTRAINT [FK_SupplierPhoneNumber_PhoneNumber]
GO
ALTER TABLE [dbo].[CustomerPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPhoneNumber_PhoneNumberSystemType] FOREIGN KEY([PhoneNumberSystemTypeId])
REFERENCES [dbo].[PhoneNumberSystemType] ([PhoneNumberSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerPhoneNumber] CHECK CONSTRAINT [FK_CustomerPhoneNumber_PhoneNumberSystemType]
GO
ALTER TABLE [dbo].[InvoiceCustomerPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceCustomerPhoneNumber_PhoneNumberSystemType] FOREIGN KEY([PhoneNumberSystemTypeId])
REFERENCES [dbo].[PhoneNumberSystemType] ([PhoneNumberSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceCustomerPhoneNumber] CHECK CONSTRAINT [FK_InvoiceCustomerPhoneNumber_PhoneNumberSystemType]
GO
ALTER TABLE [dbo].[SupplierPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_SupplierPhoneNumber_PhoneNumberSystemType] FOREIGN KEY([PhoneNumberSystemTypeId])
REFERENCES [dbo].[PhoneNumberSystemType] ([PhoneNumberSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierPhoneNumber] CHECK CONSTRAINT [FK_SupplierPhoneNumber_PhoneNumberSystemType]
GO
ALTER TABLE [dbo].[InvoiceDetail]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetail_PriceUserType] FOREIGN KEY([PriceUserTypeId])
REFERENCES [dbo].[PriceUserType] ([PriceUserTypeId])
GO
ALTER TABLE [dbo].[InvoiceDetail] CHECK CONSTRAINT [FK_InvoiceDetail_PriceUserType]
GO
ALTER TABLE [dbo].[ProductPriceUserType]  WITH CHECK ADD  CONSTRAINT [FK_ProductPriceUserType_PriceUserType] FOREIGN KEY([PriceUserTypeId])
REFERENCES [dbo].[PriceUserType] ([PriceUserTypeId])
GO
ALTER TABLE [dbo].[ProductPriceUserType] CHECK CONSTRAINT [FK_ProductPriceUserType_PriceUserType]
GO
ALTER TABLE [dbo].[InvoiceDetail]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[InvoiceDetail] CHECK CONSTRAINT [FK_InvoiceDetail_Product]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_Product]
GO
ALTER TABLE [dbo].[ProductBoundedContext]  WITH CHECK ADD  CONSTRAINT [FK_ProductBoundedContext_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductBoundedContext] CHECK CONSTRAINT [FK_ProductBoundedContext_Product]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategory_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_ProductCategory_Product]
GO
ALTER TABLE [dbo].[ProductMeasurementUnitSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ProductMeasurementUnitSystemType_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductMeasurementUnitSystemType] CHECK CONSTRAINT [FK_ProductMeasurementUnitSystemType_Product]
GO
ALTER TABLE [dbo].[ProductPriceUserType]  WITH CHECK ADD  CONSTRAINT [FK_ProductPriceUserType_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductPriceUserType] CHECK CONSTRAINT [FK_ProductPriceUserType_Product]
GO
ALTER TABLE [dbo].[ProductTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ProductTaxSystemType_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductTaxSystemType] CHECK CONSTRAINT [FK_ProductTaxSystemType_Product]
GO
ALTER TABLE [dbo].[PurchaseDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[PurchaseDetail] CHECK CONSTRAINT [FK_PurchaseDetail_Product]
GO
ALTER TABLE [dbo].[UnansweredConversation]  WITH CHECK ADD  CONSTRAINT [FK_UnansweredConversation_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[UnansweredConversation] CHECK CONSTRAINT [FK_UnansweredConversation_Product]
GO
ALTER TABLE [dbo].[InvoiceDetail]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetail_ProductSystemType] FOREIGN KEY([ProductSystemTypeId])
REFERENCES [dbo].[ProductSystemType] ([ProductSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceDetail] CHECK CONSTRAINT [FK_InvoiceDetail_ProductSystemType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductSystemType] FOREIGN KEY([ProductSystemTypeId])
REFERENCES [dbo].[ProductSystemType] ([ProductSystemTypeId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductSystemType]
GO
ALTER TABLE [dbo].[PurchaseDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetail_ProductSystemType] FOREIGN KEY([ProductSystemTypeId])
REFERENCES [dbo].[ProductSystemType] ([ProductSystemTypeId])
GO
ALTER TABLE [dbo].[PurchaseDetail] CHECK CONSTRAINT [FK_PurchaseDetail_ProductSystemType]
GO
ALTER TABLE [dbo].[ElectronicDocumentPurchase]  WITH CHECK ADD  CONSTRAINT [FK_ElectronicDocumentPurchase_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([PurchaseId])
GO
ALTER TABLE [dbo].[ElectronicDocumentPurchase] CHECK CONSTRAINT [FK_ElectronicDocumentPurchase_Purchase]
GO
ALTER TABLE [dbo].[PurchaseAccountPayable]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseAccountPayable_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([PurchaseId])
GO
ALTER TABLE [dbo].[PurchaseAccountPayable] CHECK CONSTRAINT [FK_PurchaseAccountPayable_Purchase]
GO
ALTER TABLE [dbo].[PurchaseDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetail_Purchase] FOREIGN KEY([PurchaseId])
REFERENCES [dbo].[Purchase] ([PurchaseId])
GO
ALTER TABLE [dbo].[PurchaseDetail] CHECK CONSTRAINT [FK_PurchaseDetail_Purchase]
GO
ALTER TABLE [dbo].[PurchaseAccountPayablePayment]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseAccountPayablePayment_PurchaseAccountPayable] FOREIGN KEY([PurchaseAccountPayableId])
REFERENCES [dbo].[PurchaseAccountPayable] ([PurchaseAccountPayableId])
GO
ALTER TABLE [dbo].[PurchaseAccountPayablePayment] CHECK CONSTRAINT [FK_PurchaseAccountPayablePayment_PurchaseAccountPayable]
GO
ALTER TABLE [dbo].[PurchaseDetailTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetailTaxSystemType_PurchaseDetail] FOREIGN KEY([PurchaseDetailId])
REFERENCES [dbo].[PurchaseDetail] ([PurchaseDetailId])
GO
ALTER TABLE [dbo].[PurchaseDetailTaxSystemType] CHECK CONSTRAINT [FK_PurchaseDetailTaxSystemType_PurchaseDetail]
GO
ALTER TABLE [dbo].[Purchase]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_PurchaseSystemType] FOREIGN KEY([PurchaseSystemTypeId])
REFERENCES [dbo].[PurchaseSystemType] ([PurchaseSystemTypeId])
GO
ALTER TABLE [dbo].[Purchase] CHECK CONSTRAINT [FK_Purchase_PurchaseSystemType]
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFeedback_RatingReasonType] FOREIGN KEY([RatingReasonTypeId])
REFERENCES [dbo].[RatingReasonType] ([RatingReasonTypeId])
GO
ALTER TABLE [dbo].[CustomerFeedback] CHECK CONSTRAINT [FK_CustomerFeedback_RatingReasonType]
GO
ALTER TABLE [dbo].[CustomerSalesperson]  WITH CHECK ADD  CONSTRAINT [FK_CustomerSalesperson_Salesperson] FOREIGN KEY([SalespersonId])
REFERENCES [dbo].[Salesperson] ([SalespersonId])
GO
ALTER TABLE [dbo].[CustomerSalesperson] CHECK CONSTRAINT [FK_CustomerSalesperson_Salesperson]
GO
ALTER TABLE [dbo].[InvoiceSalesperson]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceSalesperson_Salesperson] FOREIGN KEY([SalespersonId])
REFERENCES [dbo].[Salesperson] ([SalespersonId])
GO
ALTER TABLE [dbo].[InvoiceSalesperson] CHECK CONSTRAINT [FK_InvoiceSalesperson_Salesperson]
GO
ALTER TABLE [dbo].[ApplicationSetting]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationSetting_Setting] FOREIGN KEY([SettingId])
REFERENCES [dbo].[Setting] ([SettingId])
GO
ALTER TABLE [dbo].[ApplicationSetting] CHECK CONSTRAINT [FK_ApplicationSetting_Setting]
GO
ALTER TABLE [dbo].[ApplicationUserSetting]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationUserSetting_Setting] FOREIGN KEY([SettingId])
REFERENCES [dbo].[Setting] ([SettingId])
GO
ALTER TABLE [dbo].[ApplicationUserSetting] CHECK CONSTRAINT [FK_ApplicationUserSetting_Setting]
GO
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_SettingGroup] FOREIGN KEY([SettingGroupId])
REFERENCES [dbo].[SettingGroup] ([SettingGroupId])
GO
ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_SettingGroup]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_State]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_State]
GO
ALTER TABLE [dbo].[County]  WITH CHECK ADD  CONSTRAINT [FK_County_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[County] CHECK CONSTRAINT [FK_County_State]
GO
ALTER TABLE [dbo].[StateCountryGovCode]  WITH CHECK ADD  CONSTRAINT [FK_StateCountryGovCode_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[StateCountryGovCode] CHECK CONSTRAINT [FK_StateCountryGovCode_State]
GO
ALTER TABLE [dbo].[Country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Subcontinent] FOREIGN KEY([SubcontinentId])
REFERENCES [dbo].[Subcontinent] ([SubcontinentId])
GO
ALTER TABLE [dbo].[Country] CHECK CONSTRAINT [FK_Country_Subcontinent]
GO
ALTER TABLE [dbo].[SubcontinentLocale]  WITH CHECK ADD  CONSTRAINT [FK_SubcontinentLocale_Subcontinent] FOREIGN KEY([SubcontinentId])
REFERENCES [dbo].[Subcontinent] ([SubcontinentId])
GO
ALTER TABLE [dbo].[SubcontinentLocale] CHECK CONSTRAINT [FK_SubcontinentLocale_Subcontinent]
GO
ALTER TABLE [dbo].[Purchase]  WITH CHECK ADD  CONSTRAINT [FK_Purchase_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[Purchase] CHECK CONSTRAINT [FK_Purchase_Supplier]
GO
ALTER TABLE [dbo].[SupplierAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAddress_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierAddress] CHECK CONSTRAINT [FK_SupplierAddress_Supplier]
GO
ALTER TABLE [dbo].[SupplierContact]  WITH CHECK ADD  CONSTRAINT [FK_SupplierContact_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierContact] CHECK CONSTRAINT [FK_SupplierContact_Supplier]
GO
ALTER TABLE [dbo].[SupplierDocumentIdentification]  WITH CHECK ADD  CONSTRAINT [FK_SupplierDocumentIdentification_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierDocumentIdentification] CHECK CONSTRAINT [FK_SupplierDocumentIdentification_Supplier]
GO
ALTER TABLE [dbo].[SupplierEconomicActivitySystemType]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEconomicActivitySystemType_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierEconomicActivitySystemType] CHECK CONSTRAINT [FK_SupplierEconomicActivitySystemType_Supplier]
GO
ALTER TABLE [dbo].[SupplierEmailAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierEmailAddress_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierEmailAddress] CHECK CONSTRAINT [FK_SupplierEmailAddress_Supplier]
GO
ALTER TABLE [dbo].[SupplierPayment]  WITH CHECK ADD  CONSTRAINT [FK_SupplierPayment_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierPayment] CHECK CONSTRAINT [FK_SupplierPayment_Supplier]
GO
ALTER TABLE [dbo].[SupplierPhoneNumber]  WITH CHECK ADD  CONSTRAINT [FK_SupplierPhoneNumber_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierPhoneNumber] CHECK CONSTRAINT [FK_SupplierPhoneNumber_Supplier]
GO
ALTER TABLE [dbo].[SupplierTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_SupplierTaxSystemType_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierTaxSystemType] CHECK CONSTRAINT [FK_SupplierTaxSystemType_Supplier]
GO
ALTER TABLE [dbo].[PurchaseAccountPayablePayment]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseAccountPayablePayment_SupplierPayment] FOREIGN KEY([SupplierPaymentId])
REFERENCES [dbo].[SupplierPayment] ([SupplierPaymentId])
GO
ALTER TABLE [dbo].[PurchaseAccountPayablePayment] CHECK CONSTRAINT [FK_PurchaseAccountPayablePayment_SupplierPayment]
GO
ALTER TABLE [dbo].[Classification]  WITH CHECK ADD  CONSTRAINT [FK_Classification_TableEntity] FOREIGN KEY([TableEntityId])
REFERENCES [dbo].[TableEntity] ([TableEntityId])
GO
ALTER TABLE [dbo].[Classification] CHECK CONSTRAINT [FK_Classification_TableEntity]
GO
ALTER TABLE [dbo].[InvoiceDetailJunction]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetailJunction_TableEntity] FOREIGN KEY([TableEntityId])
REFERENCES [dbo].[TableEntity] ([TableEntityId])
GO
ALTER TABLE [dbo].[InvoiceDetailJunction] CHECK CONSTRAINT [FK_InvoiceDetailJunction_TableEntity]
GO
ALTER TABLE [dbo].[Localization]  WITH CHECK ADD  CONSTRAINT [FK_Localization_TableEntity] FOREIGN KEY([TableEntityId])
REFERENCES [dbo].[TableEntity] ([TableEntityId])
GO
ALTER TABLE [dbo].[Localization] CHECK CONSTRAINT [FK_Localization_TableEntity]
GO
ALTER TABLE [dbo].[TableProperty]  WITH CHECK ADD  CONSTRAINT [FK_TableProperty_TableEntity] FOREIGN KEY([TableEntityId])
REFERENCES [dbo].[TableEntity] ([TableEntityId])
GO
ALTER TABLE [dbo].[TableProperty] CHECK CONSTRAINT [FK_TableProperty_TableEntity]
GO
ALTER TABLE [dbo].[Localization]  WITH CHECK ADD  CONSTRAINT [FK_Localization_TableProperty] FOREIGN KEY([TablePropertyId])
REFERENCES [dbo].[TableProperty] ([TablePropertyId])
GO
ALTER TABLE [dbo].[Localization] CHECK CONSTRAINT [FK_Localization_TableProperty]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_TaxpayerSystemType] FOREIGN KEY([TaxpayerSystemTypeId])
REFERENCES [dbo].[TaxpayerSystemType] ([TaxpayerSystemTypeId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_TaxpayerSystemType]
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD  CONSTRAINT [FK_Supplier_TaxpayerSystemType] FOREIGN KEY([TaxpayerSystemTypeId])
REFERENCES [dbo].[TaxpayerSystemType] ([TaxpayerSystemTypeId])
GO
ALTER TABLE [dbo].[Supplier] CHECK CONSTRAINT [FK_Supplier_TaxpayerSystemType]
GO
ALTER TABLE [dbo].[CustomerTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO
ALTER TABLE [dbo].[CustomerTaxSystemType] CHECK CONSTRAINT [FK_CustomerTaxSystemType_TaxSystemType]
GO
ALTER TABLE [dbo].[InvoiceDetailTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetailTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceDetailTaxSystemType] CHECK CONSTRAINT [FK_InvoiceDetailTaxSystemType_TaxSystemType]
GO
ALTER TABLE [dbo].[InvoiceTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO
ALTER TABLE [dbo].[InvoiceTaxSystemType] CHECK CONSTRAINT [FK_InvoiceTaxSystemType_TaxSystemType]
GO
ALTER TABLE [dbo].[ProductTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_ProductTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO
ALTER TABLE [dbo].[ProductTaxSystemType] CHECK CONSTRAINT [FK_ProductTaxSystemType_TaxSystemType]
GO
ALTER TABLE [dbo].[PurchaseDetailTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetailTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO
ALTER TABLE [dbo].[PurchaseDetailTaxSystemType] CHECK CONSTRAINT [FK_PurchaseDetailTaxSystemType_TaxSystemType]
GO
ALTER TABLE [dbo].[SupplierTaxSystemType]  WITH CHECK ADD  CONSTRAINT [FK_SupplierTaxSystemType_TaxSystemType] FOREIGN KEY([TaxSystemTypeId])
REFERENCES [dbo].[TaxSystemType] ([TaxSystemTypeId])
GO
ALTER TABLE [dbo].[SupplierTaxSystemType] CHECK CONSTRAINT [FK_SupplierTaxSystemType_TaxSystemType]
GO















CREATE NONCLUSTERED INDEX [IX_Address_ZipCode] ON [dbo].[Address]
(
	[ZipCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerAddress_AddressId] ON [dbo].[CustomerAddress]
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerAddress_AddressId] ON [dbo].[InvoiceCustomerAddress]
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierAddress_AddressId] ON [dbo].[SupplierAddress]
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AddressSystemType_AddressSystemTypeCode] ON [dbo].[AddressSystemType]
(
	[AddressSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AddressSystemType_AddressSystemTypeName] ON [dbo].[AddressSystemType]
(
	[AddressSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerAddress_AddressSystemTypeId] ON [dbo].[CustomerAddress]
(
	[AddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerAddress_AddressSystemTypeId] ON [dbo].[InvoiceCustomerAddress]
(
	[AddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierAddress_AddressSystemTypeId] ON [dbo].[SupplierAddress]
(
	[AddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Aggregate_AggregateName] ON [dbo].[Aggregate]
(
	[AggregateName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TableEntity_AggregateId] ON [dbo].[TableEntity]
(
	[AggregateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationSetting_StringValue] ON [dbo].[ApplicationSetting]
(
	[StringValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationSetting_JsonValue] ON [dbo].[ApplicationSetting]
(
	[JsonValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUser_FirstName] ON [dbo].[ApplicationUser]
(
	[FirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUser_LastName] ON [dbo].[ApplicationUser]
(
	[LastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUser_UserName] ON [dbo].[ApplicationUser]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUserSetting_ApplicationUserId] ON [dbo].[ApplicationUserSetting]
(
	[ApplicationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_ApplicationUserId] ON [dbo].[Customer]
(
	[ApplicationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Employee_ApplicationUserId] ON [dbo].[Employee]
(
	[ApplicationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Salesperson_ApplicationUserId] ON [dbo].[Salesperson]
(
	[ApplicationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUserSetting_StringValue] ON [dbo].[ApplicationUserSetting]
(
	[StringValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUserSetting_JsonValue] ON [dbo].[ApplicationUserSetting]
(
	[JsonValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Bank_BankCode] ON [dbo].[Bank]
(
	[BankCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Bank_BankName] ON [dbo].[Bank]
(
	[BankName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankAccount_BankId] ON [dbo].[BankAccount]
(
	[BankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankAccount_AccountName] ON [dbo].[BankAccount]
(
	[AccountName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankTransaction_BankAccountId] ON [dbo].[BankTransaction]
(
	[BankAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankAccountSystemType_BankAccountSystemTypeCode] ON [dbo].[BankAccountSystemType]
(
	[BankAccountSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankAccountSystemType_BankAccountSystemTypeName] ON [dbo].[BankAccountSystemType]
(
	[BankAccountSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankAccount_BankAccountSystemTypeId] ON [dbo].[BankAccount]
(
	[BankAccountSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankTransaction_AccountName] ON [dbo].[BankTransaction]
(
	[AccountName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankTransactionSystemType_BankTransactionSystemTypeCode] ON [dbo].[BankTransactionSystemType]
(
	[BankTransactionSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankTransactionSystemType_BankTransactionSystemTypeName] ON [dbo].[BankTransactionSystemType]
(
	[BankTransactionSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankTransaction_BankTransactionSystemTypeId] ON [dbo].[BankTransaction]
(
	[BankTransactionSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BoundedContext_BoundedContextCode] ON [dbo].[BoundedContext]
(
	[BoundedContextCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BoundedContext_BoundedContextName] ON [dbo].[BoundedContext]
(
	[BoundedContextName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AddressSystemType_BoundedContextId] ON [dbo].[AddressSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Aggregate_BoundedContextId] ON [dbo].[Aggregate]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Classification_BoundedContextId] ON [dbo].[Classification]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContactSystemType_BoundedContextId] ON [dbo].[ContactSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerUserType_BoundedContextId] ON [dbo].[CustomerUserType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DocumentIdentificationSystemType_BoundedContextId] ON [dbo].[DocumentIdentificationSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EconomicActivitySystemType_BoundedContextId] ON [dbo].[EconomicActivitySystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttributeSystemType_BoundedContextId] ON [dbo].[ElectronicDocumentAttributeSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_BoundedContextId] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EmailAddressSystemType_BoundedContextId] ON [dbo].[EmailAddressSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_MeasurementUnitSystemType_BoundedContextId] ON [dbo].[MeasurementUnitSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PhoneNumberSystemType_BoundedContextId] ON [dbo].[PhoneNumberSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductBoundedContext_BoundedContextId] ON [dbo].[ProductBoundedContext]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_BoundedContextId] ON [dbo].[Setting]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxpayerSystemType_BoundedContextId] ON [dbo].[TaxpayerSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxSystemType_BoundedContextId] ON [dbo].[TaxSystemType]
(
	[BoundedContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Category_CategoryCode] ON [dbo].[Category]
(
	[CategoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Category_CategoryName] ON [dbo].[Category]
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductCategory_CategoryId] ON [dbo].[ProductCategory]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_City_CityName] ON [dbo].[City]
(
	[CityName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Address_CityId] ON [dbo].[Address]
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Classification_ClassificationCode] ON [dbo].[Classification]
(
	[ClassificationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Classification_ClassificationName] ON [dbo].[Classification]
(
	[ClassificationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Category_ClassificationId] ON [dbo].[Category]
(
	[ClassificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Contact_FirstName] ON [dbo].[Contact]
(
	[FirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Contact_LastName] ON [dbo].[Contact]
(
	[LastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerContact_ContactId] ON [dbo].[CustomerContact]
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceContact_ContactId] ON [dbo].[InvoiceContact]
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierContact_ContactId] ON [dbo].[SupplierContact]
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContactSystemType_ContactSystemTypeCode] ON [dbo].[ContactSystemType]
(
	[ContactSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContactSystemType_ContactSystemTypeName] ON [dbo].[ContactSystemType]
(
	[ContactSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerContact_ContactSystemTypeId] ON [dbo].[CustomerContact]
(
	[ContactSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceContact_ContactSystemTypeId] ON [dbo].[InvoiceContact]
(
	[ContactSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierContact_ContactSystemTypeId] ON [dbo].[SupplierContact]
(
	[ContactSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Continent_ContinentName] ON [dbo].[Continent]
(
	[ContinentName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContinentLocale_ContinentId] ON [dbo].[ContinentLocale]
(
	[ContinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_ContinentId] ON [dbo].[Country]
(
	[ContinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Subcontinent_ContinentId] ON [dbo].[Subcontinent]
(
	[ContinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContinentLocale_ContinentLocaleName] ON [dbo].[ContinentLocale]
(
	[ContinentLocaleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContinentLocale_ISOLanguageCode] ON [dbo].[ContinentLocale]
(
	[ISOLanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_CountryName] ON [dbo].[Country]
(
	[CountryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_CountryCodeISO2] ON [dbo].[Country]
(
	[CountryCodeISO2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_CountryCodeISO3] ON [dbo].[Country]
(
	[CountryCodeISO3] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_CountryAreaCode] ON [dbo].[Country]
(
	[CountryAreaCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Address_CountryId] ON [dbo].[Address]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Bank_CountryId] ON [dbo].[Bank]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CountryLocale_CountryId] ON [dbo].[CountryLocale]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DocumentIdentificationSystemType_CountryId] ON [dbo].[DocumentIdentificationSystemType]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EconomicActivitySystemType_CountryId] ON [dbo].[EconomicActivitySystemType]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttributeSystemType_CountryId] ON [dbo].[ElectronicDocumentAttributeSystemType]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_CountryId] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_CountryId] ON [dbo].[Setting]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_State_CountryId] ON [dbo].[State]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StateCountryGovCode_CountryId] ON [dbo].[StateCountryGovCode]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxpayerSystemType_CountryId] ON [dbo].[TaxpayerSystemType]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxSystemType_CountryId] ON [dbo].[TaxSystemType]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CountryLocale_CountryLocaleName] ON [dbo].[CountryLocale]
(
	[CountryLocaleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CountryLocale_ISOLanguageCode] ON [dbo].[CountryLocale]
(
	[ISOLanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_County_CountyCode] ON [dbo].[County]
(
	[CountyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_County_CountyName] ON [dbo].[County]
(
	[CountyName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_County_CountyPostalCode] ON [dbo].[County]
(
	[CountyPostalCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Address_CountyId] ON [dbo].[Address]
(
	[CountyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_District_CountyId] ON [dbo].[District]
(
	[CountyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Currency_CurrencyName] ON [dbo].[Currency]
(
	[CurrencyName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Currency_CurrencyCodeISO] ON [dbo].[Currency]
(
	[CurrencyCodeISO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankAccount_CurrencyId] ON [dbo].[BankAccount]
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BankTransaction_CurrencyId] ON [dbo].[BankTransaction]
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_CurrencyId] ON [dbo].[Country]
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurrencyLocale_CurrencyLocaleName] ON [dbo].[CurrencyLocale]
(
	[CurrencyLocaleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurrencyLocale_ISOLanguageCode] ON [dbo].[CurrencyLocale]
(
	[ISOLanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_CustomerCode] ON [dbo].[Customer]
(
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_CustomerFirstName] ON [dbo].[Customer]
(
	[CustomerFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_CustomerLastName] ON [dbo].[Customer]
(
	[CustomerLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_CustomerCommercialName] ON [dbo].[Customer]
(
	[CustomerCommercialName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerAddress_CustomerId] ON [dbo].[CustomerAddress]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerContact_CustomerId] ON [dbo].[CustomerContact]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerDocumentIdentification_CustomerId] ON [dbo].[CustomerDocumentIdentification]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerEconomicActivitySystemType_CustomerId] ON [dbo].[CustomerEconomicActivitySystemType]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerEmailAddress_CustomerId] ON [dbo].[CustomerEmailAddress]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerPayment_CustomerId] ON [dbo].[CustomerPayment]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerPhoneNumber_CustomerId] ON [dbo].[CustomerPhoneNumber]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerSalesperson_CustomerId] ON [dbo].[CustomerSalesperson]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerTaxSystemType_CustomerId] ON [dbo].[CustomerTaxSystemType]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Invoice_CustomerId] ON [dbo].[Invoice]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomer_CustomerId] ON [dbo].[InvoiceCustomer]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceAccountReceivablePayment_CustomerPaymentId] ON [dbo].[InvoiceAccountReceivablePayment]
(
	[CustomerPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerUserType_CustomerUserTypeCode] ON [dbo].[CustomerUserType]
(
	[CustomerUserTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerUserType_CustomerUserTypeName] ON [dbo].[CustomerUserType]
(
	[CustomerUserTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_CustomerUserTypeId] ON [dbo].[Customer]
(
	[CustomerUserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_District_DistrictCode] ON [dbo].[District]
(
	[DistrictCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_District_DistrictName] ON [dbo].[District]
(
	[DistrictName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Address_DistrictId] ON [dbo].[Address]
(
	[DistrictId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerDocumentIdentification_DocumentIdentificationId] ON [dbo].[CustomerDocumentIdentification]
(
	[DocumentIdentificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerDocumentIdentification_DocumentIdentificationId] ON [dbo].[InvoiceCustomerDocumentIdentification]
(
	[DocumentIdentificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierDocumentIdentification_DocumentIdentificationId] ON [dbo].[SupplierDocumentIdentification]
(
	[DocumentIdentificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DocumentIdentificationSystemType_DocumentIdentificationSystemTypeCode] ON [dbo].[DocumentIdentificationSystemType]
(
	[DocumentIdentificationSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DocumentIdentificationSystemType_DocumentIdentificationSystemTypeName] ON [dbo].[DocumentIdentificationSystemType]
(
	[DocumentIdentificationSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerDocumentIdentification_DocumentIdentificationSystemTypeId] ON [dbo].[CustomerDocumentIdentification]
(
	[DocumentIdentificationSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerDocumentIdentification_DocumentIdentificationSystemTypeId] ON [dbo].[InvoiceCustomerDocumentIdentification]
(
	[DocumentIdentificationSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierDocumentIdentification_DocumentIdentificationSystemTypeId] ON [dbo].[SupplierDocumentIdentification]
(
	[DocumentIdentificationSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EconomicActivitySystemType_EconomicActivitySystemTypeCode] ON [dbo].[EconomicActivitySystemType]
(
	[EconomicActivitySystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EconomicActivitySystemType_EconomicActivitySystemTypeName] ON [dbo].[EconomicActivitySystemType]
(
	[EconomicActivitySystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerEconomicActivitySystemType_EconomicActivitySystemTypeId] ON [dbo].[CustomerEconomicActivitySystemType]
(
	[EconomicActivitySystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierEconomicActivitySystemType_EconomicActivitySystemTypeId] ON [dbo].[SupplierEconomicActivitySystemType]
(
	[EconomicActivitySystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttribute_ElectronicDocumentId] ON [dbo].[ElectronicDocumentAttribute]
(
	[ElectronicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentInvoice_ElectronicDocumentId] ON [dbo].[ElectronicDocumentInvoice]
(
	[ElectronicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentPurchase_ElectronicDocumentId] ON [dbo].[ElectronicDocumentPurchase]
(
	[ElectronicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmission_ElectronicDocumentId] ON [dbo].[ElectronicDocumentTransmission]
(
	[ElectronicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttribute_ElectronicDocumentAttributeSystemTypeStringValue] ON [dbo].[ElectronicDocumentAttribute]
(
	[ElectronicDocumentAttributeSystemTypeStringValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttribute_ElectronicDocumentAttributeSystemTypeJsonValue] ON [dbo].[ElectronicDocumentAttribute]
(
	[ElectronicDocumentAttributeSystemTypeJsonValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttributeSystemType_ElectronicDocumentAttributeSystemTypeCode] ON [dbo].[ElectronicDocumentAttributeSystemType]
(
	[ElectronicDocumentAttributeSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttributeSystemType_ElectronicDocumentAttributeSystemTypeName] ON [dbo].[ElectronicDocumentAttributeSystemType]
(
	[ElectronicDocumentAttributeSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentAttribute_ElectronicDocumentAttributeSystemTypeId] ON [dbo].[ElectronicDocumentAttribute]
(
	[ElectronicDocumentAttributeSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmission_LastTransmissionAttemptErrorMessage] ON [dbo].[ElectronicDocumentTransmission]
(
	[LastTransmissionAttemptErrorMessage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionAttempt_ElectronicDocumentTransmissionId] ON [dbo].[ElectronicDocumentTransmissionAttempt]
(
	[ElectronicDocumentTransmissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionAttempt_ElectronicDocumentTransmissionAttemptErrorMessage] ON [dbo].[ElectronicDocumentTransmissionAttempt]
(
	[ElectronicDocumentTransmissionAttemptErrorMessage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionAttempt_ElectronicDocumentTransmissionAttemptResponseStatusMessage] ON [dbo].[ElectronicDocumentTransmissionAttempt]
(
	[ElectronicDocumentTransmissionAttemptResponseStatusMessage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_ElectronicDocumentTransmissionSystemTypeCode] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[ElectronicDocumentTransmissionSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_ElectronicDocumentTransmissionSystemTypeName] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[ElectronicDocumentTransmissionSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_ElectronicDocumentTransmissionSystemTypeSuccessStatusCodes] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[ElectronicDocumentTransmissionSystemTypeSuccessStatusCodes] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_ElectronicDocumentTransmissionSystemTypeContentType] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[ElectronicDocumentTransmissionSystemTypeContentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmission_ElectronicDocumentTransmissionSystemTypeId] ON [dbo].[ElectronicDocumentTransmission]
(
	[ElectronicDocumentTransmissionSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerEmailAddress_EmailAddressId] ON [dbo].[CustomerEmailAddress]
(
	[EmailAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerEmailAddress_EmailAddressId] ON [dbo].[InvoiceCustomerEmailAddress]
(
	[EmailAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierEmailAddress_EmailAddressId] ON [dbo].[SupplierEmailAddress]
(
	[EmailAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EmailAddressSystemType_EmailAddressSystemTypeCode] ON [dbo].[EmailAddressSystemType]
(
	[EmailAddressSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EmailAddressSystemType_EmailAddressSystemTypeName] ON [dbo].[EmailAddressSystemType]
(
	[EmailAddressSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerEmailAddress_EmailAddressSystemTypeId] ON [dbo].[CustomerEmailAddress]
(
	[EmailAddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerEmailAddress_EmailAddressSystemTypeId] ON [dbo].[InvoiceCustomerEmailAddress]
(
	[EmailAddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierEmailAddress_EmailAddressSystemTypeId] ON [dbo].[SupplierEmailAddress]
(
	[EmailAddressSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Employee_EmployeeCode] ON [dbo].[Employee]
(
	[EmployeeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Employee_EmployeeFirstName] ON [dbo].[Employee]
(
	[EmployeeFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Employee_EmployeeLastName] ON [dbo].[Employee]
(
	[EmployeeLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Salesperson_EmployeeId] ON [dbo].[Salesperson]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentInvoice_InvoiceId] ON [dbo].[ElectronicDocumentInvoice]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceAccountReceivable_InvoiceId] ON [dbo].[InvoiceAccountReceivable]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceContact_InvoiceId] ON [dbo].[InvoiceContact]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomer_InvoiceId] ON [dbo].[InvoiceCustomer]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_InvoiceId] ON [dbo].[InvoiceDetail]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetailGroup_InvoiceId] ON [dbo].[InvoiceDetailGroup]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSalesperson_InvoiceId] ON [dbo].[InvoiceSalesperson]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceTaxSystemType_InvoiceId] ON [dbo].[InvoiceTaxSystemType]
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceAccountReceivablePayment_InvoiceAccountReceivableId] ON [dbo].[InvoiceAccountReceivablePayment]
(
	[InvoiceAccountReceivableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomer_InvoiceCustomerFirstName] ON [dbo].[InvoiceCustomer]
(
	[InvoiceCustomerFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomer_InvoiceCustomerLastName] ON [dbo].[InvoiceCustomer]
(
	[InvoiceCustomerLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomer_InvoiceCustomerCommercialName] ON [dbo].[InvoiceCustomer]
(
	[InvoiceCustomerCommercialName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerAddress_InvoiceCustomerId] ON [dbo].[InvoiceCustomerAddress]
(
	[InvoiceCustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerDocumentIdentification_InvoiceCustomerId] ON [dbo].[InvoiceCustomerDocumentIdentification]
(
	[InvoiceCustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerEmailAddress_InvoiceCustomerId] ON [dbo].[InvoiceCustomerEmailAddress]
(
	[InvoiceCustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerPhoneNumber_InvoiceCustomerId] ON [dbo].[InvoiceCustomerPhoneNumber]
(
	[InvoiceCustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_InvoiceDetailMeasurementUnitSystemTypeName] ON [dbo].[InvoiceDetail]
(
	[InvoiceDetailMeasurementUnitSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetailJunction_InvoiceDetailId] ON [dbo].[InvoiceDetailJunction]
(
	[InvoiceDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetailTaxSystemType_InvoiceDetailId] ON [dbo].[InvoiceDetailTaxSystemType]
(
	[InvoiceDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetailGroup_InvoiceDetailGroupCode] ON [dbo].[InvoiceDetailGroup]
(
	[InvoiceDetailGroupCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_InvoiceDetailGroupId] ON [dbo].[InvoiceDetail]
(
	[InvoiceDetailGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceJournalSystemType_InvoiceJournalSystemTypeCode] ON [dbo].[InvoiceJournalSystemType]
(
	[InvoiceJournalSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceJournalSystemType_InvoiceJournalSystemTypeName] ON [dbo].[InvoiceJournalSystemType]
(
	[InvoiceJournalSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSystemTypeInvoiceJournalSystemType_InvoiceJournalSystemTypeId] ON [dbo].[InvoiceSystemTypeInvoiceJournalSystemType]
(
	[InvoiceJournalSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSystemType_InvoiceSystemTypeCode] ON [dbo].[InvoiceSystemType]
(
	[InvoiceSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSystemType_InvoiceSystemTypeName] ON [dbo].[InvoiceSystemType]
(
	[InvoiceSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentTransmissionSystemType_InvoiceSystemTypeId] ON [dbo].[ElectronicDocumentTransmissionSystemType]
(
	[InvoiceSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Invoice_InvoiceSystemTypeId] ON [dbo].[Invoice]
(
	[InvoiceSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSystemTypeInvoiceJournalSystemType_InvoiceSystemTypeId] ON [dbo].[InvoiceSystemTypeInvoiceJournalSystemType]
(
	[InvoiceSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Localization_LocalizationValue] ON [dbo].[Localization]
(
	[LocalizationValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Localization_ISOLanguageCode] ON [dbo].[Localization]
(
	[ISOLanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_MeasurementUnitSystemType_MeasurementUnitSystemTypeCode] ON [dbo].[MeasurementUnitSystemType]
(
	[MeasurementUnitSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_MeasurementUnitSystemType_MeasurementUnitSystemTypeName] ON [dbo].[MeasurementUnitSystemType]
(
	[MeasurementUnitSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_MeasurementUnitSystemTypeId] ON [dbo].[InvoiceDetail]
(
	[MeasurementUnitSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductMeasurementUnitSystemType_MeasurementUnitSystemTypeId] ON [dbo].[ProductMeasurementUnitSystemType]
(
	[MeasurementUnitSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseDetail_MeasurementUnitSystemTypeId] ON [dbo].[PurchaseDetail]
(
	[MeasurementUnitSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PaymentSystemType_PaymentSystemTypeCode] ON [dbo].[PaymentSystemType]
(
	[PaymentSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PaymentSystemType_PaymentSystemTypeName] ON [dbo].[PaymentSystemType]
(
	[PaymentSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerPayment_PaymentSystemTypeId] ON [dbo].[CustomerPayment]
(
	[PaymentSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierPayment_PaymentSystemTypeId] ON [dbo].[SupplierPayment]
(
	[PaymentSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerPhoneNumber_PhoneNumberId] ON [dbo].[CustomerPhoneNumber]
(
	[PhoneNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerPhoneNumber_PhoneNumberId] ON [dbo].[InvoiceCustomerPhoneNumber]
(
	[PhoneNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierPhoneNumber_PhoneNumberId] ON [dbo].[SupplierPhoneNumber]
(
	[PhoneNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PhoneNumberSystemType_PhoneNumberSystemTypeCode] ON [dbo].[PhoneNumberSystemType]
(
	[PhoneNumberSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PhoneNumberSystemType_PhoneNumberSystemTypeName] ON [dbo].[PhoneNumberSystemType]
(
	[PhoneNumberSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerPhoneNumber_PhoneNumberSystemTypeId] ON [dbo].[CustomerPhoneNumber]
(
	[PhoneNumberSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceCustomerPhoneNumber_PhoneNumberSystemTypeId] ON [dbo].[InvoiceCustomerPhoneNumber]
(
	[PhoneNumberSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierPhoneNumber_PhoneNumberSystemTypeId] ON [dbo].[SupplierPhoneNumber]
(
	[PhoneNumberSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PriceUserType_PriceUserTypeCode] ON [dbo].[PriceUserType]
(
	[PriceUserTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PriceUserType_PriceUserTypeName] ON [dbo].[PriceUserType]
(
	[PriceUserTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_PriceUserTypeId] ON [dbo].[InvoiceDetail]
(
	[PriceUserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductPriceUserType_PriceUserTypeId] ON [dbo].[ProductPriceUserType]
(
	[PriceUserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Product_ProductCode] ON [dbo].[Product]
(
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Product_ProductName] ON [dbo].[Product]
(
	[ProductName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_ProductId] ON [dbo].[InvoiceDetail]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductBoundedContext_ProductId] ON [dbo].[ProductBoundedContext]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductCategory_ProductId] ON [dbo].[ProductCategory]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductMeasurementUnitSystemType_ProductId] ON [dbo].[ProductMeasurementUnitSystemType]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductPriceUserType_ProductId] ON [dbo].[ProductPriceUserType]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseDetail_ProductId] ON [dbo].[PurchaseDetail]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductSystemType_ProductSystemTypeCode] ON [dbo].[ProductSystemType]
(
	[ProductSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProductSystemType_ProductSystemTypeName] ON [dbo].[ProductSystemType]
(
	[ProductSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetail_ProductSystemTypeId] ON [dbo].[InvoiceDetail]
(
	[ProductSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Product_ProductSystemTypeId] ON [dbo].[Product]
(
	[ProductSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseDetail_ProductSystemTypeId] ON [dbo].[PurchaseDetail]
(
	[ProductSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ElectronicDocumentPurchase_PurchaseId] ON [dbo].[ElectronicDocumentPurchase]
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseAccountPayable_PurchaseId] ON [dbo].[PurchaseAccountPayable]
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseDetail_PurchaseId] ON [dbo].[PurchaseDetail]
(
	[PurchaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseAccountPayablePayment_PurchaseAccountPayableId] ON [dbo].[PurchaseAccountPayablePayment]
(
	[PurchaseAccountPayableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseDetail_PurchaseDetailMeasurementUnitSystemTypeName] ON [dbo].[PurchaseDetail]
(
	[PurchaseDetailMeasurementUnitSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseSystemType_PurchaseSystemTypeCode] ON [dbo].[PurchaseSystemType]
(
	[PurchaseSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseSystemType_PurchaseSystemTypeName] ON [dbo].[PurchaseSystemType]
(
	[PurchaseSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Purchase_PurchaseSystemTypeId] ON [dbo].[Purchase]
(
	[PurchaseSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Salesperson_SalespersonCode] ON [dbo].[Salesperson]
(
	[SalespersonCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Salesperson_SalespersonFirstName] ON [dbo].[Salesperson]
(
	[SalespersonFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Salesperson_SalespersonLastName] ON [dbo].[Salesperson]
(
	[SalespersonLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerSalesperson_SalespersonId] ON [dbo].[CustomerSalesperson]
(
	[SalespersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSalesperson_SalespersonId] ON [dbo].[InvoiceSalesperson]
(
	[SalespersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_SettingCode] ON [dbo].[Setting]
(
	[SettingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_SettingName] ON [dbo].[Setting]
(
	[SettingName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_SettingValueType] ON [dbo].[Setting]
(
	[SettingValueType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_StringDefaultValue] ON [dbo].[Setting]
(
	[StringDefaultValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_JsonDefaultValue] ON [dbo].[Setting]
(
	[JsonDefaultValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationSetting_SettingId] ON [dbo].[ApplicationSetting]
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUserSetting_SettingId] ON [dbo].[ApplicationUserSetting]
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SettingGroup_SettingGroupName] ON [dbo].[SettingGroup]
(
	[SettingGroupName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Setting_SettingGroupId] ON [dbo].[Setting]
(
	[SettingGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_State_StateCode] ON [dbo].[State]
(
	[StateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_State_StateName] ON [dbo].[State]
(
	[StateName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Address_StateId] ON [dbo].[Address]
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_City_StateId] ON [dbo].[City]
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_County_StateId] ON [dbo].[County]
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StateCountryGovCode_StateId] ON [dbo].[StateCountryGovCode]
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StateCountryGovCode_StateGovCode] ON [dbo].[StateCountryGovCode]
(
	[StateGovCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Subcontinent_SubcontinentName] ON [dbo].[Subcontinent]
(
	[SubcontinentName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Country_SubcontinentId] ON [dbo].[Country]
(
	[SubcontinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SubcontinentLocale_SubcontinentId] ON [dbo].[SubcontinentLocale]
(
	[SubcontinentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SubcontinentLocale_SubcontinentLocaleName] ON [dbo].[SubcontinentLocale]
(
	[SubcontinentLocaleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SubcontinentLocale_ISOLanguageCode] ON [dbo].[SubcontinentLocale]
(
	[ISOLanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Supplier_SupplierCode] ON [dbo].[Supplier]
(
	[SupplierCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Supplier_SupplierFirstName] ON [dbo].[Supplier]
(
	[SupplierFirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Supplier_SupplierLastName] ON [dbo].[Supplier]
(
	[SupplierLastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Supplier_SupplierCommercialName] ON [dbo].[Supplier]
(
	[SupplierCommercialName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Purchase_SupplierId] ON [dbo].[Purchase]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierAddress_SupplierId] ON [dbo].[SupplierAddress]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierContact_SupplierId] ON [dbo].[SupplierContact]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierDocumentIdentification_SupplierId] ON [dbo].[SupplierDocumentIdentification]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierEconomicActivitySystemType_SupplierId] ON [dbo].[SupplierEconomicActivitySystemType]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierEmailAddress_SupplierId] ON [dbo].[SupplierEmailAddress]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierPayment_SupplierId] ON [dbo].[SupplierPayment]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierPhoneNumber_SupplierId] ON [dbo].[SupplierPhoneNumber]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierTaxSystemType_SupplierId] ON [dbo].[SupplierTaxSystemType]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PurchaseAccountPayablePayment_SupplierPaymentId] ON [dbo].[PurchaseAccountPayablePayment]
(
	[SupplierPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TableEntity_TableEntityName] ON [dbo].[TableEntity]
(
	[TableEntityName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Classification_TableEntityId] ON [dbo].[Classification]
(
	[TableEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetailJunction_TableEntityId] ON [dbo].[InvoiceDetailJunction]
(
	[TableEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Localization_TableEntityId] ON [dbo].[Localization]
(
	[TableEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TableProperty_TableEntityId] ON [dbo].[TableProperty]
(
	[TableEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TableProperty_TablePropertyName] ON [dbo].[TableProperty]
(
	[TablePropertyName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Localization_TablePropertyId] ON [dbo].[Localization]
(
	[TablePropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxpayerSystemType_TaxpayerSystemTypeCode] ON [dbo].[TaxpayerSystemType]
(
	[TaxpayerSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxpayerSystemType_TaxpayerSystemTypeName] ON [dbo].[TaxpayerSystemType]
(
	[TaxpayerSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer_TaxpayerSystemTypeId] ON [dbo].[Customer]
(
	[TaxpayerSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Supplier_TaxpayerSystemTypeId] ON [dbo].[Supplier]
(
	[TaxpayerSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxSystemType_TaxSystemTypeCode] ON [dbo].[TaxSystemType]
(
	[TaxSystemTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TaxSystemType_TaxSystemTypeName] ON [dbo].[TaxSystemType]
(
	[TaxSystemTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomerTaxSystemType_TaxSystemTypeId] ON [dbo].[CustomerTaxSystemType]
(
	[TaxSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceDetailTaxSystemType_TaxSystemTypeId] ON [dbo].[InvoiceDetailTaxSystemType]
(
	[TaxSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InvoiceTaxSystemType_TaxSystemTypeId] ON [dbo].[InvoiceTaxSystemType]
(
	[TaxSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SupplierTaxSystemType_TaxSystemTypeId] ON [dbo].[SupplierTaxSystemType]
(
	[TaxSystemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO













USE [master]
GO
ALTER DATABASE [PCIShield_Core_Db] SET  READ_WRITE 
GO
