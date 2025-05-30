// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求选项
/// </summary>
public sealed class HttpRemoteOptions
{
    /// <summary>
    ///     默认 JSON 序列化配置
    /// </summary>
    /// <remarks>参考文献：https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/configure-options。</remarks>
    public static readonly JsonSerializerOptions JsonSerializerOptionsDefault = new(JsonSerializerOptions.Default)
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // 允许 String 转 Number
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        // 解决中文乱码问题
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        AllowTrailingCommas = true,
        Converters =
        {
            new FlexibleDateTimeConverter(),
            new FlexibleDateTimeOffsetConverter(),
            // 允许 Number 或 Boolean 转 String
            new StringJsonConverter()
        }
    };

    /// <summary>
    ///     默认请求内容类型
    /// </summary>
    public string? DefaultContentType { get; set; } = Constants.TEXT_PLAIN_MIME_TYPE;

    /// <summary>
    ///     默认文件下载保存目录
    /// </summary>
    public string? DefaultFileDownloadDirectory { get; set; }

    /// <summary>
    ///     请求分析工具日志级别
    /// </summary>
    /// <remarks>默认值为 <see cref="LogLevel.Warning" /></remarks>
    public LogLevel ProfilerLogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    ///     指示请求是否应遵循重定向响应
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool AllowAutoRedirect { get; set; } = true;

    /// <summary>
    ///     请求所遵循的最大重定向数
    /// </summary>
    /// <remarks>默认值为：50 次。</remarks>
    public int MaximumAutomaticRedirections { get; set; } = 50;

    /// <summary>
    ///     回退请求基地址
    /// </summary>
    /// <remarks>当未配置 <see cref="HttpClient" /> 的 <see cref="HttpClient.BaseAddress" /> 且请求地址为相对地址时使用。</remarks>
    public Uri? FallbackBaseAddress { get; set; }

    /// <summary>
    ///     JSON 序列化配置
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new(JsonSerializerOptionsDefault);

    /// <summary>
    ///     <inheritdoc cref="IConfiguration" />
    /// </summary>
    /// <remarks>支持作为替换 URL 地址中配置模板参数的提供源。</remarks>
    public IConfiguration? Configuration { get; set; }

    /// <summary>
    ///     URL 参数格式化程序
    /// </summary>
    public IUrlParameterFormatter? UrlParameterFormatter { get; set; } = new UrlParameterFormatter();

    /// <summary>
    ///     自定义 HTTP 声明式 <see cref="IHttpDeclarativeExtractor" /> 集合提供器
    /// </summary>
    /// <value>返回多个包含实现 <see cref="IHttpDeclarativeExtractor" /> 集合的集合。</value>
    internal IReadOnlyList<Func<IEnumerable<IHttpDeclarativeExtractor>>>? HttpDeclarativeExtractors { get; set; }

    /// <summary>
    ///     指示是否配置（注册）了日志程序
    /// </summary>
    internal bool IsLoggingRegistered { get; set; }
}