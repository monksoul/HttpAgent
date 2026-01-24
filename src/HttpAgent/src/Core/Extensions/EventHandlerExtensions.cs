// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="EventHandler{TEventArgs}" /> 扩展类
/// </summary>
internal static class EventHandlerExtensions
{
    /// <summary>
    ///     尝试执行事件处理程序
    /// </summary>
    /// <param name="handler">
    ///     <see cref="EventHandler{TEventArgs}" />
    /// </param>
    /// <param name="sender">
    ///     <see cref="object" />
    /// </param>
    /// <param name="args">
    ///     <typeparamref name="TEventArgs" />
    /// </param>
    /// <typeparam name="TEventArgs">事件参数</typeparam>
    internal static void TryInvoke<TEventArgs>(this EventHandler<TEventArgs>? handler, object? sender, TEventArgs args)
    {
        // 空检查
        if (handler is null)
        {
            return;
        }

        try
        {
            handler(sender, args);
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }
}