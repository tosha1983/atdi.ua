using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Atdi.Platform.Logging;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket
{
    public class WebSocketServer
    {
        private class HandshakeRequest
        {
            // ReSharper disable NotAccessedField.Local
            public string Url;
            public string Host;
            public string Upgrade;
            public string Connection;
            public string Origin;
            public string SecWebSocketKey;
            public string SecWebSocketProtocol;
            public string SecWebSocketVersion;
        }

        private const string PublicWebSocketKey = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        private readonly WebSocketPipeline _pipeline;
        private readonly int _port;
        private readonly int _bufferSize;
        private readonly ILogger _logger;

        public WebSocketServer(WebSocketPipeline pipeline, int port, ILogger logger, int bufferSize = 65536)
        {
            _pipeline = pipeline;
            _port = port;
            _bufferSize = bufferSize;
            _logger = logger;
        }

        public void Run()
        {
            TcpListener tcpListener = null;
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, _port);

                tcpListener.Start();

                // wait client of 1 minute
                var i = 0;
                while (!tcpListener.Pending())
                {
                    Thread.Sleep(100);
                    if (i++ > 600)
                    {
                        throw new TimeoutException();
                    }
                }

                using (var client = tcpListener.AcceptTcpClient())
                {
                    var stream = client.GetStream();

                    while (!stream.DataAvailable)
                    {
                    }

                    while (client.Available < 3)
                    {
                    }

                    var buffer = new byte[client.Available];
                    var count = stream.Read(buffer, 0, client.Available);
                    var data = Encoding.UTF8.GetString(buffer, 0, count);

                    if (TryDecodeHandshake(data, out var request))
                    {
                        buffer = BuildHandshakeResponse(request);
                        stream.Write(buffer, 0, buffer.Length);
                        var context = new WebSocketContext(client);
                        ProcessEvent(client, context);
                    }
                    else
                    {
                        buffer = BuildForbiddenResponse();
                        stream.Write(buffer, 0, buffer.Length);
                        _pipeline.OnDisconnect(null, DM.SensorOnlineMeasurementStatus.CanceledBySensor, "The client could not go through the handshake phase");
                    }

                    if (client.Connected)
                    {
                        client.Close();
                    }
                }
            }
            catch (TimeoutException)
            {
                _pipeline.OnDisconnect(null, DM.SensorOnlineMeasurementStatus.CanceledBySensor, "Client connection timed out");
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                CloseSocket(tcpListener);
                tcpListener = null;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.WebSocket, Categories.Running, e, this);
            }
            finally
            {
                CloseSocket(tcpListener);
            }
        }

        private void CloseSocket(TcpListener tcpListener)
        {
            if (tcpListener == null)
            {
                return;
            }
            try
            {
                tcpListener.Stop();
                _logger.Verbouse(Contexts.WebSocket, Categories.Stopping, "The TCP Listener was stopped");
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.WebSocket, Categories.Stopping, e, this);
            }
        }

        //public void Stop()
        //{
        //    this._stopped = true;
        //}

        private static bool TryDecodeHandshake(string data, out HandshakeRequest request)
        {
            request = null;

            var parts = data.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (parts.Length < 2)
            {
                return false;
            }

            var getParts = parts[0].Split(new[] { "GET " }, StringSplitOptions.None);
            if (getParts.Length < 2)
            {
                return false;
            }
            var httpParts = getParts[1].Split(new[] { " HTTP" }, StringSplitOptions.None);
            if (httpParts.Length < 1)
            {
                return false;
            }
            request = new HandshakeRequest
            {
                Url = WebUtility.UrlDecode(httpParts[0])
            };

            for (var i = 1; i < parts.Length; i++)
            {
                var line = parts[i];
                var index = line.IndexOf(":", StringComparison.Ordinal);
                if (index == -1)
                {
                    continue;
                }
                var key = line.Substring(0, index);
                var value = line.Substring(index + 1).Trim();
                if("Host".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.Host = value;
                }
                else if ("Upgrade".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.Upgrade = value;
                }
                else if ("Connection".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.Connection = value;
                }
                else if ("Sec-WebSocket-Key".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.SecWebSocketKey = value;
                }
                else if ("Origin".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.Origin = value;
                }
                else if ("Sec-WebSocket-Protocol".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.SecWebSocketProtocol = value;
                }
                else if ("Sec-WebSocket-Version".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    request.SecWebSocketVersion = value;
                }
            }
            //if (string.IsNullOrEmpty(request.Connection) || !"Upgrade".Equals(request.Connection, StringComparison.OrdinalIgnoreCase))
            //{
            //    return false;
            //}
            if (string.IsNullOrEmpty(request.Upgrade) || !"websocket".Equals(request.Upgrade, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        private byte[] BuildForbiddenResponse()
        {
            var header = "HTTP/1.1 403 Forbidden" + "\r\n\r\n";
            return Encoding.ASCII.GetBytes(header);
        }
        private byte[] BuildHandshakeResponse(HandshakeRequest request)
        {
            var baseKey = string.Concat(request.SecWebSocketKey, PublicWebSocketKey);
            var hashedKey = HashKey(baseKey);
            var responseHeader = 
                "HTTP/1.1 101 Switching Protocols\r\n" +
                "Upgrade: websocket\r\n" +
                "Connection: Upgrade\r\n" +
                "Sec-WebSocket-Accept: " + hashedKey +
                "\r\n\r\n";

            return Encoding.ASCII.GetBytes(responseHeader);
        }

        private static string HashKey(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] hashBytes;
            using (var sha1 = SHA1.Create())
            {
                hashBytes = sha1.ComputeHash(keyBytes);
            }
            return Convert.ToBase64String(hashBytes);
        }


        private void ProcessEvent(TcpClient client, WebSocketContext context)
        {
            var isConnected = false;
            var status = DM.SensorOnlineMeasurementStatus.CanceledBySensor;
            var reason = "";
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[_bufferSize];
                var data = new List<byte>();
                var continuationFrame = false;

                while (client.Connected && stream.CanRead)
                {
                    if (!isConnected)
                    {
                        Thread.Sleep(500);
                        _pipeline.OnConnect(context);
                        isConnected = true;
                    }

                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    stream.Flush();
                    var opCode = buffer[0] & 0x0F;
                    var finalMessage = ((buffer[0] & 0x80) == 0x80);
                    var maskKey = ((buffer[1] & 0x80) == 0x80);
                    ulong payloadLength;
                    var initialPayloadLength = buffer[1] & 0x7F;

                    var messageKind = WebSocketMessageKind.Unknown;

                    switch (opCode)
                    {
                        case 0x00:
                            continuationFrame = true;
                            break;
                        case 0x01:
                            continuationFrame = false;
                            messageKind = WebSocketMessageKind.Text;
                            break;
                        case 0x02:
                            continuationFrame = false;
                            messageKind = WebSocketMessageKind.Binary;
                            break;
                        case 0x08:
                            continuationFrame = false;
                            messageKind = WebSocketMessageKind.Closed;
                            break;
                        case 0x09:
                            continuationFrame = false;
                            messageKind = WebSocketMessageKind.Ping;
                            break;
                        case 0x0A:
                            continuationFrame = false;
                            messageKind = WebSocketMessageKind.Pong;
                            break;
                        default:
                            stream.Close();
                            client.Close();
                            break;
                    }


                    byte[] payloadLengthBytes;
                    var maskKeyBytes = new byte[4];

                    ulong dataLength;

                    switch (initialPayloadLength)
                    {
                        case 126:
                            payloadLengthBytes = new byte[2];
                            Array.Copy(buffer, 2, payloadLengthBytes, 0, payloadLengthBytes.Length);
                            payloadLength = BitConverter.ToUInt16(payloadLengthBytes.Reverse().ToArray(), 0);

                            dataLength = payloadLength;
                            if (maskKey)
                            {
                                Array.Copy(buffer, 4, maskKeyBytes, 0, maskKeyBytes.Length);
                                var tempData = new byte[payloadLength];
                                Array.Copy(buffer, 8, tempData, 0, tempData.Length);

                                for (var i = 0; i < tempData.Length; i++)
                                {
                                    tempData[i] = (byte)(maskKeyBytes[i % 4] ^ tempData[i]);
                                }

                                data.AddRange(tempData);
                            }
                            else
                            {
                                var tempData = new byte[payloadLength];
                                Array.Copy(buffer, 4, tempData, 0, tempData.Length);
                                data.AddRange(tempData);
                            }

                            break;
                        case 127:
                            payloadLengthBytes = new byte[8];
                            Array.Copy(buffer, 2, payloadLengthBytes, 0, payloadLengthBytes.Length);
                            payloadLength = BitConverter.ToUInt64(payloadLengthBytes.Reverse().ToArray(), 0);
                            dataLength = payloadLength;

                            if (maskKey)
                            {
                                Array.Copy(buffer, 10, maskKeyBytes, 0, maskKeyBytes.Length);
                                var tempData = new byte[payloadLength];

                                Array.Copy(buffer, 14, tempData, 0, tempData.Length);
                                for (var i = 0; i < tempData.Length; i++)
                                {
                                    tempData[i] = (byte)(maskKeyBytes[i % 4] ^ tempData[i]);
                                }
                                data.AddRange(tempData);
                            }
                            else
                            {
                                var tempData = new byte[payloadLength];

                                Array.Copy(buffer, 10, tempData, 0, tempData.Length);
                                data.AddRange(tempData);
                            }
                            break;
                        default:
                            payloadLength = (uint)initialPayloadLength;
                            dataLength = payloadLength;

                            if (maskKey)
                            {
                                Array.Copy(buffer, 2, maskKeyBytes, 0, maskKeyBytes.Length);
                                var tempData = new byte[payloadLength];
                                Array.Copy(buffer, 6, tempData, 0, tempData.Length);
                                for (var i = 0; i < tempData.Length; i++)
                                {
                                    tempData[i] = (byte)(maskKeyBytes[i % 4] ^ tempData[i]);
                                }
                                data.AddRange(tempData);
                            }
                            else
                            {
                                var tempData = new byte[(bytesRead - 2)];
                                Array.Copy(buffer, 2, tempData, 0, tempData.Length);
                                data.AddRange(tempData);
                            }
                            break;
                    }


                    if (!continuationFrame && finalMessage)
                    {
                        var message = new WebSocketMessage
                        {
                            Kind = messageKind,
                            Length = dataLength,
                            Data = data.ToArray()
                        };

                        _pipeline.Handle(message, context);
                        data.Clear();
                    }

                    if (messageKind == WebSocketMessageKind.Closed)
                    {
                        context.SendMessage(new WebSocketMessage { Kind = WebSocketMessageKind.Closed, Data = new byte[] { } });
                        stream.Close();
                        client.Close();
                        status = DM.SensorOnlineMeasurementStatus.CanceledByClient;
                        reason = "The client asked to close the connection";
                    }
                    else if (messageKind == WebSocketMessageKind.Ping)
                    {
                        context.SendMessage(new WebSocketMessage { Kind = WebSocketMessageKind.Pong, Data = new byte[] { } });
                    }
                    //else if (messageKind == WebSocketMessageKind.Pong)
                    //{
                    //    context.SendMessage(new WebSocketMessage { Kind = WebSocketMessageKind.Ping, Data = new byte[] { } });
                    //}
                }
            }
            catch (Exception e)
            {
                reason = $"Sensor interrupted measurement due to unexpected error: {e.Message}";
                _logger.Exception(Contexts.WebSocket, Categories.Processing, e, this);
            }
            finally
            {
                if (client.Connected)
                {
                    client.Close();
                }
                if (isConnected)
                {
                    _pipeline.OnDisconnect(context, status, reason);
/*
                    isConnected = false;
*/
                }
            }
            
        }
    }
}
