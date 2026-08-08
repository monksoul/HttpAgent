// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 请求内容提取器
/// </summary>
internal sealed class CurlDataExtractor : HttpCurlExtractorBase
{
    /// <inheritdoc />
    protected override string[] Flags =>
        ["-d", "--data", "--data-raw", "--data-binary", "--data-urlencode", "--data-ascii"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(argument))
        {
            return;
        }

        // 检查是否已设置了内容类型
        var hasExplicitContentType = !string.IsNullOrWhiteSpace(httpRequestBuilder.ContentType);

        // 根据 flag 推断默认内容类型
        var defaultContentType = flag switch
        {
            "--data-binary" => MediaTypeNames.Application.Octet,
            _ => MediaTypeNames.Application.FormUrlEncoded
        };

        // 获取有效的内容类型
        var effectiveContentType = hasExplicitContentType ? null : defaultContentType;

        // 空检查
        if (httpRequestBuilder.RawContent is null)
        {
            // 检查是否是 application/x-www-form-urlencoded 请求内容
            if ((effectiveContentType ?? httpRequestBuilder.ContentType).IsIn([
                    MediaTypeNames.Application.FormUrlEncoded
                ]))
            {
                httpRequestBuilder.AddStringContentForFormUrlEncodedContentProcessor(flag == "--data-urlencode");
            }

            // 设置请求内容
            httpRequestBuilder.SetContent(argument, effectiveContentType);
        }
        else
        {
            // 追加请求内容
            httpRequestBuilder.AppendContent(argument);
        }
    }
}