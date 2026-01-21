# 🛡️ PCIShield - Enterprise PCI Compliance Management

**An open-source, production-grade PCI compliance management platform demonstrating enterprise C#/.NET architecture patterns.**

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![MudBlazor](https://img.shields.io/badge/UI-MudBlazor-7B1FA2)](https://mudblazor.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE.txt)

---

## 🎯 What Is This?

PCIShield is a **full-stack enterprise application** for managing PCI DSS (Payment Card Industry Data Security Standard) compliance for merchants. It showcases real-world patterns used in Fortune 500 applications, including:

- 🏗️ **8-Layer Clean Architecture** (UI → Orchestrator → HTTP → API → Service → Domain → Specification → Infrastructure)
- 🤖 **AI-Powered Assistant** (AG-UI Protocol with OpenAI integration)
- ⚡ **Reactive State Management** (Rx.NET with BehaviorSubject)
- 🔄 **Real-time Collaboration** (SignalR multi-user editing)
- 📊 **Advanced Query Patterns** ( Specifications !)

---

## 🏛️ Architecture Highlights

```
┌─────────────────────────────────────────────────────────────┐
│ Layer 1: MudBlazor UI (830+ line master pages)              │
├─────────────────────────────────────────────────────────────┤
│ Layer 2: Reactive Orchestrator (Rx.NET + FluentValidation)  │
├─────────────────────────────────────────────────────────────┤
│ Layer 3: HTTP Client Service (3,200+ lines, 80+ methods)    │
├─────────────────────────────────────────────────────────────┤
│ Layer 4: FastEndpoints API (Vertical Slice Architecture)    │
├─────────────────────────────────────────────────────────────┤
│ Layer 5: Domain Services (LanguageExt Either<> patterns)    │
├─────────────────────────────────────────────────────────────┤
│ Layer 6: Rich Domain Model (DDD with Aggregate Roots)       │
├─────────────────────────────────────────────────────────────┤
│ Layer 7: Specification Pattern (Graph topology analysis!)   │
├─────────────────────────────────────────────────────────────┤
│ Layer 8: Infrastructure (EF Core + Redis + Elasticsearch)   │
└─────────────────────────────────────────────────────────────┘
```

---

## 🧰 Tech Stack

| Category | Technologies |
|----------|--------------|
| **Frontend** | Blazor WebAssembly, MudBlazor, Rx.NET |
| **Backend** | ASP.NET Core 9, FastEndpoints, MediatR |
| **Database** | SQL Server, EF Core 9, Redis, Elasticsearch |
| **AI Integration** | OpenAI GPT-5, AG-UI Protocol, Microsoft.Extensions.AI |
| **Patterns** | DDD, CQRS, Specification Pattern, Repository Factory |
| **DevOps** | Docker, Hangfire, Quartz.NET |
| **Auth** | JWT, ASP.NET Identity, Role-based policies |

---

## ✨ Key Features

### 🎛️ Merchant Management

- Full CRUD with complex nested aggregates (9 child entities)
- Master-detail UI with tabbed navigation
- Real-time multi-user editing detection via SignalR

### 🤖 AI Copilot - Talk to Your ERP

Users can **converse naturally** with the ERP system using the AI Copilot:

- 💬 Natural language commands: *"Set compliance rank to 85"*, *"Show me all Level 1 merchants"*
- 🛡️ Tool-calling architecture with **human approval** for destructive operations
- ⚡ Server-Sent Events (SSE) streaming for real-time responses
- 🎯 **GAT (Graph-Aware Topology)** spec selection - AI chooses optimal query strategy

#### AI Copilot Components

```
┌─────────────────────────────────────────────────────────────┐
│  MerchantAiCopilotTab.razor     │ Chat UI with streaming   │
├─────────────────────────────────────────────────────────────┤
│  CascadeSpecSelector.razor      │ Visual spec picker       │
├─────────────────────────────────────────────────────────────┤
│  SpecSelectorMudSelect.razor    │ Dropdown with GAT metrics│
├─────────────────────────────────────────────────────────────┤
│  HttpSpecExecutionService.cs    │ Dynamic spec execution   │
├─────────────────────────────────────────────────────────────┤
│  SpecCatalogService.cs          │ 157 specs with metadata  │
└─────────────────────────────────────────────────────────────┘
```

The AI understands the **entity graph topology** and automatically selects the best specification based on:

- Query complexity and depth requirements
- PageRank and centrality scores
- Caching strategies and eager loading priorities

### 📈 Compliance Analytics & Graph Theory Specifications

- Risk scoring algorithms
- Compliance forecasting (Phase 2 roadmap)
- **Graph Topology Analysis** with 157 specification classes in a single 9,744-line file!

#### 🧠 Graph Theory-Powered Specifications

The `MerchantGetListSpec.cs` is a **masterpiece of query optimization** using graph theory concepts:

```csharp
// GRAPH PHYSICS: Identified as Reference Data (High In-Degree)
// Graph Pagination Analysis: Volume=1000, Complexity=0.900
// PageRank=1.9400, OptimalPageSize=25
// Graph GetById Analysis: FanOut=9, Depth=4, HubScore=0.0000
```

**157 Specification Classes** including:

| Spec Family | Examples | Purpose |
|-------------|----------|---------|
| **Core** | `MerchantByIdSpec`, `MerchantSearchSpec` | Basic CRUD operations |
| **Graph Analysis** | `MerchantAdvancedGraphSpecV7`, `MerchantTopologicalSpecV8` | Entity relationship traversal |
| **Deep Intelligence** | `MerchantDeepLinkGraphSpecV9`, `MerchantHolographicSpecV10` | Multi-level eager loading |
| **Security** | `MerchantSecurityReachabilitySpecV14`, `MerchantClusterFortressSpecV16` | Access path analysis |
| **Analytics** | `MerchantProcessMiningSpecV16`, `MerchantCriticalPathSpecV21` | Business intelligence |
| **360° Views** | `MerchantHolistic360SpecV24`, `MerchantIntegritySpecV25` | Complete entity graphs |

Each spec is annotated with **graph metrics**: `PageRank`, `FanIn/FanOut`, `HubScore`, `CacheStrategy`, `EagerLoadPriority`

#### ⚡ Performance Optimization Chain

All 157 specifications implement the **triple optimization chain**:

```csharp
// Specs WITH merchantId parameter (147 specs):
Query.AsNoTracking()
    .AsSplitQuery()
    .EnableCache($"MerchantTopologicalSpecV8-{merchantId}");

// Specs WITHOUT merchantId parameter (7 specs - search, list, etc.):
Query.AsNoTracking()
    .AsSplitQuery()
    .EnableCache("MerchantSearchSpec");
```

### 🔒 Enterprise-Grade Security

- JWT authentication with security stamp validation
- Role-based authorization (Admin, Merchant, QSA)
- Audit logging via Serilog

---

## 🚀 Quick Start

### Prerequisites

- .NET 10 SDK
- SQL Server (or LocalDB)
- Node.js (for Blazor tooling)
- OpenAI API Key

# Start both API and Blazor

dotnet run --project src/PCIShield.Api
dotnet run --project src/PCIShield.BlazorAdmin

```

### Access

- **Blazor Admin**: <https://localhost:7234>
- **API Swagger**: <https://localhost:5001/swagger>

---

## 🗄️ Database Setup

PCIShield uses **two databases**:

| Database | Purpose | Tables |
|----------|---------|--------|
| `GraphErpAuthDb` | ASP.NET Identity (users, roles, JWT) | 7 |
| `GraphErpDb` | Operational data (merchants, assessments, etc.) | 367 |

### Database Scripts (SuperSeed)

Located in `src/PCIShield.Infrastructure/Data/SuperSeed/`:

```bash
# 1. Create schema (drops & recreates GraphErpDb)
sqlcmd -S YOUR_SERVER -d master -U sa -P YOUR_PASSWORD -C -i "MASTER_MERGED_V47.sql"

# 2. Seed base operational data (131 tables with realistic data)
sqlcmd -S YOUR_SERVER -d GraphErpDb -U sa -P YOUR_PASSWORD -C -i "AGGREGATE_DATA_EXPANDED.sql"

# 3. Seed PCI compliance core tables (8 tables)
sqlcmd -S YOUR_SERVER -d GraphErpDb -U sa -P YOUR_PASSWORD -C -i "PCI_CORE_SEED.sql"

# 4. Enrich "hero" merchants with full entity graphs (for UI showcase)
sqlcmd -S YOUR_SERVER -d GraphErpDb -U sa -P YOUR_PASSWORD -C -i "MERCHANT_FULL_GRAPH_SEED.sql"
```

### Hero Merchants for Testing

After running the seeds, these merchants have **rich entity graphs** perfect for UI testing:

| Merchant Name | Level | Assets | Payment Channels | Compensating Controls | Total Items |
|---------------|-------|--------|------------------|----------------------|-------------|
| **Emerentor Direct Company** | 2 | 9 | 6 | 4 | 25 |
| **Cippebar Inc** | 1 | 5 | 4 | 1 | 15 |
| **Lomtinentor International** | 3 | 1 | 3 | 0 | 9 |

### Diagnostic Queries

Located in `src/PCIShield.Api/Documentation/_oscar/`:

- `Find-Aggregates.sql` - Analyze table row counts and FK relationships
- `Find-MerchantGraph.sql` - Analyze merchant entity graph completeness

---

## 📂 Project Structure

```
src/
├── PCIShield.Api/                 # ASP.NET Core API (FastEndpoints)
│   ├── Agents/                    # AI Copilot (AG-UI)
│   ├── Endpoints/                 # Vertical slice endpoints
│   └── Auth/                      # JWT + Identity
├── PCIShield.BlazorAdmin/         # Blazor WebAssembly frontend
│   └── Client/
│       ├── Pages/Merchant/        # MudBlazor master pages
│       └── Shared/Components/     # AI Command Panel
├── PCIShield.Infrastructure/      # EF Core, Redis, Elasticsearch
├── PCIShield.Domain/              # Entities, DTOs, Specifications
└── PCIShield.Client.Services/     # HTTP client services
```

## Setup

### 1) Configure secrets (example)

### - OpenAI: set OPENAI_API_KEY (optional)

### - DB: set SQL connection string / SA password (dev only)

### 2) Run dependencies (recommended)

docker compose up -d

### 3) Run API + Blazor

dotnet run --project src/PCIShield.Api
dotnet run --project src/PCIShield.BlazorAdmin

---

## 🤝 About the Author

I'm a **Senior .NET Software Engineer** with 15+ years of experience building enterprise applications. This project demonstrates my ability to:

- ✅ Design and implement **clean, maintainable architecture**
- ✅ Apply **advanced patterns** (DDD, CQRS, Specifications, Reactive)
- ✅ Integrate **modern AI capabilities** into existing codebases
- ✅ Write **production-ready code** (not tutorials or toy projects)

**Available for contract work** with U.S. companies (remote). I specialize in:

- Blazor / MudBlazor UI development
- ASP.NET Core API design
- Database architecture (SQL Server, PostgreSQL)
- AI/LLM integration

📧 [Contact me on LinkedIn](https://www.linkedin.com/in/oscaragreda/)

---

## 📜 License

MIT License - See [LICENSE.txt](LICENSE.txt)

---

## 🙏 Acknowledgments

- [MudBlazor](https://mudblazor.com/) - Beautiful Blazor component library
- [FastEndpoints](https://fast-endpoints.com/) - Elegant API framework
- [Ardalis Specifications](https://github.com/ardalis/Specification) - Query pattern
- [LanguageExt](https://github.com/louthy/language-ext) - Functional C#
