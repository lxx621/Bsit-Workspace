# API 设计与 RESTful 规范

## URL 命名

- 使用名词复数：`/api/orders`、`/api/warehouses`
- 层级关系：`/api/orders/{id}/items`
- 全小写，单词用连字符分隔：`/api/audit-logs`

## HTTP 方法对应

| 方法 | 操作 | 示例 |
|------|------|------|
| GET | 查询 | `GET /api/orders/{id}` |
| POST | 创建 | `POST /api/orders` |
| PUT | 全量更新 | `PUT /api/orders/{id}` |
| PATCH | 部分更新 | `PATCH /api/orders/{id}/status` |
| DELETE | 删除（软删除） | `DELETE /api/orders/{id}` |

## 状态码标准

| 状态码 | 含义 |
|--------|------|
| 200 | 成功 |
| 201 | 创建成功 |
| 400 | 请求参数错误 |
| 401 | 未认证 |
| 403 | 无权限 |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

## 统一返回格式

所有接口使用 `ApiResult<T>` 统一返回：

```json
{
  "success": true,
  "code": 200,
  "message": "操作成功",
  "data": { }
}
```

## Controller 文件组织

Controller 按业务模块放入对应子目录：

```
Controllers/
├── Auth/
│   └── AuthController.cs
├── Customer/
│   └── CustomerController.cs
├── Order/
│   ├── OrderController.cs
│   └── OrderAuditController.cs      # 同一模块多个控制器
└── Warehouse/
    └── WarehouseController.cs
```

- 命名空间跟随目录：`namespace Bsit.SupplyChain.Api.Controllers.Customer;`
- 子目录名称使用 PascalCase，与模块名一致
- 路由由 `[Route]` 特性决定，与物理目录无关

## Swagger 注解

所有 Controller 必须添加 `[ApiController]` 和 `[Route("api/[controller]")]` 特性。
