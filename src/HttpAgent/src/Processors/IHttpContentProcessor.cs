// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpContent" /> 请求内容处理器
/// </summary>
/// <remarks>用于将原始请求内容转换成 <see cref="HttpContent" /> 实例</remarks>
public interface IHttpContentProcessor
{
    /// <summary>
    ///     <inheritdoc cref="IServiceProvider" />
    /// </summary>
    IServiceProvider? ServiceProvider { get; set; }

    /// <summary>
    ///     判断当前处理器是否可以处理指定的内容类型
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentProcessorContext" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool CanProcess(HttpContentProcessorContext context);

    /// <summary>
    ///     将原始请求内容转换为 <see cref="HttpContent" /> 实例
    /// </summary>
    /// <remarks>若需要返回多个 <see cref="HttpContent" />，可通过创建 <see cref="CompositeHttpContent" /> 实例返回。</remarks>
    /// <param name="context">
    ///     <see cref="HttpContentProcessorContext" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    HttpContent? Process(HttpContentProcessorContext context);
}