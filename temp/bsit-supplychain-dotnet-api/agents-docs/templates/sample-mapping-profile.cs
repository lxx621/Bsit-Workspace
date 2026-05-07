// ============================================================
// 样板文件：AutoMapper Profile 完整示例
// 位置：Application/Mappings/
// 说明：以"客户(Customer)"为例，展示 Entity ↔ DTO 的标准映射配置
// ============================================================

namespace Bsit.SupplyChain.Application.Mappings;

using AutoMapper;
using Bsit.SupplyChain.Domain.Entities.Customer;
using Bsit.SupplyChain.Domain.Entities.Customer.ValueObjects;
using Bsit.SupplyChain.Application.Dtos.Customer;

/// <summary>
/// 客户模块 AutoMapper 映射配置
/// 每个业务模块一个 Profile 文件
/// </summary>
public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // ─── Entity → DTO（查询场景） ───

        CreateMap<Customer, CustomerDetailDto>()
            // 状态文本：将枚举转为中文
            .ForMember(dest => dest.StatusText,
                opt => opt.MapFrom(src => src.Status == Domain.Enums.CustomerStatus.Active ? "活跃" : "停用"))
            // 值对象属性展开到 DTO 的平面字段
            .ForMember(dest => dest.ContactPerson,
                opt => opt.MapFrom(src => src.Contact.ContactPerson))
            .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.Contact.Phone))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Contact.Email))
            .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => src.Contact.Address))
            // 状态映射为 int
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => (int)src.Status));

        // ─── DTO → Entity（写入场景，推荐用领域工厂方法替代） ───
        // 如果确实需要 DTO → Entity 映射，必须 Ignore 以下字段：

        // CreateMap<CustomerCreateDto, Customer>()
        //     .ForMember(dest => dest.Id, opt => opt.Ignore())           // ID 由工厂方法生成
        //     .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())    // 审计字段由 Aop 赋值
        //     .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())    // 审计字段由 Service 赋值
        //     .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
        //     .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
        //     .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
        //     .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
        //     .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
        //     .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());// 领域事件排除
    }
}
