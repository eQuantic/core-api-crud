using System.Reflection;

namespace eQuantic.Core.Api.Options;

public class DocumentationOptions
{
    public DocumentationOptions WithTitle(string title)
    {
        Title = title;
        return this;
    }

    public DocumentationOptions WithXmlCommentsFile(string fileName)
    {
        XmlCommentsFile = fileName;
        return this;
    }
    
    public DocumentationOptions FromAssembly(Assembly assembly)
    {
        Assembly = assembly;
        if (string.IsNullOrEmpty(XmlCommentsFile))
            XmlCommentsFile = $"{Assembly.GetName().Name}.xml";
        
        return this;
    }
    public string? Title { get; private set; }
    public string? XmlCommentsFile { get; private set; }
    public Assembly? Assembly { get; private set; }
}