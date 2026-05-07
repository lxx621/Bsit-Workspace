# .NET 编码风格规范

## 命名规范

| 元素 | 风格 | 示例 |
|------|------|------|
| 类、接口、方法、属性 | PascalCase | `OrderService`、`IOrderRepository`、`GetByIdAsync` |
| 私有字段 | _camelCase | `_orderRepository`、`_logger` |
| 局部变量、参数 | camelCase | `orderId`、`pageSize` |
| 常量 | PascalCase | `MaxPageSize`、`DefaultTimeout` |
| 接口 | I 前缀 + PascalCase | `IOrderService`、`IUnitOfWork` |

## async/await 模式

- 异步方法必须以 `Async` 后缀命名（如 `GetByIdAsync`）。
- 禁止使用 `.Result` 或 `.Wait()` 阻塞异步调用。
- 所有 I/O 操作（数据库、网络、文件）必须使用异步方法。

## using 排序

```csharp
// 1. System 命名空间
using System;
using System.Collections.Generic;
// 2. Microsoft 命名空间
using Microsoft.AspNetCore.Mvc;
// 3. 第三方库
using AutoMapper;
using SqlSugar;
// 4. 项目内部命名空间
using Bsit.SupplyChain.Domain.Entities;
```

## 中文注释要求

所有公共类、方法、属性必须包含 XML 注释，内容包括：
- **用途**：类/方法的职责说明
- **参数**：每个参数的含义与约束
- **返回值**：返回数据的说明
- **核心逻辑**：关键算法或业务流程说明
- **异常说明**：可能抛出的异常类型与触发条件

```csharp
/// <summary>
/// 根据订单ID查询订单详情
/// </summary>
/// <param name="id">订单唯一标识（GUID）</param>
/// <returns>订单详情 DTO，包含订单项列表</returns>
/// <exception cref="AppException">当订单不存在时抛出 404 异常</exception>
public async Task<ApiResult<OrderDetailDto>> GetByIdAsync(Guid id)
```

## 文件编码

- 所有文件统一使用无 BOM 的 UTF-8 编码。
