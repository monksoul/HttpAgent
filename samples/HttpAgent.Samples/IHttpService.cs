namespace HttpAgent.Samples;

[Profiler]
public interface IHttpService : IHttpDeclarative
{
    // 获取网站内容
    [Get("https://furion.net")]
    Task<string> GetWebSiteContent();

    [Profiler]
    // 携带请求数据
    [Post("https://localhost:7044/HttpRemote/AddModel")]
    [Query("query1", 1)] // 设置查询参数
    Task<YourRemoteModel> PostData([Query(AliasAs = "query2")] string param,
        [Body(MediaTypeNames.Application.Json)]
        object data); // 设置查询参数并指定别名和请求内容

    [Profiler(false)]
    // Form 表单提交
    [Post("https://localhost:7044/HttpRemote/AddForm?id=1")]
    Task<YourRemoteFormResult> PostForm(Action<HttpMultipartFormDataBuilder> formBuilder);

    // Form 表单提交
    [Post("https://localhost:7044/HttpRemote/AddForm?id=1")]
    Task<YourRemoteFormResult> PostForm2([Multipart(AsFormItem = false)] object obj,
        [Multipart("file", AsFileFrom = FileSourceType.Path)]
        string filePath);

    // URL 编码表单提交
    [Post("https://localhost:7044/HttpRemote/AddURLForm")]
    Task<YourRemoteModel> PostURLForm([Body(MediaTypeNames.Application.FormUrlEncoded)] object data);
}

public interface INormalService
{
    [Get("https://baidu.com/")]
    Task<string> GetBaidu();
}

public interface IHttpChildService : INormalService, IHttpService
{
    [Get("https://baiqian.com/")]
    Task<string> GetBaiqian();
}