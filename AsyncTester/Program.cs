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
            for (int i = 0; i < 50; i++)
            {
                Console.WriteLine($"Policy request {i}");
                taskList.Add(testClient.GetPolicies(i.ToString()));
            }

            await Task.WhenAll(taskList);

            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        static async Task Do()
        {

        }
                
    }
    
}
