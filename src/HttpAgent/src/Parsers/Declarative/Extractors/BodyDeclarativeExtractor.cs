// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="BodyAttribute" /> 特性提取器
/// </summary>
internal sealed class BodyDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 查找单个贴有 [Body] 特性的参数
        var bodyParameter =
            context.UnFrozenParameters.SingleOrDefault(u => u.Key.IsDefined(typeof(BodyAttribute), true));

        // 解析参数信息
        var (parameter, value) = bodyParameter;

        // 空检查
        if (parameter is null)
        {
            return;
        }

        // 获取 BodyAttribute 实例
        var bodyAttribute = parameter.GetCustomAttribute<BodyAttribute>(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(bodyAttribute);

        switch (value)
        {
            // 检查是否作为文件路径处理
            case string filePath when bodyAttribute.AsFile:
                // 从本地路径或互联网地址中设置文件内容
                httpRequestBuilder.SetFileContent(filePath, contentType: bodyAttribute.ContentType);
                break;
            // 检查是否为原始字符串内容
            case string stringValue when bodyAttribute.RawString:
                // 设置原始字符串内容
                httpRequestBuilder.SetRawStringContent(stringValue, bodyAttribute.ContentType);
                break;
            default:
                // 设置请求内容
                httpRequestBuilder.SetContent(value, bodyAttribute.ContentType,
                    disposeResourcesOnRequestCompletion: bodyAttribute.DisposeResourcesOnRequestCompletion);
                break;
        }

        // 检查是否是 application/x-www-form-urlencoded 请求内容
        if (httpRequestBuilder.ContentType.IsIn([MediaTypeNames.Application.FormUrlEncoded]))
        {
            // 检查是否启用 StringContent 方式构建 application/x-www-form-urlencoded 请求内容
            if (bodyAttribute.UseStringContent || !bodyAttribute.UrlEncode)
            {
                httpRequestBuilder.AddStringContentForFormUrlEncodedContentProcessor(bodyAttribute.UrlEncode);
            }
        }

        // 设置内容编码
        if (!string.IsNullOrWhiteSpace(bodyAttribute.ContentEncoding))
        {
            httpRequestBuilder.SetContentEncoding(bodyAttribute.ContentEncoding);
        }
    }
}