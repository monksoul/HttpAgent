// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="RemoveTrailingSlashAttribute" /> 特性提取器
/// </summary>
internal sealed class RemoveTrailingSlashDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [RemoveTrailingSlash] 特性
        if (!context.IsMethodDefined<RemoveTrailingSlashAttribute>(out var removeTrailingSlashAttribute, true))
        {
            return;
        }

        // 设置是否移除 URL 地址末尾的 "/"
        httpRequestBuilder.RemoveTrailingSlash(removeTrailingSlashAttribute.Enabled);
    }
}