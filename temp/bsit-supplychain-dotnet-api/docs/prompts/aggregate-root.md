# 提示词模板：生成聚合根

## 使用方法

将以下提示词中的 `【xxx】` 替换为实际值后发送给 AI。

## 提示词

```
请为我创建一个【模块名称】聚合根。

基本信息：
- 模块名称：【如 Supplier（供应商）】
- 数据库表名：【如 Suppliers】
- 主要属性：【如 Code(编码,string,32), Name(名称,string,100), Level(等级,枚举), ContactPhone(联系电话,string,20)】
- 值对象：【如 Address(省,市,区,详细地址) 或 "无"】
- 需要领域事件：【如 SupplierCreatedEvent 或 "无"】
- 业务方法：【如 Deactivate(停用)、UpdateLevel(调整等级) 或 "无"】

请严格参考样板文件 agents-docs/templates/sample-aggregate.cs 的格式生成。
生成完毕后对照 agents-docs/checklist.md 的 Domain 层部分自检。
```

## 示例（完整调用）

```
请为我创建一个供应商(Supplier)聚合根。

基本信息：
- 模块名称：Supplier
- 数据库表名：Suppliers
- 主要属性：Code(供应商编码,string,32), Name(供应商名称,string,100), Level(合作等级,int), ContactPhone(联系电话,string,20)
- 值对象：SupplierAddress(Province, City, District, Detail)
- 需要领域事件：SupplierCreatedEvent
- 业务方法：Deactivate(停用), UpdateLevel(调整合作等级)

请严格参考样板文件 agents-docs/templates/sample-aggregate.cs 的格式生成。
生成完毕后对照 agents-docs/checklist.md 的 Domain 层部分自检。
```
