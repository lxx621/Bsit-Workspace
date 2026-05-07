// ============================================================
// 样板文件：Application Service 接口 + 实现 完整示例
// 接口位置：Application/Interfaces/
// 实现位置：Application/Services/
// 说明：以"客户(Customer)"为例，展示 UnitOfWork + AutoMapper + ICurrentUser 的标准用法
// ============================================================

// ─────────────────────────────────────────────────────────────
// 文件 1：Application/Interfaces/ICustomerService.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Interfaces;

using Bsit.SupplyChain.Application.Dtos.Customer;
using Bsit.SupplyChain.Application.Dtos.Common;
using Bsit.SupplyChain.Common.Models;

/// <summary>
/// 客户业务服务接口
/// 定义客户模块的应用层用例
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// 根据ID查询客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>统一返回结果，包含客户详情 DTO</returns>
    Task<ApiResult<CustomerDetailDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// 分页查询客户列表
    /// </summary>
    /// <param name="query">分页查询参数</param>
    /// <returns>统一返回结果，包含分页列表</returns>
    Task<ApiResult<PagedResultDto<CustomerDetailDto>>> GetPagedListAsync(CustomerQueryDto query);

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>统一返回结果，包含新创建的客户详情</returns>
    Task<ApiResult<CustomerDetailDto>> CreateAsync(CustomerCreateDto dto);

    /// <summary>
    /// 更新客户信息
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>统一返回结果</returns>
    Task<ApiResult> UpdateAsync(Guid id, CustomerUpdateDto dto);

    /// <summary>
    /// 删除客户（软删除）
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>统一返回结果</returns>
    Task<ApiResult> DeleteAsync(Guid id);
}

// ─────────────────────────────────────────────────────────────
// 文件 2：Application/Services/CustomerService.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.Services;

using AutoMapper;
using Microsoft.Extensions.Logging;
using Bsit.SupplyChain.Application.Dtos.Customer;
using Bsit.SupplyChain.Application.Dtos.Common;
using Bsit.SupplyChain.Application.Interfaces;
using Bsit.SupplyChain.Common.Exceptions;
using Bsit.SupplyChain.Common.Models;
using Bsit.SupplyChain.Domain.Entities.Customer;
using Bsit.SupplyChain.Domain.Entities.Customer.ValueObjects;
using Bsit.SupplyChain.Domain.Interfaces;

/// <summary>
/// 客户业务服务实现
/// 职责：编排客户模块的业务用例，协调仓储、映射、事务
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CustomerService> _logger;

    /// <summary>
    /// 构造函数（依赖注入）
    /// </summary>
    /// <param name="customerRepo">客户仓储</param>
    /// <param name="unitOfWork">工作单元（事务 + 事件分发）</param>
    /// <param name="mapper">AutoMapper 实例</param>
    /// <param name="currentUser">当前登录用户信息</param>
    /// <param name="logger">日志记录器</param>
    public CustomerService(
        ICustomerRepository customerRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<CustomerService> logger)
    {
        _customerRepo = customerRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ApiResult<CustomerDetailDto>> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null)
            throw new AppException(404, "客户不存在");

        var dto = _mapper.Map<CustomerDetailDto>(customer);
        return ApiResult<CustomerDetailDto>.Ok(dto);
    }

    /// <inheritdoc />
    public async Task<ApiResult<PagedResultDto<CustomerDetailDto>>> GetPagedListAsync(CustomerQueryDto query)
    {
        var (items, totalCount) = await _customerRepo.GetPagedListAsync(
            query.Keyword, query.Status, query.PageIndex, query.PageSize);

        var dtoList = _mapper.Map<List<CustomerDetailDto>>(items);

        var result = new PagedResultDto<CustomerDetailDto>
        {
            Items = dtoList,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };

        return ApiResult<PagedResultDto<CustomerDetailDto>>.Ok(result);
    }

    /// <inheritdoc />
    public async Task<ApiResult<CustomerDetailDto>> CreateAsync(CustomerCreateDto dto)
    {
        // 1. 业务校验：编码唯一性
        if (await _customerRepo.IsCodeExistsAsync(dto.Code))
            throw new AppException(400, $"客户编码 '{dto.Code}' 已存在");

        // 2. 通过领域工厂方法创建聚合根
        var contact = new ContactInfo(dto.ContactPerson, dto.Phone, dto.Email, dto.Address);
        var customer = Customer.Create(dto.Code, dto.Name, contact, dto.Remark);

        // 3. 设置审计字段（CreatedBy 需手动赋值，CreatedAt 由 SqlSugar Aop 自动赋值）
        customer.CreatedBy = _currentUser.UserId;

        // 4. 使用 UnitOfWork 管理事务 + 领域事件
        _unitOfWork.BeginTran();
        try
        {
            await _customerRepo.AddAsync(customer);
            _unitOfWork.CollectEvents(customer);       // 收集领域事件
            await _unitOfWork.CommitAsync();            // 提交事务 + 发布事件

            _logger.LogInformation("客户创建成功，ID：{CustomerId}，编码：{Code}", customer.Id, customer.Code);
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }

        var resultDto = _mapper.Map<CustomerDetailDto>(customer);
        return ApiResult<CustomerDetailDto>.Ok(resultDto, "创建成功");
    }

    /// <inheritdoc />
    public async Task<ApiResult> UpdateAsync(Guid id, CustomerUpdateDto dto)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null)
            throw new AppException(404, "客户不存在");

        // 通过聚合根方法更新（保持领域逻辑封装）
        var contact = new ContactInfo(dto.ContactPerson, dto.Phone, dto.Email, dto.Address);
        customer.UpdateInfo(dto.Name, contact, dto.Remark);
        customer.UpdatedBy = _currentUser.UserId;

        _unitOfWork.BeginTran();
        try
        {
            await _customerRepo.UpdateAsync(customer);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }

        return ApiResult.Ok("更新成功");
    }

    /// <inheritdoc />
    public async Task<ApiResult> DeleteAsync(Guid id)
    {
        var affected = await _customerRepo.SoftDeleteAsync(id);
        if (affected == 0)
            throw new AppException(404, "客户不存在");

        _logger.LogInformation("客户已软删除，ID：{CustomerId}", id);
        return ApiResult.Ok("删除成功");
    }
}
