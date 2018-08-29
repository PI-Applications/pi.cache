# PI.Cache

![Visual Studio Team services](https://img.shields.io/vso/build/pi-applications-dk/8c43066a-ced2-41f9-822b-b5a7154a9b31/83.svg)
[![NuGet](https://img.shields.io/nuget/v/PI.Cache.svg)](https://www.nuget.org/packages/PI.Cache/)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/PI.Cache.svg)](https://www.nuget.org/packages/PI.Cache/)

Use this package if you want to support async cache.

## Supported frameworks
- .NET Framework 4.5
- .NET Framework 4.6
- .NET Standard 1.6
- .NET Standard 2.0

## How to use
When requesting the cache client you need a cache key and the function to be called when the cache is empty.

You can use either an lambda expression or method as function parameter.

### Example lambda
```csharp
public async Task<List<Product>> GetProducts()
{
    return await _cacheClient.GetItem("PRODUCTS", () =>
    {
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
```

### Example method
```csharp
public async Task<List<Product>> GetProducts()
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
```

### Only cache sometimes
If you need to store the result only some times you can return `CacheValue<TValue>`.

In this example the cache result will only be stored if the address has buildings.

```csharp
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
```