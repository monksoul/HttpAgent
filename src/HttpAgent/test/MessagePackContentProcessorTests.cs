// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MessagePackContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new MessagePackContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(MessagePackContentProcessor)));

        Assert.NotNull(MessagePackContentProcessor._serializerCache);
        Assert.NotNull(MessagePackContentProcessor._messagePackSerializerLazy);
        Assert.NotNull(MessagePackContentProcessor._messagePackSerializerLazy.Value);
        Assert.NotNull(MessagePackContentProcessor.MessagePackSerializer);

        Assert.Equal(MessagePackContentProcessor._messagePackSerializerLazy.Value,
            MessagePackContentProcessor.MessagePackSerializer);
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new MessagePackContentProcessor();

        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/msgpack")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "application/msgpack")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(Array.Empty<byte>(), "Application/Msgpack")));
        Assert.True(
            processor.CanProcess(new HttpContentProcessorContext(new MessagePackModel1(), "Application/Msgpack")));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new MessagePackContentProcessor();

        Assert.Throws<TargetInvocationException>(() =>
            processor.Process(new HttpContentProcessorContext(new MessagePackModel0(), "application/msgpack")));
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var processor = new MessagePackContentProcessor();

        var messagePackContent1 = processor.Process(new HttpContentProcessorContext(null, "application/msgpack"));
        Assert.Null(messagePackContent1);

        var messagePackContent2 =
            processor.Process(new HttpContentProcessorContext(Array.Empty<byte>(), "application/msgpack"));
        Assert.NotNull(messagePackContent2);
        Assert.NotNull(messagePackContent2.ReadAsStream());
        Assert.Equal("application/msgpack", messagePackContent2.Headers.ContentType?.MediaType);
        Assert.Null(messagePackContent2.Headers.ContentType?.CharSet);

        var messagePackContent3 =
            processor.Process(new HttpContentProcessorContext(Array.Empty<byte>(), "application/msgpack",
                Encoding.UTF32));
        Assert.NotNull(messagePackContent3);
        Assert.NotNull(messagePackContent3.ReadAsStream());
        Assert.Equal("application/msgpack", messagePackContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", messagePackContent3.Headers.ContentType?.CharSet);

        var messagePackContent4 =
            processor.Process(new HttpContentProcessorContext(new MessagePackModel1(), "application/msgpack"));
        Assert.NotNull(messagePackContent4);
        Assert.NotNull(messagePackContent4.ReadAsStream());
        Assert.Equal("application/msgpack", messagePackContent4.Headers.ContentType?.MediaType);
        Assert.Null(messagePackContent4.Headers.ContentType?.CharSet);
    }

    [Fact]
    public void CreateSerializerDelegate_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => MessagePackContentProcessor.CreateSerializerDelegate(null!));

    [Fact]
    public void CreateSerializerDelegate_ReturnOK()
    {
        Assert.Throws<ArgumentNullException>(() => MessagePackContentProcessor.CreateSerializerDelegate(null!));

        var messagePackSerializerType = Type.GetType("MessagePack.MessagePackSerializer, MessagePack")!;
        var serializeMethod = messagePackSerializerType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .SingleOrDefault(u =>
                u is { Name: "Serialize", IsGenericMethod: true } && u.ReturnType == typeof(byte[]) &&
                u.GetParameters().Length == 3 &&
                u.GetGenericArguments().Length == 1)!;

        var func = MessagePackContentProcessor.CreateSerializerDelegate(serializeMethod);
        Assert.NotNull(func);
        var bytes = func(new MessagePackModel1());
        Assert.NotNull(bytes);
        Assert.False(MessagePackContentProcessor._serializerCache.IsEmpty);

        var func1 = MessagePackContentProcessor.CreateSerializerDelegate(serializeMethod);
        Assert.NotNull(func1);
        var bytes1 = func1(new MessagePackModel1());
        Assert.NotNull(bytes1);
        Assert.False(MessagePackContentProcessor._serializerCache.IsEmpty);
    }
}