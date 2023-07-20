using Microsoft.EntityFrameworkCore;

namespace eQuantic.Core.Api.Sample;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
        : base(options) { }
}