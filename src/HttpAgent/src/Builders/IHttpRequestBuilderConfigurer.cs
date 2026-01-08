// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpRequestBuilder" /> 统一配置器
/// </summary>
/// <remarks>用于在构建 <see cref="HttpRequestMessage" /> 时调用，可对 <see cref="HttpRequestBuilder" /> 实例进行统一预处理。</remarks>
public interface IHttpRequestBuilderConfigurer
{
    /// <summary>
    ///     配置
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    void Configure(HttpRequestBuilder httpRequestBuilder);
}