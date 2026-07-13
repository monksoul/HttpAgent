// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     异常抑制管道处理器
/// </summary>
/// <remarks>确保该处理器位于管道最外层。</remarks>
internal sealed class SuppressExceptionPipelineHandler : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        try
        {
            // 调用下一个处理器的委托
            return await next();
        }
        // 检查是否启用异常抑制机制
        catch (Exception e) when (ShouldSuppressException(context.Builder.SuppressExceptionTypes, e))
        {
            return context.ResponseMessage;
        }
    }

    /// <summary>
    ///     检查是否启用异常抑制机制
    /// </summary>
    /// <param name="suppressExceptionTypes">受抑制的异常类型列表</param>
    /// <param name="exception">
    ///     <see cref="Exception" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool ShouldSuppressException(HashSet<Type>? suppressExceptionTypes, Exception? exception)
    {
        // 空检查
        if (suppressExceptionTypes is null or { Count: 0 } || exception is null)
        {
            return false;
        }

        return suppressExceptionTypes.Any(u => u.IsInstanceOfType(exception));
    }
}