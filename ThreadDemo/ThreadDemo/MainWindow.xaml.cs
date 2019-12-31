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

namespace ThreadDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParallelTest par = null;
        TaskTest ttest = null;
        public MainWindow()
        {
            InitializeComponent();
            par = new ParallelTest();
            ttest = new TaskTest();
        }

        private void ParallelBtn_Click(object sender, RoutedEventArgs e)
        {
            par.TestFor();
            //par.TestForEx();
            //par.TestForBreak();
            //par.TestInvoke();
            //Dispatcher.Invoke((Action)delegate { });
        }

        private void TaskBtn_Click(object sender, RoutedEventArgs e)
        {
            //ttest.TaskUsingThreadPool();
            ttest.TakeLongRuning();
        }
        
        private void Async_AwaitBtn_Click(object sender, RoutedEventArgs e)
        {
            ttest.MakeBreadfastAsync();
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            FindAppFile($@"C:\Program Files (x86)");//
        }

        //检索目录及子目录中的指定文件
        private void FindAppFile(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                var files = System.IO.Directory.GetFiles(path).AsParallel();
                Parallel.ForEach(files, (item) => {
                    var Name = System.IO.Path.GetFileName(item);
                    if (Name == "notepad++.exe")//
                    {
                        MessageBox.Show(item);
                        return;
                    }
                });

                var dirs = System.IO.Directory.GetDirectories(path);
                foreach (var item in dirs)
                {
                    FindAppFile(item);
                }
            }
            
        }


        private void Thread_contend_Click(object sender, RoutedEventArgs e)
        {
            ThreadSynchroniz.RaceConditions();
        }

        private void DeadLock_Click(object sender, RoutedEventArgs e)
        {
            ThreadSynchroniz.DeadLock();
        }

        
    }
}
