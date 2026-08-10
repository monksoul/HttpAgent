// 版权归百小僧及百签科技（广东）有限公司。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL HEAD 请求提取器
/// </summary>
internal sealed class CurlHeadExtractor : HttpCurlExtractorBase
{
    /// <inheritdoc />
    protected override string[] Flags => ["-I", "--head"];

    /// <inheritdoc />
    protected override bool RequiresArgument => false;

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument) =>
        // 设置请求方式
        httpRequestBuilder.SetHttpMethod(HttpMethod.Head);
}