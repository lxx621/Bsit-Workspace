## DefaultDataSeeder.cs 的作用

**DefaultDataSeeder** 是种子数据初始化器，用于在应用程序首次启动时向数据库插入初始数据。

## 典型用途

### 1. **初始化管理员账号**
```csharp
// Infrastructure/SeedData/DefaultDataSeeder.cs
namespace Bsit.SupplyChain.Infrastructure.SeedData;

using SqlSugar;
using Bsit.SupplyChain.Domain.Entities.Account;
using Bsit.SupplyChain.Common.Helpers;

/// <summary>
/// 种子数据初始化器
/// </summary>
public class DefaultDataSeeder
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<<DefaultDataSeeder> _logger;

    public DefaultDataSeeder(ISqlSugarClient db, ILogger<<DefaultDataSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// 初始化种子数据
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("开始初始化种子数据...");

        // 1. 检查是否已初始化
        var hasAdmin = await _db.Queryable<User>()
            .Where(u => u.UserName == "admin")
            .AnyAsync();

        if (hasAdmin)
        {
            _logger.LogInformation("种子数据已存在，跳过初始化");
            return;
        }

        // 2. 创建管理员账号
        var admin = new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            PasswordHash = HashHelper.HashPassword("Admin@123"),  // 密码哈希
            Role = UserRole.Admin,
            Email = "admin@bsit.com",
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };

        await _db.Insertable(admin).ExecuteCommandAsync();
        _logger.LogInformation("管理员账号创建成功");

        // 3. 初始化基础配置数据
        await SeedSystemConfigAsync();

        // 4. 初始化字典数据
        await SeedDictionaryAsync();

        _logger.LogInformation("种子数据初始化完成");
    }

    private async Task SeedSystemConfigAsync()
    {
        // 插入系统配置
        var configs = new List<<SystemConfig>
        {
            new() { Key = "SystemName", Value = "Bsit供应链管理系统" },
            new() { Key = "DefaultPageSize", Value = "20" },
        };
        await _db.Insertable(configs).ExecuteCommandAsync();
    }

    private async Task SeedDictionaryAsync()
    {
        // 插入字典数据（订单状态、支付方式等）
        var dictionaries = new List<<Dictionary>
        {
            new() { Type = "OrderStatus", Code = "Created", Name = "已创建" },
            new() { Type = "OrderStatus", Code = "Paid", Name = "已支付" },
            new() { Type = "OrderStatus", Code = "Shipped", Name = "已发货" },
            new() { Type = "OrderStatus", Code = "Completed", Name = "已完成" },
        };
        await _db.Insertable(dictionaries).ExecuteCommandAsync();
    }
}
```

### 2. **初始化业务基础数据**
- 订单状态枚举
- 支付方式
- 物流公司信息
- 仓库类型
- 商品分类

### 3. **初始化测试数据**
- 测试用户账号
- 示例订单
- 示例商品

## 调用时机

```csharp
// Program.cs
public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    
    // ... 配置服务
    
    var app = builder.Build();
    
    // ... 配置中间件
    
    // 应用启动时初始化种子数据
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<<DefaultDataSeeder>();
        await seeder.SeedAsync();
    }
    
    app.Run();
}
```

## 设计原则

### 1. **幂等性**
```csharp
// 先检查数据是否存在，避免重复插入
var hasAdmin = await _db.Queryable<User>()
    .Where(u => u.UserName == "admin")
    .AnyAsync();

if (hasAdmin) return;  // 已存在则跳过
```

### 2. **幂等性**
- 每次启动都执行，但不会重复插入数据
- 通过检查关键字段判断是否已初始化

### 3. **环境区分**
```csharp
// 只在开发环境插入测试数据
if (builder.Environment.IsDevelopment())
{
    await SeedTestDataAsync();
}
```

### 4. **日志记录**
```csharp
_logger.LogInformation("开始初始化种子数据...");
_logger.LogInformation("种子数据初始化完成");
```

## 优势

- **快速启动**：新部署的系统可以直接使用，无需手动创建基础数据
- **一致性**：保证不同环境的初始数据一致
- **可维护**：集中管理初始数据，易于修改
- **自动化**：应用启动时自动执行，无需人工干预

## 总结

DefaultDataSeeder 用于在应用首次启动时自动初始化数据库的基础数据（如管理员账号、系统配置、字典数据等），确保系统可以立即投入使用。