// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="JsonResponseStringUnwrapAttribute" /> 特性提取器
/// </summary>
internal sealed class JsonResponseStringUnwrapDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [JsonResponseStringUnwrap] 特性
        if (!context.IsMethodDefined<JsonResponseStringUnwrapAttribute>(out var jsonResponseWrapperAttribute, true))
        {
            return;
        }

        // 设置是否启用 JSON 响应内容字符串的解包处理（双重序列化）
        httpRequestBuilder.UseJsonResponseStringUnwrap(jsonResponseWrapperAttribute.Enabled);
    }
}