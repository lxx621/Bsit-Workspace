# Bsit.SupplyChain.Api AI 辅助开发项目目录规划指引

---

**适用范围**：`bsit-supplychain-dotnet-api`（.NET 10 后端 WebAPI，DDD 分层架构）  
**核心原则**：统一根目录骨架 + 渐进式披露（`Agents.md` 入口 + `agents-docs/` 详细规则）+ 临时文件规范（`temp/`）  
**参考文档**：《Bsit.SupplyChain.Api 系统分层架构详细指引》、《AI 辅助开发项目目录规划指引》

---

## 一、项目概览

| 维度 | 说明 |
|------|------|
| **公司业务** | 物流管理、供应链管理、仓储管理一站式软件解决方案 |
| **系统名称** | Bsit.SupplyChain.Api（后端 WebAPI） |
| **技术栈** | .NET 10 WebAPI / Autofac / SqlSugar / AutoMapper / FluentValidation / MediatR / NLog / MSSQL |
| **架构风格** | 前后端分离 + 领域驱动设计(DDD) 四层架构 |
| **前端仓库** | `bsit-supplychain-web`（Vue3 + Vite + Pinia） |
| **开发环境** | Windows，所有文件统一无 BOM 的 UTF-8 编码 |

---

## 二、仓库根目录结构

```
bsit-supplychain-dotnet-api/
├── .windsurf/                   # Windsurf/Cascade 配置（可选，其他 AI 工具同理）
│   └── workflows/               # AI 工作流定义（端到端操作步骤）
│       ├── new-module.md        # 创建完整业务模块（11步）
│       ├── new-api-endpoint.md  # 为已有模块新增 API 接口（7步）
│       └── fix-bug.md           # Bug 修复标准流程（5步）
├── .gitignore                   # Git 忽略规则（务必包含 temp/）
├── Agents.md                    # AI 入口文件（固定文件名，全局规则 + 项目概览）
├── agents-docs/                 # 渐进式披露的详细规则目录（按主题拆分）
│   ├── dotnet-coding-style.md   # .NET 编码风格规范
│   ├── architecture-layers.md   # 分层架构与依赖规则
│   ├── repository-pattern.md    # 仓储模式与 UnitOfWork 规范
│   ├── api-conventions.md       # API 设计与 RESTful 规范
│   ├── sqlsugar-usage.md        # SqlSugar ORM 使用规范
│   ├── error-handling.md        # 异常处理与统一响应规范
│   ├── domain-events.md         # 领域事件机制说明
│   ├── validation-rules.md      # FluentValidation 验证规范
│   ├── automapper-rules.md      # AutoMapper 映射规范
│   ├── logging-guide.md         # NLog 日志系统集成规范
│   ├── auth-jwt.md              # JWT 认证鉴权规范
│   ├── testing.md               # 单元测试与集成测试规范
│   ├── checklist.md             # AI 代码生成自检清单（30+ 检查项）
│   └── templates/               # 完整代码样板（AI 照葫芦画瓢）
│       ├── sample-aggregate.cs          # 聚合根 + 值对象 + 常量样板
│       ├── sample-repository.cs         # 仓储接口 + 实现样板
│       ├── sample-service.cs            # Service 接口 + 实现样板
│       ├── sample-controller.cs         # Controller 样板
│       ├── sample-dto-validator.cs      # DTO + Validator 样板
│       ├── sample-mapping-profile.cs    # AutoMapper Profile 样板
│       └── sample-domain-event.cs       # 领域事件 + Handler 样板
├── docs/                        # 人类与 AI 共享的文档（按类型分类归档）
│   ├── requirements/            # 业务需求文档（订单状态机、仓储规则等）
│   ├── design/                  # 设计文档（领域模型描述、架构图等）
│   ├── specs/                   # 技术规格（OpenAPI 规范、数据库设计文档）
│   ├── api/                     # API 文档（接口说明、错误码定义）
│   ├── database/                # SQL 脚本、迁移脚本、种子数据
│   ├── prompts/                 # AI 提示词模板（含具体示例，可直接填参使用）
│   ├── guides/                  # 手册（环境搭建、Docker 部署、开发指南）
│   └── bak/                     # 文档备份
├── temp/                        # 临时文件目录（AI 专用，已 .gitignore）
└── src/                         # .NET 10 解决方案源代码（DDD 分层）
    ├── Bsit.SupplyChain.Dotnet.Api.sln
    ├── main/                                # 核心分层项目
    │   ├── Bsit.SupplyChain.Api/     # 表示层（WebAPI 主项目）
    │   ├── Bsit.SupplyChain.Application/    # 应用层（业务用例/DTO/服务）
    │   ├── Bsit.SupplyChain.Domain/         # 领域层（聚合根/实体/值对象/领域事件）
    │   ├── Bsit.SupplyChain.Infrastructure/ # 基础设施层（仓储实现/SqlSugar/DbContext）
    │   └── Bsit.SupplyChain.Common/         # 公共工具层（JWT/统一返回/异常/配置）
    ├── tests/                               # 测试项目
    │   ├── Bsit.SupplyChain.UnitTests/      # 单元测试
    │   └── Bsit.SupplyChain.IntegrationTests/  # 集成测试
    └── tools/                               # 工具项目
        └── Bsit.SupplyChain.SqlSugarTool/   # SqlSugar DbFirst 实体生成工具
```

**强制规则**：
- 所有 AI 生成的临时文件（代码片段、草稿、测试脚本等）**必须**放在 `temp/` 下，使用后立即删除。
- `temp/` 已加入 `.gitignore`，禁止提交任何临时文件。
- `Agents.md` 必须清晰引用 `agents-docs/` 中的规则文件。
- `docs/` 目录按类型（需求/设计/API/手册等）分类归档。

---

## 三、分层架构与依赖关系

```
┌─────────────────────────────────────────────────────────┐
│               Bsit.SupplyChain.Api               │
│        (WebAPI 主项目 / 控制器 / 中间件 / Swagger)       │
└──────────┬──────────────────────────────┬───────────────┘
           │ 依赖                         │ 依赖
           ▼                              ▼
┌──────────────────────┐    ┌──────────────────────────────┐
│ Bsit.SupplyChain.    │    │ Bsit.SupplyChain.            │
│ Application          │    │ Infrastructure               │
│ (业务用例/DTO/服务)   │    │ (数据访问/仓储/SqlSugar)     │
└──────────┬───────────┘    └──────────┬───────────────────┘
           │ 依赖                      │ 依赖
           ▼                          ▼
┌─────────────────────────────────────────────────────────┐
│               Bsit.SupplyChain.Domain                   │
│   (聚合根 / 实体 / 值对象 / 领域事件 / 仓储接口)        │
└─────────────────────────┬───────────────────────────────┘
                          │ 依赖
                          ▼
┌─────────────────────────────────────────────────────────┐
│                Bsit.SupplyChain.Common                  │
│     (JWT助手 / 统一返回模型 / 异常定义 / 工具类)        │
└─────────────────────────────────────────────────────────┘
```

| 层（项目） | 职责 | 允许依赖 |
|------------|------|----------|
| **Api** (表示层) | 接收HTTP请求，路由分发，认证鉴权，Swagger配置，审计中间件，全局异常处理 | App, Infra, Common |
| **Application** (应用层) | 业务用例编排，DTO转换，调用仓储接口，领域事件处理 | Domain, Common |
| **Domain** (领域层) | 聚合根、实体、值对象、领域事件、仓储接口、领域服务 | Common（可选） |
| **Infrastructure** (基础设施层) | 实现仓储接口，封装SqlSugar操作，数据库上下文管理 | Domain, Common |
| **Common** (公共层) | 通用工具、JWT Helper、统一响应模型、自定义异常 | 无 |

> **依赖规则**：上层可依赖下层，下层不可依赖上层；所有依赖均指向领域核心或公共层。

---

## 四、各层详细目录结构

### 4.1 Bsit.SupplyChain.Api（表示层）

```
Bsit.SupplyChain.Api/
├── Controllers/                          # API 控制器（按业务模块组织）
├── Middlewares/                           # 中间件（审计日志、全局异常、请求追踪）
├── Filters/                              # 过滤器（模型校验、权限验证）
├── Extensions/                           # 扩展方法（服务注册、管道配置、Autofac 容器）
├── Services/                             # Api 层专有服务（如 CurrentUser）
├── appsettings.json                      # 配置文件
├── appsettings.Development.json
├── nlog.config                           # NLog 配置
├── Program.cs                            # 启动入口
└── Bsit.SupplyChain.Api.csproj
```

### 4.2 Bsit.SupplyChain.Application（应用层）

```
Bsit.SupplyChain.Application/
├── Dtos/                                 # 数据传输对象（按业务模块子目录）
│   ├── Account/
│   ├── Common/                           # 分页请求/结果基类
│   └── {Module}/
├── Interfaces/                           # 应用服务接口
├── Services/                             # 应用服务实现
├── Validators/                           # FluentValidation 验证器（按模块子目录）
├── Mappings/                             # AutoMapper 映射配置（按模块拆分）
├── Behaviors/                            # MediatR Pipeline Behaviors
├── EventHandlers/                        # 领域事件处理程序
└── AutofacModule.cs                      # Autofac 模块注册
```

### 4.3 Bsit.SupplyChain.Domain（领域层）

```
Bsit.SupplyChain.Domain/
├── Entities/                             # 聚合（按聚合名称子目录）
│   └── {AggregateName}/
│       ├── {AggregateName}.cs            # 聚合根
│       ├── {EntityName}.cs               # 实体
│       ├── ValueObjects/                 # 值对象子目录
│       └── {AggregateName}Constants.cs   # 聚合常量
├── Events/                               # 领域事件定义
├── Enums/                                # 全局共享枚举
├── Interfaces/                           # 仓储接口 + 工作单元接口
├── DomainServices/                       # 领域服务（跨聚合业务逻辑）
└── Constants/                            # 全局业务常量
```

### 4.4 Bsit.SupplyChain.Infrastructure（基础设施层）

```
Bsit.SupplyChain.Infrastructure/
├── DbContext/                            # SqlSugar 配置（连接、拦截器、全局过滤器）
├── Repositories/                         # 仓储接口实现
├── UnitOfWork/                           # 工作单元实现
├── SeedData/                             # 种子数据
├── Extensions/                           # SqlSugar 扩展方法
└── AutofacModule.cs                      # Autofac 模块注册
```

### 4.5 Bsit.SupplyChain.Common（公共工具层）

```
Bsit.SupplyChain.Common/
├── Helpers/                              # 工具类（JWT、Hash、DateTime、String）
├── Models/                               # 统一返回模型、分页参数
├── Configurations/                       # 配置选项类（JwtSettings、AppSettings）
└── Exceptions/                           # 自定义异常（AppException、ValidationException）
```

### 4.6 测试项目

```
tests/
├── Bsit.SupplyChain.UnitTests/
│   ├── Application/Services/
│   ├── Application/EventHandlers/
│   ├── Domain/
│   └── Common/
└── Bsit.SupplyChain.IntegrationTests/
    ├── Repositories/
    ├── Api/
    ├── Middlewares/
    └── TestStartup.cs
```

---

## 五、Agents.md 内容规范

`Agents.md` 是 AI 工具读取的入口文件，放置在仓库根目录，提供全局规则和项目概览。以下是推荐模板：

```markdown
# AI 项目指南 - Bsit.SupplyChain.Api

## 1. 通用规则
- 语言：全程中文交互与输出。
- 系统：严格限定 Windows 环境开发。
- 编码：所有文件统一无 BOM 的 UTF-8 格式。
- 读写：大文件必须分段读写以优化资源。
- 目录 `docs`：按类型（需求/设计/API/手册等）分类归档。
- 时间：必须且仅通过 Time MCP 获取，禁用本地系统时间。
- 技术决策：遇技术疑点必须调用 Context7 MCP 查阅官方文档。
- 注释：代码必须含详尽中文注释（含用途、参数、返回值、核心逻辑、异常说明）。
- 当前开发项目文件夹：src/main

## 2. 项目概述
- 技术栈：.NET 10 WebAPI / Autofac / SqlSugar / AutoMapper / FluentValidation / MediatR / NLog / MSSQL
- 前端仓库：`bsit-supplychain-web`（Vue3 + Vite + Pinia）
- 架构模式：领域驱动设计（DDD）+ 依赖倒置 + 四层架构
- 核心特性：Swagger 接口文档、JWT 认证、请求审计日志、NLog 日志、统一返回格式、CORS 跨域

## 3. 目录说明
- `src/main/`：核心 DDD 分层项目（Api / Application / Domain / Infrastructure / Common）
- `src/tests/`：测试项目（UnitTests / IntegrationTests）
- `src/tools/`：工具项目（SqlSugarTool）
- `docs/`：按类型分类（requirements / design / specs / api / database / prompts / guides）
- `agents-docs/`：详细编码与架构规则，AI 按需读取（渐进式披露）
- `temp/`：AI 临时文件，用完即删

## 4. 任务路由（AI 按任务类型读取对应文件）

| 我要做什么 | 必须先读（按顺序） | 参考样板 |
|------------|---------------------|----------|
| 新建聚合根/实体/值对象 | `agents-docs/architecture-layers.md` → `dotnet-coding-style.md` | `agents-docs/templates/sample-aggregate.cs` |
| 新建仓储接口 + 实现 | `agents-docs/repository-pattern.md` → `sqlsugar-usage.md` | `agents-docs/templates/sample-repository.cs` |
| 新建 Application Service | `agents-docs/architecture-layers.md` → `repository-pattern.md` | `agents-docs/templates/sample-service.cs` |
| 新建 DTO + Validator | `agents-docs/validation-rules.md` → `automapper-rules.md` | `agents-docs/templates/sample-dto-validator.cs` |
| 新建 Controller | `agents-docs/api-conventions.md` → `architecture-layers.md` | `agents-docs/templates/sample-controller.cs` |
| 新建 AutoMapper Profile | `agents-docs/automapper-rules.md` | `agents-docs/templates/sample-mapping-profile.cs` |
| 新建领域事件 + Handler | `agents-docs/domain-events.md` | `agents-docs/templates/sample-domain-event.cs` |
| 创建完整业务模块（端到端） | **执行工作流** `.windsurf/workflows/new-module.md` | 所有样板 |
| 排查异常 / 日志问题 | `agents-docs/error-handling.md` → `logging-guide.md` | — |
| 编写 / 修改测试 | `agents-docs/testing.md` | — |

> **规则**：执行任务前，必须先读取对应的规则文件和样板文件，禁止凭记忆生成代码。

## 5. AI 行为规则
- **临时文件**：任何临时生成文件必须放入 `temp/`，任务结束后立即删除。禁止在 `src/`、`docs/`、`agents-docs/` 创建临时文件。
- **代码生成**：遵循 `agents-docs/dotnet-coding-style.md` 和 `agents-docs/architecture-layers.md` 的分层依赖规则。
- **仓储实现**：参考 `agents-docs/repository-pattern.md`，接口定义在 Domain 层，实现在 Infrastructure 层。
- **API 设计**：遵循 `agents-docs/api-conventions.md` 的 RESTful 规范。
- **数据库访问**：使用 SqlSugar（参考 `agents-docs/sqlsugar-usage.md`），禁止在 Domain 层出现 SqlSugar 代码。
- **异常处理**：使用全局异常中间件，自定义业务异常（参考 `agents-docs/error-handling.md`）。
- **请求验证**：所有请求 DTO 必须配备 FluentValidation 验证器（参考 `agents-docs/validation-rules.md`）。
- **对象映射**：必须使用 AutoMapper，禁止将 Entity 直接暴露给 Api 层（参考 `agents-docs/automapper-rules.md`）。
- **日志系统**：只用 `ILogger<T>`，不直接依赖 NLog（参考 `agents-docs/logging-guide.md`）。
- **自检**：代码生成完毕后，必须对照 `agents-docs/checklist.md` 逐项核实。

## 6. 分层依赖规则
- Api → Application, Infrastructure, Common
- Application → Domain, Common
- Infrastructure → Domain, Common
- Domain → Common（可选）
- Common → 无依赖
- **上层可依赖下层，下层不可依赖上层**

## 7. 常用提示词位置
- 生成聚合根：`docs/prompts/aggregate-root.md`
- 生成仓储：`docs/prompts/repository.md`
- 生成 API Controller：`docs/prompts/controller.md`
- 生成 Service：`docs/prompts/service.md`
- 生成 DTO + Validator：`docs/prompts/dto-validator.md`

## 8. 与前端协作
- 每次 API 变更后，更新 `docs/specs/openapi.yaml`。
- 通知前端仓库更新 API 客户端。
- 错误码定义见 `docs/api/error-codes.md`。

## 9. 临时文件管理
- 所有临时生成的文件（代码、文档、脚本、数据）请放入 `temp/` 目录。
- 使用完毕后立即删除。
- 该目录已加入 `.gitignore`，无需担心提交问题。
- 禁止在项目其他任何位置创建临时文件。
```

---

## 六、agents-docs/ 详细规则目录说明

| 文件 | 核心内容 |
|:---|:---|
| `dotnet-coding-style.md` | 命名规范（PascalCase public / camelCase private），async/await 模式，using 排序，中文注释要求（用途、参数、返回值、核心逻辑、异常）。 |
| `architecture-layers.md` | 依赖方向（Api → Application → Domain ← Infrastructure），禁止跨层调用，每层允许的依赖，BaseEntity 基类规范。 |
| `repository-pattern.md` | 仓储接口设计（泛型 IRepository<T>），聚合根加载策略，UnitOfWork 模式（BeginTran → CollectEvents → CommitAsync）。 |
| `api-conventions.md` | URL 命名（名词复数），HTTP 方法对应操作，状态码标准，Swagger 注解规范。 |
| `sqlsugar-usage.md` | SqlSugarClient 初始化配置，审计字段自动赋值（Aop.DataExecuting），软删除全局过滤器，禁止在 Domain 层引用。 |
| `error-handling.md` | 自定义 AppException，全局 ExceptionMiddleware，统一 ApiResult 返回格式，审计日志 vs 系统日志区分。 |
| `domain-events.md` | 领域事件定义（record + INotification），聚合根收集事件，UnitOfWork 事务后分发，MediatR 注册。 |
| `validation-rules.md` | FluentValidation 验证器组织（按模块子目录），命名规范，中文错误消息，MediatR Pipeline Behavior 统一验证。 |
| `automapper-rules.md` | Profile 按模块拆分，Entity→DTO / DTO→Entity 映射方向，ID/审计字段/领域事件必须 Ignore，写入推荐工厂方法。 |
| `logging-guide.md` | NLog 双目标配置（文件+MSSQL），各层使用 ILogger<T>，日志级别规范，数据库表结构。 |
| `auth-jwt.md` | JWT 认证流程，Token 生成/刷新，ICurrentUser 服务，Swagger 集成 Bearer 认证。 |
| `testing.md` | xUnit 测试组织，按层分目录，Application/Domain/Common 单元测试，Repository/Api 集成测试。 |
| `checklist.md` | AI 代码生成自检清单（30+ 检查项），涵盖通用规则、分层架构、Domain/Infrastructure/Application/Api 各层、测试。 |
| `templates/` | 完整代码样板目录（7 个 .cs 文件），以"客户(Customer)"模块为具体示例，AI 照葫芦画瓢生成新模块代码。 |

### 6.1 规则文件通用性说明

`Agents.md` 和 `agents-docs/` 中的规则分为两类：

| 类型 | 说明 | 文件 |
|------|------|------|
| **通用规则**（跨项目） | 与具体业务/技术栈无关，可直接复用到其他项目 | `Agents.md` 第1节（语言/系统/编码/读写/时间/Context7 MCP/注释规则）、`checklist.md` 通用规则部分 |
| **项目专有规则** | 与本项目的技术栈、DDD 架构、组件选型强关联 | `architecture-layers.md`、`repository-pattern.md`、`sqlsugar-usage.md`、`domain-events.md`、`validation-rules.md`、`automapper-rules.md`、`api-conventions.md`、`auth-jwt.md`、`error-handling.md`、`logging-guide.md`、`testing.md`、`dotnet-coding-style.md`、`templates/` |

> **复用建议**：新建其他项目时，通用规则可直接拷贝；项目专有规则需根据新项目的技术栈和架构重新编写或调整。

### 6.2 代码样板目录（agents-docs/templates/）

| 样板文件 | 内容说明 | AI 使用场景 |
|----------|----------|-------------|
| `sample-aggregate.cs` | 聚合根 + 值对象 + 聚合常量 + 枚举（完整 Customer 示例） | 新建聚合根时参考 |
| `sample-repository.cs` | 仓储接口（Domain 层） + 仓储实现（Infrastructure 层） | 新建仓储时参考 |
| `sample-service.cs` | Service 接口 + 实现（含 UnitOfWork + AutoMapper + ICurrentUser） | 新建服务时参考 |
| `sample-controller.cs` | RESTful Controller（含 Swagger 注解 + JWT 认证） | 新建控制器时参考 |
| `sample-dto-validator.cs` | Create/Update/Detail/Query 四种 DTO + 两个 Validator | 新建 DTO 时参考 |
| `sample-mapping-profile.cs` | AutoMapper Profile（含值对象展开和 Ignore 示例） | 新建映射时参考 |
| `sample-domain-event.cs` | 领域事件定义（record + INotification） + EventHandler | 新建领域事件时参考 |

### 6.3 AI 工作流目录（.windsurf/workflows/）

| 工作流文件 | 说明 |
|------------|------|
| `new-module.md` | 端到端创建完整业务模块（11 步：聚合根 → 领域事件 → 仓储接口 → 仓储实现 → DTO → Validator → Mapping → Service → EventHandler → Controller → 自检），含文件清单汇总 |
| `new-api-endpoint.md` | 为已有模块新增 API 接口（7 步） |
| `fix-bug.md` | Bug 修复标准流程（5 步：理解问题 → 定位根因 → 最小化修复 → 验证 → 自检） |

---

## 七、docs/ 文档目录分类

| 子目录 | 存放内容 | 示例文件 |
|:---|:---|:---|
| `requirements/` | 业务需求文档 | `order-state-machine.md`、`warehouse-rules.md` |
| `design/` | 设计文档、架构图 | `domain-model.md`、`system-architecture.md` |
| `specs/` | 技术规格 | `openapi.yaml`、`database-design.md` |
| `api/` | API 文档 | `error-codes.md`、`api-changelog.md` |
| `database/` | SQL 脚本 | `init-tables.sql`、`seed-data.sql` |
| `prompts/` | AI 提示词模板 | `aggregate-root.md`、`controller.md` |
| `guides/` | 开发手册 | `environment-setup.md`、`docker-deploy.md` |
| `bak/` | 文档备份 | 旧版文档存档 |

---

## 八、临时文件管理规范（`temp/` 目录）

1. **任何 AI 生成的临时文件、代码片段、草稿文档、测试脚本、中间数据等，必须存放在 `temp/` 下**。
2. **禁止将临时文件放入 `src/`、`docs/`、`agents-docs/` 等正式目录**。
3. **任务完成后，AI 必须立即删除所创建的临时文件**。
4. 如需保留临时文件用于跨会话，应移入正式目录并记录原因，随后删除 `temp/` 中的原副本。
5. `temp/` 已加入 `.gitignore`，确保不会意外提交。

---

## 九、渐进式披露（Progressive Disclosure）

- **入口文件**：`Agents.md` 提供项目概览、全局规则和指向 `agents-docs/` 的引用。
- **详细规则**：`agents-docs/` 按主题拆分，每个文件专注一个方面。AI 只需在需要时读取相关文件，无需一次性加载全部内容。
- **文档目录**：`docs/` 按类型归档，存储长期存在的业务与技术资料。
- **效果**：降低 AI 上下文窗口压力，提高响应速度和准确性；同时便于人工维护和团队协作。

---

## 十、.gitignore 关键条目

```gitignore
# AI 临时文件
temp/

# IDE
.vs/
.vscode/
*.user
*.suo

# Build
bin/
obj/
publish/

# Logs
logs/

# OS
Thumbs.db
Desktop.ini
```

---

## 十一、实施清单

1. **初始化仓库**：按上述结构创建 Git 仓库，配置 `.gitignore`。
2. **填充 Agents.md**：根据第五节模板，结合实际技术版本和业务需求修改。
3. **编写核心规则文件**：优先完成 `agents-docs/` 中以下三个文件：
   - `architecture-layers.md`（分层架构与依赖规则）
   - `dotnet-coding-style.md`（编码风格规范）
   - `repository-pattern.md`（仓储模式规范）
4. **创建 docs 目录骨架**：按第七节分类创建子目录。
5. **配置 AI 工具**：确保 AI 工具指向项目根目录，可读取 `Agents.md`。
6. **团队培训**：告知所有成员临时文件规范，PR 审查时检查是否有文件误入 `temp/`。

---

## 十二、总结

本指引将《系统分层架构详细指引》中的 DDD 四层架构与《AI 辅助开发项目目录规划指引》中的渐进式披露、临时文件管理等最佳实践相结合，为 Bsit.SupplyChain.Api 项目提供：

- **高 AI 可读性**：`Agents.md` 入口 + `agents-docs/` 按需加载
- **清晰架构边界**：DDD 分层 + 严格依赖方向
- **规范化文档管理**：`docs/` 按类型分类归档
- **安全的临时文件机制**：`temp/` 隔离 + 自动清理
- **便于团队协作**：统一规范 + 渐进式披露降低上手成本
