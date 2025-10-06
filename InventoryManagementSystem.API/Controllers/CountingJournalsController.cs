using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountingJournalsController : ControllerBase
{
    private readonly ICountingService _countingService;

    public CountingJournalsController(ICountingService countingService)
    {
        _countingService = countingService;
    }

    // GET: api/<CountingController>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var response = await _countingService.GetCountingJournalsAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    // GET api/<CountingController>/5
    [HttpGet("{journalId}")]
    public async Task<IActionResult> Get(string journalId)
    {
        var response = await _countingService.GetCountingJournalAsync(journalId);
        return StatusCode(response.StatusCode, response);
    }

    // // POST api/<CountingController>
    // [HttpPost]
    // public void Post([FromBody] string value)
    // {
    // }
    //
    // // PUT api/<CountingController>/5
    // [HttpPut("{id}")]
    // public void Put(int id, [FromBody] string value)
    // {
    // }
    //
    // // DELETE api/<CountingController>/5
    // [HttpDelete("{id}")]
    // public void Delete(int id)
    // {
    // }
}