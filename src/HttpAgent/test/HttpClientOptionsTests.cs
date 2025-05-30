﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpClientOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpClientOptions = new HttpClientOptions();
        Assert.NotNull(httpClientOptions);
        Assert.True(httpClientOptions.IsDefault);

        Assert.NotNull(httpClientOptions.JsonSerializerOptions);
        Assert.NotEqual(HttpRemoteOptions.JsonSerializerOptionsDefault, httpClientOptions.JsonSerializerOptions);
    }
}