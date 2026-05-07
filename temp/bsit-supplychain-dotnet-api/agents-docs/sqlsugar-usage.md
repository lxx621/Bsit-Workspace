# SqlSugar ORM 使用规范

## 基本原则

- SqlSugar 相关代码**仅允许出现在 Infrastructure 层**。
- Domain 层通过仓储接口抽象数据访问，不依赖 SqlSugar。
- SqlSugarClient 注册为 Scoped（每个 HTTP 请求一个实例）。

## 初始化配置（SqlSugarSetup.cs）

位于 `Infrastructure/DbContext/SqlSugarSetup.cs`，负责：
1. 创建 SqlSugarClient 实例（连接字符串从 IConfiguration 获取）
2. 配置审计字段自动赋值（Aop.DataExecuting 拦截器）
3. 配置软删除全局过滤器（QueryFilter）
4. 配置 SQL 执行日志（开发环境）

## 审计字段自动赋值

- `CreatedAt`：Insert 时由 `Aop.DataExecuting` 自动设置为当前时间
- `UpdatedAt`：Update 时由 `Aop.DataExecuting` 自动设置为当前时间
- `CreatedBy` / `UpdatedBy`：需在 Service 层通过 `ICurrentUser.UserId` 手动赋值

## 软删除

- `IsDeleted` 全局过滤器自动排除已删除记录
- 需查询已删除数据时使用 `db.Queryable<T>().ClearFilter()` 临时禁用
- 软删除操作通过 `BaseRepository.SoftDeleteAsync()` 执行

## 乐观并发

- `RowVersion` 字段由 SqlSugar `IsEnableUpdateVersionValidation` 自动管理
- Update 时自动校验版本号，冲突时抛出异常
