using System.ComponentModel.DataAnnotations;

namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController(
    IHttpRemoteService httpRemoteService,
    IHttpService httpService,
    IHttpContextAccessor httpContextAccessor,
    IAuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<int> AllowAutoRedirect()
    {
        var httpResponseMessage = await httpRemoteService.HeadAsync(
            "https://gitee.com/Hgui/FastTunnel/releases/download/v2.1.2/FastTunnel.Server.tar.gz");

        return (int?)httpResponseMessage?.StatusCode ?? 0;
    }

    /// <summary>
    ///     获取网站内容
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<string?> GetWebSiteContent()
    {
        var content = await httpRemoteService.GetAsStringAsync("https://furion.net");

        // 1. 构建器方式

        // 直接获取 String 类型
        var content1 = await httpRemoteService.SendAsStringAsync(HttpRequestBuilder.Get("https://furion.net"));

        // 通过泛型指定 String 类型
        var content2 = await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Get("https://furion.net"));

        // 获取 HttpRemoteResult 类型
        var result = await httpRemoteService.SendAsync<string>(HttpRequestBuilder.Get("https://furion.net"));
        var content3 = result?.Result;

        // 获取 HttpResponseMessage 类型
        var httpResponseMessage = await httpRemoteService.SendAsync(HttpRequestBuilder.Get("https://furion.net"));
        if (httpResponseMessage is not null)
        {
            var content4 = await httpResponseMessage.Content.ReadAsStringAsync();
        }

        // 2. 请求谓词方式

        // 通过泛型指定 String 类型
        var content5 = await httpRemoteService.GetAsAsync<string>("https://furion.net");

        // 获取 HttpRemoteResult 类型
        var result2 = await httpRemoteService.GetAsync<string>("https://furion.net");
        var content6 = result2?.Result;

        // 获取 HttpResponseMessage 类型
        var httpResponseMessage2 = await httpRemoteService.GetAsync("https://furion.net");
        if (httpResponseMessage2 is not null)
        {
            var content7 = await httpResponseMessage2.Content.ReadAsStringAsync();
        }

        return content;
    }

    /// <summary>
    ///     携带请求数据
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<YourRemoteModel?> PostData()
    {
        var content = await httpRemoteService.PostAsAsync<YourRemoteModel>("https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetJsonContent(new { id = 1, name = "furion" })); // 设置请求内容

        // 构建器方式
        var content2 = await httpRemoteService.SendAsAsync<YourRemoteModel>(HttpRequestBuilder
            .Post("https://localhost:7044/HttpRemote/AddModel")
            .WithQueryParameter("query1", 1) // 设置查询参数（支持单个设置）
            .WithQueryParameter("query2", "furion") // 设置查询参数（支持单个设置）
            .SetJsonContent("{\"id\":1,\"name\":\"furion\"}")); // 设置请求内容（支持直接传入 JSON 字符串）

        // 更多方式可参考 19.2.1 使用

        // 自定义 Content-Type
        var content3 = await httpRemoteService.PostAsAsync<YourRemoteModel>(
            "https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetContent(new { id = 1, name = "furion" }, "application/json")); // 设置请求内容

        // 自定义 Content-Type 支持配置 Charset
        var content4 = await httpRemoteService.PostAsAsync<YourRemoteModel>(
            "https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetContent(new { id = 1, name = "furion" }, "application/json;charset=utf-8")); // 设置请求内容

        // 自定义 Content-Type 支持配置请求编码
        var content5 = await httpRemoteService.PostAsAsync<YourRemoteModel>(
            "https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetContent(new { id = 1, name = "furion" }, "application/json;charset=utf-8",
                    Encoding.UTF8)); // 设置请求内容

        return content;
    }

    /// <summary>
    ///     Form 表单提交
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<YourRemoteFormResult?> PostForm()
    {
        var content = await httpRemoteService.PostAsAsync<YourRemoteFormResult>(
            "https://localhost:7044/HttpRemote/AddForm?id=1",
            builder => builder.SetMultipartContent(multipart => multipart // 设置表单内容
                .AddJson(new { id = 1, name = "furion" }) // 设置常规字段
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg"))); // 设置文件（支持流方式、字节数组方式、远程 URL 地址和 Base64 字符串

        // 使用构建器模式
        var content2 = await httpRemoteService.SendAsAsync<YourRemoteFormResult>(HttpRequestBuilder
            .Post("https://localhost:7044/HttpRemote/AddForm?id=1")
            .SetMultipartContent(multipart => multipart // 设置表单内容
                .AddJson(new { id = 1, name = "furion" }) // 设置常规字段
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg"))); // 设置文件（支持流方式、字节数组方式、远程 URL 地址和 Base64 字符串

        // 更多详细用法可参考第 19.2.1 节

        // 以下是一些 `Form` 表单提交的常见例子
        var content3 = await httpRemoteService.PostAsAsync<YourRemoteFormResult>(
            "https://localhost:7044/HttpRemote/AddForm?id=1",
            builder => builder.SetMultipartContent(multipart => multipart // 设置表单内容
                .AddJson(new { id = 1, name = "furion" }) // 设置常规字段
                .AddFormItem("age", "Age") // 支持设置单个值
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg") // 设置单个文件（对应表单 File 字段）
                // 支持互联网文件地址
                .AddFileFromRemote("https://furion.net/img/furionlogo.png", "files") // 设置多个文件（对应表单 Files 字段）
                // 支持读取本地文件作为字节数组
                .AddFileAsByteArray(@"C:\Workspaces\httptest.jpg", "files"))); // 设置多个文件（对应表单 Files 字段）

        return content;
    }

    /// <summary>
    ///     URL 编码表单提交
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<YourRemoteModel?> PostUrlForm()
    {
        var content = await httpRemoteService.PostAsAsync<YourRemoteModel>(
            "https://localhost:7044/HttpRemote/AddURLForm",
            builder => builder
                .SetFormUrlEncodedContent(new
                    { id = 1, name = "furion" })); // 设置 application/x-www-form-urlencoded 请求内容

        var content2 = await httpRemoteService.PostAsAsync<YourRemoteModel>(
            "https://localhost:7044/HttpRemote/AddURLForm",
            builder => builder
                .SetFormUrlEncodedContent(new { id = 1, name = "furion" },
                    useStringContent: true)); // 使用 StringContent 构建表单数据

        return content;
    }

    [HttpGet]
    public async Task Declarative()
    {
        // 获取网站内容
        var content1 = await httpService.GetWebSiteContent();

        // 携带请求数据
        var content2 = await httpService.PostData("furion", new { id = 1, name = "furion" });

        // Form 表单提交
        var content3 = await httpService.PostForm(multipart => multipart
            .AddJson(new { id = 1, name = "furion" }) // 设置常规字段
            .AddFileAsStream(@"C:\Workspaces\httptest.jpg"));

        var content4 = await httpService.PostForm2(new { id = 1, name = "furion" }, @"C:\Workspaces\httptest.jpg");

        // URL 编码表单提交
        var content5 = await httpService.PostURLForm(new { id = 1, name = "furion" });
    }

    /// <summary>
    ///     下载网络资源
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task DownloadFile()
    {
        // 从指定 URL 下载 ASP.NET Core 运行时，并保存到 C:\Workspaces\ 目录中
        // 如果未指定文件名，系统将自动从下载地址中解析出文件名，例如：aspnetcore-runtime-8.0.10-win-x64.exe
        await httpRemoteService.DownloadFileAsync(
            "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , fileExistsBehavior: FileExistsBehavior.Overwrite);

        // 打印下载进度
        await httpRemoteService.DownloadFileAsync(
            "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , async progress =>
            {
                Console.WriteLine(progress.ToSummaryString()); // 输出简要进度字符串
                await Task.CompletedTask;
            }
            , FileExistsBehavior.Overwrite);

        // 打印下载进度
        await httpRemoteService.DownloadFileAsync(
            "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , async progress =>
            {
                Console.WriteLine(progress.ToString()); // 输出带缩进进度字符串
                await Task.CompletedTask;
            }
            , FileExistsBehavior.Overwrite);

        // 使用构建器模式
        await httpRemoteService.SendAsync(HttpRequestBuilder.DownloadFile(
            "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , fileExistsBehavior: FileExistsBehavior.Overwrite));

        // 更多详细用法可参考第 19.2.1 节
    }

    /// <summary>
    ///     上传文件资源
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task UploadFile()
    {
        // 1. 使用 Form 表单方式

        // 上传单个文件
        await httpRemoteService.PostAsync("https://localhost:7044/HttpRemote/AddFile", builder => builder
            .SetMultipartContent(multipart => multipart
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg")));

        // 上传多个文件
        await httpRemoteService.PostAsync("https://localhost:7044/HttpRemote/AddFiles", builder => builder
            .SetMultipartContent(multipart => multipart
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "files")
                .AddFileFromRemote("https://furion.net/img/furionlogo.png", "files")));

        // 使用构建器模式
        await httpRemoteService.SendAsync(HttpRequestBuilder.Post("https://localhost:7044/HttpRemote/AddFile")
            .SetMultipartContent(multipart => multipart
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg")));

        // 上传文件带进度
        await httpRemoteService.UploadFileAsync("https://localhost:7044/HttpRemote/AddFile",
            @"C:\Workspaces\httptest.jpg", "file"
            , async progress =>
            {
                Console.WriteLine(progress.ToSummaryString()); // 输出简要进度字符串
                await Task.CompletedTask;
            });

        // 支持限制文件类型和大小
        await httpRemoteService.SendAsync(HttpRequestBuilder.UploadFile("https://localhost:7044/HttpRemote/AddFile",
                @"C:\Workspaces\httptest.jpg", "file"
                , async progress =>
                {
                    Console.WriteLine(progress.ToSummaryString()); // 输出简要进度字符串
                    await Task.CompletedTask;
                })
            .SetAllowedFileExtensions(".jpg;.png") // 限制只允许 jpg 和 png 类型
            .SetMaxFileSizeInBytes(5 * 1024 * 1024)); // 限制 5MB
    }

    /// <summary>
    ///     请求分析工具
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task Profiler()
    {
        // 构建器方式
        await httpRemoteService.SendAsync(HttpRequestBuilder.Get("https://furion.net").WithHeader("X-Header", "custom")
            .Profiler()); // 启用请求分析工具

        // HTTP 请求谓词方式
        await httpRemoteService.GetAsync("https://furion.net"
            , builder => builder.Profiler()); // 启用请求分析工具
    }

    /// <summary>
    ///     添加授权凭证
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task Authentication()
    {
        // 添加 JWT (JSON Web Token) 身份验证
        await httpRemoteService.SendAsync(HttpRequestBuilder.Get("http://furion.net")
            .AddJwtBearerAuthentication("your token"));

        // 添加 Basic 身份验证
        await httpRemoteService.SendAsync(HttpRequestBuilder.Get("http://furion.net")
            .AddBasicAuthentication("username", "password"));

        // 添加自定义 Schema 身份验证
        await httpRemoteService.SendAsync(HttpRequestBuilder.Get("http://furion.net")
            .AddAuthentication(new AuthenticationHeaderValue("X-Token", "your token")));
    }

    /// <summary>
    ///     设置 Cookie
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task Cookie()
    {
        await httpRemoteService.SendAsync(HttpRequestBuilder.Get("http://furion.net")
            .WithCookie("cookieName", "cookieValue") // 设置单个
            .WithCookies(new { name = "furion", author = "monksoul" })); // 设置多个
    }

    /// <summary>
    ///     压力模拟测试
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task StressTestHarness()
    {
        var stressTestHarnessResult = await httpRemoteService.StressTestHarnessAsync("https://furion.net/");
        Console.WriteLine(stressTestHarnessResult.ToString()); // 打印压力测试结果

        var stressTestHarnessResult1 = await httpRemoteService.SendAsync(HttpRequestBuilder
            .StressTestHarness("https://furion.net/")
            .SetNumberOfRequests(1000) // 设置并发请求数量
            .SetNumberOfRounds(5) // 设置压测轮次
            .SetMaxDegreeOfParallelism(500)); // 设置最大并发度

        // 在大多数情况下，只需要设置并发请求数量即可
        var stressTestHarnessResult2 = await httpRemoteService.StressTestHarnessAsync("https://furion.net/", 500);

        var stressTestHarnessResult3 =
            await httpRemoteService.SendAsync(HttpRequestBuilder.StressTestHarness("https://furion.net/", 500));
    }

    /// <summary>
    ///     长轮询
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task LongPolling(CancellationToken cancellationToken)
    {
        await httpRemoteService.LongPollingAsync("https://localhost:7044/HttpRemote/LongPolling"
            , async (responseMessage, token) =>
            {
                Console.WriteLine(await responseMessage.Content.ReadAsStringAsync(cancellationToken));
                await Task.CompletedTask;
            }, cancellationToken: cancellationToken);

        // 使用构建器模式
        //await httpRemoteService.SendAsync(HttpRequestBuilder
        //    .LongPolling("https://localhost:7044/HttpRemote/LongPolling"
        //    , async (responseMessage, token) =>
        //    {
        //        Console.WriteLine(await responseMessage.Content.ReadAsStringAsync(cancellationToken));
        //        await Task.CompletedTask;
        //    }), cancellationToken: cancellationToken);
    }

    /// <summary>
    ///     Server Sent Events
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task ServerSentEvents(CancellationToken cancellationToken)
    {
        await httpRemoteService.ServerSentEventsAsync("https://localhost:7044/HttpRemote/Events"
            // 接收到数据时的操作
            , async (data, token) =>
            {
                Console.WriteLine(data.Data.ToString());
                await Task.CompletedTask;
            }, builder => builder
                // 连接打开时操作
                .SetOnOpen(() => { Console.WriteLine("连接成功。"); })
                // 连接未打开时操作
                .SetOnError(ex => { Console.WriteLine("连接错误。" + ex.Message); }), cancellationToken);

        // 使用构建器模式
        //await httpRemoteService.SendAsync(HttpRequestBuilder
        //   .ServerSentEvents("https://localhost:7044/HttpRemote/Events"
        //   // 接收到数据时的操作
        //   , async (data, token) =>
        //   {
        //       Console.WriteLine(data.Data.ToString());
        //       await Task.CompletedTask;
        //   })
        //   // 连接打开时操作
        //   .SetOnOpen(() =>
        //   {
        //       Console.WriteLine("连接成功。");
        //   })
        //   // 连接未打开时操作
        //   .SetOnError((ex) =>
        //   {
        //       Console.WriteLine("连接错误。" + ex.Message);
        //   }), cancellationToken: cancellationToken);
    }

    /// <summary>
    ///     WebSocket
    /// </summary>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task WebSocket(CancellationToken cancellationToken)
    {
        using var webSocketClient = new WebSocketClient("wss://localhost:7044/ws");

        // 连接成功事件
        webSocketClient.Connected += (sender, s) => Console.WriteLine("连接成功");
        // 连接关闭事件
        webSocketClient.Closed += (sender, args) => Console.WriteLine("连接关闭");
        // 接收文本消息
        webSocketClient.TextReceived += (sender, s) => Console.WriteLine(s.Message);
        // 接收二进制消息
        webSocketClient.BinaryReceived += (sender, s) => Console.WriteLine(s.Message);

        // 连接服务器
        await webSocketClient.ConnectAsync(cancellationToken);

        for (var i = 0; i < 10; i++)
        {
            // 向服务器发送消息
            await webSocketClient.SendAsync($"Message at {DateTime.UtcNow}\n\n", cancellationToken: cancellationToken);

            await Task.Delay(1000, cancellationToken);
        }

        // 关闭连接
        await webSocketClient.CloseAsync(cancellationToken);
    }

    /// <summary>
    ///     转发代理到网站
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // 禁用浏览器缓存
    public Task<IActionResult?> ForwardToWebSite()
    {
        return httpContextAccessor.HttpContext.ForwardAsResultAsync("https://github.com");
    }

    /// <summary>
    ///     转发代理到图片
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // 禁用浏览器缓存
    public Task<IActionResult?> ForwardToImage()
    {
        return httpContextAccessor.HttpContext.ForwardAsResultAsync(
            "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA1u7RJI.img?w=584&h=326&m=6");
    }

    /// <summary>
    ///     转发代理到文件
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // 禁用浏览器缓存
    public Task<IActionResult?> ForwardToDownload()
    {
        return httpContextAccessor.HttpContext.ForwardAsResultAsync(
            "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            /*, forwardOptions: new HttpContextForwardOptions
            {
                OnForward = (context, message) =>
                {
                    context.Response.Headers.ContentDisposition = new StringValues("attachment; filename=abc.exe"); // 自定义返回名称
                }
            }*/);
    }

    /// <summary>
    ///     转发代理到表单
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public Task<YourRemoteFormResult?> ForwardToForm(int id, [FromForm] YourRemoteFormModel model)
    {
        return httpContextAccessor.HttpContext.ForwardAsAsync<YourRemoteFormResult>(
            "https://localhost:7044/HttpRemote/AddForm");
    }

    /// <summary>
    ///     转发代理到网站
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // 禁用浏览器缓存
    [Forward("https://github.com")]
    public Task<IActionResult?> ForwardToWebSite2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     转发代理到图片
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // 禁用浏览器缓存
    [Forward("https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA1u7RJI.img?w=584&h=326&m=6")]
    public Task<IActionResult?> ForwardToImage2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     转发代理到文件
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // 禁用浏览器缓存
    [Forward(
        "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe")]
    public Task<IActionResult?> ForwardToDownload2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     转发代理到表单
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Forward("https://localhost:7044/HttpRemote/AddForm")]
    public Task<YourRemoteFormResult?> ForwardToForm2(int id, [FromForm] YourRemoteFormModel model)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     转发代理到字符串
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Forward("https://localhost:7044/GetStart/PostRawString")]
    public Task<string> ForwardToString2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     转发代理到无返回值
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Forward("https://localhost:7044/GetStart/PostRawString")]
    public Task ForwardToVoid2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     HttpRemoteResult
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<string?> GetResult()
    {
        var result1 = await httpRemoteService.GetAsync<string>("https://furion.net/");

        // 构建器方式
        var result2 = await httpRemoteService.SendAsync<string>(HttpRequestBuilder.Get("https://furion.net/"));

        Console.WriteLine(result1?.ToString());

        return result1?.Result;
    }

    /// <summary>
    ///     原始字符串
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Task<string?> PostRawString()
    {
        return httpRemoteService.PostAsStringAsync("https://localhost:7044/HttpRemote/RawString",
            builder => builder.SetRawStringContent("Furion", "application/json"));
    }

    [HttpGet]
    public async Task CheckAuth()
    {
        var str = await authService.GetDataAsync();
        var result = await authService.LoginAsync("admin", "admin");
    }

    [HttpGet]
    public async Task AllowAutoRedirect2([FromServices] IHttpClientFactory httpClientFactory)
    {
        var result = await httpRemoteService.GetAsStringAsync(
            "https://ocr.1datatech.net/oauth/1.0/token",
            builder => builder.WithQueryParameter("a", "123", true)
                .Profiler());
    }

    [HttpGet]
    public async Task<string> DeepSeek(CancellationToken cancellationToken)
    {
        var result = await httpRemoteService.SendAsync<string>(HttpRequestBuilder
            .Post("https://api.deepseek.com/chat/completions")
            .Profiler(false) // 建议关闭请求分析工具
            .AddJwtBearerAuthentication("您的 APIKEY")
            .SetJsonContent("""
                            {
                                "model": "deepseek-chat",
                                "messages": [
                                    {"role": "system", "content": "你是一个专业的 C# 领域人才。"},
                                    {"role": "user", "content": "Furion 框架未来前景？"}
                                ],
                                "stream": false
                            }
                            """), cancellationToken);

        // 使用流变对象获取实际内容
        dynamic clay = Clay.Parse(result?.Result, ClayOptions.Flexible);
        var content = clay.choices[0].message.content;

        return content;
    }

    [HttpGet]
    public async Task<string> DeepSeek_Stream(CancellationToken cancellationToken)
    {
        await httpRemoteService.SendAsync(HttpRequestBuilder.ServerSentEvents(HttpMethod.Post,
            new Uri("https://api.deepseek.com/chat/completions")
            , async (data, token) =>
            {
                // 输出完成
                if (data.Data == "[DONE]")
                {
                    Console.WriteLine("++++++++++++ 结束 ++++++++++++");
                    return;
                }

                // 控制打字机速度
                await Task.Delay(60, token);

                // 使用流变对象获取实际内容
                dynamic clay = Clay.Parse(data.Data, ClayOptions.Flexible);
                var content = clay.choices[0].delta.content;

                Console.WriteLine(content);
            }).WithRequest(builder => builder
            .Profiler(false) // 建议关闭请求分析工具
            .AddJwtBearerAuthentication("您的 APIKEY")
            .SetJsonContent("""
                            {
                                "model": "deepseek-chat",
                                "messages": [
                                    {"role": "system", "content": "你是一个专业的 C# 领域人才。"},
                                    {"role": "user", "content": "Furion 框架的作者是谁？"}
                                ],
                                "stream": true
                            }
                            """)), cancellationToken);

        return "OK";
    }

    [HttpGet]
    public async Task DeepSeekChat([FromServices] IHttpContextAccessor httpContextAccessor, [FromQuery] string message,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext!;

        // 设置响应头，指定内容类型为 text/event-stream
        httpContext.Response.ContentType = "text/event-stream; charset=utf-8";
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";
        httpContext.Response.Headers["X-Accel-Buffering"] = "no";

        await httpRemoteService.SendAsync(HttpRequestBuilder.ServerSentEvents(HttpMethod.Post,
            new Uri("https://api.deepseek.com/chat/completions")
            , async (data, token) =>
            {
                // DeepSeek 输出完成标记
                if (data.Data == "[DONE]") return;

                // 控制打字机速度
                await Task.Delay(60, token);

                // 使用流变对象获取实际内容
                dynamic clay = Clay.Parse(data.Data, ClayOptions.Flexible);
                var content = clay.choices[0].delta.content;

                // 确保数据被立即发送到客户端
                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(content), token);
                await httpContext.Response.Body.FlushAsync(token);
            }).WithRequest(builder => builder
            .Profiler(false) // 建议关闭请求分析工具
            .AddJwtBearerAuthentication("您的 APIKEY")
            .SetJsonContent($$"""
                              {
                              "model": "deepseek-chat",
                              "messages": [
                                  {"role": "system", "content": "你是一个专业的 C# 领域人才。"},
                                  {"role": "user", "content": "{{message}}"}
                              ],
                              "stream": true
                              }
                              """)), cancellationToken);

        await httpContext.Response.CompleteAsync();
    }

    [HttpGet]
    public async Task<string?> WebService()
    {
        var result = await httpRemoteService.PostAsStringAsync("http://您的主机地址/Share/DatabaseManager.asmx",
            builder => builder.WithHeader("SOAPAction", "http://tempuri.org/GetDatabaseList")
                .SetXmlContent("""
                               <?xml version="1.0" encoding="utf-8"?>
                               <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                                 <soap:Header>
                                   <Erp7SoapHeader xmlns="http://tempuri.org/">
                                     <ID></ID>
                                   </Erp7SoapHeader>
                                 </soap:Header>
                                 <soap:Body>
                                   <GetDatabaseList xmlns="http://tempuri.org/" />
                                 </soap:Body>
                               </soap:Envelope>
                               """, Encoding.UTF8));

        var result2 = await httpRemoteService.PostAsStringAsync("http://您的主机地址/Share/DatabaseManager.asmx",
            builder => builder.SetXmlContent("""
                                             <?xml version="1.0" encoding="utf-8"?>
                                             <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                                               <soap12:Header>
                                                 <Erp7SoapHeader xmlns="http://tempuri.org/">
                                                   <ID></ID>
                                                 </Erp7SoapHeader>
                                               </soap12:Header>
                                               <soap12:Body>
                                                 <GetDatabaseList xmlns="http://tempuri.org/" />
                                               </soap12:Body>
                                             </soap12:Envelope>
                                             """, Encoding.UTF8, "application/soap+xml"));

        // 使用 XDocument 解析 XML
        var xDocument = XDocument.Parse(result!);
        // SOAP 1.1
        var bodyContent = xDocument.Descendants(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"))
            .FirstOrDefault()?.Value!;
        // SOAP 1.2
        // var bodyContent = xDocument.Descendants(XName.Get("Body", "http://www.w3.org/2003/05/soap-envelope")).FirstOrDefault()?.Value!;

        // Base64 解码
        var data = Convert.FromBase64String(bodyContent);

        // GZip 解压缩
        using var input = new MemoryStream(data);
        await using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        await gzip.CopyToAsync(output);

        var body = Encoding.UTF8.GetString(output.ToArray());

        return body;
    }

    [HttpGet]
    public async Task<string?> ServiceDiscovery()
    {
        return await httpRemoteService.SendAsStringAsync(HttpRequestBuilder.Get("docs")
            .SetHttpClientName("furion"));
    }

    [HttpGet]
    public async Task<IActionResult?> ForwardToFile2([Required] [FromQuery] string url)
    {
        return await httpContextAccessor.HttpContext.ForwardAsResultAsync(url);
    }
}