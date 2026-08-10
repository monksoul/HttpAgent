// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求基地址特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class BaseAddressAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="BaseAddressAttribute" />
    /// </summary>
    /// <remarks>可充当前缀使用。</remarks>
    /// <param name="baseAddress">请求基地址</param>
    public BaseAddressAttribute(string? baseAddress) => BaseAddress = baseAddress;

    /// <summary>
    ///     请求基地址
    /// </summary>
    public string? BaseAddress { get; set; }
}