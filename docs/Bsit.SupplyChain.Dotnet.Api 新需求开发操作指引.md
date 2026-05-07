# Bsit.SupplyChain.Api 新需求开发操作指引

---

**适用场景**：当有一个新的业务需求要落地开发时，按本指引从"需求到代码"完成全流程操作。  
**前置条件**：项目已按《AI 开发项目目录规划指引》完成初始化（Agents.md、agents-docs/、docs/ 等目录已就位）。

---

## 一、总览流程

```
需求文档 → 规则检查/补充 → 提示词准备 → AI 工作流执行 → 自检 → 测试 → 提交
   ①          ②              ③            ④            ⑤     ⑥     ⑦
```

---

## 二、详细步骤

### 步骤 ①：编写需求文档

**做什么**：将业务需求整理成结构化的 Markdown 文档。

**放在哪**：`docs/requirements/{模块名}.md`

**内容模板**：

```markdown
# {模块名} 业务需求

## 1. 业务背景
简述为什么需要这个模块，解决什么业务问题。

## 2. 核心概念
- 聚合名称：{如 Supplier}
- 数据库表名：{如 Suppliers}
- 所属业务域：{如 采购管理}

## 3. 数据字段

| 字段名 | 中文名 | 类型 | 必填 | 约束/说明 |
|--------|--------|------|------|-----------|
| Code | 编码 | string | 是 | 最大32位，唯一 |
| Name | 名称 | string | 是 | 最大100位 |
| ... | ... | ... | ... | ... |

## 4. 业务规则
- 编码创建后不可修改
- 停用后不可重复停用
- ...

## 5. 接口需求

| 接口 | 方法 | URL | 说明 |
|------|------|-----|------|
| 查询详情 | GET | /api/suppliers/{id} | 返回供应商详情 |
| 分页列表 | GET | /api/suppliers | 支持关键词、状态筛选 |
| 创建 | POST | /api/suppliers | 创建新供应商 |
| 更新 | PUT | /api/suppliers/{id} | 更新供应商信息 |
| 删除 | DELETE | /api/suppliers/{id} | 软删除 |

## 6. 值对象（如有）
- ContactInfo(ContactPerson, Phone, Email, Address)

## 7. 领域事件（如有）
- SupplierCreatedEvent：创建供应商后触发
```

**小贴士**：需求文档越结构化，后续 AI 生成代码的质量越高。

---

### 步骤 ②：检查并补充规则文件

**做什么**：检查现有 `agents-docs/` 规则是否覆盖新需求场景，如不够则补充。

**检查清单**：

| 检查项 | 常见需补充情况 | 操作 |
|--------|---------------|------|
| 聚合根是否有新模式？ | 如需多对多关联、聚合嵌套 | 更新 `agents-docs/architecture-layers.md` 或新增专题文档 |
| 是否有新的基础设施依赖？ | 如引入 Redis、消息队列 | 新增 `agents-docs/redis-usage.md` 等 |
| 是否有新的 API 模式？ | 如文件上传、批量操作 | 更新 `agents-docs/api-conventions.md` |
| 是否有新的验证模式？ | 如需查库校验唯一性 | 更新 `agents-docs/validation-rules.md` 示例 |
| 错误码是否需要扩展？ | 新增业务特有错误码 | 更新 `docs/api/error-codes.md` |

**通常不需要改的**：
- `Agents.md` 中的通用规则（语言/编码/时间等）
- `agents-docs/templates/` 样板文件（除非发现样板有误或需扩展新模式）
- `agents-docs/checklist.md`（除非增加了新的检查维度）

---

### 步骤 ③：准备提示词

**做什么**：基于需求文档，用 `docs/prompts/` 中的模板准备实际的 AI 提示词。

**两种方式**（选其一）：

#### 方式 A：使用工作流（推荐，适合完整模块）

如果需要创建一个完整的业务模块（聚合根到控制器），直接告诉 AI：

```
请按照 .windsurf/workflows/new-module.md 工作流，
根据 docs/requirements/{模块名}.md 的需求文档，
创建 {模块名} 的完整业务模块。
```

AI 会自动按 11 步工作流依次读取规则文件、参考样板、生成代码。

#### 方式 B：使用提示词模板（适合单个文件或局部功能）

1. 打开对应的提示词模板（`docs/prompts/` 下）
2. 将 `【xxx】` 替换为需求文档中的实际值
3. 发送给 AI

**提示词模板对照表**：

| 我要做什么 | 使用模板 |
|------------|----------|
| 新建聚合根 | `docs/prompts/aggregate-root.md` |
| 新建仓储 | `docs/prompts/repository.md` |
| 新建 Service | `docs/prompts/service.md` |
| 新建 Controller | `docs/prompts/controller.md` |
| 新建 DTO + Validator | `docs/prompts/dto-validator.md` |

---

### 步骤 ④：执行 AI 工作流

**做什么**：让 AI 开始生成代码。

**执行前确认**：
- [ ] 需求文档已就位（`docs/requirements/{模块名}.md`）
- [ ] 规则文件已就绪（步骤 ② 的检查已完成）
- [ ] AI 工具已连接到项目根目录，可读取 `Agents.md`

**执行过程中 AI 的行为**：
1. AI 读取 `Agents.md` → 获取全局规则和任务路由表
2. AI 根据任务路由表读取对应的 `agents-docs/` 规则文件
3. AI 读取 `agents-docs/templates/` 中的样板文件
4. AI 读取 `docs/requirements/{模块名}.md` 获取需求细节
5. AI 按工作流步骤逐一生成代码文件
6. AI 对照 `agents-docs/checklist.md` 自检

**如果 AI 执行中出错**：
- 检查规则文件是否有矛盾或遗漏
- 检查需求文档是否足够清晰
- 必要时在 `agents-docs/` 中补充说明

---

### 步骤 ⑤：人工审查 + 自检

**做什么**：AI 生成代码后，开发人员进行人工审查。

**审查要点**：

1. **分层合规**：打开 `agents-docs/checklist.md`，逐项核对
2. **业务逻辑**：对照需求文档检查业务规则是否正确实现
3. **命名规范**：检查类名、方法名、文件路径是否符合 `dotnet-coding-style.md`
4. **安全**：检查认证注解、权限控制是否到位
5. **临时文件**：确认 `temp/` 目录中无残留文件

---

### 步骤 ⑥：编写/补充测试

**做什么**：为新生成的代码编写单元测试和集成测试。

**参考**：`agents-docs/testing.md`

**可让 AI 辅助**：

```
请为 {模块名}Service 编写单元测试，
参考 agents-docs/testing.md 的规范，
覆盖以下场景：
- GetByIdAsync: 存在 → 返回详情 / 不存在 → 抛 404
- CreateAsync: 正常创建 / 编码重复 → 抛 400
- DeleteAsync: 存在 → 软删除成功 / 不存在 → 抛 404
```

---

### 步骤 ⑦：提交代码

**做什么**：代码审查通过后提交 Git。

**提交前检查清单**：
- [ ] `temp/` 目录已清空
- [ ] 所有新文件编码为无 BOM UTF-8
- [ ] 需求文档已提交（`docs/requirements/{模块名}.md`）
- [ ] 如有新增错误码，`docs/api/error-codes.md` 已更新
- [ ] 如有 API 变更，通知前端团队

**Git 提交规范**：

```
feat(supplier): 新增供应商管理模块

- Domain: Customer 聚合根、值对象、领域事件
- Infrastructure: CustomerRepository 仓储实现
- Application: CustomerService、DTO、Validator、Mapping、EventHandler
- Api: CustomerController（CRUD + 停用接口）
```

---

## 三、文件变更速查表

一个完整的新模块开发，通常涉及以下文件的**新增或修改**：

### 新增文件（AI 生成）

| 层 | 文件路径 | 说明 |
|----|----------|------|
| 需求 | `docs/requirements/{模块名}.md` | 需求文档（人工编写） |
| Domain | `src/.../Domain/Entities/{模块}/` | 聚合根、值对象、常量 |
| Domain | `src/.../Domain/Events/{模块}CreatedEvent.cs` | 领域事件 |
| Domain | `src/.../Domain/Interfaces/I{模块}Repository.cs` | 仓储接口 |
| Infrastructure | `src/.../Infrastructure/Repositories/{模块}Repository.cs` | 仓储实现 |
| Application | `src/.../Application/Dtos/{模块}/` | DTO（Create/Update/Detail/Query） |
| Application | `src/.../Application/Validators/{模块}/` | Validator |
| Application | `src/.../Application/Mappings/{模块}MappingProfile.cs` | 映射配置 |
| Application | `src/.../Application/Interfaces/I{模块}Service.cs` | 服务接口 |
| Application | `src/.../Application/Services/{模块}Service.cs` | 服务实现 |
| Application | `src/.../Application/EventHandlers/` | 事件处理程序 |
| Api | `src/.../Api/Controllers/{模块}Controller.cs` | 控制器 |
| Tests | `src/tests/.../` | 单元/集成测试 |

### 可能修改的文件

| 文件 | 修改原因 |
|------|----------|
| `docs/api/error-codes.md` | 新增模块特有错误码 |
| `agents-docs/` 下某规则文件 | 新需求引入新的技术模式 |
| `docs/specs/openapi.yaml` | 新增 API 接口定义 |

---

## 四、常见问题

### Q1：需求很小（只加一个接口），也要走全流程吗？

不需要。如果只是为已有模块添加接口，跳过步骤 ①（需求文档可简化），直接用"方式 B 单个提示词"或告诉 AI 执行 `.windsurf/workflows/new-api-endpoint.md` 工作流。

### Q2：AI 生成的代码不符合预期怎么办？

1. 检查需求文档是否描述清晰（字段、规则、接口）
2. 检查 `agents-docs/` 规则文件是否有歧义
3. 让 AI 重新读取对应的样板文件（`agents-docs/templates/`）
4. 明确指出需要修正的部分，AI 会根据规则文件修正

### Q3：新需求的技术方案现有规则不覆盖怎么办？

1. 在 `agents-docs/` 下新建专题规则文件（如 `redis-cache.md`、`file-upload.md`）
2. 在 `Agents.md` 的任务路由表中添加对应条目
3. 如有通用性，考虑同步更新 `agents-docs/templates/` 增加样板
4. 更新 `agents-docs/checklist.md` 增加新的检查项

### Q4：多人协作时如何避免冲突？

1. 需求文档先提交，确保团队对需求理解一致
2. 模块间尽量独立（不同聚合各自开发，减少 merge 冲突）
3. 规则文件的修改需团队评审（通过 PR 审查）
4. `temp/` 目录不提交，各开发者独立使用

---

## 五、快速上手示例

以"新建供应商管理模块"为例：

```
1. 我编写 docs/requirements/supplier.md（字段、规则、接口）

2. 我检查 agents-docs/ 规则 → 当前规则足够覆盖，无需修改

3. 我对 AI 说：
   "请按照 .windsurf/workflows/new-module.md 工作流，
    根据 docs/requirements/supplier.md 的需求文档，
    创建供应商(Supplier)的完整业务模块。"

4. AI 自动执行：
   - 读取 Agents.md → 获取路由表
   - 读取规则文件 + 样板文件
   - 按 11 步创建 17 个文件
   - 对照 checklist.md 自检

5. 我审查 AI 生成的代码 → 修正问题

6. 我让 AI 补充单元测试

7. 我提交代码 + 需求文档
```

---

## 六、总结

| 阶段 | 人做什么 | AI 做什么 | 输出物 |
|------|----------|-----------|--------|
| **需求准备** | 编写结构化需求文档 | — | `docs/requirements/{模块名}.md` |
| **规则检查** | 检查/补充 agents-docs 规则 | — | 更新后的规则文件 |
| **提示词准备** | 选择工作流或填写提示词模板 | — | AI 指令 |
| **代码生成** | 监督 + 提供反馈 | 读取规则 → 参考样板 → 生成代码 → 自检 | 17+ 个代码文件 |
| **审查测试** | 人工审查 + 发起测试 | 辅助编写测试 | 通过审查的代码 + 测试 |
| **提交上线** | Git 提交 + 通知前端 | — | 完成 |
