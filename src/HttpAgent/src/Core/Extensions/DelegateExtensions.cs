// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     委托拓展类
/// </summary>
internal static class DelegateExtensions
{
    /// <summary>
    ///     尝试执行异步委托
    /// </summary>
    /// <param name="func">异步委托</param>
    /// <param name="parameter1">参数 1</param>
    /// <param name="parameter2">参数 2</param>
    /// <typeparam name="T1">参数类型</typeparam>
    /// <typeparam name="T2">参数类型</typeparam>
    internal static async Task TryInvokeAsync<T1, T2>(this Func<T1, T2, Task>? func, T1 parameter1, T2 parameter2)
    {
        // 空检查
        if (func is null)
        {
            return;
        }

        try
        {
            await func(parameter1, parameter2);
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }

    /// <summary>
    ///     尝试执行异步委托
    /// </summary>
    /// <param name="func">异步委托</param>
    /// <param name="parameter">参数</param>
    /// <typeparam name="T">参数类型</typeparam>
    internal static async Task TryInvokeAsync<T>(this Func<T, Task>? func, T parameter)
    {
        // 空检查
        if (func is null)
        {
            return;
        }

        try
        {
            await func(parameter);
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }

    /// <summary>
    ///     尝试执行同步委托
    /// </summary>
    /// <param name="action">同步委托</param>
    /// <param name="parameter1">参数 1</param>
    /// <param name="parameter2">参数 2</param>
    /// <typeparam name="T1">参数类型</typeparam>
    /// <typeparam name="T2">参数类型</typeparam>
    internal static void TryInvoke<T1, T2>(this Action<T1, T2>? action, T1 parameter1, T2 parameter2)
    {
        // 空检查
        if (action is null)
        {
            return;
        }

        try
        {
            action(parameter1, parameter2);
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }

    /// <summary>
    ///     尝试执行同步委托
    /// </summary>
    /// <param name="action">同步委托</param>
    /// <param name="parameter">参数</param>
    /// <typeparam name="T">参数类型</typeparam>
    internal static void TryInvoke<T>(this Action<T>? action, T parameter)
    {
        // 空检查
        if (action is null)
        {
            return;
        }

        try
        {
            action(parameter);
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }

    /// <summary>
    ///     尝试执行同步委托
    /// </summary>
    /// <param name="action">同步委托</param>
    internal static void TryInvoke(this Action? action)
    {
        // 空检查
        if (action is null)
        {
            return;
        }

        try
        {
            action();
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }

    /// <summary>
    ///     将当前配置委托合并到目标委托字段中
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="field">目标委托字段</param>
    /// <typeparam name="T">委托参数类型</typeparam>
    internal static void Combine<T>(this Action<T> configure, ref Action<T>? field)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 如果 field 未设置则直接赋值
        if (field is null)
        {
            field = configure;
        }
        // 否则创建级联调用委托
        else
        {
            // 复制一个新的委托避免死循环
            var original = field;

            field = value =>
            {
                original.Invoke(value);
                configure.Invoke(value);
            };
        }
    }

    /// <summary>
    ///     将当前配置委托合并到目标委托字段中
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="field">目标委托字段</param>
    /// <typeparam name="T1">委托参数类型</typeparam>
    /// <typeparam name="T2">委托参数类型</typeparam>
    internal static void Combine<T1, T2>(this Action<T1, T2> configure, ref Action<T1, T2>? field)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 如果 field 未设置则直接赋值
        if (field is null)
        {
            field = configure;
        }
        // 否则创建级联调用委托
        else
        {
            // 复制一个新的委托避免死循环
            var original = field;

            field = (value1, value2) =>
            {
                original.Invoke(value1, value2);
                configure.Invoke(value1, value2);
            };
        }
    }
}