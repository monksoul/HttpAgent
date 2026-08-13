// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求日志服务抽象基类
/// </summary>
public abstract class HttpRemoteLoggerBase : IHttpRemoteLogger
{
    /// <summary>
    ///     日志消息格式化器
    /// </summary>
    /// <remarks>用于在未注册 <see cref="ILogger" /> 时通过 <see cref="HttpRemoteOptions.FallbackLogger" /> 输出结构化日志。</remarks>
    public readonly Lazy<Func<string?, object?[], string?>> LogMessageFormatter = new(() =>
    {
        try
        {
            // 获取内部的 Microsoft.Extensions.Logging.FormattedLogValues 类型
            if (Type.GetType(
                    "Microsoft.Extensions.Logging.FormattedLogValues, Microsoft.Extensions.Logging.Abstractions") is
                { } formattedLogValuesType)
            {
                return (message, args) =>
                {
                    try
                    {
                        // 初始化 FormattedLogValues 实例
                        var instance = Activator.CreateInstance(formattedLogValuesType, message, args);
                        return instance?.ToString();
                    }
                    catch
                    {
                        return message;
                    }
                };
            }
        }
        catch
        {
            // ignored
        }

        return (message, _) => message;
    });

    /// <inheritdoc />
    public void LogInformation(string? message, params object?[] args) =>
        Log(LogLevel.Information, null, message, args);

    /// <inheritdoc />
    public void LogTrace(string? message, params object?[] args) => Log(LogLevel.Trace, null, message, args);

    /// <inheritdoc />
    public void LogDebug(string? message, params object?[] args) => Log(LogLevel.Debug, null, message, args);

    /// <inheritdoc />
    public void LogWarning(string? message, params object?[] args) => Log(LogLevel.Warning, null, message, args);

    /// <inheritdoc />
    public void LogWarning(Exception? exception, string? message, params object?[] args) =>
        Log(LogLevel.Warning, exception, message, args);

    /// <inheritdoc />
    public void LogCritical(string? message, params object?[] args) => Log(LogLevel.Critical, null, message, args);

    /// <inheritdoc />
    public void LogError(Exception? exception, string? message, params object?[] args) =>
        Log(LogLevel.Error, exception, message, args);

    /// <inheritdoc />
    public abstract void Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args);
}