# HttpAgent

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/HttpAgent/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/HttpAgent.svg?cacheSeconds=10800)](https://www.nuget.org/packages/HttpAgent) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

HttpAgent is a high-performance, flexible, and user-friendly open-source library that provides comprehensive support for
HTTP functionalities, including file transfer, polling, testing tools, real-time communication, request management,
media type handling, MessagePack support, Declarative Requests, and more. It is characterized by low resource
consumption and high test coverage.

![HttpAgent.drawio](https://gitee.com/dotnetchina/HttpAgent/raw/master/drawio/HttpAgent.drawio.jpg "HttpAgent.drawio.jpg")

[**View High-Definition Architecture Diagram**](https://diagram-viewer.giteeusercontent.com?repo=dotnetchina/HttpAgent&ref=master&file=drawio/HttpAgent.drawio)

## Features

- **Comprehensive `HTTP` Method Support**: Covers `GET`, `POST`, `PUT`, `DELETE`, `PATCH`, `HEAD`, `TRACE` and
  `OPTIONS` methods.
- **File Transfer Functionality**: Supports file upload and download with real-time transfer progress monitoring.
- **Polling Mechanism**: Supports standard polling and long polling with customizable polling intervals.
- **Testing Toolkit**: Capable of stress testing, performance testing, and simulation testing with the ability to
  generate detailed test reports; supports configuration of the number of requests, concurrency levels, and test
  iteration counts.
- **`HTTP` Proxy and Microservice Integration**: Supports `HTTP` proxy and request forwarding, suitable for integration
  development in microservice architectures.
- **Real-Time Communication Capability**: Implements real-time data push via `Server-Sent Events (SSE)` and is
  compatible with `WebSocket` protocol.
- **Request Management and Log Auditing**: Includes built-in request interception mechanisms and `HTTP` request log
  auditing functions.
- **Media Type Handling**: Provides predefined handling mechanisms for mainstream `MediaTypes` and offers an open
  interface for custom handling.
- **Declarative Requests**: Specifying the behavior of requests through simple attribute annotations or interface
  definitions.
- **`MessagePack` Support**: Integrates `MessagePack` serialization and deserialization capabilities to enhance data
  exchange efficiency.
- **`WebService` Support**: Comprehensive support for `SOAP` and `RESTful WebService`, including automatic client proxy
  generation, `WSDL` parsing, `XML/JSON` data binding, simplifying integration and interaction.
- **Flexible Response Transformation**: Provides common `HTTP` response transformers and supports custom transformation
  logic.
- **High Performance and Resource Management**: Employs memory optimization techniques and connection pool management to
  ensure low resource consumption and high performance.
- **Architecture Design**: Flexible architecture design that is easy to use and extend.
- **Cross-Platform Independence**: Supports cross-platform operation without external dependencies.
- **High-Quality Code Assurance**: Adheres to high-standard coding practices, with unit and integration test coverage as
  high as `98%`.
- **`.NET 8+` Compatibility**: Can be deployed and used in environments running `.NET 8` and higher versions.

## Installation

```powershell
dotnet add package HttpAgent
```

## Getting Started

We have many examples on our [homepage](https://furion.net/docs/http-agent/). Here's your first one to get you started:

1. Register `HttpRemote` service:

```cs
services.AddHttpRemote();
```

2. Retrieve internet data:

```cs
public class YourService(IHttpRemoteService httpRemoteService)
{
    public async Task<string?> GetContent()
    {
        return await httpRemoteService.GetAsStringAsync("https://furion.net/");
    }
}
```

[More Documentation](https://furion.net/docs/http-agent/)

## Documentation

You can find the HttpAgent documentation on our [homepage](https://furion.net/docs/http-agent/).

## Contributing

The main purpose of this repository is to continue developing the core of HttpAgent, making it faster and easier to use.
The development of HttpAgent is publicly hosted on [Gitee](https://gitee.com/dotnetchina/HttpAgent), and we appreciate
community contributions for bug fixes and improvements.

## License

HttpAgent is released under the [MIT](./LICENSE) open source license.

[![](./assets/baiqian.svg)](https://baiqian.com)