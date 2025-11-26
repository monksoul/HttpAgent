// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc />
/// <param name="logger">
///     <see cref="Logger{T}" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="IOptions{TOptions}" />
/// </param>
/// <param name="isLoggingRegistered">是否配置（注册）了日志程序</param>
internal sealed class HttpRemoteLogger(
    ILogger<Logging> logger,
    IOptions<HttpRemoteOptions> httpRemoteOptions,
    bool isLoggingRegistered) : IHttpRemoteLogger
{
    /// <summary>
    ///     日志消息格式化器
    /// </summary>
    /// <remarks>用于在未注册 <see cref="ILogger" /> 时通过 <see cref="HttpRemoteOptions.FallbackLogger" /> 输出结构化日志。</remarks>
    internal Lazy<Func<string?, object?[], string?>> _logMessageFormatter = new(() =>
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
    public void LogInformation(string message, params object?[] args) => Log(LogLevel.Information, null, message, args);

    /// <inheritdoc />
    public void LogTrace(string message, params object?[] args) => Log(LogLevel.Trace, null, message, args);

    /// <inheritdoc />
    public void LogDebug(string message, params object?[] args) => Log(LogLevel.Debug, null, message, args);

    /// <inheritdoc />
    public void LogWarning(string message, params object?[] args) => Log(LogLevel.Warning, null, message, args);

    /// <inheritdoc />
    public void LogCritical(string message, params object?[] args) => Log(LogLevel.Critical, null, message, args);

    /// <inheritdoc />
    public void LogError(Exception exception, string message, params object?[] args) =>
        Log(LogLevel.Error, exception, message, args);

    /// <inheritdoc />
    public void Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        // 检查是否注册了日志输出程序
        if (isLoggingRegistered)
        {
            logger.Log(logLevel, exception, message, args);
        }
        else
        {
            // 调用备用日志输出委托
            httpRemoteOptions.Value.FallbackLogger?.Invoke(_logMessageFormatter.Value(message, args));
        }
    }
}