﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="ProfilerAttribute" /> 特性提取器
/// </summary>
internal sealed class ProfilerDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [Profiler] 特性
        if (!context.IsMethodDefined<ProfilerAttribute>(out var profilerAttribute, true))
        {
            return;
        }

        // 设置是否启用请求分析工具
        httpRequestBuilder.Profiler(profilerAttribute.Enabled);
    }
}