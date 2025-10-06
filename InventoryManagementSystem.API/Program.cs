using InventoryManagementSystem.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInventoryManagementService();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.MapGet("/InventJournalTable", async ([FromQuery] string journalId, GMKInventoryManagementService service) =>
//     {
//         var request = new GMKInventoryManagementServiceGetInventJournalTableRequest
//         {
//             CallContext = new CallContext
//             {
//                 Company = "GMK"
//             },
//             parm = new GMKInventJournalTableDataContract
//             {
//                 JournalId = journalId
//             }
//         };
//         var result = await service.getInventJournalTableAsync(request);
//         
//         return result;
//     })
//     .WithName("GetInventJournalTable");
//
//
// // create parameters for createCountingJournal
// app.MapPost("/CreateCountingJournal",
//         async ([FromBody] CreateCountingJournalDto dto, GMKInventoryManagementService service) =>
//         {
//             var request = new GMKInventoryManagementServiceCreateCountingJournalRequest
//             {
//                 CallContext = new CallContext
//                 {
//                     Company = "GMK"
//                 },
//                 parm = new GMKInventJournalTransDataContract
//                 {
//                     InventSiteId = dto.InventSiteId,
//                     InventLocationId = dto.InventLocationId,
//                     WMSLocationId = dto.WMSLocationId,
//                 }
//             };
//             var result = await service.createCountingJournalAsync(request);
//
//             return result;
//         })
//     .WithName("CreateCountingJournal")
//     .DisableRequestTimeout();
// //.WithRequestTimeout(TimeSpan.FromMinutes(5));

app.Run();