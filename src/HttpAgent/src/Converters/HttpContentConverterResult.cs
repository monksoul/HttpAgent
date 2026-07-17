namespace HttpAgent;

/// <summary>
///     HTTP 内容转换结果
/// </summary>
/// <param name="ResponseMessage">
///     <see cref="HttpResponseMessage" />
/// </param>
/// <param name="Result">
///     <typeparamref name="TResult" />
/// </param>
/// <param name="Converter">
///     <see cref="IHttpContentConverter" />
/// </param>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public sealed record HttpContentConverterResult<TResult>(
    HttpResponseMessage ResponseMessage,
    TResult? Result,
    IHttpContentConverter Converter) : IDisposable
{
    /// <inheritdoc />
    public void Dispose()
    {
        // 检查是否保持 HttpResponseMessage 存活
        if (!Converter.KeepsResponseAlive)
        {
            ResponseMessage.Dispose();
        }
    }
}

/// <summary>
///     HTTP 内容转换结果
/// </summary>
/// <param name="ResponseMessage">
///     <see cref="HttpResponseMessage" />
/// </param>
/// <param name="Result">
///     <see cref="object" />
/// </param>
/// <param name="Converter">
///     <see cref="IHttpContentConverter" />
/// </param>
public sealed record HttpContentConverterResult(
    HttpResponseMessage ResponseMessage,
    object? Result,
    IHttpContentConverter Converter) : IDisposable
{
    /// <inheritdoc />
    public void Dispose()
    {
        // 检查是否保持 HttpResponseMessage 存活
        if (!Converter.KeepsResponseAlive)
        {
            ResponseMessage.Dispose();
        }
    }
}