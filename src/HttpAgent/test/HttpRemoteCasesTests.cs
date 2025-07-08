// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteCasesTests
{
    [Fact]
    public async Task SendJson_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendJson")
                    .SetJsonContent(new HttpRemoteModel { Id = 1, Name = "Furion" }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendJson2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendJson")
                    .SetJsonContent("{\"id\":1,\"name\":\"Furion\"}"));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFormUrlEncoded_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFormUrlEncoded")
                    .SetFormUrlEncodedContent(new HttpRemoteModel { Id = 1, Name = "Furion" }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFormUrlEncoded2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFormUrlEncoded")
                    .SetFormUrlEncodedContent(new HttpRemoteModel { Id = 1, Name = "Furion" }, useStringContent: true));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFormUrlEncoded3_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFormUrlEncoded")
                    .SetFormUrlEncodedContent("id=1&name=Furion", useStringContent: true));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFile")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileAsStream(filePath);
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFile2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFile")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileAsByteArray(filePath);
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFile3_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFile")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileFromRemote(
                            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("Installer_迅捷屏幕录像工具_1.7.9_123.exe", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFile4_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var base64String =
            Convert.ToBase64String(await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "test.txt")));
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFile")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileFromBase64String(base64String, "file", "test.txt");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFiles_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var filePath2 = Path.Combine(AppContext.BaseDirectory, "test2.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFiles")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileAsStream(filePath, "files");
                        mBuilder.AddFileAsStream(filePath2, "files");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("test.txt;test2.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFiles2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var filePath2 = Path.Combine(AppContext.BaseDirectory, "test2.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFiles")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileAsByteArray(filePath, "files");
                        mBuilder.AddFileAsByteArray(filePath2, "files");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("test.txt;test2.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddJson(new { id = 1, name = "furion" });
                        mBuilder.AddFileAsStream(filePath);
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFormItem(1, "id");
                        mBuilder.AddFormItem("furion", "name");
                        mBuilder.AddFileAsStream(filePath);
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart3_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddJson("{\"id\":1,\"name\":\"furion\"}");
                        mBuilder.AddFileAsStream(filePath);
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart4_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddObject(new HttpRemoteMultipartModel2
                        {
                            Id = 1, Name = "furion", File = MultipartFile.CreateFromPath(filePath)
                        });
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task Send_IncludeRedirect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Get($"http://localhost:{port}/HttpRemote/RedirectTo"));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.Equal("Redirect", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task Send_GetFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<Stream>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/GetFile"));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult?.StatusCode);
        Assert.NotNull(httpRemoteResult?.Result);
        Assert.True(httpRemoteResult.Result.CanRead);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendRawString_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var str =
            await httpRemoteService.SendAsAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/RawString")
                    .SetRawStringContent("Furion", "application/json"));

        Assert.Equal("Furion", str);

        await app.StopAsync();
    }
}