// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     ETag 缓存项
/// </summary>
public sealed class HttpETagCacheItem
{
    /// <summary>
    ///     服务器返回的 ETag 值
    /// </summary>
    /// <remarks>无前后双引号。</remarks>
    public string? ETag { get; set; }

    /// <summary>
    ///     响应状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    ///     响应内容的字节数组
    /// </summary>
    public byte[]? ContentBytes { get; set; }

    /// <summary>
    ///     响应内容标头
    /// </summary>
    public Dictionary<string, IEnumerable<string>>? ContentHeaders { get; set; }

    /// <summary>
    ///     响应标头
    /// </summary>
    public Dictionary<string, IEnumerable<string>>? ResponseHeaders { get; set; }

    /// <summary>
    ///     原因短语
    /// </summary>
    public string? ReasonPhrase { get; set; }
}