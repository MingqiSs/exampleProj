using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        private
        static void Main(string[] args)
        {

            //char[] s = new char[] { 'h', 'e', 'l', 'l', '0' };
            //ReverseString(s);
            //Console.WriteLine(string.Join(",", s));


            //GetTypeDe();
            // Spinklock();
            // test();
            //  BC();

            AddDictionaries();
            Console.ReadLine();
        }

        /// <summary>
        /// 反转字符串 等同于Array.Reverse
        /// </summary>
        ///todo: 交换开头下标和结尾下标的字符，
        //递归处理中间剩下的字符。 
        /// <param name="s"></param>
        public static void ReverseString(char[] s, int i = 0)
        {
            // 递归出口
            if (i >= s.Length / 2) return;
            char temp;
            temp = s[i];
            s[i] = s[s.Length - 1 - i];
            s[s.Length - 1 - i] = temp;
            ReverseString(s, i + 1);
        }

        public static void GetTypeDe()
        {
            var Watch = new Stopwatch();
            object objName = string.Empty;


            var type = GetPerson().GetType();
            Watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                objName = type.GetProperty("Name").GetValue(GetPerson());
            }
            Watch.Stop();
            Console.WriteLine(Watch.Elapsed);


            dynamic dynPerson = GetPerson();
            Watch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                objName = dynPerson.Name;
            }
            Watch.Stop();
            Console.WriteLine(Watch.Elapsed);

        }
        static object GetPerson()
        {
            return new Person { Name = "Leo" };
        }


        /// <summary>
        /// 自增CAS实现
        /// </summary>
        public static void AtomicityForInterLock()
        {
            long result = 0;
            Console.WriteLine("开始计算");
            Parallel.For(0, 10, (i) =>
            {
                for (int j = 0; j < 10000; j++)
                {
                    //自增
                    Interlocked.Increment(ref result);
                }
            });
            Console.WriteLine($"结束计算");
            Console.WriteLine($"result正确值应为：{10000 * 10}");
            Console.WriteLine($"result    现值为：{result}");
            Console.ReadLine();
        }
        private static Object _obj = new object();
        /// <summary>
        /// 原子操作基于Lock实现
        /// </summary>
        public static void AtomicityForLock()
        {
            long result = 0;
            Console.WriteLine("开始计算");
            //10个并发执行
            Parallel.For(0, 10, (i) =>
            {
                //lock锁
                //lock (_obj)
                //{
                for (int j = 0; j < 10000; j++)
                {
                    result++;
                }
                // }
            });
            Console.WriteLine("结束计算");
            Console.WriteLine($"result正确值应为：{10000 * 10}");
            Console.WriteLine($"result    现值为：{result}");
            Console.ReadLine();

        }

        #region 创建自旋锁
        //创建自旋锁
        private static SpinLock spin = new SpinLock();
        public static void Spinklock()
        {
            Action action = () =>
            {
                bool lockTaken = false;
                try
                {
                    //申请获取锁
                    spin.Enter(ref lockTaken);
                    //临界区
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"当前线程{Thread.CurrentThread.ManagedThreadId.ToString()},输出:1");
                    }
                }
                finally
                {
                    //工作完毕，或者产生异常时，检测一下当前线程是否占有锁，如果有了锁释放它
                    //避免出行死锁
                    if (lockTaken)
                    {
                        spin.Exit();
                    }
                }
            };

            Action action2 = () =>
            {
                bool lockTaken = false;
                try
                {
                    //申请获取锁
                    spin.Enter(ref lockTaken);
                    //临界区
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"当前线程{Thread.CurrentThread.ManagedThreadId.ToString()},输出:2");
                    }

                }
                finally
                {
                    //工作完毕，或者产生异常时，检测一下当前线程是否占有锁，如果有了锁释放它
                    //避免出行死锁
                    if (lockTaken)
                    {
                        spin.Exit();
                    }
                }

            };
            //并行执行2个action
            Parallel.Invoke(action, action2);
        }
        #endregion

        #region 读写锁
        //读写锁， //策略支持递归
        private static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private static int index = 0;
        static void read()
        {
            try
            {
                //进入读锁
                rwl.EnterReadLock();
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"线程id:{Thread.CurrentThread.ManagedThreadId},读数据,读到index:{index}");
                }
            }
            finally
            {
                //退出读锁
                rwl.ExitReadLock();
            }
        }
        static void write()
        {
            try
            {
                //尝试获写锁
                while (!rwl.TryEnterWriteLock(50))
                {
                    Console.WriteLine($"线程id:{Thread.CurrentThread.ManagedThreadId},等待写锁");
                }
                Console.WriteLine($"线程id:{Thread.CurrentThread.ManagedThreadId},获取到写锁");
                for (int i = 0; i < 5; i++)
                {
                    index++;
                    Thread.Sleep(50);
                }
                Console.WriteLine($"线程id:{Thread.CurrentThread.ManagedThreadId},写操作完成");
            }
            finally
            {
                //退出写锁
                rwl.ExitWriteLock();
            }
        }
        /// <summary>
        /// 执行多线程读写
        /// </summary>
        public static void test()
        {
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            Task[] task = new Task[6];
            task[1] = taskFactory.StartNew(write); //写
            task[0] = taskFactory.StartNew(read); //读
            task[2] = taskFactory.StartNew(read); //读
            task[3] = taskFactory.StartNew(write); //写
            task[4] = taskFactory.StartNew(read); //读
            task[5] = taskFactory.StartNew(read); //读

            for (var i = 0; i < 6; i++)
            {
                task[i].Wait();
            }

        }

        #endregion

        #region 线程安全集合用法
        /// <summary>
        /// 线程安全集合用法
        /// </summary>
        public static void BC()
        {
            //线程安全集合
            using (BlockingCollection<int> blocking = new BlockingCollection<int>())
            {
                int NUMITEMS = 10000;

                for (int i = 1; i < NUMITEMS; i++)
                {
                    blocking.Add(i);
                }
                //完成添加
                blocking.CompleteAdding();
                int outerSum = 0;

                // 定义一个委托方法取出集合元素
                Action action = () =>
                {
                    int localItem;
                    int localSum = 0;

                    //取出并删除元素，先进先出
                    while (blocking.TryTake(out localItem))
                    {
                        localSum += localItem;
                    }
                    //两数相加替换第一个值
                    Interlocked.Add(ref outerSum, localSum);
                };
                //并行3个线程执行，多个线程同时取集合的数据
                Parallel.Invoke(action, action, action);

                Console.WriteLine($"0+...{NUMITEMS - 1} = {((NUMITEMS * (NUMITEMS - 1)) / 2)},输出结果：{outerSum}");
                //此集合是否已标记为已完成添加且为空
                Console.WriteLine($"线程安全集合.IsCompleted={blocking.IsCompleted}");
            }
        }
        #endregion

        #region 线程安全字典用法
        //普通字典
        private static IDictionary<string, string> Dictionaries { get; set; } = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 字典增加值
        /// </summary>
        public static void AddDictionaries()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //并发1000个线程写
            Parallel.For(0, 1000, (i) =>
            {
                var key = $"key-{i}";
                var value = $"value-{i}";

                // 不加锁会报错
                //lock (Dictionaries)
                //{
                    Dictionaries.Add(key, value);
               // }
            });
            sw.Stop();
            Console.WriteLine("Dictionaries 当前数据量为： {0}", Dictionaries.Count);
            Console.WriteLine("Dictionaries 执行时间为： {0} ms", sw.ElapsedMilliseconds);
        }
        #endregion

        public class Person
        {
            public string Name { get; set; }

        }


    }
}
