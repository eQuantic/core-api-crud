namespace eQuantic.Core.Application.Crud.Options;

public class ReadOptions
{
    public bool OnlyOwner { get; set; }
    public string[] Roles { get; set; } = [];
}