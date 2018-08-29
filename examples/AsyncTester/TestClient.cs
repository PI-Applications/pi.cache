using PI.Cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncTester
{
    public class TestClient
    {
        private readonly CacheClient _cacheClient;

        public TestClient()
        {
            _cacheClient = new CacheClient();
        }

        public async Task<List<Product>> GetProducts()
        {
            Console.WriteLine("Requesting products");

            return await _cacheClient.GetItem("PRODUCTS", () =>
            {
                throw new Exception("YOYOYOYOYOYO");

                return Task.Run(async () =>
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

                    await Task.Delay(1000);

                    return new List<Product>(list);
                });
            });
        }

        public async Task<List<Product>> GetProducts2()
        {
            return await _cacheClient.GetItem("PRODUCTS", GetProductsBuildCache);
        }

        public async Task<List<Product>> GetProductsBuildCache()
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

            await Task.Delay(1000);

            return list;
        }

        public async Task<List<Policy>> GetPolicies(string customerNo)
        {
            var policies = new List<Policy>();

            var products = await GetProducts();

            Console.WriteLine("Products count: " + products.Count);

            for (int i = 0; i < 5; i++)
            {
                policies.Add(new Policy
                {
                    Id = i,
                    Description = $"Test {i}"
                });
            }

            return policies;
        }

        public async Task<AddressInfo> LookupAddress(string addressId)
        {
            return await _cacheClient.GetItem(addressId, () =>
            {
                return Task.Run(async () =>
                {
                    await Task.Delay(1000);

                    var addressInfo = new AddressInfo();

                    if (addressId == "1")
                        addressInfo.HasBuildings = true;

                    return new CacheValue<AddressInfo>(addressInfo, addressInfo.HasBuildings);
                });
            });
        }

        public class AddressInfo
        {
            public string AdressId { get; set; }
            public bool HasBuildings { get; set; }
        }

        public class Product
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        public class Policy
        {
            public long Id { get; set; }
            public string Description { get; set; }
        }
    }
}
