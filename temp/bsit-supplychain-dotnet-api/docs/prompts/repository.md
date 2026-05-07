# 提示词模板：生成仓储

## 使用方法

将 `【xxx】` 替换为实际值后发送给 AI。

## 提示词

```
请为【模块名称】生成仓储接口和实现。

基本信息：
- 模块名称：【如 Supplier】
- 聚合根类型：【如 Supplier（位于 Domain/Entities/Supplier/）】
- 特有查询方法：
  - 【如 GetByCodeAsync(string code) — 根据编码查询】
  - 【如 IsCodeExistsAsync(string code, Guid? excludeId) — 编码唯一性检查】
  - 【如 GetPagedListAsync(keyword, level, pageIndex, pageSize) — 分页查询】

请严格参考样板文件 agents-docs/templates/sample-repository.cs 的格式生成。
接口放 Domain/Interfaces/，实现放 Infrastructure/Repositories/。
生成完毕后对照 agents-docs/checklist.md 的 Infrastructure 层部分自检。
```

## 示例（完整调用）

```
请为供应商(Supplier)生成仓储接口和实现。

基本信息：
- 模块名称：Supplier
- 聚合根类型：Supplier（位于 Domain/Entities/Supplier/）
- 特有查询方法：
  - GetByCodeAsync(string code) — 根据供应商编码查询
  - IsCodeExistsAsync(string code, Guid? excludeId) — 编码唯一性检查
  - GetPagedListAsync(string? keyword, int? level, int pageIndex, int pageSize) — 按关键词和等级分页查询

请严格参考样板文件 agents-docs/templates/sample-repository.cs 的格式生成。
接口放 Domain/Interfaces/，实现放 Infrastructure/Repositories/。
生成完毕后对照 agents-docs/checklist.md 的 Infrastructure 层部分自检。
```
