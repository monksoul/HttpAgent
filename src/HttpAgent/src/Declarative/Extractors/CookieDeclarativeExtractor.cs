// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="CookieAttribute" /> 特性提取器
/// </summary>
internal sealed class CookieDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        /* 情况一：当特性作用于方法或接口时 */

        // 获取 CookieAttribute 特性集合
        var cookieAttributes = context.GetMethodDefinedCustomAttributes<CookieAttribute>(true, false)?.ToArray();

        // 空检查
        if (cookieAttributes is { Length: > 0 })
        {
            // 遍历所有 [Cookie] 特性并添加到 HttpRequestBuilder 中
            foreach (var cookieAttribute in cookieAttributes)
            {
                // 获取 Cookie 键
                var cookieName = cookieAttribute.Name;

                // 空检查
                ArgumentException.ThrowIfNullOrEmpty(cookieName);

                // 设置 Cookies
                if (cookieAttribute.HasSetValue)
                {
                    httpRequestBuilder.WithCookie(cookieName, cookieAttribute.Value);
                }
                // 移除 Cookies
                else
                {
                    httpRequestBuilder.RemoveCookies(cookieName);
                }
            }
        }

        /* 情况二：当特性作用于参数时 */

        // 查找所有贴有 [Cookie] 特性的参数集合
        var cookieParameters = context.UnFrozenParameters.Where(u => u.Key.IsDefined(typeof(CookieAttribute), true))
            .ToArray();

        // 空检查
        if (cookieParameters.Length == 0)
        {
            return;
        }

        // 遍历所有贴有 [Cookie] 特性的参数
        foreach (var (parameter, value) in cookieParameters)
        {
            // 获取 CookieAttribute 特性集合
            var parameterCookieAttributes = parameter.GetCustomAttributes<CookieAttribute>(true);

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 遍历所有 [Cookie] 特性并添加到 HttpRequestBuilder 中
            foreach (var cookieAttribute in parameterCookieAttributes)
            {
                // 检查参数是否贴了 [AliasAs] 特性
                if (!aliasAsDefined)
                {
                    parameterName = string.IsNullOrWhiteSpace(cookieAttribute.AliasAs)
                        ? string.IsNullOrWhiteSpace(cookieAttribute.Name) ? parameterName : cookieAttribute.Name.Trim()
                        : cookieAttribute.AliasAs.Trim();
                }

                // 检查类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
                if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
                {
                    httpRequestBuilder.WithCookie(parameterName, value ?? cookieAttribute.Value);

                    continue;
                }

                // 空检查
                if (value is not null)
                {
                    httpRequestBuilder.WithCookies(value);
                }
            }
        }
    }
}