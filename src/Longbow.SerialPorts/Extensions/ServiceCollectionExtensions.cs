// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.SerialPorts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// ITcpSocketFactory 扩展方法
/// </summary>
[UnsupportedOSPlatform("browser")]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 增加 ITcpSocketFactory 服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    [UnsupportedOSPlatform("browser")]
    public static IServiceCollection AddSerialPortFactory(this IServiceCollection services)
    {
        // 添加 ISerialPortsFactory 服务
        services.TryAddSingleton<ISerialPortFactory, DefaultSerialPortFactory>();

        return services;
    }
}
