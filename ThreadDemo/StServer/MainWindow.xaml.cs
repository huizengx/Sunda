using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace StServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Inital();
        }
        MyThreadPool JobPool = null;//定义处理线程池
        SocketManager m_socket = null;
        private void Inital()
        {
            m_socket = new SocketManager(200, 1024);
            m_socket.Init();
            //m_socket.Start(new IPEndPoint(IPAddress.Any, 13909));
            m_socket.ReceiveClientData += SocketManager_ReceiveClientData;
            m_socket.ClientNumberChange += M_socket_ClientNumberChange;
            JobPool = new MyThreadPool(8);
        }

        private void M_socket_ClientNumberChange(int num, AsyncUserToken token)
        {
            if (token!=null)
            {
                this.Dispatcher.Invoke(() => {
                    listBoxInfo.Items.Add($"Num:{num}--{token.IPAddress}");
                });
            }
        }

        private void SocketManager_ReceiveClientData(AsyncUserToken token, byte[] buff)
        {
            JobPool.AddTask(
                (obj) => {
                    string ret = string.Empty;
                    foreach (var item in buff)
                    {
                        ret += item + ",";
                    }

                    System.Threading.Thread.Sleep(1000);
                    //委托UI线程更新界面任务
                    this.Dispatcher.Invoke(() => {
                        listBoxMsg.Items.Add($"id:"+System.Threading.Thread.CurrentThread.ManagedThreadId+"==>"+ret);
                    });

                    Console.WriteLine($"id:" + System.Threading.Thread.CurrentThread.ManagedThreadId + "==>" + ret);

                    int index = buff.Length + 2;
                    byte[] temp = new byte[index];
                    Array.Copy(buff, 0, temp, 0, buff.Length);
                    temp[3] = 0x0A;
                    temp[index - 1] = 0xBB;
                    temp[index - 2] = 0xCB;
                    m_socket.SendMessage(token, temp);//回复消息
                },
                buff,
                (e) => {
                    this.Dispatcher.Invoke(() => {
                        listBoxMsg.Items.Add($"ThreadPool Err: "+e.Message);
                    });
                }
                );

            /*
            string ret = string.Empty;
            foreach (var item in buff)
            {
                ret += item + ",";
            }

            this.Dispatcher.Invoke(() => {
                listBoxMsg.Items.Add(ret);
            });
            int index = buff.Length + 2;
            byte[] temp = new byte[index];
            Array.Copy(buff, 0, temp, 0, buff.Length);
            temp[3] = 0x0A;
            temp[index - 1] = 0xBB;
            temp[index - 2] = 0xCB;
            m_socket.SendMessage(token, temp);
            */
        }

        private void bntCancel_Click(object sender, RoutedEventArgs e)
        {
            m_socket.Stop();
            JobPool.FinalPool();
        }

        private void bntStart_Click(object sender, RoutedEventArgs e)
        {
            JobPool.Start(8);
            m_socket.Start(new IPEndPoint(IPAddress.Any, 13909));
        }
        
    }
}
