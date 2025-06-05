using System.Reflection;
using LeaveRequestSample;
using LeaveRequestSample.Models;
using LeaveRequestSample.Services;
using LeaveRequestSample.Workflows;
using Meridian.Application.Interfaces;
using Meridian.Infrastructure.WorkflowDI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMeridianWorkflow(options =>
{
    options.SetFileStorageProvider(typeof(StorageService));
    options.ConfigureDb(config =>
        config.Use(dbOptions => dbOptions.UseInMemoryDatabase("WorkflowTestDb")));

    options.Workflows =
    [
        new LeaveRequestWorkflow(),
    ];
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
var currentUser = "admin";
var roles = new List<string> { "supervisor", "sectionHead", "hr" };
app.MapPost("/create-request",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromBody] LeaveRequestData requestData) =>
        Results.Ok(await workflow.CreateRequestAsync(requestData, currentUser)))
    .WithName("CreateLeaveRequest")
    .WithOpenApi();

app.MapGet("/get-request/{id:guid}",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromRoute] Guid id) =>
        Results.Ok(await workflow.GetRequestAsync(id)))
    .WithName("CreateRequest")
    .WithOpenApi();

app.MapGet("/get-request-with-history/{id:guid}",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromRoute] Guid id) =>
        Results.Ok(await workflow.GetRequestWithHistoryAsync(id)))
    .WithName("CreateRequestWithHistory")
    .WithOpenApi();

app.MapPost("/do-action/{actionName}/{requestId:guid}",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromRoute] string actionName,
                [FromRoute] Guid requestId) =>
            Results.Ok(await workflow.DoActionAsync(requestId, actionName, currentUser, roles, [])))
    .WithName("DoActionWithoutData")
    .WithOpenApi();

app.MapPost("/do-action-data/{actionName}/{requestId:guid}",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromRoute] string actionName,
                [FromRoute] Guid requestId, [FromBody] LeaveRequestData requestData) =>
            Results.Ok(await workflow.DoActionAsync(requestId, actionName, currentUser, roles, [], requestData)))
    .WithName("DoActionWithData")
    .WithOpenApi();

app.MapPost("/get-history/{requestId:guid}",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromRoute] Guid requestId) =>
            Results.Ok(await workflow.GetRequestHistoryAsync(requestId)))
    .WithName("GetRequestHistory")
    .WithOpenApi();

app.MapPost("/get-available-actions/{requestId:guid}",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow, [FromRoute] Guid requestId) =>
        Results.Ok(await workflow.GetAvailableActions(requestId, currentUser, roles, [])))
    .WithName("GetRequestRequestActions")
    .WithOpenApi();

app.MapPost("/my-tasks",
        async ([FromServices] IWorkflowService<LeaveRequestData> workflow) =>
        Results.Ok(await workflow.GetUserTasksAsync(currentUser, roles, [])))
    .WithName("GetLoggedInUserTasks")
    .WithOpenApi();

app.Run();