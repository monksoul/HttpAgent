// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpDeclarativeExtractorContextTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeExtractorContext(null!, null!, null!));

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeExtractorContext(method!, null!, null!));
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeExtractorContext(method!, [], null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        Assert.Equal([
            typeof(Action<HttpRequestBuilder>), typeof(Action<HttpMultipartFormDataBuilder>),
            typeof(HttpCompletionOption),
            typeof(CancellationToken)
        ], HttpDeclarativeExtractorContext._frozenParameterTypes);

        var method1 = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var context1 = new HttpDeclarativeExtractorContext(method1!, [],
            new HttpDeclarativeMethodMetadata(method1!, typeof(IHttpDeclarativeTest)));
        Assert.Equal(method1, context1.Method);
        Assert.Equal([], context1.Args);
        Assert.NotNull(context1.MethodMetadata);
        Assert.Null(context1._serviceProvider);
        Assert.Empty(context1.Parameters);
        Assert.Empty(context1.UnFrozenParameters);
        Assert.NotNull(context1.MethodMetadata);
        Assert.Equal(typeof(ReadOnlyDictionary<,>), context1.Parameters.GetType().GetGenericTypeDefinition());
        Assert.Equal(typeof(ReadOnlyDictionary<,>), context1.UnFrozenParameters.GetType().GetGenericTypeDefinition());

        var method2 = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method2));
        var args2 = new object?[] { 1, "furion" };
        var context2 =
            new HttpDeclarativeExtractorContext(method2!, args2,
                new HttpDeclarativeMethodMetadata(method2!, typeof(IHttpDeclarativeTest)));
        Assert.Equal(method2, context2.Method);
        Assert.Equal(args2, context2.Args);
        Assert.Equal(2, context2.Parameters.Count);

        Assert.Equal("id", context2.Parameters.Keys.First().Name);
        Assert.Equal(1, context2.Parameters.Values.First());
        Assert.Equal("name", context2.Parameters.Keys.Last().Name);
        Assert.Equal("furion", context2.Parameters.Values.Last());

        var method3 = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl));
        var args3 = new object?[] { "https://furion.net", CancellationToken.None };
        var context3 =
            new HttpDeclarativeExtractorContext(method3!, args3,
                new HttpDeclarativeMethodMetadata(method3!, typeof(IHttpDeclarativeTest)));
        Assert.Equal(method3, context3.Method);
        Assert.Equal(args3, context3.Args);
        Assert.Equal(2, context3.Parameters.Count);
        Assert.Single(context3.UnFrozenParameters);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var method4 = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl));
        var args4 = new object?[] { "https://furion.net", CancellationToken.None };
        var context4 = new HttpDeclarativeExtractorContext(method4!, args4,
            new HttpDeclarativeMethodMetadata(method4!, typeof(IHttpDeclarativeTest)),
            serviceProvider);
        Assert.NotNull(context4._serviceProvider);
    }

    [Fact]
    public void IsFrozenParameter_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpDeclarativeExtractorContext.IsFrozenParameter(null!));

    [Fact]
    public void IsFrozenParameter_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Frozen))!;
        var parameters = method.GetParameters();

        Assert.False(HttpDeclarativeExtractorContext.IsFrozenParameter(parameters[0]));
        Assert.False(HttpDeclarativeExtractorContext.IsFrozenParameter(parameters[1]));
        Assert.True(HttpDeclarativeExtractorContext.IsFrozenParameter(parameters[2]));
        Assert.True(HttpDeclarativeExtractorContext.IsFrozenParameter(parameters[3]));
        Assert.True(HttpDeclarativeExtractorContext.IsFrozenParameter(parameters[4]));
        Assert.True(HttpDeclarativeExtractorContext.IsFrozenParameter(parameters[5]));
    }

    [Fact]
    public void IsMethodDefined_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetAttribute))!;
        var context1 = new HttpDeclarativeExtractorContext(method, [],
            new HttpDeclarativeMethodMetadata(method, typeof(IHttpDeclarativeTest)));
        Assert.True(context1.IsMethodDefined<TimeoutAttribute>(out var attribute, true));
        Assert.NotNull(attribute);

        Assert.False(context1.IsMethodDefined<ProfilerAttribute>(out var attribute2, true));
        Assert.Null(attribute2);
    }

    [Fact]
    public void GetMethodDefinedCustomAttributes_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetMethodAttributes))!;
        var context1 = new HttpDeclarativeExtractorContext(method, [],
            new HttpDeclarativeMethodMetadata(method, typeof(IHttpDeclarativeTest)));
        var attributes = context1.GetMethodDefinedCustomAttributes<PathAttribute>(true, false);
        Assert.NotNull(attributes);
        Assert.Equal(2, attributes.Length);

        var attributes2 = context1.GetMethodDefinedCustomAttributes<QueryAttribute>(true, false);
        Assert.Null(attributes2);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetMethodAttributes))!;
        var context1 = new HttpDeclarativeExtractorContext(method, [],
            new HttpDeclarativeMethodMetadata(method, typeof(IHttpDeclarativeTest)));
        Assert.Null(context1._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        context1.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(context1._serviceProvider);
    }
}