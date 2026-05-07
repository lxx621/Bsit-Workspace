// ============================================================
// 样板文件：仓储接口 + 仓储实现 完整示例
// 接口位置：Domain/Interfaces/
// 实现位置：Infrastructure/Repositories/
// 说明：以"客户(Customer)"为例
// ============================================================

// ─────────────────────────────────────────────────────────────
// 文件 1：Domain/Interfaces/ICustomerRepository.cs（仓储接口）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Domain.Interfaces;

using Bsit.SupplyChain.Domain.Entities.Customer;

/// <summary>
/// 客户仓储接口
/// 继承 IRepository&lt;Customer&gt; 获得通用 CRUD，并扩展聚合特有查询
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// 根据客户编码查询客户
    /// </summary>
    /// <param name="code">客户编码</param>
    /// <returns>客户实体，不存在返回 null</returns>
    Task<Customer?> GetByCodeAsync(string code);

    /// <summary>
    /// 检查客户编码是否已存在
    /// </summary>
    /// <param name="code">客户编码</param>
    /// <param name="excludeId">排除的客户ID（编辑时排除自身）</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null);

    /// <summary>
    /// 分页查询客户列表
    /// </summary>
    /// <param name="keyword">搜索关键词（匹配编码或名称）</param>
    /// <param name="status">状态筛选（可选）</param>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>分页结果（列表 + 总数）</returns>
    Task<(List<Customer> Items, int TotalCount)> GetPagedListAsync(
        string? keyword, int? status, int pageIndex, int pageSize);
}

// ─────────────────────────────────────────────────────────────
// 文件 2：Infrastructure/Repositories/CustomerRepository.cs（仓储实现）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Infrastructure.Repositories;

using SqlSugar;
using Bsit.SupplyChain.Domain.Entities.Customer;
using Bsit.SupplyChain.Domain.Interfaces;

/// <summary>
/// 客户仓储实现
/// 继承 BaseRepository&lt;Customer&gt; 获得通用 CRUD
/// 使用 SqlSugar 实现聚合特有查询
/// </summary>
public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    /// <summary>
    /// 构造函数，注入 SqlSugar 客户端
    /// </summary>
    /// <param name="db">SqlSugar 客户端实例（Scoped 生命周期）</param>
    public CustomerRepository(ISqlSugarClient db) : base(db) { }

    /// <inheritdoc />
    public async Task<Customer?> GetByCodeAsync(string code)
    {
        return await Db.Queryable<Customer>()
            .Where(c => c.Code == code)
            .FirstAsync();
    }

    /// <inheritdoc />
    public async Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null)
    {
        return await Db.Queryable<Customer>()
            .Where(c => c.Code == code)
            .WhereIF(excludeId.HasValue, c => c.Id != excludeId!.Value)
            .AnyAsync();
    }

    /// <inheritdoc />
    public async Task<(List<Customer> Items, int TotalCount)> GetPagedListAsync(
        string? keyword, int? status, int pageIndex, int pageSize)
    {
        // 总数变量，SqlSugar 的 ToPageListAsync 通过 RefAsync 输出
        var totalCount = new RefAsync<int>();

        var list = await Db.Queryable<Customer>()
            // 关键词模糊搜索（编码或名称）
            .WhereIF(!string.IsNullOrWhiteSpace(keyword),
                c => c.Code.Contains(keyword!) || c.Name.Contains(keyword!))
            // 状态筛选
            .WhereIF(status.HasValue, c => (int)c.Status == status!.Value)
            // 排序：创建时间倒序
            .OrderByDescending(c => c.CreatedAt)
            // 分页查询
            .ToPageListAsync(pageIndex, pageSize, totalCount);

        return (list, totalCount.Value);
    }
}
