﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextForwardOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var options = new HttpContextForwardOptions();
        Assert.True(options.WithStatusCode);
        Assert.True(options.WithResponseHeaders);
        Assert.True(options.WithResponseContentHeaders);
        Assert.Null(options.OnForward);

        options.WithStatusCode = false;
        options.WithResponseHeaders = false;
        options.OnForward = (_, _) => { };
    }
}