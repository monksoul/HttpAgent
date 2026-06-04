// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ByteArrayContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new ByteArrayContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(ByteArrayContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new ByteArrayContentProcessor();

        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "application/octet-stream")));
        Assert.True(
            processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "application/octet-stream")));
        Assert.True(
            processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "Application/Octet-stream")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "image/jpeg")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "image/png")));
        Assert.True(
            processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "application/json")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(new ByteArrayContent([]),
            "application/octet-stream")));
        Assert.False(processor.CanProcess(new HttpContentProcessorContext(new FormUrlEncodedContent([]),
            "application/octet-stream")));
        Assert.False(
            processor.CanProcess(new HttpContentProcessorContext(new StringContent(""), "application/octet-stream")));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new ByteArrayContentProcessor();

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            processor.Process(new HttpContentProcessorContext("furion", "application/octet-stream"));
        });

        Assert.Equal("Expected a byte array, but received an object of type `System.String`.", exception.Message);
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var processor = new ByteArrayContentProcessor();

        var byteArrayContent1 = processor.Process(new HttpContentProcessorContext(null, "application/octet-stream"));
        Assert.Null(byteArrayContent1);

        var byteArrayContent2 =
            processor.Process(new HttpContentProcessorContext(Array.Empty<byte>(), "application/octet-stream"));
        Assert.NotNull(byteArrayContent2);
        Assert.NotNull(byteArrayContent2.ReadAsStream());
        Assert.Equal("application/octet-stream", byteArrayContent2.Headers.ContentType?.MediaType);
        Assert.Null(byteArrayContent2.Headers.ContentType?.CharSet);

        var byteArrayContent3 =
            processor.Process(new HttpContentProcessorContext(Array.Empty<byte>(), "application/octet-stream",
                Encoding.UTF32));
        Assert.NotNull(byteArrayContent3);
        Assert.NotNull(byteArrayContent3.ReadAsStream());
        Assert.Equal("application/octet-stream", byteArrayContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", byteArrayContent3.Headers.ContentType?.CharSet);

        var byteArrayContent4 =
            processor.Process(new HttpContentProcessorContext(new ByteArrayContent([]), "application/octet-stream"));
        Assert.NotNull(byteArrayContent4);
        Assert.NotNull(byteArrayContent4.ReadAsStream());
        Assert.Equal("application/octet-stream", byteArrayContent4.Headers.ContentType?.MediaType);
        Assert.Null(byteArrayContent4.Headers.ContentType?.CharSet);
    }
}