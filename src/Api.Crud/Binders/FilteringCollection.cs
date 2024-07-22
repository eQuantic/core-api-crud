using System.Reflection;
using eQuantic.Core.Mvc.Binders.Filtering;
using eQuantic.Linq.Filter;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Crud.Binders;

public class FilteringCollection : List<IFiltering>
{
    public FilteringCollection() : base()
    {
    }

    public FilteringCollection(IEnumerable<IFiltering> collection) : base(collection)
    {
    }
    
    public static bool TryParse(string? value, IFormatProvider? provider,
        out FilteringCollection? filteringCollection)
    {
        if (string.IsNullOrEmpty(value))
        {
            filteringCollection = null;
            return false;
        }

        var filtering = FilteringParser.Parse(value);
        filteringCollection = new FilteringCollection(filtering);
        return true;
    }
    
    public static ValueTask<FilteringCollection?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var filterBy = context.Request.Query["filterBy"];
        var model = new List<IFiltering>();
        foreach (var value in filterBy)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }
            
            model.AddRange(FilteringParser.Parse(value));
        }
        return ValueTask.FromResult<FilteringCollection?>(new FilteringCollection(model));
    }
}