// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpMultipartFormDataBuilder" /> 扩展类
/// </summary>
public static class HttpMultipartFormDataBuilderExtensions
{
    /// <summary>
    ///     添加文件
    /// </summary>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="formFile">
    ///     <see cref="IFormFile" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public static HttpMultipartFormDataBuilder AddFile(this HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        IFormFile formFile, string? name = null, string? fileName = null, string? contentType = null,
        Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formFile);

        // 添加文件流
        return httpMultipartFormDataBuilder.AddStream(formFile.OpenReadStream(), name ?? formFile.Name,
            fileName ?? formFile.FileName, contentType ?? formFile.ContentType, contentEncoding,
            true);
    }

    /// <summary>
    ///     添加多个文件
    /// </summary>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="formFiles">
    ///     <see cref="IFormFile" /> 集合
    /// </param>
    /// <param name="name">表单名称</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public static HttpMultipartFormDataBuilder AddFiles(this HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        IEnumerable<IFormFile> formFiles, string? name = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formFiles);

        // 逐条添加文件
        foreach (var formFile in formFiles)
        {
            httpMultipartFormDataBuilder.AddFile(formFile, name ?? formFile.Name);
        }

        return httpMultipartFormDataBuilder;
    }

    /// <summary>
    ///     添加文件
    /// </summary>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="browserFile">
    ///     <see cref="IBrowserFile" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="maxAllowedSize">流可以提供的最大字节数，默认值为：<c>500KB</c></param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public static HttpMultipartFormDataBuilder AddFile(this HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        IBrowserFile browserFile, string? name = null, string? fileName = null, string? contentType = null,
        Encoding? contentEncoding = null, long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(browserFile);

        // 添加文件流
        return httpMultipartFormDataBuilder.AddStream(browserFile.OpenReadStream(maxAllowedSize, cancellationToken),
            name ?? "file", fileName ?? browserFile.Name, contentType ?? browserFile.ContentType, contentEncoding,
            true);
    }

    /// <summary>
    ///     添加多个文件
    /// </summary>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="browserFiles">
    ///     <see cref="IBrowserFile" /> 集合
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="maxAllowedSize">流可以提供的最大字节数，默认值为：<c>500KB</c></param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public static HttpMultipartFormDataBuilder AddFiles(this HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        IEnumerable<IBrowserFile> browserFiles, string? name = null, long maxAllowedSize = 512000,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(browserFiles);

        // 逐条添加文件
        foreach (var browserFile in browserFiles)
        {
            httpMultipartFormDataBuilder.AddFile(browserFile, name ?? "file", maxAllowedSize: maxAllowedSize,
                cancellationToken: cancellationToken);
        }

        return httpMultipartFormDataBuilder;
    }
}