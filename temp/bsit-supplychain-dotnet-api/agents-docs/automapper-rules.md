# AutoMapper 映射规范

## 基本原则

- 所有跨层数据传输必须通过 DTO + AutoMapper，**禁止**将 Domain Entity 直接暴露给 Api 层。
- Profile 按模块拆分，放在 `Application/Mappings/` 目录下。
- 命名：`{模块名}MappingProfile.cs`

## 映射方向

- **Entity → DTO**（查询场景）：从数据库查出 Entity，映射为 DTO 返回前端
- **DTO → Entity**（写入场景）：推荐使用领域工厂方法替代直接 Map

## 必须 Ignore 的字段

- ID（由领域层生成）
- 审计字段（CreatedAt / CreatedBy / UpdatedAt / UpdatedBy）
- 领域事件（DomainEvents）

## 注册方式

```csharp
builder.Services.AddAutoMapper(
    typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
```

## 使用规范

- 构造函数注入 `IMapper`，禁止 `new Mapper()`
- `IMapper` 自动支持 `List` 集合映射
