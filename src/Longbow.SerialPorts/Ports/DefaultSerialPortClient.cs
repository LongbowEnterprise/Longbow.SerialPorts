﻿// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.Modbus;
using System.Buffers;
using System.IO.Ports;

namespace Longbow.SerialPorts;

class DefaultSerialPortClient(SerialPortOptions options) : ISerialPortClient
{
    [NotNull]
    private SerialPort? _serialPort = null;
    private TaskCompletionSource _readTaskCompletionSource = new();
    private CancellationTokenSource _receiveCancellationTokenSource = new();
    private Memory<byte> _buffer = Memory<byte>.Empty;

    public bool IsOpen => _serialPort?.IsOpen ?? false;

    public async ValueTask<bool> OpenAsync(CancellationToken token = default)
    {
        _serialPort ??= new(options.PortName, options.BaudRate, options.Parity, options.DataBits, options.StopBits);
        _serialPort.RtsEnable = options.RtsEnable;
        _serialPort.DtrEnable = options.DtrEnable;

        _serialPort.Handshake = options.Handshake;
        _serialPort.DiscardNull = options.DiscardNull;
        _serialPort.ReadBufferSize = options.ReadBufferSize;
        _serialPort.WriteBufferSize = options.WriteBufferSize;

        _serialPort.ReadTimeout = options.ReadTimeout;
        _serialPort.WriteTimeout = options.WriteTimeout;

        _serialPort.DataReceived += DataReceived;
        _serialPort.ErrorReceived += ErrorReceived;

        var ret = false;
        try
        {
            await Task.Run(() =>
            {
                _serialPort.Open();
                ret = true;
            }, token);
        }
        catch (OperationCanceledException) { }

        return ret;
    }

    private void DataReceived(object? sender, SerialDataReceivedEventArgs e)
    {
        if (e.EventType == SerialData.Chars)
        {
            var bytesToRead = _serialPort.BytesToRead;
            if (bytesToRead > 0)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bytesToRead);
                if (_serialPort.Read(buffer, 0, bytesToRead) == bytesToRead)
                {
                    _buffer = new Memory<byte>(buffer, 0, bytesToRead);
                    _readTaskCompletionSource.TrySetResult();
                }
                else
                {
                    _readTaskCompletionSource.TrySetException(new IOException($"Failed to read the expected number of bytes from the serial port. expected: {bytesToRead}"));
                }
            }
        }

    }

    private void ErrorReceived(object? sender, SerialErrorReceivedEventArgs e)
    {
        // 处理串口错误
        //Exception = new IOException($"Serial port error: {e.EventType}");
        _readTaskCompletionSource.TrySetResult();
    }

    public async ValueTask<bool> SendAsync(ReadOnlyMemory<byte> data, CancellationToken token = default)
    {
        _serialPort.ThrowIfNotOpen();

        // 取消等待读取的任务
        if (_readTaskCompletionSource.Task.IsCompleted == false)
        {
            _readTaskCompletionSource.TrySetCanceled(CancellationToken.None);
        }
        _readTaskCompletionSource = new TaskCompletionSource();

        // 取消接收数据的任务
        if (_receiveCancellationTokenSource.IsCancellationRequested == false)
        {
            _receiveCancellationTokenSource.Cancel();
        }
        _receiveCancellationTokenSource.Dispose();
        _receiveCancellationTokenSource = new();

        // 清空缓存
        _buffer = Memory<byte>.Empty;

        _serialPort.DiscardInBuffer();
        _serialPort.DiscardOutBuffer();

        var ret = false;
        try
        {
            await Task.Run(() =>
            {
                _serialPort.Write(data.ToArray(), 0, data.Length);
                ret = true;
            }, token);
        }
        catch (OperationCanceledException) { }

        return ret;
    }

    public async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken token = default)
    {
        token.Register(_receiveCancellationTokenSource.Cancel);

        try
        {
            await _readTaskCompletionSource.Task.WaitAsync(_receiveCancellationTokenSource.Token);
        }
        catch (OperationCanceledException) { }
        return _buffer;
    }

    public ValueTask CloseAsync()
    {
        if (_receiveCancellationTokenSource.IsCancellationRequested == false)
        {
            _receiveCancellationTokenSource.Cancel();
        }

        if (_serialPort is { IsOpen: true })
        {
            _serialPort.Close();
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await CloseAsync();

        _receiveCancellationTokenSource.Cancel();
        _receiveCancellationTokenSource.Dispose();
    }
}
