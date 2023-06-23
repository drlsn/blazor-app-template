using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MyApp.UI.Common.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options=>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme= OpenIdConnectDefaults.AuthenticationScheme;
})
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.ClientSecret = Environment.GetEnvironmentVariable("KinergizeServerClientSecret");
        options.Scope.Add("offline_access");
        options.Scope.Add(options.ClientId);
    });

//builder.Services.AddScoped<TokenProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/signin-oidc"))
    {
        context.Response.Redirect($"http://localhost:7073/signin-oidc");
        return;
    }

    await next();
});

app.Run();
