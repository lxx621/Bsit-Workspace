# JWT 认证鉴权规范

## 认证流程

1. `AuthController.Login()` 接收 `LoginRequestDto`
2. `AuthService` 通过 `IUserRepository` 获取用户
3. 验证密码哈希（`HashHelper`）
4. 通过 `JwtHelper` 生成 Access Token + Refresh Token
5. 持久化 Refresh Token
6. 返回 `ApiResult<LoginResultDto>`

## Token 配置

- 配置类：`Common/Configurations/JwtSettings.cs`
- 配置来源：`appsettings.json` 的 `Jwt` 节点
- 包含：Issuer、Audience、Key、AccessTokenExpireMinutes、RefreshTokenExpireDays

## ICurrentUser 服务

- 接口定义：`Application/Interfaces/ICurrentUser.cs`
- 实现：`Api/Services/CurrentUser.cs`（从 HttpContext JWT Claims 提取）
- 注册为 Scoped

## Swagger 集成

Swagger 配置 Bearer 认证，支持在 Swagger UI 中直接输入 JWT Token 进行测试。
