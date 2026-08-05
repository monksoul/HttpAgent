namespace HttpAgent.Samples;

public static class HttpRequestBuilderConfiguratorExtensions
{
    /// <summary>
    ///     如果 HTTP 响应的 IsSuccessStatusCode 属性是 <c>false</c>，则引发异常
    /// </summary>
    /// <param name="configurator">
    ///     <see cref="HttpRequestBuilderConfigurator{THttpBuilder}" />
    /// </param>
    /// <typeparam name="THttpBuilder">派生构建器自身类型</typeparam>
    /// <returns>
    ///     <typeparamref name="THttpBuilder" />
    /// </returns>
    public static THttpBuilder EnsureSuccessStatusCode<THttpBuilder>(
        this HttpRequestBuilderConfigurator<THttpBuilder> configurator)
        where THttpBuilder : HttpRequestBuilderConfigurator<THttpBuilder>
    {
        return configurator.With(builder => builder.EnsureSuccessStatusCode());
    }
}