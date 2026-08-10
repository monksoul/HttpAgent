// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonMultipartExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonMultipartExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("multipart", prop!.GetValue(new JsonMultipartExtractor()));
    }

    [Fact]
    public void Extract_SimpleField_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"name\":\"John\"}}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("name", items[0].Name);
        Assert.Equal("John", items[0].RawContent);
    }

    [Fact]
    public void Extract_MultipleFields_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"name\":\"John\",\"age\":\"30\"}}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Equal(2, items.Count);
        Assert.Equal("name", items[0].Name);
        Assert.Equal("John", items[0].RawContent);
        Assert.Equal("age", items[1].Name);
        Assert.Equal("30", items[1].RawContent);
    }

    [Fact]
    public void Extract_NullValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"nothing\":null}}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Empty(items);
    }

    [Fact]
    public void Extract_BooleanValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"flag\":true}}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("true", items[0].RawContent);
    }

    [Fact]
    public void Extract_NumberValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"count\":42}}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("42", items[0].RawContent);
    }

    [Fact]
    public void Extract_ObjectValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"data\":{\"key\":\"val\"}}}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("{\"key\":\"val\"}", items[0].RawContent);
    }

    [Fact]
    public void Extract_FileUploadBasic_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "content");
            builder = CreateBuilder();
            var context = CreateContext($"{{\"multipart\":{{\"file\":\"@{filePath.Replace("\\", "\\\\")}\"}}}}");
            new JsonMultipartExtractor().Extract(builder, context);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Single(items);
            Assert.Equal("file", items[0].Name);
            Assert.NotNull(items[0].FileName);
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
    public void Extract_FileUploadWithType_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "content");
            builder = CreateBuilder();
            var escapedPath = filePath.Replace("\\", "\\\\");
            var context = CreateContext($"{{\"multipart\":{{\"file\":\"@{escapedPath};type=image/png\"}}}}");
            new JsonMultipartExtractor().Extract(builder, context);
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
    public void Extract_FileUploadWithFilename_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "content");
            builder = CreateBuilder();
            var escapedPath = filePath.Replace("\\", "\\\\");
            var context = CreateContext($"{{\"multipart\":{{\"file\":\"@{escapedPath};filename=renamed.txt\"}}}}");
            new JsonMultipartExtractor().Extract(builder, context);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Single(items);
            Assert.Equal("renamed.txt", items[0].FileName);
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
    public void Extract_FileUploadWithTypeAndFilename_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "content");
            builder = CreateBuilder();
            var escapedPath = filePath.Replace("\\", "\\\\");
            var context = CreateContext(
                $"{{\"multipart\":{{\"upload\":\"@{escapedPath};type=image/jpeg;filename=photo.jpg\"}}}}");
            new JsonMultipartExtractor().Extract(builder, context);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
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
    public void Extract_FileUploadArray_ReturnOK()
    {
        var filePath1 = Path.GetTempFileName();
        var filePath2 = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath1, "content1");
            File.WriteAllText(filePath2, "content2");
            builder = CreateBuilder();
            var escapedPath1 = filePath1.Replace("\\", "\\\\");
            var escapedPath2 = filePath2.Replace("\\", "\\\\");
            var context = CreateContext(
                $"{{\"multipart\":{{\"files\":[\"@{escapedPath1}\",\"@{escapedPath2}\"]}}}}");
            new JsonMultipartExtractor().Extract(builder, context);
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Equal(2, items.Count);
            Assert.All(items, i => Assert.Equal("files", i.Name));
            Assert.All(items, i => Assert.NotNull(i.FileName));
        }
        finally
        {
            builder.ReleaseResources();
            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
            }

            if (File.Exists(filePath2))
            {
                File.Delete(filePath2);
            }
        }
    }

    [Fact]
    public void Extract_NonJsonObject_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":\"string\"}");
        new JsonMultipartExtractor().Extract(builder, context);
        Assert.Null(builder.MultipartFormDataBuilder);
    }

    [Fact]
    public void ProcessMultipartItem_NullNode_ReturnOK()
    {
        var builder = CreateBuilder();
        var multipart = new HttpMultipartFormDataBuilder(builder);
        JsonMultipartExtractor.ProcessMultipartItem(multipart, "field", null);
        var items = multipart._partContents;
        Assert.Empty(items);
    }

    [Fact]
    public void ProcessMultipartItem_NonStringValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var multipart = new HttpMultipartFormDataBuilder(builder);
        var node = JsonNode.Parse("123")!;
        JsonMultipartExtractor.ProcessMultipartItem(multipart, "num", node);
        var items = multipart._partContents;
        Assert.Single(items);
        Assert.Equal("123", items[0].RawContent);
    }

    [Fact]
    public void Extract_FileUploadFromRemote_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"multipart\":{\"file\":\"@https://furion.net/img/logo.png\"}}");
        new JsonMultipartExtractor().Extract(builder, context);

        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Single(items);
        Assert.Equal("file", items[0].Name);
        Assert.NotNull(items[0].FileName);
        Assert.Equal("logo.png", items[0].FileName);
        Assert.True(items[0].RawContent is Stream);
    }
}