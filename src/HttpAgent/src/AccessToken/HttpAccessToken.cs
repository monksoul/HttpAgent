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
    ///     刷新 Token 常量
    /// </summary>
    internal const string RefreshTokenKey = "refresh_token";

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
    ///     <inheritdoc cref="HttpAccessToken" />
    /// </summary>
    /// <param name="value">Access Token 值</param>
    /// <param name="expiresAt">Access Token 的绝对过期时间（Unix 秒）</param>
    public HttpAccessToken(string value, long expiresAt)
        : this(value, DateTimeOffset.FromUnixTimeSeconds(expiresAt))
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HttpAccessToken" />
    /// </summary>
    /// <param name="jwtToken">完整 JWT Token 字符串</param>
    /// <exception cref="ArgumentException"></exception>
    public HttpAccessToken(string jwtToken)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtToken);

        Value = jwtToken;
        ExpiresAt = JwtTokenUtility.Parse(jwtToken).GetExpirationTimeUtc()!.Value;
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
    ///     刷新 Token（可选）
    /// </summary>
    /// <remarks>内部是 <c>Items["refresh_token"]</c> 的便捷访问器。如果不需要，可保持为 <c>null</c>。</remarks>
    public string? RefreshToken
    {
        get => Items.TryGetValue(RefreshTokenKey, out var refreshToken) ? refreshToken?.ToString() : null;
        set
        {
            // 空检查
            if (value is null)
            {
                Items.Remove(RefreshTokenKey);
            }
            else
            {
                Items[RefreshTokenKey] = value;
            }
        }
    }

    /// <summary>
    ///     共享数据字典
    /// </summary>
    /// <remarks>用于存储与 Access Token 相关的自定义数据。</remarks>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <summary>
    ///     设置 Access Token 的绝对过期时间
    /// </summary>
    /// <param name="expiresAt">Access Token 的绝对过期时间</param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    public HttpAccessToken SetExpiresAt(DateTimeOffset expiresAt)
    {
        ExpiresAt = expiresAt;

        return this;
    }

    /// <summary>
    ///     设置 Access Token 的绝对过期时间
    /// </summary>
    /// <param name="expiresAt">Access Token 的绝对过期时间（Unix 秒）</param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    public HttpAccessToken SetExpiresAt(long expiresAt) => SetExpiresAt(DateTimeOffset.FromUnixTimeSeconds(expiresAt));

    /// <summary>
    ///     检查 Access Token 是否过期
    /// </summary>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool IsExpired() => DateTimeOffset.UtcNow >= ExpiresAt;
}