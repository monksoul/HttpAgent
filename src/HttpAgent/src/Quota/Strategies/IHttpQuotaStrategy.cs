// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     配额策略接口
/// </summary>
public interface IHttpQuotaStrategy
{
    /// <summary>
    ///     策略的唯一名称
    /// </summary>
    /// <remarks>用于配置中的 <see cref="HttpQuotaLimit.Strategy" /> 字段进行匹配。</remarks>
    string Name { get; }

    /// <summary>
    ///     尝试获取一个配额，并更新 <paramref name="quotaCounter" /> 的计数和窗口标识
    /// </summary>
    /// <param name="quotaCounter">
    ///     <see cref="HttpQuotaCounter" />
    /// </param>
    /// <param name="maxCount">最大允许调用次数</param>
    /// <param name="current">申请后的当前计数</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool TryAcquire(HttpQuotaCounter quotaCounter, int maxCount, out int current);
}