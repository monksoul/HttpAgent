// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;
using StringWithQualityHeaderValue = System.Net.Http.Headers.StringWithQualityHeaderValue;

namespace HttpAgent.Extensions;

/// <summary>
///     HTTP 远程服务模块拓展类
/// </summary>
public static partial class HttpRemoteExtensions
{
    /// <summary>
    ///     添加 HTTP 远程请求分析工具处理委托
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IHttpClientBuilder" />
    /// </param>
    /// <param name="disableIn">自定义禁用配置委托</param>
    /// <returns>
    ///     <see cref="IHttpClientBuilder" />
    /// </returns>
    public static IHttpClientBuilder AddProfilerDelegatingHandler(this IHttpClientBuilder builder,
        Func<bool>? disableIn = null)
    {
        // 检查是否禁用请求分析工具
        if (disableIn?.Invoke() == true)
        {
            return builder;
        }

        // 注册请求分析工具服务
        builder.Services.TryAddTransient<ProfilerDelegatingHandler>();

        // 添加请求分析工具处理委托
        return builder.AddHttpMessageHandler<ProfilerDelegatingHandler>();
    }

    /// <summary>
    ///     添加 HTTP 远程请求分析工具处理委托
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IHttpClientBuilder" />
    /// </param>
    /// <param name="disableInProduction">是否在生产环境中禁用。默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="IHttpClientBuilder" />
    /// </returns>
    public static IHttpClientBuilder AddProfilerDelegatingHandler(this IHttpClientBuilder builder,
        bool disableInProduction) =>
        builder.AddProfilerDelegatingHandler(() =>
            disableInProduction && GetHostEnvironmentName(builder.Services)?.ToLower() == "production");

    /// <summary>
    ///     配置 <see cref="HttpClient" /> 额外选项
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IHttpClientBuilder" />
    /// </param>
    /// <param name="configure">自定义配置选项</param>
    /// <returns>
    ///     <see cref="IHttpClientBuilder" />
    /// </returns>
    public static IHttpClientBuilder ConfigureOptions(this IHttpClientBuilder builder,
        Action<HttpClientOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.AddOptions<HttpClientOptions>(builder.Name).Configure(options =>
        {
            options.IsDefault = false;
            configure.Invoke(options);
        });

        return builder;
    }

    /// <summary>
    ///     配置 <see cref="HttpClient" /> 额外选项
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IHttpClientBuilder" />
    /// </param>
    /// <param name="configure">自定义配置选项</param>
    /// <returns>
    ///     <see cref="IHttpClientBuilder" />
    /// </returns>
    public static IHttpClientBuilder ConfigureOptions(this IHttpClientBuilder builder,
        Action<HttpClientOptions, IServiceProvider> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.AddOptions<HttpClientOptions>(builder.Name).Configure<IServiceProvider>((options, provider) =>
        {
            options.IsDefault = false;
            configure.Invoke(options, provider);
        });

        return builder;
    }

    /// <summary>
    ///     为 <see cref="HttpClient" /> 启用性能优化
    /// </summary>
    /// <param name="httpClient">
    ///     <see cref="HttpClient" />
    /// </param>
    public static void PerformanceOptimization(this HttpClient httpClient)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpClient);

        // 设置 Accept 头，表示可以接受任何类型的内容
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        // 添加 Accept-Encoding 头，支持 gzip、deflate 以及 Brotli 压缩算法
        // 这样服务器可以根据情况选择最合适的压缩方式发送响应，从而减少传输的数据量
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

        // 设置 Connection 头为 keep-alive，允许重用 TCP 连接，避免每次请求都重新建立连接带来的开销
        httpClient.DefaultRequestHeaders.ConnectionClose = false;
    }

    /// <summary>
    ///     分析 <see cref="HttpRequestMessage" /> 标头
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="summary">摘要</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ProfilerHeaders(this HttpRequestMessage httpRequestMessage,
        string? summary = "Request Headers") =>
        StringUtility.FormatKeyValuesSummary(
            httpRequestMessage.Headers.ConcatIgnoreNull(httpRequestMessage.Content?.Headers), summary);

    /// <summary>
    ///     分析 <see cref="HttpResponseMessage" /> 标头
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="summary">摘要</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ProfilerHeaders(this HttpResponseMessage httpResponseMessage,
        string? summary = "Response Headers") =>
        StringUtility.FormatKeyValuesSummary(
            httpResponseMessage.Headers.ConcatIgnoreNull(httpResponseMessage.Content.Headers),
            summary);

    /// <summary>
    ///     分析常规和 <see cref="HttpResponseMessage" /> 标头
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="responseSummary">响应标头摘要</param>
    /// <param name="generalSummary">常规摘要</param>
    /// <param name="generalCustomKeyValues">自定义常规摘要键值集合</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string ProfilerGeneralAndHeaders(this HttpResponseMessage httpResponseMessage,
        string? responseSummary = "Response Headers", string? generalSummary = "General",
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? generalCustomKeyValues = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 获取 HttpRequestMessage 实例
        var httpRequestMessage = httpResponseMessage.RequestMessage;

        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestMessage);

        // 获取 HttpContent 实例
        var httpContent = httpRequestMessage.Content;

        // 格式化 HTTP 声明式条目
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? declarativeKeyValues =
            httpRequestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>(Constants.DECLARATIVE_METHOD_KEY),
                out var methodSignature)
                ? [new KeyValuePair<string, IEnumerable<string>>("Declarative", [methodSignature])]
                : null;

        // 格式化 HttpClient 实例的配置条目
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? httpClientKeyValues =
            httpRequestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>(Constants.HTTP_CLIENT_NAME),
                out var httpClientName)
                ? [new KeyValuePair<string, IEnumerable<string>>("HttpClient Name", [httpClientName])]
                : null;

        // 格式化常规条目
        var generalEntry = StringUtility.FormatKeyValuesSummary(new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("Request URL",
                    [httpRequestMessage.RequestUri?.OriginalString!]),
                new KeyValuePair<string, IEnumerable<string>>("HTTP Method", [httpRequestMessage.Method.ToString()]),
                new KeyValuePair<string, IEnumerable<string>>("Status Code",
                    [$"\e[33m\e[1m{(int)httpResponseMessage.StatusCode} {httpResponseMessage.StatusCode}\e[0m"]),
                new KeyValuePair<string, IEnumerable<string>>("HTTP Version", [httpResponseMessage.Version.ToString()]),
                new KeyValuePair<string, IEnumerable<string>>("HTTP Content",
                    [$"{httpContent?.GetType().Name}"])
            }.ConcatIgnoreNull(httpClientKeyValues).ConcatIgnoreNull(declarativeKeyValues)
            .ConcatIgnoreNull(generalCustomKeyValues), generalSummary);

        // 格式化响应条目
        var responseEntry = httpResponseMessage.ProfilerHeaders(responseSummary);

        return $"{generalEntry}\r\n{responseEntry}";
    }

    /// <summary>
    ///     分析 <see cref="HttpContent" /> 内容
    /// </summary>
    /// <param name="httpContent">
    ///     <see cref="HttpContent" />
    /// </param>
    /// <param name="summary">摘要</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static async Task<string?> ProfilerAsync(this HttpContent? httpContent, string? summary = "Request Body",
        CancellationToken cancellationToken = default)
    {
        // 空检查
        if (httpContent is null)
        {
            return null;
        }

        // 修复无效的响应内容字符编码
        httpContent.FixInvalidCharset();

        // 默认只读取 5KB 的内容
        const int maxBytesToDisplay = 5120;

        /*
         * 读取内容为字节数组
         *
         * 由于 HttpContent 的流设计为单次读取（即流内容在首次读取后会被消耗，无法重复读取），
         * 当前实现（即使用 ReadAsByteArrayAsync(cancellationToken)）中对于较大内容会一次性加载至内存，
         * 这可能导致性能问题（如内存占用过高或响应延迟），不过目前尚未找到更优的解决方案。
         *
         * 强烈建议在生产环境中禁用或关闭此类一次性读取操作，尤其是对于高并发或大流量场景，
         * 以避免因内存溢出（OOM）或线程阻塞导致的服务不可用风险。
         */
        var buffer = await httpContent.ReadAsByteArrayAsync(cancellationToken);
        var total = buffer.Length;

        // 计算要显示的部分
        var bytesToShow = Math.Min(total, maxBytesToDisplay);

        // 注册 CodePagesEncodingProvider，使得程序能够识别并使用 Windows 代码页中的各种编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 获取内容编码
        var charset = httpContent.Headers.ContentType?.CharSet ?? "utf-8";
        var partialContent = Encoding.GetEncoding(charset).GetString(buffer, 0, bytesToShow);

        // 解决退格导致显示不全问题：保留 \n 和 \r，仅过滤其他 ASCII 控制字符（ASCII < 32 且不是 \n 或 \r）
        partialContent = new string(partialContent
            .Where(c => c >= 32 || c == '\n' || c == '\r')
            .ToArray());

        // 检查是否是完整的 Unicode 转义字符串
        if (total == bytesToShow && UnicodeEscapeRegex().IsMatch(partialContent))
        {
            partialContent = Regex.Unescape(partialContent);
        }

        // 如果实际读取的数据小于最大显示大小，则直接返回；否则，添加省略号表示内容被截断
        var bodyString = total <= maxBytesToDisplay
            ? partialContent
            : partialContent + $"\e[32m\e[1m ... [truncated, total: {total} bytes]\e[0m";

        return StringUtility.FormatKeyValuesSummary(
            [new KeyValuePair<string, IEnumerable<string>>(string.Empty, [bodyString])],
            $"{summary} ({httpContent.GetType().Name}, total: {total} bytes)");
    }

    /// <summary>
    ///     克隆 <see cref="HttpRequestMessage" />
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestMessage" />
    /// </returns>
    public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestMessage);

        // 初始化克隆的 HttpRequestMessage 实例
        var clonedHttpRequestMessage = new HttpRequestMessage(httpRequestMessage.Method, httpRequestMessage.RequestUri);

        // 复制请求标头
        foreach (var header in httpRequestMessage.Headers)
        {
            clonedHttpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // 复制 httpRequestMessage.Options 选项
        foreach (var (key, value) in httpRequestMessage.Options)
        {
            clonedHttpRequestMessage.Options.TryAdd(key, value); // TODO: 思考是 TryAdd 还是 AddOrUpdate
        }

        // 检查是否包含请求内容
        if (httpRequestMessage.Content is null)
        {
            return clonedHttpRequestMessage;
        }

        // 复制请求内容
        var memoryStream = new MemoryStream();
        await httpRequestMessage.Content.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        // 设置请求内容
        clonedHttpRequestMessage.Content = new StreamContent(memoryStream);

        // 复制请求内容标头
        foreach (var header in httpRequestMessage.Content.Headers)
        {
            clonedHttpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clonedHttpRequestMessage;
    }

    /// <summary>
    ///     克隆 <see cref="HttpRequestMessage" />
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestMessage" />
    /// </returns>
    public static HttpRequestMessage Clone(this HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken = default) =>
        httpRequestMessage.CloneAsync(cancellationToken).GetAwaiter().GetResult();

    /// <summary>
    ///     尝试获取响应标头 <c>Set-Cookie</c> 集合
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="setCookies">响应标头 <c>Set-Cookie</c> 集合</param>
    /// <param name="rawSetCookies">原始响应标头 <c>Set-Cookie</c> 集合</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool TryGetSetCookies(this HttpResponseMessage httpResponseMessage,
        [NotNullWhen(true)] out IList<SetCookieHeaderValue>? setCookies,
        [NotNullWhen(true)] out List<string>? rawSetCookies)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        return httpResponseMessage.Headers.TryGetSetCookies(out setCookies, out rawSetCookies);
    }

    /// <summary>
    ///     修复无效的响应内容字符编码
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    public static void FixInvalidCharset(this HttpResponseMessage? httpResponseMessage) =>
        httpResponseMessage?.Content.FixInvalidCharset();

    /// <summary>
    ///     修复无效的响应内容字符编码
    /// </summary>
    /// <param name="httpContent">
    ///     <see cref="HttpContent" />
    /// </param>
    public static void FixInvalidCharset(this HttpContent? httpContent)
    {
        // 空检查
        if (httpContent?.Headers.ContentType?.CharSet is null)
        {
            return;
        }

        // 获取内容字符编码
        var charset = httpContent.Headers.ContentType.CharSet.Trim();

        // 修复 "utf8"、"utf 8" 和 "utf-8;" 的错误写法（不区分大小写/空格）
        if (charset.Equals("utf8", StringComparison.OrdinalIgnoreCase) ||
            charset.Equals("utf 8", StringComparison.OrdinalIgnoreCase) ||
            charset.Equals("utf-8;", StringComparison.OrdinalIgnoreCase))
        {
            httpContent.Headers.ContentType.CharSet = "utf-8";
        }
    }

    /// <summary>
    ///     尝试获取响应标头 <c>Set-Cookie</c> 集合
    /// </summary>
    /// <param name="responseHeaders">
    ///     <see cref="HttpResponseHeaders" />
    /// </param>
    /// <param name="setCookies">响应标头 <c>Set-Cookie</c> 集合</param>
    /// <param name="rawSetCookies">原始响应标头 <c>Set-Cookie</c> 集合</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool TryGetSetCookies(this HttpResponseHeaders responseHeaders,
        [NotNullWhen(true)] out IList<SetCookieHeaderValue>? setCookies,
        [NotNullWhen(true)] out List<string>? rawSetCookies)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(responseHeaders);

        // 检查响应标头是否包含 Set-Cookie 设置
        if (!responseHeaders.TryGetValues(HeaderNames.SetCookie, out var setCookieValues))
        {
            setCookies = null;
            rawSetCookies = null;

            return false;
        }

        rawSetCookies = setCookieValues.ToList();
        setCookies = SetCookieHeaderValue.ParseList(rawSetCookies);

        return true;
    }

    /// <summary>
    ///     检查 HTTP 响应的内容类型是否为 XML 媒体类型
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool IsXmlContent(this HttpResponseMessage httpResponseMessage) =>
        httpResponseMessage.Content.Headers.ContentType?.MediaType.IsIn(
            [MediaTypeNames.Application.Xml, MediaTypeNames.Application.XmlPatch, MediaTypeNames.Text.Xml],
            StringComparer.OrdinalIgnoreCase) == true;

    /// <summary>
    ///     将对象转换为 JSON 字符串
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string ToJsonString(this object? obj, JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.Serialize(obj, jsonSerializerOptions ?? HttpRemoteOptions.JsonSerializerOptionsDefault);

    /// <summary>
    ///     检查是否启用 JSON 响应反序列化包装器
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsEnableJsonResponseWrapping(this HttpResponseMessage? httpResponseMessage,
        IServiceProvider? serviceProvider)
    {
        // 检查是否局部启用或禁用 JSON 响应反序列化包装器
        if (httpResponseMessage?.RequestMessage?.Options.TryGetValue(
                new HttpRequestOptionsKey<string>(Constants.ENABLE_JSON_RESPONSE_WRAPPING_KEY), out var enableValue) ==
            true)
        {
            return enableValue == "TRUE";
        }

        // 否则使用全局配置
        return HttpRemoteUtility.ResolveHttpClientOptions(httpResponseMessage, serviceProvider)
            ?.UseJsonResponseWrapping == true;
    }

    /// <summary>
    ///     获取主机环境名
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? GetHostEnvironmentName(IServiceCollection services)
    {
        // 获取主机环境对象
        var hostEnvironment = services
            .FirstOrDefault(u => u.ServiceType.FullName == "Microsoft.Extensions.Hosting.IHostEnvironment")
            ?.ImplementationInstance;

        // 空检查
        return hostEnvironment is null
            ? null
            : Convert.ToString(hostEnvironment.GetType().GetProperty("EnvironmentName")?.GetValue(hostEnvironment));
    }

    /// <summary>
    ///     Unicode 转义正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="Regex" />
    /// </returns>
    [GeneratedRegex(@"\\u([0-9a-fA-F]{4})")]
    private static partial Regex UnicodeEscapeRegex();
}