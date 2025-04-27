// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using SameSiteMode = Microsoft.Net.Http.Headers.SameSiteMode;

namespace HttpAgent.Tests;

public class HttpRemoteResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal(httpResponseMessage, httpRemoteResult.ResponseMessage);
        Assert.Null(httpRemoteResult.ContentType);
        Assert.Null(httpRemoteResult.CharSet);
        Assert.NotNull(httpRemoteResult.ContentEncoding);
        Assert.Empty(httpRemoteResult.ContentEncoding);
        Assert.Equal(0, httpRemoteResult.ContentLength);
        Assert.NotNull(httpRemoteResult.Server);
        Assert.Empty(httpRemoteResult.Server);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.Null(httpRemoteResult.Result);
        Assert.Equal(0, httpRemoteResult.RequestDuration);
        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.NotNull(httpRemoteResult.Headers);
        Assert.NotNull(httpRemoteResult.ContentHeaders);
        Assert.Equal("1.1", httpRemoteResult.Version.ToString());
        Assert.Null(httpRemoteResult.HttpClientName);
    }

    [Fact]
    public void ParseStatusCode_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unused);

        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage);
        httpRemoteResult.ParseStatusCode();

        Assert.Equal(HttpStatusCode.Unused, httpRemoteResult.StatusCode);
        Assert.False(httpRemoteResult.IsSuccessStatusCode);
    }

    [Fact]
    public void ParseHeaders_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Headers.TryAddWithoutValidation("test", "furion");
        httpResponseMessage.Headers.TryAddWithoutValidation("Server", "Apache/2.4.1 (Unix)");
        var stringContent = new StringContent("furion", Encoding.UTF8,
            new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });
        httpResponseMessage.Content = stringContent;

        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage);
        httpRemoteResult.ParseHeaders();

        Assert.Equal("furion", httpRemoteResult.Headers.GetValues("test").FirstOrDefault());
        Assert.NotNull(httpRemoteResult.Headers.Server);
        Assert.NotEmpty(httpRemoteResult.Headers.Server);
        Assert.Equal("Apache/2.4.1 (Unix)", httpRemoteResult.Headers.Server.ToString());
        Assert.Equal(6, httpRemoteResult.ContentHeaders.ContentLength);
        Assert.Equal("application/json", httpRemoteResult.ContentHeaders.ContentType?.MediaType);
        Assert.Equal("utf-8", httpRemoteResult.ContentHeaders.ContentType?.CharSet);
    }

    [Fact]
    public void ParseContentMetadata_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        var stringContent = new StringContent("furion", Encoding.UTF8,
            new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });
        httpResponseMessage.Content = stringContent;

        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage);
        httpRemoteResult.ParseContentMetadata(httpResponseMessage.Content.Headers);

        Assert.Equal(6, httpRemoteResult.ContentLength);
        Assert.Equal("application/json", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
    }

    [Fact]
    public void ParseSetCookies_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage);
        httpRemoteResult.ParseSetCookies(httpResponseMessage.Headers);

        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);

        var httpResponseMessage2 = new HttpResponseMessage();
        const string setCookieHeader =
            "BDUSS_BFESS=hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm; Path=/; Domain=baidu.com; Expires=Fri, 01 Sep 2034 02:22:19 GMT; Max-Age=315360000; HttpOnly; Secure; SameSite=None";

        httpResponseMessage2.Headers.Add("Set-Cookie", setCookieHeader);

        var httpRemoteResult2 = new HttpRemoteResult<string>(httpResponseMessage2);
        httpRemoteResult2.ParseSetCookies(httpResponseMessage2.Headers);

        Assert.NotNull(httpRemoteResult2.RawSetCookies);
        Assert.NotNull(httpRemoteResult2.SetCookies);
        Assert.Equal(setCookieHeader, httpRemoteResult2.RawSetCookies.First());
        Assert.Single(httpRemoteResult2.SetCookies);

        var cookies = httpRemoteResult2.SetCookies.First();
        Assert.Equal("baidu.com", cookies.Domain);
        Assert.Equal("/", cookies.Path);
        Assert.Equal("2034/9/1 2:22:19 +00:00", cookies.Expires.ToString());
        Assert.Equal(TimeSpan.FromSeconds(315360000), cookies.MaxAge);
        Assert.True(cookies.HttpOnly);
        Assert.True(cookies.Secure);
        Assert.Equal(SameSiteMode.None, cookies.SameSite);
        Assert.Equal("BDUSS_BFESS", cookies.Name);
        Assert.Equal(
            "hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm",
            cookies.Value);
    }

    [Fact]
    public void Initialize_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Version = new Version(1, 2);
        httpResponseMessage.RequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/"));
        httpResponseMessage.RequestMessage.Options.TryAdd(Constants.HTTP_CLIENT_NAME, "github");
        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage);
        httpRemoteResult.Initialize();

        Assert.Equal("1.2", httpRemoteResult.Version.ToString());
        Assert.Equal("github", httpRemoteResult.HttpClientName);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);

        var httpResponseMessage2 = new HttpResponseMessage();
        const string setCookieHeader =
            "BDUSS_BFESS=hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm; Path=/; Domain=baidu.com; Expires=Fri, 01 Sep 2034 02:22:19 GMT; Max-Age=315360000; HttpOnly; Secure; SameSite=None";

        httpResponseMessage2.Headers.Add("Set-Cookie", setCookieHeader);

        var httpRemoteResult2 = new HttpRemoteResult<string>(httpResponseMessage2);
        httpRemoteResult2.Initialize();

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.NotNull(httpRemoteResult2.RawSetCookies);
        Assert.NotNull(httpRemoteResult2.SetCookies);
        Assert.Equal(setCookieHeader, httpRemoteResult2.RawSetCookies.First());
        Assert.Single(httpRemoteResult2.SetCookies);

        var cookies = httpRemoteResult2.SetCookies.First();
        Assert.Equal("baidu.com", cookies.Domain);
        Assert.Equal("/", cookies.Path);
        Assert.Equal("2034/9/1 2:22:19 +00:00", cookies.Expires.ToString());
        Assert.Equal(TimeSpan.FromSeconds(315360000), cookies.MaxAge);
        Assert.True(cookies.HttpOnly);
        Assert.True(cookies.Secure);
        Assert.Equal(SameSiteMode.None, cookies.SameSite);
        Assert.Equal("BDUSS_BFESS", cookies.Name);
        Assert.Equal(
            "hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm",
            cookies.Value);
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        var httpResponseMessage =
            new HttpResponseMessage { RequestMessage = httpRequestMessage, StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage) { RequestDuration = 200 };

        Assert.Equal(
            "Request Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\nGeneral: \r\n\tRequest URL:               http://localhost\r\n\tHTTP Method:               GET\r\n\tStatus Code:               200 OK\r\n\tHTTP Version:              1.1\r\n\tHTTP Content:              \r\n\tRequest Duration (ms):     200.00\r\nResponse Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json\r\n\tContent-Length:      0",
            httpRemoteResult.ToString());
    }

    [Fact]
    public void Deconstruct_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        var stringContent = new StringContent("furion", Encoding.UTF8,
            new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });
        httpResponseMessage.Content = stringContent;
        var httpRemoteResult = new HttpRemoteResult<string>(httpResponseMessage) { RequestDuration = 200 };

        var (result, response) = httpRemoteResult;
        Assert.Equal("furion", result);
        Assert.NotNull(response);

        var (result2, response2, isSuccess) = httpRemoteResult;
        Assert.Equal("furion", result2);
        Assert.NotNull(response2);
        Assert.True(isSuccess);

        var (result3, response3, isSuccess2, statusCode) = httpRemoteResult;
        Assert.Equal("furion", result3);
        Assert.NotNull(response3);
        Assert.True(isSuccess2);
        Assert.Equal(HttpStatusCode.OK, statusCode);
    }
}