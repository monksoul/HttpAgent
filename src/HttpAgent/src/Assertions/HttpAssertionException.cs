// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求断言异常
/// </summary>
public sealed class HttpAssertionException : Exception
{
    /// <summary>
    ///     <inheritdoc cref="HttpAssertionException" />
    /// </summary>
    /// <param name="message">异常信息</param>
    public HttpAssertionException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     抛出 <see cref="HttpAssertionException" /> 异常
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <exception cref="HttpAssertionException"></exception>
    [DoesNotReturn]
    public static void Throw(string message) => throw new HttpAssertionException(message);

    /// <summary>
    ///     抛出 <see cref="HttpAssertionException" /> 异常
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <returns>
    ///     <see cref="Task" />
    /// </returns>
    [DoesNotReturn]
    public static Task ThrowAsync(string message)
    {
        Throw(message);

        return Task.CompletedTask;
    }
}