using InventoryManagementSystem.API.Authorization;
using InventoryManagementSystem.API.Filters;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountingJournalsController : ControllerBase
{
    private readonly ICountingService _countingService;

    public CountingJournalsController(ICountingService countingService)
    {
        _countingService = countingService;
    }

    // GET: api/<CountingJournalsController>
    [HttpGet]
    [Authorize(Policy = Policies.ViewCountingJournals)]
    public async Task<IActionResult> Get([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var response = await _countingService.GetCountingJournalsAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/<CountingJournalsController>/5
    [HttpGet("{journalId}")]
    [BlockedJournalFilter]
    [Authorize(Policy = Policies.ViewCountingJournals)]
    public async Task<IActionResult> Get(string journalId)
    {
        var response = await _countingService.GetCountingJournalAsync(journalId);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/<CountingJournalsController>/5/summary
    [HttpGet("{journalId}/summary")]
    [Authorize(Policy = Policies.ViewCountingJournals)]
    public async Task<IActionResult> GetSummary(string journalId)
    {
        var response = await _countingService.GetCountingJournalSummaryAsync(journalId);
        return StatusCode(response.StatusCode, response);
    }

    // POST api/<CountingJournalsController>
    [HttpPost]
    [Authorize(Policy = Policies.CreateCountingJournals)]
    public async Task<IActionResult> Post([FromBody] CreateCountingJournalDto dto)
    {
        // // Check inventLocation access from user groups
        // if (!User.HasInventLocationAccess(dto.InventLocationId))
        // {
        //     return Forbid();
        // }
        
        var response = await _countingService.CreateCountingJournalAsync(dto);
        return StatusCode(response.StatusCode, response);
    }
    //
    // // PUT api/<CountingJournalsController>/5
    // [HttpPut("{id}")]
    // public void Put(int id, [FromBody] string value)
    // {
    // }
    //
    // // DELETE api/<CountingJournalsController>/5
    // [HttpDelete("{id}")]
    // public void Delete(int id)
    // {
    // }
}