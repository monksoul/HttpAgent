// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpResponseMessage" /> 内容转换器
/// </summary>
public class HttpResponseMessageConverter : HttpContentConverterBase<HttpResponseMessage>
{
    /// <inheritdoc />
    public override bool KeepsResponseAlive => true;

    /// <inheritdoc />
    public override HttpResponseMessage? Read(HttpContentConverterContext context,
        CancellationToken cancellationToken = default) => context.ResponseMessage;

    /// <inheritdoc />
    public override Task<HttpResponseMessage?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<HttpResponseMessage?>(context.ResponseMessage);
}