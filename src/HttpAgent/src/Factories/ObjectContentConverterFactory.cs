// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc cref="IObjectContentConverterFactory" />
internal sealed class ObjectContentConverterFactory : IObjectContentConverterFactory
{
    /// <inheritdoc />
    public IHttpContentConverter<TResult> GetConverter<TResult>(HttpResponseMessage httpResponseMessage)
    {
        // 检查 HTTP 响应的内容类型是否为 XML 媒体类型
        if (httpResponseMessage.IsXmlContent())
        {
            return new XmlObjectContentConverter<TResult>();
        }

        return new ObjectContentConverter<TResult>();
    }

    /// <inheritdoc />
    public IHttpContentConverter GetConverter(Type resultType, HttpResponseMessage httpResponseMessage)
    {
        // 检查 HTTP 响应的内容类型是否为 XML 媒体类型
        if (httpResponseMessage.IsXmlContent())
        {
            return new XmlObjectContentConverter();
        }

        return new ObjectContentConverter();
    }
}