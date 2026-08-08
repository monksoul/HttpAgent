// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 请求地址提取器
/// </summary>
internal sealed class CurlUrlExtractor : IOrderedHttpCurlExtractor
{
    /// <inheritdoc />
    public int Order => 100;

    /// <inheritdoc />
    public bool TryExtract(HttpRequestBuilder httpRequestBuilder, HttpCurlTokenExtractorContext context)
    {
        // 获取当前 Token
        var currentToken = context.CurrentToken;

        // 检查当前 Token 是否匹配 "--url"
        if (context.CurrentTokenMatches("--url"))
        {
            // 预览下一个 Token
            var url = context.PeekNext();

            // 空检查
            if (!string.IsNullOrWhiteSpace(url))
            {
                // 设置请求地址
                httpRequestBuilder.SetRequestUri(new Uri(url, UriKind.RelativeOrAbsolute));

                // 推进游标
                context.Advance(2);

                return true;
            }

            // 推进游标
            context.Advance();

            return true;
        }

        // 处理隐式位置参数，不以 - 开头且看起来像 URL
        // ReSharper disable once InvertIf
        if (!currentToken.StartsWith('-') && LooksLikeUrl(currentToken))
        {
            // 设置请求地址
            httpRequestBuilder.SetRequestUri(new Uri(currentToken, UriKind.RelativeOrAbsolute));

            // 推进游标
            context.Advance();

            return true;
        }

        return false;
    }

    /// <summary>
    ///     检查 Token 是否看起来像 URL
    /// </summary>
    /// <param name="token">Token 值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool LooksLikeUrl(string? token)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return token.StartsWith('/') ||
               token.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
               token.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
               token.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) ||
               (Uri.TryCreate(token, UriKind.Absolute, out var uri) &&
                token.StartsWith(uri.Scheme + "://", StringComparison.OrdinalIgnoreCase));
    }
}