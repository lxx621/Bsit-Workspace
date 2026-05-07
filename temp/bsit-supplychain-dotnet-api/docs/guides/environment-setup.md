# 环境搭建指南

## 前置要求

| 工具 | 版本 | 说明 |
|------|------|------|
| .NET SDK | 10.0+ | 后端运行时 |
| SQL Server | 2019+ | 数据库 |
| Visual Studio / VS Code | 最新版 | IDE |
| Git | 最新版 | 版本控制 |

## 快速开始

1. 克隆仓库
2. 还原 NuGet 包：`dotnet restore`
3. 配置数据库连接：修改 `appsettings.Development.json`
4. 运行迁移/种子数据
5. 启动项目：`dotnet run --project src/Bsit.SupplyChain.Api`
6. 访问 Swagger：`https://localhost:{port}/swagger`
