using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CnnWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void onTestCase_001(object sender, RoutedEventArgs e)
        {
            TestCase_ShortConnectWithoutAnySendData();
        }

        private static void TestCase_ShortConnectWithoutAnySendData()
        {
            string msg = null;
            try
            {
                using (ClientWebSocket _ws = new ClientWebSocket())
                {
                    Task connectTask = _ws.ConnectAsync(new Uri("ws://localhost:8131"), CancellationToken.None);
                    connectTask.Wait();
                    byte[] data = new byte[1024];
                    ArraySegment<byte> Buffer = new ArraySegment<byte>(data);
                    var receiveTask = _ws.ReceiveAsync(Buffer, CancellationToken.None);
                    if (TaskStatus.RanToCompletion == connectTask.Status)
                    {
                        receiveTask.Wait();
                        if (TaskStatus.RanToCompletion == receiveTask.Status)
                        {
                            // server send "completed" back when it found client left, the string "completed" was defined in websocket server
                            msg = Encoding.UTF8.GetString(Buffer.Array, 0, receiveTask.Result.Count).Trim();
                            Debug.Assert(string.Equals(msg, "Hey all, a new client has joined us", StringComparison.OrdinalIgnoreCase));
                        }
                    }

                    // use CloseOutputAsync in client socket insead of CloseAsync
                    // https://mcguirev10.com/2019/08/17/how-to-close-websocket-correctly.html, closeTask.status == WebSocketState.CloseSent means closed
                    //Task closeTask = _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "completed", CancellationToken.None);
                    Task closeTask = _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                    closeTask.Wait();
                    if (TaskStatus.RanToCompletion == closeTask.Status)
                    {
                        msg = "Closed";
                        var finalReceiveTask = _ws.ReceiveAsync(Buffer, CancellationToken.None);
                        finalReceiveTask.Wait();
                        if (TaskStatus.RanToCompletion == finalReceiveTask.Status)
                        {
                            // server send "completed" back when it found client left, the string "completed" was defined in websocket server
                            msg = Encoding.UTF8.GetString(Buffer.Array, 0, finalReceiveTask.Result.Count).Trim();
                            Debug.Assert(string.Equals(msg, "completed", StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                msg = ex.Message;
            }
            //MessageBox.Show(msg);
            return;
        }
    }
}
