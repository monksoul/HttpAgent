// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="HeaderAttribute" /> 特性提取器
/// </summary>
internal sealed class HeaderDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        /* 情况一：当特性作用于方法或接口时 */

        // 获取 HeaderAttribute 特性集合
        var headerAttributes = context.GetMethodDefinedCustomAttributes<HeaderAttribute>(true, false)?.ToArray();

        // 空检查
        if (headerAttributes is { Length: > 0 })
        {
            // 遍历所有 [Header] 特性并添加到 HttpRequestBuilder 中
            foreach (var headerAttribute in headerAttributes)
            {
                // 获取请求标头键
                var headerName = headerAttribute.Name;

                // 空检查
                ArgumentException.ThrowIfNullOrEmpty(headerName);

                // 设置请求标头
                if (headerAttribute.HasSetValue)
                {
                    httpRequestBuilder.WithHeader(headerName, headerAttribute.Value, headerAttribute.Escape,
                        headerAttribute.Replace, headerAttribute.Format);
                }
                else
                {
                    // 尝试将字符串按第一个冒号拆分为键值对
                    if (TrySplitHeader(headerName, out var key, out var value))
                    {
                        httpRequestBuilder.WithHeader(key, value, headerAttribute.Escape, headerAttribute.Replace);
                    }
                    else
                    {
                        httpRequestBuilder.RemoveHeaders(headerName);
                    }
                }
            }
        }

        /* 情况二：当特性作用于参数时 */

        // 查找所有贴有 [Header] 特性的参数集合
        var headerParameters = context.UnFrozenParameters.Where(u => u.Key.IsDefined(typeof(HeaderAttribute), true))
            .ToArray();

        // 空检查
        if (headerParameters.Length == 0)
        {
            return;
        }

        // 遍历所有贴有 [Header] 特性的参数
        foreach (var (parameter, value) in headerParameters)
        {
            // 获取 HeaderAttribute 特性集合
            var parameterHeaderAttributes = parameter.GetCustomAttributes<HeaderAttribute>(true);

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 遍历所有 [Header] 特性并添加到 HttpRequestBuilder 中
            foreach (var headerAttribute in parameterHeaderAttributes)
            {
                // 检查参数是否贴了 [AliasAs] 特性
                if (!aliasAsDefined)
                {
                    parameterName = string.IsNullOrWhiteSpace(headerAttribute.AliasAs)
                        ? string.IsNullOrWhiteSpace(headerAttribute.Name) ? parameterName : headerAttribute.Name.Trim()
                        : headerAttribute.AliasAs.Trim();
                }

                // 检查类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
                if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
                {
                    httpRequestBuilder.WithHeader(parameterName, value ?? headerAttribute.Value, headerAttribute.Escape,
                        headerAttribute.Replace, headerAttribute.Format);

                    continue;
                }

                // 空检查
                if (value is not null)
                {
                    httpRequestBuilder.WithHeaders(value, headerAttribute.Escape, headerAttribute.Replace);
                }
            }
        }
    }

    /// <summary>
    ///     尝试将字符串按第一个冒号拆分为键值对
    /// </summary>
    /// <remarks>如果输入非空且至少包含一个冒号，返回 <c>true</c>，否则返回 <c>false</c>。</remarks>
    /// <param name="input">待拆分的字符串</param>
    /// <param name="key">输出拆分后的键</param>
    /// <param name="value">输出拆分后的值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool TrySplitHeader(string input, [NotNullWhen(true)] out string? key, out string? value)
    {
        key = null;
        value = null;

        // 空检查
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        // 检查是否包含冒号
        var colonIndex = input.IndexOf(':');
        if (colonIndex < 0)
        {
            return false;
        }

        // 冒号前为 key，冒号后为 value
        key = input[..colonIndex].Trim();
        value = input[(colonIndex + 1)..].Trim();

        return true;
    }
}