// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ServerSentEventsDataTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var serverSentEventsData = new ServerSentEventsData();

        Assert.NotNull(serverSentEventsData._dataBuffer);
        Assert.Equal(0, serverSentEventsData._dataBuffer.Length);
        Assert.Null(serverSentEventsData._cachedData);
        Assert.NotNull(serverSentEventsData._customFields);
        Assert.Empty(serverSentEventsData._customFields);
        Assert.Null(serverSentEventsData.Event);
        Assert.NotNull(serverSentEventsData.Data);
        Assert.Empty(serverSentEventsData.Data);
        Assert.Null(serverSentEventsData.Id);
        Assert.NotNull(serverSentEventsData.CustomFields);
        Assert.Empty(serverSentEventsData.CustomFields);
    }

    [Fact]
    public void AppendData_ReturnOK()
    {
        var serverSentEventsData = new ServerSentEventsData();
        serverSentEventsData.AppendData(null);
        Assert.Null(serverSentEventsData._cachedData);
        Assert.Equal(string.Empty, serverSentEventsData.Data);
        Assert.NotNull(serverSentEventsData._cachedData);

        serverSentEventsData.AppendData("furion");
        Assert.Null(serverSentEventsData._cachedData);
        Assert.Equal("furion", serverSentEventsData.Data);
        Assert.NotNull(serverSentEventsData._cachedData);
    }

    [Fact]
    public void AddCustomField_ReturnOK()
    {
        var serverSentEventsData = new ServerSentEventsData();
        serverSentEventsData.AddCustomField("custom", "这是一条自定义数据");
        serverSentEventsData.AddCustomField("custom", "这是一条自定义数据2");
        Assert.Equal(2, serverSentEventsData._customFields.Count);
        Assert.Equal(2, serverSentEventsData.CustomFields.Count);
        Assert.Equal("这是一条自定义数据", serverSentEventsData.CustomFields.First().Value);
        Assert.Equal("这是一条自定义数据2", serverSentEventsData.CustomFields.Last().Value);
    }
}