// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="MultipartFormDataContent" /> 构建器
/// </summary>
public sealed class HttpMultipartFormDataBuilder
{
    /// <inheritdoc cref="HttpRequestBuilder" />
    internal readonly HttpRequestBuilder _httpRequestBuilder;

    /// <summary>
    ///     <see cref="MultipartFormDataItem" /> 集合
    /// </summary>
    internal readonly List<MultipartFormDataItem> _partContents;

    /// <inheritdoc cref="OnPreAddContent" />
    internal Action<HttpContent, string>? _onPreAddContent;

    /// <summary>
    ///     用于处理在添加 <see cref="HttpContent" /> 表单项内容前，对表单名称进行自定义转换的转换器
    /// </summary>
    internal Func<string, string?>? FormNameTransformer;

    /// <summary>
    ///     <inheritdoc cref="HttpMultipartFormDataBuilder" />
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal HttpMultipartFormDataBuilder(HttpRequestBuilder httpRequestBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestBuilder);

        _httpRequestBuilder = httpRequestBuilder;
        _partContents = [];
    }

    /// <summary>
    ///     多部分表单内容的边界
    /// </summary>
    public string? Boundary { get; set; } = $"--------------------------{DateTime.Now.Ticks:x}";

    /// <summary>
    ///     是否移除默认的多部分内容的 <c>Content-Type</c>
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool OmitContentType { get; set; } = true;

    /// <summary>
    ///     用于处理在添加 <see cref="HttpContent" /> 表单项内容时的操作
    /// </summary>
    internal Action<HttpContent, string>? OnPreAddContent
    {
        get => _onPreAddContent;
        private set => _onPreAddContent = value;
    }

    /// <summary>
    ///     设置多部分表单内容的边界
    /// </summary>
    /// <param name="boundary">多部分表单内容的边界</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder SetBoundary(string? boundary)
    {
        Boundary = boundary;

        return this;
    }

    /// <summary>
    ///     设置用于处理在添加 <see cref="HttpContent" /> 表单项内容时的操作
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder SetOnPreAddContent(Action<HttpContent, string> configure)
    {
        configure.Combine(ref _onPreAddContent);

        return this;
    }

    /// <summary>
    ///     设置用于处理在添加 <see cref="HttpContent" /> 表单项内容前，对表单名称进行自定义转换的转换器
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder SetFormNameTransformer(Func<string, string>? configure)
    {
        FormNameTransformer = configure;

        return this;
    }

    /// <summary>
    ///     设置用于处理在添加 <see cref="HttpContent" /> 表单项内容前，对表单名称进行自定义转换的转换器
    /// </summary>
    /// <param name="namingPolicy">
    ///     <see cref="FormNamingPolicy" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public HttpMultipartFormDataBuilder SetFormNameTransformer(FormNamingPolicy namingPolicy) =>
        SetFormNameTransformer(namingPolicy switch
        {
            FormNamingPolicy.None => null,
            FormNamingPolicy.CamelCase => JsonNamingPolicy.CamelCase.ConvertName,
            FormNamingPolicy.SnakeCaseLower => JsonNamingPolicy.SnakeCaseLower.ConvertName,
            FormNamingPolicy.SnakeCaseUpper => JsonNamingPolicy.SnakeCaseUpper.ConvertName,
            FormNamingPolicy.KebabCaseLower => JsonNamingPolicy.KebabCaseLower.ConvertName,
            FormNamingPolicy.KebabCaseUpper => JsonNamingPolicy.KebabCaseUpper.ConvertName,
            _ => throw new ArgumentOutOfRangeException(nameof(namingPolicy), namingPolicy, null)
        });

    /// <summary>
    ///     添加 JSON 内容
    /// </summary>
    /// <param name="rawJson">JSON 字符串/原始对象</param>
    /// <param name="name">表单名称。该值不为空时作为表单的一项。否则将遍历对象类型的每一个公开属性作为表单的项。</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="contentType">内容类型</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    /// <exception cref="JsonException"></exception>
    public HttpMultipartFormDataBuilder AddJson(object? rawJson, string? name = null, Encoding? contentEncoding = null,
        string? contentType = null)
    {
        // 检查是否配置表单名或不是字符串类型
        if (!string.IsNullOrWhiteSpace(name) || rawJson is not string rawString)
        {
            return AddObject(rawJson, name, contentType ?? MediaTypeNames.Application.Json, contentEncoding);
        }

        // 尝试验证并获取 JsonDocument 实例（需 using）
        var jsonDocument = JsonUtility.Parse(rawString);

        // 添加请求结束时需要释放的对象
        _httpRequestBuilder.AddDisposable(jsonDocument);

        return AddObject(jsonDocument, name, contentType ?? MediaTypeNames.Application.Json, contentEncoding);
    }

    /// <summary>
    ///     添加单个表单项内容
    /// </summary>
    /// <param name="value">表单值</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFormItem(object? value, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddObject(value, name, MediaTypeNames.Text.Plain, contentEncoding);
    }

    /// <summary>
    ///     添加 HTML 内容
    /// </summary>
    /// <param name="htmlString">HTML 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddHtml(string? htmlString, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddObject(htmlString, name, MediaTypeNames.Text.Html, contentEncoding);
    }

    /// <summary>
    ///     添加 XML 内容
    /// </summary>
    /// <param name="xmlString">XML 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="contentType">内容类型</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddXml(string? xmlString, string name, Encoding? contentEncoding = null,
        string? contentType = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddObject(xmlString, name, contentType ?? MediaTypeNames.Text.Xml, contentEncoding);
    }

    /// <summary>
    ///     添加文本内容
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddText(string? text, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddObject(text, name, MediaTypeNames.Text.Plain, contentEncoding);
    }

    /// <summary>
    ///     添加对象内容
    /// </summary>
    /// <param name="rawObject">原始对象</param>
    /// <param name="name">表单名称。该值不为空时作为表单的一项。否则将遍历对象类型的每一个公开属性作为表单的项。</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddObject(object? rawObject, string? name = null, string? contentType = null,
        Encoding? contentEncoding = null)
    {
        // 解析内容类型字符串
        Encoding? encoding = null;
        var mediaType = string.IsNullOrWhiteSpace(contentType)
            ? Constants.TEXT_PLAIN_MIME_TYPE
            : ParseContentType(contentType, contentEncoding, out encoding);

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaType);

        // 检查是否配置表单名
        if (!string.IsNullOrWhiteSpace(name))
        {
            _partContents.Add(new MultipartFormDataItem(name)
            {
                ContentType = mediaType, RawContent = rawObject, ContentEncoding = encoding
            });

            return this;
        }

        // 空检查
        ArgumentNullException.ThrowIfNull(rawObject);

        // 将对象转换为字典集合再追加
        var formDataItems = rawObject.ObjectToDictionary()!;

        // 遍历字典集合并逐条追加
        foreach (var (itemName, rawContent) in formDataItems)
        {
            // 检查原始请求内容是否是 MultipartFile 类型
            if (rawContent is MultipartFile multipartFile)
            {
                AddFile(multipartFile);
            }
            else
            {
                _partContents.Add(new MultipartFormDataItem(itemName.ToInvariantCultureString()!)
                {
                    ContentType = Helpers.GetContentTypeOrDefault(rawContent, MediaTypeNames.Text.Plain),
                    RawContent = rawContent,
                    ContentEncoding = encoding
                });
            }
        }

        return this;
    }

    /// <summary>
    ///     从互联网 URL 中添加文件
    /// </summary>
    /// <remarks>文件大小限制在 <c>100MB</c> 以内。</remarks>
    /// <param name="url">互联网 URL 地址</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public HttpMultipartFormDataBuilder AddFileFromRemote(string url, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 尝试获取文件的名称
        var newFileName = fileName ?? Helpers.GetFileNameFromUri(new Uri(url, UriKind.Absolute));

        // 从互联网 URL 地址中加载流
        var fileStream = Helpers.GetStreamFromRemote(url);

        return AddStream(fileStream, name, newFileName, contentType, contentEncoding,
            true);
    }

    /// <summary>
    ///     从 Base64 字符串中添加文件
    /// </summary>
    /// <remarks>文件大小限制在 <c>100MB</c> 以内。</remarks>
    /// <param name="base64String">Base64 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public HttpMultipartFormDataBuilder AddFileFromBase64String(string base64String, string name = "file",
        string? fileName = null, string? contentType = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(base64String);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 将 Base64 字符串转换成字节数组
        var bytes = Convert.FromBase64String(base64String);

        // 获取字节数组长度
        var fileLength = bytes.Length;

        // 限制文件字节数组大小在 100MB 以内
        const long maxFileSizeInBytes = 104857600L;
        if (fileLength > maxFileSizeInBytes)
        {
            throw new InvalidOperationException(
                $"The file size exceeds the maximum allowed size of `{maxFileSizeInBytes.ToSizeUnits("MB"):F2}MB`.");
        }

        return AddByteArray(bytes, name, fileName, contentType, contentEncoding);
    }

    /// <summary>
    ///     从本地路径中添加文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileAsStream(string filePath, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file `{filePath}` does not exist.");
        }

        // 获取文件的名称
        var newFileName = fileName ?? Path.GetFileName(filePath);

        // 读取文件流（没有 using）
        var fileStream = File.OpenRead(filePath);

        return AddStream(fileStream, name, newFileName, contentType, contentEncoding,
            true);
    }

    /// <summary>
    ///     从本地路径中添加文件（带文件传输进度）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="progressChannel">文件传输进度信息的通道</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileWithProgressAsStream(string filePath,
        Channel<FileTransferProgress> progressChannel, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(progressChannel);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file `{filePath}` does not exist.");
        }

        // 获取文件的名称
        var newFileName = fileName ?? Path.GetFileName(filePath);

        // 读取文件流（没有 using）
        var fileStream = File.OpenRead(filePath);

        // 初始化带读写进度的文件流
        var progressFileStream = new ProgressFileStream(fileStream, filePath, progressChannel, newFileName);

        return AddStream(progressFileStream, name, newFileName, contentType, contentEncoding,
            true);
    }

    /// <summary>
    ///     从本地路径中添加文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileAsByteArray(string filePath, string name = "file",
        string? fileName = null, string? contentType = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file `{filePath}` does not exist.");
        }

        // 获取文件的名称
        var newFileName = fileName ?? Path.GetFileName(filePath);

        // 读取文件字节数组
        var bytes = File.ReadAllBytes(filePath);

        return AddByteArray(bytes, name, newFileName, contentType, contentEncoding);
    }

    /// <summary>
    ///     添加文件
    /// </summary>
    /// <remarks>使用 <c>MultipartFile.CreateFrom[Source]</c> 静态方法创建。</remarks>
    /// <param name="multipartFile">
    ///     <see cref="MultipartFile" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFile(MultipartFile multipartFile, string? name = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(multipartFile);

        // 获取表单名称
        var formName = name ?? multipartFile.Name!;

        switch (multipartFile.FileSourceType)
        {
            // 字节数组
            case FileSourceType.ByteArray:
                return AddByteArray((byte[])multipartFile.Source!, formName, multipartFile.FileName,
                    multipartFile.ContentType, multipartFile.ContentEncoding);
            // Stream
            case FileSourceType.Stream:
                return AddStream((Stream)multipartFile.Source!, formName, multipartFile.FileName,
                    multipartFile.ContentType, multipartFile.ContentEncoding);
            // 本地文件路径
            case FileSourceType.Path:
                return AddFileAsStream((string)multipartFile.Source!, formName, multipartFile.FileName,
                    multipartFile.ContentType, multipartFile.ContentEncoding);
            // Base64 字符串文件
            case FileSourceType.Base64String:
                return AddFileFromBase64String((string)multipartFile.Source!, formName,
                    multipartFile.FileName, multipartFile.ContentType, multipartFile.ContentEncoding);
            // 互联网文件地址
            case FileSourceType.Remote:
                return AddFileFromRemote((string)multipartFile.Source!, formName, multipartFile.FileName,
                    multipartFile.ContentType, multipartFile.ContentEncoding);
            // 不做处理
            case FileSourceType.None:
            default:
                return this;
        }
    }

    /// <summary>
    ///     添加流
    /// </summary>
    /// <param name="stream">
    ///     <see cref="Stream" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="disposeStreamOnRequestCompletion">是否在请求结束后自动释放流。默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddStream(Stream stream, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null, bool disposeStreamOnRequestCompletion = false)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 解析内容类型字符串
        var mediaType = ParseContentType(contentType, contentEncoding, out var encoding);

        // 获取文件 MIME 类型
        var mimeType = !string.IsNullOrWhiteSpace(mediaType) ? mediaType :
            string.IsNullOrWhiteSpace(fileName) ? MediaTypeNames.Application.Octet :
            FileTypeMapper.GetContentType(fileName);

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = mimeType, RawContent = stream, ContentEncoding = encoding, FileName = fileName
        });

        // 是否在请求结束后自动释放流
        if (disposeStreamOnRequestCompletion)
        {
            _httpRequestBuilder.AddDisposable(stream);
        }

        return this;
    }

    /// <summary>
    ///     添加字节数组
    /// </summary>
    /// <param name="byteArray">字节数组</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddByteArray(byte[] byteArray, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(byteArray);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 解析内容类型字符串
        var mediaType = ParseContentType(contentType, contentEncoding, out var encoding);

        // 获取文件 MIME 类型
        var mimeType = !string.IsNullOrWhiteSpace(mediaType) ? mediaType :
            string.IsNullOrWhiteSpace(fileName) ? MediaTypeNames.Application.Octet :
            FileTypeMapper.GetContentType(fileName);

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = mimeType, RawContent = byteArray, ContentEncoding = encoding, FileName = fileName
        });

        return this;
    }

    /// <summary>
    ///     添加 URL 编码表单
    /// </summary>
    /// <param name="rawObject">原始对象</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="useStringContent">
    ///     是否使用 <see cref="StringContent" /> 构建
    ///     <see cref="FormUrlEncodedContent" />。默认 <c>false</c>。
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFormUrlEncoded(object? rawObject, string name,
        Encoding? contentEncoding = null, bool useStringContent = false)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = MediaTypeNames.Application.FormUrlEncoded,
            RawContent = rawObject,
            ContentEncoding = contentEncoding
        });

        // 检查是否启用 StringContent 方式构建 application/x-www-form-urlencoded 请求内容
        if (useStringContent)
        {
            _httpRequestBuilder.AddStringContentForFormUrlEncodedContentProcessor();
        }

        return this;
    }

    /// <summary>
    ///     添加多部分表单内容
    /// </summary>
    /// <param name="rawObject">原始对象</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddMultipartFormData(object? rawObject, string name,
        Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = MediaTypeNames.Multipart.FormData,
            RawContent = rawObject,
            ContentEncoding = contentEncoding
        });

        return this;
    }

    /// <summary>
    ///     添加 <see cref="HttpContent" />
    /// </summary>
    /// <param name="httpContent">
    ///     <see cref="HttpContent" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder Add(HttpContent httpContent, string? name, string? contentType = null,
        Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpContent);

        // 尝试从 ContentDisposition 中解析 Name
        var formName = string.IsNullOrWhiteSpace(name) ? httpContent.Headers.ContentDisposition?.Name : name;

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(formName, nameof(name));

        string? mediaType;
        Encoding? encoding = null;
        MediaTypeHeaderValue? mediaTypeHeaderValue = null;

        // 空检查
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            mediaType = ParseContentType(contentType, contentEncoding, out encoding);
        }
        else
        {
            mediaTypeHeaderValue = httpContent.Headers.ContentType;
            mediaType = mediaTypeHeaderValue?.MediaType;
        }

        // 尝试从 FileName 中解析 MediaType
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            mediaType = FileTypeMapper.GetContentType(
                httpContent.Headers.ContentDisposition?.FileName?.TrimStart('"').TrimEnd('"')!,
                null!);
        }

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaType, nameof(contentType));

        // 设置或解析内容编码
        encoding = contentEncoding ?? encoding ?? (string.IsNullOrWhiteSpace(mediaTypeHeaderValue?.CharSet)
            ? null
            : Encoding.GetEncoding(mediaTypeHeaderValue.CharSet));

        _partContents.Add(new MultipartFormDataItem(formName)
        {
            ContentType = mediaType, RawContent = httpContent, ContentEncoding = encoding
        });

        return this;
    }

    /// <summary>
    ///     设置是否移除默认的多部分内容的 <c>Content-Type</c>
    /// </summary>
    /// <param name="omit">如果为 <c>true</c> 则移除，默认为 <c>false</c></param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder SetOmitContentType(bool omit)
    {
        OmitContentType = omit;

        return this;
    }

    /// <summary>
    ///     构建 <see cref="MultipartFormDataContent" /> 实例
    /// </summary>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="httpContentProcessorFactory">
    ///     <see cref="IHttpContentProcessorFactory" />
    /// </param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 集合</param>
    /// <returns>
    ///     <see cref="MultipartFormDataContent" />
    /// </returns>
    internal MultipartFormDataContent? Build(HttpRemoteOptions httpRemoteOptions,
        IHttpContentProcessorFactory httpContentProcessorFactory,
        params IHttpContentProcessor[]? processors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);
        ArgumentNullException.ThrowIfNull(httpContentProcessorFactory);

        // 空检查
        if (_partContents.IsNullOrEmpty())
        {
            return null;
        }

        // 获取多部分表单内容的边界；注意：这里可能出现前后双引号问题
        var boundary = Boundary?.TrimStart('"').TrimEnd('"');

        // 初始化 multipartFormDataContent 实例
        var multipartFormDataContent = string.IsNullOrWhiteSpace(boundary)
            ? new MultipartFormDataContent()
            : new MultipartFormDataContent(boundary);

        // 处理 OSS 对象存储服务必须设置 Content-Type 问题
        if (!string.IsNullOrWhiteSpace(boundary))
        {
            multipartFormDataContent.Headers.ContentType =
                MediaTypeHeaderValue.Parse($"{MediaTypeNames.Multipart.FormData}; boundary={boundary}");
        }

        // 为确保请求分析工具能完整显示所有表单数据（避免因打印裁剪导致信息丢失），先对数据排序，再逐条遍历添加
        // 这里留有一个疑问：若第三方接口依赖请求数据的顺序，当前做法是否可能导致请求失败？
        foreach (var dataItem in _partContents.OrderBy(u => u.RawContent is byte[] or Stream ? 1 : 0))
        {
            // 尝试获取转换后的表单名称
            var transformedName = FormNameTransformer?.Invoke(dataItem.Name) ?? dataItem.Name;

            // 构建 HttpContent 实例
            var httpContent = BuildHttpContent(dataItem, transformedName, httpContentProcessorFactory, processors);

            // 空检查
            if (httpContent is null)
            {
                continue;
            }

            // 检查是否移除默认的多部分内容的 Content-Type，解决对接 Java 程序时可能出现失败问题
            if (OmitContentType)
            {
                httpContent.Headers.ContentType = null;
            }

            // 调用用于处理在添加 HttpContent 表单项内容时的操作
            OnPreAddContent?.Invoke(httpContent, transformedName);

            // 添加 HttpContent 表单项内容
            multipartFormDataContent.Add(httpContent, transformedName);
        }

        return multipartFormDataContent;
    }

    /// <summary>
    ///     构建 <see cref="HttpContent" /> 实例
    /// </summary>
    /// <param name="multipartFormDataItem">
    ///     <see cref="MultipartFormDataItem" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="httpContentProcessorFactory">
    ///     <see cref="IHttpContentProcessorFactory" />
    /// </param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 集合</param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    internal static HttpContent? BuildHttpContent(MultipartFormDataItem multipartFormDataItem, string name,
        IHttpContentProcessorFactory httpContentProcessorFactory, params IHttpContentProcessor[]? processors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(multipartFormDataItem);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(httpContentProcessorFactory);

        // 空检查
        var contentType = multipartFormDataItem.ContentType;
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        // 构建 HttpContent 实例
        var httpContent = httpContentProcessorFactory.Build(multipartFormDataItem.RawContent, contentType,
            multipartFormDataItem.ContentEncoding, processors);

        // 空检查
        if (httpContent is not null && httpContent.Headers.ContentDisposition is null)
        {
            // 设置表单项内容 Content-Disposition 标头
            httpContent.Headers.ContentDisposition =
                new ContentDispositionHeaderValue(Constants.FORM_DATA_DISPOSITION_TYPE)
                {
                    Name = name.AddQuotes(),
                    FileName =
                        (string.IsNullOrWhiteSpace(multipartFormDataItem.FileName) &&
                         contentType.IsIn([MediaTypeNames.Application.Octet], StringComparer.OrdinalIgnoreCase)
                            // 解决发送文件或二进制【未设置】文件名问题
                            ? $"Unnamed_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}"
                            : multipartFormDataItem.FileName).AddQuotes()
                };
        }

        return httpContent;
    }

    /// <summary>
    ///     解析内容类型字符串
    /// </summary>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="encoding">内容编码</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? ParseContentType(string? contentType, Encoding? contentEncoding, out Encoding? encoding)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(contentType))
        {
            encoding = null;
            return null;
        }

        // 解析内容类型字符串
        var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(contentType);

        // 解析/设置内容编码
        encoding = contentEncoding ?? (!string.IsNullOrWhiteSpace(mediaTypeHeaderValue.CharSet)
            ? Encoding.GetEncoding(mediaTypeHeaderValue.CharSet)
            : contentEncoding);

        return mediaTypeHeaderValue.MediaType;
    }
}