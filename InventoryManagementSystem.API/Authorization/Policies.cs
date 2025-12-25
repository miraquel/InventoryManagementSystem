namespace InventoryManagementSystem.API.Authorization;

public static class Policies
{
    // Counting Journals
    public const string ViewCountingJournals = "ViewCountingJournals";
    public const string CreateCountingJournals = "CreateCountingJournals";
    public const string ManageCountingJournals = "ManageCountingJournals";
    
    // Counting Journal Lines
    public const string ViewCountingJournalLines = "ViewCountingJournalLines";
    public const string CreateCountingJournalLines = "CreateCountingJournalLines";
    public const string EditCountingJournalLines = "EditCountingJournalLines";
    public const string DeleteCountingJournalLines = "DeleteCountingJournalLines";
    public const string LockUnlockJournals = "LockUnlockJournals";
    
    // Items
    public const string ViewItems = "ViewItems";
    public const string ViewOnHand = "ViewOnHand";
    
    // Locations
    public const string ViewLocations = "ViewLocations";
    
    // Warehouse Access
    public const string WarehouseAccess = "WarehouseAccess";
    
    // Admin
    public const string RequireAdminRole = "RequireAdminRole";
}

