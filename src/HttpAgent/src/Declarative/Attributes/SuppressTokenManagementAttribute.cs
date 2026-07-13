// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式禁用框架的 Access Token 自动管理特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class SuppressTokenManagementAttribute : Attribute;