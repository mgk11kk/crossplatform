using Auth0.AspNetCore.Authentication;
using Auth0.ManagementApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);


// Configure Auth0
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.Scope = "openid profile email phone";
});
// Register HttpClient as a service using IHttpClientFactory
builder.Services.AddHttpClient();

// Register Auth0TokenService as a Singleton
builder.Services.AddSingleton<Auth0TokenService>();

// Register IManagementApiClient as a Singleton
builder.Services.AddSingleton<IManagementApiClient>(sp =>
{
    // Fetch the management API token from Auth0TokenService
    var tokenService = sp.GetRequiredService<Auth0TokenService>();
    var token = tokenService.GetManagementApiTokenAsync().Result;

    // Replace with your actual Auth0 domain
    var domain = builder.Configuration["Auth0:Domain"];

    return new ManagementApiClient(token, new Uri($"https://{domain}/api/v2"));
});


// Add services to the container.
builder.Services.AddControllersWithViews();
// Add authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();