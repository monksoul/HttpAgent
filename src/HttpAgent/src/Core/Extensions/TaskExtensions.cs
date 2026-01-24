// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="Task" /> 扩展类
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    ///     确保异步返回值不为 <code>null</code>
    /// </summary>
    /// <param name="task">
    ///     <see cref="Task" />
    /// </param>
    /// <typeparam name="T">异步类型</typeparam>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    public static async Task<T> ThrowIfNull<T>(this Task<T?> task)
    {
        var result = await task.ConfigureAwait(false);

        // 空检查
        ArgumentNullException.ThrowIfNull(result);

        return result;
    }

    /// <summary>
    ///     异步返回值为 <code>null</code> 时返回默认值
    /// </summary>
    /// <param name="task">
    ///     <see cref="Task" />
    /// </param>
    /// <typeparam name="T">异步类型</typeparam>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    public static Task<T> OrDefault<T>(this Task<T?> task) => task.OrDefault(default!);

    /// <summary>
    ///     异步返回值为 <code>null</code> 时返回默认值
    /// </summary>
    /// <param name="task">
    ///     <see cref="Task" />
    /// </param>
    /// <param name="defaultValue">默认值</param>
    /// <typeparam name="T">异步类型</typeparam>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    public static async Task<T> OrDefault<T>(this Task<T?> task, T defaultValue)
    {
        var result = await task.ConfigureAwait(false);

        return result ?? defaultValue;
    }
}