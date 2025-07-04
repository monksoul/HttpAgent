// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HelpersTests
{
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
        var stream = HttpAgent.Helpers.GetStreamFromRemote(
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe");
        Assert.NotNull(stream);
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
    public void IsFormUrlEncodedString_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpAgent.Helpers.IsFormUrlEncodedFormat(null!));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.IsFormUrlEncodedFormat(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.IsFormUrlEncodedFormat(" "));
    }

    [Theory]
    [InlineData("furion", false)]
    [InlineData("id=1&name=furion", true)]
    [InlineData("id=1", true)]
    [InlineData("name=furion", true)]
    [InlineData("key=value%20with%20space", true)]
    [InlineData("invalid&key", false)]
    [InlineData("key value", false)]
    [InlineData("=value", false)]
    [InlineData("key=", true)]
    [InlineData("key%3Dvalue", false)]
    public void IsFormUrlEncodedFormat_ReturnOK(string output, bool result) =>
        Assert.Equal(result, HttpAgent.Helpers.IsFormUrlEncodedFormat(output));

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
            HttpAgent.Helpers.GetContentTypeOrDefault(new { }, MediaTypeNames.Text.Plain));
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
        Assert.Equal(MediaTypeNames.Multipart.FormData,
            HttpAgent.Helpers.GetContentTypeOrDefault(new MultipartContent(), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Text.Plain,
            HttpAgent.Helpers.GetContentTypeOrDefault(new StringContent(""), MediaTypeNames.Text.Plain));
        Assert.Equal(MediaTypeNames.Application.Json,
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
    }
}