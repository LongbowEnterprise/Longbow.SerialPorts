# Longbow.SerialPorts

`Longbow.SerialPorts` 是一个基于 .NET 的串口通信库，旨在为开发者提供简单、高效的串口数据传输解决方案。该库支持异步操作，适用于需要与串口设备进行交互的各种应用场景，如工业自动化、物联网设备通信等。

## 🚀 特性

- 异步串口通信：支持异步打开、发送、接收和关闭串口连接。
- 配置灵活：可自定义串口参数，包括波特率、数据位、校验位、停止位等。
- 跨平台支持：适用于支持 .NET 的多种操作系统环境。
- 依赖注入友好：提供 `IServiceCollection` 扩展方法，便于集成到依赖注入容器中。

## 📦 安装

你可以通过 NuGet 安装 `Longbow.SerialPorts`：

```bash
dotnet add package Longbow.SerialPorts
```

## 🛠️ 快速开始

### 1. 注册服务

在 `Startup.cs` 或 `Program.cs` 中注册服务：

```csharp
services.AddSerialPortFactory();
```

然后通过 `ISerialPortClient` 获取或创建客户端实例：

```csharp
var factory = serviceProvider.GetRequiredService<ITcpSocketFactory>();
var client = factory.GetOrCreate("com1", options => 
{
    options.PortName = "COM1";
});
```

### 打开串口

```csharp
await client.OpenAsync();
```

### 发送数据

```csharp
await client.SendAsync(Encoding.UTF8.GetBytes("Hello Serial Port"));
```

### 接收数据

```csharp
var data = await client.ReceiveAsync();
Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()));
```

### 关闭串口

```csharp
await client.CloseAsync();
```

## 配置选项

`SerialPortOptions` 类提供了丰富的串口配置选项，包括：

- `PortName`: 串口名称（如 COM1）
- `BaudRate`: 波特率
- `DataBits`: 数据位
- `Parity`: 校验位
- `StopBits`: 停止位
- `Handshake`: 流控制协议
- `ReadBufferSize` / `WriteBufferSize`: 读写缓冲区大小
- `ReadTimeout` / `WriteTimeout`: 读写超时时间

## 🤝 贡献

欢迎贡献代码和反馈意见！请参阅 [CONTRIBUTING.md](CONTRIBUTING.md) 获取更多信息。

## 📄 许可证

本项目采用 [Apache License](LICENSE)，请查看 `LICENSE` 文件以获取详细信息。

## 🔗 相关链接

- [Github 项目主页](https://github.com/LongbowEnterprise/Longbow.SerialPorts?wt.mc_id=DT-MVP-5004174)
- [NuGet 包](https://www.nuget.org/packages/Longbow.SerialPorts?wt.mc_id=DT-MVP-5004174)

## 📞 联系方式

如需联系开发者，请查看项目主页或提交问题到 [Github Issues](https://github.com/LongbowEnterprise/Longbow.SerialPorts/issues?wt.mc_id=DT-MVP-5004174)。
