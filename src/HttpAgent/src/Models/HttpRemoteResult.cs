// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求结果
/// </summary>
/// <remarks>用于将原始的 <see cref="HttpResponseMessage" /> 进行包装转换。</remarks>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public sealed class HttpRemoteResult<TResult>
{
    /// <summary>
    ///     <inheritdoc cref="HttpRemoteResult{TResult}" />
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    public HttpRemoteResult(HttpResponseMessage httpResponseMessage)
    {
        ResponseMessage = httpResponseMessage;

        // 初始化
        Initialize();
    }

    /// <inheritdoc cref="HttpResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; }

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; private set; }

    /// <summary>
    ///     字符集
    /// </summary>
    public string? CharSet { get; private set; }

    /// <summary>
    ///     内容编码
    /// </summary>
    public ICollection<string> ContentEncoding { get; private set; } = null!;

    /// <summary>
    ///     内容大小
    /// </summary>
    public long? ContentLength { get; private set; }

    /// <summary>
    ///     响应 <c>Server</c> 标头
    /// </summary>
    public HttpHeaderValueCollection<ProductInfoHeaderValue> Server { get; private set; } = null!;

    /// <summary>
    ///     原始响应标头 <c>Set-Cookie</c> 集合
    /// </summary>
    public List<string>? RawSetCookies { get; private set; }

    /// <summary>
    ///     <see cref="Microsoft.Net.Http.Headers.SetCookieHeaderValue" /> 集合
    /// </summary>
    public IList<SetCookieHeaderValue>? SetCookies { get; private set; }

    /// <summary>
    ///     响应状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    ///     是否请求成功
    /// </summary>
    public bool IsSuccessStatusCode { get; private set; }

    /// <summary>
    ///     <typeparamref name="TResult" />
    /// </summary>
    /// <remarks>注意 <c>HEAD</c> 请求不包含响应体。</remarks>
    public TResult? Result { get; internal init; }

    /// <summary>
    ///     请求耗时（毫秒）
    /// </summary>
    public long RequestDuration { get; internal init; }

    /// <summary>
    ///     响应标头
    /// </summary>
    public HttpResponseHeaders Headers { get; private set; } = null!;

    /// <summary>
    ///     响应体标头
    /// </summary>
    public HttpContentHeaders ContentHeaders { get; private set; } = null!;

    /// <summary>
    ///     HTTP 版本
    /// </summary>
    public Version Version { get; private set; } = null!;

    /// <summary>
    ///     <see cref="HttpClient" /> 实例的配置名称
    /// </summary>
    public string? HttpClientName { get; private set; }

    /// <summary>
    ///     初始化
    /// </summary>
    internal void Initialize()
    {
        // 解析响应状态码
        ParseStatusCode();

        // 解析响应标头
        ParseHeaders();

        // 解析响应内容标头部分信息
        ParseContentMetadata(ResponseMessage.Content.Headers);

        // 解析响应标头 Set-Cookie 集合
        ParseSetCookies(ResponseMessage.Headers);

        // 获取 HTTP 版本
        Version = ResponseMessage.Version;

        // 获取 HttpClient 实例的配置名称
        if (ResponseMessage.RequestMessage?.Options.TryGetValue(
                new HttpRequestOptionsKey<string>(Constants.HTTP_CLIENT_NAME), out var httpClientName) == true)
        {
            HttpClientName = httpClientName;
        }
    }

    /// <summary>
    ///     解析响应状态码
    /// </summary>
    internal void ParseStatusCode()
    {
        StatusCode = ResponseMessage.StatusCode;
        IsSuccessStatusCode = ResponseMessage.IsSuccessStatusCode;
    }

    /// <summary>
    ///     解析响应标头
    /// </summary>
    internal void ParseHeaders()
    {
        Headers = ResponseMessage.Headers;
        ContentHeaders = ResponseMessage.Content.Headers;
        Server = ResponseMessage.Headers.Server;
    }

    /// <summary>
    ///     解析响应体标头元数据
    /// </summary>
    /// <param name="contentHeaders">
    ///     <see cref="HttpContentHeaders" />
    /// </param>
    internal void ParseContentMetadata(HttpContentHeaders contentHeaders)
    {
        ContentLength = contentHeaders.ContentLength;
        ContentType = contentHeaders.ContentType?.MediaType;
        CharSet = contentHeaders.ContentType?.CharSet;
        ContentEncoding = contentHeaders.ContentEncoding;
    }

    /// <summary>
    ///     解析响应标头 <c>Set-Cookie</c> 集合
    /// </summary>
    /// <param name="responseHeaders">
    ///     <see cref="HttpResponseHeaders" />
    /// </param>
    internal void ParseSetCookies(HttpResponseHeaders responseHeaders)
    {
        // 尝试获取响应标头 Set-Cookie 集合
        if (!responseHeaders.TryGetSetCookies(out var setCookies, out var rawSetCookies))
        {
            return;
        }

        SetCookies = setCookies;
        RawSetCookies = rawSetCookies;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        // 格式化请求条目
        var requestEntry = ResponseMessage.RequestMessage?.ProfilerHeaders();

        // 格式化常规和响应条目
        var generalAndResponseEntry = ResponseMessage.ProfilerGeneralAndHeaders(generalCustomKeyValues:
            [new KeyValuePair<string, IEnumerable<string>>("Request Duration (ms)", [$"{RequestDuration:N2}"])]);

        return $"{requestEntry}\r\n{generalAndResponseEntry}";
    }
}