// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL HTTP 版本提取器
/// </summary>
internal sealed class CurlVersionExtractor : HttpCurlExtractorBase
{
    /// <inheritdoc />
    protected override string[] Flags => ["-0", "--http1.0", "--http1.1", "--http2", "--http3"];

    /// <inheritdoc />
    protected override bool RequiresArgument => false;

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument)
    {
        var version = flag.ToLowerInvariant() switch
        {
            "-0" or "--http1.0" => new Version(1, 0),
            "--http1.1" => new Version(1, 1),
            "--http2" => new Version(2, 0),
            "--http3" => new Version(3, 0),
            _ => null
        };

        // 空检查
        if (version is not null)
        {
            // 设置 HTTP 版本
            httpRequestBuilder.SetVersion(version);
        }
    }
}