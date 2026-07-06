// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CompositeHttpContentTests
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new CompositeHttpContent(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var compositeHttpContent = new CompositeHttpContent();
        Assert.NotNull(compositeHttpContent._contents);
        Assert.Empty(compositeHttpContent._contents);
        Assert.NotNull(compositeHttpContent.Contents);
        Assert.Empty(compositeHttpContent.Contents);

        var compositeHttpContent2 = new CompositeHttpContent(new StringContent(""), new StringContent(""));
        Assert.NotNull(compositeHttpContent2._contents);
        Assert.Equal(2, compositeHttpContent2._contents.Count);
    }

    [Fact]
    public void Add_ReturnOK()
    {
        var compositeHttpContent = new CompositeHttpContent();
        compositeHttpContent.Add(null!);
        Assert.Empty(compositeHttpContent._contents);

        compositeHttpContent.Add(new StringContent(""));
        Assert.Single(compositeHttpContent._contents);

        var compositeHttpContent2 = new CompositeHttpContent();
        compositeHttpContent2.Add(new StringContent(""));

        compositeHttpContent.Add(compositeHttpContent2);
        Assert.Equal(2, compositeHttpContent._contents.Count);
    }

    [Fact]
    public void AddRange_Invalid_Parameters()
    {
        var compositeHttpContent = new CompositeHttpContent();
        Assert.Throws<ArgumentNullException>(() => compositeHttpContent.AddRange(null!));
    }

    [Fact]
    public void AddRange_ReturnOK()
    {
        var compositeHttpContent = new CompositeHttpContent();
        compositeHttpContent.AddRange();
        Assert.Empty(compositeHttpContent._contents);

        compositeHttpContent.AddRange(new StringContent(""));
        Assert.Single(compositeHttpContent._contents);

        compositeHttpContent.AddRange(new StringContent(""), new StringContent(""));
        Assert.Equal(3, compositeHttpContent._contents.Count);
    }
}