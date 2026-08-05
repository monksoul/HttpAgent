// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     异步事件委托
/// </summary>
/// <typeparam name="TEventArgs">事件参数类型</typeparam>
public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs e);

/// <summary>
///     <see cref="AsyncEventHandler{TEventArgs}" /> 扩展类
/// </summary>
internal static class AsyncEventHandlerExtensions
{
    /// <summary>
    ///     尝试异步执行事件处理程序
    /// </summary>
    /// <param name="handler">
    ///     <see cref="AsyncEventHandler{TEventArgs}" />
    /// </param>
    /// <param name="sender">
    ///     <see cref="object" />
    /// </param>
    /// <param name="args">
    ///     <typeparamref name="TEventArgs" />
    /// </param>
    /// <typeparam name="TEventArgs">事件参数类型</typeparam>
    internal static async Task TryInvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs>? handler, object sender,
        TEventArgs args)
    {
        // 空检查
        if (handler is null)
        {
            return;
        }

        // 等待所有操作完成并按顺序执行
        foreach (var asyncHandler in handler.GetInvocationList())
        {
            try
            {
                await ((AsyncEventHandler<TEventArgs>)asyncHandler).Invoke(sender, args).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }
    }
}