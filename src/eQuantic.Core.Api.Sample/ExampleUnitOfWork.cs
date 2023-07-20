using eQuantic.Core.Data.EntityFramework.Repository;

namespace eQuantic.Core.Api.Sample;

public class ExampleUnitOfWork: DefaultUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleUnitOfWork"/> class
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="dbContext">The db context</param>
    public ExampleUnitOfWork(IServiceProvider serviceProvider, ExampleDbContext dbContext) : base(serviceProvider, dbContext)
    {
    }
}