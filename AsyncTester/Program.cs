using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            RunAsync().Wait();
        }

        static async Task RunAsync()
        {

            var testClient = new TestClient();
            var taskList = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Product request {i}");
                taskList.Add(testClient.GetProducts());
            }

            await Task.WhenAll(taskList);

            Console.WriteLine("DONE");
            Console.ReadKey();
        }
                
    }
    
}
