// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpClient" /> 配置选项
/// </summary>
public sealed class HttpClientOptions
{
    /// <summary>
    ///     JSON 序列化配置
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } =
        new(HttpRemoteOptions.JsonSerializerOptionsDefault);

    /// <summary>
    ///     标识选项是否配置为默认值（未配置）
    /// </summary>
    /// <remarks>用于避免通过 <see cref="IOptionsSnapshot{TOptions}" /> 获取选项时无法确定是否已配置该选项。默认值为：<c>true</c>。</remarks>
    internal bool IsDefault { get; set; } = true;

    /// <summary>
    ///     指定 JSON 响应反序列化包装器
    /// </summary>
    /// <remarks>使用时需明确调用 <see cref="HttpRequestBuilder.JsonResponseWrapping()" />。</remarks>
    /// TODO: 未来可以考虑不同的 HTTP 状态码使用不同的包装器
    public JsonResponseWrapper? JsonResponseWrapper { get; set; }

    /// <summary>
    ///     是否全局启用 JSON 响应反序列化包装器
    /// </summary>
    public bool? UseJsonResponseWrapping { get; set; }
}