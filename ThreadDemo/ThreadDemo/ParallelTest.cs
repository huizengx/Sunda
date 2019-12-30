using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadDemo
{
    /*
     * Parallel是对线程的抽象。
     * 任务并行：CPU多个核心被利用起来，更快速完成多个任务的活动。
     * 数据并行：数据集上执行的工作被划分为多个任务
    */
    class ParallelTest
    {
        public ParallelTest() { }

        #region 数据并行
        //使用多个任务完成同一个作业。（数据并行）
        public void TestFor()
        {
            var result = Parallel.For(0, 100, (e) =>
            {
                Console.WriteLine($"{e},task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                ThreadDemo.Logs.Logger.WriteLog($"{e},task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(10);
            });
            Console.WriteLine($"Is completed:{result.IsCompleted}");
        }

        //Parallel 只等待它创建的任务，而不等待其他后台活动。
        public void TestForEx()
        {
            var result = Parallel.For(0, 10, async (e) =>
            {
                Console.WriteLine($"{e},task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(1000);
                Console.WriteLine($"{e},task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
            });
            Console.WriteLine($"Is completed:{result.IsCompleted}");
        }

        //提前停止Parallel.For，不是完成所有迭代
        public void TestForBreak()
        {
            var result = Parallel.For(5, 40, async (int i, ParallelLoopState pls) => {
                Console.WriteLine($"{i},task:{Task.CurrentId},thread:{Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(10);
                if (i>24)
                {
                    pls.Break();
                }
            });
            Console.WriteLine($"Is completed:{result.IsCompleted}");
            Console.WriteLine($"LowestBreakIteration:{result.LowestBreakIteration}");
        }

        #endregion

        #region 任务并行
        public void TestInvoke()
        {
            Parallel.Invoke(Foo, Bar);
        }

        private void Foo()
        {
            Console.WriteLine("Foo");
        }

        private void Bar()
        {
            Console.WriteLine("Bar");
        }

        #endregion 

    }
}
