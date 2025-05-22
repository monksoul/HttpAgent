// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="PathSegmentAttribute" /> 特性提取器
/// </summary>
internal sealed class PathSegmentDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        /* 情况一：当特性作用于方法或接口时 */

        // 获取 PathSegmentAttribute 特性集合
        var pathSegmentAttributes =
            context.GetMethodDefinedCustomAttributes<PathSegmentAttribute>(true, false)?.ToArray();

        // 空检查
        if (pathSegmentAttributes is { Length: > 0 })
        {
            // 遍历所有 [PathSegment] 特性并添加到 HttpRequestBuilder 中
            foreach (var pathSegmentAttribute in pathSegmentAttributes)
            {
                // 获取路径片段
                var segment = pathSegmentAttribute.Segment;

                // 空检查
                ArgumentException.ThrowIfNullOrWhiteSpace(segment);

                // 设置路径片段
                if (!pathSegmentAttribute.Remove)
                {
                    httpRequestBuilder.WithPathSegment(segment);
                }
                // 移除路径片段
                else
                {
                    httpRequestBuilder.RemovePathSegments(segment);
                }
            }
        }

        /* 情况二：当特性作用于参数时 */

        // 查找所有贴有 [PathSegment] 特性的参数集合
        var pathSegmentParameters = context.UnFrozenParameters
            .Where(u => u.Key.IsDefined(typeof(PathSegmentAttribute), true))
            .ToArray();

        // 空检查
        if (pathSegmentParameters.Length == 0)
        {
            return;
        }

        // 遍历所有贴有 [PathSegment] 特性的参数
        foreach (var (parameter, value) in pathSegmentParameters)
        {
            // 获取 PathSegmentAttribute 特性集合
            var parameterPathSegmentAttributes = parameter.GetCustomAttributes<PathSegmentAttribute>(true);

            // 遍历所有 [PathSegment] 特性并添加到 HttpRequestBuilder 中
            foreach (var pathSegmentAttribute in parameterPathSegmentAttributes)
            {
                // 空检查
                if (value is null && string.IsNullOrWhiteSpace(pathSegmentAttribute.Segment))
                {
                    continue;
                }

                // 获取路径片段：若参数值为 null，则使用 [PathSegment] 特性的 Segment
                var segment = value?.ToString() ?? pathSegmentAttribute.Segment;

                // 空检查
                if (string.IsNullOrWhiteSpace(segment))
                {
                    continue;
                }

                // 设置路径片段
                if (!pathSegmentAttribute.Remove)
                {
                    httpRequestBuilder.WithPathSegment(segment);
                }
                // 移除路径片段
                else
                {
                    httpRequestBuilder.RemovePathSegments(segment);
                }
            }
        }
    }
}