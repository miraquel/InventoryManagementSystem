using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using Riok.Mapperly.Abstractions;

namespace InventoryManagementSystem.Service;

[Mapper(ThrowOnMappingNullMismatch = false)]
public partial class MapperlyMapper
{
    // InventJournalTable
    [MapperIgnoreTarget(nameof(InventJournalTableDto.InventJournalTrans))]
    public partial InventJournalTableDto MapToDto(GMKInventJournalTableDataContract inventJournalTable);
    
    [MapperIgnoreSource(nameof(InventJournalTableDto.InventJournalTrans))]
    public partial GMKInventJournalTableDataContract MapToDataContract(InventJournalTableDto inventJournalTableDto);
    
    [MapperIgnoreSource(nameof(InventJournalTableDto.InventJournalTrans))]
    public partial IEnumerable<InventJournalTableDto> MapToDto(IEnumerable<GMKInventJournalTableDataContract> inventJournalTables);
    
    [MapperIgnoreSource(nameof(InventJournalTableDto.InventJournalTrans))]
    public partial IEnumerable<GMKInventJournalTableDataContract> MapToDataContract(IEnumerable<InventJournalTableDto> inventJournalTableDtos);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<InventJournalTableDto> MapToDto(GMKInventJournalTablePagedDataContract pagedResult);
    
    [MapperIgnoreSource(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.Columns))]
    public partial GMKInventJournalTablePagedDataContract MapToDataContract(PagedListDto<InventJournalTableDto> pagedResult);
    
    // InventJournalTrans
    public partial InventJournalTransDto MapToDto(GMKInventJournalTransDataContract inventJournalTrans);
    public partial GMKInventJournalTransDataContract MapToDataContract(InventJournalTransDto inventJournalTransDto);
    public partial IEnumerable<InventJournalTransDto> MapToDto(IEnumerable<GMKInventJournalTransDataContract> inventJournalTrans);
    public partial IEnumerable<GMKInventJournalTransDataContract> MapToDataContract(IEnumerable<InventJournalTransDto> inventJournalTransDtos);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<InventJournalTransDto> MapToDto(GMKInventJournalTransPagedDataContract pagedResult);
    
    [MapperIgnoreSource(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.Columns))]
    public partial GMKInventJournalTransPagedDataContract MapToDataContract(PagedListDto<InventJournalTransDto> pagedResult);
    
    // CountingJournalSummary
    public partial CountingJournalSummaryDto MapToDto(GMKCountingJournalSummaryDataContract summary);
    public partial GMKCountingJournalSummaryDataContract MapToDataContract(CountingJournalSummaryDto summaryDto);
    
    // InventLocation
    public partial InventLocationDto MapToDto(GMKInventLocationDataContract inventLocation);
    public partial GMKInventLocationDataContract MapToDataContract(InventLocationDto inventLocationDto);
    public partial List<InventLocationDto> MapToInventLocationDtoList(GMKInventLocationDataContract[] inventLocations);
    
    // WMSLocation
    public partial WMSLocationDto MapToDto(GMKWMSLocationDataContract wmsLocation);
    public partial GMKWMSLocationDataContract MapToDataContract(WMSLocationDto wmsLocationDto);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<WMSLocationDto> MapToDto(GMKWMSLocationPagedListDataContract pagedResult);
    
    // InventTable
    public partial InventTableDto MapToDto(GMKInventTableDataContract inventTable);
    public partial GMKInventTableDataContract MapToDataContract(InventTableDto inventTableDto);
    public partial IEnumerable<InventTableDto> MapToDto(IEnumerable<GMKInventTableDataContract> inventTables);
    public partial IEnumerable<GMKInventTableDataContract> MapToDataContract(IEnumerable<InventTableDto> inventTableDtos);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<InventTableDto> MapToDto(GMKInventTablePagedListDataContract pagedResult);
    
    [MapperIgnoreSource(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.Columns))]
    public partial GMKInventTablePagedListDataContract MapToDataContract(PagedListDto<InventTableDto> pagedResult);
    
    // OnHand
    public partial OnHandDto MapToDto(GMKOnHandDataContract onHand);
    public partial GMKOnHandDataContract MapToDataContract(OnHandDto onHandDto);
    public partial IEnumerable<OnHandDto> MapToDto(IEnumerable<GMKOnHandDataContract> onHands);
    public partial IEnumerable<GMKOnHandDataContract> MapToDataContract(IEnumerable<OnHandDto> onHandDtos);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<OnHandDto> MapToDto(GMKOnHandPagedListDataContract pagedResult);
    
    [MapperIgnoreSource(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.Columns))]
    public partial GMKOnHandPagedListDataContract MapToDataContract(PagedListDto<OnHandDto> pagedResult);
    
    public partial List<InventLocationDto> MapToInventLocationDtoListFromOnHand(GMKInventLocationDataContract[] inventLocations);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<WMSLocationDto> MapToWMSLocationDtoFromOnHand(GMKWMSLocationPagedListDataContract pagedResult);
    
    // InventBatch
    public partial InventBatchDto MapToDto(GMKBatchDataContract inventBatch);
    public partial GMKBatchDataContract MapToDataContract(InventBatchDto inventBatchDto);
    public partial IEnumerable<InventBatchDto> MapToDto(IEnumerable<GMKBatchDataContract> inventBatches);
    public partial IEnumerable<GMKBatchDataContract> MapToDataContract(IEnumerable<InventBatchDto> inventBatchDtos);
    
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreTarget(nameof(PagedListDto<>.Columns))]
    public partial PagedListDto<InventBatchDto> MapToDto(GMKBatchPagedListDataContract pagedResult);
    
    [MapperIgnoreSource(nameof(PagedListDto<>.HasPreviousPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.HasNextPage))]
    [MapperIgnoreSource(nameof(PagedListDto<>.Columns))]
    public partial GMKBatchPagedListDataContract MapToDataContract(PagedListDto<InventBatchDto> pagedResult);
}