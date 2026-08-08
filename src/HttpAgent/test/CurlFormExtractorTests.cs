// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlFormExtractorTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var ctor = typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, Type.EmptyTypes, null);
        Assert.NotNull(ctor);
        return (HttpRequestBuilder)ctor.Invoke(null);
    }

    private static HttpCurlTokenExtractorContext CreateContext(params string[] tokens) => new(tokens);

    [Fact]
    public void Flags_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var flagsProperty = typeof(CurlFormExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-F", "--form"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.MultipartFormDataBuilder);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-F");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.MultipartFormDataBuilder);
    }

    [Fact]
    public void TryExtract_TextFormItem_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-F", "name=John");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("name", items[0].Name);
        Assert.Equal(MediaTypeNames.Text.Plain, items[0].ContentType);
        Assert.Equal("John", items[0].RawContent);
    }

    [Fact]
    public void TryExtract_TextFormItemWithType_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--form", "desc=hello;type=text/html");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("desc", items[0].Name);
        Assert.Equal(MediaTypeNames.Text.Html, items[0].ContentType);
        Assert.Equal("hello", items[0].RawContent);
    }

    [Fact]
    public void TryExtract_FileUpload_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        var builder = CreateBuilder();
        try
        {
            File.WriteAllText(filePath, "test content");
            var extractor = new CurlFormExtractor();
            var context = CreateContext("-F", $"file=@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Single(items);
            Assert.Equal("file", items[0].Name);
            Assert.NotNull(items[0].FileName);
            Assert.Equal(Path.GetFileName(filePath), items[0].FileName);
            Assert.NotNull(items[0].RawContent);
        }
        finally
        {
            builder.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void TryExtract_FileUploadWithType_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        var builder = CreateBuilder();
        try
        {
            File.WriteAllText(filePath, "content");
            var extractor = new CurlFormExtractor();
            var context = CreateContext("-F", $"file=@{filePath};type=image/png");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Single(items);
            Assert.Equal("image/png", items[0].ContentType);
        }
        finally
        {
            builder.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void TryExtract_FileUploadWithFilename_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        var builder = CreateBuilder();
        try
        {
            File.WriteAllText(filePath, "content");
            const string customName = "custom.txt";
            var extractor = new CurlFormExtractor();
            var context = CreateContext("-F", $"file=@{filePath};filename={customName}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Single(items);
            Assert.Equal(customName, items[0].FileName);
        }
        finally
        {
            builder.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void TryExtract_InvalidFormat_Invalid_Parameters()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-F", "invalid");

        var exception = Assert.Throws<ArgumentException>(() => extractor.TryExtract(builder, context));
        Assert.Contains("Invalid form format", exception.Message);
    }

    [Fact]
    public void TryExtract_NoValueAfterEquals_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-F", "name=");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.NotNull(items);
        Assert.Single(items);
        Assert.Equal("name", items[0].Name);
    }

    [Fact]
    public void TryExtract_AppendingToExistingMultipart_ReturnOK()
    {
        var extractor = new CurlFormExtractor();
        var builder = CreateBuilder();
        var context1 = CreateContext("-F", "a=1");
        extractor.TryExtract(builder, context1);

        var context2 = CreateContext("-F", "b=2");
        var result = extractor.TryExtract(builder, context2);

        Assert.True(result);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Equal(2, items.Count);
        Assert.Equal("a", items[0].Name);
        Assert.Equal("b", items[1].Name);
    }

    [Fact]
    public void ProcessFormItem_TextItem_ReturnOK()
    {
        var builder = CreateBuilder();
        var multipart = new HttpMultipartFormDataBuilder(builder);
        CurlFormExtractor.ProcessFormItem(multipart, "field", "value");

        var items = multipart._partContents;
        Assert.Single(items);
        Assert.Equal("field", items[0].Name);
        Assert.Equal("value", items[0].RawContent);
    }

    [Fact]
    public void ProcessFormItem_FileItem_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        var builder = CreateBuilder();
        try
        {
            File.WriteAllText(filePath, "data");
            var multipart = new HttpMultipartFormDataBuilder(builder);
            CurlFormExtractor.ProcessFormItem(multipart, "file", $"@{filePath}");

            var items = multipart._partContents;
            Assert.Single(items);
            Assert.Equal("file", items[0].Name);
            Assert.NotNull(items[0].RawContent);
            Assert.Equal(Path.GetFileName(filePath), items[0].FileName);
        }
        finally
        {
            builder.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void ProcessFileItem_WithTypeAndFilename_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        var builder = CreateBuilder();
        try
        {
            File.WriteAllText(filePath, "content");
            var multipart = new HttpMultipartFormDataBuilder(builder);
            CurlFormExtractor.ProcessFileItem(multipart, "upload", $"{filePath};type=image/jpeg;filename=photo.jpg");

            var items = multipart._partContents;
            Assert.Single(items);
            Assert.Equal("upload", items[0].Name);
            Assert.Equal("image/jpeg", items[0].ContentType);
            Assert.Equal("photo.jpg", items[0].FileName);
        }
        finally
        {
            builder.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void ProcessTextItem_WithContentType_ReturnOK()
    {
        var builder = CreateBuilder();
        var multipart = new HttpMultipartFormDataBuilder(builder);
        CurlFormExtractor.ProcessTextItem(multipart, "comment", "hello;type=text/plain");

        var items = multipart._partContents;
        Assert.Single(items);
        Assert.Equal("comment", items[0].Name);
        Assert.Equal("hello", items[0].RawContent);
        Assert.Equal("text/plain", items[0].ContentType);
    }
}