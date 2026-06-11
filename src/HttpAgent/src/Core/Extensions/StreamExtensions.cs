// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="Stream" /> 扩展类
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    ///     将字节数组写入本地文件
    /// </summary>
    /// <param name="data">字节数组</param>
    /// <param name="filePath">目标文件路径</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task SaveToFileAsync(this byte[] data, string filePath,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 初始化 MemoryStream 实例
        using var stream = new MemoryStream(data, false);

        await stream.SaveToFileAsync(filePath, cancellationToken);
    }

    /// <summary>
    ///     将字节数组写入本地文件
    /// </summary>
    /// <param name="data">字节数组</param>
    /// <param name="filePath">目标文件路径</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void SaveToFile(this byte[] data, string filePath)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 初始化 MemoryStream 实例
        using var stream = new MemoryStream(data, false);

        stream.SaveToFile(filePath);
    }

    /// <summary>
    ///     将流从当前位置开始的内容写入本地文件
    /// </summary>
    /// <remarks>如果希望总是从流的开头保存，可提前设置：<c>stream.Seek(0, SeekOrigin.Begin);</c>。</remarks>
    /// <param name="stream">
    ///     <see cref="Stream" />
    /// </param>
    /// <param name="filePath">目标文件路径</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task SaveToFileAsync(this Stream stream, string filePath,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 解析文件路径
        var resolvedPath = ResolveFilePath(filePath);

        // 获取路径的目录路径
        var directory = Path.GetDirectoryName(resolvedPath);

        // 存在检查
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            // 创建目标文件夹
            Directory.CreateDirectory(directory);
        }

        // 初始化 FileStream 实例
        await using var fileStream = new FileStream(resolvedPath, FileMode.Create, FileAccess.Write, FileShare.None,
            4096, true);

        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    /// <summary>
    ///     将流从当前位置开始的内容写入本地文件
    /// </summary>
    /// <remarks>如果希望总是从流的开头保存，可提前设置：<c>stream.Seek(0, SeekOrigin.Begin);</c>。</remarks>
    /// <param name="stream">
    ///     <see cref="Stream" />
    /// </param>
    /// <param name="filePath">目标文件路径</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void SaveToFile(this Stream stream, string filePath)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 解析文件路径
        var resolvedPath = ResolveFilePath(filePath);

        // 获取路径的目录路径
        var directory = Path.GetDirectoryName(resolvedPath);

        // 存在检查
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            // 创建目标文件夹
            Directory.CreateDirectory(directory);
        }

        // 初始化 FileStream 实例
        using var fileStream = new FileStream(resolvedPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096);

        stream.CopyTo(fileStream);
    }

    /// <summary>
    ///     解析文件路径
    /// </summary>
    /// <remarks>如果已是绝对路径，直接返回。如果是相对路径，基于 <see cref="AppContext.BaseDirectory" /> 解析。</remarks>
    /// <param name="filePath">文件路径</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string ResolveFilePath(string filePath)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 移除前后空格
        filePath = filePath.Trim();

        // 处理绝对和相对路径问题
        var basePath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(AppContext.BaseDirectory, filePath);

        return Path.GetFullPath(basePath);
    }
}