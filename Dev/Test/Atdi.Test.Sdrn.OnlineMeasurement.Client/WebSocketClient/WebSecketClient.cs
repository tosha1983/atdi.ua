using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketClientImpl
{
    public class WebSocketClient : IDisposable
    {
        private readonly Uri _uri;
        private readonly WebSocketHandler _handler;
        private readonly ClientWebSocket _webSocket;
        private Thread _thread;
        private volatile bool _isDisposing;
        private WebSocketContext _context;

        private readonly int _connectionTimeout = 5000; // 5 sec
        private readonly int _receiveChunkSize = 64 * 1024; // 64 kb 
        public WebSocketClient(Uri uri, WebSocketHandler handler)
        {
            this._uri = uri;
            this._handler = handler;
            this._webSocket = new ClientWebSocket(); 
        }

        public WebSocketContext Connect()
        {
            using (var cts = new CancellationTokenSource(_connectionTimeout))
            {
                _webSocket.ConnectAsync(_uri, cts.Token).GetAwaiter().GetResult();
            }
                
            if (this._webSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("The Websocket connection is not opened");
            }

            this._thread = new Thread(this.Process)
            {
                Name = $"ATDI.Platform.WebSocket.Process"
            };

            this._thread.Start();

            this._context = new WebSocketContext(_webSocket);
            return this._context;
        }

        public void Dispose()
        {
            if (!_isDisposing)
            {
                

                if (_webSocket.State == WebSocketState.Open)
                {
                    _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).GetAwaiter().GetResult();
                }
                _isDisposing = true;
                _thread.Abort();

                _context = null;
            }           
        }

        private void Process()
        {
            var buffer = new byte[_receiveChunkSize];

            try
            {
                var textBuffer = new StringBuilder();
                while(_webSocket.State == WebSocketState.Open)
                {
                    var result = _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).GetAwaiter().GetResult();
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                         _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).GetAwaiter().GetResult();
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var dataAsText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        textBuffer.Append(dataAsText);
                        if (result.EndOfMessage)
                        {
                            _handler.Handle(textBuffer.ToString(), _context);
                            textBuffer = new StringBuilder();
                        }
                        
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                if (_isDisposing)
                {
                    // this is normal process
                }
}
            catch (Exception)
            {
                //this.Logger.Exception(Contexts.EntityOrm, Categories.Processing, e, this);
            }

        }
    }
}
