// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RateLimitedStreamTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new RateLimitedStream(null!, 0));
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new RateLimitedStream(new MemoryStream(), -1));
        Assert.Equal("The bytes per second must be greater than zero. (Parameter 'bytesPerSecond')", exception.Message);

        var exception2 = Assert.Throws<ArgumentOutOfRangeException>(() => new RateLimitedStream(new MemoryStream(), 0));
        Assert.Equal("The bytes per second must be greater than zero. (Parameter 'bytesPerSecond')",
            exception2.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        Assert.Equal(4096, RateLimitedStream.CHUNK_SIZE);
        Assert.Equal(5, rateLimitedStream._bytesPerSecond);
        Assert.Equal(fileStream, rateLimitedStream._innerStream);
        Assert.NotNull(rateLimitedStream._lockObject);
        Assert.NotNull(rateLimitedStream._stopwatch);
        Assert.Equal(5, rateLimitedStream._availableTokens);
        Assert.Equal(0, rateLimitedStream._lastTokenRefillTime);

        Assert.Equal(fileStream.CanRead, rateLimitedStream.CanRead);
        Assert.Equal(fileStream.CanSeek, rateLimitedStream.CanSeek);
        Assert.Equal(fileStream.CanWrite, rateLimitedStream.CanWrite);
        Assert.Equal(fileStream.CanTimeout, rateLimitedStream.CanTimeout);
        Assert.Equal(21, rateLimitedStream.Length);
        Assert.Equal(fileStream.Position, rateLimitedStream.Position);
    }

    [Fact]
    public void Position_Set_ReturnOK()
    {
        var testData = new byte[100];
        using var memoryStream = new MemoryStream(testData);
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        rateLimitedStream.Position = 50;

        Assert.Equal(50, rateLimitedStream.Position);
        Assert.Equal(50, memoryStream.Position);
    }

    [Fact]
    public void Flush_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.Flush();
    }

    [Fact]
    public async Task FlushAsync_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        await using var fileStream = File.OpenRead(filePath);
        await using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        await rateLimitedStream.FlushAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public void Seek_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        var position = rateLimitedStream.Seek(0, SeekOrigin.Begin);

        Assert.Equal(0, position);
    }

    [Fact]
    public void SetLength_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test6.txt");
        using var fileStream = File.OpenWrite(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.SetLength(21);

        Assert.Equal(21, rateLimitedStream.Length);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileStream = File.OpenRead(filePath);
        var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.Dispose();

        Assert.False(fileStream.CanRead);
    }

    [Fact]
    public void Read_ReturnOK()
    {
        var testData = new byte[8192];
        using var memoryStream = new MemoryStream(testData);
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);
        var buffer = new byte[4096];
        var totalBytesRead = 0;

        while (totalBytesRead < testData.Length)
        {
            var bytesRead =
                rateLimitedStream.Read(buffer, 0, Math.Min(buffer.Length, testData.Length - totalBytesRead));
            totalBytesRead += bytesRead;
            Assert.InRange(bytesRead, 0, 4096);

            if (totalBytesRead < testData.Length)
            {
                Thread.Sleep(1000);
            }
        }

        Assert.Equal(testData.Length, totalBytesRead);
    }

    [Fact]
    public void Read_ZeroCount_ReturnOK()
    {
        using var memoryStream = new MemoryStream(new byte[100]);
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        var bytesRead = rateLimitedStream.Read(new byte[10], 0, 0);

        Assert.Equal(0, bytesRead);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        var testData = new byte[8192];
        using var memoryStream = new MemoryStream(testData);
        await using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);
        var buffer = new byte[4096];
        var totalBytesRead = 0;

        while (totalBytesRead < testData.Length)
        {
            var bytesRead = await rateLimitedStream.ReadAsync(buffer, 0,
                Math.Min(buffer.Length, testData.Length - totalBytesRead), TestContext.Current.CancellationToken);
            totalBytesRead += bytesRead;
            Assert.InRange(bytesRead, 0, 4096);

            if (totalBytesRead < testData.Length)
            {
                await Task.Delay(1000, TestContext.Current.CancellationToken);
            }
        }

        Assert.Equal(testData.Length, totalBytesRead);
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var testData = new byte[8192];
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        rateLimitedStream.Write(testData, 0, testData.Length / 2);

        Thread.Sleep(1000);

        rateLimitedStream.Write(testData, testData.Length / 2, testData.Length / 2);

        Assert.Equal(testData.Length, memoryStream.Length);
    }

    [Fact]
    public void Write_LargerThanChunk_ReturnOK()
    {
        var testData = new byte[10000];
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 10240);

        rateLimitedStream.Write(testData, 0, testData.Length);

        Assert.Equal(testData.Length, memoryStream.Length);
    }

    [Fact]
    public async Task WriteAsync_ReturnOK()
    {
        var testData = new byte[8192];
        using var memoryStream = new MemoryStream();
        await using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        await rateLimitedStream.WriteAsync(testData, 0, testData.Length / 2, TestContext.Current.CancellationToken);

        await Task.Delay(1000, TestContext.Current.CancellationToken);

        await rateLimitedStream.WriteAsync(testData, testData.Length / 2, testData.Length / 2,
            TestContext.Current.CancellationToken);

        Assert.Equal(testData.Length, memoryStream.Length);
    }

    [Fact]
    public async Task WriteAsync_LargerThanChunk_ReturnOK()
    {
        var testData = new byte[10000];
        using var memoryStream = new MemoryStream();
        await using var rateLimitedStream = new RateLimitedStream(memoryStream, 10240);

        await rateLimitedStream.WriteAsync(testData, 0, testData.Length, TestContext.Current.CancellationToken);

        Assert.Equal(testData.Length, memoryStream.Length);
    }

    [Fact]
    public void RefillTokens_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);

        rateLimitedStream._availableTokens = 0;
        Thread.Sleep(100);
        rateLimitedStream.RefillTokens();

        Assert.True(rateLimitedStream._availableTokens > 0);
        Assert.True(rateLimitedStream._availableTokens <= 1024);
    }

    [Fact]
    public void RefillTokens_NoTimePassed_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);

        rateLimitedStream._availableTokens = 0;
        rateLimitedStream._lastTokenRefillTime = rateLimitedStream._stopwatch.ElapsedMilliseconds;
        rateLimitedStream.RefillTokens();

        Assert.Equal(0, rateLimitedStream._availableTokens);
    }

    [Fact]
    public void RefillTokens_CapAtMax_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);

        rateLimitedStream._availableTokens = 1024;
        Thread.Sleep(100);
        rateLimitedStream.RefillTokens();

        Assert.Equal(1024, rateLimitedStream._availableTokens);
    }

    [Fact]
    public void WaitForTokens_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);

        rateLimitedStream._availableTokens = 512;
        rateLimitedStream.WaitForTokens(256);

        Assert.Equal(256, rateLimitedStream._availableTokens);
    }

    [Fact]
    public void WaitForTokens_InsufficientTokens_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        rateLimitedStream._availableTokens = 0;
        rateLimitedStream._lastTokenRefillTime = rateLimitedStream._stopwatch.ElapsedMilliseconds;

        rateLimitedStream.WaitForTokens(100);

        Assert.True(rateLimitedStream._availableTokens >= 0);
    }

    [Fact]
    public async Task WaitForTokensAsync_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        await using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);

        rateLimitedStream._availableTokens = 512;
        await rateLimitedStream.WaitForTokensAsync(256, TestContext.Current.CancellationToken);

        Assert.Equal(256, (int)rateLimitedStream._availableTokens);
    }

    [Fact]
    public async Task WaitForTokensAsync_InsufficientTokens_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        await using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        rateLimitedStream._availableTokens = 0;
        rateLimitedStream._lastTokenRefillTime = rateLimitedStream._stopwatch.ElapsedMilliseconds;

        await rateLimitedStream.WaitForTokensAsync(100, TestContext.Current.CancellationToken);

        Assert.True(rateLimitedStream._availableTokens >= 0);
    }
}