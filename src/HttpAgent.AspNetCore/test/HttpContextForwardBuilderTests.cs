// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextForwardBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpContextForwardBuilder(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpContextForwardBuilder(new DefaultHttpContext(), null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        Assert.NotNull(HttpContextForwardBuilder._actionResultContentConverterInstance);
        Assert.NotNull(HttpContextForwardBuilder._actionResultContentConverterInstance.Value);

        var services = new ServiceCollection();
        using var provider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = provider };

        var builder = new HttpContextForwardBuilder(httpContext, HttpMethod.Get);
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpContextForwardBuilder(httpContext, HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, builder2.HttpMethod);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.NotNull(builder2.HttpContext);
        Assert.NotNull(builder2.ForwardOptions);

        var httpContext2 = new DefaultHttpContext
        {
            Request = { Headers = { ["X-Forward-To"] = "https://furion.net" } }, RequestServices = provider
        };
        var builder3 = new HttpContextForwardBuilder(httpContext2, HttpMethod.Get);
        Assert.Equal(HttpMethod.Get, builder3.HttpMethod);
        Assert.NotNull(builder3.RequestUri);
        Assert.Equal("https://furion.net/", builder3.RequestUri.ToString());
    }

    [Fact]
    public void IgnoreRequestHeaders_ReturnOK() =>
        Assert.Equal([
                "X-Forward-To", "Host", "Accept", "Accept-CH", "Accept-Charset", "Accept-Encoding", "Accept-Language",
                "Accept-Patch", "Accept-Post", "Accept-Ranges"
            ],
            HttpContextForwardBuilder._ignoreRequestHeaders);

    [Fact]
    public void GetTargetUri_ReturnOK()
    {
        var httpContext1 = new DefaultHttpContext();
        Assert.Null(HttpContextForwardBuilder.GetTargetUri(httpContext1));

        Assert.NotNull(HttpContextForwardBuilder.GetTargetUri(httpContext1, new Uri("https://furion.net")));
    }

    [Fact]
    public void GetForwardOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        using var provider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = provider };

        Assert.NotNull(HttpContextForwardBuilder.GetForwardOptions(httpContext, null));

        var forwardOptions = new HttpContextForwardOptions();
        var forwardOptions1 = HttpContextForwardBuilder.GetForwardOptions(httpContext, forwardOptions);
        Assert.Equal(forwardOptions1.GetHashCode(), forwardOptions1.GetHashCode());

        var services2 = new ServiceCollection();
        services2.AddHttpContextAccessor();
        services2.AddOptions<HttpContextForwardOptions>().Configure(o =>
        {
            o.OnForward = (_, _) => { };
        });
        using var provider2 = services2.BuildServiceProvider();
        var httpContext2 = new DefaultHttpContext { RequestServices = provider2 };
        var forwardOptions2 = HttpContextForwardBuilder.GetForwardOptions(httpContext2, null);
        Assert.NotNull(forwardOptions2);
        Assert.NotNull(forwardOptions2.OnForward);
    }

    [Fact]
    public async Task CopyQueryAndRouteValues_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test/{id:int}", async (HttpContext context, [FromQuery] string name, [FromRoute] int id) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyQueryAndRouteValues(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.QueryParameters);
            Assert.Single(httpRequestBuilder.QueryParameters);
            Assert.Equal("name", httpRequestBuilder.QueryParameters.First().Key);

            Assert.NotNull(httpRequestBuilder.PathParameters);
            Assert.Equal(2, httpRequestBuilder.PathParameters.Count);
            Assert.Equal("name", httpRequestBuilder.PathParameters.ElementAt(0).Key);
            Assert.Equal("furion", httpRequestBuilder.PathParameters.ElementAt(0).Value);
            Assert.Equal("id", httpRequestBuilder.PathParameters.ElementAt(1).Key);
            Assert.Equal("1", httpRequestBuilder.PathParameters.ElementAt(1).Value);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test/1?name=furion")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyQueryAndRouteValues_WithQueryParameters_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test/{id:int}", async (HttpContext context, [FromQuery] string name, [FromRoute] int id) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri,
                new HttpContextForwardOptions { WithQueryParameters = false });
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyQueryAndRouteValues(httpRequestBuilder);

            Assert.Null(httpRequestBuilder.QueryParameters);

            Assert.NotNull(httpRequestBuilder.PathParameters);
            Assert.Equal(2, httpRequestBuilder.PathParameters.Count);
            Assert.Equal("name", httpRequestBuilder.PathParameters.ElementAt(0).Key);
            Assert.Equal("furion", httpRequestBuilder.PathParameters.ElementAt(0).Value);
            Assert.Equal("id", httpRequestBuilder.PathParameters.ElementAt(1).Key);
            Assert.Equal("1", httpRequestBuilder.PathParameters.ElementAt(1).Value);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test/1?name=furion")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyHeaders_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyHeaders_WithResetHostRequestHeader_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri,
                new HttpContextForwardOptions { ResetHostRequestHeader = true });
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(2, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyNonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal("application/json", httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("24", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            HttpContextForwardBuilder.CopyNonMultipartFormData(context.Request.Body,
                new MediaTypeHeaderValue(context.Request.ContentType!).MediaType!,
                httpRequestBuilder);
            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyTextMultipartSectionAsync_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("433", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            var boundary1 = context.Request.ContentType!.Split('=')[1];
            var httpMultipartFormDataBuilder =
                new HttpMultipartFormDataBuilder(httpRequestBuilder) { Boundary = boundary1 };
            var multipartReader = new MultipartReader(boundary1, context.Request.Body);
            var multipartSection = await multipartReader.ReadNextSectionAsync(context.RequestAborted);
            Assert.NotNull(multipartSection);

            await HttpContextForwardBuilder.CopyTextMultipartSectionAsync(multipartSection,
                httpMultipartFormDataBuilder,
                context.RequestAborted);
            Assert.Single(httpMultipartFormDataBuilder._partContents);
            Assert.Equal("1", httpMultipartFormDataBuilder._partContents[0].RawContent);
            Assert.Equal("Id", httpMultipartFormDataBuilder._partContents[0].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);

            multipartSection = await multipartReader.ReadNextSectionAsync(context.RequestAborted);
            Assert.NotNull(multipartSection);

            await HttpContextForwardBuilder.CopyTextMultipartSectionAsync(multipartSection,
                httpMultipartFormDataBuilder,
                context.RequestAborted);
            Assert.Equal(2, httpMultipartFormDataBuilder._partContents.Count);
            Assert.Equal("Furion", httpMultipartFormDataBuilder._partContents[1].RawContent);
            Assert.Equal("Name", httpMultipartFormDataBuilder._partContents[1].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);

            await context.Response.WriteAsync("Hello World!");
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

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyFileMultipartSection_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("433", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            var boundary1 = context.Request.ContentType!.Split('=')[1];
            var httpMultipartFormDataBuilder =
                new HttpMultipartFormDataBuilder(httpRequestBuilder) { Boundary = boundary1 };
            var multipartReader = new MultipartReader(boundary1, context.Request.Body);
            var multipartSection = await multipartReader.ReadNextSectionAsync(context.RequestAborted);
            Assert.NotNull(multipartSection);

            await HttpContextForwardBuilder.CopyTextMultipartSectionAsync(multipartSection,
                httpMultipartFormDataBuilder,
                context.RequestAborted);
            Assert.Single(httpMultipartFormDataBuilder._partContents);
            Assert.Equal("1", httpMultipartFormDataBuilder._partContents[0].RawContent);
            Assert.Equal("Id", httpMultipartFormDataBuilder._partContents[0].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);

            multipartSection = await multipartReader.ReadNextSectionAsync(context.RequestAborted);
            Assert.NotNull(multipartSection);

            await HttpContextForwardBuilder.CopyTextMultipartSectionAsync(multipartSection,
                httpMultipartFormDataBuilder,
                context.RequestAborted);
            Assert.Equal(2, httpMultipartFormDataBuilder._partContents.Count);
            Assert.Equal("Furion", httpMultipartFormDataBuilder._partContents[1].RawContent);
            Assert.Equal("Name", httpMultipartFormDataBuilder._partContents[1].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);

            multipartSection = await multipartReader.ReadNextSectionAsync(context.RequestAborted);
            Assert.NotNull(multipartSection);

            await HttpContextForwardBuilder.CopyFileMultipartSectionAsync(multipartSection.AsFileSection()!,
                httpMultipartFormDataBuilder, context.RequestAborted);
            Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
            Assert.True(httpMultipartFormDataBuilder._partContents[2].RawContent is Stream);
            Assert.Equal("File", httpMultipartFormDataBuilder._partContents[2].Name);
            Assert.Equal("test.txt", httpMultipartFormDataBuilder._partContents[2].FileName);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[2].ContentType);
            Assert.NotNull(httpRequestBuilder.Disposables);
            Assert.Single(httpRequestBuilder.Disposables);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Single());

            await context.Response.WriteAsync("Hello World!");
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

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyMultipartFormDataAsync_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("433", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            await HttpContextForwardBuilder.CopyMultipartFormDataAsync(context.Request.Body,
                context.Request.ContentType!, httpRequestBuilder, context.RequestAborted);

            Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
            var httpMultipartFormDataBuilder = httpRequestBuilder.MultipartFormDataBuilder;

            Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
            Assert.Equal("1", httpMultipartFormDataBuilder._partContents[0].RawContent);
            Assert.Equal("Id", httpMultipartFormDataBuilder._partContents[0].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);

            Assert.Equal("Furion", httpMultipartFormDataBuilder._partContents[1].RawContent);
            Assert.Equal("Name", httpMultipartFormDataBuilder._partContents[1].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);

            Assert.True(httpMultipartFormDataBuilder._partContents[2].RawContent is Stream);
            Assert.Equal("File", httpMultipartFormDataBuilder._partContents[2].Name);
            Assert.Equal("test.txt", httpMultipartFormDataBuilder._partContents[2].FileName);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[2].ContentType);
            Assert.NotNull(httpRequestBuilder.Disposables);
            Assert.Single(httpRequestBuilder.Disposables);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Single());

            await context.Response.WriteAsync("Hello World!");
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

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyBodyAsync_NotContent_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            await httpContextForwardBuilder.CopyBodyAsync(httpRequestBuilder);
            Assert.Null(httpRequestBuilder.RawContent);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyBodyAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal("application/json", httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("24", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            await httpContextForwardBuilder.CopyBodyAsync(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CopyBodyAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            httpContextForwardBuilder.CopyHeaders(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("433", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            await httpContextForwardBuilder.CopyBodyAsync(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
            var httpMultipartFormDataBuilder = httpRequestBuilder.MultipartFormDataBuilder;

            Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
            Assert.Equal("1", httpMultipartFormDataBuilder._partContents[0].RawContent);
            Assert.Equal("Id", httpMultipartFormDataBuilder._partContents[0].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);

            Assert.Equal("Furion", httpMultipartFormDataBuilder._partContents[1].RawContent);
            Assert.Equal("Name", httpMultipartFormDataBuilder._partContents[1].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);

            Assert.True(httpMultipartFormDataBuilder._partContents[2].RawContent is Stream);
            Assert.Equal("File", httpMultipartFormDataBuilder._partContents[2].Name);
            Assert.Equal("test.txt", httpMultipartFormDataBuilder._partContents[2].FileName);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[2].ContentType);
            Assert.NotNull(httpRequestBuilder.Disposables);
            Assert.Equal(2, httpRequestBuilder.Disposables.Count);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Last());

            await context.Response.WriteAsync("Hello World!");
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

        await app.StopAsync();
    }

    [Fact]
    public async Task BuildAsync_NotContent_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync();
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            Assert.False(httpRequestBuilder0.AutoSetHostHeaderEnabled);
            Assert.False(httpRequestBuilder0.EnsureSuccessStatusCodeEnabled);
            Assert.NotNull(httpRequestBuilder0.HttpContentConverterProviders);
            Assert.Single(httpRequestBuilder0.HttpContentConverterProviders);

            var httpRequestBuilder =
                await httpContextForwardBuilder.BuildAsync(u => u.SetTimeout(TimeSpan.FromSeconds(150)));
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            Assert.Null(httpRequestBuilder.RawContent);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task BuildAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync();
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            var httpRequestBuilder =
                await httpContextForwardBuilder.BuildAsync(u => u.SetTimeout(TimeSpan.FromSeconds(150)));
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal("application/json", httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("24", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task BuildAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync();
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            var httpRequestBuilder =
                await httpContextForwardBuilder.BuildAsync(u => u.SetTimeout(TimeSpan.FromSeconds(150)));
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("433", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
            var httpMultipartFormDataBuilder = httpRequestBuilder.MultipartFormDataBuilder;

            Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
            Assert.Equal("1", httpMultipartFormDataBuilder._partContents[0].RawContent);
            Assert.Equal("Id", httpMultipartFormDataBuilder._partContents[0].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);

            Assert.Equal("Furion", httpMultipartFormDataBuilder._partContents[1].RawContent);
            Assert.Equal("Name", httpMultipartFormDataBuilder._partContents[1].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);

            Assert.True(httpMultipartFormDataBuilder._partContents[2].RawContent is Stream);
            Assert.Equal("File", httpMultipartFormDataBuilder._partContents[2].Name);
            Assert.Equal("test.txt", httpMultipartFormDataBuilder._partContents[2].FileName);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[2].ContentType);
            Assert.NotNull(httpRequestBuilder.Disposables);
            Assert.Equal(2, httpRequestBuilder.Disposables.Count);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Last());

            await context.Response.WriteAsync("Hello World!");
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

        await app.StopAsync();
    }

    [Fact]
    public async Task Build_NotContent_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder0 = httpContextForwardBuilder.Build();
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            Assert.False(httpRequestBuilder0.AutoSetHostHeaderEnabled);
            Assert.False(httpRequestBuilder0.EnsureSuccessStatusCodeEnabled);
            Assert.NotNull(httpRequestBuilder0.HttpContentConverterProviders);
            Assert.Single(httpRequestBuilder0.HttpContentConverterProviders);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder =
                httpContextForwardBuilder.Build(u => u.SetTimeout(TimeSpan.FromSeconds(150)));
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            Assert.Null(httpRequestBuilder.RawContent);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task Build_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder0 = httpContextForwardBuilder.Build();
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder =
                httpContextForwardBuilder.Build(u => u.SetTimeout(TimeSpan.FromSeconds(150)));
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal("application/json", httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("24", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task Build_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder0 = httpContextForwardBuilder.Build();
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder =
                httpContextForwardBuilder.Build(u => u.SetTimeout(TimeSpan.FromSeconds(150)));
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(3, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.Headers.ElementAt(1).Value.First());
            Assert.Equal("Content-Length", httpRequestBuilder.Headers.ElementAt(2).Key);
            Assert.Equal("433", httpRequestBuilder.Headers.ElementAt(2).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(3).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(3).Value.First());

            Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
            var httpMultipartFormDataBuilder = httpRequestBuilder.MultipartFormDataBuilder;

            Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
            Assert.Equal("1", httpMultipartFormDataBuilder._partContents[0].RawContent);
            Assert.Equal("Id", httpMultipartFormDataBuilder._partContents[0].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);

            Assert.Equal("Furion", httpMultipartFormDataBuilder._partContents[1].RawContent);
            Assert.Equal("Name", httpMultipartFormDataBuilder._partContents[1].Name);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);

            Assert.True(httpMultipartFormDataBuilder._partContents[2].RawContent is Stream);
            Assert.Equal("File", httpMultipartFormDataBuilder._partContents[2].Name);
            Assert.Equal("test.txt", httpMultipartFormDataBuilder._partContents[2].FileName);
            Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[2].ContentType);
            Assert.NotNull(httpRequestBuilder.Disposables);
            Assert.Equal(2, httpRequestBuilder.Disposables.Count);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Last());

            await context.Response.WriteAsync("Hello World!");
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

        await app.StopAsync();
    }

    [Fact]
    public async Task ReadBodyAsync_Invalid_Parameters()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            try
            {
                await httpContextForwardBuilder.ReadBodyAsync(httpRequestBuilder);
            }
            catch (Exception e)
            {
                if (e.Message != "Please ensure that the `app.UseEnableBuffering()` middleware is registered.")
                {
                    throw;
                }
            }

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task ReadBodyAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(context, httpMethod, requestUri);
            var httpRequestBuilder = HttpRequestBuilder.Create(httpMethod, requestUri);

            await httpContextForwardBuilder.ReadBodyAsync(httpRequestBuilder);

            Assert.NotNull(httpRequestBuilder.Disposables);
            Assert.Single(httpRequestBuilder.Disposables);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }
}