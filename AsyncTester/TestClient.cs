using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTester
{
    public class TestClient
    {
        private readonly CacheClient<string> _cacheClient;

        public TestClient()
        {
            _cacheClient = new CacheClient<string>();
        }

        public async Task<List<Product>> GetProducts()
        {
            Console.WriteLine("Requesting products");

            return await _cacheClient.GetItem("PRODUCTS", () =>
            {
                var list = new List<Product>();
                for (int i = 0; i < 50; i++)
                {
                    list.Add(new Product
                    {
                        Id = i,
                        Name = $"Product {i}"
                    });
                }

                Task.Delay(1000).Wait();

                return Task.FromResult(list);
            });
        }

        public class Product
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
