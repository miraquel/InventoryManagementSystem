using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Common.Logging;

/// <summary>
/// Centralized high-performance logging messages using LoggerMessage source generators.
/// These extension methods can be reused across all services.
/// </summary>
public static partial class LogMessages
{
    // ==================== Generic CRUD Operations ====================
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving {EntityType} - {IdName}: {IdValue}")]
    public static partial void LogRetrievingEntity(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving {EntityType} - Page: {PageNumber}, PageSize: {PageSize}")]
    public static partial void LogRetrievingEntitiesPaged(this ILogger logger, string entityType, int pageNumber, int pageSize);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving {EntityType} - Page: {PageNumber}, PageSize: {PageSize}, SearchQuery: {SearchQuery}")]
    public static partial void LogRetrievingEntitiesPagedWithSearch(this ILogger logger, string entityType, int pageNumber, int pageSize, string? searchQuery);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} retrieved successfully - {IdName}: {IdValue}")]
    public static partial void LogEntityRetrievedSuccessfully(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} retrieved successfully - TotalCount: {TotalCount}")]
    public static partial void LogEntitiesRetrievedSuccessfully(this ILogger logger, string entityType, int totalCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} retrieved successfully - Count: {Count}")]
    public static partial void LogEntitiesListRetrievedSuccessfully(this ILogger logger, string entityType, int count);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve {EntityType} - No response from service")]
    public static partial void LogFailedToRetrieveEntityNoResponse(this ILogger logger, string entityType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve {EntityType} - {IdName}: {IdValue}, No response from service")]
    public static partial void LogFailedToRetrieveEntityNoResponseWithId(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Information, Message = "Creating {EntityType}")]
    public static partial void LogCreatingEntity(this ILogger logger, string entityType);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} created successfully - {IdName}: {IdValue}")]
    public static partial void LogEntityCreatedSuccessfully(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} created successfully - {Message}")]
    public static partial void LogEntityCreatedSuccessfullyWithMessage(this ILogger logger, string entityType, string message);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create {EntityType} - No response from service")]
    public static partial void LogFailedToCreateEntityNoResponse(this ILogger logger, string entityType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create {EntityType} - {Message}")]
    public static partial void LogFailedToCreateEntity(this ILogger logger, string entityType, string message);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create {EntityType} - {IdName}: {IdValue}, No response from service")]
    public static partial void LogFailedToCreateEntityNoResponseWithId(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create {EntityType} - {IdName}: {IdValue}, {Message}")]
    public static partial void LogFailedToCreateEntityWithId(this ILogger logger, string entityType, string idName, string idValue, string message);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updating {EntityType} - {IdName}: {IdValue}")]
    public static partial void LogUpdatingEntity(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} updated successfully - {IdName}: {IdValue}")]
    public static partial void LogEntityUpdatedSuccessfully(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to update {EntityType} - {IdName}: {IdValue}, No response from service")]
    public static partial void LogFailedToUpdateEntityNoResponse(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to update {EntityType} - {IdName}: {IdValue}, {Message}")]
    public static partial void LogFailedToUpdateEntity(this ILogger logger, string entityType, string idName, string idValue, string message);

    [LoggerMessage(Level = LogLevel.Information, Message = "Deleting {EntityType} - {IdName}: {IdValue}")]
    public static partial void LogDeletingEntity(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Information, Message = "{EntityType} deleted successfully - {IdName}: {IdValue}")]
    public static partial void LogEntityDeletedSuccessfully(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to delete {EntityType} - {IdName}: {IdValue}, No response from service")]
    public static partial void LogFailedToDeleteEntityNoResponse(this ILogger logger, string entityType, string idName, string idValue);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to delete {EntityType} - {IdName}: {IdValue}, {Message}")]
    public static partial void LogFailedToDeleteEntity(this ILogger logger, string entityType, string idName, string idValue, string message);

    // ==================== Counting Journal Specific ====================
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating counting journal - Site: {InventSiteId}, Location: {InventLocationId}, WMSLocation: {WMSLocationId}")]
    public static partial void LogCreatingCountingJournal(this ILogger logger, string inventSiteId, string inventLocationId, string wmsLocationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Creating counting journal line - JournalId: {JournalId}, ItemId: {ItemId}, Qty: {Qty}")]
    public static partial void LogCreatingCountingJournalLine(this ILogger logger, string journalId, string itemId, decimal qty);

    [LoggerMessage(Level = LogLevel.Information, Message = "Counting journal line created successfully - JournalId: {JournalId}, ItemId: {ItemId}")]
    public static partial void LogCountingJournalLineCreatedSuccessfully(this ILogger logger, string journalId, string itemId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updating counting journal line - InventTransId: {InventTransId}, Counted: {Counted}")]
    public static partial void LogUpdatingCountingJournalLine(this ILogger logger, string inventTransId, decimal counted);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updating journal block status - JournalId: {JournalId}, LockRecord: {LockRecord}")]
    public static partial void LogUpdatingJournalBlockStatus(this ILogger logger, string journalId, bool lockRecord);

    [LoggerMessage(Level = LogLevel.Information, Message = "Journal block status updated successfully - JournalId: {JournalId}, LockRecord: {LockRecord}")]
    public static partial void LogJournalBlockStatusUpdatedSuccessfully(this ILogger logger, string journalId, bool lockRecord);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving counting journal lines - JournalId: {JournalId}, Page: {PageNumber}, PageSize: {PageSize}")]
    public static partial void LogRetrievingCountingJournalLines(this ILogger logger, string journalId, int pageNumber, int pageSize);

    [LoggerMessage(Level = LogLevel.Information, Message = "Counting journal lines retrieved successfully - JournalId: {JournalId}, TotalCount: {TotalCount}")]
    public static partial void LogCountingJournalLinesRetrievedSuccessfully(this ILogger logger, string journalId, int totalCount);

    // ==================== Inventory Specific ====================

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving on-hand inventory - Page: {PageNumber}, PageSize: {PageSize}, SearchQuery: {SearchQuery}, InventLocationId: {InventLocationId}, WMSLocationId: {WMSLocationId}")]
    public static partial void LogRetrievingOnHandInventory(this ILogger logger, int pageNumber, int pageSize, string? searchQuery, string? inventLocationId, string? wmsLocationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving on-hand information - ItemId: {ItemId}, InventBatchId: {InventBatchId}, InventSiteId: {InventSiteId}, InventLocationId: {InventLocationId}, WMSLocationId: {WMSLocationId}")]
    public static partial void LogRetrievingOnHandInfo(this ILogger logger, string itemId, string inventBatchId, string inventSiteId, string inventLocationId, string wmsLocationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "On-hand information retrieved successfully - ItemId: {ItemId}, Quantity: {Quantity}")]
    public static partial void LogOnHandInfoRetrievedSuccessfully(this ILogger logger, string itemId, decimal quantity);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving on-hand inventory locations - InventSiteId: {InventSiteId}, SearchQuery: {SearchQuery}")]
    public static partial void LogRetrievingOnHandInventoryLocations(this ILogger logger, string inventSiteId, string? searchQuery);

    [LoggerMessage(Level = LogLevel.Information, Message = "On-hand inventory locations retrieved successfully - InventSiteId: {InventSiteId}, Count: {Count}")]
    public static partial void LogOnHandInventoryLocationsRetrievedSuccessfully(this ILogger logger, string inventSiteId, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving on-hand WMS locations - InventLocationId: {InventLocationId}, Page: {PageNumber}, PageSize: {PageSize}, SearchQuery: {SearchQuery}")]
    public static partial void LogRetrievingOnHandWMSLocations(this ILogger logger, string inventLocationId, int pageNumber, int pageSize, string? searchQuery);

    [LoggerMessage(Level = LogLevel.Information, Message = "On-hand WMS locations retrieved successfully - InventLocationId: {InventLocationId}, TotalCount: {TotalCount}")]
    public static partial void LogOnHandWMSLocationsRetrievedSuccessfully(this ILogger logger, string inventLocationId, int totalCount);
    
    [LoggerMessage(Level = LogLevel.Error, Message = "Inventory batch retrieved does not match requested IDs. Requested: {itemId}/{inventBatchId}, Retrieved: {responseItemId}/{responseInventBatchId}")]
    public static partial void LogInventoryBatchRetrievedDoesNotMatchRequestedIdsRequestedItemIdInventBatchId(this ILogger logger, string itemId, string inventBatchId, string responseItemId, string responseInventBatchId);

    // ==================== Location Specific ====================

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving WMS location - WMSLocationId: {WMSLocationId}, InventLocationId: {InventLocationId}")]
    public static partial void LogRetrievingWMSLocation(this ILogger logger, string wmsLocationId, string inventLocationId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve WMS location - WMSLocationId: {WMSLocationId}, InventLocationId: {InventLocationId}, No response from service")]
    public static partial void LogFailedToRetrieveWMSLocation(this ILogger logger, string wmsLocationId, string inventLocationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving WMS locations - InventLocationId: {InventLocationId}, Page: {PageNumber}, PageSize: {PageSize}, WMSLocationId: {WMSLocationId}")]
    public static partial void LogRetrievingWMSLocationsPaged(this ILogger logger, string inventLocationId, int pageNumber, int pageSize, string? wmsLocationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "WMS locations retrieved successfully - InventLocationId: {InventLocationId}, TotalCount: {TotalCount}")]
    public static partial void LogWMSLocationsRetrievedSuccessfully(this ILogger logger, string inventLocationId, int totalCount);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "WMS location not found: {wmsLocationId} in InventLocationId: {inventLocationId}")]
    public static partial void LogWMSLocationNotFoundWmsLocationIdInInventlocationidInventlocationid(this ILogger logger, string wmsLocationId, string inventLocationId);

    // ==================== Session ====================

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieving session ID")]
    public static partial void LogRetrievingSessionId(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Session ID retrieved successfully - SessionId: {SessionId}")]
    public static partial void LogSessionIdRetrievedSuccessfully(this ILogger logger, int sessionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve session ID - No response from service")]
    public static partial void LogFailedToRetrieveSessionIdNoResponse(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to retrieve session ID")]
    public static partial void LogFailedToRetrieveSessionId(this ILogger logger, Exception exception);
}

