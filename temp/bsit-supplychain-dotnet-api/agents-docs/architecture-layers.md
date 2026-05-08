# 分层架构与依赖规则

## 架构总览

系统遵循 **依赖倒置** 与 **关注点分离** 原则，代码组织为五个核心项目（均位于 `src/main/` 目录下）：

```
Api（表示层） → Application（应用层） → Domain（领域层） ← Infrastructure（基础设施层）
                                         ↓
                                    Common（公共层）
```

## 各层职责

| 层 | 项目名 | 职责 | 允许依赖 |
|----|--------|------|----------|
| 表示层 | `Bsit.SupplyChain.Api` | 控制器、中间件、过滤器、Swagger、认证、后台服务 | Application, Infrastructure, Common |
| 应用层 | `Bsit.SupplyChain.Application` | 业务用例编排、DTO、服务、验证器、映射、事件处理 | Domain, Common |
| 领域层 | `Bsit.SupplyChain.Domain` | 聚合根、实体、值对象、领域事件、仓储接口、领域服务 | Common（可选） |
| 基础设施层 | `Bsit.SupplyChain.Infrastructure` | 仓储实现、SqlSugar 配置、UnitOfWork、种子数据 | Domain, Common |
| 公共层 | `Bsit.SupplyChain.Common` | JWT 助手、统一返回模型、自定义异常、配置类 | 无 |

## 严格禁止

- Domain 层不可引用 Infrastructure 或 Api 层。
- Domain 层不可引用 SqlSugar 或任何 ORM 框架（仅定义仓储接口）。
- Application 层不可引用 Infrastructure 层。
- Common 层不可引用任何业务层。
- 禁止跨层直接调用，必须通过接口依赖注入。

## 业务模块子目录规则

以下目录**必须**按业务模块创建子目录（如 `Order/`、`Warehouse/`），不得将所有文件平铺在同一目录下：

| 层 | 目录 | 子目录示例 | 说明 |
|----|------|-----------|------|
| Application | `Interfaces/` | `Auth/`, `Order/` | `ICurrentUser.cs` 等公共接口保留根目录 |
| Application | `Services/` | `Auth/`, `Order/` | 与 `Interfaces/` 模块名一一对应 |
| Application | `Dtos/` | `Account/`, `Order/`, `Common/` | `Common/` 放分页基类等 |
| Application | `Validators/` | `Account/`, `Order/` | 与 `Dtos/` 模块名一一对应 |
| Application | `Mappings/` | `Account/`, `Order/` | 每个模块一个 Profile |
| Application | `EventHandlers/` | `Order/`, `Warehouse/` | 按事件所属模块分组 |
| Domain | `Entities/` | `Order/`, `Warehouse/` | 按聚合分组 |
| Domain | `Events/` | `Order/`, `Warehouse/` | 按事件所属模块分组 |
| Domain | `Interfaces/` | `Order/`, `Account/` | `IUnitOfWork.cs`、`IRepository.cs` 保留根目录 |
| Infrastructure | `Repositories/` | `Order/`, `Account/` | `BaseRepository.cs` 保留根目录 |
| Api | `Controllers/` | `Auth/`, `Order/` | 按业务模块分组 |
| Api | `BackgroundServices/` | — | 后台服务，不分子目录（横切关注点） |

**不需要按模块分子目录的**（横切关注点）：
- `Middlewares/`、`Filters/`、`Extensions/`、`Behaviors/`、`Enums/`、`Constants/`、`Helpers/`、`BackgroundServices/`

## BaseEntity 基类

所有聚合根必须继承 `BaseEntity`，提供：
1. 审计字段（CreatedAt / CreatedBy / UpdatedAt / UpdatedBy）
2. 软删除（IsDeleted / DeletedAt）
3. 乐观并发（RowVersion）
4. 领域事件收集（DomainEvents）
