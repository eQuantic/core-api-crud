using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace eQuantic.Core.Api.Crud.Tests;

/// <summary>
/// Integration tests over the "traditional" controllers: the auto-registered ones (under <c>mvc</c>)
/// and the explicit hand-written one (under <c>manual</c>). Covers CRUD, references, complex keys,
/// validation and not-found, asserting parity with the Minimal API route shapes.
/// </summary>
public sealed class ControllersIntegrationTests : IDisposable
{
    private readonly ControllerApiFactory _factory = new();

    private HttpClient CreateClient()
        => _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public void Dispose() => _factory.Dispose();

    private sealed record ExampleDto(int Id, string Name);

    // ---- Auto-registered CRUD (int key) ----

    [Fact]
    public async Task Create_returns_201_with_location_and_id()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/mvc/examples", new { name = "Foo" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/mvc/examples/", response.Headers.Location!.ToString());
        var id = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(id > 0);
    }

    [Fact]
    public async Task GetById_existing_returns_200_with_entity()
    {
        var seeded = _factory.Examples.Seed("Bar");
        var client = CreateClient();

        var response = await client.GetAsync($"/mvc/examples/{seeded.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ExampleDto>();
        Assert.Equal(seeded.Id, dto!.Id);
        Assert.Equal("Bar", dto.Name);
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/mvc/examples/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPagedList_returns_200()
    {
        _factory.Examples.Seed("A");
        _factory.Examples.Seed("B");
        var client = CreateClient();

        var response = await client.GetAsync("/mvc/examples");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPagedList_binds_filterBy_and_orderBy_from_the_query_string()
    {
        var client = CreateClient();

        // v3 query syntax; the typed FilteringCollection<T>/SortingCollection<T> bind natively via TryParse.
        var response = await client.GetAsync("/mvc/examples?filterBy=name:eq(Foo)&orderBy=name:desc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var request = _factory.Examples.LastPagedRequest;
        Assert.NotNull(request);
        Assert.Equal(1, request!.FilterBy?.Count);
        Assert.Equal(1, request.OrderBy?.Count);
    }

    [Fact]
    public async Task Update_existing_returns_200()
    {
        var seeded = _factory.Examples.Seed("Old");
        var client = CreateClient();

        var response = await client.PutAsJsonAsync($"/mvc/examples/{seeded.Id}", new { name = "New" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_existing_returns_200()
    {
        var seeded = _factory.Examples.Seed("Del");
        var client = CreateClient();

        var response = await client.DeleteAsync($"/mvc/examples/{seeded.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_invalid_request_returns_400()
    {
        var client = CreateClient();

        // ExampleRequest.Name is [Required]; [ApiController] returns 400 automatically.
        var response = await client.PostAsJsonAsync("/mvc/examples", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---- Explicit hand-written controller ----

    [Fact]
    public async Task Explicit_controller_create_returns_201()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/manual/examples", new { name = "Manual" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains("/manual/examples/", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Explicit_controller_get_by_id_returns_200()
    {
        var seeded = _factory.Examples.Seed("Man");
        var client = CreateClient();

        var response = await client.GetAsync($"/manual/examples/{seeded.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ---- Referenced (parent route) ----

    [Fact]
    public async Task Referenced_get_by_id_resolves_parent_and_returns_200()
    {
        _factory.Children.Seed(10, "Child");
        var client = CreateClient();

        var response = await client.GetAsync("/mvc/examples/5/childExamples/10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Referenced_create_returns_201_under_parent()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/mvc/examples/5/childExamples", new { name = "Child" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains("/mvc/examples/5/childExamples/", response.Headers.Location!.ToString());
    }

    // ---- Complex key ----

    [Fact]
    public async Task ComplexKey_get_by_id_returns_200()
    {
        _factory.Complex.Seed("C1", "L1");
        var client = CreateClient();

        var response = await client.GetAsync("/mvc/exampleWithComplexKeys/C1/L1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ComplexKey_create_returns_201()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/mvc/exampleWithComplexKeys", new { code = "C2", location = "L2" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
