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

## Swagger 注解

所有 Controller 必须添加 `[ApiController]` 和 `[Route("api/[controller]")]` 特性。
