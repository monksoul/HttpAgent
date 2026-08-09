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

        object content;

        // 标记是否为 --data-urlencode 的文件读取
        var isUrlEncodeFileRead = false;

        // 判断是否是文件读取语法（--data-raw 不支持 @file）
        var isFileRead = !string.Equals(flag, "--data-raw", StringComparison.OrdinalIgnoreCase) &&
                         argument.StartsWith('@') && argument.Length > 1;
        if (isFileRead)
        {
            // 解析文件路径
            var filePath = argument[1..];

            // 如果是 --data-binary，读取为字节数组
            if (string.Equals(flag, "--data-binary", StringComparison.OrdinalIgnoreCase))
            {
                content = File.ReadAllBytes(filePath);
            }
            else
            {
                // 读取文件文本内容
                var fileText = File.ReadAllText(filePath);

                // 如果是 --data-urlencode，对文件内容整体进行 URL 编码
                if (string.Equals(flag, "--data-urlencode", StringComparison.OrdinalIgnoreCase))
                {
                    content = Uri.EscapeDataString(fileText).Replace("%20", "+");

                    // 标记已手动编码
                    isUrlEncodeFileRead = true;
                }
                else
                {
                    content = fileText;
                }
            }
        }
        else
        {
            content = argument;
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
            var isFormUrlEncoded =
                (effectiveContentType ?? httpRequestBuilder.ContentType).IsIn([
                    MediaTypeNames.Application.FormUrlEncoded
                ]);

            // 添加 URL 编码处理器，只有非文件读取且非 --data-urlencode 文件读取时才添加
            if (isFormUrlEncoded && !isFileRead && !isUrlEncodeFileRead)
            {
                httpRequestBuilder.AddStringContentForFormUrlEncodedContentProcessor(flag == "--data-urlencode");
            }

            // 设置请求内容
            httpRequestBuilder.SetContent(content, effectiveContentType);
        }
        else
        {
            // 追加请求内容
            httpRequestBuilder.AppendContent(content);
        }
    }
}