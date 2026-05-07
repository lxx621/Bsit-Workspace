# 异常处理与统一响应规范

## 自定义异常

- `AppException`：业务异常，包含状态码和消息。业务层通过 `throw new AppException(404, "订单不存在")` 抛出。
- `ValidationException`：校验异常，包含验证错误列表。

## 全局异常中间件（ExceptionMiddleware）

位于 `Api/Middlewares/ExceptionMiddleware.cs`，职责：
1. 捕获 `AppException` → 记录 Warning 日志 → 返回对应状态码的 ApiResult
2. 捕获未处理 `Exception` → 记录 Error 日志 → 返回 500 ApiResult
3. 所有异常响应统一为 `ApiResult` JSON 格式

## 统一返回模型（ApiResult）

```json
{
  "success": false,
  "code": 404,
  "message": "订单不存在",
  "data": null
}
```

## 规则

- 控制器/服务中**不捕获异常**，让异常向上传播到中间件。
- 业务异常使用 `AppException`，系统异常由中间件统一处理。
- 异常日志通过 `ILogger` 记录（NLog 按级别写入文件/数据库）。

## 审计日志 vs 系统日志

| 类型 | 目的 | 存储 |
|------|------|------|
| 审计日志 | 业务合规、操作追溯 | AuditLogs 表 |
| 系统日志 | 开发运维排错 | 文件 + SystemLogs 表（NLog） |
