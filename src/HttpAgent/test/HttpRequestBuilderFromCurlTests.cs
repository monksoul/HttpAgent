// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;

namespace HttpAgent.Tests;

public class HttpRequestBuilderFromCurlTests
{
    private static string CreateTempFile(string content)
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, content, new UTF8Encoding(false));
        return path;
    }

    [Fact]
    public void FromCurl_NullCommand_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.FromCurl(null!));

    [Fact]
    public void FromCurl_EmptyCommand_Invalid_Parameters() =>
        Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromCurl(string.Empty));

    [Fact]
    public void FromCurl_WhiteSpaceCommand_Invalid_Parameters() =>
        Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromCurl("   \t  "));

    [Fact]
    public void FromCurl_NoTokens_Invalid_Parameters() =>
        Assert.Throws<InvalidOperationException>(() => HttpRequestBuilder.FromCurl("curl"));

    [Fact]
    public void FromCurl_WithoutCurlPrefix_Invalid_Parameters()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            HttpRequestBuilder.FromCurl("-X GET http://example.com"));
        Assert.Equal("The cURL command must start with 'curl'.", exception.Message);
    }

    [Fact]
    public void FromCurl_SimpleGet_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl http://example.com");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Equal("http://example.com", builder.RequestUri?.OriginalString);
        Assert.Equal("curl http://example.com", builder.Properties[Constants.CURL_COMMAND_KEY]);
    }

    [Fact]
    public void FromCurl_GetWithQueryString_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl 'https://api.example.com/data?page=1&size=10'");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Equal("https://api.example.com/data?page=1&size=10", builder.RequestUri?.OriginalString);
    }

    [Fact]
    public void FromCurl_ImplicitPostWithData_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -d 'name=value' http://example.com");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
        Assert.Equal("name=value", builder.RawContent);
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
    }

    [Fact]
    public void FromCurl_ExplicitPutWithJson_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(@"
            curl -X PUT 'https://api.example.com/users/1' \
            -H 'Content-Type: application/json' \
            -d '{\""name\"":\""John\"",\""age\"":30}'
        ");
        Assert.Equal(HttpMethod.Put, builder.HttpMethod);
        Assert.Equal("application/json", builder.ContentType);
        Assert.Contains("John", builder.RawContent?.ToString());
    }

    [Fact]
    public void FromCurl_DeleteRequest_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -X DELETE http://example.com/resource/1");
        Assert.Equal(HttpMethod.Delete, builder.HttpMethod);
    }

    [Fact]
    public void FromCurl_PatchWithJson_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(@"
            curl -X PATCH http://example.com/resource/1 \
            -H 'Content-Type: application/json' \
            -d '{\""key\"":\""value\""}'
        ");
        Assert.Equal(HttpMethod.Patch, builder.HttpMethod);
        Assert.Equal("application/json", builder.ContentType);
    }

    [Fact]
    public void FromCurl_HeadRequest_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -I http://example.com");
        Assert.Equal(HttpMethod.Head, builder.HttpMethod);
    }

    [Fact]
    public void FromCurl_OptionsRequest_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -X OPTIONS http://example.com");
        Assert.Equal(HttpMethod.Options, builder.HttpMethod);
    }

    [Fact]
    public void FromCurl_MultipleHeaders_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(
            "curl -H 'Accept: application/json' -H 'X-API-Key: secret' http://example.com");
        Assert.NotNull(builder.Headers);
        Assert.Equal("application/json", builder.Headers["Accept"][0]);
        Assert.Equal("secret", builder.Headers["X-API-Key"][0]);
    }

    [Fact]
    public void FromCurl_BasicAuth_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -u user:pass http://example.com");
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void FromCurl_BasicAuthWithoutPassword_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -u username http://example.com");
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("username:", decoded);
    }

    [Fact]
    public void FromCurl_BearerAuth_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl --bearer my-token http://example.com");
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Bearer", builder.AuthenticationHeader.Scheme);
        Assert.Equal("my-token", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void FromCurl_DigestAuth_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -u user:pass --digest http://example.com");
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader.Scheme);
        Assert.Equal("user|:|pass", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void FromCurl_BasicToDigestSwitch_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -u user:pass --digest http://example.com");
        Assert.Equal("Digest", builder.AuthenticationHeader?.Scheme);
    }

    [Fact]
    public void FromCurl_Cookies_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -b 'session=abc; user=john' http://example.com");
        Assert.NotNull(builder.Cookies);
        Assert.Equal("abc", builder.Cookies["session"]);
        Assert.Equal("john", builder.Cookies["user"]);
    }

    [Fact]
    public void FromCurl_FormUrlEncoded_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(
            "curl -X POST -H 'Content-Type: application/x-www-form-urlencoded' -d 'id=200&name=furion' http://example.com");
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        Assert.Equal("id=200&name=furion", builder.RawContent);
    }

    [Fact]
    public void FromCurl_DataUrlencode_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(
            "curl --data-urlencode 'id=200' --data-urlencode 'name=fu rion' http://example.com");
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        Assert.NotNull(builder.RawContent);
        Assert.Contains("id=200", builder.RawContent?.ToString());
        Assert.Contains("name=fu", builder.RawContent?.ToString());
    }

    [Fact]
    public void FromCurl_DataBinary_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(filePath, new byte[] { 0x01, 0x02, 0x03 });

            var builder = HttpRequestBuilder.FromCurl(
                $"curl --data-binary '@{filePath}' http://example.com");

            Assert.Equal("application/octet-stream", builder.ContentType);
            var rawContent = Assert.IsType<byte[]>(builder.RawContent);
            Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, rawContent);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_DataRaw_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl --data-raw 'raw data' http://example.com");
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        Assert.Equal("raw data", builder.RawContent);
    }

    [Fact]
    public void FromCurl_DataAscii_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl --data-ascii 'key=value' http://example.com");
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        Assert.Equal("key=value", builder.RawContent);
    }

    [Fact]
    public void FromCurl_MultipleDataAppend_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -d 'a=1' -d 'b=2' http://example.com");
        Assert.Equal("a=1&b=2", builder.RawContent);
    }

    [Fact]
    public void FromCurl_ContentTypeWithDataNotOverride_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(
            "curl -H 'Content-Type: application/json' -d '{\"a\":1}' http://example.com");
        Assert.Equal("application/json", builder.ContentType);
    }

    [Fact]
    public void FromCurl_MultipartFormWithFields_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -F 'name=John' -F 'age=30' http://example.com");
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.Equal(2, items.Count);
        Assert.Equal("name", items[0].Name);
        Assert.Equal("John", items[0].RawContent);
        Assert.Equal("age", items[1].Name);
        Assert.Equal("30", items[1].RawContent);
    }

    [Fact]
    public void FromCurl_FileUpload_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "test content");
            builder = HttpRequestBuilder.FromCurl($"curl -F 'file=@{filePath}' http://example.com");
            var items = builder.MultipartFormDataBuilder!._partContents;
            Assert.Single(items);
            Assert.Equal("file", items[0].Name);
            Assert.NotNull(items[0].FileName);
            Assert.Equal(Path.GetFileName(filePath), items[0].FileName);
        }
        finally
        {
            builder?.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void FromCurl_FileUploadWithType_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "content");
            builder = HttpRequestBuilder.FromCurl($"curl -F 'file=@{filePath};type=image/png' http://example.com");
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Equal("image/png", items[0].ContentType);
        }
        finally
        {
            builder?.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void FromCurl_FileUploadWithFilename_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        HttpRequestBuilder builder = null!;
        try
        {
            File.WriteAllText(filePath, "data");
            builder = HttpRequestBuilder.FromCurl(
                $"curl -F 'file=@{filePath};filename=renamed.txt' http://example.com");
            Assert.NotNull(builder.MultipartFormDataBuilder);
            var items = builder.MultipartFormDataBuilder._partContents;
            Assert.Equal("renamed.txt", items[0].FileName);
        }
        finally
        {
            builder?.ReleaseResources();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public void FromCurl_FormWithEmptyValue_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -F 'name=' http://example.com");
        Assert.NotNull(builder.MultipartFormDataBuilder);
        var items = builder.MultipartFormDataBuilder._partContents;
        Assert.NotNull(items);
        Assert.Single(items);
        Assert.Equal("name", items[0].Name);
    }

    [Fact]
    public void FromCurl_UserAgent_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -A 'MyAgent/1.0' http://example.com");
        Assert.NotNull(builder.Headers);
        Assert.Equal("MyAgent/1.0", builder.Headers["User-Agent"][0]);
    }

    [Fact]
    public void FromCurl_Referer_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -e 'https://referer.com' http://example.com");
        Assert.NotNull(builder.Headers);
        Assert.Equal("https://referer.com", builder.Headers[HeaderNames.Referer][0]);
    }

    [Fact]
    public void FromCurl_Timeout_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -m 10 http://example.com");
        Assert.NotNull(builder.TimeoutOptions?.Timeout);
        Assert.Equal(TimeSpan.FromSeconds(10), builder.TimeoutOptions!.Timeout);
    }

    [Fact]
    public void FromCurl_HttpVersion_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl --http2 http://example.com");
        Assert.NotNull(builder.Version);
        Assert.Equal(new Version(2, 0), builder.Version);
    }

    [Fact]
    public void FromCurl_CombinedOptions_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl(@"
            curl -X POST 'http://example.com/api' \
            -H 'Content-Type: application/json' \
            -H 'Accept: application/json' \
            -u user:pass \
            -d '{\""key\"":\""value\""}'
        ");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
        Assert.Equal("application/json", builder.ContentType);
        Assert.NotNull(builder.Headers);
        Assert.Equal("application/json", builder.Headers["Accept"][0]);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader.Scheme);
        Assert.Contains("value", builder.RawContent?.ToString());
    }

    [Fact]
    public void FromCurl_WithCustomExtractor_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl http://example.com", options =>
            options.AddExtractor(new CurlMethodExtractor()));
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
    }

    [Fact]
    public void FromCurl_ImplicitGetWhenNoData_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl https://example.com");
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Null(builder.RawContent);
    }

    [Fact]
    public void FromCurl_ImplicitPostWhenDataExists_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -d 'a=1' http://example.com");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
    }

    [Fact]
    public void FromCurl_ImplicitPostWhenFormExists_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl -F 'name=val' http://example.com");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
    }

    [Fact]
    public void FromCurl_UnrecognizedTokensSkipped_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl --unknown-flag http://example.com");
        Assert.Equal("http://example.com", builder.RequestUri?.OriginalString);
    }

    [Fact]
    public void FromCurl_RelativeUrl_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl /api/data");
        Assert.Equal("/api/data", builder.RequestUri?.OriginalString);
        Assert.False(builder.RequestUri?.IsAbsoluteUri);
    }

    [Fact]
    public void FromCurl_DataFlagWithFileRead_ReturnOK()
    {
        var filePath = CreateTempFile("line1\nline2");
        try
        {
            var builder = HttpRequestBuilder.FromCurl($"curl -d @{filePath} http://example.com");

            Assert.Equal("line1\nline2", builder.RawContent);
            Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_DataAliasWithFileRead_ReturnOK()
    {
        var filePath = CreateTempFile("data content");
        try
        {
            var builder = HttpRequestBuilder.FromCurl($"curl --data @{filePath} http://example.com");

            Assert.Equal("data content", builder.RawContent);
            Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_DataAsciiFlagWithFileRead_ReturnOK()
    {
        var filePath = CreateTempFile("ascii data");
        try
        {
            var builder = HttpRequestBuilder.FromCurl($"curl --data-ascii @{filePath} http://example.com");

            Assert.Equal("ascii data", builder.RawContent);
            Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_DataBinaryFlagWithFileRead_ReturnOK()
    {
        var filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(filePath, "binary bytes"u8.ToArray());

            var builder = HttpRequestBuilder.FromCurl($"curl --data-binary @{filePath} http://example.com");

            var rawContent = Assert.IsType<byte[]>(builder.RawContent);
            Assert.Equal("binary bytes"u8.ToArray(), rawContent);
            Assert.Equal("application/octet-stream", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_DataUrlencodeFlagWithFileRead_EncodesContent_ReturnOK()
    {
        var filePath = CreateTempFile("hello world=foo");
        try
        {
            var builder = HttpRequestBuilder.FromCurl($"curl --data-urlencode @{filePath} http://example.com");

            Assert.Equal("hello+world%3Dfoo", builder.RawContent);
            Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
            Assert.Null(builder.HttpContentProcessorProviders);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_DataRawFlagWithAtSign_NotFileRead_ReturnOK()
    {
        var builder = HttpRequestBuilder.FromCurl("curl --data-raw @some/path http://example.com");

        Assert.Equal("@some/path", builder.RawContent);
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
    }

    [Fact]
    public void FromCurl_FileReadWhenContentTypeAlreadySet_NoOverride_ReturnOK()
    {
        var filePath = CreateTempFile("data");
        try
        {
            var builder = HttpRequestBuilder.FromCurl(
                $"curl -H 'Content-Type: application/json' -d @{filePath} http://example.com");

            Assert.Equal("data", builder.RawContent);
            Assert.Equal("application/json", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_FileReadAppendContent_ReturnOK()
    {
        var filePath = CreateTempFile("second");
        try
        {
            var builder = HttpRequestBuilder.FromCurl(
                $"curl -d first -d @{filePath} http://example.com");

            Assert.Equal("first&second", builder.RawContent);
            Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void FromCurl_FileReadNonExistentFile_Invalid_Parameters()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Assert.Throws<FileNotFoundException>(() =>
            HttpRequestBuilder.FromCurl($"curl -d @{nonExistentPath} http://example.com"));
    }
}