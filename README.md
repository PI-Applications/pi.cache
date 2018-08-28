# PI.Cache SDK

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
    return await CacheClient.GetItem("PRODUCTS", () =>
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
```

### Example method
```csharp
public async Task<List<Product>> GetProducts()
{
    return await CacheClient.GetItem("PRODUCTS", GetProductsBuildCache);
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