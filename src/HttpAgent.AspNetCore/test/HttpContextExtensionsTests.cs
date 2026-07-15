// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextExtensionsTests
{
    [Fact]
    public async Task GetFullRequestUrl_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        string[] urls = ["--urls", $"http://localhost:{port}"];
        var builder = WebApplication.CreateBuilder(urls);

        await using var app = builder.Build();

        app.MapGet("/test", async httpContext =>
        {
            var urlAddress = httpContext.Request.GetFullRequestUrl();
            Assert.Equal($"http://localhost:{port}/test", urlAddress);

            await Task.CompletedTask;
        });

        await app.StartAsync();

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(nameof(HttpContextExtensionsTests));

        var httpResponseMessage = await httpClient.GetAsync($"http://localhost:{port}/test");
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public void CreateForwardBuilder_ReturnOK()
    {
        var services = new ServiceCollection();
        using var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var builder = httpContext.CreateForwardBuilder(HttpMethod.Get, (Uri?)null);
        Assert.NotNull(builder);
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Null(builder.RequestUri);
        Assert.NotNull(builder.ForwardOptions);

        var httpContext2 = new DefaultHttpContext
        {
            Request = { Headers = { ["X-Forward-To"] = "https://furion.net" }, Method = "GET" },
            RequestServices = provider
        };

        var builder2 = httpContext2.CreateForwardBuilder(HttpMethod.Get, (Uri?)null,
            forwardOptions: new HttpContextForwardOptions
            {
                AllowedHosts = ["*"]
            });
        Assert.NotNull(builder2);
        Assert.Equal(HttpMethod.Get, builder2.HttpMethod);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("https://furion.net/", builder2.RequestUri.ToString());
        Assert.NotNull(builder2.ForwardOptions);

        var services2 = new ServiceCollection();
        services2.AddOptions<HttpContextForwardOptions>().Configure(options =>
        {
            options.WithResponseContentHeaders = false;
        });
        using var provider2 = services2.BuildServiceProvider();
        var httpContext3 = new DefaultHttpContext { RequestServices = provider2 };
        var builder3 = httpContext3.CreateForwardBuilder(HttpMethod.Get, (Uri?)null);
        Assert.NotNull(builder3);
        Assert.Equal(HttpMethod.Get, builder3.HttpMethod);
        Assert.Null(builder3.RequestUri);
        Assert.NotNull(builder3.ForwardOptions);
        Assert.False(builder3.ForwardOptions.WithResponseContentHeaders);

        var builder4 = httpContext2.CreateForwardBuilder((Uri?)null,
            forwardOptions: new HttpContextForwardOptions
            {
                AllowedHosts = ["*"]
            });
        Assert.NotNull(builder4);
        Assert.Equal(HttpMethod.Get, builder4.HttpMethod);

        var builder5 = httpContext2.CreateForwardBuilder(HttpMethod.Get, "https://furion.net",
            forwardOptions: new HttpContextForwardOptions
            {
                AllowedHosts = ["*"]
            });
        Assert.NotNull(builder5);
        Assert.Equal("https://furion.net/", builder4.RequestUri?.ToString());

        var builder6 = httpContext2.CreateForwardBuilder("https://furion.net",
            forwardOptions: new HttpContextForwardOptions
            {
                AllowedHosts = ["*"]
            });
        Assert.NotNull(builder6);
        Assert.Equal(HttpMethod.Get, builder6.HttpMethod);
    }

    [Fact]
    public void PrepareForwardService_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        using var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider };

        var tuple = HttpContextExtensions.PrepareForwardService(httpContext, HttpMethod.Get, null);
        Assert.NotNull(tuple.httpContextForwardBuilder);
        Assert.NotNull(tuple.httpRequestBuilder);
        Assert.NotNull(tuple.httpRemoteService);
    }

    [Fact]
    public async Task PrepareForwardServiceAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider };

        var tuple = await HttpContextExtensions.PrepareForwardServiceAsync(httpContext, HttpMethod.Get, null);
        Assert.NotNull(tuple.httpContextForwardBuilder);
        Assert.NotNull(tuple.httpRequestBuilder);
        Assert.NotNull(tuple.httpRemoteService);
    }

    [Fact]
    public async Task ForwardAsync_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpContextExtensions.ForwardAsync<string>(null!, null!, (Uri?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new DefaultHttpContext().ForwardAsync<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAsync_ReturnOK()
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
            var result1 = await context.ForwardAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result2 = await context.ForwardAsync<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = await context.ForwardAsync<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 = await context.ForwardAsync<string>($"http://localhost:{port}/HttpRemote/Request1",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result + " " + result2!.Result + " " + result3!.Result + " " +
                                              result4!.Result);
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
    public async Task ForwardAsync_NonMultipartFormData_ReturnOK()
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
            var result1 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            var result2 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 =
                await context.ForwardAsync<HttpRemoteAspNetCoreModel1>($"http://localhost:{port}/HttpRemote/Request2",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result!.Id + " " + result1.Result.Name + " " +
                                              result2!.Result!.Id + " " + result2.Result.Name +
                                              " " + result3!.Result!.Id + " " + result3.Result.Name + " " +
                                              result4!.Result!.Id + " " +
                                              result4.Result.Name + "");
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
    public async Task ForwardAsync_FormUrlEncoded_ReturnOK()
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

        app.MapPost("/test", [Consumes("application/x-www-form-urlencoded")]
            async (HttpContext context, [FromForm] HttpRemoteAspNetCoreModel1 model) =>
            {
                var result1 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                    new Uri($"http://localhost:{port}/HttpRemote/Request3"),
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

                var result2 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(
                    new Uri($"http://localhost:{port}/HttpRemote/Request3"),
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
                var result3 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                    $"http://localhost:{port}/HttpRemote/Request3",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
                var result4 =
                    await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(
                        $"http://localhost:{port}/HttpRemote/Request3",
                        forwardOptions: new HttpContextForwardOptions
                            { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

                await context.Response.WriteAsync(result1!.Result!.Id + " " + result1.Result.Name + " " +
                                                  result2!.Result!.Id + " " + result2.Result.Name +
                                                  " " + result3!.Result!.Id + " " + result3.Result.Name + " " +
                                                  result4!.Result!.Id + " " +
                                                  result4.Result.Name + "");
            }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var formUrlEncodedContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("id", "1"), new KeyValuePair<string, string>("name", "furion")
        ]);

        httpRequestMessage.Content = formUrlEncodedContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_FormFile_ReturnOK()
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
            var result1 = await context.ForwardAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request4"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            var result2 = await context.ForwardAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request4"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = await context.ForwardAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request4",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 =
                await context.ForwardAsync<string>($"http://localhost:{port}/HttpRemote/Request4",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result + " " + result2!.Result + " " + result3!.Result + " " +
                                              result4!.Result);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "file", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("test.txt test.txt test.txt test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_FormFileCollection_ReturnOK()
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
            var result1 = await context.ForwardAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request5"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            var result2 = await context.ForwardAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request5"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = await context.ForwardAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request5",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 =
                await context.ForwardAsync<string>($"http://localhost:{port}/HttpRemote/Request5",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result + " " + result2!.Result + " " + result3!.Result + " " +
                                              result4!.Result);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "files", "test.txt");
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "files", "test2.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("test.txt;test2.txt test.txt;test2.txt test.txt;test2.txt test.txt;test2.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_MultipartFormData_ReturnOK()
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
            var result1 = await context.ForwardAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            var result2 = await context.ForwardAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = await context.ForwardAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 =
                await context.ForwardAsync<string>($"http://localhost:{port}/HttpRemote/Request6",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result + " " + result2!.Result + " " + result3!.Result + " " +
                                              result4!.Result);
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
    public async Task ForwardAsync_FromBody_ReturnOK()
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

        app.MapPost("/test", async (HttpContext context, [FromBody] string str) =>
        {
            var str1 = await context.ForwardAsAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request7"));

            var str2 = await context.ForwardAsAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request7"));
            var str3 = await context.ForwardAsAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request7");
            var str4 = await context.ForwardAsAsync<string>(
                $"http://localhost:{port}/HttpRemote/Request7");

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        // [FromBody] 必须
        // httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var stringContent = new StringContent("\"Furion\"", Encoding.UTF8, "application/json");

        httpRequestMessage.Content = stringContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Furion Furion Furion Furion", str);

        await app.StopAsync();
    }

    [Fact]
    public void Forward_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.Forward<string>(null!, null!, (Uri?)null));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHttpContext().Forward<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task Forward_ReturnOK()
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
            var result1 = context.Forward<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result2 = context.Forward<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = context.Forward<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 = context.Forward<string>($"http://localhost:{port}/HttpRemote/Request1",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result + " " + result2!.Result + " " + result3!.Result + " " +
                                              result4!.Result);
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
    public async Task Forward_NonMultipartFormData_ReturnOK()
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
            var result1 = context.Forward<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            var result2 = context.Forward<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = context.Forward<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 =
                context.Forward<HttpRemoteAspNetCoreModel1>($"http://localhost:{port}/HttpRemote/Request2",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result!.Id + " " + result1.Result.Name + " " +
                                              result2!.Result!.Id + " " + result2.Result.Name +
                                              " " + result3!.Result!.Id + " " + result3.Result.Name + " " +
                                              result4!.Result!.Id + " " +
                                              result4.Result.Name + "");
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
    public async Task Forward_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

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
            var result1 = context.Forward<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            var result2 = context.Forward<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"),
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result3 = context.Forward<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6",
                forwardOptions: new HttpContextForwardOptions
                    { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });
            var result4 =
                context.Forward<string>($"http://localhost:{port}/HttpRemote/Request6",
                    forwardOptions: new HttpContextForwardOptions
                        { IgnoreResponseHeaders = ["Content-Length"], AllowedHosts = ["*"] });

            await context.Response.WriteAsync(result1!.Result + " " + result2!.Result + " " + result3!.Result + " " +
                                              result4!.Result);
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
    public async Task ForwardAsync_Redirect_ReturnOK()
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
            var result1 = await context.ForwardAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request8"));

            await context.Response.WriteAsync(result1!.Result!);
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
    public async Task ForwardAsync_GetFile_ReturnOK()
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
            var result1 = await context.ForwardAsync<Stream>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request9"));

            await result1!.Result!.CopyToAsync(context.Response.Body);
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
    public async Task ForwardAsync_HttpResponse_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpContextExtensions.ForwardAsync(null!, null!, (Uri?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new DefaultHttpContext().ForwardAsync(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAsync_HttpResponse_ReturnOK()
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
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var httpResponseMessage2 =
                await context.ForwardAsync(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var httpResponseMessage3 = await context.ForwardAsync(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var httpResponseMessage4 = await context.ForwardAsync($"http://localhost:{port}/HttpRemote/Request1");

            var str1 = await httpResponseMessage1!.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2!.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3!.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4!.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
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
    public async Task ForwardAsync_HttpResponse_NonMultipartFormData_ReturnOK()
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
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var httpResponseMessage2 = await context.ForwardAsync(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var httpResponseMessage3 = await context.ForwardAsync(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var httpResponseMessage4 =
                await context.ForwardAsync($"http://localhost:{port}/HttpRemote/Request2");

            var model1 = await httpResponseMessage1!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model2 = await httpResponseMessage2!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model3 = await httpResponseMessage3!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model4 = await httpResponseMessage4!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " +
                                              model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " +
                                              model4!.Id + " " +
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
    public async Task ForwardAsync_HttpResponse_MultipartFormData_ReturnOK()
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
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var httpResponseMessage2 = await context.ForwardAsync(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var httpResponseMessage3 = await context.ForwardAsync(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var httpResponseMessage4 =
                await context.ForwardAsync($"http://localhost:{port}/HttpRemote/Request6");

            var str1 = await httpResponseMessage1!.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2!.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3!.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4!.Content.ReadAsStringAsync();

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
    public void Forward_HttpResponse_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.Forward<string>(null!, null!, (Uri?)null));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHttpContext().Forward<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task Forward_HttpResponse_ReturnOK()
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
            var httpResponseMessage1 = context.Forward(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage2 = context.Forward(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage3 = context.Forward(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage4 = context.Forward($"http://localhost:{port}/HttpRemote/Request1");

            var str1 = await httpResponseMessage1!.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2!.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3!.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4!.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
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
    public async Task Forward_HttpResponse_NonMultipartFormData_ReturnOK()
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
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage1 = context.Forward(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage2 = context.Forward(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage3 = context.Forward(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage4 =
                context.Forward($"http://localhost:{port}/HttpRemote/Request2");

            var model1 = await httpResponseMessage1!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model2 = await httpResponseMessage2!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model3 = await httpResponseMessage3!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model4 = await httpResponseMessage4!.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " +
                                              model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " +
                                              model4!.Id + " " +
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
    public async Task Forward_HttpResponse_MultipartFormData_ReturnOK()
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
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage1 = context.Forward(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage2 = context.Forward(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage3 = context.Forward(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage4 =
                context.Forward($"http://localhost:{port}/HttpRemote/Request6");

            var str1 = await httpResponseMessage1!.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2!.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3!.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4!.Content.ReadAsStringAsync();

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
    public async Task ForwardAsync_HttpResponse_Redirect_ReturnOK()
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
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request8"));

            var str1 = await httpResponseMessage1!.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1);
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
    public async Task ForwardAsync_HttpResponse_GetFile_ReturnOK()
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
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request9"));

            var stream = await httpResponseMessage1!.Content.ReadAsStreamAsync();

            await stream.CopyToAsync(context.Response.Body);
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
    public void ForwardResponseMessage_ReturnOK_Invalid_Parameters()
    {
        var httpContext = new DefaultHttpContext();
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        Assert.Throws<ArgumentNullException>(() => HttpContextExtensions.ForwardResponseMessage(null!, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.ForwardResponseMessage(httpContext, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.ForwardResponseMessage(httpContext, httpResponseMessage, null!));
    }

    [Fact]
    public void ForwardResponseMessage_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        httpResponseMessage.Headers.TryAddWithoutValidation("Framework", "furion");
        httpResponseMessage.Headers.TryAddWithoutValidation("Transfer-Encoding", "chunked");
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.txt" };

        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        using var provider = services.BuildServiceProvider();
        var defaultHttpContext = new DefaultHttpContext { RequestServices = provider };

        HttpContextExtensions.ForwardResponseMessage(defaultHttpContext, null, new HttpContextForwardOptions());

        HttpContextExtensions.ForwardResponseMessage(defaultHttpContext, httpResponseMessage,
            new HttpContextForwardOptions());
        Assert.Equal(500, defaultHttpContext.Response.StatusCode);
        Assert.Equal("furion", defaultHttpContext.Response.Headers["Framework"]);
        Assert.DoesNotContain(defaultHttpContext.Response.Headers, h => h.Key == "Transfer-Encoding");
        Assert.Equal("attachment; filename=test.txt", defaultHttpContext.Response.Headers.ContentDisposition);

        var defaultHttpContext2 = new DefaultHttpContext();
        HttpContextExtensions.ForwardResponseMessage(defaultHttpContext2, httpResponseMessage,
            new HttpContextForwardOptions { WithResponseStatusCode = false, WithResponseHeaders = false });
        Assert.Equal(200, defaultHttpContext2.Response.StatusCode);
        Assert.DoesNotContain(defaultHttpContext2.Response.Headers, u => u.Key == "Framework");

        var defaultHttpContext3 = new DefaultHttpContext();
        HttpContextExtensions.ForwardResponseMessage(defaultHttpContext3, httpResponseMessage,
            new HttpContextForwardOptions
            {
                WithResponseStatusCode = false,
                WithResponseHeaders = false,
                OnForward =
                    (ctx, res) => { ctx.Response.ContentLength = 10; }
            });
        Assert.Equal(200, defaultHttpContext3.Response.StatusCode);
        Assert.DoesNotContain(defaultHttpContext3.Response.Headers, u => u.Key == "Framework");
        Assert.Equal(10, defaultHttpContext3.Response.Headers.ContentLength);
    }

    [Fact]
    public void ForwardHttpHeaders_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        httpResponseMessage.Headers.TryAddWithoutValidation("Framework", "furion");
        httpResponseMessage.Headers.TryAddWithoutValidation("Transfer-Encoding", "chunked");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Length", "21");
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.txt" };

        var defaultHttpContext = new DefaultHttpContext();
        HttpContextExtensions.ForwardHttpHeaders(defaultHttpContext.Response, httpResponseMessage.Headers,
            new HttpContextForwardOptions());
        HttpContextExtensions.ForwardHttpHeaders(defaultHttpContext.Response, httpResponseMessage.Content.Headers,
            new HttpContextForwardOptions());
        Assert.Equal("furion", defaultHttpContext.Response.Headers["Framework"]);
        Assert.DoesNotContain(defaultHttpContext.Response.Headers, h => h.Key == "Transfer-Encoding");
        Assert.Equal("attachment; filename=test.txt", defaultHttpContext.Response.Headers.ContentDisposition);

        var defaultHttpContext3 = new DefaultHttpContext();
        HttpContextExtensions.ForwardHttpHeaders(defaultHttpContext3.Response, httpResponseMessage.Content.Headers,
            new HttpContextForwardOptions());
        Assert.Equal(21, defaultHttpContext3.Response.ContentLength);

        var defaultHttpContext4 = new DefaultHttpContext();
        HttpContextExtensions.ForwardHttpHeaders(defaultHttpContext4.Response, httpResponseMessage.Content.Headers,
            new HttpContextForwardOptions { IgnoreResponseHeaders = ["Content-Length"] });
        Assert.Null(defaultHttpContext4.Response.ContentLength);
    }

    [Fact]
    public void IgnoreResponseHeaders_ReturnOK() =>
        Assert.Equal(
            [
                "Content-Type", "Connection", "Transfer-Encoding", "Keep-Alive", "Upgrade", "Proxy-Connection"
            ],
            HttpContextExtensions._ignoreResponseHeaders);

    [Fact]
    public async Task ForwardAsync_WithException_ReturnOK()
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
            var actionResult = await context.ForwardAsync<IActionResult>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request11"),
                hbuilder => { hbuilder.AddHttpContentConverters(() => [new IActionResultContentConverter()]); });

            var contentResult = actionResult!.Result as ContentResult;

            await context.Response.WriteAsync(contentResult?.Content ?? string.Empty);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri($"http://localhost:{port}/test")));
        // httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Contains("出错了", str);

        await app.StopAsync();
    }
}