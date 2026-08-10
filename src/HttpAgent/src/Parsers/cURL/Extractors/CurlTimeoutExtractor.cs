// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 超时时间提取器
/// </summary>
internal sealed class CurlTimeoutExtractor : HttpCurlExtractorBase
{
    /// <inheritdoc />
    protected override string[] Flags => ["-m", "--max-time"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(argument))
        {
            return;
        }

        // 尝试解析超时时间
        if (double.TryParse(argument, out var seconds) && seconds > 0)
        {
            // 设置超时时间
            httpRequestBuilder.SetTimeout(TimeSpan.FromSeconds(seconds));
        }
    }
}