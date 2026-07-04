using HttpAgent.BlazorWebAssembly.Samples;
using HttpAgent.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpRemote()
    .ConfigureOptions((options, serviceProvider) =>
    {
        var navigation = serviceProvider.GetRequiredService<NavigationManager>();
        options.FallbackBaseAddress = new Uri(navigation.BaseUri);
    }).ConfigureHttpClientDefaults(client => { client.AddProfilerDelegatingHandler(); });

await builder.Build().RunAsync();