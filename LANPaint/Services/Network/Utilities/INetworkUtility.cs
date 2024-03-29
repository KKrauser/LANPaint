﻿using System.Net;
using System.Net.NetworkInformation;

namespace LANPaint.Services.Network.Utilities;

public interface INetworkUtility
{
    public IPAddress GetIpAddress(NetworkInterface networkInterface);
    public bool IsReadyToUse(NetworkInterface networkInterface);
    public bool IsReadyToUse(IPAddress ipAddress);
}