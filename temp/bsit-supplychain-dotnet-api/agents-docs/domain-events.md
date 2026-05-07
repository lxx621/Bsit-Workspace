# 领域事件机制说明

## 事件定义（Domain 层）

- 位于 `Domain/Events/` 目录
- 使用 `record` 类型 + 实现 `INotification` 接口
- 命名使用过去式（如 `OrderCreatedEvent`）
- 保证不可变性

```csharp
public record OrderCreatedEvent(Guid OrderId, string CustomerId, decimal Total, DateTime OccurredOn)
    : INotification;
```

## 事件收集（聚合根）

- 聚合根通过 `BaseEntity.AddDomainEvent()` 收集事件
- 事件集合通过 `[SugarColumn(IsIgnore = true)]` 排除数据库映射

## 事件分发（UnitOfWork）

- Service 层执行仓储操作后调用 `_unitOfWork.CollectEvents(entity)`
- `CommitAsync()` 提交事务成功后，逐一发布领域事件
- 事件处理程序位于 `Application/EventHandlers/`

## MediatR 注册

- 仅扫描 Application 程序集（所有 Handler 在此层）
- Domain 层不含 Handler，无需扫描

```csharp
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
});
```
