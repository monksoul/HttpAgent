// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求模块常量配置
/// </summary>
internal static class Constants
{
    /// <summary>
    ///     请求跟踪标识标头
    /// </summary>
    internal const string X_TRACE_ID_HEADER = "X-Trace-ID";

    /// <summary>
    ///     未知 <c>User Agent</c> 版本
    /// </summary>
    internal const string UNKNOWN_USER_AGENT_VERSION = "unknown";

    /// <summary>
    ///     内容正文部分的处置类型
    /// </summary>
    internal const string FORM_DATA_DISPOSITION_TYPE = "form-data";

    /// <summary>
    ///     Basic 授权标识
    /// </summary>
    internal const string BASIC_AUTHENTICATION_SCHEME = "Basic";

    /// <summary>
    ///     JWT (JSON Web Token) 授权标识
    /// </summary>
    internal const string JWT_BEARER_AUTHENTICATION_SCHEME = "Bearer";

    /// <summary>
    ///     Digest 授权标识
    /// </summary>
    internal const string DIGEST_AUTHENTICATION_SCHEME = "Digest";

    /// <summary>
    ///     <c>text/plain</c> 内容类型
    /// </summary>
    internal const string TEXT_PLAIN_MIME_TYPE = "text/plain";

    /// <summary>
    ///     响应结束符标头
    /// </summary>
    internal const string X_END_OF_STREAM_HEADER = "X-End-Of-Stream";

    /// <summary>
    ///     请求原始地址标头
    /// </summary>
    internal const string X_ORIGINAL_URL_HEADER = "X-Original-URL";

    /// <summary>
    ///     请求转发目标地址标头
    /// </summary>
    internal const string X_FORWARD_TO_HEADER = "X-Forward-To";

    /// <summary>
    ///     压力测试标头
    /// </summary>
    internal const string X_STRESS_TEST_HEADER = "X-Stress-Test";

    /// <summary>
    ///     压力测试标头值
    /// </summary>
    internal const string X_STRESS_TEST_VALUE = "Harness";

    /// <summary>
    ///     禁用请求分析工具键
    /// </summary>
    /// <remarks>被用于从 <see cref="HttpRequestMessage" /> 的 <c>Options</c> 属性中读取。</remarks>
    internal const string DISABLE_PROFILER_KEY = "__DISABLE_PROFILER__";

    /// <summary>
    ///     请求分析工具打印标识键
    /// </summary>
    /// <remarks>解决重复打印问题。被用于从 <see cref="HttpRequestMessage" /> 的 <c>Options</c> 属性中读取。</remarks>
    internal const string PROFILER_PRINTED_KEY = "__PROFILER_PRINTED__";

    /// <summary>
    ///     启用 JSON 响应反序列化包装器键
    /// </summary>
    /// <remarks>被用于从 <see cref="HttpRequestMessage" /> 的 <c>Options</c> 属性中读取。</remarks>
    internal const string ENABLE_JSON_RESPONSE_WRAPPING_KEY = "__ENABLE__JSON_RESPONSE_WRAPPING__";

    /// <summary>
    ///     HTTP 声明式请求方法签名键
    /// </summary>
    /// <remarks>被用于从 <see cref="HttpRequestMessage" /> 的 <c>Options</c> 属性中读取。</remarks>
    internal const string DECLARATIVE_METHOD_KEY = "__DECLARATIVE_METHOD__";

    /// <summary>
    ///     HTTP 请求 <see cref="HttpClient" /> 实例的配置名称键
    /// </summary>
    /// <remarks>被用于从 <see cref="HttpRequestMessage" /> 的 <c>Options</c> 属性中读取。</remarks>
    internal const string HTTP_CLIENT_NAME = "__HTTP_CLIENT_NAME__";

    /// <summary>
    ///     浏览器的 <c>User-Agent</c> 标头值
    /// </summary>
    internal const string USER_AGENT_OF_BROWSER =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0";

    /// <summary>
    ///     移动端浏览器的 <c>User-Agent</c> 标头值
    /// </summary>
    internal const string USER_AGENT_OF_MOBILE_BROWSER =
        "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Mobile Safari/537.36 Edg/142.0.0.0";

    /// <summary>
    ///     <c>Referer</c> 标头请求基地址模板
    /// </summary>
    internal const string REFERER_HEADER_BASE_ADDRESS_TEMPLATE = "{BASE_ADDRESS}";
}