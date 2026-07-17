// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     字节数组内容转换器
/// </summary>
public class ByteArrayContentConverter : HttpContentConverterBase<byte[]>
{
    /// <inheritdoc />
    public override byte[]? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default) =>
        AsyncUtility.RunSync(() => ReadAsync(context, cancellationToken));

    /// <inheritdoc />
    public override async Task<byte[]?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default) =>
        await context.ResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
}