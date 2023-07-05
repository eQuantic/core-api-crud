# eQuantic.Core.Api.Crud Library

The **eQuantic Core API CRUD** provides all the implementation needed to publish CRUD APIs.

To install **eQuantic.Core.Api.Crud**, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```dos
Install-Package eQuantic.Core.Api.Crud
```

## Example of implementation

### The data entities
```csharp
[Table("orders")]
public class OrderData : EntityDataBase
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    public virtual ICollection<OrderItemData> Items { get; set; } = new HashSet<OrderItemData>();
}

[Table("orderItems")]
public class OrderItemData : EntityDataBase, IWithReferenceId<OrderItemData, int>
{
    [Key]
    public int Id { get; set; }
    public int OrderId { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public virtual OrderData? Order { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
```

### The models
```csharp
public class Order
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### The request models
```csharp
public class OrderRequest
{
    public DateTime? Date { get; set; }
}

public class OrderItemRequest
{
    public string? Name { get; set; }
}
```

### The services
```csharp
public interface IOrderService : ICrudServiceBase<Order, OrderRequest>
{
    
}
public class OrderService : CrudServiceBase<Order, OrderRequest, OrderData, UserData>, IOrderService
{
    public OrderService(IQueryableUnitOfWork unitOfWork, IMapperFactory mapperFactory) : base(unitOfWork, mapperFactory)
    {
    }
}
```

### The `Program.cs`

```csharp
...

var app = builder.Build();

...

app.MapCrud<Order, OrderRequest, IOrderService>();
```