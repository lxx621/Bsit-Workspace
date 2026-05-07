// ============================================================
// 样板文件：DTO + FluentValidation 验证器 完整示例
// DTO 位置：Application/Dtos/{模块名}/
// Validator 位置：Application/Validators/{模块名}/
// 说明：以"客户(Customer)"为例，含 Create / Update / Query / Detail 四种 DTO
// ============================================================

// ─────────────────────────────────────────────────────────────
// 文件 1：Application/Dtos/Customer/CustomerCreateDto.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Dtos.Customer;

/// <summary>
/// 客户创建请求 DTO
/// </summary>
public class CustomerCreateDto
{
    /// <summary>客户编码</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>客户名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>联系人</summary>
    public string ContactPerson { get; set; } = string.Empty;

    /// <summary>联系电话</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>电子邮箱</summary>
    public string? Email { get; set; }

    /// <summary>联系地址</summary>
    public string? Address { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

// ─────────────────────────────────────────────────────────────
// 文件 2：Application/Dtos/Customer/CustomerUpdateDto.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Dtos.Customer;

/// <summary>
/// 客户更新请求 DTO（不含 Code，编码创建后不可修改）
/// </summary>
public class CustomerUpdateDto
{
    /// <summary>客户名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>联系人</summary>
    public string ContactPerson { get; set; } = string.Empty;

    /// <summary>联系电话</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>电子邮箱</summary>
    public string? Email { get; set; }

    /// <summary>联系地址</summary>
    public string? Address { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

// ─────────────────────────────────────────────────────────────
// 文件 3：Application/Dtos/Customer/CustomerDetailDto.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Dtos.Customer;

/// <summary>
/// 客户详情返回 DTO
/// </summary>
public class CustomerDetailDto
{
    /// <summary>客户ID</summary>
    public Guid Id { get; set; }

    /// <summary>客户编码</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>客户名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>联系人</summary>
    public string ContactPerson { get; set; } = string.Empty;

    /// <summary>联系电话</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>电子邮箱</summary>
    public string? Email { get; set; }

    /// <summary>联系地址</summary>
    public string? Address { get; set; }

    /// <summary>状态（1=活跃，2=停用）</summary>
    public int Status { get; set; }

    /// <summary>状态文本</summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
}

// ─────────────────────────────────────────────────────────────
// 文件 4：Application/Dtos/Customer/CustomerQueryDto.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Dtos.Customer;

using Bsit.SupplyChain.Application.Dtos.Common;

/// <summary>
/// 客户分页查询请求 DTO
/// 继承 PagedRequestDto 获得 PageIndex / PageSize
/// </summary>
public class CustomerQueryDto : PagedRequestDto
{
    /// <summary>搜索关键词（匹配编码或名称）</summary>
    public string? Keyword { get; set; }

    /// <summary>状态筛选（1=活跃，2=停用，null=全部）</summary>
    public int? Status { get; set; }
}

// ─────────────────────────────────────────────────────────────
// 文件 5：Application/Validators/Customer/CustomerCreateDtoValidator.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Validators.Customer;

using FluentValidation;
using Bsit.SupplyChain.Application.Dtos.Customer;

/// <summary>
/// 客户创建请求验证器
/// </summary>
public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
{
    public CustomerCreateDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("客户编码不能为空")
            .MaximumLength(32).WithMessage("客户编码最多32个字符")
            .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("客户编码仅允许字母、数字和连字符");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("客户名称不能为空")
            .MaximumLength(100).WithMessage("客户名称最多100个字符");

        RuleFor(x => x.ContactPerson)
            .NotEmpty().WithMessage("联系人不能为空")
            .MaximumLength(50).WithMessage("联系人最多50个字符");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("联系电话不能为空")
            .MaximumLength(20).WithMessage("联系电话最多20个字符");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("邮箱格式不正确")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注最多500个字符");
    }
}

// ─────────────────────────────────────────────────────────────
// 文件 6：Application/Validators/Customer/CustomerUpdateDtoValidator.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Validators.Customer;

using FluentValidation;
using Bsit.SupplyChain.Application.Dtos.Customer;

/// <summary>
/// 客户更新请求验证器
/// </summary>
public class CustomerUpdateDtoValidator : AbstractValidator<CustomerUpdateDto>
{
    public CustomerUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("客户名称不能为空")
            .MaximumLength(100).WithMessage("客户名称最多100个字符");

        RuleFor(x => x.ContactPerson)
            .NotEmpty().WithMessage("联系人不能为空")
            .MaximumLength(50).WithMessage("联系人最多50个字符");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("联系电话不能为空")
            .MaximumLength(20).WithMessage("联系电话最多20个字符");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("邮箱格式不正确")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注最多500个字符");
    }
}
