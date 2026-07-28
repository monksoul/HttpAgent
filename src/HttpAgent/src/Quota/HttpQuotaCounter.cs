// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     配额计数器
/// </summary>
public sealed class HttpQuotaCounter
{
    /// <summary>
    ///     当前窗口内的调用次数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     当前窗口的字符串标识
    /// </summary>
    /// <remarks>用来区分不同的配额周期，当这个标识改变时，配额计数器会自动归零，从新周期开始计算。</remarks>
    public string WindowKey { get; set; } = string.Empty;
}