// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     基于内存缓存的配额管理器
/// </summary>
internal sealed class HttpQuotaManager : IHttpQuotaManager
{
    /// <summary>
    ///     计数器缓存字典
    /// </summary>
    /// <remarks>键格式：{httpClientName}:{quotaKey}。</remarks>
    internal readonly ConcurrentDictionary<string, HttpQuotaCounter> _counters = new();

    /// <summary>
    ///     策略名称到策略实例的映射（忽略大小写）
    /// </summary>
    internal readonly Dictionary<string, IHttpQuotaStrategy> _strategies;

    /// <summary>
    ///     <inheritdoc cref="HttpQuotaManager" />
    /// </summary>
    /// <param name="strategies"><see cref="IHttpQuotaStrategy" /> 集合</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpQuotaManager(IEnumerable<IHttpQuotaStrategy> strategies)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(strategies);

        _strategies = new Dictionary<string, IHttpQuotaStrategy>(StringComparer.OrdinalIgnoreCase);

        // 构建策略名称到策略实例的映射
        foreach (var strategy in strategies)
        {
            _strategies[strategy.Name] = strategy;
        }
    }

    /// <inheritdoc />
    public bool TryIncrement(string? httpClientName, string quotaKey, HttpQuotaLimit quotaLimit, out int current)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(quotaKey);
        ArgumentNullException.ThrowIfNull(quotaLimit);

        // 空检查
        if (string.IsNullOrWhiteSpace(quotaLimit.Strategy))
        {
            throw new InvalidOperationException(
                $"Quota limit for key '{quotaKey}' has no strategy specified. Please set {nameof(HttpQuotaLimit.Strategy)} to a registered IHttpQuotaStrategy name (e.g., \"daily\").");
        }

        // 尝试从映射表中获取配额策略实例
        if (!_strategies.TryGetValue(quotaLimit.Strategy, out var quotaStrategy))
        {
            throw new InvalidOperationException(
                $"No quota strategy registered with name '{quotaLimit.Strategy}' (required by quota key '{quotaKey}'). Please use `AddDefaultQuotaStrategies()` or `AddQuotaStrategy<T>()` on the HttpRemoteBuilder to register the strategy.");
        }

        // 小于或等于 0 检查
        if (quotaLimit.MaxCount <= 0)
        {
            throw new InvalidOperationException(
                $"Invalid MaxCount ({quotaLimit.MaxCount}) for quota key '{quotaKey}'. It must be greater than zero.");
        }

        // 初始化计数器缓存键
        var key = $"{httpClientName ?? string.Empty}:{quotaKey}";

        // 从缓存字典中获取或创建配额计数器
        var quotaCounter = _counters.GetOrAdd(key, _ => new HttpQuotaCounter());

        // 尝试获取一个配额，并更新 quotaCounter 的计数和窗口标识
        lock (quotaCounter)
        {
            return quotaStrategy.TryAcquire(quotaCounter, quotaLimit.MaxCount, out current);
        }
    }
}