// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Collections.Concurrent;

namespace Longbow.SerialPorts;

sealed class DefaultSerialPortFactory : ISerialPortFactory
{
    private readonly ConcurrentDictionary<string, ISerialPortClient> _pool = new();

    public ISerialPortClient GetOrCreate(string name, Action<SerialPortOptions>? configureOptions = null) => string.IsNullOrEmpty(name)
        ? CreateClient(configureOptions)
        : _pool.GetOrAdd(name, key => CreateClient(configureOptions));

    public ISerialPortClient GetOrCreate(Action<SerialPortOptions>? configureOptions = null) => CreateClient(configureOptions);

    private static DefaultSerialPortClient CreateClient(Action<SerialPortOptions>? configureOptions = null)
    {
        var options = new SerialPortOptions();
        configureOptions?.Invoke(options);
        return new DefaultSerialPortClient(options);
    }

    public ISerialPortClient? Remove(string name)
    {
        ISerialPortClient? client = null;
        if (_pool.TryRemove(name, out var c))
        {
            client = c;
        }
        return client;
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            // 释放托管资源
            foreach (var socket in _pool.Values)
            {
                await socket.DisposeAsync();
            }
            _pool.Clear();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
