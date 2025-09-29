// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     文件传输结果信息
/// </summary>
public sealed class FileTransferResult
{
    /// <summary>
    ///     传输是否成功完成
    /// </summary>
    public bool IsSuccess { get; internal set; }

    /// <summary>
    ///     文件传输 URL
    /// </summary>
    public Uri? RequestUri { get; internal set; }

    /// <summary>
    ///     文件的路径
    /// </summary>
    public string? FilePath { get; internal set; }

    /// <summary>
    ///     文件的大小
    /// </summary>
    /// <remarks>以字节为单位。</remarks>
    public long FileSize { get; internal set; }

    /// <summary>
    ///     传输耗时
    /// </summary>
    /// <remarks>以毫秒为单位。</remarks>
    public long ElapsedMilliseconds { get; internal set; }

    /// <summary>
    ///     响应状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; internal set; }
}