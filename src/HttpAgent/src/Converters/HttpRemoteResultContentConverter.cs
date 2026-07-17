// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpRemoteResult{TResult}" /> 内容转换器
/// </summary>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public class HttpRemoteResultContentConverter<TResult> : HttpContentConverterBase<HttpRemoteResult<TResult>>
{
    /// <inheritdoc />
    public override bool KeepsResponseAlive => true;

    /// <inheritdoc />
    public override HttpRemoteResult<TResult>? Read(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context.Factory);

        // 获取 HttpResponseMessage 实例
        var httpResponseMessage = context.ResponseMessage;

        // 将 HttpResponseMessage 转换为 TResult 实例
        var httpContentConverterResult = context.Factory.Read<TResult>(context, cancellationToken);

        return new HttpRemoteResult<TResult>(httpResponseMessage)
        {
            Result = httpContentConverterResult.Result, RequestDuration = context.RequestDuration
        };
    }

    /// <inheritdoc />
    public override async Task<HttpRemoteResult<TResult>?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context.Factory);

        // 获取 HttpResponseMessage 实例
        var httpResponseMessage = context.ResponseMessage;

        // 将 HttpResponseMessage 转换为 TResult 实例
        var httpContentConverterResult = await context.Factory.ReadAsync<TResult>(context, cancellationToken);

        return new HttpRemoteResult<TResult>(httpResponseMessage)
        {
            Result = httpContentConverterResult.Result, RequestDuration = context.RequestDuration
        };
    }
}