# 提示词模板：生成 API Controller

## 使用方法

将 `【xxx】` 替换为实际值后发送给 AI。

## 提示词

```
请为【模块名称】生成 API Controller。

基本信息：
- 模块名称：【如 Supplier】
- 服务接口：【如 ISupplierService】
- 需要认证：【是/否】
- 接口列表：
  - 【GET /{id} — 根据ID获取详情】
  - 【GET / — 分页列表查询】
  - 【POST / — 创建】
  - 【PUT /{id} — 更新】
  - 【DELETE /{id} — 删除（软删除）】

请严格参考样板文件 agents-docs/templates/sample-controller.cs 的格式生成。
生成完毕后对照 agents-docs/checklist.md 的 Api 层部分自检。
```

## 示例（完整调用）

```
请为供应商(Supplier)生成 API Controller。

基本信息：
- 模块名称：Supplier
- 服务接口：ISupplierService
- 需要认证：是
- 接口列表：
  - GET /api/supplier/{id} — 根据ID获取供应商详情
  - GET /api/supplier — 分页查询供应商列表（关键词、等级筛选）
  - POST /api/supplier — 创建供应商
  - PUT /api/supplier/{id} — 更新供应商信息
  - DELETE /api/supplier/{id} — 删除供应商（软删除）
  - PATCH /api/supplier/{id}/deactivate — 停用供应商

请严格参考样板文件 agents-docs/templates/sample-controller.cs 的格式生成。
生成完毕后对照 agents-docs/checklist.md 的 Api 层部分自检。
```
