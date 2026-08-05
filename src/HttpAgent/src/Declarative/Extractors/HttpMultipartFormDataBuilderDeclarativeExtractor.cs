// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="HttpMultipartFormDataBuilder" /> 多部分表单内容配置提取器
/// </summary>
internal sealed class HttpMultipartFormDataBuilderDeclarativeExtractor : IFrozenHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 尝试解析单个 Action<HttpMultipartFormDataBuilder> 类型参数
        if (context.Args.SingleOrDefault(u => u is Action<HttpMultipartFormDataBuilder>) is not
            Action<HttpMultipartFormDataBuilder> multipartFormDataBuilderAction)
        {
            return;
        }

        // 处理和 [Multipart] 特性提取器冲突问题
        if (httpRequestBuilder.MultipartFormDataBuilder is not null)
        {
            httpRequestBuilder.WithMultipart(multipartFormDataBuilderAction);
        }
        else
        {
            // 设置多部分表单内容
            httpRequestBuilder.SetMultipartContent(multipartFormDataBuilderAction);
        }
    }

    /// <inheritdoc />
    public int Order => 2;
}