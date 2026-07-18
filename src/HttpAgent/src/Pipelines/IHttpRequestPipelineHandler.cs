// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 请求管道处理器接口
/// </summary>
/// <remarks>确保实现该接口的类型是无状态的。</remarks>
public interface IHttpRequestPipelineHandler
{
    /// <summary>
    ///     处理管道步骤
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpRequestPipelineContext" />
    /// </param>
    /// <param name="next">下一个处理器的委托</param>
    /// <returns>
    ///     <see cref="HttpResponseMessage" />
    /// </returns>
    Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context, Func<Task<HttpResponseMessage?>> next);
}