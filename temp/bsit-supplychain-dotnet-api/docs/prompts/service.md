# 提示词模板：生成 Application Service

## 使用方法

将 `【xxx】` 替换为实际值后发送给 AI。

## 提示词

```
请为【模块名称】生成 Application Service 接口和实现。

基本信息：
- 模块名称：【如 Supplier】
- 聚合根：【如 Supplier（Domain/Entities/Supplier/）】
- 仓储接口：【如 ISupplierRepository】
- DTO 列表：【如 SupplierCreateDto, SupplierUpdateDto, SupplierDetailDto, SupplierQueryDto】
- 方法列表：
  - 【GetByIdAsync(Guid id) — 获取详情】
  - 【GetPagedListAsync(query) — 分页列表】
  - 【CreateAsync(dto) — 创建】
  - 【UpdateAsync(id, dto) — 更新】
  - 【DeleteAsync(id) — 软删除】

请严格参考样板文件 agents-docs/templates/sample-service.cs 的格式生成。
写入操作必须使用 UnitOfWork 模式，设置审计字段。
生成完毕后对照 agents-docs/checklist.md 的 Application 层部分自检。
```

## 示例（完整调用）

```
请为供应商(Supplier)生成 Application Service 接口和实现。

基本信息：
- 模块名称：Supplier
- 聚合根：Supplier（Domain/Entities/Supplier/）
- 仓储接口：ISupplierRepository
- DTO 列表：SupplierCreateDto, SupplierUpdateDto, SupplierDetailDto, SupplierQueryDto
- 方法列表：
  - GetByIdAsync(Guid id) — 获取供应商详情
  - GetPagedListAsync(SupplierQueryDto query) — 分页查询供应商列表
  - CreateAsync(SupplierCreateDto dto) — 创建供应商（含编码唯一性校验）
  - UpdateAsync(Guid id, SupplierUpdateDto dto) — 更新供应商信息
  - DeleteAsync(Guid id) — 软删除供应商
  - DeactivateAsync(Guid id) — 停用供应商

请严格参考样板文件 agents-docs/templates/sample-service.cs 的格式生成。
写入操作必须使用 UnitOfWork 模式，设置审计字段。
生成完毕后对照 agents-docs/checklist.md 的 Application 层部分自检。
```
