# JSON 序列化规范（Newtonsoft.Json）

## 概述

系统统一使用 **Newtonsoft.Json** 作为 JSON 序列化/反序列化引擎，**禁止**使用 `System.Text.Json`。核心配置要求：**禁用 Unicode 转义**，确保中文等非 ASCII 字符直接输出而不是转为 `\uXXXX` 格式。

## NuGet 包

| 项目 | 包名 | 说明 |
|------|------|------|
| Api | `Microsoft.AspNetCore.Mvc.NewtonsoftJson` | ASP.NET Core MVC 集成 |
| Common | `Newtonsoft.Json` | 公共层工具类中使用 |

## 全局配置（Program.cs）

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // ★ 禁用 Unicode 转义，中文等字符直接输出
        options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.Default;
        // camelCase 属性命名
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        // 日期格式
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        // 忽略循环引用
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
```

## 手动序列化（JsonHelper）

非 Controller 场景（中间件、后台服务、日志记录）使用 `Common/Helpers/JsonHelper.cs`：

```csharp
// Common/Helpers/JsonHelper.cs
namespace Bsit.SupplyChain.Common.Helpers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// JSON 序列化/反序列化工具类
/// 统一配置：禁用 Unicode 转义、camelCase 命名、标准日期格式
/// </summary>
public static class JsonHelper
{
    /// <summary>全局共享的序列化配置（只读，线程安全）</summary>
    public static readonly JsonSerializerSettings DefaultSettings = new()
    {
        StringEscapeHandling = StringEscapeHandling.Default,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        DateFormatString = "yyyy-MM-dd HH:mm:ss",
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    /// <summary>将对象序列化为 JSON 字符串（中文直接输出，不转义）</summary>
    public static string Serialize(object? obj)
        => JsonConvert.SerializeObject(obj, DefaultSettings);

    /// <summary>将 JSON 字符串反序列化为指定类型</summary>
    public static T? Deserialize<T>(string json)
        => JsonConvert.DeserializeObject<T>(json, DefaultSettings);
}
```

## 使用规范

| 规则 | 说明 |
|------|------|
| **统一引擎** | 全项目使用 Newtonsoft.Json，**禁止** `System.Text.Json` |
| **禁用 Unicode 转义** | `StringEscapeHandling.Default`，中文直接输出 |
| **命名策略** | `CamelCasePropertyNamesContractResolver` |
| **日期格式** | `yyyy-MM-dd HH:mm:ss` |
| **手动序列化** | 使用 `JsonHelper.Serialize()` / `JsonHelper.Deserialize<T>()`，禁止 `new JsonSerializerSettings()` |
| **循环引用** | `ReferenceLoopHandling.Ignore` |
| **ExceptionMiddleware** | 响应 JSON 使用 `JsonHelper.Serialize(result)` |
