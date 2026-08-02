// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileTransferProgressTests
{
    public FileTransferProgressTests()
    {
        FileTransferProgress._activeDownloadCount = 0;
        FileTransferProgress._multiLineRegistrations.Clear();
        FileTransferProgress._multiLineTotalRows = 0;
    }

    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new FileTransferProgress(null!, 0));
        Assert.Throws<ArgumentException>(() =>
            new FileTransferProgress(string.Empty, 0));
        Assert.Throws<ArgumentException>(() => new FileTransferProgress(" ", 0));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        Assert.NotNull(fileTransferProgress.FilePath);
        Assert.Equal(@"C:\Workspaces\furion.index.html", fileTransferProgress.FilePath);
        Assert.Equal(1000L, fileTransferProgress.FileSize);

        Assert.NotNull(fileTransferProgress.FileName);
        Assert.Equal("furion.index.html", fileTransferProgress.FileName);

        Assert.Equal(double.Epsilon, FileTransferProgress._epsilon);
        Assert.Equal(0, fileTransferProgress._lastDisplayLength);
        Assert.Equal(0, fileTransferProgress.Transferred);
        Assert.Equal(0, fileTransferProgress.PercentageComplete);
        Assert.Equal(0, fileTransferProgress.TransferRate);
        Assert.Equal(TimeSpan.Zero, fileTransferProgress.TimeElapsed);
        Assert.Equal(TimeSpan.Zero, fileTransferProgress.EstimatedTimeRemaining);
        Assert.False(fileTransferProgress._hasPrintedHeader);
        Assert.Equal(2000, FileTransferProgress.SpeedCalculationWindowMs);
        Assert.NotNull(fileTransferProgress._transferHistory);
        Assert.NotNull(fileTransferProgress._historyLock);
        Assert.NotNull(FileTransferProgress._consoleLock);
        Assert.Equal(0, FileTransferProgress._activeDownloadCount);
    }

    [Fact]
    public void New_WithCustomFileName_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L, "custom_name.html");

        Assert.Equal("custom_name.html", fileTransferProgress.FileName);
        Assert.Equal(@"C:\Workspaces\furion.index.html", fileTransferProgress.FilePath);
    }

    [Fact]
    public void New_MultiLineFields_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        Assert.False(fileTransferProgress._multiLineCompleted);
        Assert.Equal(-1, fileTransferProgress._multiLineIndex);
        Assert.Equal(0, fileTransferProgress._multiLineProgressRowOffset);
        Assert.NotNull(FileTransferProgress._multiLineRegistrations);
        Assert.Empty(FileTransferProgress._multiLineRegistrations);
        Assert.Equal(0, FileTransferProgress._multiLineTotalRows);
    }

    [Fact]
    public void IncrementActiveCount_ReturnOK()
    {
        Assert.Equal(0, FileTransferProgress._activeDownloadCount);

        FileTransferProgress.IncrementActiveCount();
        Assert.Equal(1, FileTransferProgress._activeDownloadCount);

        FileTransferProgress.IncrementActiveCount();
        Assert.Equal(2, FileTransferProgress._activeDownloadCount);
    }

    [Fact]
    public void DecrementActiveCount_ReturnOK()
    {
        FileTransferProgress.IncrementActiveCount();
        FileTransferProgress.IncrementActiveCount();
        Assert.Equal(2, FileTransferProgress._activeDownloadCount);

        FileTransferProgress.DecrementActiveCount();
        Assert.Equal(1, FileTransferProgress._activeDownloadCount);

        FileTransferProgress.DecrementActiveCount();
        Assert.Equal(0, FileTransferProgress._activeDownloadCount);
    }

    [Fact]
    public void GetConsoleWidth_ReturnOK()
    {
        var width = FileTransferProgress.GetConsoleWidth();

        Assert.True(width > 0);
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        var result = FileTransferProgress.StripAnsi(fileTransferProgress.ToString());

        Assert.Contains("Transfer Progress:", result);
        Assert.Contains("furion.index.html", result);
        Assert.Contains(@"C:\Workspaces\furion.index.html", result);
        Assert.Contains("0.95MB", result);
        Assert.Contains("0.48MB", result);
        Assert.Contains("50.00%", result);
        Assert.Contains("2.38MB/s", result);
        Assert.Contains("0.20", result);
    }

    [Fact]
    public async Task ToStringAsync_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        var result = FileTransferProgress.StripAnsi(await fileTransferProgress.ToStringAsync());

        Assert.Contains("Transfer Progress:", result);
        Assert.Contains("furion.index.html", result);
        Assert.Contains(@"C:\Workspaces\furion.index.html", result);
        Assert.Contains("0.95MB", result);
        Assert.Contains("0.48MB", result);
        Assert.Contains("50.00%", result);
        Assert.Contains("2.38MB/s", result);
        Assert.Contains("0.20", result);
    }

    [Fact]
    public void ToSummaryString_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        Assert.Equal(
            @"Transferred 0.48MB of 0.95MB (50.00% complete), Speed: 2.38MB/s, Time: 0.20s, ETA: 0.20s. File: furion.index.html, Path: C:\Workspaces\furion.index.html.",
            fileTransferProgress.ToSummaryString());
    }

    [Fact]
    public void ToSummaryString_Completed_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(1000, TimeSpan.FromMilliseconds(200));

        var summary = fileTransferProgress.ToSummaryString();

        Assert.Contains("100.00% complete", summary);
        Assert.Contains("Done!", summary);
        Assert.DoesNotContain("ETA", summary);
    }

    [Fact]
    public void ToSummaryString_UnknownSize_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        var summary = fileTransferProgress.ToSummaryString();

        Assert.Contains("Unknown size", summary);
        Assert.Contains("ETA: Unknown", summary);
        Assert.DoesNotContain("Done!", summary);
    }

    [Fact]
    public async Task ToSummaryStringAsync_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        Assert.Equal(
            @"Transferred 0.48MB of 0.95MB (50.00% complete), Speed: 2.38MB/s, Time: 0.20s, ETA: 0.20s. File: furion.index.html, Path: C:\Workspaces\furion.index.html.",
            await fileTransferProgress.ToSummaryStringAsync());
    }

    [Fact]
    public void UpdateProgress_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        Assert.Equal(500, fileTransferProgress.Transferred);
        Assert.Equal(TimeSpan.FromMilliseconds(200), fileTransferProgress.TimeElapsed);
        Assert.Equal(50, fileTransferProgress.PercentageComplete);
        Assert.Equal(2500, fileTransferProgress.TransferRate);
        Assert.Equal(0.2, fileTransferProgress.EstimatedTimeRemaining.TotalSeconds);

        var fileTransferProgress2 =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress2.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        Assert.Equal(500, fileTransferProgress2.Transferred);
        Assert.Equal(TimeSpan.FromMilliseconds(200), fileTransferProgress2.TimeElapsed);
        Assert.Equal(-1, fileTransferProgress2.PercentageComplete);
        Assert.Equal(2500, fileTransferProgress2.TransferRate);
        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress2.EstimatedTimeRemaining);

        var fileTransferProgress3 =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress3.UpdateProgress(500, TimeSpan.Zero);

        Assert.Equal(500, fileTransferProgress3.Transferred);
        Assert.Equal(TimeSpan.Zero, fileTransferProgress3.TimeElapsed);
        Assert.Equal(-1, fileTransferProgress3.PercentageComplete);
        Assert.Equal(0, fileTransferProgress3.TransferRate);
        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress3.EstimatedTimeRemaining);
    }

    [Fact]
    public void UpdateProgress_TransferHistory_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 10000L);

        fileTransferProgress.UpdateProgress(1000, TimeSpan.FromMilliseconds(100));
        Assert.Single(fileTransferProgress._transferHistory);

        fileTransferProgress.UpdateProgress(2000, TimeSpan.FromMilliseconds(200));
        Assert.Equal(2, fileTransferProgress._transferHistory.Count);

        fileTransferProgress.UpdateProgress(3000, TimeSpan.FromMilliseconds(300));
        Assert.Equal(3, fileTransferProgress._transferHistory.Count);
    }

    [Fact]
    public void CalculateEstimatedTimeRemaining_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));
        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress.CalculateEstimatedTimeRemaining());

        var fileTransferProgress2 =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress2.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        Assert.Equal(0.2, fileTransferProgress2.CalculateEstimatedTimeRemaining().TotalSeconds);
    }

    [Fact]
    public void CalculateEstimatedTimeRemaining_ZeroRate_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress.CalculateEstimatedTimeRemaining());
    }

    [Fact]
    public void BuildProgressText_KnownSize_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        var text = fileTransferProgress.BuildProgressText(false, 120);

        Assert.Contains("50.00%", text);
        Assert.Contains("#", text);
        Assert.Contains(".", text);
        Assert.Contains("Speed:", text);
        Assert.Contains("ETA:", text);
        Assert.DoesNotContain("Done!", text);
    }

    [Fact]
    public void BuildProgressText_Completed_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(1000, TimeSpan.FromMilliseconds(200));

        var text = fileTransferProgress.BuildProgressText(true, 120);

        Assert.Contains("100.00%", text);
        Assert.Contains("Done!", text);
        Assert.DoesNotContain("ETA:", text);
        Assert.Equal(20, text.IndexOf(']') - text.IndexOf('[') - 1);
    }

    [Fact]
    public void BuildProgressText_UnknownSize_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        var text = fileTransferProgress.BuildProgressText(false, 120);

        Assert.Contains("Unknown size", text);
        Assert.Contains("ETA: Unknown", text);
        Assert.True(text.Contains("|") || text.Contains("/") || text.Contains("-") || text.Contains("\\"));
    }

    [Fact]
    public void BuildProgressText_Truncate_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000000L);
        fileTransferProgress.UpdateProgress(500000000, TimeSpan.FromMilliseconds(200));

        var text = fileTransferProgress.BuildProgressText(false, 40);

        Assert.True(FileTransferProgress.GetDisplayLength(text) <= 40);
        Assert.EndsWith("...", text);
    }

    [Fact]
    public void BuildProgressText_TruncateCompleted_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000000L);
        fileTransferProgress.UpdateProgress(1000000000, TimeSpan.FromMilliseconds(200));

        var text = fileTransferProgress.BuildProgressText(true, 60);

        Assert.Contains("Done!", text);
        Assert.Contains("...", text);
    }

    [Fact]
    public void UpdateConsoleProgress_SingleLine_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        fileTransferProgress.UpdateConsoleProgress();

        Assert.True(fileTransferProgress._hasPrintedHeader);
    }

    [Fact]
    public void UpdateConsoleProgress_SingleLineCompleted_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(1000, TimeSpan.FromMilliseconds(200));

        fileTransferProgress.UpdateConsoleProgress();

        Assert.False(fileTransferProgress._hasPrintedHeader);
    }

    [Fact]
    public async Task UpdateConsoleProgressAsync_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        await fileTransferProgress.UpdateConsoleProgressAsync();

        Assert.True(fileTransferProgress._hasPrintedHeader);
    }

    [Fact]
    public void UpdateConsoleProgress_MultiLine_ReturnOK()
    {
        FileTransferProgress._activeDownloadCount = 2;

        var progress1 = new FileTransferProgress(@"C:\Workspaces\file1.png", 1000L);
        var progress2 = new FileTransferProgress(@"C:\Workspaces\file2.png", 1000L);

        progress1.UpdateProgress(100, TimeSpan.FromMilliseconds(100));
        progress2.UpdateProgress(200, TimeSpan.FromMilliseconds(100));

        progress1.UpdateConsoleProgress();
        progress2.UpdateConsoleProgress();

        Assert.Equal(0, progress1._multiLineIndex);
        Assert.Equal(1, progress2._multiLineIndex);
        Assert.Equal(2, FileTransferProgress._multiLineRegistrations.Count);
        Assert.Equal(4, FileTransferProgress._multiLineTotalRows);
        Assert.Equal(1, progress1._multiLineProgressRowOffset);
        Assert.Equal(3, progress2._multiLineProgressRowOffset);
        Assert.True(progress1._hasPrintedHeader);
        Assert.True(progress2._hasPrintedHeader);
    }

    [Fact]
    public void UpdateMultiLineProgress_Register_ReturnOK()
    {
        FileTransferProgress._activeDownloadCount = 2;

        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        fileTransferProgress.UpdateMultiLineProgress(false);

        Assert.Equal(0, fileTransferProgress._multiLineIndex);
        Assert.Single(FileTransferProgress._multiLineRegistrations);
        Assert.Equal(2, FileTransferProgress._multiLineTotalRows);
        Assert.Equal(1, fileTransferProgress._multiLineProgressRowOffset);
        Assert.True(fileTransferProgress._hasPrintedHeader);
        Assert.True(fileTransferProgress._lastDisplayLength > 0);
    }

    [Fact]
    public void UpdateMultiLineProgress_Update_ReturnOK()
    {
        FileTransferProgress._activeDownloadCount = 2;

        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        fileTransferProgress.UpdateMultiLineProgress(false);
        var firstDisplayLength = fileTransferProgress._lastDisplayLength;

        fileTransferProgress.UpdateProgress(800, TimeSpan.FromMilliseconds(400));
        fileTransferProgress.UpdateMultiLineProgress(false);

        Assert.Equal(0, fileTransferProgress._multiLineIndex);
        Assert.Single(FileTransferProgress._multiLineRegistrations);
        Assert.Equal(2, FileTransferProgress._multiLineTotalRows);
        Assert.True(fileTransferProgress._lastDisplayLength > 0);
    }

    [Fact]
    public void UpdateMultiLineProgress_CompletedOnRegister_ReturnOK()
    {
        FileTransferProgress._activeDownloadCount = 2;

        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(1000, TimeSpan.FromMilliseconds(200));

        fileTransferProgress.UpdateMultiLineProgress(true);

        Assert.True(fileTransferProgress._multiLineCompleted);
    }

    [Fact]
    public void HandleMultiLineCompleted_Partial_ReturnOK()
    {
        FileTransferProgress._activeDownloadCount = 2;

        var progress1 = new FileTransferProgress(@"C:\Workspaces\file1.png", 1000L);
        var progress2 = new FileTransferProgress(@"C:\Workspaces\file2.png", 1000L);

        progress1.UpdateMultiLineProgress(false);
        progress2.UpdateMultiLineProgress(false);

        Assert.Equal(2, FileTransferProgress._multiLineRegistrations.Count);

        progress1.HandleMultiLineCompleted();

        Assert.True(progress1._multiLineCompleted);
        Assert.False(progress2._multiLineCompleted);
        Assert.Equal(2, FileTransferProgress._multiLineRegistrations.Count);
        Assert.Equal(4, FileTransferProgress._multiLineTotalRows);
    }

    [Fact]
    public void HandleMultiLineCompleted_All_ReturnOK()
    {
        FileTransferProgress._activeDownloadCount = 2;

        var progress1 = new FileTransferProgress(@"C:\Workspaces\file1.png", 1000L);
        var progress2 = new FileTransferProgress(@"C:\Workspaces\file2.png", 1000L);

        progress1.UpdateMultiLineProgress(false);
        progress2.UpdateMultiLineProgress(false);

        progress1.HandleMultiLineCompleted();
        progress2.HandleMultiLineCompleted();

        Assert.True(progress1._multiLineCompleted);
        Assert.True(progress2._multiLineCompleted);
        Assert.Empty(FileTransferProgress._multiLineRegistrations);
        Assert.Equal(0, FileTransferProgress._multiLineTotalRows);
    }

    [Fact]
    public void UpdateSingleLineProgress_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        fileTransferProgress.UpdateSingleLineProgress(false);

        Assert.True(fileTransferProgress._hasPrintedHeader);
        Assert.True(fileTransferProgress._lastDisplayLength > 0);
    }

    [Fact]
    public void UpdateSingleLineProgress_Completed_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(1000, TimeSpan.FromMilliseconds(200));

        fileTransferProgress.UpdateSingleLineProgress(false);
        Assert.True(fileTransferProgress._hasPrintedHeader);

        fileTransferProgress.UpdateSingleLineProgress(true);
        Assert.False(fileTransferProgress._hasPrintedHeader);
    }

    [Fact]
    public void FallbackWrite_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        fileTransferProgress.FallbackWrite("更新进度", false);

        Assert.True(fileTransferProgress._lastDisplayLength > 0);
    }

    [Fact]
    public void FallbackWrite_Completed_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress._hasPrintedHeader = true;

        fileTransferProgress.FallbackWrite("完成", true);

        Assert.False(fileTransferProgress._hasPrintedHeader);
    }

    [Fact]
    public void FallbackWrite_Padding_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        fileTransferProgress.FallbackWrite("这是一段比较长的文本内容用于测试填充", false);
        var firstLength = fileTransferProgress._lastDisplayLength;

        fileTransferProgress.FallbackWrite("短文本", false);

        Assert.Equal(firstLength, fileTransferProgress._lastDisplayLength);
    }

    [Fact]
    public void GetDisplayLength_ReturnOK() => Assert.True(FileTransferProgress.GetDisplayLength("更新进度") > 0);

    [Fact]
    public void GetDisplayLength_WithAnsi_ReturnOK()
    {
        var withAnsi = FileTransferProgress.GetDisplayLength("\e[32mDone!\e[0m");
        var withoutAnsi = FileTransferProgress.GetDisplayLength("Done!");

        Assert.Equal(withoutAnsi, withAnsi);
    }

    [Fact]
    public void StripAnsi_ReturnOK() => Assert.Equal("Done!", FileTransferProgress.StripAnsi("\e[32mDone!\e[0m"));

    [Fact]
    public void StripAnsi_NoAnsi_ReturnOK() => Assert.Equal("Hello", FileTransferProgress.StripAnsi("Hello"));

    [Fact]
    public void FileSize_InternalSet_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);

        Assert.Equal(-1, fileTransferProgress.FileSize);

        fileTransferProgress.FileSize = 2000L;

        Assert.Equal(2000L, fileTransferProgress.FileSize);
    }
}