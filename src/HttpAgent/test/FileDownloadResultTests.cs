// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileDownloadResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var fileDownloadResult = new FileDownloadResult();
        Assert.False(fileDownloadResult.IsSuccess);
        Assert.Null(fileDownloadResult.RequestUri);
        Assert.Null(fileDownloadResult.FilePath);
        Assert.Equal(0, fileDownloadResult.FileSize);
        Assert.Equal(0, fileDownloadResult.ElapsedMilliseconds);
        Assert.Equal(0, (int)fileDownloadResult.StatusCode);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
    }
}