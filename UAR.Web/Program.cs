using UAR.Web.Data;
using UAR.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<DropdownService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<UserLookupService>();
builder.Services.AddScoped<ProgramLookupService>();

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

app.Run();
