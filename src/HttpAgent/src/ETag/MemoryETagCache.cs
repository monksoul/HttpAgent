// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     基于内存的 ETag 缓存
/// </summary>
internal sealed class MemoryETagCache : IHttpETagCache
{
    /// <summary>
    ///     内部缓存字典
    /// </summary>
    internal readonly ConcurrentDictionary<string, HttpETagCacheItem> _cache = new();

    /// <inheritdoc />
    public bool TryGet(string cacheKey, out HttpETagCacheItem? eTagCacheItem)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);

        return _cache.TryGetValue(cacheKey, out eTagCacheItem);
    }

    /// <inheritdoc />
    public void Set(string cacheKey, HttpETagCacheItem eTagCacheItem)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);
        ArgumentNullException.ThrowIfNull(eTagCacheItem);

        _cache[cacheKey] = eTagCacheItem;
    }

    /// <inheritdoc />
    public void Remove(string cacheKey)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);

        _cache.TryRemove(cacheKey, out _);
    }
}