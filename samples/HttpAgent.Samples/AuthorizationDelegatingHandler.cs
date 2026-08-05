namespace HttpAgent.Samples;

public class AuthorizationDelegatingHandler : DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 参考 SendAsync 代码

        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 添加 Bearer 身份验证
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "your token");

        // 添加 Basic 身份验证
        var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("username" + ":" + "password"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

        // 添加自定义 Schema 身份验证
        request.Headers.Authorization = new AuthenticationHeaderValue("X-Token", "your token");

        return base.SendAsync(request, cancellationToken);
    }
}