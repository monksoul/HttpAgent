// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

[ApiController]
[Route("[controller]/[action]")]
public class HttpRemoteController : ControllerBase
{
    [HttpPost]
    public Task<HttpRemoteModel> SendJson(HttpRemoteModel model) => Task.FromResult(model);

    [HttpPost]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<HttpRemoteModel> SendFormUrlEncoded([FromForm] HttpRemoteModel model) => Task.FromResult(model);

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public Task<string> SendFile(IFormFile file) => Task.FromResult(file.FileName);

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    public Task<string> SendFile2()
    {
        var contentDisposition = ContentDispositionHeaderValue.Parse(Request.Headers.ContentDisposition!);

        return Task.FromResult(contentDisposition.FileName?.Trim('"') ?? "");
    }

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public Task<string> SendFiles(IFormFileCollection files) =>
        Task.FromResult(string.Join(';', files.Select(u => u.FileName)));

    [HttpPost]
    public Task<string> SendMultipart([FromForm] HttpRemoteMultipartModel model) =>
        Task.FromResult($"{model.Id};{model.Name};{model.File?.FileName}");

    [HttpGet]
    public IActionResult RedirectTo() => RedirectToAction("RedirectFrom", "HttpRemote");

    [HttpGet]
    public IActionResult RedirectFrom() => Content("Redirect");

    [HttpPost]
    public IActionResult GetFile() =>
        new FileStreamResult(System.IO.File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")),
            "application/octet-stream") { FileDownloadName = "test.txt" };

    [HttpPost]
    // [Consumes("text/plain")]
    public string RawString([FromBody] string str) => str;
}