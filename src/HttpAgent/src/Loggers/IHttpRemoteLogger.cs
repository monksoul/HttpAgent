// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求日志服务
/// </summary>
public interface IHttpRemoteLogger
{
    /// <summary>
    ///     输出 Information 日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void LogInformation(string message, params object?[] args);

    /// <summary>
    ///     输出 Trace 日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void LogTrace(string message, params object?[] args);

    /// <summary>
    ///     输出 Debug 日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void LogDebug(string message, params object?[] args);

    /// <summary>
    ///     输出 Warning 日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void LogWarning(string message, params object?[] args);

    /// <summary>
    ///     输出 Critical 日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void LogCritical(string message, params object?[] args);

    /// <summary>
    ///     输出 Error 日志
    /// </summary>
    /// <param name="exception">异常信息</param>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void LogError(Exception exception, string message, params object?[] args);

    /// <summary>
    ///     输出日志
    /// </summary>
    /// <param name="logLevel">日志类别</param>
    /// <param name="exception">异常信息</param>
    /// <param name="message">日志消息</param>
    /// <param name="args">结构化参数</param>
    void Log(LogLevel logLevel, Exception? exception, string? message, params object?[] args);
}