---
description: 修复 Bug 的标准流程，确保定位根因而非修补表面症状
---

# Bug 修复工作流

## 步骤 1：理解问题

- 确认复现步骤、预期行为、实际行为
- 查看相关日志（`logs/` 目录或 SystemLogs 表）
- 读取 `agents-docs/error-handling.md` 和 `agents-docs/logging-guide.md`

## 步骤 2：定位根因

- 根据错误信息追溯调用链：Controller → Service → Repository → Domain
- 检查分层依赖是否合规（参考 `agents-docs/architecture-layers.md`）
- 添加临时日志（放 `temp/` 记录调试过程，修复后删除）

## 步骤 3：最小化修复

- 优先上游修复（根因处），避免下游打补丁
- 单行修复优于重构，除非根因是设计问题
- 修改后确保不破坏现有测试

## 步骤 4：验证

- 运行相关单元测试：`dotnet test --filter "FullyQualifiedName~{类名}"`
- 如无现成测试，补充回归测试
- 清理 `temp/` 中的调试产物

## 步骤 5：自检

- 对照 `agents-docs/checklist.md` 检查修改的代码
