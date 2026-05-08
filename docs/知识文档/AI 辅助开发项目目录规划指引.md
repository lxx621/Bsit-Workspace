以下是为你的两个 Git 项目（前端 `bsit-supplychain-web`、后端 `bsit-supplychain-dotnet-api`）制定的 **AI 辅助开发项目目录规划指引**。内容涵盖结构规范、`Agents.md` 模板、渐进式披露目录说明以及临时文件管理规则。你可以直接将该文档作为团队规范或项目 README 的补充。

---

# AI 辅助开发项目目录规划指引

**适用范围**：`bsit-supplychain-web`（Vue3 前端）、`bsit-supplychain-dotnet-api`（.NET 后端）  
**核心原则**：统一根目录骨架 + 渐进式披露（`Agents.md` 入口 + `agents-docs/` 详细规则）+ 临时文件规范（`temp/`）

## 一、总体结构（两个仓库通用）

每个仓库根目录均遵循以下结构（具体内容各自专有）：

```
<repository-root>/
├── .claude/                     # Claude Code 配置（可选，其他工具同理）
├── .gitignore
├── Agents.md                    # AI 入口文件（固定文件名）
├── agents-docs/                 # 渐进式披露的详细规则目录
├── docs/                        # 人类与 AI 共享的文档（需求、spec、提示词等）
├── temp/                        # 临时文件目录（AI 产生，用完即删，已 ignore）
└── src/                         # 源代码（前端或后端的实际代码）
```

**强制规则**：
- 所有 AI 生成的临时文件（代码片段、草稿、测试脚本等）**必须**放在 `temp/` 下，使用后立即删除。
- `temp/` 已加入 `.gitignore`，禁止提交任何临时文件。
- 每个仓库的 `Agents.md` 必须清晰引用 `agents-docs/` 中的规则，并说明如何与另一侧仓库协作。

---

## 二、前端仓库（`bsit-supplychain-web`）

### 2.1 目录详细结构

```
bsit-supplychain-web/
├── .claude/                     # 可选，Claude Code 配置
├── .gitignore
├── Agents.md
├── agents-docs/
│   ├── vue3-coding-style.md
│   ├── component-structure.md
│   ├── state-management.md
│   ├── api-call-pattern.md
│   └── testing-guide.md
├── docs/
│   ├── requirements/            # 前端页面交互需求、用户故事
│   ├── specs/                   # UI/UX 设计规范、设计稿链接
│   ├── prompts/                 # AI 提示词模板（如“生成一个登录组件”）
│   └── guides/                  # 本地开发、构建、部署说明
├── temp/                        # 临时文件（AI 专用）
└── src/                         # Vue 3 项目（Vite 标准结构）
    ├── assets/
    ├── components/
    ├── views/
    ├── stores/
    ├── api/                     # 后端 API 调用封装
    ├── router/
    ├── utils/
    ├── App.vue
    └── main.ts
```

### 2.2 `Agents.md` 内容示例

```markdown
# AI 项目指南 - 前端（Vue 3）

## 1. 项目概述
- 技术栈：Vue 3 + Vite + Pinia + Axios
- 后端 API 仓库：`bsit-supplychain-dotnet-api`
- 后端 API 契约：以后端仓库 `docs/specs/openapi.yaml` 为准

## 2. 目录说明
- `src/`：源代码，遵循标准 Vite 结构
- `docs/`：需求、UI 规范、提示词、本地开发指南
- `agents-docs/`：详细的编码与架构规则，请按需读取
- `temp/`：AI 生成临时文件的位置，使用后必须清理

## 3. AI 行为规则
- **临时文件**：任何临时产物（代码、文档、脚本）必须放入 `temp/`，任务结束后立即删除。
- **代码生成**：优先使用 `agents-docs/vue3-coding-style.md` 中规定的组合式 API 风格。
- **API 调用**：参考 `agents-docs/api-call-pattern.md`，从后端 OpenAPI 生成类型安全的客户端。
- **组件拆分**：遵循 `agents-docs/component-structure.md` 的单文件组件规范。

## 4. 常用提示词位置
- 组件生成提示词：`docs/prompts/vue-component-gen.md`
- API 集成提示词：`docs/prompts/api-integration.md`

## 5. 与后端协作
- 接口变更时，后端会更新 OpenAPI 文件，前端运行 `npm run generate-api` 重新生成客户端。
- 遇到 API 问题，参考后端仓库的 `docs/specs/error-codes.md`。
```

### 2.3 `agents-docs/` 下各 MD 文件内容要点

| 文件 | 核心内容 |
|:---|:---|
| `vue3-coding-style.md` | 强制使用 `<script setup>` 语法，Prop/Emits 类型定义，ref/reactive 使用原则，生命周期钩子顺序。 |
| `component-structure.md` | 组件目录组织（`components/common/`, `components/domain/`），模板、样式、逻辑的顺序。 |
| `state-management.md` | Pinia store 的定义模式（Setup Store 风格），跨模块状态共享规范。 |
| `api-call-pattern.md` | 使用 Axios 实例拦截器，统一错误处理，请求/响应类型定义（从 OpenAPI 生成）。 |
| `testing-guide.md` | Vitest 单元测试、组件测试的编写规范，Mock 后端 API 的方法。 |

---

## 三、后端仓库（`bsit-supplychain-dotnet-api`）

### 3.1 目录详细结构

```
bsit-supplychain-dotnet-api/
├── .claude/
├── .gitignore
├── Agents.md
├── agents-docs/
│   ├── dotnet-coding-style.md
│   ├── architecture-layers.md
│   ├── repository-pattern.md
│   ├── api-conventions.md
│   ├── sqlsugar-usage.md
│   ├── error-handling.md
│   └── testing.md
├── docs/
│   ├── requirements/            # 后端业务需求（如订单状态机）
│   ├── specs/                   # 领域模型描述、OpenAPI 规范、数据库设计文档
│   ├── prompts/                 # 后端代码生成提示词（如“生成一个仓储实现”）
│   ├── database/                # SQL 脚本、迁移脚本
│   └── guides/                  # 环境搭建、Docker 部署
├── temp/
└── src/                         # .NET 9 解决方案（DDD 分层）
    ├── Bsit.SupplyChain.Api/                # Controllers, 过滤器
    ├── Bsit.SupplyChain.Application/        # Commands, Queries, Handlers, DTOs
    ├── Bsit.SupplyChain.Domain/             # Entities, Value Objects, Repositories 接口, Domain Events
    ├── Bsit.SupplyChain.Infrastructure/     # 仓储实现, SqlSugar, DbContext, 消息队列
    └── Bsit.SupplyChain.Tests/              # 单元测试、集成测试
```

### 3.2 `Agents.md` 内容示例

```markdown
# AI 项目指南 - .NET 后端（DDD）

## 1. 项目概述
- 技术栈：.NET 9/10, Autofac, SqlSugar, SQL Server
- 前端仓库：`bsit-supplychain-web`
- 架构模式：领域驱动设计（DDD）+ 依赖倒置 + CQRS（可选）

## 2. 目录说明
- `src/`：按 DDD 四层组织（Api/Application/Domain/Infrastructure）
- `docs/specs/openapi.yaml`：导出的 API 文档，供前端使用
- `docs/database/`：建表脚本、索引、存储过程
- `agents-docs/`：详细编码与架构规则
- `temp/`：AI 临时文件，用完即删

## 3. AI 行为规则
- **临时文件**：任何临时生成文件必须放入 `temp/`，任务结束后删除。
- **代码生成**：遵循 `agents-docs/dotnet-coding-style.md` 和 `architecture-layers.md` 的分层依赖规则。
- **仓储实现**：参考 `repository-pattern.md`，接口定义在 Domain 层，实现在 Infrastructure 层。
- **API 设计**：遵循 `api-conventions.md` 的 RESTful 规范，使用 Swagger 注解。
- **数据库访问**：使用 SqlSugar，禁止在 Domain 层出现 SqlSugar 相关代码。
- **错误处理**：使用全局异常过滤器，自定义业务异常，参考 `error-handling.md`。

## 4. 常用提示词位置
- 生成聚合根：`docs/prompts/aggregate-root.md`
- 生成仓储：`docs/prompts/repository.md`
- 生成 API Controller：`docs/prompts/controller.md`

## 5. 与前端协作
- 每次 API 变更后，运行 `dotnet swagger tofile --output openapi.json` 更新 `docs/specs/openapi.yaml`。
- 通知前端仓库更新 API 客户端。
```

### 3.3 `agents-docs/` 下各 MD 文件内容要点

| 文件 | 核心内容 |
|:---|:---|
| `dotnet-coding-style.md` | 命名规范（PascalCase for public, camelCase for private），async/await 模式，using 排序。 |
| `architecture-layers.md` | 依赖方向（API → Application → Domain ← Infrastructure），禁止跨层调用，明确每层允许的依赖。 |
| `repository-pattern.md` | 仓储接口设计（同步/异步方法），聚合根加载策略，工作单元（Unit of Work）模式。 |
| `api-conventions.md` | URL 命名（名词复数，层级关系），HTTP 方法对应操作，状态码标准，版本管理（URL 或 Header）。 |
| `sqlsugar-usage.md` | SqlSugar 初始化配置，仓储实现中使用 SqlSugarClient，禁止在 Domain 层引用。 |
| `error-handling.md` | 自定义业务异常（`DomainException`），全局异常中间件，返回 Problem Details。 |
| `testing.md` | xUnit 测试项目组织，使用 Moq，集成测试用 `WebApplicationFactory`。 |

---

## 四、临时文件管理规范（`temp/` 目录）

### 4.1 规则说明

1. **任何 AI 技能（Skill）或手动命令生成的临时文件、代码片段、草稿文档、测试脚本、中间数据等，必须存放在项目根目录的 `temp/` 下**。
2. **禁止将临时文件放入 `src/`、`docs/`、`agents-docs/` 等正式目录**。
3. **任务完成后，AI 必须立即删除所创建的临时文件**（例如使用 `rm temp/xxx` 或确认用户已清理）。
4. 如需保留临时文件用于跨会话，应移入正式目录并记录原因，随后删除 `temp/` 中的原副本。
5. `temp/` 已加入 `.gitignore`，确保不会意外提交。

### 4.2 在 `Agents.md` 中的声明（模板）

```markdown
## 临时文件管理
- 所有临时生成的文件（代码、文档、脚本、数据）请放入 `temp/` 目录。
- 使用完毕后立即删除。
- 该目录已加入 `.gitignore`，无需担心提交问题。
- 禁止在项目其他任何位置创建临时文件。
```

---

## 五、渐进式披露（Progressive Disclosure）设计说明

- **入口文件**：`Agents.md` 提供项目概览、全局规则和指向详细文档的引用。
- **详细规则目录**：`agents-docs/` 按主题拆分，每个文件专注一个具体方面（编码风格、架构、测试等）。AI 或人类只需在需要时读取相关文件，无需一次性加载全部内容。
- **文档目录**：`docs/` 进一步按功能划分（`requirements/`, `specs/`, `prompts/`, `database/`, `guides/`），存储长期存在的业务与技术资料。
- **效果**：降低 AI 上下文窗口压力，提高响应速度和准确性；同时便于人工维护和团队协作。

---

## 六、实施建议

1. **初始化仓库**：按照上述结构创建两个 Git 仓库，添加 `.gitignore` 并确保 `temp/` 被忽略。
2. **填充 `Agents.md`**：根据上述模板修改具体技术细节（如 .NET 版本、SqlSugar 版本等）。
3. **编写核心规则文件**：至少完成 `agents-docs/` 中的前三个最重要文件（如 `dotnet-coding-style.md`、`architecture-layers.md`、`repository-pattern.md`），其余可随开发逐步补充。
4. **配置 AI 工具**：在 Claude Code/Cursor 中指向项目根目录，确保它们能读取 `Agents.md`。
5. **团队培训**：告知所有成员 AI 临时文件规范，并在 PR 审查时注意是否有文件误入 `temp/` 或被提交。

---

## 七、总结

通过统一的前后端仓库目录结构、明确的 `Agents.md` 入口、渐进式披露的 `agents-docs/` 和 `docs/`，以及强制性的临时文件管理，你的项目将具备：
- ✅ 高 AI 可读性与协作效率  
- ✅ 清晰的架构边界  
- ✅ 可维护的技术债务控制  
- ✅ 便于新人上手的文档体系  

请将本指引作为项目开发的规范基准，并根据实际情况迭代优化。