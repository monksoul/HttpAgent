// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;

namespace HttpAgent;

/// <summary>
///     cURL 请求标头提取器
/// </summary>
internal sealed class CurlHeaderExtractor : HttpCurlExtractorBase
{
    /// <inheritdoc />
    protected override string[] Flags => ["-H", "--header"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(argument))
        {
            return;
        }

        // 尝试将字符串按第一个冒号拆分为键值对
        if (!Helpers.TrySplitHeader(argument, out var key, out var value))
        {
            throw new ArgumentException($"Invalid header format: '{argument}'. Expected 'Key: Value'.");
        }

        // 特殊处理 "Content-Type" 请求标头
        if (string.Equals(key, HeaderNames.ContentType, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(value))
        {
            // 设置内容类型
            httpRequestBuilder.SetContentType(value);

            return;
        }

        // 设置请求标头
        httpRequestBuilder.WithHeader(key, value);
    }
}