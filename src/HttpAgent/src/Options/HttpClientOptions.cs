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
    ///     指定 JSON 响应反序列化包装器
    /// </summary>
    /// <remarks>
    ///     <para>使用时需明确调用 <see cref="HttpRequestBuilder.UseJsonResponseWrapper()" />。</para>
    ///     <para>若还需对响应做额外校验或转换，可通过 <see cref="JsonResponseWrapper.ResultHandler" /> 实现。</para>
    /// </remarks>
    public JsonResponseWrapper? JsonResponseWrapper { get; set; }

    /// <summary>
    ///     是否全局启用 JSON 响应反序列化包装器
    /// </summary>
    public bool? UseJsonResponseWrapper { get; set; }

    /// <summary>
    ///     Access Token 提供器配置
    /// </summary>
    public IHttpAccessTokenProvider? HttpAccessTokenProvider { get; set; }

    /// <summary>
    ///     事件处理程序提供器配置
    /// </summary>
    public IHttpRequestEventHandler? HttpRequestEventHandler { get; set; }

    /// <summary>
    ///     接口调用配额限制配置
    /// </summary>
    /// <remarks>
    ///     <para>用于对接像微信 API 这样对不同接口有独立调用限制的场景。需配合 <see cref="HttpRequestBuilder.SetQuotaKey(string)" /> 为每个请求指定对应的配额键。</para>
    ///     <para>推荐在 <c>appsettings.json</c> 等配置文件中定义，避免在代码中硬编码大量键值。示例如下：</para>
    ///     <code>
    ///     {
    ///       "HttpQuotas": {
    ///         "weixin": {
    ///           "wechat/accesstoken": { "MaxCount": 2000, "Strategy": "daily" },
    ///           "wechat/menu_create":  { "MaxCount": 1000, "Strategy": "weekly" },
    ///           "wechat/upload_media": { "MaxCount": 50000, "Strategy": "monthly" }
    ///         }
    ///       }
    ///     }
    ///     </code>
    /// </remarks>
    public Dictionary<string, HttpQuotaLimit>? QuotaLimits { get; set; }

    /// <summary>
    ///     标识选项是否配置为默认值（未配置）
    /// </summary>
    /// <remarks>用于避免通过 <see cref="IOptionsSnapshot{TOptions}" /> 获取选项时无法确定是否已配置该选项。默认值为：<c>true</c>。</remarks>
    internal bool IsDefault { get; set; } = true;
}