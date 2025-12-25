using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagementSystem.API.Filters;

public class BlockedJournalFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get the journalId from route parameters
        if (!context.ActionArguments.TryGetValue("journalId", out var journalIdObj) || journalIdObj is not string journalId)
        {
            await next();
            return;
        }

        // Get the counting service from DI
        var countingService = context.HttpContext.RequestServices.GetService<ICountingService>();
        if (countingService == null)
        {
            await next();
            return;
        }

        // Fetch the journal to check BlockUserId
        var response = await countingService.GetCountingJournalAsync(journalId);
        
        if (response is not ServiceResponse<InventJournalTableDto> { Data: { } journal })
        {
            await next();
            return;
        }

        // If JournalSessionId is not set or is "0", the journal is not considered locked.
        if (string.IsNullOrWhiteSpace(journal.JournalSessionId) || journal.JournalSessionId == "0")
        {
            await next();
            return;
        }

        var sessionResponse = await countingService.GetSessionIdAsync();
        if (sessionResponse is ServiceResponse<int> { IsSuccess: true, Data: var sessionId } &&
            int.TryParse(journal.JournalSessionId, out var journalSessionId) &&
            journalSessionId == sessionId)
        {
            await next();
            return;
        }

        // Journal is blocked by a different session
        var serviceResponse = ServiceResponse.Failure("This journal is currently being edited by another user session.", StatusCodes.Status423Locked);
        serviceResponse.Errors.Add($"Journal Session ID: {journal.JournalSessionId}");
        serviceResponse.Errors.Add($"Blocked By: {journal.BlockUserId}");

        context.Result = new ObjectResult(serviceResponse)
        {
            StatusCode = StatusCodes.Status423Locked
        };
    }
}
