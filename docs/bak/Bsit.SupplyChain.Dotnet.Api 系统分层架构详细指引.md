# Bsit.SupplyChain.Dotnet.Api 系统分层架构详细指引

---

## 一、项目概览

| 维度 | 说明 |
|------|------|
| **公司业务** | 物流管理、供应链管理、仓储管理一站式软件解决方案 |
| **系统名称** | Bsit.SupplyChain.Dotnet.Api（后端 WebAPI） |
| **技术栈** | .NET 10 WebAPI / Autofac / SqlSugar / Vue3 / MSSQL |
| **架构风格** | 前后端分离 + 领域驱动设计(DDD) 四层架构 |
| **核心特性** | Swagger 接口文档、JWT 认证（Access Token + Refresh Token）、请求审计日志、NLog 系统日志（文件+MSSQL）、统一返回格式、领域事件、单元测试与集成测试、SqlSugar 工具链 |

> **说明**：本文档聚焦于后端 WebAPI 项目的分层架构设计，是 `Bsit.SupplyChain` 系统中后端部分的详细指引。示例中出现的 `Order`（订单）、`Warehouse`（仓储）、`Transport`（运输）、`Account`（账户）等业务模块**仅为演示分层组织方式而列举的典型示例**，实际项目开发中需根据真实业务需求进行裁剪、扩展或重命名。

---

## 二、整体分层架构

系统严格遵循 **依赖倒置** 与 **关注点分离** 原则，将后端代码组织为五个核心项目、两个测试项目、一个工具项目，依赖关系由外向内流动。

```
┌─────────────────────────────────────────────────────────┐
│               Bsit.SupplyChain.Dotnet.Api               │
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
Bsit.SupplyChain.Dotnet.sln
│
├── src/
│   ├── Bsit.SupplyChain.Dotnet.Api              # WebAPI 主项目（启动）
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

> **命名说明**：解决方案及项目命名建议统一采用 `Bsit.SupplyChain.Dotnet` 前缀，以便与前端项目区分。示例中的 API 项目即 `Bsit.SupplyChain.Dotnet.Api`。

### 3.1 Bsit.SupplyChain.Dotnet.Api 详细结构

```
Bsit.SupplyChain.Dotnet.Api/
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
├── appsettings.json                      # 配置文件（连接字符串、JWT参数、Swagger信息等）
├── appsettings.Development.json
├── nlog.config                           # NLog 配置文件
├── Program.cs                            # 应用启动入口，配置服务容器与管道
└── Bsit.SupplyChain.Dotnet.Api.csproj
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
│   │   └── OrderQueryDto.cs
│   ├── Audit/
│   │   └── AuditLogDto.cs               # 审计日志传输对象
│   └── Common/
│       └── PagedResultDto.cs             # 分页结果基类
├── Interfaces/
│   ├── IAuthService.cs                   # 认证服务接口
│   ├── IOrderService.cs                  # 订单业务服务接口（示例）
│   ├── IWarehouseService.cs              # 仓储业务服务接口（示例）
│   ├── ITransportService.cs             # 运输业务服务接口（示例）
│   └── IAuditLogService.cs              # 审计日志业务服务接口（供中间件调用）
├── Services/
│   ├── AuthService.cs                    # 认证服务实现
│   ├── OrderService.cs                   # 订单业务服务实现（示例）
│   ├── WarehouseService.cs               # 仓储业务服务实现（示例）
│   ├── TransportService.cs              # 运输业务服务实现（示例）
│   └── AuditLogService.cs               # 审计日志服务实现（调用仓储保存）
├── EventHandlers/
│   ├── OrderCreatedEventHandler.cs       # 领域事件处理程序（示例）
│   └── OrderShippedEventHandler.cs       # 领域事件处理程序（示例）
├── AutoMapperProfile.cs                  # AutoMapper 映射配置（Entity <-> DTO，可选）
└── AutofacModule.cs                      # Autofac 模块：注册本层服务与事件处理程序
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
├── Interfaces/                             # 仓储接口
│   ├── IOrderRepository.cs                 # 示例
│   ├── IWarehouseRepository.cs             # 示例
│   ├── ITransportRepository.cs            # 示例
│   ├── IUserRepository.cs
│   ├── IAuditLogRepository.cs
│   └── IRepository.cs                      # 通用仓储接口（可选）
│
├── DomainServices/                         # 领域服务（跨聚合的业务逻辑）
│   └── ShipmentRoutingService.cs           # 示例
│
└── Constants/
    ├── AppConstants.cs                     # 全局业务常量
    └── CacheKeyConstants.cs                # 缓存Key常量（可选）
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

```csharp
// Domain/Entities/BaseEntity.cs（可选基类）
namespace Bsit.SupplyChain.Domain.Entities;

public abstract class BaseEntity
{
    private readonly List<INotification> _domainEvents = new();
    
    [SugarIgnore] // SqlSugar 忽略映射
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

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

### 5.5 工作单元与事件分发（Infrastructure 层）

```csharp
// Infrastructure/UnitOfWork/UnitOfWork.cs（示例框架）
namespace Bsit.SupplyChain.Infrastructure.UnitOfWork;

public class UnitOfWork
{
    private readonly SqlSugarClient _db;
    private readonly IPublisher _publisher;

    public UnitOfWork(SqlSugarClient db, IPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task SaveAndDispatchEventsAsync()
    {
        // 从跟踪的实体中收集所有领域事件
        var domainEvents = new List<INotification>();
        // ... 从变更追踪器获取实体并收集事件

        // 先持久化数据
        await _db.Saveable(...).ExecuteCommandAsync();

        // 数据保存成功后，再发布事件
        foreach (var e in domainEvents)
        {
            await _publisher.Publish(e);
        }
        
        // 清除已发布的事件
    }
}
```

### 5.6 注册 MediatR（Program.cs）

```csharp
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Bsit.SupplyChain.Domain.Entities.Order.Order).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
});
```

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

### Api 层 Program.cs 注册

```csharp
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new Application.AutofacModule());
    containerBuilder.RegisterModule(new Infrastructure.AutofacModule());
});
```

### Application 层 AutofacModule.cs

```csharp
public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(ThisAssembly)
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
               
        builder.RegisterAssemblyTypes(ThisAssembly)
               .AsClosedTypesOf(typeof(INotificationHandler<>))
               .InstancePerDependency();
    }
}
```

### Infrastructure 层 AutofacModule.cs

```csharp
public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c => SqlSugarSetup.GetClient()).SingleInstance();
        builder.RegisterAssemblyTypes(ThisAssembly)
               .Where(t => t.Name.EndsWith("Repository"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        builder.RegisterType<UnitOfWork>().InstancePerLifetimeScope();
    }
}
```

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

## 十一、开发约定与最佳实践

| 类别 | 规则 |
|------|------|
| **接口命名** | 上层仅依赖接口，接口定义在 `Domain/Interfaces/` 或 `Application/Interfaces/` |
| **注入范围** | SqlSugarClient → 单例；Repository/Service/UnitOfWork → InstancePerLifetimeScope；JwtHelper/HashHelper → 单例 |
| **DTO 使用** | 跨层传输一律使用 DTO，Entity 不返回至 Api 层 |
| **异常处理** | 业务层抛 `AppException`，由全局 `ExceptionMiddleware` 统一捕获，Controller 无 try-catch |
| **配置管理** | 通过 `IOptions<T>` 读取配置，配置类放 `Common/Configurations/` |
| **领域纯净性** | Domain 层不依赖 MediatR NuGet 包，仅引用 `MediatR.Contracts`（提供 `INotification` 接口） |
| **事件一致性** | 领域事件必须在数据保存成功后分发，保证业务操作与通知的原子性 |
| **日志使用** | 所有项目只使用 `ILogger<T>`，不直接依赖 NLog；启动时配置 NLog 作为提供程序 |
| **模块示例说明** | 本文档中的 `Order`、`Warehouse`、`Transport`、`Account` 等均为架构演示示例，实际项目须根据业务需求重新定义聚合、实体与服务接口 |

---

## 十二、总结

本架构以微软整洁架构与 DDD 分层思想为基础，专为 **Bsit.SupplyChain.Dotnet.Api** 后端 WebAPI 量身定制。从接收 HTTP 请求、认证鉴权、业务编排、领域模型到数据持久化，各层职责分明，同时通过 **NLog** 实现灵活的系统日志，通过 **全局异常中间件** 保证 API 返回的安全与友好，通过 **领域事件** 解耦核心业务。配合 SqlSugar 工具链、测试项目、Autofac 依赖注入，整个方案在保证可维护性、可测试性的同时，能够高效支撑物流、供应链、仓储等复杂业务场景的持续演进。

> **重要提醒**：架构中的具体业务模块（如订单、仓储、运输等）仅为展示分层组织方式而设，**实际项目的聚合划分、实体定义、服务接口等必须严格依据真实业务需求进行**，切勿生硬照搬示例结构。