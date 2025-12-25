using InventoryManagementSystem.API.Authorization;
using InventoryManagementSystem.API.Filters;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

[Route("api/CountingJournals/{journalId}/lines")]
[ApiController]
[BlockedJournalFilter]
[Authorize]
public class CountingJournalLinesController : ControllerBase
{
    private readonly ICountingService _countingService;

    public CountingJournalLinesController(ICountingService countingService)
    {
        _countingService = countingService;
    }

    // GET: api/CountingJournals/{journalId}/lines
    [HttpGet]
    [Authorize(Policy = Policies.ViewCountingJournalLines)]
    public async Task<IActionResult> Get(
        [FromQuery] int pageNumber, 
        [FromQuery] int pageSize, 
        string journalId,
        [FromQuery] string? itemId = null,
        [FromQuery] string? inventBatchId = null)
    {
        var response = await _countingService.GetCountingJournalLinesAsync(pageNumber, pageSize, journalId, itemId, inventBatchId);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/CountingJournals/{journalId}/lines/{inventTransId}
    [HttpGet("{inventTransId}")]
    [Authorize(Policy = Policies.ViewCountingJournalLines)]
    public async Task<IActionResult> Get(string journalId, string inventTransId)
    {
        var response = await _countingService.GetCountingJournalLineAsync(inventTransId);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/CountingJournals/{journalId}/lines/batch
    [HttpGet("batch")]
    [Authorize(Policy = Policies.ViewCountingJournalLines)]
    public async Task<IActionResult> GetByBatch(
        string journalId, 
        [FromQuery] string itemId,
        [FromQuery] string inventSiteId,
        [FromQuery] string inventLocationId,
        [FromQuery] string wmsLocationId,
        [FromQuery] string inventBatchId)
    {
        var response = await _countingService.GetInventJournalTransByBatchAsync(
            journalId, itemId, inventSiteId, inventLocationId, wmsLocationId, inventBatchId);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/CountingJournals/{journalId}/lines
    [HttpPost]
    [Authorize(Policy = Policies.CreateCountingJournalLines)]
    public async Task<IActionResult> Post(string journalId, [FromBody] CreateCountingJournalLineDto dto)
    {
        // Ensure the journalId from the route matches the DTO
        if (journalId != dto.JournalId)
        {
            return BadRequest(new { message = "JournalId in the URL does not match the DTO." });
        }

        // // Check inventLocation access from user groups
        // if (!User.HasInventLocationAccess(dto.InventLocationId))
        // {
        //     return Forbid();
        // }

        dto.JournalId = journalId;

        var response = await _countingService.CreateCountingJournalLineAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    // PUT api/CountingJournals/{journalId}/lines/{inventTransId}
    [HttpPut("{inventTransId}")]
    [Authorize(Policy = Policies.EditCountingJournalLines)]
    public async Task<IActionResult> Put(string journalId, string inventTransId, [FromBody] UpdateCountingJournalLineDto dto)
    {
        // Ensure the inventTransId from the route matches the DTO
        if (inventTransId != dto.InventTransId)
        {
            return BadRequest(new { message = "InventTransId in the URL does not match the DTO." });
        }

        dto.InventTransId = inventTransId;

        var response = await _countingService.UpdateCountingJournalLineAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/CountingJournals/{journalId}/lines/{inventTransId}
    [HttpDelete("{inventTransId}")]
    [Authorize(Policy = Policies.DeleteCountingJournalLines)]
    public async Task<IActionResult> Delete(string journalId, string inventTransId)
    {
        var response = await _countingService.DeleteCountingJournalLineAsync(inventTransId);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/CountingJournals/{journalId}/lines/lock
    [HttpPost("lock")]
    [Authorize(Policy = Policies.LockUnlockJournals)]
    public async Task<IActionResult> LockJournal(string journalId)
    {
        var response = await _countingService.UpdateBlockInventJournalTableAsync(journalId, true);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE api/CountingJournals/{journalId}/lines/lock
    [HttpDelete("lock")]
    [Authorize(Policy = Policies.LockUnlockJournals)]
    public async Task<IActionResult> UnlockJournal(string journalId)
    {
        var response = await _countingService.UpdateBlockInventJournalTableAsync(journalId, false);
        return StatusCode(response.StatusCode, response);
    }
}