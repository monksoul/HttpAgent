// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="EnsureSuccessStatusCodeAttribute" /> 特性提取器
/// </summary>
internal sealed class EnsureSuccessStatusCodeDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [EnsureSuccessStatusCode] 特性
        if (!context.IsMethodDefined<EnsureSuccessStatusCodeAttribute>(out var ensureSuccessStatusCodeAttribute, true))
        {
            return;
        }

        // 设置是否如果 HTTP 响应的 IsSuccessStatusCode 属性是 false，则引发异常
        httpRequestBuilder.EnsureSuccessStatusCode(ensureSuccessStatusCodeAttribute.Enabled);
    }
}