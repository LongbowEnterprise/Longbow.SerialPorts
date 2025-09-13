// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.SerialPorts;

/// <summary>
/// ISerialPortFactory Interface
/// </summary>
public interface ISerialPortFactory : IAsyncDisposable
{
    /// <summary>
    /// 生成 <see cref="ISerialPortClient"/> 实例方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
    ISerialPortClient GetOrCreate(string? name = null, Action<SerialPortOptions>? valueFactory = null);

    /// <summary>
    /// 移除指定名称 <see cref="ISerialPortClient"/> 实例方法
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISerialPortClient? Remove(string name);
}
