// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IActionResult" /> 内容转换器
/// </summary>
public class IActionResultContentConverter : HttpContentConverterBase<IActionResult>
{
    /// <inheritdoc />
    public override IActionResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 尝试为无内容响应生成对应的 IActionResult
        if (TryGetEmptyContentResult(httpResponseMessage, out var statusCode, out var emptyContentResult))
        {
            return emptyContentResult;
        }

        // 获取响应内容标头
        var contentHeaders = httpResponseMessage.Content.Headers;

        // 获取内容类型
        var contentType = contentHeaders.ContentType;
        var mediaType = contentType?.MediaType;

        switch (mediaType)
        {
            case MediaTypeNames.Application.Json:
            case MediaTypeNames.Application.JsonPatch:
            case MediaTypeNames.Application.Xml:
            case MediaTypeNames.Application.XmlPatch:
            case MediaTypeNames.Text.Xml:
            case MediaTypeNames.Text.Html:
            case MediaTypeNames.Text.Plain:
            case MediaTypeNames.Application.Soap:
                // 读取字符串内容
                var stringContent = httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter()
                    .GetResult();

                return new ContentResult
                {
                    Content = stringContent, StatusCode = (int)statusCode, ContentType = contentType?.ToString()
                };
            default:
                // 读取流内容
                var streamContent = httpResponseMessage.Content.ReadAsStream(cancellationToken);

                return new FileStreamResult(streamContent, contentType?.ToString() ?? MediaTypeNames.Application.Octet)
                {
                    // 尝试从响应标头 Content-Disposition 中解析文件名
                    FileDownloadName = Helpers.ExtractFileNameFromContentDisposition(contentHeaders.ContentDisposition),
                    LastModified = contentHeaders.LastModified?.UtcDateTime
                };
        }
    }

    /// <inheritdoc />
    public override async Task<IActionResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 尝试为无内容响应生成对应的 IActionResult
        if (TryGetEmptyContentResult(httpResponseMessage, out var statusCode, out var emptyContentResult))
        {
            return emptyContentResult;
        }

        // 获取响应内容标头
        var contentHeaders = httpResponseMessage.Content.Headers;

        // 获取内容类型
        var contentType = contentHeaders.ContentType;
        var mediaType = contentType?.MediaType;

        switch (mediaType)
        {
            case MediaTypeNames.Application.Json:
            case MediaTypeNames.Application.JsonPatch:
            case MediaTypeNames.Application.Xml:
            case MediaTypeNames.Application.XmlPatch:
            case MediaTypeNames.Text.Xml:
            case MediaTypeNames.Text.Html:
            case MediaTypeNames.Text.Plain:
            case MediaTypeNames.Application.Soap:
                // 读取字符串内容
                var stringContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                return new ContentResult
                {
                    Content = stringContent, StatusCode = (int)statusCode, ContentType = contentType?.ToString()
                };
            default:
                // 读取流内容
                var streamContent = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);

                return new FileStreamResult(streamContent, contentType?.ToString() ?? MediaTypeNames.Application.Octet)
                {
                    // 尝试从响应标头 Content-Disposition 中解析文件名
                    FileDownloadName = Helpers.ExtractFileNameFromContentDisposition(contentHeaders.ContentDisposition),
                    LastModified = contentHeaders.LastModified?.UtcDateTime
                };
        }
    }

    /// <summary>
    ///     尝试为无内容响应生成对应的 <see cref="IActionResult" />
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <param name="emptyContentResult">
    ///     <see cref="IActionResult" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool TryGetEmptyContentResult(HttpResponseMessage httpResponseMessage,
        out HttpStatusCode statusCode, [NotNullWhen(true)] out IActionResult? emptyContentResult)
    {
        // 获取状态码
        statusCode = httpResponseMessage.StatusCode;

        emptyContentResult = statusCode switch
        {
            // 无响应体的状态码
            HttpStatusCode.NoContent => new NoContentResult(),
            HttpStatusCode.ResetContent => new StatusCodeResult((int)HttpStatusCode.ResetContent),
            // 304 特殊处理：返回空内容但必须保留 ETag/Last-Modified 等验证头
            HttpStatusCode.NotModified => new StatusCodeResult((int)HttpStatusCode.NotModified),
            HttpStatusCode.SwitchingProtocols => new StatusCodeResult((int)HttpStatusCode.SwitchingProtocols),
            HttpStatusCode.Processing => new StatusCodeResult((int)HttpStatusCode.Processing),

            // 可能存在响应体的状态码。这里不应该处理，因为它们通常包含错误详情
            // HttpStatusCode.BadRequest => new BadRequestResult(),
            // HttpStatusCode.Unauthorized => new UnauthorizedResult(),
            // HttpStatusCode.NotFound => new NotFoundResult(),
            // HttpStatusCode.Conflict => new ConflictResult(),
            // HttpStatusCode.UnsupportedMediaType => new UnsupportedMediaTypeResult(),
            // HttpStatusCode.UnprocessableEntity => new UnprocessableEntityResult(),
            _ => null
        };

        return emptyContentResult is not null;
    }
}