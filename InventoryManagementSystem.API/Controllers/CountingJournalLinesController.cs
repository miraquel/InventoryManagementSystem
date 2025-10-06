using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountingJournalLinesController : ControllerBase
{
    private readonly ICountingService _countingService;

    public CountingJournalLinesController(ICountingService countingService)
    {
        _countingService = countingService;
    }

    // GET: api/<CountingJournalLinesController>
    [HttpGet]
    public async Task<IActionResult> Get(int pageNumber, int pageSize, string journalId)
    {
        var response = await _countingService.GetCountingJournalLinesAsync(pageNumber, pageSize, journalId);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/<CountingJournalLinesController>/5
    [HttpGet("{inventTransId}")]
    public async Task<IActionResult> Get(string inventTransId)
    {
        var response = await _countingService.GetCountingJournalLineAsync(inventTransId);
        return StatusCode(response.StatusCode, response);
    }
    
    // // POST api/<CountingJournalLinesController>
    // [HttpPost]
    // public void Post([FromBody] string value)
    // {
    // }
    //
    // // PUT api/<CountingJournalLinesController>/5
    // [HttpPut("{id}")]
    // public void Put(int id, [FromBody] string value)
    // {
    // }
    //
    // // DELETE api/<CountingJournalLinesController>/5
    // [HttpDelete("{id}")]
    // public void Delete(int id)
    // {
    // }
}