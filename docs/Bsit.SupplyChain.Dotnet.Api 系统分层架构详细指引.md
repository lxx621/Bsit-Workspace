# Bsit.SupplyChain.Api 系统分层架构详细指引

---

## 一、项目概览

| 维度 | 说明 |
|------|------|
| **公司业务** | 物流管理、供应链管理、仓储管理一站式软件解决方案 |
| **系统名称** | Bsit.SupplyChain.Api（后端 WebAPI） |
| **技术栈** | .NET 10 WebAPI / Autofac / SqlSugar / AutoMapper / FluentValidation / MediatR / NLog / Vue3 / MSSQL |
| **架构风格** | 前后端分离 + 领域驱动设计(DDD) 四层架构 |
| **核心特性** | Swagger 接口文档、JWT 认证（Access Token + Refresh Token）、请求审计日志、NLog 系统日志（文件+MSSQL）、统一返回格式、AutoMapper 对象映射、FluentValidation 请求验证、领域事件（MediatR）、CORS 跨域、软删除与审计字段、单元测试与集成测试、SqlSugar 工具链 |

> **说明**：本文档聚焦于后端 WebAPI 项目的分层架构设计，是 `Bsit.SupplyChain` 系统中后端部分的详细指引。示例中出现的 `Order`（订单）、`Warehouse`（仓储）、`Transport`（运输）、`Account`（账户）等业务模块**仅为演示分层组织方式而列举的典型示例**，实际项目开发中需根据真实业务需求进行裁剪、扩展或重命名。

---

## 二、整体分层架构

系统严格遵循 **依赖倒置** 与 **关注点分离** 原则，将后端代码组织为五个核心项目、两个测试项目、一个工具项目，依赖关系由外向内流动。

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

### 分层职责表

| 层（项目） | 职责 | 允许依赖 |
|------------|------|----------|
| **Api** (表示层) | 接收HTTP请求，路由分发，认证鉴权，Swagger配置，审计中间件，全局异常处理，集成 NLog | App, Infra, Common |
| **Application** (应用层) | 业务用例流程编排，DTO转换，调用仓储接口，领域事件处理，返回统一结果 | Domain, Common |
| **Domain** (领域层) | 定义聚合根、实体、值对象、领域事件、仓储接口、领域服务、业务常量 | Common（可选） |
| **Infrastructure** (基础设施层) | 实现Domain层定义的仓储接口，封装SqlSugar操作，数据库上下文管理，领域事件持久化与分发 | Domain, Common |
| **Common** (公共层) | 通用工具、JWT Helper、统一响应模型、自定义异常、配置选项类等，不依赖任何业务层 | 无 |

> **依赖规则**：上层可依赖下层，下层不可依赖上层；所有依赖均指向领域核心或公共层，确保领域层纯净。

---

## 三、完整解决方案目录结构

```
Bsit.SupplyChain.Dotnet.Api.sln
│
├── src/
│   ├── Bsit.SupplyChain.Api              # WebAPI 主项目（启动）
│   ├── Bsit.SupplyChain.Application              # 应用层
│   ├── Bsit.SupplyChain.Domain                   # 领域层
│   ├── Bsit.SupplyChain.Infrastructure           # 基础设施层
│   └── Bsit.SupplyChain.Common                   # 公共工具层
│
├── tests/
│   ├── Bsit.SupplyChain.UnitTests                # 单元测试
│   └── Bsit.SupplyChain.IntegrationTests         # 集成测试
│
└── tools/
    └── Bsit.SupplyChain.SqlSugarTool             # SqlSugar DbFirst 实体生成工具
```

> **命名说明**：解决方案文件采用 `Bsit.SupplyChain.Dotnet.Api.sln`（含 Dotnet 以区分前端仓库），各 C# 项目统一采用 `Bsit.SupplyChain.{层名}` 前缀命名（如 `Bsit.SupplyChain.Api`、`Bsit.SupplyChain.Domain` 等）。

### 3.1 Bsit.SupplyChain.Api 详细结构

```
Bsit.SupplyChain.Api/
├── Controllers/
│   ├── AuthController.cs                 # 认证接口（登录、刷新Token）
│   ├── OrderController.cs                # 订单业务接口（示例）
│   ├── WarehouseController.cs            # 仓储业务接口（示例）
│   └── TransportController.cs            # 运输业务接口（示例）
├── Middlewares/
│   ├── AuditLogMiddleware.cs             # 审计日志中间件（记录请求/响应）
│   ├── ExceptionMiddleware.cs            # 全局异常处理中间件
│   └── CorrelationIdMiddleware.cs        # 请求追踪ID中间件（可选）
├── Filters/
│   ├── ModelValidationFilter.cs          # 模型校验过滤器
│   └── PermissionFilter.cs              # 权限验证过滤器（基于JWT角色/策略）
├── Extensions/
│   ├── ServiceCollectionExtensions.cs    # IServiceCollection 扩展方法（注册Swagger、认证等）
│   ├── ApplicationBuilderExtensions.cs   # IApplicationBuilder 扩展方法（使用中间件）
│   └── AutofacContainerSetup.cs          # Autofac 容器配置入口
├── Services/
│   └── CurrentUser.cs                   # ICurrentUser 实现（从 HttpContext 提取当前用户）
├── appsettings.json                      # 配置文件（连接字符串、JWT参数、Swagger信息等）
├── appsettings.Development.json
├── nlog.config                           # NLog 配置文件
├── Program.cs                            # 应用启动入口，配置服务容器与管道
└── Bsit.SupplyChain.Api.csproj
```

### 3.2 Bsit.SupplyChain.Application 详细结构

```
Bsit.SupplyChain.Application/
├── Dtos/
│   ├── Account/
│   │   ├── LoginRequestDto.cs            # 登录请求模型
│   │   ├── LoginResultDto.cs             # 登录返回（AccessToken, RefreshToken, 过期时间）
│   │   └── RefreshTokenRequestDto.cs     # 刷新Token请求模型
│   ├── Order/                             # 订单相关DTO（示例）
│   │   ├── OrderCreateDto.cs
│   │   ├── OrderDetailDto.cs
│   │   └── OrderQueryDto.cs
│   ├── Audit/
│   │   └── AuditLogDto.cs               # 审计日志传输对象
│   └── Common/
│       ├── PagedRequestDto.cs            # 分页请求参数基类（PageIndex, PageSize, SortField, SortOrder）
│       └── PagedResultDto.cs             # 分页结果基类（Items, TotalCount, PageIndex, PageSize）
├── Interfaces/
│   ├── IAuthService.cs                   # 认证服务接口
│   ├── IOrderService.cs                  # 订单业务服务接口（示例）
│   ├── IWarehouseService.cs              # 仓储业务服务接口（示例）
│   ├── ITransportService.cs             # 运输业务服务接口（示例）
│   ├── IAuditLogService.cs              # 审计日志业务服务接口（供中间件调用）
│   └── ICurrentUser.cs                  # 当前登录用户信息接口（从 JWT Claims 提取）
├── Services/
│   ├── AuthService.cs                    # 认证服务实现
│   ├── OrderService.cs                   # 订单业务服务实现（示例）
│   ├── WarehouseService.cs               # 仓储业务服务实现（示例）
│   ├── TransportService.cs              # 运输业务服务实现（示例）
│   └── AuditLogService.cs               # 审计日志服务实现（调用仓储保存）
├── Validators/                            # FluentValidation 验证器（按业务模块组织）
│   ├── Account/
│   │   ├── LoginRequestDtoValidator.cs   # 登录请求参数验证
│   │   └── RefreshTokenRequestDtoValidator.cs
│   └── Order/
│       └── OrderCreateDtoValidator.cs    # 订单创建参数验证（示例）
├── Mappings/                              # AutoMapper 映射配置（按业务模块拆分）
│   ├── AccountMappingProfile.cs          # 账户相关映射（User <-> LoginResultDto 等）
│   ├── OrderMappingProfile.cs            # 订单相关映射（Order <-> OrderDetailDto 等）（示例）
│   └── AuditMappingProfile.cs            # 审计日志映射
├── Behaviors/                             # MediatR Pipeline Behaviors（横切关注点）
│   ├── LoggingBehavior.cs                # 请求/响应日志记录行为
│   └── ValidationBehavior.cs             # FluentValidation 自动验证行为
├── EventHandlers/
│   ├── OrderCreatedEventHandler.cs       # 领域事件处理程序（示例）
│   └── OrderShippedEventHandler.cs       # 领域事件处理程序（示例）
└── AutofacModule.cs                      # Autofac 模块：注册本层服务、验证器、事件处理程序
```

### 3.3 Bsit.SupplyChain.Domain 详细结构

```
Bsit.SupplyChain.Domain/
├── Entities/
│   ├── Order/                              # “订单”聚合（示例）
│   │   ├── Order.cs                        # 聚合根
│   │   ├── OrderItem.cs                    # 实体
│   │   ├── ValueObjects/                   # 值对象子目录
│   │   │   ├── Address.cs
│   │   │   ├── OrderStatus.cs
│   │   │   └── Money.cs
│   │   └── OrderConstants.cs               # 聚合内常量
│   │
│   ├── Warehouse/                          # “仓储”聚合（示例）
│   │   ├── Inventory.cs                    # 聚合根
│   │   ├── StockRecord.cs                  # 实体
│   │   ├── ValueObjects/
│   │   │   └── WarehouseLocation.cs
│   │   └── WarehouseConstants.cs
│   │
│   ├── Transport/                          # “运输”聚合（示例）
│   │   ├── Waybill.cs                      # 聚合根
│   │   ├── TransportStop.cs                # 实体
│   │   ├── ValueObjects/
│   │   │   └── GeoCoordinate.cs
│   │   └── TransportConstants.cs
│   │
│   ├── Account/                            # 用户聚合（示例）
│   │   ├── User.cs                         # 聚合根
│   │   ├── ValueObjects/
│   │   │   └── PasswordHash.cs
│   │   └── AccountConstants.cs
│   │
│   └── Audit/                              # 审计日志聚合
│       └── AuditLog.cs
│
├── Events/                                 # 领域事件定义
│   ├── OrderCreatedEvent.cs                # 示例
│   ├── OrderShippedEvent.cs               # 示例
│   ├── InventoryReservedEvent.cs           # 示例
│   └── WaybillCreatedEvent.cs             # 示例
│
├── Enums/                                  # 全局共享的枚举
│   ├── ShipMethod.cs
│   ├── PaymentStatus.cs
│   └── UserRole.cs
│
├── Interfaces/                             # 仓储接口 + 工作单元接口
│   ├── IUnitOfWork.cs                      # 工作单元接口（事务协调、事件分发）
│   ├── IOrderRepository.cs                 # 示例
│   ├── IWarehouseRepository.cs             # 示例
│   ├── ITransportRepository.cs            # 示例
│   ├── IUserRepository.cs
│   ├── IAuditLogRepository.cs
│   └── IRepository.cs                      # 通用仓储接口（泛型 CRUD 基类）
│
├── DomainServices/                         # 领域服务（跨聚合的业务逻辑）
│   └── ShipmentRoutingService.cs           # 示例
│
└── Constants/
    ├── AppConstants.cs                     # 全局业务常量
    └── CacheKeyConstants.cs                # 缓存Key常量
```

### 3.4 Bsit.SupplyChain.Infrastructure 详细结构

```
Bsit.SupplyChain.Infrastructure/
├── DbContext/
│   └── SqlSugarSetup.cs                  # SqlSugarClient 配置（连接字符串、实体映射、全局过滤器）
├── Repositories/
│   ├── OrderRepository.cs                # IOrderRepository 实现（示例）
│   ├── WarehouseRepository.cs            # 示例
│   ├── TransportRepository.cs           # 示例
│   ├── UserRepository.cs                 # IUserRepository 实现
│   ├── AuditLogRepository.cs             # IAuditLogRepository 实现
│   └── BaseRepository.cs                 # 通用仓储基类（可选）
├── UnitOfWork/
│   └── UnitOfWork.cs                     # 工作单元（协调事务与领域事件分发）
├── SeedData/
│   └── DefaultDataSeeder.cs             # 初始化种子数据（管理员账号等）
├── Extensions/
│   └── SqlSugarQueryableExtensions.cs    # SqlSugar 扩展方法（通用排序、分页等）
└── AutofacModule.cs                      # Autofac 模块：注册仓储、SqlSugarClient、UnitOfWork
```

### 3.5 Bsit.SupplyChain.Common 详细结构

```
Bsit.SupplyChain.Common/
├── Helpers/
│   ├── JwtHelper.cs                      # JWT Token 生成、Refresh Token生成、Token验证
│   ├── HashHelper.cs                     # 密码哈希（BCrypt或SHA256）
│   ├── DateTimeHelper.cs                # 时间转换工具
│   └── StringHelper.cs                  # 字符串处理扩展
├── Models/
│   ├── ApiResult.cs                      # 统一返回模型（Success, Message, Data, Code）
│   ├── ApiResultCode.cs                  # 返回码枚举
│   └── PaginationParams.cs               # 分页请求参数基类
├── Configurations/
│   ├── JwtSettings.cs                    # JWT 配置选项类（映射appsettings.json）
│   └── AppSettings.cs                    # 全局应用配置选项类
└── Exceptions/
    ├── AppException.cs                   # 自定义业务异常
    └── ValidationException.cs            # 校验异常
```

### 3.6 测试项目详细结构

```
tests/
├── Bsit.SupplyChain.UnitTests/
│   ├── Application/
│   │   ├── Services/
│   │   │   ├── AuthServiceTests.cs
│   │   │   ├── OrderServiceTests.cs      # 示例
│   │   │   └── AuditLogServiceTests.cs
│   │   └── EventHandlers/
│   │       └── OrderCreatedEventHandlerTests.cs  # 示例
│   ├── Domain/
│   │   ├── OrderTests.cs                # 示例
│   │   └── InventoryTests.cs            # 示例
│   └── Common/
│       ├── JwtHelperTests.cs
│       └── HashHelperTests.cs
│
└── Bsit.SupplyChain.IntegrationTests/
    ├── Repositories/
    │   ├── OrderRepositoryTests.cs      # 示例
    │   └── UserRepositoryTests.cs
    ├── Api/
    │   ├── AuthControllerTests.cs
    │   └── OrderControllerTests.cs      # 示例
    ├── Middlewares/
    │   └── AuditLogMiddlewareTests.cs
    └── TestStartup.cs
```

### 3.7 SqlSugar 工具项目结构

```
tools/
└── Bsit.SupplyChain.SqlSugarTool/
    ├── Program.cs                         # 控制台入口，配置连接字符串，调用生成逻辑
    ├── DbFirstGenerator.cs                # 封装 SqlSugar DbFirst API
    ├── appsettings.json                   # 工具专用配置（数据库连接）
    └── Templates/                         # 自定义实体模板（可选）
        └── EntityTemplate.cs
```

---

## 四、DDD 概念在 Domain 层的组织规范

### 聚合内目录规范

```
Entities/{AggregateName}/
├── {AggregateName}.cs                     # 聚合根，以聚合名称命名
├── {EntityName}.cs                        # 实体，有业务含义的名词
├── ValueObjects/                          # 值对象子目录（统一放置）
│   ├── {ValueObject1}.cs
│   └── {ValueObject2}.cs
└── {AggregateName}Constants.cs            # 聚合常量（也可命名为 {AggregateName}Defaults.cs）
```

### DDD 元素识别与区分规则

| DDD 元素 | 位置 | 识别标志 |
|----------|------|----------|
| **聚合根** | 聚合文件夹下，与聚合同名 | 拥有全局唯一标识；外部只通过它访问聚合内其他对象；包含领域行为方法 |
| **实体** | 聚合文件夹内，独立命名的 `.cs` 文件 | 拥有局部唯一标识（在聚合内唯一）；可变，有生命周期 |
| **值对象** | 聚合文件夹下的 `ValueObjects/` 子目录 | 不可变（推荐使用 `record` 类型）；由值定义相等性；无独立标识 |
| **领域事件** | `Domain/Events/` 目录 | 继承 `INotification`；使用 `record` 类型保证不可变性；命名使用过去式（如 `OrderCreatedEvent`） |
| **聚合常量** | 聚合文件夹内 `{Name}Constants.cs` | 仅该聚合使用的魔法数字或业务约束 |
| **全局枚举** | `Domain/Enums/` 目录 | 跨聚合共享的枚举类型 |
| **仓储接口** | `Domain/Interfaces/` 目录 | 每个聚合一个仓储接口，仅定义数据访问契约 |
| **领域服务** | `Domain/DomainServices/` 目录 | 当业务逻辑跨越多个聚合时使用；无状态 |

> **关于示例模块的说明**：
> 以上列出的 `Order`、`Warehouse`、`Transport`、`Account` 等聚合仅作为**架构填充示例**，目的是展示如何按照 DDD 风格组织不同业务模块的代码。**实际项目必须根据业务需求**（如可能包括报关、结算、客户管理等）来确定具体的聚合名称、实体定义和目录结构，**不可生搬硬套**。

---

## 五、领域事件完整实现机制

### 5.1 事件定义（Domain 层）

```csharp
// Domain/Events/OrderCreatedEvent.cs
namespace Bsit.SupplyChain.Domain.Events;

/// <summary>
/// 订单已创建领域事件（示例）
/// </summary>
public record OrderCreatedEvent(Guid OrderId, string CustomerId, decimal Total, DateTime OccurredOn) 
    : INotification;
```

### 5.2 聚合根基类（Domain 层）

所有聚合根**必须**继承 `BaseEntity`，它统一提供四大能力：审计字段、软删除、乐观并发、领域事件收集。

```csharp
// Domain/Entities/BaseEntity.cs（必选基类）
namespace Bsit.SupplyChain.Domain.Entities;

using MediatR;
using SqlSugar;

/// <summary>
/// 所有聚合根的基类，提供：
/// 1. 通用审计字段（创建人、创建时间、修改人、修改时间）
/// 2. 软删除支持（IsDeleted + DeletedAt）
/// 3. 乐观并发控制（RowVersion）
/// 4. 领域事件收集与分发
/// </summary>
public abstract class BaseEntity
{
    // ─── 审计字段（SqlSugar 自动映射到数据库列） ───
    
    /// <summary>创建时间，Insert 时由 Aop.DataExecuting 自动赋值</summary>
    [SugarColumn(IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>创建人ID，Insert 时由 Service 层赋值</summary>
    [SugarColumn(IsOnlyIgnoreUpdate = true, IsNullable = true)]
    public string? CreatedBy { get; set; }
    
    /// <summary>最后修改时间，Update 时由 Aop.DataExecuting 自动赋值</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>最后修改人ID</summary>
    [SugarColumn(IsNullable = true)]
    public string? UpdatedBy { get; set; }
    
    // ─── 软删除 ───
    
    /// <summary>是否已删除（逻辑删除标记）</summary>
    public bool IsDeleted { get; set; } = false;
    
    /// <summary>删除时间</summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? DeletedAt { get; set; }
    
    // ─── 乐观并发 ───
    
    /// <summary>行版本号，每次 Update 自动 +1，SqlSugar 用此字段做并发检查</summary>
    [SugarColumn(IsEnableUpdateVersionValidation = true)]
    public long RowVersion { get; set; }
    
    // ─── 领域事件 ───
    
    private readonly List<INotification> _domainEvents = new();
    
    /// <summary>当前实体待发布的领域事件集合（不映射到数据库）</summary>
    [SugarColumn(IsIgnore = true)]
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>添加领域事件（仅聚合根内部调用）</summary>
    protected void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
    
    /// <summary>清除所有已收集的领域事件（UnitOfWork 提交后调用）</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

> **使用要点**：
> - `CreatedAt` / `UpdatedAt` 在 `SqlSugarSetup.cs` 中通过 `Aop.DataExecuting` 拦截器自动赋值，无需手动设置。
> - `IsDeleted` 在 `SqlSugarSetup.cs` 中配置全局过滤器 `QueryFilter`，查询时自动排除已删除记录。
> - `RowVersion` 由 SqlSugar 内置的 `IsEnableUpdateVersionValidation` 自动管理并发检查。
> - 领域事件通过 `[SugarColumn(IsIgnore = true)]` 排除数据库映射。

### 5.3 聚合根示例（Domain 层）

```csharp
// Domain/Entities/Order/Order.cs（示例）
namespace Bsit.SupplyChain.Domain.Entities.Order;

[SugarTable("Orders")]
public class Order : BaseEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public Address ShippingAddress { get; private set; }
    public decimal Total { get; private set; }
    
    private Order() { } // ORM 所需

    public static Order Create(string customerId, Address address, List<OrderItem> items)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ShippingAddress = address,
            Items = items,
            Total = items.Sum(i => i.UnitPrice.Amount * i.Quantity)
        };
        
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerId, order.Total, DateTime.UtcNow));
        return order;
    }
}
```

### 5.4 事件处理程序（Application 层）

```csharp
// Application/EventHandlers/OrderCreatedEventHandler.cs（示例）
namespace Bsit.SupplyChain.Application.EventHandlers;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    
    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("订单 {OrderId} 已创建，客户: {CustomerId}，金额: {Total}", 
            notification.OrderId, notification.CustomerId, notification.Total);
        return Task.CompletedTask;
    }
}
```

### 5.5 IUnitOfWork 接口（Domain 层）

SqlSugar **没有** EF Core 那样的变更追踪器（ChangeTracker），因此 UnitOfWork 需要基于 SqlSugar 的 `BeginTran/CommitTran/RollbackTran` 显式事务来实现。领域事件由 Application Service 在调用仓储前/后显式收集，UnitOfWork 负责事务提交后统一分发。

```csharp
// Domain/Interfaces/IUnitOfWork.cs
namespace Bsit.SupplyChain.Domain.Interfaces;

using MediatR;

/// <summary>
/// 工作单元接口（定义在 Domain 层，实现在 Infrastructure 层）
/// 职责：协调数据库事务 + 领域事件分发
/// </summary>
public interface IUnitOfWork
{
    /// <summary>开启事务</summary>
    void BeginTran();
    
    /// <summary>收集实体的领域事件（Service 层在执行仓储操作后调用）</summary>
    void CollectEvents(BaseEntity entity);
    
    /// <summary>提交事务并分发所有已收集的领域事件</summary>
    Task CommitAsync();
    
    /// <summary>回滚事务</summary>
    void Rollback();
}
```

### 5.6 UnitOfWork 实现（Infrastructure 层）

```csharp
// Infrastructure/UnitOfWork/UnitOfWork.cs
namespace Bsit.SupplyChain.Infrastructure.UnitOfWork;

using Bsit.SupplyChain.Domain.Entities;
using Bsit.SupplyChain.Domain.Interfaces;
using MediatR;
using SqlSugar;

/// <summary>
/// 基于 SqlSugar 事务的工作单元实现
/// 用法：Service 中 BeginTran → 执行仓储操作 → CollectEvents → CommitAsync
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ISqlSugarClient _db;
    private readonly IPublisher _publisher;
    private readonly List<INotification> _pendingEvents = new();

    public UnitOfWork(ISqlSugarClient db, IPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    /// <summary>开启数据库事务</summary>
    public void BeginTran() => _db.Ado.BeginTran();

    /// <summary>
    /// 收集实体上的领域事件到待发布列表
    /// 典型调用时机：仓储 Insert/Update 之后、CommitAsync 之前
    /// </summary>
    public void CollectEvents(BaseEntity entity)
    {
        _pendingEvents.AddRange(entity.DomainEvents);
    }

    /// <summary>
    /// 提交事务，成功后逐一发布领域事件
    /// 事件在事务提交后发布，保证数据已持久化
    /// </summary>
    public async Task CommitAsync()
    {
        try
        {
            _db.Ado.CommitTran();
            
            // 事务成功后，发布所有领域事件
            foreach (var domainEvent in _pendingEvents)
            {
                await _publisher.Publish(domainEvent);
            }
        }
        finally
        {
            _pendingEvents.Clear();
        }
    }

    /// <summary>回滚事务并清空事件</summary>
    public void Rollback()
    {
        _db.Ado.RollbackTran();
        _pendingEvents.Clear();
    }
}
```

> **Application Service 中的典型用法**：
> ```csharp
> public async Task<ApiResult> CreateOrderAsync(OrderCreateDto dto)
> {
>     _unitOfWork.BeginTran();
>     try
>     {
>         var order = Order.Create(dto.CustomerId, ...);
>         await _orderRepository.AddAsync(order);
>         _unitOfWork.CollectEvents(order);         // 收集领域事件
>         await _unitOfWork.CommitAsync();           // 提交事务 + 发布事件
>         return ApiResult.Ok(_mapper.Map<OrderDetailDto>(order));
>     }
>     catch
>     {
>         _unitOfWork.Rollback();
>         throw;
>     }
> }
> ```

### 5.7 注册 MediatR（Program.cs）

仅扫描 Application 程序集（所有 Handler 在此层），Domain 层不含 Handler 无需扫描。

```csharp
// Program.cs 中注册 MediatR
builder.Services.AddMediatR(cfg =>
{
    // 仅扫描 Application 程序集（EventHandler 全部在此层定义）
    cfg.RegisterServicesFromAssembly(
        typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
});
```

### 5.8 SqlSugarSetup 配置（Infrastructure 层）

`SqlSugarSetup.cs` 负责创建 SqlSugar 客户端实例，并配置**审计字段自动赋值**和**软删除全局过滤器**。

```csharp
// Infrastructure/DbContext/SqlSugarSetup.cs
namespace Bsit.SupplyChain.Infrastructure.DbContext;

using SqlSugar;
using Bsit.SupplyChain.Domain.Entities;

/// <summary>
/// SqlSugar 客户端工厂
/// 配置：连接字符串、审计拦截器、软删除全局过滤器
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// 创建 SqlSugarClient 实例（由 Autofac 调用，每个请求一个）
    /// </summary>
    public static ISqlSugarClient CreateClient()
    {
        var db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = "Server=...;Database=...;...",  // 实际从 IConfiguration 获取
            DbType = DbType.SqlServer,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });

        // ─── 审计字段自动赋值拦截器 ───
        db.Aop.DataExecuting = (oldValue, entityInfo) =>
        {
            // Insert 时自动设置 CreatedAt
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                if (entityInfo.PropertyName == nameof(BaseEntity.CreatedAt))
                    entityInfo.SetValue(DateTime.Now);
            }
            // Update 时自动设置 UpdatedAt
            if (entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                if (entityInfo.PropertyName == nameof(BaseEntity.UpdatedAt))
                    entityInfo.SetValue(DateTime.Now);
            }
        };

        // ─── 软删除全局过滤器 ───
        // 所有继承 BaseEntity 的表查询时自动追加 WHERE IsDeleted = 0
        db.QueryFilter.AddTableFilter<BaseEntity>(it => it.IsDeleted == false);

        // ─── SQL 执行日志（开发环境可开启） ───
        db.Aop.OnLogExecuting = (sql, pars) =>
        {
            // 可对接 ILogger 输出，生产环境建议关闭
            Console.WriteLine($"[SqlSugar] {sql}");
        };

        return db;
    }
}
```

> **要点**：
> - `CreatedBy` / `UpdatedBy` 需要在 Service 层通过 `ICurrentUser.UserId` 手动赋值（因为 SqlSugar Aop 层无法直接获取 HttpContext）。
> - 软删除过滤器对所有查询自动生效，如需查询已删除数据可使用 `db.Queryable<T>().ClearFilter()` 临时禁用。
> - `ConnectionString` 实际开发中应从 `IConfiguration` 注入，此处简化展示。

### 5.9 通用仓储接口与基类

#### IRepository（Domain 层）

```csharp
// Domain/Interfaces/IRepository.cs
namespace Bsit.SupplyChain.Domain.Interfaces;

using Bsit.SupplyChain.Domain.Entities;

/// <summary>
/// 泛型仓储接口（所有聚合仓储的基类接口）
/// T 约束为 BaseEntity，保证只有聚合根可被仓储操作
/// </summary>
public interface IRepository<T> where T : BaseEntity, new()
{
    Task<T?> GetByIdAsync(dynamic id);
    Task<List<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> SoftDeleteAsync(dynamic id);
}
```

#### BaseRepository（Infrastructure 层，可选）

```csharp
// Infrastructure/Repositories/BaseRepository.cs
namespace Bsit.SupplyChain.Infrastructure.Repositories;

using SqlSugar;
using Bsit.SupplyChain.Domain.Entities;
using Bsit.SupplyChain.Domain.Interfaces;

/// <summary>
/// 通用仓储基类，封装 SqlSugar 的 CRUD 操作
/// 具体仓储可继承此类并扩展特有查询
/// </summary>
public class BaseRepository<T> : IRepository<T> where T : BaseEntity, new()
{
    protected readonly ISqlSugarClient Db;

    public BaseRepository(ISqlSugarClient db) => Db = db;

    public async Task<T?> GetByIdAsync(dynamic id)
        => await Db.Queryable<T>().InSingleAsync(id);

    public async Task<List<T>> GetAllAsync()
        => await Db.Queryable<T>().ToListAsync();

    public async Task<int> AddAsync(T entity)
        => await Db.Insertable(entity).ExecuteCommandAsync();

    public async Task<int> UpdateAsync(T entity)
        => await Db.Updateable(entity)
                    .ExecuteCommandWithOptLockAsync();  // 自动校验 RowVersion

    /// <summary>软删除：设置 IsDeleted = true，不真正删除数据</summary>
    public async Task<int> SoftDeleteAsync(dynamic id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return 0;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.Now;
        return await Db.Updateable(entity)
                       .UpdateColumns(it => new { it.IsDeleted, it.DeletedAt })
                       .ExecuteCommandAsync();
    }
}
```

> **使用方式**：具体仓储继承 `BaseRepository<T>` 并添加聚合特有查询：
> ```csharp
> public class OrderRepository : BaseRepository<Order>, IOrderRepository
> {
>     public OrderRepository(ISqlSugarClient db) : base(db) { }
>     
>     public async Task<List<Order>> GetByCustomerIdAsync(string customerId)
>         => await Db.Queryable<Order>()
>                    .Where(o => o.CustomerId == customerId)
>                    .ToListAsync();
> }
> ```

### 5.10 ICurrentUser 当前用户服务

`CreatedBy` / `UpdatedBy` 需要获取当前登录用户ID，通过 `ICurrentUser` 从 JWT Claims 提取。

```csharp
// Application/Interfaces/ICurrentUser.cs
namespace Bsit.SupplyChain.Application.Interfaces;

/// <summary>
/// 当前登录用户信息（从 JWT Token 的 Claims 中提取）
/// </summary>
public interface ICurrentUser
{
    /// <summary>当前用户ID（未登录时为 null）</summary>
    string? UserId { get; }
    /// <summary>当前用户名</summary>
    string? UserName { get; }
    /// <summary>是否已认证</summary>
    bool IsAuthenticated { get; }
}
```

```csharp
// Api/Services/CurrentUser.cs
namespace Bsit.SupplyChain.Api.Services;

using System.Security.Claims;
using Bsit.SupplyChain.Application.Interfaces;

/// <summary>
/// 从 HttpContext 中提取当前用户信息
/// 注册为 Scoped（InstancePerLifetimeScope）
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    public string? UserId => _accessor.HttpContext?.User
        .FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName => _accessor.HttpContext?.User
        .FindFirst(ClaimTypes.Name)?.Value;

    public bool IsAuthenticated => _accessor.HttpContext?.User
        .Identity?.IsAuthenticated ?? false;
}
```

> **注册方式**（Program.cs）：
> ```csharp
> builder.Services.AddHttpContextAccessor();
> builder.Services.AddScoped<ICurrentUser, CurrentUser>();
> ```
>
> **Service 中使用**：在 Service 构造函数注入 `ICurrentUser`，写入前设置审计字段：
> ```csharp
> entity.CreatedBy = _currentUser.UserId;
> ```

---

## 六、日志系统集成方案

系统日志采用 **NLog** 提供程序，通过 `Microsoft.Extensions.Logging.ILogger<T>` 接口在各层使用。NLog 配置为**文件日志 + MSSQL 数据库日志**双目标，可按环境灵活调整。

### 6.1 NuGet 包（仅 Api 项目引用）

```
NLog.Extensions.Logging
NLog.Database
```

### 6.2 Program.cs 配置

```csharp
using NLog;
using NLog.Extensions.Logging;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddNLog();

    // ... 其他服务注册 (Autofac, SqlSugar, JWT, Swagger)
    var app = builder.Build();
    // ... 中间件管道
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "应用程序启动失败");
    throw;
}
finally
{
    LogManager.Shutdown();
}
```

### 6.3 nlog.config 配置示例

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Warn"
      internalLogFile="logs/nlog-internal.log">

  <variable name="defaultLayout" 
            value="${longdate} | ${level:uppercase=true} | ${logger} | ${aspnet-request-url} | ${message} ${exception:format=tostring}"/>

  <targets>
    <!-- 文件日志：按日期分文件，保留30天 -->
    <target name="fileTarget" 
            xsi:type="File"
            fileName="logs/${shortdate}.log"
            layout="${defaultLayout}"
            maxArchiveDays="30"
            archiveEvery="Day"
            archiveNumbering="Date"
            concurrentWrites="true" />

    <!-- 数据库日志：存入 SystemLogs 表 -->
    <target name="dbTarget" 
            xsi:type="Database"
            connectionString="${configsetting:item=ConnectionStrings.Default}"
            commandText="INSERT INTO SystemLogs (Level, Logger, Message, Exception, Url, UserId, CreatedAt) 
                         VALUES (@Level, @Logger, @Message, @Exception, @Url, @UserId, @CreatedAt)">
      <parameter name="@Level" layout="${level:uppercase=true}" />
      <parameter name="@Logger" layout="${logger}" />
      <parameter name="@Message" layout="${message}" />
      <parameter name="@Exception" layout="${exception:format=tostring}" />
      <parameter name="@Url" layout="${aspnet-request-url}" />
      <parameter name="@UserId" layout="${aspnet-user-identity}" />
      <parameter name="@CreatedAt" layout="${longdate}" />
    </target>
  </targets>

  <rules>
    <logger name="*" minlevel="Info" writeTo="fileTarget" />
    <logger name="*" minlevel="Error" writeTo="dbTarget" />
    <logger name="Bsit.SupplyChain.Application.*" minlevel="Debug" writeTo="fileTarget" />
  </rules>
</nlog>
```

> **数据库表脚本**（MSSQL）：
```sql
CREATE TABLE SystemLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Level NVARCHAR(10),
    Logger NVARCHAR(256),
    Message NVARCHAR(MAX),
    Exception NVARCHAR(MAX),
    Url NVARCHAR(512),
    UserId NVARCHAR(128),
    CreatedAt DATETIME2
);
```

### 6.4 各层使用日志

所有服务、控制器通过构造函数注入 `ILogger<T>`：

```csharp
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
    {
        _logger.LogInformation("开始创建订单，客户ID：{CustomerId}", dto.CustomerId);
        // ... 业务逻辑
        _logger.LogWarning("订单库存不足，已进入等待队列");
        // ...
        _logger.LogError(ex, "订单创建异常");
    }
}
```

---

## 七、异常处理与 API 响应

### 7.1 全局异常中间件

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex) // 自定义业务异常
        {
            _logger.LogWarning(ex, "业务异常：{Message}，路径：{Path}", ex.Message, context.Request.Path);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex) // 未处理异常
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(ex, "系统异常，请求ID：{RequestId}，路径：{Path}", requestId, context.Request.Path);
            await HandleExceptionAsync(context, 500, "服务器内部错误，请联系管理员");
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode >= 400 && statusCode < 600 ? statusCode : 500;

        var result = new ApiResult
        {
            Success = false,
            Code = statusCode,
            Message = message,
            Data = null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}
```

### 7.2 审计日志 vs 系统日志

| 类型 | 目的 | 记录内容 | 存储 | 使用方式 |
|------|------|----------|------|----------|
| **审计日志** (`AuditLog`) | 业务合规、操作追溯 | 哪个用户、什么时间、调了什么接口、参数、结果、耗时 | `AuditLogs` 表，由 `AuditLogMiddleware` + `IAuditLogService` 写入 | 业务人员可查询 |
| **系统日志** (`SystemLog`) | 开发运维排错、系统监控 | 日志级别、Logger 名称、消息、异常堆栈、请求URL | 文件 + `SystemLogs` 表（NLog 直接写入） | 开发/运维人员使用日志平台 |

---

## 八、Key 功能协作流程

### 8.1 用户认证流程（JWT 登录）
1. `AuthController.Login()` 接收 `LoginRequestDto`，调用 `IAuthService.LoginAsync()`
2. `AuthService` 通过 `IUserRepository.GetByUserNameAsync()` 获取用户
3. 验证密码哈希（`HashHelper`），通过后调用 `JwtHelper` 生成 Token 对
4. 将 Refresh Token 持久化（通过 `IUserRepository` 更新用户记录或单独存储）
5. 返回 `ApiResult<LoginResultDto>`

### 8.2 订单创建与领域事件流程（示例）
1. `OrderController.Create()` 接收 `OrderCreateDto`，调用 `IOrderService.CreateAsync()`
2. `OrderService` 调用 `Order.Create()` 静态工厂方法创建聚合根
3. 聚合根内部校验业务规则、添加 `OrderCreatedEvent`
4. `OrderService` 调用 `IOrderRepository.AddAsync(order)`
5. `UnitOfWork` 保存聚合根到数据库，提取并分发领域事件
6. `OrderCreatedEventHandler` 异步执行通知/日志等后续操作

### 8.3 异常与日志协同流程
1. 控制器/服务中**不捕获异常**，让它向上传播
2. `ExceptionMiddleware` 捕获后**先通过 `ILogger` 记录**（NLog 按级别写入文件/数据库）
3. 再构造 `ApiResult`，返回合适的 HTTP 状态码和用户友好信息
4. 审计日志由独立的 `AuditLogMiddleware` 记录，与系统异常互不影响

---

## 九、Autofac 集成方案

### 9.1 Api 层 Program.cs 注册

```csharp
// Program.cs — 替换默认 DI 容器为 Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new Application.AutofacModule());
    containerBuilder.RegisterModule(new Infrastructure.AutofacModule());
});

// 注册 AutoMapper（扫描 Application 层所有 Profile）
builder.Services.AddAutoMapper(typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);

// 注册 FluentValidation（扫描 Application 层所有 Validator）
builder.Services.AddValidatorsFromAssembly(typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
```

### 9.2 Application 层 AutofacModule.cs

```csharp
// Application/AutofacModule.cs
namespace Bsit.SupplyChain.Application;

using Autofac;
using MediatR;

/// <summary>
/// Application 层 Autofac 模块
/// 注册：所有 Service、EventHandler
/// </summary>
public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册所有以 "Service" 结尾的类 → 对应接口，每个请求一个实例
        builder.RegisterAssemblyTypes(ThisAssembly)
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
               
        // 注册所有 MediatR 事件处理程序（INotificationHandler<T>）
        builder.RegisterAssemblyTypes(ThisAssembly)
               .AsClosedTypesOf(typeof(INotificationHandler<>))
               .InstancePerDependency();
    }
}
```

### 9.3 Infrastructure 层 AutofacModule.cs

```csharp
// Infrastructure/AutofacModule.cs
namespace Bsit.SupplyChain.Infrastructure;

using Autofac;
using Bsit.SupplyChain.Domain.Interfaces;

/// <summary>
/// Infrastructure 层 Autofac 模块
/// 注册：SqlSugarClient、所有 Repository、UnitOfWork
/// </summary>
public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // ★ SqlSugarClient 必须注册为 Scoped（每个请求一个实例）
        builder.Register(c => SqlSugarSetup.CreateClient())
               .As<SqlSugar.ISqlSugarClient>()
               .InstancePerLifetimeScope();
        
        builder.RegisterAssemblyTypes(ThisAssembly)
               .Where(t => t.Name.EndsWith("Repository"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        
        builder.RegisterType<UnitOfWork.UnitOfWork>()
               .As<IUnitOfWork>()
               .InstancePerLifetimeScope();
    }
}
```

> **关键说明**：`SqlSugarClient` 不可注册为 `SingleInstance`，必须使用 `InstancePerLifetimeScope`（Scoped），保证每个 HTTP 请求独立使用一个实例。

---

## 十、Swagger 与 JWT 认证集成

### Program.cs 配置

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bsit.SupplyChain.Dotnet API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
```

---

## 十一、AutoMapper 对象映射（必选）

### 11.1 概述

系统中**所有**跨层数据传输必须通过 DTO，**禁止**将 Domain Entity 直接暴露给 Api 层。AutoMapper 负责 Entity 与 DTO 的自动映射。

**映射方向**：
- **Entity → DTO**（查询场景）：从数据库查出 Entity，映射为 DTO 返回前端
- **DTO → Entity**（写入场景）：接收前端 DTO，映射为 Entity 存入数据库

### 11.2 NuGet 包

| 项目 | 包名 |
|------|------|
| **Application** | `AutoMapper` |
| **Api** | `AutoMapper.Extensions.Microsoft.DependencyInjection` |

### 11.3 注册方式（Program.cs）

```csharp
// 自动扫描 Application 层所有继承 Profile 的类
builder.Services.AddAutoMapper(typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
```

### 11.4 映射配置文件（Application/Mappings/）

每个业务模块一个 Profile 文件，放在 `Application/Mappings/` 目录下。

```csharp
// Application/Mappings/OrderMappingProfile.cs（示例）
namespace Bsit.SupplyChain.Application.Mappings;

using AutoMapper;
using Bsit.SupplyChain.Domain.Entities.Order;
using Bsit.SupplyChain.Application.Dtos.Order;

/// <summary>
/// 订单模块映射配置
/// </summary>
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Entity -> DTO（查询场景）
        CreateMap<Order, OrderDetailDto>()
            .ForMember(dest => dest.StatusText,
                       opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<OrderItem, OrderItemDto>();

        // DTO -> Entity（写入场景，推荐用领域工厂方法替代）
        CreateMap<OrderCreateDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
    }
}
```

### 11.5 在 Service 中使用

```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;       // 注入 IMapper
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepo, IUnitOfWork unitOfWork,
        IMapper mapper, ILogger<OrderService> logger)
    {
        _orderRepo = orderRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResult<OrderDetailDto>> GetByIdAsync(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) throw new AppException(404, "订单不存在");
        var dto = _mapper.Map<OrderDetailDto>(order);
        return ApiResult<OrderDetailDto>.Ok(dto);
    }
}
```

### 11.6 使用规范

| 规则 | 说明 |
|------|------|
| **文件位置** | `Application/Mappings/`，按模块拆分 |
| **命名** | `{模块名}MappingProfile.cs` |
| **注入** | 构造函数注入 `IMapper`，禁止 `new Mapper()` |
| **Ignore** | ID、审计字段、领域事件必须 `opt.Ignore()` |
| **写入** | 推荐领域工厂方法而非直接 Map |
| **集合** | `IMapper` 自动支持 `List` 映射 |

---

## 十二、FluentValidation 请求验证（必选）

### 12.1 概述

所有 API 请求的 DTO 参数**必须**通过 FluentValidation 进行验证，验证逻辑集中在 `Application/Validators/` 目录管理。

### 12.2 NuGet 包

| 项目 | 包名 |
|------|------|
| **Application** | `FluentValidation` |
| **Api** | `FluentValidation.DependencyInjectionExtensions` |

### 12.3 注册方式（Program.cs）

```csharp
builder.Services.AddValidatorsFromAssembly(
    typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
```

### 12.4 验证器示例

```csharp
// Application/Validators/Account/LoginRequestDtoValidator.cs
namespace Bsit.SupplyChain.Application.Validators.Account;

using FluentValidation;
using Bsit.SupplyChain.Application.Dtos.Account;

/// <summary>
/// 登录请求参数验证器
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名最多50个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码至少6个字符");
    }
}
```

### 12.5 在 Controller 中使用

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequestDto> _validator;

    public AuthController(IAuthService authService, IValidator<LoginRequestDto> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    [HttpPost("login")]
    public async Task<ApiResult<LoginResultDto>> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
            throw new AppException(400, errors);
        }
        return await _authService.LoginAsync(dto);
    }
}
```

### 12.6 MediatR Pipeline Behavior（统一验证）

```csharp
// Application/Behaviors/ValidationBehavior.cs
namespace Bsit.SupplyChain.Application.Behaviors;

using FluentValidation;
using MediatR;

/// <summary>
/// MediatR 管道行为：自动执行 FluentValidation 验证
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct))))
                .SelectMany(r => r.Errors)
                .Where(f => f != null).ToList();

            if (failures.Count > 0)
                throw new Common.Exceptions.ValidationException(failures);
        }
        return await next();
    }
}
```

### 12.7 使用规范

| 规则 | 说明 |
|------|------|
| **文件位置** | `Application/Validators/{模块名}/` |
| **命名** | `{DtoName}Validator.cs` |
| **一对一** | 每个需验证的 DTO 对应一个 Validator |
| **错误消息** | 必须提供中文 `.WithMessage()` |
| **异步** | 需查库的验证用 `MustAsync()` |

---

## 十三、CORS 跨域配置

前后端分离（Vue3 + .NET WebAPI）必须配置 CORS 策略。

### Program.cs 配置

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                ?? new[] { "http://localhost:5173" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 中间件管道中启用（UseRouting 之后、UseAuthorization 之前）
app.UseCors("AllowVueApp");
```

### appsettings.json

```json
{
  "Cors": {
    "Origins": ["http://localhost:5173", "https://your-production-domain.com"]
  }
}
```

---

## 十四、开发约定与最佳实践

| 类别 | 规则 |
|------|------|
| **接口命名** | 上层仅依赖接口，定义在 `Domain/Interfaces/` 或 `Application/Interfaces/` |
| **注入范围** | SqlSugarClient → Scoped；Repository/Service/UnitOfWork → Scoped；JwtHelper/HashHelper → 单例 |
| **DTO 使用** | 跨层传输一律 DTO + AutoMapper，Entity 禁止返回 Api 层 |
| **请求验证** | 所有请求 DTO 必须配备 FluentValidation 验证器 |
| **对象映射** | 必须使用 AutoMapper，Profile 按模块拆分放 `Application/Mappings/` |
| **异常处理** | 业务层抛 `AppException`，全局 `ExceptionMiddleware` 统一捕获 |
| **配置管理** | `IOptions<T>` 读取配置，配置类放 `Common/Configurations/` |
| **领域纯净性** | Domain 层仅引用 `MediatR.Contracts`，不依赖完整 MediatR |
| **事件一致性** | 领域事件必须在事务提交后分发 |
| **日志使用** | 只用 `ILogger<T>`，不直接依赖 NLog |
| **软删除** | 继承 `BaseEntity` 自动获得 `IsDeleted`，全局过滤器自动排除 |
| **审计字段** | `CreatedAt`/`UpdatedAt` 由 SqlSugar `Aop.DataExecuting` 自动赋值 |
| **并发控制** | `RowVersion` 由 SqlSugar 自动管理 |
| **CORS** | 必须配置，允许源从 `appsettings.json` 读取 |

---

## 十五、总结

本架构以整洁架构与 DDD 分层思想为基础，专为 **Bsit.SupplyChain.Api** 后端 WebAPI 量身定制。核心组件协作如下：

| 组件 | 职责 |
|------|------|
| **Autofac** | 依赖注入，按层模块化注册 |
| **AutoMapper** | Entity ↔ DTO 映射，层间数据隔离 |
| **FluentValidation** | 请求参数验证，集中管理 |
| **MediatR** | 领域事件发布/订阅，解耦业务 |
| **NLog** | 系统日志（文件 + MSSQL） |
| **SqlSugar** | ORM 数据访问 |
| **JWT** | 认证鉴权 |
| **全局异常中间件** | 统一异常响应 |

> **重要提醒**：架构中的具体业务模块仅为展示分层组织方式而设，**实际项目的聚合划分、实体定义、服务接口等必须严格依据真实业务需求进行**，切勿生硬照搬示例结构。
