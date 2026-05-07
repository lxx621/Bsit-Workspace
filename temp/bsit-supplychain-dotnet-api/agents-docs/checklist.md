# AI 代码生成自检清单

每次生成或修改代码后，必须逐项核实以下内容。未通过的项必须修正后才能完成任务。

---

## 通用规则

- [ ] 文件编码为无 BOM 的 UTF-8
- [ ] 所有公共类、方法、属性包含中文 XML 注释（用途、参数、返回值、异常说明）
- [ ] 关键业务逻辑处有行内注释说明核心逻辑
- [ ] 命名规范：PascalCase（公共）/ _camelCase（私有字段）/ camelCase（局部变量）
- [ ] 异步方法以 `Async` 后缀命名
- [ ] 无硬编码魔法数字（使用常量或配置）
- [ ] 无临时文件残留在 `src/`、`docs/`、`agents-docs/` 中

## 分层架构

- [ ] Domain 层未引用 SqlSugar、Infrastructure、Api
- [ ] Application 层未引用 Infrastructure
- [ ] Common 层未引用任何业务层
- [ ] 依赖方向正确：上层 → 下层，无反向依赖
- [ ] 跨层通信通过接口（依赖注入），无直接 new 实例

## Domain 层

- [ ] 聚合根继承 `BaseEntity`
- [ ] 使用 `[SugarTable]` 和 `[SugarColumn]` 特性
- [ ] 包含私有无参构造函数（ORM 需要）
- [ ] 属性使用 `private set`
- [ ] 静态工厂方法 `Create()` 内添加领域事件
- [ ] 值对象使用 `record` 类型
- [ ] 领域事件使用 `record` + `INotification`，命名过去式

## Infrastructure 层

- [ ] 仓储继承 `BaseRepository<T>` 并实现对应接口
- [ ] 仅使用 SqlSugar API 操作数据
- [ ] 分页查询使用 `RefAsync<int>` + `ToPageListAsync`

## Application 层

- [ ] DTO 放在 `Dtos/{模块名}/` 下
- [ ] 每个需验证的 DTO 有对应的 Validator
- [ ] Validator 每条规则含中文 `.WithMessage()`
- [ ] MappingProfile 按模块拆分，放在 `Mappings/`
- [ ] Entity→DTO 映射中，值对象属性正确展开
- [ ] DTO→Entity 映射（如有）中，ID/审计字段/DomainEvents 已 Ignore
- [ ] Service 注入 IRepository / IUnitOfWork / IMapper / ICurrentUser / ILogger
- [ ] 写入操作使用 UnitOfWork：BeginTran → 操作 → CollectEvents → CommitAsync（catch → Rollback）
- [ ] 设置审计字段：`entity.CreatedBy = _currentUser.UserId`
- [ ] 业务异常使用 `throw new AppException(statusCode, message)`

## Api 层

- [ ] Controller 使用 `[ApiController]` + `[Route("api/[controller]")]`
- [ ] 需认证的接口添加 `[Authorize]`
- [ ] 返回值使用 `ApiResult<T>` 或 `ApiResult`
- [ ] Swagger 注解 `[ProducesResponseType]` 完整
- [ ] RESTful URL：名词复数、小写、连字符分隔

## 测试（如涉及）

- [ ] 测试类命名：`{被测类名}Tests.cs`
- [ ] 测试方法命名：`{方法名}_{场景}_{预期结果}`
- [ ] 未删除或弱化已有测试
