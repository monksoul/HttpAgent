// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpRemoteApplicationBuilderExtensionsTests
{
    [Fact]
    public async Task EnableBuffering_Invalid_Parameters()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        await using var app = builder.Build();

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, AspNetCoreModel model) =>
        {
            var httpRequest = context.Request;
            httpRequest.Body.Position = 0;
            using var memoryStream = new MemoryStream();
            await httpRequest.Body.CopyToAsync(memoryStream, context.RequestAborted);
            memoryStream.Position = 0;

            await context.Response.WriteAsJsonAsync(model);
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new AspNetCoreModel { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task EnableBuffering_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        await using var app = builder.Build();
        app.UseEnableBuffering();

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, AspNetCoreModel model) =>
        {
            var httpRequest = context.Request;
            httpRequest.Body.Position = 0;
            using var memoryStream = new MemoryStream();
            await httpRequest.Body.CopyToAsync(memoryStream, context.RequestAborted);
            memoryStream.Position = 0;

            await context.Response.WriteAsJsonAsync(model);
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new AspNetCoreModel { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}",
            await httpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task UseHttpRemoteClient_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddHttpRemote();

        await using var app = builder.Build().UseHttpRemoteClient();
        (app as IApplicationBuilder).UseHttpRemoteClient();

        Assert.Same(app.Services, HttpRemoteClient._externalServiceProvider);
        Assert.Same(app.Services.GetRequiredService<IHttpRemoteService>(), HttpRemoteClient.Service);

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    private class AspNetCoreModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}