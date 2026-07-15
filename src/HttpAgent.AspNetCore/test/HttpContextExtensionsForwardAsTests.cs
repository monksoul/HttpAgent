// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextExtensionsForwardAsTests
{
    [Fact]
    public async Task ForwardAs_WithQueryParameters_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async (HttpContext context, [FromQuery] int number) =>
        {
            var str = context.ForwardAs<int>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request10"));

            await context.Response.WriteAsync(str.ToString());
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test?number=10")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("11", str);

        await app.StopAsync();
    }

    [Fact]
    public void ForwardAs_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.ForwardAs<string>(null!, null!, (Uri?)null));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHttpContext().ForwardAs<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = context.ForwardAs<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str2 = context.ForwardAs<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str3 = context.ForwardAs<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var str4 = context.ForwardAs<string>($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAs_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var model1 = context.ForwardAs<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var model2 = context.ForwardAs<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var model3 = context.ForwardAs<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var model4 =
                context.ForwardAs<HttpRemoteAspNetCoreModel1>($"http://localhost:{port}/HttpRemote/Request2");

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " + model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " + model4!.Id + " " +
                                              model4.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAs_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var str1 = context.ForwardAs<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var str2 = context.ForwardAs<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var str3 = context.ForwardAs<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var str4 =
                context.ForwardAs<string>($"http://localhost:{port}/HttpRemote/Request6");

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpContextExtensions.ForwardAsAsync<string>(null!, null!, (Uri?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new DefaultHttpContext().ForwardAsAsync<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = await context.ForwardAsAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str2 = await context.ForwardAsAsync<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str3 = await context.ForwardAsAsync<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var str4 = await context.ForwardAsAsync<string>($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Contains(httpResponseMessage.Headers, x => x.Key == "Framework");
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var model1 = await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var model2 = await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var model3 = await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var model4 =
                await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(
                    $"http://localhost:{port}/HttpRemote/Request2");

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " + model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " + model4!.Id + " " +
                                              model4.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var str1 = await context.ForwardAsAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var str2 = await context.ForwardAsAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var str3 = await context.ForwardAsAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var str4 =
                await context.ForwardAsAsync<string>($"http://localhost:{port}/HttpRemote/Request6");

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_Redirect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = await context.ForwardAsAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request8"));

            await context.Response.WriteAsync(str!);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Redirect", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_GetFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async context =>
        {
            var stream = await context.ForwardAsAsync<Stream>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request9"));

            await stream!.CopyToAsync(context.Response.Body);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        Assert.True(stream.CanRead);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_WithQueryParameters_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async (HttpContext context, [FromQuery] int number) =>
        {
            var str = await context.ForwardAsAsync<int>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request10"));

            await context.Response.WriteAsync(str.ToString());
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test?number=10")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("11", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsString_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var str = context.ForwardAsString(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            // ReSharper disable once MethodHasAsyncOverload
            var str2 = context.ForwardAsString(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            // ReSharper disable once MethodHasAsyncOverload
            var str3 = context.ForwardAsString(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            // ReSharper disable once MethodHasAsyncOverload
            var str4 = context.ForwardAsString($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsStringAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = await context.ForwardAsStringAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str2 = await context.ForwardAsStringAsync(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str3 = await context.ForwardAsStringAsync(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var str4 = await context.ForwardAsStringAsync($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsByteArray_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var str = Encoding.UTF8.GetString(context.ForwardAsByteArray(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"))!);
            // ReSharper disable once MethodHasAsyncOverload
            var str2 = Encoding.UTF8.GetString(
                context.ForwardAsByteArray(new Uri($"http://localhost:{port}/HttpRemote/Request1"))!);
            // ReSharper disable once MethodHasAsyncOverload
            var str3 = Encoding.UTF8.GetString(context.ForwardAsByteArray(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1")!);
            // ReSharper disable once MethodHasAsyncOverload
            var str4 = Encoding.UTF8.GetString(
                context.ForwardAsByteArray($"http://localhost:{port}/HttpRemote/Request1")!);

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsByteArrayAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = Encoding.UTF8.GetString((await context.ForwardAsByteArrayAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1")))!);
            var str2 = Encoding.UTF8.GetString(
                (await context.ForwardAsByteArrayAsync(new Uri($"http://localhost:{port}/HttpRemote/Request1")))!);
            var str3 = Encoding.UTF8.GetString((await context.ForwardAsByteArrayAsync(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1"))!);
            var str4 = Encoding.UTF8.GetString(
                (await context.ForwardAsByteArrayAsync($"http://localhost:{port}/HttpRemote/Request1"))!);

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsStream_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var str = StreamToString(context.ForwardAsStream(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"))!);
            // ReSharper disable once MethodHasAsyncOverload
            var str2 = StreamToString(
                context.ForwardAsStream(new Uri($"http://localhost:{port}/HttpRemote/Request1"))!);
            // ReSharper disable once MethodHasAsyncOverload
            var str3 = StreamToString(context.ForwardAsStream(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1")!);
            // ReSharper disable once MethodHasAsyncOverload
            var str4 = StreamToString(
                context.ForwardAsStream($"http://localhost:{port}/HttpRemote/Request1")!);

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsStreamAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = StreamToString((await context.ForwardAsStreamAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1")))!);
            var str2 = StreamToString(
                (await context.ForwardAsStreamAsync(new Uri($"http://localhost:{port}/HttpRemote/Request1")))!);
            var str3 = StreamToString((await context.ForwardAsStreamAsync(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1"))!);
            var str4 = StreamToString(
                (await context.ForwardAsStreamAsync($"http://localhost:{port}/HttpRemote/Request1"))!);

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var str = ActionResultToString(context.ForwardAsResult(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"))!);
            // ReSharper disable once MethodHasAsyncOverload
            var str2 = ActionResultToString(
                context.ForwardAsResult(new Uri($"http://localhost:{port}/HttpRemote/Request1"))!);
            // ReSharper disable once MethodHasAsyncOverload
            var str3 = ActionResultToString(context.ForwardAsResult(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1")!);
            // ReSharper disable once MethodHasAsyncOverload
            var str4 = ActionResultToString(
                context.ForwardAsResult($"http://localhost:{port}/HttpRemote/Request1")!);

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsResultAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = ActionResultToString((await context.ForwardAsResultAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1")))!);
            var str2 = ActionResultToString(
                (await context.ForwardAsResultAsync(new Uri($"http://localhost:{port}/HttpRemote/Request1")))!);
            var str3 = ActionResultToString((await context.ForwardAsResultAsync(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1"))!);
            var str4 = ActionResultToString(
                (await context.ForwardAsResultAsync($"http://localhost:{port}/HttpRemote/Request1"))!);

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAs_Object_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var str = context.ForwardAs(typeof(string), HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1")) as string;
            // ReSharper disable once MethodHasAsyncOverload
            var str2 =
                context.ForwardAs(typeof(string), new Uri($"http://localhost:{port}/HttpRemote/Request1")) as string;
            // ReSharper disable once MethodHasAsyncOverload
            var str3 = context.ForwardAs(typeof(string), HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1") as string;
            // ReSharper disable once MethodHasAsyncOverload
            var str4 = context.ForwardAs(typeof(string), $"http://localhost:{port}/HttpRemote/Request1") as string;

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_Object_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = await context.ForwardAsAsync(typeof(string), HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1")) as string;
            var str2 =
                await context.ForwardAsAsync(typeof(string), new Uri($"http://localhost:{port}/HttpRemote/Request1"))
                    as string;
            var str3 = await context.ForwardAsAsync(typeof(string), HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1") as string;
            var str4 =
                await context.ForwardAsAsync(typeof(string),
                    $"http://localhost:{port}/HttpRemote/Request1") as string;

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    private static string StreamToString(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static string ActionResultToString(IActionResult actionResult)
    {
        if (actionResult is ContentResult contentResult)
        {
            return contentResult.Content!;
        }

        return string.Empty;
    }
}