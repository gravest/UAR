using UAR.Web.Data;
using UAR.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<DropdownService>();
builder.Services.AddScoped<ApprovalEmailService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<UserLookupService>();
builder.Services.AddScoped<ProgramLookupService>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.MapPost("/approvals/manager/{id:int}/approve", async (
    int id,
    HttpRequest httpRequest,
    RequestService requestService,
    ApprovalEmailService approvalEmailService) =>
{
    var request = await requestService.GetByIdAsync(id);
    if (request is null)
    {
        return Results.NotFound();
    }

    if (RdoApprovalEvaluator.RequiresRdoApproval(request))
    {
        if (string.IsNullOrWhiteSpace(request.RdoApprover))
        {
            return Results.BadRequest("RDO Approver is required before manager approval.");
        }

        request.Status = ApprovalWorkflow.PendingRdoApprovalStatus;
        await requestService.UpdateAsync(request);

        var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";
        await approvalEmailService.SendRdoApprovalRequestAsync(request, baseUrl);

        return Results.Ok(request);
    }

    request.Status = ApprovalWorkflow.ApprovedStatus;
    await requestService.UpdateAsync(request);
    return Results.Ok(request);
});

app.MapPost("/approvals/manager/{id:int}/reject", async (int id, RequestService requestService) =>
{
    var request = await requestService.GetByIdAsync(id);
    if (request is null)
    {
        return Results.NotFound();
    }

    request.Status = ApprovalWorkflow.RejectedStatus;
    await requestService.UpdateAsync(request);
    return Results.Ok(request);
});

app.MapPost("/approvals/rdo/{id:int}/approve", async (int id, RequestService requestService) =>
{
    var request = await requestService.GetByIdAsync(id);
    if (request is null)
    {
        return Results.NotFound();
    }

    if (!RdoApprovalEvaluator.RequiresRdoApproval(request))
    {
        return Results.BadRequest("RDO approval is not required for this request.");
    }

    request.Status = ApprovalWorkflow.ApprovedStatus;
    await requestService.UpdateAsync(request);
    return Results.Ok(request);
});

app.MapPost("/approvals/rdo/{id:int}/reject", async (int id, RequestService requestService) =>
{
    var request = await requestService.GetByIdAsync(id);
    if (request is null)
    {
        return Results.NotFound();
    }

    request.Status = ApprovalWorkflow.RejectedStatus;
    await requestService.UpdateAsync(request);
    return Results.Ok(request);
});

app.Run();
