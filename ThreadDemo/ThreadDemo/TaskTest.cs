using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadDemo.Logs;

namespace ThreadDemo
{
    class TaskTest
    {
        public TaskTest() { }

        static object locker = new object();
        private static void TaskMethod(object title)
        {
            lock (locker)
            {
                Console.WriteLine(title);
                Console.WriteLine($"task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine($"is pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
                Console.WriteLine($"Is background thread {Thread.CurrentThread.IsBackground}");
                Console.WriteLine();
            }
        }

        //Task 中执行的线程都是默认来自线程池
        public void TaskUsingThreadPool()
        {
            var tf = new TaskFactory();
            tf.StartNew(TaskMethod, "using TaskFactory");

            Task t2 = Task.Factory.StartNew(TaskMethod,"factory via a task");

            var t3 = new Task(TaskMethod, "using a task constructor and start");
            t3.Start();

            Task t4 = Task.Run(()=>TaskMethod("using the run method"));
        }

        //创建一个单独的，长时间运行的任务。
        //比如：可以创建日志记录任务。侦听任务等。
        public void TakeLongRuning()
        {
            //LongRunning,执行线程不是后台线程。
            var t1 = new Task(TaskMethod, "long running",TaskCreationOptions.LongRunning);
            t1.Start();
        }

        //创建异步方法
        private async Task<string> FilterValue()
        {
            var data = Enumerable.Range(100, 300);
            //await Task.Run(()=> { Parallel.ForEach(data, DoSomething); });
            string ret = string.Empty;

            var getdataArry = Task.Factory.StartNew(() => {
                Random random = new Random();
                int[] values = new int[100];
                for (int i = 0; i < values.GetUpperBound(0); i++)
                {
                    values[i] = random.Next();
                }
                return values;
            });

            var processdata = getdataArry.ContinueWith((x) => {
                int n = x.Result.Length;
                long sum = 0;
                double mean;
                for (int ctr = 0; ctr < x.Result.GetUpperBound(0); ctr++)
                {
                    sum += x.Result[ctr];
                }
                mean = sum / (double)n;
                return Tuple.Create(n, sum, mean);
            });

            var displaydata = processdata.ContinueWith((x) =>
            {
                return ($"N={x.Result.Item1:N0},Total={x.Result.Item2:N0},Mean={x.Result.Item3:N2}");
            });

            ret = await Task.Run(() =>
            {
                Console.WriteLine($"Start Task CurrentId: {Task.CurrentId}");
                var loopResult = Parallel.ForEach(data, (e) =>
                {
                    if (e == 129)
                    {
                        Task.Delay(e);
                    }
                    else if (e == 301)
                    {

                    }
                    Console.WriteLine($"{e}, ID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                });
                return "1";
            });
            return ret;
        }

        public async void MakeBreadfastAsync()
        {
            Logger.WriteLog($"--Prepare task--{DateTime.Now.ToString("MM:hh:mm:ss:fff")}");
            var fryeggtask = FryEggAsync(2);
            var frybacontask = FryBacon(3);
            var frytoastbreadtask = MakeToastBreadWithButterJamAsync(6);
            var allTasks = new List<Task> { frybacontask, fryeggtask, frytoastbreadtask };//创建人物集合
            Logger.WriteLog($"--Start task-- {DateTime.Now.ToString("MM:hh:mm:ss:fff")}");
            while (allTasks.Any())
            {
                Task finished = await Task.WhenAny(allTasks);//
                if (finished == fryeggtask)
                {
                    Logger.WriteLog($"===>fry egg ready ");
                }
                else if (finished == frybacontask)
                {
                    Logger.WriteLog($"===>fry bacon ready ");
                }
                else if (finished == frytoastbreadtask)
                {
                    Logger.WriteLog($"===>fry toastbread ready ");
                }
                allTasks.Remove(finished);
            }
            Logger.WriteLog($"--End task--{DateTime.Now.ToString("MM:hh:mm:ss:fff")}");
        }

        //这个方法会阻塞线程
        public async void BreakfastTask()
        {
            //存储任务
            Logger.WriteLog($"--Prepare task--");
            Task<int> tegg = FryEgg(2);
            Task<string> tbacon = FryBacon(3);
            Task<string> ttoast = ToastBread(5);

            Logger.WriteLog($"--Start task--");
            //启动所有任务，逐个人物执行，属于同步
            int agg = await tegg;
            Logger.WriteLog($"===>fry egg ready {agg}");
            string bacon = await tbacon;
            Logger.WriteLog($"===>fry bacon ready {bacon}");
            string toast = await ttoast;
            Logger.WriteLog($"===>fry toast ready {toast}");
            Logger.WriteLog($"--End task--");
        }

        private Task<int> FryEgg(int num)
        {
            return Task.Run(() =>
            {
                Logger.WriteLog($"FryEgg, task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                for (int i = 0; i < num; i++)
                {
                    //Task.Delay(1000);
                    System.Threading.Thread.Sleep(1000);
                    Logger.WriteLog($"fry egg {i}");
                }
                return num;
            });
        }

        private Task<string> FryBacon(int num)
        {
            return Task.Run(() =>
            {
                Logger.WriteLog($"FryBacon task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                for (int i = 0; i < num; i++)
                {
                    Task.Delay(500);//异步方法
                    Logger.WriteLog($"fry Bacon {i}");
                }
                return num.ToString();
            });
        }

        private Task<string> ToastBread(int num)
        {
            return Task.Run(() =>
            {
                Logger.WriteLog($"ToastBread task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                for (int i = 0; i < num; i++)
                {
                    Task.Delay(1000);
                    Logger.WriteLog($"fry ToastBread {i}");
                }
                return num.ToString();
            });
        }

        private async Task<int> FryEggAsync(int num)
        {
            Task<int> tegg = FryEgg(num);
            return await tegg;
        }

        private async Task<string> FryBaconAsync(int num)
        {
            Task<string> tegg = FryBacon(num);
            return await tegg;
        }

        //封装异步方法
        private async Task<string> ToastBreadAsync(int num)
        {
            Task<string> ttoast = ToastBread(num);
            var result= await ttoast;
            return result;
        }

        private void ApplyButter(string toast)
        {
            Logger.WriteLog($"Apply Butter to toastbread");
        }
        private void ApplyJam(string toast)
        {
            Logger.WriteLog($"Apply Jam to toastbread");
        }

        //先烤完面包，再涂上黄油和果酱
        async Task<string> MakeToastBreadWithButterJamAsync(int mun)
        {
            string toast= await ToastBreadAsync(mun);
            ApplyButter(toast);
            ApplyJam(toast);
            return toast;
        }

    }
}
