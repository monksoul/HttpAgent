// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteServerSentEventsExtensionsTests
{
    [Fact]
    public void ToMcpMessage_ReturnOK()
    {
        var sseData = new ServerSentEventsData();
        const string json = """{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{"limit":10}}""";
        sseData.AppendData(json);

        var result = sseData.ToMcpMessage();
        Assert.NotNull(result);
        Assert.Equal("2.0", result.JsonRpc);
        Assert.Equal("1", result.Id?.ToString());
        Assert.Equal("tools/list", result.Method);
        Assert.NotNull(result.Params);
        var paramElement = (JsonElement)result.Params;
        Assert.Equal(10, paramElement.GetProperty("limit").GetInt32());

        var sseData3 = new ServerSentEventsData();
        var result3 = sseData3.ToMcpMessage();
        Assert.Null(result3);

        var sseData4 = new ServerSentEventsData();
        const string json2 = """{"jsonrpc":"2.0","id":1,"result":{"temperature":25,"city":"Beijing"}}""";
        sseData4.AppendData(json2);
        var msg = sseData4.ToMcpMessage();

        Assert.NotNull(msg);
        var weather = msg.GetResult<WeatherResponse>();
        Assert.NotNull(weather);
        Assert.Equal(25, weather.Temperature);
        Assert.Equal("Beijing", weather.City);
    }

    [Fact]
    public void GetResult_ReturnOK()
    {
        var sseData = new ServerSentEventsData();
        const string json = """{"jsonrpc":"2.0","id":1}""";
        sseData.AppendData(json);
        var msg = sseData.ToMcpMessage();
        Assert.NotNull(msg);
        var result = msg.GetResult<WeatherResponse>();
        Assert.Null(result);

        var sseData2 = new ServerSentEventsData();
        const string json2 = """{"jsonrpc":"2.0","id":1,"result":["apple","banana"]}""";
        sseData2.AppendData(json2);
        var msg2 = sseData2.ToMcpMessage();
        Assert.NotNull(msg2);

        var fruits = msg2.GetResult<string[]>();

        Assert.NotNull(fruits);
        Assert.Equal(2, fruits.Length);
        Assert.Equal("apple", fruits[0]);
        Assert.Equal("banana", fruits[1]);
    }

    [Fact]
    public void GetData_ValidErrorData_ReturnOK()
    {
        var sseData = new ServerSentEventsData();
        const string json =
            """{"jsonrpc":"2.0","id":1,"error":{"code":-32600,"message":"Invalid Request","data":{"detail":"Missing parameter"}}}""";
        sseData.AppendData(json);
        var msg = sseData.ToMcpMessage();
        Assert.NotNull(msg);
        Assert.NotNull(msg.Error);

        var errorDetail = msg.Error.GetData<ErrorDetail>();

        Assert.NotNull(errorDetail);
        Assert.Equal("Missing parameter", errorDetail.Detail);
    }

    [Fact]
    public void GetData_ErrorDataIsNull_ReturnOK()
    {
        var sseData = new ServerSentEventsData();
        const string json =
            """{"jsonrpc":"2.0","id":1,"error":{"code":-32600,"message":"Invalid Request"}}""";
        sseData.AppendData(json);
        var msg = sseData.ToMcpMessage();
        Assert.NotNull(msg);
        Assert.NotNull(msg.Error);

        var detail = msg.Error.GetData<ErrorDetail>();

        Assert.Null(detail);
    }

    [Fact]
    public void GetData_ErrorIsNull_ReturnOK()
    {
        var sseData = new ServerSentEventsData();
        const string json = """{"jsonrpc":"2.0","id":1,"result":{}}""";
        sseData.AppendData(json);
        var msg = sseData.ToMcpMessage();
        Assert.NotNull(msg);
        Assert.Null(msg.Error);

        var detail = msg.Error?.GetData<ErrorDetail>();
        Assert.Null(detail);
    }

    private class WeatherResponse
    {
        public int Temperature { get; set; }
        public string? City { get; set; }
    }

    private class ErrorDetail
    {
        public string? Detail { get; set; }
    }
}