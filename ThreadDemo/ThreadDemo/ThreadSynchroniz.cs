using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo
{
    //线程同步

    class ThreadSynchroniz
    {
        public static void RaceConditions()
        {
            var state = new StateObject();
            for (int i = 0; i < 2; i++)
            {
                Task.Run(() => {
                    new SampleTask().RaceCondition(state);
                });
            }
        }

        public static void DeadLock()
        {
            var s1 = new StateObject();
            var s2 = new StateObject();
            Task.Run(() => {new SampleTask(s1, s2).DeadLock1();});
            Task.Run(() => { new SampleTask(s1, s2).DeadLock2(); });
        }
    }

    class StateObject
    {
        private int state = 5;

        object saftobj = new object();
        public void ChangeState(int loop)
        {
            if (state == 5)
            {
                state++;
                Console.WriteLine($"state:{state},thread:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                Trace.Assert(state == 6, "rece condition occurred after" + loop + "loops");
            }
            state = 5;
            Console.WriteLine($"===state:{state},thread:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            //方法2：设置共享对象为线程安全
            //lock (saftobj)
            //{
            //    if (state == 5)
            //    {
            //        state++;
            //        Console.WriteLine($"state:{state},thread:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            //        //Trace.Assert(state == 6, "rece condition occurred after" + loop + "loops");
            //    }
            //    state = 5;
            //    Console.WriteLine($"===state:{state},thread:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            //}

        }
    }


    class SampleTask
    {
        public void RaceCondition(object o)
        {
            //Trace.Assert(o is StateObject, "o must be type of StateObject");
            StateObject state = o as StateObject;
            int i = 0;

            while (true)
            {
                state.ChangeState(i++);
                System.Threading.Thread.Sleep(1);
            }

            //方法1：===在线程中锁定共享对象
            //while (true)
            //{
            //    lock (state)//no race condition with this lock
            //    {
            //        state.ChangeState(i++);
            //    }
            //}

        }

        public SampleTask() { }

        /*
        死锁:一个线程锁定s1,接着锁定s2
             另一个线程锁定s2接着锁定s1
        */
        
        private StateObject s1;
        private StateObject s2;

        public SampleTask(StateObject s1, StateObject s2)
        {
            this.s1 = s1; this.s2 = s2;
        }
        public void DeadLock1()
        {
            int i = 0;
            while (true)
            {
                lock(s1)
                {
                    lock (s2)
                    {
                        s1.ChangeState(i);
                        s2.ChangeState(i++);
                        Console.WriteLine($"still running,{i}");
                    }
                }
            }
        }

        public void DeadLock2()
        {
            int i = 0;
            while (true)
            {
                lock (s2)
                {
                    lock (s1)
                    {
                        s1.ChangeState(i);
                        s2.ChangeState(i++);
                        Console.WriteLine($"still running,{i}");
                    }
                }
            }
        }

    }

}
