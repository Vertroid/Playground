using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlayground
{
    internal class TestAsync
    {
        static CancellationTokenSource cts = new CancellationTokenSource();
        static void TestAsyncFunctionThreadIds()
        {
            _ = AsyncTaskIntFunction1();
            _ = AsyncTaskIntFunction2();
            SyncFunction();
            Task task = new Task(AsyncFunction, cts.Token);
            try
            {
                task.Start();
                cts.Cancel();
                Thread.Sleep(500);
                task.Wait();
            }
            catch (TaskCanceledException oce)
            {
                Console.WriteLine(oce.Message);
            }
            Console.WriteLine($"AsyncFunction {task.Status}!");


            cts.Dispose();
        }

        static void TestCancellationTokenRegister()
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Task task = new Task(() =>
            {
                int i = 1;
                token.Register(() =>
                {
                    Console.WriteLine("Operation is canceled");
                    i = 50;
                    Console.WriteLine($"Count is equal to '{i}'");
                });
                for (; i < 100; i++)
                {
                    Console.WriteLine($"Count is equal to '{i}'");
                    //add some timeout to emulate real-life execution
                    Thread.Sleep(10);
                }
            }, token);
            task.Start();

            // add some timeout to emulate real-life execution
            Thread.Sleep(200);
            // cancel the parallel operation
            cancelTokenSource.Cancel();
            // wait till the operation is completed
            task.Wait();
            // check the operation status
            Console.WriteLine($"Task Status is equal to '{task.Status}'");
            // release resources
            cancelTokenSource.Dispose();
        }

        static void AsyncFunction()
        {
            Thread.Sleep(2000);
            cts.Token.ThrowIfCancellationRequested();
            Console.WriteLine($"AsyncFunction {Thread.CurrentThread.ManagedThreadId}");
        }

        static async Task AsyncTaskIntFunction1()
        {
            await Task.Run(() =>
            {
                int hours = 0;
                Console.WriteLine($"AsyncTaskIntFunction1 {Thread.CurrentThread.ManagedThreadId}");
                return hours;
            });
        }

        static async Task<int> AsyncTaskIntFunction2()
        {
            int hours = 0;
            Console.WriteLine($"AsyncTaskIntFunction2 {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(0);
            return hours;
        }

        static void SyncFunction()
        {
            Console.WriteLine($"SyncFunction {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
