# HttpAgent

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/HttpAgent/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/HttpAgent.svg?cacheSeconds=10800)](https://www.nuget.org/packages/HttpAgent) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

HttpAgent 是一个高性能、灵活且易用的开源库，提供了全面的 HTTP 功能支持，包括文件传输、轮询、测试工具、实时通信、请求管理、Media
类型处理、MessagePack 支持、声明式请求等，并具有低资源消耗和高测试覆盖率的特点。

![HttpAgent.drawio](https://gitee.com/dotnetchina/HttpAgent/raw/master/drawio/HttpAgent.drawio.jpg "HttpAgent.drawio.jpg")

[**查看高清架构图**](https://diagram-viewer.giteeusercontent.com?repo=dotnetchina/HttpAgent&ref=master&file=drawio/HttpAgent.drawio)

## 特性

- **全面的 `HTTP` 方法支持**：涵盖 `GET`, `POST`, `PUT`, `DELETE`, `PATCH`, `HEAD`, `TRACE` 和 `OPTIONS` 方法。
- **文件传输功能**：支持文件上传和下载，并提供实时传输进度监控。
- **轮询机制**：支持标准轮询与长轮询，并允许自定义轮询间隔。
- **测试工具集**：具备压力测试、性能测试及模拟测试能力，并能生成详细的测试报告；支持配置请求数量、并发数及测试迭代次数。
- **`HTTP` 代理与微服务集成**：支持 `HTTP` 代理和请求转发，适用于微服务架构中的集成开发。
- **实时通信能力**：通过 `Server-Sent Events (SSE)` 实现实时数据推送，并兼容 `WebSocket` 协议。
- **请求管理和日志审计**：内置请求拦截机制及 `HTTP` 请求日志审计功能。
- **媒体类型处理**：预设主流 `MediaType` 的处理机制，并开放自定义处理接口。
- **`MessagePack` 支持**：集成 `MessagePack` 序列化与反序列化功能，提高数据交换效率。
- **`WebService` 支持**：全面支持 `SOAP` 和 `RESTful WebService`，包括自动生成客户端代理、`WSDL` 解析、`XML/JSON` 数据绑定，简化集成与交互。
- **声明式请求**：通过简单的属性注解或接口定义来指定请求的行为。
- **响应转换灵活性**：提供常用的 `HTTP` 响应转换器，并支持自定义转换逻辑。
- **高性能与资源管理**：采用内存优化技术和连接池管理，确保低资源消耗与高性能表现。
- **架构设计**：架构设计灵活，易于使用与扩展。
- **跨平台无依赖**：支持跨平台运行，无需外部依赖。
- **高质量代码保障**：遵循高标准编码规范，拥有高达 `98%` 的单元测试与集成测试覆盖率。
- **`.NET 8+` 兼容性**：可在 `.NET 8` 及更高版本环境中部署使用。

## 安装

```powershell
dotnet add package HttpAgent
```

## 快速入门

我们在[主页](https://furion.net/docs/http-agent/)上有不少例子，这是让您入门的第一个：

1. 注册 `HttpRemote` 服务：

```cs
services.AddHttpRemote();
```

2. 获取互联网数据：

```cs
public class YourService(IHttpRemoteService httpRemoteService)
{
    public async Task<string?> GetContent()
    {
        return await httpRemoteService.GetAsStringAsync("https://furion.net/");
    }
}
```

[更多文档](https://furion.net/docs/http-agent/)

## 文档

您可以在[主页](https://furion.net/docs/http-agent/)找到 HttpAgent 文档。

## 贡献

该存储库的主要目的是继续发展 HttpAgent 核心，使其更快、更易于使用。HttpAgent
的开发在 [Gitee](https://gitee.com/dotnetchina/HttpAgent) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

HttpAgent 采用 [MIT](./LICENSE) 开源许可证。

[![](./assets/baiqian.svg)](https://baiqian.com)