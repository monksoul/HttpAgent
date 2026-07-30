// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     ETag 缓存接口
/// </summary>
public interface IHttpETagCache
{
    /// <summary>
    ///     尝试获取指定键的缓存项
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="eTagCacheItem">
    ///     <see cref="HttpETagCacheItem" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool TryGet(string cacheKey, out HttpETagCacheItem? eTagCacheItem);

    /// <summary>
    ///     设置或更新指定键的缓存项
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="eTagCacheItem">
    ///     <see cref="HttpETagCacheItem" />
    /// </param>
    void Set(string cacheKey, HttpETagCacheItem eTagCacheItem);

    /// <summary>
    ///     移除指定键的缓存项
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    void Remove(string cacheKey);
}