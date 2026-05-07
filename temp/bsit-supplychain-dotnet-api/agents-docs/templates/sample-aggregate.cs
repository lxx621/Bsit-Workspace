// ============================================================
// 样板文件：聚合根 + 实体 + 值对象 完整示例
// 位置：Domain/Entities/{聚合名称}/
// 说明：以"客户(Customer)"聚合为例，展示标准的 DDD 聚合组织方式
// ============================================================

// ─────────────────────────────────────────────────────────────
// 文件 1：Domain/Entities/Customer/Customer.cs（聚合根）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Domain.Entities.Customer;

using SqlSugar;
using Bsit.SupplyChain.Domain.Events;

/// <summary>
/// 客户聚合根
/// 职责：管理客户基本信息、联系方式，封装客户相关业务规则
/// 数据库表：Customers
/// </summary>
[SugarTable("Customers")]
public class Customer : BaseEntity
{
    /// <summary>客户唯一标识（主键，GUID）</summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; private set; }

    /// <summary>客户编码（业务编号，唯一）</summary>
    [SugarColumn(Length = 32)]
    public string Code { get; private set; } = string.Empty;

    /// <summary>客户名称</summary>
    [SugarColumn(Length = 100)]
    public string Name { get; private set; } = string.Empty;

    /// <summary>联系方式（值对象，JSON 存储）</summary>
    [SugarColumn(IsJson = true)]
    public ContactInfo Contact { get; private set; } = default!;

    /// <summary>客户状态（枚举）</summary>
    public CustomerStatus Status { get; private set; }

    /// <summary>备注信息</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; private set; }

    /// <summary>ORM 所需的私有无参构造函数</summary>
    private Customer() { }

    /// <summary>
    /// 静态工厂方法：创建客户
    /// 核心逻辑：校验必填字段 → 初始化实体 → 添加领域事件
    /// </summary>
    /// <param name="code">客户编码（不可为空）</param>
    /// <param name="name">客户名称（不可为空）</param>
    /// <param name="contact">联系方式值对象</param>
    /// <param name="remark">备注（可选）</param>
    /// <returns>新创建的客户实体</returns>
    /// <exception cref="ArgumentException">当 code 或 name 为空时抛出</exception>
    public static Customer Create(string code, string name, ContactInfo contact, string? remark = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("客户编码不能为空", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("客户名称不能为空", nameof(name));

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Code = code.Trim(),
            Name = name.Trim(),
            Contact = contact,
            Status = CustomerStatus.Active,
            Remark = remark
        };

        // 添加领域事件（在 UnitOfWork 提交后分发）
        customer.AddDomainEvent(new CustomerCreatedEvent(customer.Id, customer.Code, customer.Name, DateTime.UtcNow));

        return customer;
    }

    /// <summary>
    /// 更新客户信息
    /// </summary>
    /// <param name="name">新名称</param>
    /// <param name="contact">新联系方式</param>
    /// <param name="remark">新备注</param>
    public void UpdateInfo(string name, ContactInfo contact, string? remark)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("客户名称不能为空", nameof(name));

        Name = name.Trim();
        Contact = contact;
        Remark = remark;
    }

    /// <summary>
    /// 停用客户
    /// 业务规则：已停用的客户不可重复停用
    /// </summary>
    public void Deactivate()
    {
        if (Status == CustomerStatus.Inactive)
            throw new InvalidOperationException("客户已处于停用状态");

        Status = CustomerStatus.Inactive;
    }
}

// ─────────────────────────────────────────────────────────────
// 文件 2：Domain/Entities/Customer/ValueObjects/ContactInfo.cs（值对象）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Domain.Entities.Customer.ValueObjects;

/// <summary>
/// 联系方式值对象（不可变）
/// 由值定义相等性，无独立标识
/// </summary>
public record ContactInfo(
    /// <summary>联系人姓名</summary>
    string ContactPerson,
    /// <summary>联系电话</summary>
    string Phone,
    /// <summary>电子邮箱（可选）</summary>
    string? Email,
    /// <summary>联系地址（可选）</summary>
    string? Address
);

// ─────────────────────────────────────────────────────────────
// 文件 3：Domain/Entities/Customer/CustomerConstants.cs（聚合常量）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Domain.Entities.Customer;

/// <summary>
/// 客户聚合常量定义
/// </summary>
public static class CustomerConstants
{
    /// <summary>客户编码最大长度</summary>
    public const int CodeMaxLength = 32;

    /// <summary>客户名称最大长度</summary>
    public const int NameMaxLength = 100;

    /// <summary>备注最大长度</summary>
    public const int RemarkMaxLength = 500;
}

// ─────────────────────────────────────────────────────────────
// 文件 4：Domain/Enums/CustomerStatus.cs（枚举，如果跨聚合共享放 Enums/）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Domain.Enums;

/// <summary>
/// 客户状态枚举
/// </summary>
public enum CustomerStatus
{
    /// <summary>活跃</summary>
    Active = 1,

    /// <summary>停用</summary>
    Inactive = 2
}
