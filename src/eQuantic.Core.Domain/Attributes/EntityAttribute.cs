namespace eQuantic.Core.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EntityAttribute : Attribute
{
    public EntityAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", nameof(name));
        }
        
        Name = name;
    }
    
    public string Name { get; }
}