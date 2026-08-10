// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL Referer 提取器
/// </summary>
internal sealed class CurlRefererExtractor : HttpCurlExtractorBase
{
    /// <inheritdoc />
    protected override string[] Flags => ["-e", "--referer"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument)
    {
        // 空检查
        if (!string.IsNullOrWhiteSpace(argument))
        {
            // 设置请求来源地址
            httpRequestBuilder.SetReferer(argument);
        }
    }
}