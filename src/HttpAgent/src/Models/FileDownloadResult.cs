// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     文件下载结果信息
/// </summary>
public sealed class FileDownloadResult
{
    /// <summary>
    ///     下载是否成功完成
    /// </summary>
    /// <remarks>因文件存在而跳过也被视为成功。</remarks>
    public bool IsSuccess { get; internal set; }

    /// <summary>
    ///     下载的文件 URL
    /// </summary>
    public Uri? RequestUri { get; internal set; }

    /// <summary>
    ///     文件保存的目标路径
    /// </summary>
    public string? FilePath { get; internal set; }

    /// <summary>
    ///     文件的总大小
    /// </summary>
    /// <remarks>以字节为单位。</remarks>
    public long FileSize { get; internal set; }

    /// <summary>
    ///     下载耗时
    /// </summary>
    /// <remarks>以毫秒为单位。</remarks>
    public long ElapsedMilliseconds { get; internal set; }

    /// <summary>
    ///     响应状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; internal set; }

    /// <summary>
    ///     当目标文件已存在时的行为
    /// </summary>
    public FileExistsBehavior FileExistsBehavior { get; internal set; }

    /// <summary>
    ///     是否因为文件已存在且配置为跳过而未执行下载
    /// </summary>
    public bool WasSkipped { get; internal set; }

    /// <summary>
    ///     下载是否使用了多线程分块
    /// </summary>
    public bool UsedMultiThreadedDownload { get; internal set; }
}