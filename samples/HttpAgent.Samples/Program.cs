using System.Net;
using System.Reflection;
using HttpAgent;
using HttpAgent.AspNetCore.Extensions;
using HttpAgent.Samples;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shapeless.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var cookieContainer = new CookieContainer();
cookieContainer.Add(new Uri("https://furion.net"), new Cookie("cookieName", "cookieValue"));

// 为默认客户端启用
builder.Services.AddHttpClient(string.Empty, client => { })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        CookieContainer = cookieContainer,
        UseCookies = false,
        // 忽略 SSL 证书验证
        ServerCertificateCustomValidationCallback = HttpRemoteUtility.IgnoreSslErrors,
        SslProtocols = HttpRemoteUtility.AllSslProtocols,
        AllowAutoRedirect = false
    });
//.AddProfilerDelegatingHandler();

// 为特定客户端启用
builder.Services.AddHttpClient("furion", client => { client.BaseAddress = new Uri("https://furion"); });
//.AddProfilerDelegatingHandler();

builder.Services.TryAddTransient<AuthorizationDelegatingHandler>();

builder.Services.AddServiceDiscovery();
builder.Services.AddHttpRemote(options =>
    {
        // 注册单个 HTTP 声明式请求接口
        // options.AddHttpDeclarative<ISampleService>();

        // 扫描程序集批量注册 HTTP 声明式请求接口（推荐此方式注册）
        options.AddHttpDeclarativeFromAssemblies(Assembly.GetEntryAssembly());
        options.AddHttpDeclarativeExtractorFromAssemblies(Assembly.GetEntryAssembly());
        options.AddHttpContentConverters(() => [new ClayContentConverter()]);
    }).ConfigureOptions(options => options.JsonSerializerOptions.AddClayConverters())
    .ConfigureHttpClientDefaults(clientBuilder =>
    {
        clientBuilder.AddServiceDiscovery();
        clientBuilder.AddProfilerDelegatingHandler();
    });

var app = builder.Build();

// 启用请求正文缓存（支持 Body 重复读）
app.UseEnableBuffering();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();