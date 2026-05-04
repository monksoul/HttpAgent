// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="SimulateBrowserAttribute" /> 特性提取器
/// </summary>
internal sealed class SimulateBrowserDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [SimulateBrowser] 特性
        if (!context.IsMethodDefined<SimulateBrowserAttribute>(out var simulateBrowserAttribute, true))
        {
            return;
        }

        // 设置模拟浏览器环境
        httpRequestBuilder.SimulateBrowser(simulateBrowserAttribute.IsMobile);
    }
}