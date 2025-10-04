// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileTransferResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var fileTransferResult = new FileTransferResult();
        Assert.False(fileTransferResult.IsSuccess);
        Assert.Null(fileTransferResult.RequestUri);
        Assert.Null(fileTransferResult.FilePath);
        Assert.Equal(0, fileTransferResult.FileSize);
        Assert.Equal(0, fileTransferResult.ElapsedMilliseconds);
        Assert.Null(fileTransferResult.StatusCode);
    }
}