# 提示词模板：生成 DTO + FluentValidation 验证器

## 使用方法

将 `【xxx】` 替换为实际值后发送给 AI。

## 提示词

```
请为【模块名称】的【操作类型】操作生成 DTO 和验证器。

基本信息：
- 模块名称：【如 Supplier】
- 操作类型：【Create / Update / Query / Detail】
- 字段列表（格式：字段名 | 类型 | 必填 | 约束）：
  - 【如 Code | string | 是 | 最大32位，仅字母数字连字符】
  - 【如 Name | string | 是 | 最大100位】
  - 【如 Phone | string | 是 | 最大20位】
  - 【如 Email | string | 否 | 邮箱格式】
  - 【如 Remark | string | 否 | 最大500位】

请严格参考样板文件 agents-docs/templates/sample-dto-validator.cs 的格式生成。
验证器每条规则必须含中文 .WithMessage()。
生成完毕后对照 agents-docs/checklist.md 的 Application 层部分自检。
```

## 示例（完整调用）

```
请为供应商(Supplier)的创建(Create)操作生成 DTO 和验证器。

基本信息：
- 模块名称：Supplier
- 操作类型：Create
- 字段列表：
  - Code | string | 是 | 最大32位，仅允许字母、数字和连字符
  - Name | string | 是 | 最大100位
  - Level | int | 是 | 1-5之间
  - ContactPhone | string | 是 | 最大20位
  - Email | string | 否 | 邮箱格式
  - Province | string | 是 | 最大50位
  - City | string | 是 | 最大50位
  - District | string | 否 | 最大50位
  - AddressDetail | string | 否 | 最大200位
  - Remark | string | 否 | 最大500位

请严格参考样板文件 agents-docs/templates/sample-dto-validator.cs 的格式生成。
验证器每条规则必须含中文 .WithMessage()。
生成完毕后对照 agents-docs/checklist.md 的 Application 层部分自检。
```
