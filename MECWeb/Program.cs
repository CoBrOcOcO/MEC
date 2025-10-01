using Azure.Core;
using MECWeb.Components;
using MECWeb.DbModels;
using MECWeb.Localization;
using MECWeb.Services;
using Microsoft.AspNetCore.Authentication;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// HIER DEN CODE ZUM ERHÖHEN DES UPLOAD-LIMITS EINFÜGEN
// =====================================================================
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Erhöhe das Limit für den Request-Body auf ca. 2 GB
    serverOptions.Limits.MaxRequestBodySize = 2_000_000_000;
});
// =====================================================================

// Add MudBlazor services
builder.Services.AddMudServices();

// This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Configure QuestPDF
QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

// ⚠️ ENTFERNT: PDF Storage wird nicht mehr benötigt
// PDFs werden nur noch on-the-fly generiert ohne Speicherung

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Languages");
builder.Services.AddScoped<LanguageNotifier>();
builder.Services.AddScoped(typeof(IStringLocalizer<>), typeof(CultureStringLocalizer<>));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("de");
    options.AddSupportedCultures(["de", "en"]);
    options.AddSupportedUICultures(["de", "en"]);
});

// Controllers with Views
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ✅ GITEA SERVICES
builder.Services.AddHttpClient();
builder.Services.AddScoped<GiteaService>();
builder.Services.AddScoped<RepositoryValidationService>();

// Application Services
builder.Services.AddScoped<AppCardService>();
builder.Services.AddScoped<PurchaseCardService>();
builder.Services.AddScoped<EnhancedProjectService>();

builder.Services.AddSingleton(new ProjectFileService(
    builder.Configuration.GetValue<string>("ProjectBaseFolder") ??
    Path.Combine(Directory.GetCurrentDirectory(), "Projects")));

// PDF Services for Installation - ONLY GENERATION, NO STORAGE
// ⚠️ PdfStorageService ENTFERNT - wird nicht mehr benötigt
builder.Services.AddScoped<InstallationPdfGenerator>();
builder.Services.AddScoped<InstallationPdfService>();

// Add Blazor Server Side with Microsoft Identity
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Map Controllers (BEFORE UseRouting)
app.MapControllers();

app.UseHttpsRedirection();
app.UseRouting();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Map static assets and Razor components
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
        // dbContext.Database.Migrate(); // Uncomment if using migrations
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();