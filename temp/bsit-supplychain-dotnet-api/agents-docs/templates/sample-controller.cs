// ============================================================
// 样板文件：API Controller 完整示例
// 位置：Api/Controllers/Customer/
// 说明：以“客户(Customer)”为例，展示 RESTful + ApiResult + Swagger + JWT 的标准用法
// 规范：Controller 按业务模块子目录组织，命名空间跟随目录
// ============================================================

namespace Bsit.SupplyChain.Api.Controllers.Customer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bsit.SupplyChain.Application.Dtos.Customer;
using Bsit.SupplyChain.Application.Dtos.Common;
using Bsit.SupplyChain.Application.Interfaces.Customer;
using Bsit.SupplyChain.Common.Models;

/// <summary>
/// 客户管理接口
/// 提供客户的 CRUD 操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    /// <summary>
    /// 构造函数，注入客户服务
    /// </summary>
    /// <param name="customerService">客户业务服务</param>
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID（GUID）</param>
    /// <returns>客户详情</returns>
    /// <response code="200">查询成功</response>
    /// <response code="404">客户不存在</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<CustomerDetailDto>), 200)]
    public async Task<ApiResult<CustomerDetailDto>> GetById(Guid id)
    {
        return await _customerService.GetByIdAsync(id);
    }

    /// <summary>
    /// 分页查询客户列表
    /// </summary>
    /// <param name="query">查询参数（关键词、状态、分页）</param>
    /// <returns>分页列表</returns>
    /// <response code="200">查询成功</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<CustomerDetailDto>>), 200)]
    public async Task<ApiResult<PagedResultDto<CustomerDetailDto>>> GetPagedList(
        [FromQuery] CustomerQueryDto query)
    {
        return await _customerService.GetPagedListAsync(query);
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的客户详情</returns>
    /// <response code="200">创建成功</response>
    /// <response code="400">参数验证失败或编码重复</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<CustomerDetailDto>), 200)]
    public async Task<ApiResult<CustomerDetailDto>> Create([FromBody] CustomerCreateDto dto)
    {
        return await _customerService.CreateAsync(dto);
    }

    /// <summary>
    /// 更新客户信息
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">客户不存在</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    public async Task<ApiResult> Update(Guid id, [FromBody] CustomerUpdateDto dto)
    {
        return await _customerService.UpdateAsync(id, dto);
    }

    /// <summary>
    /// 删除客户（软删除）
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>操作结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">客户不存在</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult), 200)]
    public async Task<ApiResult> Delete(Guid id)
    {
        return await _customerService.DeleteAsync(id);
    }
}
