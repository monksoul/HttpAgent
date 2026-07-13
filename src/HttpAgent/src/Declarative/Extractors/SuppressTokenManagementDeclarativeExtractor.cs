// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="SuppressTokenManagementAttribute" /> 特性提取器
/// </summary>
internal sealed class SuppressTokenManagementDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法是否贴有 [SuppressTokenManagement] 特性
        if (!context.IsMethodDefined<SuppressTokenManagementAttribute>(out _, true))
        {
            return;
        }

        // 设置禁用框架的 Access Token 自动管理
        httpRequestBuilder.WithoutTokenManagement();
    }
}