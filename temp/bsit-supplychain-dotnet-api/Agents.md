# AI 项目指南 - Bsit.SupplyChain.Api

## 1. 通用规则

- 语言：全程中文交互与输出。
- 系统：严格限定 Windows 环境开发。
- 编码：所有文件统一无 BOM 的 UTF-8 格式。
- 读写：大文件必须分段读写以优化资源。
- 目录 `docs`：按类型（需求/设计/API/手册等）分类归档。
- 时间：必须且仅通过 Time MCP 获取，禁用本地系统时间。
- 技术决策：遇技术疑点必须调用 Context7 MCP 查阅官方文档。
- 注释：代码必须含详尽中文注释（含用途、参数、返回值、核心逻辑、异常说明）。
- 当前开发项目文件夹：src/main

## 2. 项目概述

- 技术栈：.NET 10 WebAPI / Autofac / SqlSugar / AutoMapper / FluentValidation / MediatR / NLog / MSSQL
- 前端仓库：`bsit-supplychain-web`（Vue3 + Vite + Pinia）
- 架构模式：领域驱动设计（DDD）+ 依赖倒置 + 四层架构
- 核心特性：Swagger 接口文档、JWT 认证（Access Token + Refresh Token）、请求审计日志、NLog 系统日志（文件+MSSQL）、统一返回格式、AutoMapper 对象映射、FluentValidation 请求验证、领域事件（MediatR）、CORS 跨域、软删除与审计字段、单元测试与集成测试、SqlSugar 工具链

## 3. 目录说明

- `src/main/`：核心 DDD 分层项目（Api / Application / Domain / Infrastructure / Common）
- `src/tests/`：测试项目（UnitTests / IntegrationTests）
- `src/tools/`：工具项目（SqlSugarTool）
- `docs/`：按类型分类归档
  - `requirements/`：业务需求文档
  - `design/`：设计文档、架构图
  - `specs/`：技术规格（OpenAPI、数据库设计）
  - `api/`：API 文档、错误码
  - `database/`：SQL 脚本、迁移脚本
  - `prompts/`：AI 提示词模板
  - `guides/`：开发手册
- `agents-docs/`：详细编码与架构规则，AI 按需读取（渐进式披露）
- `temp/`：AI 临时文件，用完即删

## 4. 任务路由（AI 按任务类型读取对应文件）

| 我要做什么 | 必须先读（按顺序） | 参考样板 |
|------------|---------------------|----------|
| 新建聚合根/实体/值对象 | `agents-docs/architecture-layers.md` → `dotnet-coding-style.md` | `agents-docs/templates/sample-aggregate.cs` |
| 新建仓储接口 + 实现 | `agents-docs/repository-pattern.md` → `sqlsugar-usage.md` | `agents-docs/templates/sample-repository.cs` |
| 新建 Application Service | `agents-docs/architecture-layers.md` → `repository-pattern.md` | `agents-docs/templates/sample-service.cs` |
| 新建 DTO + Validator | `agents-docs/validation-rules.md` → `automapper-rules.md` | `agents-docs/templates/sample-dto-validator.cs` |
| 新建 Controller | `agents-docs/api-conventions.md` → `architecture-layers.md` | `agents-docs/templates/sample-controller.cs` |
| 新建 AutoMapper Profile | `agents-docs/automapper-rules.md` | `agents-docs/templates/sample-mapping-profile.cs` |
| 新建领域事件 + Handler | `agents-docs/domain-events.md` | `agents-docs/templates/sample-domain-event.cs` |
| 创建完整业务模块（端到端） | **执行工作流** `.windsurf/workflows/new-module.md` | 所有样板 |
| 排查异常 / 日志问题 | `agents-docs/error-handling.md` → `logging-guide.md` | — |
| 编写 / 修改测试 | `agents-docs/testing.md` | — |

> **规则**：执行任务前，必须先读取对应的规则文件和样板文件，禁止凭记忆生成代码。

## 5. AI 行为规则

- **临时文件**：任何临时生成文件必须放入 `temp/`，任务结束后立即删除。禁止在 `src/`、`docs/`、`agents-docs/` 创建临时文件。
- **代码生成**：遵循 `agents-docs/dotnet-coding-style.md` 和 `agents-docs/architecture-layers.md` 的分层依赖规则。
- **仓储实现**：参考 `agents-docs/repository-pattern.md`，接口定义在 Domain 层，实现在 Infrastructure 层。
- **API 设计**：遵循 `agents-docs/api-conventions.md` 的 RESTful 规范，使用 Swagger 注解。
- **数据库访问**：使用 SqlSugar（参考 `agents-docs/sqlsugar-usage.md`），禁止在 Domain 层出现 SqlSugar 相关代码。
- **异常处理**：使用全局异常中间件，自定义业务异常（参考 `agents-docs/error-handling.md`）。
- **请求验证**：所有请求 DTO 必须配备 FluentValidation 验证器（参考 `agents-docs/validation-rules.md`）。
- **对象映射**：必须使用 AutoMapper，禁止将 Entity 直接暴露给 Api 层（参考 `agents-docs/automapper-rules.md`）。
- **日志系统**：只用 `ILogger<T>`，不直接依赖 NLog（参考 `agents-docs/logging-guide.md`）。
- **认证鉴权**：JWT 认证参考 `agents-docs/auth-jwt.md`。
- **自检**：代码生成完毕后，必须对照 `agents-docs/checklist.md` 逐项核实。

## 6. 分层依赖规则

```
Api → Application, Infrastructure, Common
Application → Domain, Common
Infrastructure → Domain, Common
Domain → Common（可选）
Common → 无依赖
```

**上层可依赖下层，下层不可依赖上层；所有依赖均指向领域核心或公共层，确保领域层纯净。**

## 7. 常用提示词位置

- 生成聚合根：`docs/prompts/aggregate-root.md`
- 生成仓储：`docs/prompts/repository.md`
- 生成 API Controller：`docs/prompts/controller.md`
- 生成 Service：`docs/prompts/service.md`
- 生成 DTO + Validator：`docs/prompts/dto-validator.md`

## 8. 与前端协作

- 每次 API 变更后，更新 `docs/specs/openapi.yaml`，通知前端仓库更新 API 客户端。
- 错误码定义见 `docs/api/error-codes.md`。

## 9. 临时文件管理

- 所有临时生成的文件（代码、文档、脚本、数据）请放入 `temp/` 目录。
- 使用完毕后立即删除。
- 该目录已加入 `.gitignore`，无需担心提交问题。
- 禁止在项目其他任何位置创建临时文件。
