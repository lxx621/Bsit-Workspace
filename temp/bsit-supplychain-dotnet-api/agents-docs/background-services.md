# 后台服务规范（Background Services）

## 概述

系统中需要**定时执行**或**后台异步执行**的任务（如定时数据同步、队列消费、过期数据清理等），统一使用 .NET 内置的 `BackgroundService`（继承自 `IHostedService`）实现。

## 放置位置

后台服务放置在 **Api 层**（表示层）的 `BackgroundServices/` 目录下，不分子目录。

```
Bsit.SupplyChain.Api/
├── BackgroundServices/
│   ├── OrderExpireCleanupService.cs
│   ├── DataSyncTimedService.cs
│   └── QueueProcessingService.cs
└── ...
```

## 分层职责

| 关注点 | 所在层 | 说明 |
|--------|--------|------|
| 后台服务宿主类 | Api (`BackgroundServices/`) | 继承 `BackgroundService`，控制执行周期和生命周期 |
| 业务逻辑 | Application (`Services/`) | 后台服务调用的具体业务逻辑，通过接口注入 |
| 数据访问 | Infrastructure (`Repositories/`) | 业务逻辑所需的数据库操作 |
| 配置项 | Common (`Configurations/`) 或 `appsettings.json` | 定时间隔、开关等配置 |

## 关键规范

| 规则 | 说明 |
|------|------|
| **目录位置** | `Api/BackgroundServices/`，不分子目录（属于横切关注点） |
| **命名** | `{功能描述}Service.cs`，如 `OrderExpireCleanupService`、`DataSyncTimedService` |
| **Scope 管理** | 后台服务是 Singleton 生命周期，**必须**通过 `IServiceProvider.CreateScope()` 获取 Scoped 服务（如 Repository、SqlSugarClient） |
| **异常处理** | 循环体内必须 `try-catch`，防止单次异常终止整个服务；记录 Error 日志 |
| **取消令牌** | 必须响应 `CancellationToken`（`stoppingToken`），确保应用关闭时优雅停止 |
| **配置化** | 执行间隔、开关等参数从 `appsettings.json` 读取，通过 `IOptions<T>` 注入 |
| **日志** | 启动、每次执行完毕、异常、停止均需记录日志 |
| **业务逻辑分离** | 后台服务类仅负责调度（定时/触发），具体业务逻辑委托给 Application 层服务 |

## 注册方式

```csharp
// Program.cs
builder.Services.AddHostedService<OrderExpireCleanupService>();
```

## 代码模板

```csharp
// Api/BackgroundServices/XxxService.cs
namespace Bsit.SupplyChain.Api.BackgroundServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bsit.SupplyChain.Common.Configurations;

/// <summary>
/// Xxx 后台服务
/// 定时执行 Xxx 业务逻辑
/// </summary>
public class XxxService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<XxxService> _logger;
    private readonly TimeSpan _interval;

    public XxxService(
        IServiceProvider serviceProvider,
        ILogger<XxxService> logger,
        IOptions<BackgroundServiceSettings> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _interval = TimeSpan.FromMinutes(options.Value.IntervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Xxx 服务已启动，执行间隔：{Interval}", _interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ★ 必须创建 Scope
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IXxxService>();

                await service.ExecuteAsync(stoppingToken);

                _logger.LogInformation("Xxx 执行完毕");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Xxx 执行异常");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Xxx 服务已停止");
    }
}
```

## 配置示例

```json
// appsettings.json
{
  "BackgroundServices": {
    "Xxx": {
      "Enabled": true,
      "IntervalMinutes": 30
    }
  }
}
```

```csharp
// Common/Configurations/BackgroundServiceSettings.cs
namespace Bsit.SupplyChain.Common.Configurations;

/// <summary>
/// 后台服务配置选项基类
/// </summary>
public class BackgroundServiceSettings
{
    /// <summary>是否启用该后台服务</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>执行间隔（分钟）</summary>
    public int IntervalMinutes { get; set; } = 30;
}
```
