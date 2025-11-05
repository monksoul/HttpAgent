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
    ///     断言委托集合
    /// </summary>
    internal readonly List<HttpAssertion> _assertions;

    /// <summary>
    ///     <inheritdoc cref="HttpAssertionBuilder" />
    /// </summary>
    internal HttpAssertionBuilder() => _assertions = [];

    /// <summary>
    ///     添加自定义断言委托
    /// </summary>
    /// <param name="assertion">
    ///     <see cref="HttpAssertion" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    public HttpAssertionBuilder AddAssertion(HttpAssertion assertion)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assertion);

        _assertions.Add(assertion);

        return this;
    }

    /// <summary>
    ///     获取断言委托集合
    /// </summary>
    /// <returns>
    ///     <see cref="IReadOnlyList{T}" />
    /// </returns>
    internal IReadOnlyList<HttpAssertion> GetAssertions() => _assertions;
}