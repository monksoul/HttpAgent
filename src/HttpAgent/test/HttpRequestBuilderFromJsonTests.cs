// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderFromJsonTests
{
    [Fact]
    public void FromJson_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.FromJson(null!));
        Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson(" "));

        var exception = Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson("[]"));
        Assert.Equal("The provided JSON must be a valid JSON object. (Parameter 'json')", exception.Message);
    }

    [Fact]
    public void FromJson_CustomMethod_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.FromJson("{\"url\":\"http://example.com\",\"method\":\"CUSTOM\"}");
        Assert.NotNull(httpRequestBuilder.HttpMethod);
        Assert.Equal("CUSTOM", httpRequestBuilder.HttpMethod.ToString());
    }

    [Fact]
    public void FromJson_MinimalGet_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson("{\"url\":\"http://example.com\",\"method\":\"GET\"}");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Equal("http://example.com", builder.RequestUri?.OriginalString);
    }

    [Fact]
    public void FromJson_RelativeUrl_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson("{\"url\":\"/api/data\",\"method\":\"GET\"}");
        Assert.Equal("/api/data", builder.RequestUri?.OriginalString);
        Assert.False(builder.RequestUri?.IsAbsoluteUri);
    }

    [Fact]
    public void FromJson_ImplicitGet_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson("{\"url\":\"http://example.com\"}");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
    }

    [Fact]
    public void FromJson_ImplicitPostWhenData_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson("{\"url\":\"http://api\",\"data\":{\"key\":\"val\"}}");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
        Assert.NotNull(builder.RawContent);
    }

    [Fact]
    public void FromJson_ImplicitPostWhenMultipart_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(
            "{\"url\":\"http://api\",\"method\":\"POST\",\"multipart\":{\"name\":\"John\"}}");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
        Assert.NotNull(builder.MultipartFormDataBuilder);
    }

    [Fact]
    public void FromJson_ExplicitPostWithJson_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://api"",
            ""method"": ""POST"",
            ""data"": {""name"":""Furion""},
            ""contentType"": ""application/json""
        }");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
        Assert.Equal("application/json", builder.ContentType);
        Assert.NotNull(builder.RawContent);
    }

    [Fact]
    public void FromJson_FormUrlEncoded_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://api"",
            ""method"": ""POST"",
            ""data"": {""id"":1,""name"":""furion""},
            ""contentType"": ""application/x-www-form-urlencoded""
        }");
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        Assert.NotNull(builder.RawContent);
    }

    [Fact]
    public void FromJson_RawStringData_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://api"",
            ""method"": ""POST"",
            ""data"": ""This is a raw string"",
            ""contentType"": ""text/plain""
        }");
        Assert.NotNull(builder.RawContent);
        Assert.Equal("This is a raw string", builder.RawContent.ToString());
    }

    [Fact]
    public void FromJson_WithHeaders_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""headers"": {""Accept"": ""application/json"", ""X-Api-Key"": ""secret""}
        }");
        Assert.NotNull(builder.Headers);
        Assert.Equal("application/json", builder.Headers!["Accept"][0]);
        Assert.Equal("secret", builder.Headers["X-Api-Key"][0]);
    }

    [Fact]
    public void FromJson_WithCookies_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""cookies"": {""session"": ""abc123"", ""user"": ""admin""}
        }");
        Assert.NotNull(builder.Cookies);
        Assert.Equal("abc123", builder.Cookies!["session"]);
        Assert.Equal("admin", builder.Cookies["user"]);
    }

    [Fact]
    public void FromJson_WithQueryParameters_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://api"",
            ""method"": ""GET"",
            ""queries"": {""page"": 1, ""size"": 10}
        }");
        Assert.NotNull(builder.QueryParameters);
        Assert.True(builder.QueryParameters!.ContainsKey("page"));
        Assert.True(builder.QueryParameters.ContainsKey("size"));
    }

    [Fact]
    public void FromJson_WithBaseAddress_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""/api/data"",
            ""method"": ""GET"",
            ""baseURL"": ""https://base.example.com""
        }");
        Assert.Equal("https://base.example.com", builder.BaseAddress?.OriginalString);
    }

    [Fact]
    public void FromJson_WithTimeout_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""timeout"": 5000
        }");
        Assert.NotNull(builder.TimeoutOptions);
        Assert.Equal(TimeSpan.FromMilliseconds(5000), builder.TimeoutOptions!.Timeout);
    }

    [Fact]
    public void FromJson_WithHttpVersion_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""httpVersion"": ""2.0""
        }");
        Assert.Equal(new Version(2, 0), builder.Version);
    }

    [Fact]
    public void FromJson_WithClientName_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""client"": ""myclient""
        }");
        Assert.Equal("myclient", builder.HttpClientName);
    }

    [Fact]
    public void FromJson_WithProfiler_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""profiler"": true
        }");
        Assert.True(builder.ProfilerEnabled);
    }

    [Fact]
    public void FromJson_BearerAuth_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""auth"": {""type"": ""bearer"", ""token"": ""abc""}
        }");
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Bearer", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("abc", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void FromJson_BasicAuth_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""auth"": {""type"": ""basic"", ""username"": ""user"", ""password"": ""pass""}
        }");
        Assert.NotNull(builder.AuthenticationHeader);
        var decoded = Encoding.UTF8.GetString(
            Convert.FromBase64String(builder.AuthenticationHeader!.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void FromJson_DigestAuth_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
            ""auth"": {""type"": ""digest"", ""username"": ""user"", ""password"": ""secret""}
        }");
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("user|:|secret", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void FromJson_MultipartFields_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://api"",
            ""method"": ""POST"",
            ""multipart"": {""field1"": ""value1"", ""field2"": 123, ""flag"": true}
        }");
        var items = GetMultipartItems(builder);
        Assert.Equal(3, items.Count);
    }

    [Fact]
    public void FromJson_MultipartSingleFile_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "content");
            var escapedPath = filePath.Replace("\\", "\\\\");
            builder = HttpRequestBuilder.FromJson(
                $"{{\"url\":\"http://api\",\"method\":\"POST\",\"multipart\":{{\"file\":\"@{escapedPath}\"}}}}");
            var items = GetMultipartItems(builder);
            Assert.Single(items);
            Assert.Equal("file", items[0].Name);
            Assert.NotNull(items[0].FileName);
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
    public void FromJson_MultipartMultipleFiles_ReturnOK()
    {
        var filePath1 = Path.GetTempFileName();
        var filePath2 = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath1, "content1");
            File.WriteAllText(filePath2, "content2");
            var escapedPath1 = filePath1.Replace("\\", "\\\\");
            var escapedPath2 = filePath2.Replace("\\", "\\\\");
            builder = HttpRequestBuilder.FromJson(
                $"{{\"url\":\"http://api\",\"method\":\"POST\",\"multipart\":{{\"files\":\"@{escapedPath1}\",\"files2\":\"@{escapedPath2}\"}}}}");
            var items = GetMultipartItems(builder);
            Assert.Equal(2, items.Count);
            Assert.True(items.All(i => i.FileName != null));
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
    public void FromJson_CombinedOptions_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""https://api.example.com"",
            ""method"": ""PUT"",
            ""baseURL"": ""https://base.com"",
            ""headers"": { ""Accept"": ""application/json"" },
            ""queries"": { ""page"": 1 },
            ""cookies"": { ""session"": ""abc"" },
            ""timeout"": 30000,
            ""client"": ""myclient"",
            ""httpVersion"": ""2.0"",
            ""auth"": { ""type"": ""bearer"", ""token"": ""xxx"" },
            ""data"": { ""name"": ""Furion"" },
            ""contentType"": ""application/json"",
            ""encoding"": ""utf-8"",
            ""profiler"": true
        }");
        Assert.Equal(HttpMethod.Put, builder.HttpMethod);
        Assert.Equal("https://base.com", builder.BaseAddress?.OriginalString);
        Assert.NotNull(builder.Headers);
        Assert.Equal("application/json", builder.Headers!["Accept"][0]);
        Assert.NotNull(builder.QueryParameters);
        Assert.NotNull(builder.Cookies);
        Assert.Equal(TimeSpan.FromMilliseconds(30000), builder.TimeoutOptions!.Timeout);
        Assert.Equal("myclient", builder.HttpClientName);
        Assert.Equal(new Version(2, 0), builder.Version);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.NotNull(builder.RawContent);
        Assert.Equal("application/json", builder.ContentType);
        Assert.True(builder.ProfilerEnabled);
    }

    [Fact]
    public void FromJson_TrailingCommas_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""url"": ""http://example.com"",
            ""method"": ""GET"",
        }");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Equal("http://example.com", builder.RequestUri?.OriginalString);
    }

    [Fact]
    public void FromJson_CaseInsensitive_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{
            ""URL"": ""http://example.com"",
            ""Method"": ""GET""
        }");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Equal("http://example.com", builder.RequestUri?.OriginalString);
    }

    [Fact]
    public void FromJson_DeleteRequest_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{""url"":""http://example.com"",""method"":""DELETE""}");
        Assert.Equal(HttpMethod.Delete, builder.HttpMethod);
    }

    [Fact]
    public void FromJson_PatchRequest_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson(@"{""url"":""http://example.com"",""method"":""PATCH""}");
        Assert.Equal(HttpMethod.Patch, builder.HttpMethod);
    }

    [Fact]
    public void FromJson_CustomExtractor_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson("{\"url\":\"http://example.com\",\"method\":\"GET\"}", options =>
            options.AddExtractor(new JsonMethodExtractor()));
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
    }

    [Fact]
    public void FromJson_RemoveExtractor_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromJson("{\"url\":\"http://example.com\",\"method\":\"GET\"}", options =>
        {
            options.RemoveExtractor<JsonMethodExtractor>();
        });

        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Equal("http://example.com", builder.RequestUri?.OriginalString);
    }

    private static List<MultipartFormDataItem> GetMultipartItems(HttpRequestBuilder builder)
    {
        var field = typeof(HttpMultipartFormDataBuilder).GetField("_partContents",
            BindingFlags.NonPublic | BindingFlags.Instance);
        return (List<MultipartFormDataItem>)field!.GetValue(builder.MultipartFormDataBuilder)!;
    }
}