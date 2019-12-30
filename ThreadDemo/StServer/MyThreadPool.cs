using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace StServer
{
    class MyThreadPool
    {
        bool IsThreadPoolEnable = false;
        List<Thread> ThreadContainer = null;
        ConcurrentQueue<ActionTask> TaskContainer = null;
        public MyThreadPool(int number)
        {
            IsThreadPoolEnable = true;
            ThreadContainer = new List<Thread>();
            TaskContainer = new ConcurrentQueue<ActionTask>();
            for (int i = 0; i < number; i++)
            {
                var t = new Thread(RunTask) { IsBackground = true };
                t.Start();
                ThreadContainer.Add(t);
            }
        }

        public void AddTask(Action<object> job,object obj,Action<Exception> errCallBack=null)
        {
            if (TaskContainer!=null)
            {
                ActionTask at = new ActionTask();
                at.Data = obj;
                at.Job = job;
                at.ErrCallBack = errCallBack;
                TaskContainer.Enqueue(at);
            }
        }

        public void FinalPool()
        {
            IsThreadPoolEnable = false;
            TaskContainer = null;
            if (ThreadContainer!=null)
            {
                foreach (var item in ThreadContainer)
                {
                    item.Join(10);//阻塞线程
                    //item.Abort();
                }
                ThreadContainer = null;
            }
        }

        private void RunTask()
        {
            while (true&&TaskContainer!=null&&IsThreadPoolEnable)
            {
                ActionTask at = null;
                TaskContainer?.TryDequeue(out at);
                //TaskContainer.TryDequeue(out at);
                if (at==null)
                {
                    Thread.Sleep(5);
                    continue;
                }
                try
                {
                    at.Job.Invoke(at.Data);
                }
                catch (Exception err)
                {
                    at?.ErrCallBack(err);
                }
            }
        }


    }

    class ActionTask
    {
        public object Data { get; set; }
        public Action<object> Job { get; set; }
        public Action<Exception> ErrCallBack { get; set; }
 
    }
}
