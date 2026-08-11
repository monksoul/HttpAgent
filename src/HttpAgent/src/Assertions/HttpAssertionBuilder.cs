// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求断言构建器
/// </summary>
public sealed partial class HttpAssertionBuilder
{
    /// <summary>
    ///     请求断言委托集合
    /// </summary>
    /// <remarks>在发送请求前执行。</remarks>
    internal readonly List<HttpAssertion> _requestAssertions;

    /// <summary>
    ///     响应断言委托集合
    /// </summary>
    /// <remarks>在收到响应后执行。</remarks>
    internal readonly List<HttpAssertion> _responseAssertions;

    /// <summary>
    ///     <inheritdoc cref="HttpAssertionBuilder" />
    /// </summary>
    internal HttpAssertionBuilder()
    {
        _requestAssertions = [];
        _responseAssertions = [];
    }

    /// <summary>
    ///     添加自定义断言委托
    /// </summary>
    /// <remarks>默认视为响应断言。</remarks>
    /// <param name="assertion">
    ///     <see cref="HttpAssertion" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpAssertionBuilder AddAssertion(HttpAssertion assertion)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assertion);

        _responseAssertions.Add(assertion);

        return this;
    }

    /// <summary>
    ///     获取请求断言委托集合
    /// </summary>
    /// <returns>
    ///     <see cref="IReadOnlyList{T}" />
    /// </returns>
    internal IReadOnlyList<HttpAssertion> GetRequestAssertions() => _requestAssertions;

    /// <summary>
    ///     获取响应断言委托集合
    /// </summary>
    /// <returns>
    ///     <see cref="IReadOnlyList{T}" />
    /// </returns>
    internal IReadOnlyList<HttpAssertion> GetResponseAssertions() => _responseAssertions;
}