using Microsoft.EntityFrameworkCore;
using Segway.EF.SegwayCntxt;
using Segway_Portal.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Entity Framework DbContext using the default connection name from appsettings.json
builder.Services.AddDbContext<SegwayContext>(options =>
{
    var defaultConnName = builder.Configuration["DefaultConnection"] ?? "Local";
    var connectionString = builder.Configuration.GetConnectionString(defaultConnName);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException($"Connection string '{defaultConnName}' was not found in configuration.");
    }

    options.UseSqlServer(connectionString);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
