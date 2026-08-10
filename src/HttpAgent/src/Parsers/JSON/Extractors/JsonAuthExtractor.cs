// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 身份认证提取器
/// </summary>
/// <remarks>
///     <para>支持 <c>basic</c>、<c>bearer</c>、<c>digest</c> 三种认证方式。</para>
/// </remarks>
internal sealed class JsonAuthExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "auth";

    /// <inheritdoc />
    protected override string[]? Aliases => ["authentication", "authorization"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 检查是否是 JSON 对象
        if (node is not JsonObject authObj)
        {
            return;
        }

        // 获取认证类型
        var type = authObj["type"]?.GetValue<string>().ToLowerInvariant();

        // 空检查
        if (string.IsNullOrWhiteSpace(type))
        {
            return;
        }

        switch (type)
        {
            // 处理 Bearer Token 认证
            case "bearer":
                {
                    var token = authObj["token"]?.GetValue<string>();

                    // 空检查
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        // 检查是否指定了自定义标头名称
                        var headerName = authObj["header"]?.GetValue<string>();

                        // 空检查
                        if (string.IsNullOrWhiteSpace(headerName) ||
                            headerName.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        {
                            // 设置 Bearer 身份验证凭据请求授权标头
                            httpRequestBuilder.AddBearerAuthentication(token);
                        }
                        else
                        {
                            // 设置 Bearer 身份验证凭据请求授权标头（自定义标头）
                            httpRequestBuilder.AddBearerAuthentication(headerName, token);
                        }
                    }

                    break;
                }
            // 处理 Basic 认证
            case "basic":
                {
                    var username = authObj["username"]?.GetValue<string>();
                    var password = authObj["password"]?.GetValue<string>();

                    // 空检查
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        // 设置 Basic 身份验证凭据请求授权标头
                        httpRequestBuilder.AddBasicAuthentication(username, password);
                    }

                    break;
                }
            // 处理 Digest 摘要认证
            case "digest":
                {
                    var username = authObj["username"]?.GetValue<string>();
                    var password = authObj["password"]?.GetValue<string>();

                    // 空检查
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        // 设置 Digest 摘要身份验证凭据请求授权标头
                        httpRequestBuilder.AddDigestAuthentication(username, password ?? string.Empty);
                    }

                    break;
                }
        }
    }
}