// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 提取器抽象基类
/// </summary>
public abstract class HttpCurlExtractorBase : IHttpCurlExtractor
{
    /// <summary>
    ///     当前提取器匹配的命令标志集合
    /// </summary>
    /// <remarks>如 <c>["-H", "--header"]</c>。</remarks>
    protected abstract string[] Flags { get; }

    /// <summary>
    ///     指示该命令是否携带参数
    /// </summary>
    /// <remarks>如 <c>-H</c> 携带，<c>--compressed</c> 不携带。默认为 <c>true</c>。</remarks>
    protected virtual bool RequiresArgument => true;

    /// <inheritdoc />
    public bool TryExtract(HttpRequestBuilder httpRequestBuilder, HttpCurlParsingContext context)
    {
        // 检查当前 Token 是否匹配命令标志集合的任一值
        if (!context.CurrentTokenMatches(Flags))
        {
            return false;
        }

        // 获取当前匹配的命令标志
        var currentFlag = context.CurrentToken.ToLowerInvariant();

        string? argument = null;

        // 检查该命令是否携带参数
        if (RequiresArgument)
        {
            // 读取参数
            argument = context.PeekNext();
        }

        // 调用派生类的提取信息并构建 HttpRequestBuilder 实例
        Extract(httpRequestBuilder, currentFlag, argument);

        // 推进游标
        context.Advance(RequiresArgument && argument is not null ? 2 : 1);

        return true;
    }

    /// <summary>
    ///     提取信息并构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="flag">当前匹配的命令标志</param>
    /// <param name="argument">携带的参数值</param>
    protected abstract void Extract(HttpRequestBuilder httpRequestBuilder, string flag, string? argument);
}