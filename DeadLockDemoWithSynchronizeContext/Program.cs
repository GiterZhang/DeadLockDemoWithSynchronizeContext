using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DeadLockDemoWithSynchronizeContext
{
    class Program
    {
        static int resultNum = 0;

        static void Main(string[] args)
        {
            Console.WriteLine($"Main thread Id:{Thread.CurrentThread.ManagedThreadId}");
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            var task = FunAsync().ContinueWith<int>((t, state) =>
            {
                var context = state as SynchronizationContext;
                if (context != null)
                {
                    Console.WriteLine($"Continuation thread Id:{Thread.CurrentThread.ManagedThreadId}");
                    context.Post(GetNum, 15);
                    return resultNum;
                }
                else
                {
                    Console.WriteLine($"context is null");
                    return 0;
                }
            }, SynchronizationContext.Current);
            Console.WriteLine($"result is {task.Result}");
        }
        static async Task FunAsync()
        {
            await Task.Delay(1000);
            Console.WriteLine($"Async thread Id:{Thread.CurrentThread.ManagedThreadId}");
        }

        static void GetNum(object obj)
        {
            resultNum = (int)obj;
        }
    }
}
