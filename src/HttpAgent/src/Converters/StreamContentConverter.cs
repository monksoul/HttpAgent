// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     流内容转换器
/// </summary>
public class StreamContentConverter : HttpContentConverterBase<Stream>
{
    /// <inheritdoc />
    public override bool KeepsResponseAlive => true;

    /// <inheritdoc />
    public override Stream? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        httpResponseMessage.Content.ReadAsStream(cancellationToken);

    /// <inheritdoc />
    public override async Task<Stream?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
}