using System.ComponentModel;

namespace eQuantic.Core.Application.Crud.Enums;

public enum MappingPriority
{
    [Description("Sync mapper will be applied on the destination only")]
    SyncOnly = 0,
    [Description("Sync mapper will be applied on the destination if exists, otherwise the Async mapper will be used")]
    SyncOrAsync = 1,
    [Description("Sync and async mappers will be applied on the same destination instance")]
    SyncAndAsync = 2,
    
    [Description("Async mapper will be applied on the destination only")]
    AsyncOnly = 3,
    [Description("Async mapper will be applied on the destination if exists, otherwise the sync mapper will be used")]
    AsyncOrSync = 4,
    [Description("Async and async mappers will be applied on the same destination instance")]
    AsyncAndSync = 5
}