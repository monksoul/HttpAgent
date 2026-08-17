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

        services.Configure<HttpRemoteOptions>(remoteOptions =>
        {
            remoteOptions.AllowAutoRedirect = frameworkAllowAutoRedirect;
            remoteOptions.PipelineHandlerTypes =
            [
                typeof(SuppressExceptionPipelineHandler),
                typeof(ResponseAssertionPipelineHandler),
                typeof(ResponseProfilerPipelineHandler),
                typeof(RequestEventPipelineHandler),
                typeof(QuotaPipelineHandler),
                typeof(RetryPipelineHandler),
                typeof(TimeoutPipelineHandler),
                typeof(TokenManagementPipelineHandler),
                typeof(AutoRedirectPipelineHandler),
                typeof(StatusCodePipelineHandler),
                typeof(ContentLengthValidationPipelineHandler),
                typeof(RequestBuilderPipelineHandler),
                typeof(ETagPipelineHandler),
                typeof(RequestProfilerPipelineHandler),
                typeof(MockPipelineHandler),
                typeof(SendCorePipelineHandler)
            ];
        });

        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        services.AddSingleton<IHttpContentConverter>(new ClayContentConverter());
        services.AddSingleton<IHttpContentConverter>(new DynamicContentConverter());

        services.TryAddSingleton<IHttpContentProcessorFactory, HttpContentProcessorFactory>();
        services.TryAddSingleton<IHttpContentConverterFactory, HttpContentConverterFactory>();

        services.TryAddSingleton<IObjectContentConverterFactory, ObjectContentConverterFactory>();

        services.TryAddSingleton<SuppressExceptionPipelineHandler>();
        services.TryAddSingleton<ResponseAssertionPipelineHandler>();
        services.TryAddSingleton<ResponseProfilerPipelineHandler>();
        services.TryAddSingleton<RequestEventPipelineHandler>();
        services.TryAddSingleton<QuotaPipelineHandler>();
        services.TryAddSingleton<RetryPipelineHandler>();
        services.TryAddSingleton<TimeoutPipelineHandler>();
        services.TryAddSingleton<TokenManagementPipelineHandler>();
        services.TryAddSingleton<AutoRedirectPipelineHandler>();
        services.TryAddSingleton<StatusCodePipelineHandler>();
        services.TryAddSingleton<ContentLengthValidationPipelineHandler>();
        services.TryAddSingleton<RequestBuilderPipelineHandler>();
        services.TryAddSingleton<ETagPipelineHandler>();
        services.TryAddSingleton<RequestProfilerPipelineHandler>();
        services.TryAddSingleton<MockPipelineHandler>();
        services.TryAddSingleton<SendCorePipelineHandler>();

        services.TryAddSingleton<IHttpAccessTokenManager, HttpAccessTokenManager>();
        services.TryAddSingleton<IHttpQuotaManager, HttpQuotaManager>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpQuotaStrategy, DailyQuotaStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpQuotaStrategy, WeeklyQuotaStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpQuotaStrategy, MonthlyQuotaStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpQuotaStrategy, LifetimeQuotaStrategy>());

        services.TryAddSingleton<IHttpETagCache, MemoryETagCache>();

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
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<HttpRemoteOptions>>();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        var httpRemoteService = new HttpRemoteService(serviceProvider, logger, httpClientFactory,
            serviceProvider.GetRequiredService<IHttpContentProcessorFactory>(),
            serviceProvider.GetRequiredService<IHttpContentConverterFactory>(), options);

        return (httpRemoteService, serviceProvider);
    }
}