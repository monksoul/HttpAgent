// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式禁用参数数据验证特性
/// </summary>
/// <remarks>将该特性应用于接口时，将禁用该接口所有方法的参数数据验证；应用于具体方法时，则仅禁用该方法的参数验证。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class SuppressValidationAttribute : Attribute;