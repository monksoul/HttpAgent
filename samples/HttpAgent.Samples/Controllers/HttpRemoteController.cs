namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HttpRemoteController : ControllerBase
{
    [HttpPost]
    public Task<YourRemoteModel> AddModel(int query1, string query2, YourRemoteModel model)
    {
        return Task.FromResult(model);
    }

    [HttpPost]
    public Task<YourRemoteFormResult> AddForm(int id, [FromForm] YourRemoteFormModel model)
    {
        var fileInfo = model.File?.FileName;
        return Task.FromResult(new YourRemoteFormResult
        {
            FileInfo = fileInfo,
            Id = model.Id,
            Name = model.Name
        });
    }

    [HttpPost]
    public Task<YourRemoteModel> AddUrlForm([FromForm] YourRemoteModel model)
    {
        return Task.FromResult(model);
    }

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public Task<string> AddFile(IFormFile file)
    {
        return Task.FromResult(file.FileName);
    }

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public Task<string> AddFiles(IFormFileCollection files)
    {
        return Task.FromResult(string.Join("; ", files.Select(u => u.FileName)));
    }

    [HttpGet]
    public async Task<IActionResult> LongPolling([FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;

        // 空检查
        ArgumentNullException.ThrowIfNull(httpContext);

        var message = $"Message at {DateTime.UtcNow}\n\n";

        await Task.Delay(2000, httpContext.RequestAborted);

        return new ContentResult { Content = message, ContentType = "text/plain" };
    }

    [HttpGet]
    public async Task<IActionResult> Events([FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;

        // 空检查
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.ContentType = "text/event-stream";
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";
        httpContext.Response.Headers["X-Accel-Buffering"] = "no";

        // 模拟事件流
        var eventId = 0;
        while (!httpContext.RequestAborted.IsCancellationRequested)
        {
            eventId++;
            var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";

            // 发送 SSE 消息
            await httpContext.Response.WriteAsync(message);

            // 每隔两秒发送一条消息
            await Task.Delay(2000, httpContext.RequestAborted);
        }

        return new EmptyResult();
    }

    [HttpPost]
    public string RawString([FromBody] string str)
    {
        return str;
    }
}