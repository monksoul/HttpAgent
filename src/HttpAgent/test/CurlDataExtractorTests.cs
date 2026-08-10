// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlDataExtractorTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var ctor = typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, Type.EmptyTypes, null);
        Assert.NotNull(ctor);
        return (HttpRequestBuilder)ctor.Invoke(null);
    }

    private static HttpCurlParsingContext CreateContext(params string[] tokens) => new(tokens);

    private static string CreateTempFile(string content = "hello world", Encoding? encoding = null)
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, content, encoding ?? new UTF8Encoding(false));
        return path;
    }

    [Fact]
    public void Flags_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var flagsProperty = typeof(CurlDataExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-d", "--data", "--data-raw", "--data-binary", "--data-urlencode", "--data-ascii"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.RawContent);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-d");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.RawContent);
        Assert.Null(builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataFlagDefaultContentType_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-d", "key=value");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("key=value", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataAliasFlagDefaultContentType_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data", "key=value");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("key=value", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataBinaryFlagDefaultContentType_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-binary", "binary content");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("binary content", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.Octet, builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataUrlencodeFlagDefaultContentType_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-urlencode", "param=value");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("param=value", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataAsciiFlagNonFileRead_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-ascii", "some data");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("some data", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataFlagWithExistingContentType_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        builder.SetContentType(MediaTypeNames.Application.Json);
        var context = CreateContext("-d", "{\"key\":\"value\"}");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("{\"key\":\"value\"}", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.Json, builder.ContentType);
    }

    [Fact]
    public void TryExtract_AppendContent_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        builder.SetContent("first=1", MediaTypeNames.Application.FormUrlEncoded);
        var context = CreateContext("-d", "second=2");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("first=1&second=2", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_AppendContentWithExistingDictionaryOverwritten_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var dict = new Dictionary<string, object?> { ["a"] = "1" };
        builder.SetContent(dict, MediaTypeNames.Application.FormUrlEncoded);
        var context = CreateContext("-d", "b=2");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("b=2", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_DataUrlencodeEnablesProcessor_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-urlencode", "key=val");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.NotNull(builder.HttpContentProcessorProviders);
        Assert.NotEmpty(builder.HttpContentProcessorProviders);

        var processors = builder.HttpContentProcessorProviders.SelectMany(p => p()).ToList();
        Assert.Contains(processors, p => p is StringContentForFormUrlEncodedContentProcessor { UrlEncode: true });
    }

    [Fact]
    public void TryExtract_DataFlagWithFormUrlEncodedEnablesProcessor_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-d", "key=val");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.NotNull(builder.HttpContentProcessorProviders);
        Assert.NotEmpty(builder.HttpContentProcessorProviders);

        var processors = builder.HttpContentProcessorProviders.SelectMany(p => p()).ToList();
        Assert.Contains(processors, p => p is StringContentForFormUrlEncodedContentProcessor { UrlEncode: false });
    }

    [Fact]
    public void TryExtract_DataBinaryFlagDoesNotEnableFormProcessor_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-binary", "stuff");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Null(builder.HttpContentProcessorProviders);
    }

    [Fact]
    public void TryExtract_DataUrlencodeProcessorCanBeAddedOnlyOnce_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context1 = CreateContext("--data-urlencode", "a=1");
        var context2 = CreateContext("--data-urlencode", "b=2");

        extractor.TryExtract(builder, context1);
        var providerCountAfterFirst = builder.HttpContentProcessorProviders?.Count;
        extractor.TryExtract(builder, context2);
        var providerCountAfterSecond = builder.HttpContentProcessorProviders?.Count;

        Assert.NotNull(builder.HttpContentProcessorProviders);
        Assert.Equal(providerCountAfterFirst, providerCountAfterSecond);
        Assert.Equal(1, providerCountAfterSecond);

        var processors = builder.HttpContentProcessorProviders.SelectMany(p => p()).ToList();
        Assert.Contains(processors, p => p is StringContentForFormUrlEncodedContentProcessor { UrlEncode: true });
        Assert.Single(processors);
    }

    [Fact]
    public void TryExtract_DataFlagWithFileRead_TextContent_ReturnOK()
    {
        var filePath = CreateTempFile("line1\nline2");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            var context = CreateContext("-d", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal(2, context.CurrentIndex);
            Assert.Equal("line1\nline2", builder.RawContent);
            Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
            Assert.Null(builder.HttpContentProcessorProviders);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_DataAliasWithFileRead_TextContent_ReturnOK()
    {
        var filePath = CreateTempFile("data content");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            var context = CreateContext("--data", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal(2, context.CurrentIndex);
            Assert.Equal("data content", builder.RawContent);
            Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
            Assert.Null(builder.HttpContentProcessorProviders);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_DataAsciiFlagWithFileRead_TextContent_ReturnOK()
    {
        var filePath = CreateTempFile("ascii data");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            var context = CreateContext("--data-ascii", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal("ascii data", builder.RawContent);
            Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
            Assert.Null(builder.HttpContentProcessorProviders);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_DataBinaryFlagWithFileRead_BinaryContent_ReturnOK()
    {
        var filePath = CreateTempFile("binary bytes");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            var context = CreateContext("--data-binary", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal(2, context.CurrentIndex);
            var rawContent = Assert.IsType<byte[]>(builder.RawContent);
            Assert.Equal("binary bytes"u8.ToArray(), rawContent);
            Assert.Equal(MediaTypeNames.Application.Octet, builder.ContentType);
            Assert.Null(builder.HttpContentProcessorProviders);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_DataUrlencodeFlagWithFileRead_EncodesContent_ReturnOK()
    {
        var filePath = CreateTempFile("hello world=foo");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            var context = CreateContext("--data-urlencode", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal(2, context.CurrentIndex);
            Assert.Equal("hello+world%3Dfoo", builder.RawContent);
            Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
            Assert.Null(builder.HttpContentProcessorProviders);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_DataRawFlagWithAtSign_NotFileRead_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-raw", "@some/path");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("@some/path", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_FileReadWhenContentTypeAlreadySet_NoOverride_ReturnOK()
    {
        var filePath = CreateTempFile("data");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            builder.SetContentType("application/json");
            var context = CreateContext("-d", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal("data", builder.RawContent);
            Assert.Equal("application/json", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_FileReadAppendContent_ReturnOK()
    {
        var filePath = CreateTempFile("second");
        try
        {
            var extractor = new CurlDataExtractor();
            var builder = CreateBuilder();
            builder.SetContent("first", MediaTypeNames.Application.FormUrlEncoded);
            var context = CreateContext("-d", $"@{filePath}");

            var result = extractor.TryExtract(builder, context);

            Assert.True(result);
            Assert.Equal("first&second", builder.RawContent);
            Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TryExtract_FileReadWithAtOnly_NotFileRead_ReturnOK()
    {
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-d", "@");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("@", builder.RawContent);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, builder.ContentType);
    }

    [Fact]
    public void TryExtract_FileReadNonExistentFile_Invalid_Parameters()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var extractor = new CurlDataExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--data-binary", $"@{nonExistentPath}");

        Assert.Throws<FileNotFoundException>(() => extractor.TryExtract(builder, context));
    }
}