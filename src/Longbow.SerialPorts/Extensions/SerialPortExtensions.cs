﻿// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.IO.Ports;

namespace Longbow.Modbus;

static class SerialPortExtensions
{
    public static void ThrowIfNotOpen(this SerialPort? port)
    {
        if (port == null || port.IsOpen == false)
        {
            throw new InvalidOperationException("端口未打开，请调用 OpenAsync 方法打开端口");
        }
    }
}
