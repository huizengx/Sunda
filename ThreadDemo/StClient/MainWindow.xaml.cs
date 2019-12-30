using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace StClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //发送数据索引
        int i = 1;
        string ip = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            ip = TxbIpAdd.Text.Trim();
            Request.OnReceiveData += Request_OnReceiveData;
        }

        private void Btnconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                Request.Connect(ip);
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Request.Disconnect();
        }

        private void Request_OnReceiveData(object message)
        {
            string ret = string.Empty;
            byte[] buff = (byte[])message;
            foreach (var item in buff)
            {
                ret += item + ",";
            }

            this.Dispatcher.Invoke(() => {
                listboxMsg.Items.Add(i + "==Client Get:" + ret);
            });
            i++;
        }

        //0x00 0x00 0x00 0x08 0xA1 0xB1 0xA1 0xB1 0xA1 0xB1 0xA1 0xB1

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            i = 1;
            byte[] arry = new byte[] { 0x00, 0x00, 0x00, 0x08, 0xA1, 0xB1, 0xA1, 0xB1, 0xA1, 0xB1, 0xA1, 0xB1 };
            SendAsync(arry);
        }

        private async void TestSend(byte[] arry)
        {
            await SendAsync(arry);
        }

        private Task SendAsync(byte[] arry)
        {
            int larstindex = arry.Length - 1;
            var t = Task.Run(() =>
            {
                for (int i = 0; i < 200; i++)
                {
                    arry[larstindex] = (byte)i;
                    Request.Send(arry);
                    System.Threading.Thread.Sleep(10);
                }
            });
            return t;
        }



    }
}
