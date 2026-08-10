// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="UseETagAttribute" /> 特性提取器
/// </summary>
internal sealed class UseETagDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [UseETag] 特性
        if (!context.IsMethodDefined<UseETagAttribute>(out var useETagAttribute, true))
        {
            return;
        }

        // 设置是否启用 ETag 缓存处理
        httpRequestBuilder.UseETag(useETagAttribute.Enabled);
    }
}