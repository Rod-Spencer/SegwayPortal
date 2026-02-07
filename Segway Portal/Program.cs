using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Segway.EF.SegwayCntxt;
using Segway.Service.LoggerHelper;
using Segway_Portal.Components;
using Segway_Portal.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var logger = Logger_Helper.GetCurrentLogger();


// Configure Entity Framework DbContext using the default connection name from appsettings.json
builder.Services.AddDbContext<SegwayContext>(options =>
{
    var defaultConnName = builder.Configuration["DefaultConnection"] ?? "Local";
    logger.Debug($"Default Connection Name: {defaultConnName}");

    var connectionString = builder.Configuration.GetConnectionString(defaultConnName);
    logger.Debug($"Connection String: {connectionString}");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException($"Connection string '{defaultConnName}' was not found in configuration.");
    }

    options.UseSqlServer(connectionString);
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<UserService_Interface, UserService>();
builder.Services.AddHttpContextAccessor();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/account/login", async (HttpContext context, UserService_Interface userService) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var user = await userService.ValidateUserAsync(username, password);

    if (user == null)
    {
        context.Response.Redirect("/login?error=invalid");
        return;
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.User_Name),
        new Claim("UserId", user.ID.ToString())
    };

    var identity = new ClaimsIdentity(claims, "Cookies");
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync("Cookies", principal);
    context.Response.Redirect("/Tools");
});

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("Cookies");
    context.Response.Redirect("/");
});

app.Run();
