// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Extensions.Options;

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextForwardBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpContextForwardBuilder(null!));

    [Fact]
    public void New_ReturnOK()
    {
        Assert.NotNull(HttpContextForwardBuilder._actionResultContentConverterInstance);
        Assert.NotNull(HttpContextForwardBuilder._actionResultContentConverterInstance.Value);

        var builder = new HttpContextForwardBuilder(HttpMethod.Get);
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpContextForwardBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, builder2.HttpMethod);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
    }

    [Fact]
    public void IgnoreRequestHeaders_ReturnOK() =>
        Assert.Equal([Constants.X_FORWARD_TO_HEADER, "Host", "Content-Length"],
            HttpContextForwardBuilder._ignoreRequestHeaders);

    [Fact]
    public void ResolveTargetUri_Invalid_Parameters()
    {
        var httpContext = new DefaultHttpContext();
        var forwardOptions = new HttpContextForwardOptions();

        var builder = new HttpContextForwardBuilder(HttpMethod.Get, new Uri("https://furion.net"));

        var exception =
            Assert.Throws<InvalidOperationException>(() => builder.ResolveTargetUri(httpContext, forwardOptions));
        Assert.Equal(
            "No allowed hosts have been configured for request forwarding. To enable forwarding, add target hosts to HttpContextForwardOptions.AllowedHosts, or include `*` to allow all hosts (not recommended due to SSRF risk).",
            exception.Message);

        forwardOptions.AllowedHosts = ["baiqian.com"];
        var exception2 = Assert.Throws<InvalidOperationException>(() =>
            builder.ResolveTargetUri(httpContext, forwardOptions));
        Assert.Equal("The target host 'furion.net:443' is not in the allowed forwarding list.", exception2.Message);

        httpContext.Request.Headers[Constants.X_FORWARD_TO_HEADER] = "/api";
        var builder2 = new HttpContextForwardBuilder(HttpMethod.Get);

        var exception3 =
            Assert.Throws<InvalidOperationException>(() => builder2.ResolveTargetUri(httpContext, forwardOptions));
        Assert.Equal(
            "The target URL must be an absolute URI (e.g., https://baiqian.com/about). Relative paths are not supported.",
            exception3.Message);

        httpContext.Request.Headers[Constants.X_FORWARD_TO_HEADER] = "https://furion.net";

        var exception4 =
            Assert.Throws<InvalidOperationException>(() =>
                builder2.ResolveTargetUri(httpContext, new HttpContextForwardOptions()));
        Assert.Equal(
            "No allowed hosts have been configured for request forwarding. To enable forwarding, add target hosts to HttpContextForwardOptions.AllowedHosts, or include `*` to allow all hosts (not recommended due to SSRF risk).",
            exception4.Message);

        var exception5 =
            Assert.Throws<InvalidOperationException>(() =>
                builder2.ResolveTargetUri(httpContext,
                    new HttpContextForwardOptions { AllowedHosts = ["baiqian.com"] }));
        Assert.Equal("The target host 'furion.net:443' is not in the allowed forwarding list.", exception5.Message);
    }

    [Fact]
    public void ResolveTargetUri_ReturnOK()
    {
        var httpContext = new DefaultHttpContext();
        var builder = new HttpContextForwardBuilder(HttpMethod.Get, new Uri("https://furion.net"));

        var targetUrl = builder.ResolveTargetUri(httpContext, new HttpContextForwardOptions { AllowedHosts = ["*"] });
        Assert.NotNull(targetUrl);
        Assert.Equal("https://furion.net/", targetUrl.ToString());

        var builder2 = new HttpContextForwardBuilder(HttpMethod.Get);
        httpContext.Request.Headers[Constants.X_FORWARD_TO_HEADER] = "https://furion.net";
        var optionsWithHost = new HttpContextForwardOptions { AllowedHosts = ["furion.net"] };
        var uriFromHeader = builder2.ResolveTargetUri(httpContext, optionsWithHost);
        Assert.NotNull(uriFromHeader);
        Assert.Equal("https://furion.net/", uriFromHeader.ToString());

        httpContext.Request.Headers[Constants.X_FORWARD_TO_HEADER] = "";
        var uriEmptyHeader = builder2.ResolveTargetUri(httpContext, optionsWithHost);
        Assert.Null(uriEmptyHeader);

        httpContext.Request.Headers.Remove(Constants.X_FORWARD_TO_HEADER);
        var uriNoHeader = builder2.ResolveTargetUri(httpContext, optionsWithHost);
        Assert.Null(uriNoHeader);
    }

    [Fact]
    public async Task CopyQueryAndRouteValues_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test/{id:int}",
            async (HttpContext context, [FromQuery] string name, [FromQuery] string giveup, [FromRoute] int id) =>
            {
                var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
                var requestUri = new Uri($"http://localhost:{port}");

                var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
                var forwardOptions =
                    new HttpContextForwardOptions { IgnoreQueryParameters = ["giveup"], AllowedHosts = ["*"] };

                var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
                HttpContextForwardBuilder.CopyQueryAndRouteValues(
                    new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

                Assert.NotNull(httpRequestBuilder.QueryParameters);
                Assert.Single(httpRequestBuilder.QueryParameters);
                Assert.Equal("name", httpRequestBuilder.QueryParameters.First().Key);

                Assert.NotNull(httpRequestBuilder.PathParameters);
                Assert.Equal(3, httpRequestBuilder.PathParameters.Count);
                Assert.Equal("name", httpRequestBuilder.PathParameters.ElementAt(0).Key);
                Assert.Equal("furion", httpRequestBuilder.PathParameters.ElementAt(0).Value);
                Assert.Equal("giveup", httpRequestBuilder.PathParameters.ElementAt(1).Key);
                Assert.Equal("miss", httpRequestBuilder.PathParameters.ElementAt(1).Value);
                Assert.Equal("id", httpRequestBuilder.PathParameters.ElementAt(2).Key);
                Assert.Equal("1", httpRequestBuilder.PathParameters.ElementAt(2).Value);

                await context.Response.WriteAsync("Hello World!");
            });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                    new Uri($"http://localhost:{port}/test/1?name=furion&giveup=miss")),
                TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = new HttpContextForwardOptions { WithQueryParameters = false, AllowedHosts = ["*"] };

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyQueryAndRouteValues(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.Null(httpRequestBuilder.QueryParameters);

            Assert.NotNull(httpRequestBuilder.PathParameters);
            Assert.Equal(2, httpRequestBuilder.PathParameters.Count);
            Assert.Equal("name", httpRequestBuilder.PathParameters.ElementAt(0).Key);
            Assert.Equal("furion", httpRequestBuilder.PathParameters.ElementAt(0).Value);
            Assert.Equal("id", httpRequestBuilder.PathParameters.ElementAt(1).Key);
            Assert.Equal("1", httpRequestBuilder.PathParameters.ElementAt(1).Value);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test/1?name=furion")), TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyHeaders_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")), TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = new HttpContextForwardOptions { ResetHostRequestHeader = true, AllowedHosts = ["*"] };

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.True(httpRequestBuilder.AutoSetHostHeaderEnabled);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")), TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyNonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 _) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal("application/json", httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

            HttpContextForwardBuilder.CopyNonMultipartFormData(context.Request.Body,
                new MediaTypeHeaderValue(context.Request.ContentType!).MediaType!,
                httpRequestBuilder);
            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyTextMultipartSectionAsync_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyFileMultipartSection_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

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
            // Assert.Single(httpRequestBuilder.Disposables);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Last());

            await context.Response.WriteAsync("Hello World!");
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyMultipartFormDataAsync_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(
                new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions));

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

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
            // Assert.Single(httpRequestBuilder.Disposables);
            Assert.Equal(httpMultipartFormDataBuilder._partContents[2].RawContent,
                httpRequestBuilder.Disposables.Last());

            await context.Response.WriteAsync("Hello World!");
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyBodyAsync_NotContent_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);

            var buildContext = new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(buildContext);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            await HttpContextForwardBuilder.CopyBodyAsync(buildContext);
            Assert.Null(httpRequestBuilder.RawContent);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")), TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyBodyAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 _) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);

            var buildContext = new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(buildContext);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal("application/json", httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

            await HttpContextForwardBuilder.CopyBodyAsync(buildContext);

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyBodyAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);

            var buildContext = new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(buildContext);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

            await HttpContextForwardBuilder.CopyBodyAsync(buildContext);

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
            // Assert.Single(httpRequestBuilder.Disposables);
            Assert.Equal(typeof(FileBufferingReadStream), httpRequestBuilder.Disposables.Last().GetType());

            await context.Response.WriteAsync("Hello World!");
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CopyBodyAsync_UrlEncodedFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();

        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;
            await next.Invoke();
        });

        app.MapPost("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);

            var buildContext = new HttpForwardBuildContext(context, httpRequestBuilder, forwardOptions);
            HttpContextForwardBuilder.CopyHeaders(buildContext);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal("application/x-www-form-urlencoded",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

            await HttpContextForwardBuilder.CopyBodyAsync(buildContext);

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/x-www-form-urlencoded", httpRequestBuilder.ContentType);

            var content = await ((StreamContent)httpRequestBuilder.RawContent).ReadAsStringAsync();
            Assert.Equal("Id=1&Name=Furion", content);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var formData = new Dictionary<string, string> { { "Id", "1" }, { "Name", "Furion" } };
        httpRequestMessage.Content = new FormUrlEncodedContent(formData);

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task BuildAsync_NotContent_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);

            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            Assert.False(httpRequestBuilder0.AutoSetHostHeaderEnabled);
            Assert.False(httpRequestBuilder0.EnsureSuccessStatusCodeEnabled);
            Assert.NotNull(httpRequestBuilder0.HttpContentConverterProviders);
            Assert.Single(httpRequestBuilder0.HttpContentConverterProviders);

            var httpRequestBuilder =
                await httpContextForwardBuilder.BuildAsync(context,
                    u => u.SetTimeout(TimeSpan.FromSeconds(150)), forwardOptions);
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.TimeoutOptions?.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            Assert.Null(httpRequestBuilder.RawContent);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")), TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task BuildAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 _) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);

            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            var httpRequestBuilder =
                await httpContextForwardBuilder.BuildAsync(context, u => u.SetTimeout(TimeSpan.FromSeconds(150)),
                    forwardOptions);
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.TimeoutOptions?.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal("application/json", httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task BuildAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            var httpRequestBuilder =
                await httpContextForwardBuilder.BuildAsync(context, u => u.SetTimeout(TimeSpan.FromSeconds(150)),
                    forwardOptions);
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.TimeoutOptions?.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

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
            Assert.Equal(typeof(FileBufferingReadStream), httpRequestBuilder.Disposables.Last().GetType());

            await context.Response.WriteAsync("Hello World!");
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Build_NotContent_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);
            Assert.False(httpRequestBuilder0.AutoSetHostHeaderEnabled);
            Assert.False(httpRequestBuilder0.EnsureSuccessStatusCodeEnabled);
            Assert.NotNull(httpRequestBuilder0.HttpContentConverterProviders);
            Assert.Single(httpRequestBuilder0.HttpContentConverterProviders);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder =
                httpContextForwardBuilder.Build(context, u => u.SetTimeout(TimeSpan.FromSeconds(150)), forwardOptions);
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.TimeoutOptions?.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            // Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            // Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            Assert.Null(httpRequestBuilder.RawContent);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")), TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Build_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 _) =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder =
                httpContextForwardBuilder.Build(context, u => u.SetTimeout(TimeSpan.FromSeconds(150)), forwardOptions);
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.TimeoutOptions?.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal("application/json", httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

            Assert.NotNull(httpRequestBuilder.RawContent);
            Assert.True(httpRequestBuilder.RawContent is StreamContent);
            Assert.Equal("application/json", httpRequestBuilder.ContentType);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Build_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
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
            var httpContextForwardBuilder = new HttpContextForwardBuilder(httpMethod, requestUri);
            var forwardOptions = context.RequestServices
                .GetRequiredService<IOptionsMonitor<HttpContextForwardOptions>>().CurrentValue;

            var httpRequestBuilder0 = await httpContextForwardBuilder.BuildAsync(context, null, forwardOptions);
            Assert.True(httpRequestBuilder0.DisableCacheEnabled);

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder =
                httpContextForwardBuilder.Build(context, u => u.SetTimeout(TimeSpan.FromSeconds(150)), forwardOptions);
            Assert.True(httpRequestBuilder.DisableCacheEnabled);
            Assert.Equal(TimeSpan.FromSeconds(150), httpRequestBuilder.TimeoutOptions?.Timeout);

            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Single(httpRequestBuilder.Headers);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.NotNull(httpRequestBuilder.ContentHeaders);
            Assert.Single(httpRequestBuilder.ContentHeaders);
            Assert.Equal("Content-Type", httpRequestBuilder.ContentHeaders.ElementAt(0).Key);
            Assert.Equal($"multipart/form-data; boundary=\"{boundary}\"",
                httpRequestBuilder.ContentHeaders.ElementAt(0).Value.First());

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
            Assert.Equal(typeof(FileBufferingReadStream), httpRequestBuilder.Disposables.Last().GetType());

            await context.Response.WriteAsync("Hello World!");
        }).DisableAntiforgery();

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(filePath, TestContext.Current.CancellationToken);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ReadBody_Invalid_Parameters()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 _) =>
        {
            try
            {
                HttpContextForwardBuilder.ReadBody(context);
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ReadBody_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        builder.Services.Configure<HttpContextForwardOptions>(options => { options.AllowedHosts = ["*"]; });
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 _) =>
        {
            HttpContextForwardBuilder.ReadBody(context);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "Furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public void ValidateHost_Invalid_Parameters()
    {
        var optionsNull = new HttpContextForwardOptions { AllowedHosts = null };
        var uri = new Uri("https://furion.net");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uri, optionsNull));

        var optionsEmpty = new HttpContextForwardOptions { AllowedHosts = new List<string>() };
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uri, optionsEmpty));

        var optionsSimple = new HttpContextForwardOptions { AllowedHosts = new List<string> { "furion.net" } };
        var uriEvil = new Uri("https://baiqian.com");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriEvil, optionsSimple));

        var optionsWithScheme = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "https://furion.net" }
        };
        var uriHttp = new Uri("http://furion.net");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriHttp, optionsWithScheme));

        var optionsWithPort = new HttpContextForwardOptions { AllowedHosts = new List<string> { "furion.net:8080" } };
        var uriWrongPort = new Uri("http://furion.net:9090");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriWrongPort, optionsWithPort));

        var optionsNoPort = new HttpContextForwardOptions { AllowedHosts = new List<string> { "furion.net" } };
        var uriNonDefaultPort = new Uri("http://furion.net:8080");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriNonDefaultPort, optionsNoPort));

        var optionsSpecificPort = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "furion.net:8080" }
        };
        var uriDefaultPort = new Uri("http://furion.net");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriDefaultPort, optionsSpecificPort));

        var optionsSchemeAndPort = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "https://furion.net:443" }
        };
        var uriHttp443 = new Uri("http://furion.net:443");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriHttp443, optionsSchemeAndPort));

        var optionsIpv6 = new HttpContextForwardOptions { AllowedHosts = new List<string> { "[::1]" } };
        var uriIpv6Wrong = new Uri("http://[::2]");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6Wrong, optionsIpv6));

        var optionsIpv6Port = new HttpContextForwardOptions { AllowedHosts = new List<string> { "[::1]:8080" } };
        var uriIpv6WrongPort = new Uri("http://[::1]:9090");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6WrongPort, optionsIpv6Port));

        var optionsIpv6NoPort = new HttpContextForwardOptions { AllowedHosts = new List<string> { "[::1]" } };
        var uriIpv6NonDefaultPort = new Uri("http://[::1]:8080");
        Assert.Throws<InvalidOperationException>(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6NonDefaultPort, optionsIpv6NoPort));
    }

    [Fact]
    public void ValidateHost_ReturnOK()
    {
        var optionsWildcard = new HttpContextForwardOptions { AllowedHosts = new List<string> { "*" } };
        var uriAny = new Uri("https://any-domain.com:1234");
        var exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriAny, optionsWildcard));
        Assert.Null(exception);

        var optionsHostOnly = new HttpContextForwardOptions { AllowedHosts = new List<string> { "furion.net" } };
        var uriDefault = new Uri("http://furion.net");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriDefault, optionsHostOnly));
        Assert.Null(exception);

        var uriHttpsDefault = new Uri("https://furion.net");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriHttpsDefault, optionsHostOnly));
        Assert.Null(exception);

        var optionsPortWildcard = new HttpContextForwardOptions { AllowedHosts = new List<string> { "furion.net:*" } };
        var uriCustomPort = new Uri("http://furion.net:8080");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriCustomPort, optionsPortWildcard));
        Assert.Null(exception);

        var optionsWithScheme = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "https://furion.net" }
        };
        var uriHttps = new Uri("https://furion.net");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriHttps, optionsWithScheme));
        Assert.Null(exception);

        var optionsExact = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "http://furion.net:8080" }
        };
        var uriExact = new Uri("http://furion.net:8080");
        exception = Record.Exception(() => HttpContextForwardBuilder.ValidateHost(uriExact, optionsExact));
        Assert.Null(exception);

        var optionsCase = new HttpContextForwardOptions { AllowedHosts = new List<string> { "HTTPS://Furion.NET" } };
        var uriLower = new Uri("https://furion.net");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriLower, optionsCase));
        Assert.Null(exception);

        var optionsMulti = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "a.com", "b.com:8080", "https://c.com" }
        };
        var uriMatchSecond = new Uri("http://b.com:8080");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriMatchSecond, optionsMulti));
        Assert.Null(exception);

        var optionsIpv6Default = new HttpContextForwardOptions { AllowedHosts = new List<string> { "[::1]" } };
        var uriIpv6Http = new Uri("http://[::1]");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6Http, optionsIpv6Default));
        Assert.Null(exception);

        var uriIpv6Https = new Uri("https://[::1]");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6Https, optionsIpv6Default));
        Assert.Null(exception);

        var optionsIpv6Port = new HttpContextForwardOptions { AllowedHosts = new List<string> { "[::1]:8080" } };
        var uriIpv6Port = new Uri("http://[::1]:8080");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6Port, optionsIpv6Port));
        Assert.Null(exception);

        var optionsIpv6PortWildcard = new HttpContextForwardOptions { AllowedHosts = new List<string> { "[::1]:*" } };
        var uriIpv6AnyPort = new Uri("http://[::1]:9999");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6AnyPort, optionsIpv6PortWildcard));
        Assert.Null(exception);

        var optionsIpv6Full = new HttpContextForwardOptions
        {
            AllowedHosts = new List<string> { "http://[2001:db8::1]:8080" }
        };
        var uriIpv6Full = new Uri("http://[2001:db8::1]:8080");
        exception = Record.Exception(() =>
            HttpContextForwardBuilder.ValidateHost(uriIpv6Full, optionsIpv6Full));
        Assert.Null(exception);
    }
}