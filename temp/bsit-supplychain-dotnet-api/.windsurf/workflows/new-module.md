---
description: 创建一个完整的业务模块（聚合根 → 仓储 → 服务 → 控制器 → DTO → 验证器 → 映射 → 领域事件）
---

# 新建业务模块工作流

## 前置准备

1. 确认模块信息：
   - 模块名称（英文 PascalCase，如 `Customer`）
   - 数据库表名（如 `Customers`）
   - 核心字段列表（字段名 + 类型 + 约束）
   - 是否需要值对象
   - 是否需要领域事件

2. 读取必要规则文件：
   - `agents-docs/architecture-layers.md`
   - `agents-docs/dotnet-coding-style.md`
   - `agents-docs/repository-pattern.md`

## 执行步骤（严格按顺序）

### 步骤 1：Domain 层 — 创建聚合根

- 参考样板：`agents-docs/templates/sample-aggregate.cs`
- 创建文件：
  - `src/Bsit.SupplyChain.Domain/Entities/{模块名}/{模块名}.cs`（聚合根）
  - `src/Bsit.SupplyChain.Domain/Entities/{模块名}/ValueObjects/*.cs`（如需值对象）
  - `src/Bsit.SupplyChain.Domain/Entities/{模块名}/{模块名}Constants.cs`（聚合常量）
- 检查点：
  - [ ] 继承 `BaseEntity`
  - [ ] 使用 `[SugarTable]` 和 `[SugarColumn]` 特性
  - [ ] 包含静态工厂方法 `Create()`
  - [ ] 私有无参构造函数
  - [ ] 属性使用 `private set`
  - [ ] 中文 XML 注释完整

### 步骤 2：Domain 层 — 创建领域事件（如需要）

- 参考样板：`agents-docs/templates/sample-domain-event.cs`
- 创建文件：`src/Bsit.SupplyChain.Domain/Events/{模块名}CreatedEvent.cs`
- 检查点：
  - [ ] 使用 `record` 类型
  - [ ] 实现 `INotification`
  - [ ] 命名使用过去式

### 步骤 3：Domain 层 — 创建仓储接口

- 参考样板：`agents-docs/templates/sample-repository.cs`（文件 1）
- 创建文件：`src/Bsit.SupplyChain.Domain/Interfaces/I{模块名}Repository.cs`
- 检查点：
  - [ ] 继承 `IRepository<T>`
  - [ ] 仅定义数据访问契约，不含实现

### 步骤 4：Infrastructure 层 — 创建仓储实现

- 参考样板：`agents-docs/templates/sample-repository.cs`（文件 2）
- 创建文件：`src/Bsit.SupplyChain.Infrastructure/Repositories/{模块名}Repository.cs`
- 检查点：
  - [ ] 继承 `BaseRepository<T>` 并实现专有接口
  - [ ] 仅使用 SqlSugar API
  - [ ] 分页查询使用 `RefAsync<int>` + `ToPageListAsync`

### 步骤 5：Application 层 — 创建 DTO

- 参考样板：`agents-docs/templates/sample-dto-validator.cs`（DTO 部分）
- 创建目录及文件：`src/Bsit.SupplyChain.Application/Dtos/{模块名}/`
  - `{模块名}CreateDto.cs`
  - `{模块名}UpdateDto.cs`
  - `{模块名}DetailDto.cs`
  - `{模块名}QueryDto.cs`（继承 `PagedRequestDto`）

### 步骤 6：Application 层 — 创建 Validator

- 参考样板：`agents-docs/templates/sample-dto-validator.cs`（Validator 部分）
- 创建目录及文件：`src/Bsit.SupplyChain.Application/Validators/{模块名}/`
  - `{模块名}CreateDtoValidator.cs`
  - `{模块名}UpdateDtoValidator.cs`
- 检查点：
  - [ ] 每条规则含中文 `.WithMessage()`

### 步骤 7：Application 层 — 创建 Mapping Profile

- 参考样板：`agents-docs/templates/sample-mapping-profile.cs`
- 创建文件：`src/Bsit.SupplyChain.Application/Mappings/{模块名}MappingProfile.cs`
- 检查点：
  - [ ] ID / 审计字段 / DomainEvents 已 Ignore（如有 DTO→Entity 映射）

### 步骤 8：Application 层 — 创建 Service

- 参考样板：`agents-docs/templates/sample-service.cs`
- 创建文件：
  - `src/Bsit.SupplyChain.Application/Interfaces/I{模块名}Service.cs`
  - `src/Bsit.SupplyChain.Application/Services/{模块名}Service.cs`
- 检查点：
  - [ ] 注入 IRepository / IUnitOfWork / IMapper / ICurrentUser / ILogger
  - [ ] 写入操作使用 UnitOfWork 模式
  - [ ] 设置 CreatedBy / UpdatedBy 审计字段

### 步骤 9：Application 层 — 创建 EventHandler（如需要）

- 参考样板：`agents-docs/templates/sample-domain-event.cs`（Handler 部分）
- 创建文件：`src/Bsit.SupplyChain.Application/EventHandlers/{模块名}CreatedEventHandler.cs`

### 步骤 10：Api 层 — 创建 Controller

- 参考样板：`agents-docs/templates/sample-controller.cs`
- 创建文件：`src/Bsit.SupplyChain.Api/Controllers/{模块名}Controller.cs`
- 检查点：
  - [ ] `[ApiController]` + `[Route("api/[controller]")]` + `[Authorize]`
  - [ ] 使用 `ApiResult<T>` 返回
  - [ ] Swagger `[ProducesResponseType]` 注解

### 步骤 11：自检

- 对照 `agents-docs/checklist.md` 逐项核实所有生成的文件。

## 生成文件清单（汇总）

完成一个模块至少需要以下文件（以 `Customer` 为例）：

```
Domain/Entities/Customer/Customer.cs
Domain/Entities/Customer/ValueObjects/ContactInfo.cs
Domain/Entities/Customer/CustomerConstants.cs
Domain/Events/CustomerCreatedEvent.cs
Domain/Interfaces/ICustomerRepository.cs
Infrastructure/Repositories/CustomerRepository.cs
Application/Dtos/Customer/CustomerCreateDto.cs
Application/Dtos/Customer/CustomerUpdateDto.cs
Application/Dtos/Customer/CustomerDetailDto.cs
Application/Dtos/Customer/CustomerQueryDto.cs
Application/Validators/Customer/CustomerCreateDtoValidator.cs
Application/Validators/Customer/CustomerUpdateDtoValidator.cs
Application/Mappings/CustomerMappingProfile.cs
Application/Interfaces/ICustomerService.cs
Application/Services/CustomerService.cs
Application/EventHandlers/CustomerCreatedEventHandler.cs
Api/Controllers/CustomerController.cs
```
