// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 身份认证提取器
/// </summary>
internal sealed class CurlAuthExtractor : IHttpCurlExtractor
{
    /// <summary>
    ///     需要携带参数的认证标志集合
    /// </summary>
    internal static readonly string[] _flagsWithArgument = ["-u", "--user", "--bearer"];

    /// <summary>
    ///     不需要携带参数的认证方案标志集合
    /// </summary>
    internal static readonly string[] _flagsWithoutArgument = ["--basic", "--digest", "--any", "--negotiate", "--ntlm"];

    /// <inheritdoc />
    public bool TryExtract(HttpRequestBuilder httpRequestBuilder, HttpCurlParsingContext context)
    {
        // 检查是否匹配带参数的认证标志
        if (context.CurrentTokenMatches(_flagsWithArgument))
        {
            // 预览下一个 Token
            var argument = context.PeekNext();

            // 空检查
            if (!string.IsNullOrWhiteSpace(argument))
            {
                // 处理带参数的认证
                ProcessAuthWithArgument(httpRequestBuilder, context.CurrentToken, argument);

                // 推进游标
                context.Advance(2);
            }
            else
            {
                // 推进游标
                context.Advance();
            }

            return true;
        }

        // 检查是否匹配不带参数的认证标志
        // ReSharper disable once InvertIf
        if (context.CurrentTokenMatches(_flagsWithoutArgument))
        {
            // 处理认证方案切换
            ProcessAuthScheme(httpRequestBuilder, context.CurrentToken);

            // 推进游标
            context.Advance();

            return true;
        }

        return false;
    }

    /// <summary>
    ///     处理带参数的认证标志
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="flag">当前匹配的命令标志</param>
    /// <param name="argument">携带的参数值</param>
    internal static void ProcessAuthWithArgument(HttpRequestBuilder httpRequestBuilder, string flag, string argument)
    {
        // 处理 Bearer Token
        if (string.Equals(flag, "--bearer", StringComparison.OrdinalIgnoreCase))
        {
            // 设置 Bearer 身份验证凭据请求授权标头
            httpRequestBuilder.AddBearerAuthentication(argument);

            return;
        }

        string username;
        string password;

        // 处理 -u 或 --user
        var colonIndex = argument.IndexOf(':');
        if (colonIndex > 0)
        {
            username = argument[..colonIndex];
            password = argument[(colonIndex + 1)..];
        }
        else
        {
            username = argument;
            password = string.Empty;
        }

        // 设置 Basic 身份验证凭据请求授权标头
        httpRequestBuilder.AddBasicAuthentication(username, password);
    }

    /// <summary>
    ///     处理不带参数的认证方案标志
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="flag">当前匹配的命令标志</param>
    internal static void ProcessAuthScheme(HttpRequestBuilder httpRequestBuilder, string flag)
    {
        // 获取当前的身份验证凭据请求授权标头
        var currentAuth = httpRequestBuilder.AuthenticationHeader;

        // 空检查
        if (currentAuth is null || string.IsNullOrWhiteSpace(currentAuth.Parameter))
        {
            return;
        }

        // 处理当前已设置了 Basic 认证
        if (string.Equals(currentAuth.Scheme, Constants.BASIC_AUTHENTICATION_SCHEME,
                StringComparison.OrdinalIgnoreCase))
        {
            // 解码 Base64 获取 username:password
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(currentAuth.Parameter));
            var colonIndex = decoded.IndexOf(':');

            // 解析出用户名和密码
            var username = colonIndex > 0 ? decoded[..colonIndex] : decoded;
            var password = colonIndex > 0 ? decoded[(colonIndex + 1)..] : string.Empty;

            // 根据最新的 flag 切换方案
            if (string.Equals(flag, "--digest", StringComparison.OrdinalIgnoreCase))
            {
                // 设置 Digest 摘要身份验证凭据请求授权标头
                httpRequestBuilder.AddDigestAuthentication(username, password);
            }
            else if (string.Equals(flag, "--basic", StringComparison.OrdinalIgnoreCase))
            {
                // 设置 Basic 身份验证凭据请求授权标头
                httpRequestBuilder.AddBasicAuthentication(username, password);
            }
        }
        // 处理当前已设置了 Digest 认证
        else if (string.Equals(currentAuth.Scheme, Constants.DIGEST_AUTHENTICATION_SCHEME,
                     StringComparison.OrdinalIgnoreCase))
        {
            // 使用 |:| 进行分割出用户名和密码
            var parts = currentAuth.Parameter.Split("|:|");

            // ReSharper disable once InvertIf
            if (parts.Length == 2)
            {
                // 根据最新的 flag 切换方案
                if (string.Equals(flag, "--basic", StringComparison.OrdinalIgnoreCase))
                {
                    // 设置 Basic 身份验证凭据请求授权标头
                    httpRequestBuilder.AddBasicAuthentication(parts[0], parts[1]);
                }
                else if (string.Equals(flag, "--digest", StringComparison.OrdinalIgnoreCase))
                {
                    // 设置 Digest 摘要身份验证凭据请求授权标头
                    httpRequestBuilder.AddDigestAuthentication(parts[0], parts[1]);
                }
            }
        }
    }
}