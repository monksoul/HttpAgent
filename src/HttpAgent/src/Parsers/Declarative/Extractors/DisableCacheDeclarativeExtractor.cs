// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="DisableCacheAttribute" /> 特性提取器
/// </summary>
internal sealed class DisableCacheDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [DisableCache] 特性
        if (!context.IsMethodDefined<DisableCacheAttribute>(out var disableCacheAttribute, true))
        {
            return;
        }

        // 设置禁用 HTTP 缓存
        httpRequestBuilder.DisableCache(disableCacheAttribute.Disabled);
    }
}