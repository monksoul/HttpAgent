// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileTypeMapperTests
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new FileTypeMapper(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var fileTypeMapper = new FileTypeMapper();
        Assert.Equal(391, fileTypeMapper.Mappings.Count);

        var fileTypeMapper2 = new FileTypeMapper(new Dictionary<string, string> { { ".jpg", "image/jpeg" } });
        Assert.Single(fileTypeMapper2.Mappings);
    }

    [Fact]
    public void TryGetContentType_ReturnOK()
    {
        var fileTypeMapper = new FileTypeMapper();
        Assert.True(fileTypeMapper.TryGetContentType(@"C:\Workspaces\httptest.jpg", out var contentType));
        Assert.Equal("image/jpeg", contentType);

        Assert.False(fileTypeMapper.TryGetContentType(@"C:\Workspaces\index.php", out var contentType2));
        Assert.Null(contentType2);
    }

    [Fact]
    public void GetContentType_ReturnOK()
    {
        Assert.Equal("image/jpeg", FileTypeMapper.GetContentType(@"C:\Workspaces\httptest.jpg"));
        Assert.Equal("application/octet-stream", FileTypeMapper.GetContentType(@"C:\Workspaces\index.php"));
        Assert.Equal("application/x-httpd-php",
            FileTypeMapper.GetContentType(@"C:\Workspaces\index.php", "application/x-httpd-php"));
    }

    [Fact]
    public void GetExtension_ReturnOK()
    {
        Assert.Null(FileTypeMapper.GetExtension(null!));
        Assert.Null(FileTypeMapper.GetExtension(string.Empty));
        Assert.Null(FileTypeMapper.GetExtension(" "));

        Assert.Equal(".jpg", FileTypeMapper.GetExtension(@"C:\Workspaces\httptest.jpg"));
    }
}