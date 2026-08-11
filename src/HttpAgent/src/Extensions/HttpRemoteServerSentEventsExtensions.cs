// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Extensions;

/// <summary>
///     HTTP 远程服务 Server Sent Events 扩展类
/// </summary>
public static class HttpRemoteServerSentEventsExtensions
{
    /// <summary>
    ///     将 <see cref="ServerSentEventsData" /> 解析为 <see cref="McpMessageData" />
    /// </summary>
    /// <param name="serverSentEventsData">
    ///     <see cref="ServerSentEventsData" />
    /// </param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="McpMessageData" />
    /// </returns>
    public static McpMessageData? ToMcpMessage(this ServerSentEventsData serverSentEventsData,
        JsonSerializerOptions? jsonSerializerOptions = null) =>
        string.IsNullOrWhiteSpace(serverSentEventsData?.Data)
            ? null
            : JsonSerializer.Deserialize<McpMessageData>(serverSentEventsData.Data,
                jsonSerializerOptions ?? HttpRemoteOptions.JsonSerializerOptionsDefault);

    /// <summary>
    ///     将 <see cref="McpMessageData.Result" /> 转换为指定类型
    /// </summary>
    /// <param name="mcpMessageData">
    ///     <see cref="McpMessageData" />
    /// </param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    public static T? GetResult<T>(this McpMessageData mcpMessageData,
        JsonSerializerOptions? jsonSerializerOptions = null) =>
        mcpMessageData?.Result is null
            ? default
            : mcpMessageData.Result.Value.Deserialize<T>(jsonSerializerOptions ??
                                                         HttpRemoteOptions.JsonSerializerOptionsDefault);

    /// <summary>
    ///     将 <see cref="McpError.Data" /> 转换为指定类型
    /// </summary>
    /// <param name="mcpError">
    ///     <see cref="McpError" />
    /// </param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    public static T? GetData<T>(this McpError mcpError, JsonSerializerOptions? jsonSerializerOptions = null) =>
        mcpError?.Data is null
            ? default
            : mcpError.Data.Value.Deserialize<T>(
                jsonSerializerOptions ?? HttpRemoteOptions.JsonSerializerOptionsDefault);
}