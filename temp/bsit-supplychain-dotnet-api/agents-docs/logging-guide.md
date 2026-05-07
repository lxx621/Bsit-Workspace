# NLog 日志系统集成规范

## 基本原则

- 所有层只使用 `ILogger<T>`，不直接依赖 NLog API。
- NLog NuGet 包仅在 Api 项目引用。

## 双目标配置

| 目标 | 说明 |
|------|------|
| 文件日志 | 按日期分文件（`logs/{shortdate}.log`），保留30天 |
| 数据库日志 | 存入 `SystemLogs` 表，Error 级别以上 |

## 日志级别使用

| 级别 | 场景 |
|------|------|
| Debug | 开发调试信息（仅开发环境） |
| Information | 正常业务流程（如"开始创建订单"） |
| Warning | 业务异常、可恢复的错误 |
| Error | 系统异常、不可恢复的错误 |
| Fatal | 应用程序启动失败 |

## 各层使用

```csharp
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public async Task CreateAsync(OrderCreateDto dto)
    {
        _logger.LogInformation("开始创建订单，客户ID：{CustomerId}", dto.CustomerId);
    }
}
```

## 配置文件

NLog 配置位于 `Api/nlog.config`，通过 `Program.cs` 加载。
