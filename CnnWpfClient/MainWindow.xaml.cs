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

          
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            testcase_001();
            testcase_002("test case: simple message");
        }

        private static bool testcase_001()
        {
            // test case: connect to websocket server and disconnect with websocket server
            try
            {
                using (WebSocketSession ts = new WebSocketSession())
                {

                }
                return true;
            }
            catch(Exception )
            {
                //MessageBox.Show(ex.Message);
            }
            return false;
        }

        private static bool testcase_002(string sMessage)
        {
            try
            {
                // test case: send a string to server and get server's response
                using (WebSocketSession ts = new WebSocketSession())
                {
                    ArraySegment<byte> Buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sMessage));
                    var sendTask = ts.ClientWS.SendAsync(Buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    sendTask.Wait();
                    if(TaskStatus.RanToCompletion == sendTask.Status)
                    {
                        var receiveTask = ts.ClientWS.ReceiveAsync(Buffer, CancellationToken.None);
                        receiveTask.Wait();
                        if (TaskStatus.RanToCompletion == receiveTask.Status)
                        {
                            string msg = Encoding.UTF8.GetString(Buffer.Array, 0, receiveTask.Result.Count).Trim();
                            bool bOK = string.Equals(msg, "received", StringComparison.OrdinalIgnoreCase);
                            //Debug.Assert(bOK);
                            return bOK;
                        }
                    }
                }
            }
            catch (Exception )
            {
                //MessageBox.Show(ex.Message);
            }
            return false;
        }
    }
}
