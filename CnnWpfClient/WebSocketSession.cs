using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CnnWpfClient
{
    public class WebSocketSession: IDisposable
    {
        public WebSocketSession()
        {
            PortNumber = 8131;
            IsSuccess = startServer();
        }
        public WebSocketSession(int nPort)
        {
            PortNumber = nPort;
            IsSuccess = startServer();
        }

        #region property
        private ClientWebSocket _ClientWS = null;
        public ClientWebSocket ClientWS
        {
            get
            {
                if (null == _ClientWS)
                    _ClientWS = new ClientWebSocket();
                return _ClientWS;
            }
        }

        private bool IsSuccess { get; set; }

        private int mPortNumber = -1;
        public int PortNumber
        {
            get
            {
                return mPortNumber;
            }
            set
            {
                mPortNumber = value;
            }
        }
        #endregion property

        private bool startServer()
        {
            try
            {
                Uri serverUri = new Uri("ws://localhost:" + PortNumber + "/");
                Task wsTask = ClientWS.ConnectAsync(serverUri, CancellationToken.None);
                wsTask.Wait();
                if (TaskStatus.RanToCompletion == wsTask.Status)
                {
                    byte[] data = new byte[1024];
                    ArraySegment<byte> Buffer = new ArraySegment<byte>(data);
                    var receiveTask = ClientWS.ReceiveAsync(Buffer, CancellationToken.None);
                    receiveTask.Wait();
                    if (TaskStatus.RanToCompletion == receiveTask.Status)
                    {
                        string msg = Encoding.UTF8.GetString(Buffer.Array, 0, receiveTask.Result.Count).Trim();
                        bool bOK = string.Equals(msg, "Hey all, a new client has joined us", StringComparison.OrdinalIgnoreCase);
                        //Debug.Assert(bOK);
                        return bOK;
                    }
                    //return ClientWS?.State == WebSocketState.Open;
                }
                //else if (TaskStatus.Canceled == wsTask.Status)
                //{
                //    return false;
                //}
                //else if (TaskStatus.Faulted == wsTask.Status)
                //{
                //    return false;
                //}
            }
            catch (System.Exception )
            {
                //return false;
            }

            throw new TestCaseException();
        }

        private bool stopServer()
        {
            if (ClientWS == null || ClientWS.State == WebSocketState.Closed || ClientWS.State == WebSocketState.CloseSent)
                return false;

            if (ClientWS.State != WebSocketState.Open)
                return false;

            try
            {
                // use CloseOutputAsync in client socket insead of CloseAsync
                // https://mcguirev10.com/2019/08/17/how-to-close-websocket-correctly.html, closeTask.status == WebSocketState.CloseSent means closed
                //Task closeTask = _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "completed", CancellationToken.None);
                Task wsTask = ClientWS.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                wsTask.Wait();
                if (TaskStatus.RanToCompletion == wsTask.Status)
                {
                    byte[] data = new byte[1024];
                    ArraySegment<byte> Buffer = new ArraySegment<byte>(data);
                    var finalReceiveTask = ClientWS.ReceiveAsync(Buffer, CancellationToken.None);
                    finalReceiveTask.Wait();
                    if (TaskStatus.RanToCompletion == finalReceiveTask.Status)
                    {
                        // server send "completed" back when it found client left, the string "completed" was defined in websocket server
                        string msg = Encoding.UTF8.GetString(Buffer.Array, 0, finalReceiveTask.Result.Count).Trim();
                        bool bOK = string.Equals(msg, "completed", StringComparison.OrdinalIgnoreCase);
                        //Debug.Assert(bOK);
                        return bOK;
                    }
                }
                //else if (TaskStatus.Canceled == wsTask.Status)
                //{
                //    return false;
                //}
                //else if (TaskStatus.Faulted == wsTask.Status)
                //{
                //    return false;
                //}
            }
            catch (System.Exception )
            {
                //return false;
            }

            throw new TestCaseException();
        }


        public void Dispose()
        {
            if (IsSuccess)
                IsSuccess = stopServer();
            ClientWS?.Dispose();
        }
    }

    public class TestCaseException : SystemException
    {

    }
}
