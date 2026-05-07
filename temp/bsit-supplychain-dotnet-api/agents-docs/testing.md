# 单元测试与集成测试规范

## 测试项目

| 项目 | 类型 | 测试范围 |
|------|------|----------|
| `Bsit.SupplyChain.UnitTests` | 单元测试 | Application 服务、Domain 实体、Common 工具类 |
| `Bsit.SupplyChain.IntegrationTests` | 集成测试 | 仓储实现、API 端点、中间件 |

## 目录组织

```
tests/
├── Bsit.SupplyChain.UnitTests/
│   ├── Application/Services/          # 服务层测试
│   ├── Application/EventHandlers/     # 事件处理测试
│   ├── Domain/                        # 领域层测试
│   └── Common/                        # 工具类测试
└── Bsit.SupplyChain.IntegrationTests/
    ├── Repositories/                  # 仓储集成测试
    ├── Api/                           # API 端点测试
    ├── Middlewares/                   # 中间件测试
    └── TestStartup.cs                # 测试启动配置
```

## 测试框架

- xUnit 作为测试框架
- Moq 作为 Mock 框架
- 集成测试使用 `WebApplicationFactory`

## 命名规范

- 测试类：`{被测类名}Tests.cs`
- 测试方法：`{方法名}_{场景}_{预期结果}`

```csharp
[Fact]
public async Task GetByIdAsync_WhenOrderExists_ReturnsOrderDetail()
```
