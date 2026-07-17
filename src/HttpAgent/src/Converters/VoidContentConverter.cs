// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="VoidContent" /> 内容转换器
/// </summary>
public class VoidContentConverter : HttpContentConverterBase<VoidContent>
{
    /// <inheritdoc />
    public override VoidContent? Read(HttpContentConverterContext context,
        CancellationToken cancellationToken = default) => null;

    /// <inheritdoc />
    public override Task<VoidContent?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<VoidContent?>(null);
}