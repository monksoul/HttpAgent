// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Access Token 信息
/// </summary>
public sealed class HttpAccessToken
{
    /// <summary>
    ///     <inheritdoc cref="HttpAccessToken" />
    /// </summary>
    /// <param name="value">Access Token 值</param>
    /// <param name="expiresAt">Access Token 的绝对过期时间</param>
    /// <exception cref="ArgumentException"></exception>
    public HttpAccessToken(string value, DateTimeOffset expiresAt)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    ///     Access Token 值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Access Token 的绝对过期时间
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    ///     HTTP 认证方案
    /// </summary>
    public string? Scheme { get; set; }

    /// <summary>
    ///     共享数据字典
    /// </summary>
    /// <remarks>用于存储与 Access Token 相关的自定义数据。</remarks>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <summary>
    ///     检查 Access Token 是否过期
    /// </summary>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool IsExpired() => DateTimeOffset.UtcNow >= ExpiresAt;
}