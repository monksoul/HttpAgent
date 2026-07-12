// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     状态码处理管道处理器
/// </summary>
internal sealed partial class StatusCodePipelineHandler : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 调用下一个处理器的委托
        var httpResponseMessage = await next();

        // 空检查
        if (httpResponseMessage is null)
        {
            return null;
        }

        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 调用状态码处理程序
        await InvokeStatusCodeHandlersAsync(httpRequestBuilder, httpResponseMessage, context.CancellationToken);

        // 如果 HTTP 响应的 IsSuccessStatusCode 属性是 false，则引发异常
        if (httpRequestBuilder.EnsureSuccessStatusCodeEnabled)
        {
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        return httpResponseMessage;
    }

    /// <summary>
    ///     调用状态码处理程序
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal static async Task InvokeStatusCodeHandlersAsync(HttpRequestBuilder httpRequestBuilder,
        HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestBuilder);
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 空检查
        if (httpRequestBuilder.StatusCodeHandlers is null || httpRequestBuilder.StatusCodeHandlers.Count == 0)
        {
            return;
        }

        // 获取响应状态码
        var statusCode = (int)httpResponseMessage.StatusCode;

        // 查找响应状态码所有处理程序
        var statusCodeHandlers = httpRequestBuilder.StatusCodeHandlers
            .Where(u => u.Key.Any(code => IsMatchedStatusCode(code, statusCode)))
            .Select(u => u.Value).ToList();

        // 空检查
        if (statusCodeHandlers.Count == 0)
        {
            return;
        }

        // 并行执行所有的处理程序，并等待所有任务完成
        await Task.WhenAll(statusCodeHandlers.Select(handler =>
            handler.TryInvokeAsync(httpResponseMessage, cancellationToken)));
    }

    /// <summary>
    ///     检查状态码代码是否匹配响应状态码
    /// </summary>
    /// <param name="code">状态码代码</param>
    /// <param name="statusCode">响应状态码</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsMatchedStatusCode(object code, int statusCode)
    {
        switch (code)
        {
            // 处理正整数类型
            case int intStatusCode when intStatusCode == statusCode:
                return true;
            // 处理 HttpStatusCode 枚举类型
            case HttpStatusCode httpStatusCode when (int)httpStatusCode == statusCode:
                return true;
            // 处理特殊字符串
            case "*" or '*':
                return true;
            // 处理字符串类型
            case string stringStatusCode when !stringStatusCode.Contains('+') &&
                                              int.TryParse(stringStatusCode, out var intStatusCodeResult) &&
                                              intStatusCodeResult == statusCode:
                return true;
            // 处理字符串区间类型，如 200-500 或 200~500
            case string stringStatusCode when StatusCodeRangeRegex().IsMatch(stringStatusCode):
                // 根据 - 或 ~ 符号切割
                var parts = stringStatusCode.Split(['-', '~'], StringSplitOptions.RemoveEmptyEntries);

                // 比较状态码区间
                if (parts.Length == 2 && int.TryParse(parts[0], out var start) && int.TryParse(parts[1], out var end))
                {
                    return statusCode >= start && statusCode <= end;
                }

                break;
            // 处理包含比较符号的类型：如：>=200, <=300, <100, =100, >100
            case string compareStatusCode when StatusCodeCompareRegex().IsMatch(compareStatusCode):
                // 提取正则表达式内容并获取符号和数字部分
                var match = StatusCodeCompareRegex().Match(compareStatusCode);
                var symbolPart = match.Groups[1].Value;
                var numberPart = match.Groups[2].Value;

                // 获取状态码
                if (!int.TryParse(numberPart, out var number))
                {
                    return false;
                }

                return symbolPart switch
                {
                    ">=" => statusCode >= number,
                    "<=" => statusCode <= number,
                    ">" => statusCode > number,
                    "<" => statusCode < number,
                    "=" => statusCode == number,
                    _ => false
                };
        }

        return false;
    }

    /// <summary>
    ///     状态码区间正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="Regex" />
    /// </returns>
    [GeneratedRegex(@"^\d+[-~]\d+$")]
    private static partial Regex StatusCodeRangeRegex();

    /// <summary>
    ///     状态码比较正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="Regex" />
    /// </returns>
    [GeneratedRegex(@"^([<>]=?|=|>|<)(\d+)$")]
    private static partial Regex StatusCodeCompareRegex();
}