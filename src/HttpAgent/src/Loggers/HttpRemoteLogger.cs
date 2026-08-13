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
    IOptionsMonitor<HttpRemoteOptions> httpRemoteOptions,
    bool isLoggingRegistered) : HttpRemoteLoggerBase
{
    /// <inheritdoc />
    public override void Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        // 检查是否注册了日志输出程序
        if (isLoggingRegistered)
        {
            logger.Log(logLevel, exception, message, args);
        }
        else
        {
            // 调用备用日志输出委托
            httpRemoteOptions.CurrentValue.FallbackLogger?.Invoke(LogMessageFormatter.Value(message, args));
        }
    }
}