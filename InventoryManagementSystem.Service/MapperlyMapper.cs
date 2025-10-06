using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using Riok.Mapperly.Abstractions;

namespace InventoryManagementSystem.Service;

[Mapper]
public partial class MapperlyMapper
{
    // InventJournalTable
    public partial InventJournalTableDto MapToDto(GMKInventJournalTableDataContract inventJournalTable);
    public partial GMKInventJournalTableDataContract MapToDataContract(InventJournalTableDto inventJournalTableDto);
    public partial IEnumerable<InventJournalTableDto> MapToDto(IEnumerable<GMKInventJournalTableDataContract> inventJournalTables);
    public partial IEnumerable<GMKInventJournalTableDataContract> MapToDataContract(IEnumerable<InventJournalTableDto> inventJournalTableDtos);
    public partial PagedListDto<InventJournalTableDto> MapToDto(GMKInventJournalTablePagedDataContract pagedResult);
    public partial GMKInventJournalTablePagedDataContract MapToDataContract(PagedListDto<InventJournalTableDto> pagedResult);
    
    // InventJournalTrans
    public partial InventJournalTransDto MapToDto(GMKInventJournalTransDataContract inventJournalTrans);
    public partial GMKInventJournalTransDataContract MapToDataContract(InventJournalTransDto inventJournalTransDto);
    public partial IEnumerable<InventJournalTransDto> MapToDto(IEnumerable<GMKInventJournalTransDataContract> inventJournalTrans);
    public partial IEnumerable<GMKInventJournalTransDataContract> MapToDataContract(IEnumerable<InventJournalTransDto> inventJournalTransDtos);
    public partial PagedListDto<InventJournalTransDto> MapToDto(GMKInventJournalTransPagedDataContract pagedResult);
    public partial GMKInventJournalTransPagedDataContract MapToDataContract(PagedListDto<InventJournalTransDto> pagedResult);
}