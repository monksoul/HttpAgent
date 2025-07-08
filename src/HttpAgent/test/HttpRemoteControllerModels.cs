// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteModel
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}

public class HttpRemoteMultipartModel
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public IFormFile? File { get; set; }
}

public class HttpRemoteMultipartModel2
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public MultipartFile? File { get; set; }
}