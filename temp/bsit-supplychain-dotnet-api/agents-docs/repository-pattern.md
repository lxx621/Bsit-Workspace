# 仓储模式与 UnitOfWork 规范

## 仓储接口（Domain 层）

- 通用接口 `IRepository<T>` 定义在 `Domain/Interfaces/`，约束 `T : BaseEntity, new()`。
- 每个聚合一个专用仓储接口（如 `IOrderRepository`），继承 `IRepository<T>` 并扩展特有查询。
- 仓储接口仅定义数据访问契约，不包含实现细节。

## 仓储实现（Infrastructure 层）

- 通用基类 `BaseRepository<T>` 封装 SqlSugar CRUD 操作。
- 具体仓储继承 `BaseRepository<T>` 并添加聚合特有查询。
- 通过 Autofac 按约定自动注册（类名以 `Repository` 结尾）。

## UnitOfWork 模式

典型使用流程：

```csharp
_unitOfWork.BeginTran();           // 1. 开启事务
try
{
    await _repository.AddAsync(entity);  // 2. 执行仓储操作
    _unitOfWork.CollectEvents(entity);   // 3. 收集领域事件
    await _unitOfWork.CommitAsync();     // 4. 提交事务 + 发布事件
}
catch
{
    _unitOfWork.Rollback();              // 5. 异常回滚
    throw;
}
```

## 注册规范

- SqlSugarClient：Scoped（InstancePerLifetimeScope）
- Repository：Scoped
- UnitOfWork：Scoped
