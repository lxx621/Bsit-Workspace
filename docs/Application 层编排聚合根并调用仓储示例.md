## Application 层编排聚合根并调用仓储示例

### 完整示例：创建订单

```csharp
// Application/Services/OrderService.cs
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IProductRepository _productRepository;  // 可能需要查询其他聚合

    public OrderService(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUser currentUser,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _productRepository = productRepository;
    }

    /// <summary>
    /// 创建订单 - Application 层编排业务流程
    /// </summary>
    public async Task<<ApiResult<OrderDetailDto>> CreateOrderAsync(OrderCreateDto dto)
    {
        // 1. 开启事务
        _unitOfWork.BeginTran();
        try
        {
            // 2. 业务校验（编排阶段）
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new AppException("客户不存在");

            // 3. DTO 转 领域对象
            var address = new Address(
                dto.ShippingAddress.Province,
                dto.ShippingAddress.City,
                dto.ShippingAddress.Detail
            );

            // 4. 创建订单项（调用值对象构造函数）
            var items = new List<OrderItem>();
            foreach (var itemDto in dto.Items)
            {
                // 查询商品信息（可能需要调用其他仓储）
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new AppException($"商品 {itemDto.ProductId} 不存在");

                // 创建订单项实体
                var item = new OrderItem(
                    Guid.NewGuid(),
                    itemDto.ProductId,
                    product.Name,
                    itemDto.Quantity,
                    new Money(product.Price, "CNY")
                );
                items.Add(item);
            }

            // 5. 创建聚合根（调用工厂方法）
            var order = Order.Create(dto.CustomerId, address, items);

            // 6. 设置审计字段
            order.CreatedBy = _currentUser.UserId;

            // 7. 调用仓储保存（Infrastructure 层执行）
            await _orderRepository.AddAsync(order);

            // 8. 收集领域事件
            _unitOfWork.CollectEvents(order);

            // 9. 提交事务 + 发布领域事件
            await _unitOfWork.CommitAsync();

            // 10. 映射返回 DTO
            return ApiResult.Ok(_mapper.Map<OrderDetailDto>(order));
        }
        catch
        {
            // 10. 回滚事务
            _unitOfWork.Rollback();
            throw;
        }
    }

    /// <summary>
    /// 添加订单项 - 调用聚合根的原子方法
    /// </summary>
    public async Task<<ApiResult<OrderDetailDto>> AddItemAsync(Guid orderId, OrderItemDto itemDto)
    {
        _unitOfWork.BeginTran();
        try
        {
            // 1. 查询订单（调用仓储）
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new AppException("订单不存在");

            // 2. 创建订单项
            var item = new OrderItem(
                Guid.NewGuid(),
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.Quantity,
                new Money(itemDto.UnitPrice, "CNY")
            );

            // 3. 调用聚合根的原子方法（Domain 层封装业务逻辑）
            order.AddItem(item);  // 内部会验证状态、计算总额

            // 4. 设置审计字段
            order.UpdatedBy = _currentUser.UserId;

            // 5. 更新订单（调用仓储）
            await _orderRepository.UpdateAsync(order);

            // 6. 收集事件、提交事务
            _unitOfWork.CollectEvents(order);
            await _unitOfWork.CommitAsync();

            return ApiResult.Ok(_mapper.Map<OrderDetailDto>(order));
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    /// <summary>
    /// 更新订单状态 - 跨聚合编排
    /// </summary>
    public async Task<<ApiResult<OrderDetailDto>> UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        _unitOfWork.BeginTran();
        try
        {
            // 1. 查询订单
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new AppException("订单不存在");

            // 2. 调用聚合根方法
            order.UpdateStatus(newStatus);

            // 3. 如果状态变为已发货，需要扣减库存（跨聚合操作）
            if (newStatus == OrderStatus.Shipped)
            {
                foreach (var item in order.Items)
                {
                    var inventory = await _inventoryRepository.GetByProductIdAsync(item.ProductId);
                    if (inventory == null)
                        throw new AppException($"商品 {item.ProductId} 库存不存在");
                    
                    // 调用另一个聚合根的方法
                    inventory.DeductStock(item.Quantity);  // 扣减库存
                    await _inventoryRepository.UpdateAsync(inventory);
                    
                    // 收集库存聚合的事件
                    _unitOfWork.CollectEvents(inventory);
                }
            }

            // 4. 更新订单
            order.UpdatedBy = _currentUser.UserId;
            await _orderRepository.UpdateAsync(order);

            // 5. 收集所有事件、提交事务
            _unitOfWork.CollectEvents(order);
            await _unitOfWork.CommitAsync();

            return ApiResult.Ok(_mapper.Map<OrderDetailDto>(order));
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}
```

## 编排流程总结

### 1. **事务管理**
```csharp
_unitOfWork.BeginTran();  // 开启
try {
    // 业务逻辑
    await _unitOfWork.CommitAsync();  // 提交
} catch {
    _unitOfWork.Rollback();  // 回滚
}
```

### 2. **数据查询**
```csharp
var order = await _orderRepository.GetByIdAsync(orderId);  // 查询聚合根
var product = await _productRepository.GetByIdAsync(productId);  // 查询其他聚合
```

### 3. **对象转换**
```csharp
var address = new Address(...);  // DTO → 值对象
var item = new OrderItem(...);  // DTO → 实体
```

### 4. **调用聚合根方法**
```csharp
var order = Order.Create(...);  // 工厂方法
order.AddItem(item);  // 原子方法
order.UpdateStatus(status);  // 业务方法
```

### 5. **调用仓储操作**
```csharp
await _orderRepository.AddAsync(order);  // 新增
await _orderRepository.UpdateAsync(order);  // 更新
await _inventoryRepository.UpdateAsync(inventory);  // 更新其他聚合
```

### 6. **事件处理**
```csharp
_unitOfWork.CollectEvents(order);  // 收集事件
await _unitOfWork.CommitAsync();  // 提交事务 + 发布事件
```

### 7. **返回结果**
```csharp
return ApiResult.Ok(_mapper.Map<OrderDetailDto>(order));  // Domain → DTO
```

## 核心要点

- **Application 层是编排者**：协调多个聚合根、仓储、领域事件
- **Domain 层是业务逻辑**：聚合根内部封装业务规则
- **Infrastructure 层是数据访问**：仓储只负责 CRUD
- **UnitOfWork 是事务协调者**：保证跨聚合操作的一致性