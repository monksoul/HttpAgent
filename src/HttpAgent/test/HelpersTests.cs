// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HelpersTests
{
    [Fact]
    public void HttpQuery_ReturnOK()
    {
        Assert.Equal("QUERY", HttpAgent.Helpers.HttpQuery.ToString());

        var method = HttpAgent.Helpers.HttpQuery;
        Assert.Same(method, HttpAgent.Helpers.HttpQuery);
    }

    [Fact]
    public void GetStreamFromRemote_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpAgent.Helpers.GetStreamFromRemote(null!));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.GetStreamFromRemote(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.GetStreamFromRemote(" "));

        var exception =
            Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.GetStreamFromRemote(@"C:\Temp\text.txt"));
        Assert.Equal(@"Invalid internet address: `C:\Temp\text.txt`. (Parameter 'requestUri')", exception.Message);
    }

    [Fact]
    public void GetStreamFromRemote_ReturnOK()
    {
        using var stream = HttpAgent.Helpers.GetStreamFromRemote("https://furion.net");
        Assert.NotNull(stream);

        using var stream2 = HttpAgent.Helpers.GetStreamFromRemote("https://furion.net",
            (client, request) => { request.Headers.TryAddWithoutValidation("framework", "Furion"); });
        Assert.NotNull(stream2);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("http://localhost", "")]
    [InlineData("http://localhost/test", "")]
    [InlineData("http://localhost/test.pdf", "test.pdf")]
    [InlineData("http://localhost/test.pdf?id=10&name=furion", "test.pdf")]
    [InlineData(
        "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe",
        "Installer_迅捷屏幕录像工具_1.7.9_123.exe")]
    public void GetFileNameFromUri(string? url, string? fileName) =>
        Assert.Equal(fileName,
            HttpAgent.Helpers.GetFileNameFromUri(string.IsNullOrWhiteSpace(url)
                ? null
                : new Uri(url, UriKind.RelativeOrAbsolute)));

    [Fact]
    public void ParseHttpMethod_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpAgent.Helpers.ParseHttpMethod(null));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.ParseHttpMethod(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.ParseHttpMethod(" "));
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("Connect")]
    [InlineData("Options")]
    [InlineData("delete")]
    [InlineData("trace")]
    [InlineData("Unknown")]
    [InlineData("HEAD")]
    [InlineData("PUT")]
    public void ParseHttpMethod_ReturnOK(string httpMethod) => HttpAgent.Helpers.ParseHttpMethod(httpMethod);

    [Fact]
    public void DetermineRedirectMethod_ReturnOK()
    {
        Assert.True(HttpAgent.Helpers.DetermineRedirectMethod(HttpStatusCode.Ambiguous, HttpMethod.Post,
            out var redirectMethod));
        Assert.NotNull(redirectMethod);
        Assert.Equal(HttpMethod.Get, redirectMethod);

        Assert.True(HttpAgent.Helpers.DetermineRedirectMethod(HttpStatusCode.Moved, HttpMethod.Post,
            out var redirectMethod2));
        Assert.NotNull(redirectMethod2);
        Assert.Equal(HttpMethod.Get, redirectMethod2);

        Assert.True(HttpAgent.Helpers.DetermineRedirectMethod(HttpStatusCode.Redirect, HttpMethod.Post,
            out var redirectMethod3));
        Assert.NotNull(redirectMethod3);
        Assert.Equal(HttpMethod.Get, redirectMethod3);

        Assert.True(HttpAgent.Helpers.DetermineRedirectMethod(HttpStatusCode.RedirectMethod, HttpMethod.Post,
            out var redirectMethod4));
        Assert.NotNull(redirectMethod4);
        Assert.Equal(HttpMethod.Get, redirectMethod4);

        Assert.True(HttpAgent.Helpers.DetermineRedirectMethod(HttpStatusCode.RedirectKeepVerb, HttpMethod.Post,
            out var redirectMethod5));
        Assert.NotNull(redirectMethod5);
        Assert.Equal(HttpMethod.Post, redirectMethod5);

        Assert.True(HttpAgent.Helpers.DetermineRedirectMethod((HttpStatusCode)308, HttpMethod.Post,
            out var redirectMethod6));
        Assert.NotNull(redirectMethod6);
        Assert.Equal(HttpMethod.Post, redirectMethod6);
    }

    [Fact]
    public void ParseBaseAddress_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpAgent.Helpers.ParseBaseAddress(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            HttpAgent.Helpers.ParseBaseAddress(new Uri("/test", UriKind.RelativeOrAbsolute)));
        Assert.Equal("The requestUri must be an absolute URI. (Parameter 'requestUri')", exception.Message);
    }

    [Fact]
    public void ParseBaseAddress_ReturnOK() =>
        Assert.Equal("https://furion.net/",
            HttpAgent.Helpers.ParseBaseAddress(new Uri("https://furion.net/user/1")).ToString());

    [Fact]
    public void GetContentTypeOrDefault_ReturnOK()
    {
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(null, MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault("Furion", MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.Json,
            HttpAgent.Helpers.GetContentTypeOrDefault(new { }, MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.Json,
            HttpAgent.Helpers.GetContentTypeOrDefault(new ObjectModel(), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.Json,
            HttpAgent.Helpers.GetContentTypeOrDefault(new List<ObjectModel>(), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(Array.Empty<byte>(), MediaTypeNames.Text.Plain));

        using var stream = new MemoryStream();
        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(stream, MediaTypeNames.Text.Plain));

        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(new ByteArrayContent([]), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(new StreamContent(stream), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded,
            HttpAgent.Helpers.GetContentTypeOrDefault(new FormUrlEncodedContent([]), MediaTypeNames.Text.Plain));
        Assert.Equal("multipart/mixed",
            HttpAgent.Helpers.GetContentTypeOrDefault(new MultipartContent(), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(new StringContent(""), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(new StringContent(""), MediaTypeNames.Application.Json));
        Assert.Equal(MediaTypeNames.Application.Json,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonContent.Create(new { }), MediaTypeNames.Application.Json));
        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(new ReadOnlyMemoryContent(Array.Empty<byte>()),
                MediaTypeNames.Application.Octet));
        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(new ReadOnlyMemory<byte>([]), MediaTypeNames.Application.Octet));
        Assert.Equal(MediaTypeNames.Application.Octet,
            HttpAgent.Helpers.GetContentTypeOrDefault(
                MultipartFile.CreateFromPath(Path.Combine(AppContext.BaseDirectory, "test.txt")),
                MediaTypeNames.Application.Octet));

        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(new FileInfo(Path.Combine(AppContext.BaseDirectory, "test.txt")),
                MediaTypeNames.Application.Octet));

        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonNode.Parse("1"), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonNode.Parse("true"), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonNode.Parse("\"furion\""), MediaTypeNames.Text.Plain));

#if NET10_0_OR_GREATER
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonElement.Parse("1"), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonElement.Parse("true"), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(JsonElement.Parse("\"furion\""), MediaTypeNames.Text.Plain));
#endif
    }

    [Fact]
    public void ExtractFileNameFromContentDisposition_ReturnOK()
    {
        Assert.Null(HttpAgent.Helpers.ExtractFileNameFromContentDisposition(null));

        Assert.Equal("test.safetensors",
            HttpAgent.Helpers.ExtractFileNameFromContentDisposition(
                new ContentDispositionHeaderValue("attachment") { FileName = "test.safetensors" }));

        Assert.Equal("100%_complete.txt",
            HttpAgent.Helpers.ExtractFileNameFromContentDisposition(
                new ContentDispositionHeaderValue("attachment") { FileName = "\"100%_complete.txt\"" }));

        var cdStar = new ContentDispositionHeaderValue("attachment");
        cdStar.Parameters.Add(new NameValueHeaderValue("filename*", "UTF-8''%E4%B8%AD%E6%96%87.txt"));
        Assert.Equal("中文.txt", HttpAgent.Helpers.ExtractFileNameFromContentDisposition(cdStar));

        var cd2047Quoted = new ContentDispositionHeaderValue("attachment");
        cd2047Quoted.Parameters.Add(new NameValueHeaderValue("filename", "\"=?utf-8?B?5Lit5paH?=\""));
        Assert.Equal("中文", HttpAgent.Helpers.ExtractFileNameFromContentDisposition(cd2047Quoted));

        var cd2047Unquoted = new ContentDispositionHeaderValue("attachment");
        cd2047Unquoted.Parameters.Add(new NameValueHeaderValue("filename", "=?utf-8?B?5Lit5paH?="));
        Assert.Equal("中文", HttpAgent.Helpers.ExtractFileNameFromContentDisposition(cd2047Unquoted));

        var cdMojibake = new ContentDispositionHeaderValue("attachment");
        cdMojibake.Parameters.Add(new NameValueHeaderValue("filename", "\"é¿é£.safetensors\""));
        Assert.Equal("长风.safetensors", HttpAgent.Helpers.ExtractFileNameFromContentDisposition(cdMojibake));
    }

    [Fact]
    public void DecodeEncodedWord_ReturnOK()
    {
        Assert.Equal("", HttpAgent.Helpers.DecodeEncodedWord(null));
        Assert.Equal("", HttpAgent.Helpers.DecodeEncodedWord(""));
        Assert.Equal("", HttpAgent.Helpers.DecodeEncodedWord(" "));

        Assert.Equal("DateOnly之后文档.txt",
            HttpAgent.Helpers.DecodeEncodedWord("=?utf-8?B?RGF0ZU9ubHnkuYvlkI7mlofmoaMudHh0?="));

        Assert.Equal("Hello World!", HttpAgent.Helpers.DecodeEncodedWord("=?utf-8?Q?Hello_World=21?="));

        Assert.Equal("中文", HttpAgent.Helpers.DecodeEncodedWord("=?utf-8?B?5Lit?= =?utf-8?B?5paH?="));

        Assert.Equal("DateOnly之后文档.txt",
            HttpAgent.Helpers.DecodeEncodedWord("UTF-8''DateOnly%E4%B9%8B%E5%90%8E%E6%96%87%E6%A1%A3.txt"));

        Assert.Equal("长风.safetensors", HttpAgent.Helpers.DecodeEncodedWord("é¿é£.safetensors"));

        Assert.Equal("hello.txt", HttpAgent.Helpers.DecodeEncodedWord("hello.txt"));

        Assert.Equal("100%_complete.txt", HttpAgent.Helpers.DecodeEncodedWord("100%_complete.txt"));

        Assert.Equal("abc\uFFFDxyz", HttpAgent.Helpers.DecodeEncodedWord("abc\uFFFDxyz"));
    }

    [Fact]
    public void DecodeEncodedBytes_ReturnOK()
    {
        Assert.Equal(new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 },
            HttpAgent.Helpers.DecodeEncodedBytes("Hello_World=21", '='));

        Assert.Equal(new byte[] { 49, 48, 48, 37, 95, 99, 111, 109, 112, 108, 101, 116, 101 },
            HttpAgent.Helpers.DecodeEncodedBytes("100%25_complete", '%'));

        Assert.Equal(new byte[] { 61, 90, 90 },
            HttpAgent.Helpers.DecodeEncodedBytes("=ZZ", '='));

        Assert.Equal(new byte[] { 116, 101, 115, 116, 61, 50 },
            HttpAgent.Helpers.DecodeEncodedBytes("test=2", '='));

        Assert.Equal(new byte[] { 116, 101, 115, 116, 61 },
            HttpAgent.Helpers.DecodeEncodedBytes("test=", '='));

        Assert.Equal(new byte[] { 97, 98, 99 },
            HttpAgent.Helpers.DecodeEncodedBytes("abc", '='));

        Assert.Equal(new byte[] { 171 },
            HttpAgent.Helpers.DecodeEncodedBytes("=aB", '='));
    }

    [Fact]
    public void GetEncodingSafe_ReturnOK()
    {
        Assert.Equal(Encoding.UTF8, HttpAgent.Helpers.GetEncodingSafe("utf-8"));

        Assert.Equal(Encoding.UTF8, HttpAgent.Helpers.GetEncodingSafe("invalid-charset-xxx"));
        Assert.Equal(Encoding.UTF8, HttpAgent.Helpers.GetEncodingSafe(""));
    }

    [Fact]
    public void JoinNonEmptyLines_ReturnOK()
    {
        Assert.Empty(HttpAgent.Helpers.JoinNonEmptyLines());
        Assert.Empty(HttpAgent.Helpers.JoinNonEmptyLines(null));
        Assert.Empty(HttpAgent.Helpers.JoinNonEmptyLines(string.Empty));
        Assert.Empty(HttpAgent.Helpers.JoinNonEmptyLines("  "));
        Assert.Equal("Hello\r\nWorld", HttpAgent.Helpers.JoinNonEmptyLines("Hello", "World"));
        Assert.Equal("Hello\r\nWorld", HttpAgent.Helpers.JoinNonEmptyLines("Hello", null, "World"));
    }

    [Fact]
    public void CombineUrl_ReturnOK()
    {
        Assert.Empty(HttpAgent.Helpers.CombineUrl(null, null));
        Assert.Equal("/furion", HttpAgent.Helpers.CombineUrl("/furion", null));
        Assert.Equal("http://localhost/api", HttpAgent.Helpers.CombineUrl(null, "http://localhost/api"));
        Assert.Equal("http://localhost/api", HttpAgent.Helpers.CombineUrl("/furion", "http://localhost/api"));

        Assert.Equal("/furion/api", HttpAgent.Helpers.CombineUrl("/furion", "/api"));
        Assert.Equal("/furion/api", HttpAgent.Helpers.CombineUrl("/furion", "api"));
        Assert.Equal("/furion/api", HttpAgent.Helpers.CombineUrl("/furion/", "api"));
        Assert.Equal("furion/api", HttpAgent.Helpers.CombineUrl("furion", "/api"));
        Assert.Equal("furion/api", HttpAgent.Helpers.CombineUrl("furion", "api"));

        Assert.Equal("http://localhost/api", HttpAgent.Helpers.CombineUrl("http://localhost/furion", "/api"));
        Assert.Equal("http://localhost/api", HttpAgent.Helpers.CombineUrl("http://localhost/furion", "api"));
        Assert.Equal("http://localhost/furion/api", HttpAgent.Helpers.CombineUrl("http://localhost/furion/", "api"));
        Assert.Equal("http://localhost/api", HttpAgent.Helpers.CombineUrl("http://localhost/furion/", "/api"));

        Assert.Equal("http://localhost/furion/..api",
            HttpAgent.Helpers.CombineUrl("http://localhost/furion/", "..api"));
        Assert.Equal("/furion/..api", HttpAgent.Helpers.CombineUrl("/furion/", "..api"));
        Assert.Equal("furion/..api", HttpAgent.Helpers.CombineUrl("furion/", "..api"));
    }

    [Fact]
    public void GetLocalIPv4_ReturnOK()
    {
        var ipv4 = HttpRemoteUtility.GetLocalIPv4();

        Assert.NotEqual("127.0.0.1", ipv4);
        Assert.NotEqual("::1", ipv4);
        Assert.NotEqual("localhost", ipv4);

        Assert.StartsWith("192.168", ipv4);
    }

    [Fact]
    public void GetLocalMacAddress_ReturnOK()
    {
        var macAddress = HttpRemoteUtility.GetLocalMacAddress();

        Assert.NotNull(macAddress);
        Assert.Contains('-', macAddress);
    }
}