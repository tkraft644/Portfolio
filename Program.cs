using Portfolio.Models;
using Portfolio.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Email settings
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("EmailSettings"))
    .Validate(settings =>
        !settings.Enabled ||
        (!string.IsNullOrWhiteSpace(settings.FromAddress) &&
         !string.IsNullOrWhiteSpace(settings.FromDisplayName) &&
         !string.IsNullOrWhiteSpace(settings.Host) &&
         settings.Port > 0 &&
         !string.IsNullOrWhiteSpace(settings.User) &&
         !string.IsNullOrWhiteSpace(settings.Password)),
        "EmailSettings is enabled but incomplete.")
    .ValidateOnStart();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new[] { new CultureInfo("pl"), new CultureInfo("en") };
    options.DefaultRequestCulture = new RequestCulture("pl");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

// MVC
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// App services
builder.Services.AddSingleton<IExperienceCalculator, ExperienceCalculator>();
builder.Services.AddSingleton<IPortfolioContentService, PortfolioContentService>();
builder.Services.AddTransient<ICvEmailSender, SmtpCvEmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseStaticFiles();

// Serve Angular app under /app from wwwroot/app/browser
var angularBrowserRoot = Path.Combine(app.Environment.WebRootPath, "app", "browser");
app.MapGet("/app", () => Results.Redirect("/app/"));
if (Directory.Exists(angularBrowserRoot))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(angularBrowserRoot),
        RequestPath = "/app"
    });
}
else
{
    app.MapGet("/app/{*path}", () =>
        Results.Content(
            """
            <!doctype html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>Angular app not built</title>
              <style>
                body { font-family: system-ui, -apple-system, Segoe UI, Roboto, sans-serif; padding: 24px; max-width: 900px; margin: 0 auto; }
                code { background: #f4f4f4; padding: 2px 6px; border-radius: 4px; }
              </style>
            </head>
            <body>
              <h1>Angular app not built</h1>
              <p>This app serves Angular from <code>wwwroot/app/browser</code> under <code>/app</code>.</p>
              <p>Build it with:</p>
              <ul>
                <li><code>dotnet publish</code> (recommended for deployment)</li>
                <li><code>dotnet build -p:BuildClientAppOnBuild=true</code> (local build)</li>
              </ul>
              <p>Angular requires Node.js LTS (recommended: 22.x).</p>
              <p><a href="/">Back to MVC</a></p>
            </body>
            </html>
            """,
            "text/html"));
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Angular fallback
if (Directory.Exists(angularBrowserRoot))
{
    app.MapFallbackToFile("/app/{*path}", "index.html", new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(angularBrowserRoot)
    });
}

app.Run();
