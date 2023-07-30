namespace eQuantic.Core.Application.Crud.Enums;

[Flags]
public enum CrudEndpointVerbs : uint
{
    OnlyGetById = 1 << 0,
    OnlyGetPaged = 1 << 1,
    OnlyCreate = 1 << 2,
    OnlyUpdate = 1 << 3,
    OnlyDelete = 1 << 4,
    OnlyReaders = OnlyGetById | OnlyGetPaged,
    OnlyWriters = OnlyCreate | OnlyUpdate | OnlyDelete,
    All = OnlyGetById | OnlyGetPaged | OnlyCreate | OnlyUpdate | OnlyDelete | OnlyReaders | OnlyWriters
}