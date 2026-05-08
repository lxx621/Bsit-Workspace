# api分层架构职责说明

## 一、Bsit.SupplyChain.Api（表现层）

**核心职责**：
- 接收 HTTP 请求，路由分发到对应的 Controller
- 定义 API 控制器和 Action（按业务模块子目录组织）
- 配置中间件：审计日志、全局异常处理、请求追踪ID
- 定义 Filters：模型校验过滤器、权限验证过滤器
- 扩展方法：ServiceCollection 扩展（注册 Swagger、认证）、ApplicationBuilder 扩展（使用中间件）
- Autofac 容器配置入口
- 应用启动入口（Program.cs），配置服务容器与请求管道
- 集成 Swagger 接口文档、JWT 认证、NLog 日志

**依赖关系**：依赖 Application、Infrastructure、Common 层

---

## 二、Bsit.SupplyChain.Application（应用层）

**核心职责**：
- 定义请求/响应 DTO（按业务模块组织）
- 定义应用服务接口和实现（业务用例编排）
- **业务流程编排**：调用 Domain 层的聚合根工厂方法、领域行为方法
- 通过 Domain 层定义的仓储接口进行数据持久化
- 使用 UnitOfWork 协调数据库事务
- **领域事件处理**：订阅并处理 Domain 层发布的领域事件（EventHandlers）
- DTO 转换（通过 AutoMapper）
- 请求验证（通过 FluentValidation 验证器）
- MediatR Pipeline Behaviors（日志、验证等横切关注点）

**关键点**：
- 不直接操作数据库，通过仓储接口
- 不实现业务逻辑，而是编排 Domain 层的业务逻辑
- 使用 `ICurrentUser` 从 JWT Claims 获取当前用户信息

**依赖关系**：依赖 Domain、Common 层

---

## 三、Bsit.SupplyChain.Domain（领域层）

**核心职责**：
- **聚合根**：继承 BaseEntity，包含全局唯一标识，定义领域行为方法，通过工厂方法创建（如 `Order.Create()`）
- **实体**：聚合内的业务对象，有局部唯一标识
- **值对象**：不可变对象（推荐 record 类型），由值定义相等性
- **领域事件**：继承 INotification，使用 record 类型，命名使用过去式（如 `OrderCreatedEvent`）
- **仓储接口**：定义数据访问契约（每个聚合一个接口）
- **领域服务**：跨聚合的业务逻辑处理（无状态服务）
- **业务常量**：聚合内常量、全局业务常量
- **全局枚举**：跨聚合共享的枚举类型
- **IUnitOfWork 接口**：定义事务协调和领域事件分发契约

**BaseEntity 提供的能力**：
- 审计字段（CreatedAt、CreatedBy、UpdatedAt、UpdatedBy）
- 软删除（IsDeleted、DeletedAt）
- 乐观并发（RowVersion）
- 领域事件收集与分发（AddDomainEvent、ClearDomainEvents）

**依赖关系**：可依赖 Common 层（可选），不依赖任何上层

---

## 四、Bsit.SupplyChain.Infrastructure（基础设施层）

**核心职责**：
- 实现 Domain 层定义的仓储接口（按业务模块组织）
- 封装 SqlSugar 数据库操作
- 实现 IUnitOfWork（基于 SqlSugar 事务）
- SqlSugar 客户端配置（连接字符串、实体映射）
- 审计字段自动赋值拦截器（Aop.DataExecuting）
- 软删除全局过滤器（QueryFilter）
- 种子数据初始化（DefaultDataSeeder）
- SqlSugar 扩展方法（通用排序、分页等）

**依赖关系**：依赖 Domain、Common 层

---

## 五、Bsit.SupplyChain.Common（公共层）

**核心职责**：
- 通用工具类：JwtHelper、HashHelper、DateTimeHelper、StringHelper
- 统一返回模型：ApiResult、ApiResultCode、PaginationParams
- 配置选项类：JwtSettings、AppSettings（映射 appsettings.json）
- 自定义异常：AppException、ValidationException

**关键点**：
- 不依赖任何业务层（Api、Application、Domain、Infrastructure）
- 可被所有层引用

**依赖关系**：无依赖

---

## 典型业务流程示例

以"创建订单"为例：

1. **Api 层**：OrderController 接收 HTTP 请求，调用 Application 层的 IOrderService
2. **Application 层**：
   - 验证 DTO（FluentValidation）
   - 调用 Domain 层的 `Order.Create()` 工厂方法创建聚合根
   - 调用聚合根的业务方法（如添加订单项）
   - UnitOfWork.BeginTran() 开启事务
   - 调用仓储接口 `IOrderRepository.AddAsync()` 持久化
   - UnitOfWork.CollectEvents(order) 收集领域事件
   - UnitOfWork.CommitAsync() 提交事务并发布事件
3. **Domain 层**：
   - Order 聚合根在 Create() 时添加 `OrderCreatedEvent` 领域事件
4. **Infrastructure 层**：
   - OrderRepository 实现仓储接口，使用 SqlSugar 执行数据库操作
   - UnitOfWork 实现 SqlSugar 事务管理
5. **Application 层**：
   - OrderCreatedEventHandler 订阅并处理订单创建事件
6. **Common 层**：
   - 使用 ApiResult 返回统一格式结果

---

## 依赖规则

- 上层可依赖下层，下层不可依赖上层
- 所有依赖均指向领域核心或公共层
- 确保领域层纯净（不依赖任何基础设施）