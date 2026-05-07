# FluentValidation 验证规范

## 文件组织

- 验证器位于 `Application/Validators/{模块名}/`
- 命名：`{DtoName}Validator.cs`
- 每个需验证的 DTO 对应一个 Validator

## 注册方式

```csharp
builder.Services.AddValidatorsFromAssembly(
    typeof(Bsit.SupplyChain.Application.Services.AuthService).Assembly);
```

## 编写规范

- 错误消息必须提供中文 `.WithMessage()`
- 需查库的验证使用 `MustAsync()`
- 复杂验证逻辑使用 `Must()` 或 `Custom()`

## 示例

```csharp
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名最多50个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码至少6个字符");
    }
}
```

## MediatR Pipeline Behavior

`ValidationBehavior` 在 MediatR 管道中自动执行验证，无需在 Controller 中手动调用。
