---
description: 为已有模块添加新的 API 接口（如新增一个查询接口或操作接口）
---

# 新增 API 接口工作流

## 前置准备

1. 确认信息：
   - 所属模块名称
   - 接口功能描述
   - HTTP 方法与 URL
   - 请求参数 / 返回数据

2. 读取规则文件：
   - `agents-docs/api-conventions.md`
   - `agents-docs/validation-rules.md`

## 执行步骤

### 步骤 1：新增/修改 DTO

- 如需新的请求/响应 DTO，在 `Application/Dtos/{模块名}/` 下创建
- 如需验证器，在 `Application/Validators/{模块名}/` 下创建

### 步骤 2：更新 Service 接口

- 在 `Application/Interfaces/I{模块名}Service.cs` 中添加方法签名

### 步骤 3：实现 Service 方法

- 在 `Application/Services/{模块名}Service.cs` 中实现
- 写入操作使用 UnitOfWork 模式

### 步骤 4：如需新查询，更新仓储

- 在 `Domain/Interfaces/I{模块名}Repository.cs` 中添加方法
- 在 `Infrastructure/Repositories/{模块名}Repository.cs` 中实现

### 步骤 5：更新 Mapping（如有新 DTO）

- 在 `Application/Mappings/{模块名}MappingProfile.cs` 中添加映射

### 步骤 6：添加 Controller Action

- 在 `Api/Controllers/{模块名}Controller.cs` 中添加新端点
- 添加 Swagger 注解

### 步骤 7：自检

- 对照 `agents-docs/checklist.md` 核实新增代码
