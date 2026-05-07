// ============================================================
// 样板文件：领域事件定义 + EventHandler 完整示例
// 事件定义位置：Domain/Events/
// 处理程序位置：Application/EventHandlers/
// 说明：以"客户已创建(CustomerCreated)"事件为例
// ============================================================

// ─────────────────────────────────────────────────────────────
// 文件 1：Domain/Events/CustomerCreatedEvent.cs（领域事件定义）
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Domain.Events;

using MediatR;

/// <summary>
/// 客户已创建领域事件
/// 使用 record 类型保证不可变性
/// 命名使用过去式（XxxCreatedEvent / XxxUpdatedEvent / XxxDeletedEvent）
/// </summary>
/// <param name="CustomerId">客户ID</param>
/// <param name="Code">客户编码</param>
/// <param name="Name">客户名称</param>
/// <param name="OccurredOn">事件发生时间</param>
public record CustomerCreatedEvent(
    Guid CustomerId,
    string Code,
    string Name,
    DateTime OccurredOn
) : INotification;

// ─────────────────────────────────────────────────────────────
// 文件 2：Application/EventHandlers/CustomerCreatedEventHandler.cs
// ─────────────────────────────────────────────────────────────
namespace Bsit.SupplyChain.Application.EventHandlers;

using MediatR;
using Microsoft.Extensions.Logging;
using Bsit.SupplyChain.Domain.Events;

/// <summary>
/// 客户已创建事件处理程序
/// 职责：记录日志、发送通知等后续操作
/// 注意：事件在 UnitOfWork 事务提交后才被分发，数据已持久化
/// </summary>
public class CustomerCreatedEventHandler : INotificationHandler<CustomerCreatedEvent>
{
    private readonly ILogger<CustomerCreatedEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public CustomerCreatedEventHandler(ILogger<CustomerCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理客户创建事件
    /// </summary>
    /// <param name="notification">事件数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    public Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "客户已创建 - ID：{CustomerId}，编码：{Code}，名称：{Name}，时间：{OccurredOn}",
            notification.CustomerId,
            notification.Code,
            notification.Name,
            notification.OccurredOn);

        // 可扩展：发送欢迎邮件、同步到其他系统等
        // await _emailService.SendWelcomeAsync(notification.CustomerId);

        return Task.CompletedTask;
    }
}
