// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentProcessor" /> 工厂
/// </summary>
public interface IHttpContentProcessorFactory
{
    /// <summary>
    ///     <inheritdoc cref="IServiceProvider" />
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     构建 <see cref="HttpContent" /> 实例
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentProcessorContext" />
    /// </param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 数组</param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    HttpContent? Build(HttpContentProcessorContext context, params IHttpContentProcessor[]? processors);
}