using eQuantic.Core.Api.Crud.Controllers;
using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Core.Api.Sample.Services;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Sample.Controllers;

/// <summary>
/// Hand-written controller demonstrating the explicit "traditional" style: derive from
/// <see cref="CrudControllerBase{TEntity,TRequest,TKey}"/> and declare a route. Coexists with the
/// auto-registered controllers (mounted under <c>mvc</c>) and the Minimal API endpoints (at the root).
/// </summary>
[Route("manual/examples")]
public class ExamplesController : CrudControllerBase<Example, ExampleRequest, int>
{
    public ExamplesController(IExampleService service) : base(service)
    {
    }
}
