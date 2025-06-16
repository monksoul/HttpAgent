// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class Helpers
{
    internal static (HttpRemoteService, ServiceProvider) CreateHttpRemoteService(
        CustomRequestEventHandler? requestEventHandler = null,
        CustomFileTransferEventHandler? fileTransferEventHandler = null,
        CustomServerSentEventsEventHandler? sentEventsEventHandler = null,
        CustomLongPollingEventHandler? longPollingEventHandler = null,
        bool allowAutoRedirect = true, bool frameworkAllowAutoRedirect = true)
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty)
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { AllowAutoRedirect = allowAutoRedirect });
        services.AddHttpClient("test", client =>
        {
            client.BaseAddress = new Uri("http://localhost/test/");
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { AllowAutoRedirect = allowAutoRedirect });

        services.AddOptions<HttpRemoteOptions>().Configure(remoteOptions =>
        {
            remoteOptions.AllowAutoRedirect = frameworkAllowAutoRedirect;
        });

        if (requestEventHandler is not null)
        {
            services.AddTransient(sp => requestEventHandler);
        }

        if (fileTransferEventHandler is not null)
        {
            services.AddTransient(sp => fileTransferEventHandler);
        }

        if (sentEventsEventHandler is not null)
        {
            services.AddTransient(sp => sentEventsEventHandler);
        }

        if (longPollingEventHandler is not null)
        {
            services.AddTransient(sp => longPollingEventHandler);
        }

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>();
        var logger = serviceProvider.GetRequiredService<ILogger<Logging>>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        var httpContentProcessorFactory = new HttpContentProcessorFactory(serviceProvider, null);
        var httpRemoteService = new HttpRemoteService(serviceProvider, logger, httpClientFactory,
            httpContentProcessorFactory,
            new HttpContentConverterFactory(serviceProvider,
                [new ClayContentConverter(), new DynamicContentConverter()]), options);

        return (httpRemoteService, serviceProvider);
    }
}