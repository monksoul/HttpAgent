// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileInfoContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new FileInfoContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(FileInfoContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new FileInfoContentProcessor();
        using var stream = new MemoryStream();

        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "application/octet-stream")));
        Assert.False(processor.CanProcess(new HttpContentProcessorContext(stream, "Application/Octet-stream")));
        Assert.True(processor.CanProcess(
            new HttpContentProcessorContext(new FileInfo(Path.Combine(AppContext.BaseDirectory, "test.txt")),
                "text/plain")));
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var processor = new FileInfoContentProcessor();

        var streamContent1 = processor.Process(new HttpContentProcessorContext(null, "application/octet-stream"));
        Assert.Null(streamContent1);

        var fileInfo = new FileInfo(Path.Combine(AppContext.BaseDirectory, "test.txt"));
        var processorContext = new HttpContentProcessorContext(fileInfo, "text/plain");

        var streamContent2 = processor.Process(processorContext);
        Assert.NotNull(streamContent2);
        Assert.NotNull(streamContent2.ReadAsStream());
        Assert.Equal("text/plain", streamContent2.Headers.ContentType?.MediaType);
        Assert.Null(streamContent2.Headers.ContentType?.CharSet);
        Assert.NotNull(streamContent2.Headers.ContentDisposition);
        Assert.Equal("test.txt", streamContent2.Headers.ContentDisposition.FileName);
        Assert.Equal("attachment", streamContent2.Headers.ContentDisposition.DispositionType);
        Assert.NotNull(processorContext.CompletionDisposables);

        var streamContent3 =
            processor.Process(new HttpContentProcessorContext(fileInfo, "application/octet-stream", Encoding.UTF32));
        Assert.NotNull(streamContent3);
        Assert.NotNull(streamContent3.ReadAsStream());
        Assert.Equal("application/octet-stream", streamContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", streamContent3.Headers.ContentType?.CharSet);
        Assert.NotNull(streamContent3.Headers.ContentDisposition);
        Assert.Equal("test.txt", streamContent3.Headers.ContentDisposition.FileName);
        Assert.Equal("attachment", streamContent3.Headers.ContentDisposition.DispositionType);

        using var stream = new MemoryStream();
        var streamContent4 =
            processor.Process(new HttpContentProcessorContext(new StreamContent(stream), "application/octet-stream"));
        Assert.NotNull(streamContent4);
        Assert.NotNull(streamContent4.ReadAsStream());
        Assert.Equal("application/octet-stream", streamContent4.Headers.ContentType?.MediaType);
        Assert.Null(streamContent4.Headers.ContentType?.CharSet);

        var streamContent5 =
            processor.Process(new HttpContentProcessorContext(fileInfo, "text/plain") { AsFormItem = true });
        Assert.NotNull(streamContent5);
        Assert.NotNull(streamContent5.ReadAsStream());
        Assert.Equal("text/plain", streamContent5.Headers.ContentType?.MediaType);
        Assert.Null(streamContent5.Headers.ContentType?.CharSet);
        Assert.NotNull(streamContent5.Headers.ContentDisposition);
        Assert.Equal("test.txt", streamContent5.Headers.ContentDisposition.FileName);
        Assert.Equal("form-data", streamContent5.Headers.ContentDisposition.DispositionType);
    }
}