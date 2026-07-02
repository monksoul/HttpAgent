// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpMultipartFormDataBuilderExtensionsTests
{
    [Fact]
    public void AddFile_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.AddFile((IFormFile)null!));
    }

    [Fact]
    public async Task AddFile_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpRemote();
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                var httpRemoteService = context.RequestServices.GetRequiredService<IHttpRemoteService>();

                var httpRequestBuilder = HttpRequestBuilder
                    .Post($"http://localhost:{port}/test/file")
                    .SetMultipartContent(multipart => multipart.AddFile(file));
                Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
                Assert.NotNull(httpRequestBuilder.Disposables);
                Assert.Single(httpRequestBuilder.Disposables);
                Assert.Single(httpRequestBuilder.MultipartFormDataBuilder._partContents);
                var partContent = httpRequestBuilder.MultipartFormDataBuilder._partContents[0];
                Assert.Equal("file", partContent.Name);
                Assert.Equal("test.txt", partContent.FileName);
                Assert.True(partContent.RawContent is Stream);
                Assert.Equal("text/plain", partContent.ContentType);

                var fileName = await httpRemoteService.SendAsStringAsync(httpRequestBuilder);

                await context.Response.WriteAsync(fileName!);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        app.MapPost("/test/file", async (HttpContext context, IFormFile file) =>
            {
                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();
        var httpResponseMessage = await httpRemoteService.SendAsync(HttpRequestBuilder
            .Post($"http://localhost:{port}/test")
            .SetMultipartContent(multipart => multipart.AddFileAsStream(filePath)));
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
    }

    [Fact]
    public void AddFiles_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.AddFiles((IEnumerable<IFormFile>)null!));
    }

    [Fact]
    public async Task AddFiles_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpRemote();
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                var httpRemoteService = context.RequestServices.GetRequiredService<IHttpRemoteService>();

                var httpRequestBuilder = HttpRequestBuilder
                    .Post($"http://localhost:{port}/test/file")
                    .SetMultipartContent(multipart => multipart.AddFiles([file]));
                Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
                Assert.NotNull(httpRequestBuilder.Disposables);
                Assert.Single(httpRequestBuilder.Disposables);
                Assert.Single(httpRequestBuilder.MultipartFormDataBuilder._partContents);
                var partContent = httpRequestBuilder.MultipartFormDataBuilder._partContents[0];
                Assert.Equal("file", partContent.Name);
                Assert.Equal("test.txt", partContent.FileName);
                Assert.True(partContent.RawContent is Stream);
                Assert.Equal("text/plain", partContent.ContentType);

                var fileName = await httpRemoteService.SendAsStringAsync(httpRequestBuilder);

                await context.Response.WriteAsync(fileName!);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        app.MapPost("/test/file", async (HttpContext context, IFormFile file) =>
            {
                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();
        var httpResponseMessage = await httpRemoteService.SendAsync(HttpRequestBuilder
            .Post($"http://localhost:{port}/test")
            .SetMultipartContent(multipart => multipart.AddFileAsStream(filePath)));
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
    }

    [Fact]
    public void AddFile_IBrowserFile_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.AddFile((IBrowserFile)null!));
    }

    [Fact]
    public void AddFile_IBrowserFile_ReturnOK()
    {
        // Arrange
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var browserFile = new TestBrowserFile("test.txt", "text/plain", "Hello"u8.ToArray());

        // Act
        builder.AddFile(browserFile);

        // Assert
        Assert.Single(builder._partContents);
        var part = builder._partContents[0];
        Assert.Equal("file", part.Name); // 默认表单字段名
        Assert.Equal("test.txt", part.FileName);
        Assert.True(part.RawContent is Stream);
        Assert.Equal("text/plain", part.ContentType);
    }

    [Fact]
    public void AddFiles_IBrowserFile_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.AddFiles((IEnumerable<IBrowserFile>)null!));
    }

    [Fact]
    public void AddFiles_IBrowserFile_ReturnOK()
    {
        // Arrange
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var browserFile = new TestBrowserFile("test.txt", "text/plain", "World"u8.ToArray());

        // Act
        builder.AddFiles([browserFile]);

        // Assert
        Assert.Single(builder._partContents);
        var part = builder._partContents[0];
        Assert.Equal("file", part.Name);
        Assert.Equal("test.txt", part.FileName);
        Assert.True(part.RawContent is Stream);
        Assert.Equal("text/plain", part.ContentType);
    }

    internal class TestBrowserFile(string name, string contentType = "application/octet-stream", byte[]? content = null)
        : IBrowserFile
    {
        private readonly byte[] _content = content ?? [];

        public string Name { get; } = name;
        public string ContentType { get; } = contentType;
        public long Size => _content.Length;
        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default) =>
            new MemoryStream(_content);
    }
}