// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class McpMessageDataTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new McpMessageData(null!));
        Assert.Throws<ArgumentException>(() => new McpMessageData(string.Empty));
        Assert.Throws<ArgumentException>(() => new McpMessageData(" "));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var mcpMessageData = new McpMessageData();
        Assert.Equal("2.0", mcpMessageData.JsonRpc);
        Assert.Null(mcpMessageData.Id);
        Assert.Null(mcpMessageData.Method);
        Assert.Null(mcpMessageData.Params);
        Assert.Null(mcpMessageData.Result);
        Assert.Null(mcpMessageData.Error);

        var mcpMessageData2 = new McpMessageData("tool/list", new { });
        Assert.Equal("2.0", mcpMessageData2.JsonRpc);
        Assert.Null(mcpMessageData2.Id);
        Assert.Equal("tool/list", mcpMessageData2.Method);
        Assert.NotNull(mcpMessageData2.Params);
        Assert.Null(mcpMessageData2.Result);
        Assert.Null(mcpMessageData2.Error);
    }

    [Fact]
    public void McpError_ReturnOK()
    {
        var mcpError = new McpError();
        Assert.Equal(0, mcpError.Code);
        Assert.Null(mcpError.Message);
        Assert.Null(mcpError.Data);
    }
}