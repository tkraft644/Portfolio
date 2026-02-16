using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models.Settings;
using Portfolio.Observability;
using Portfolio.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var isEfDesignTime = AppDomain.CurrentDomain.GetAssemblies()
    .Any(x => string.Equals(x.GetName().Name, "Microsoft.EntityFrameworkCore.Design", StringComparison.Ordinal));

builder.WebHost.ConfigureKestrel(options => { options.AddServerHeader = false; });

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

builder.Services.AddOptions<AdminSettings>()
    .Bind(builder.Configuration.GetSection("Admin"))
    .Validate(settings => !settings.Enabled || !string.IsNullOrWhiteSpace(settings.Password), "Admin is enabled but password is missing.")
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

// Observability
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddCheck<CvPdfHealthCheck>("cv_pdf", tags: ["ready"]);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestPropertiesAndHeaders |
        HttpLoggingFields.ResponsePropertiesAndHeaders |
        HttpLoggingFields.Duration;
});

// Security
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
        options.Cookie.Name = "portfolio_admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("admin", "true"));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("contact", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
});

// App services
builder.Services.AddSingleton<IExperienceCalculator, ExperienceCalculator>();
builder.Services.AddTransient<ICvEmailSender, SmtpCvEmailSender>();

var portfolioDbConnectionString = builder.Configuration.GetConnectionString("Portfolio");
if (!string.IsNullOrWhiteSpace(portfolioDbConnectionString))
{
    builder.Services.AddDbContext<PortfolioDbContext>(options =>
        options.UseSqlServer(
            portfolioDbConnectionString,
            sql => sql.EnableRetryOnFailure()));

    builder.Services.AddScoped<PortfolioDbMigrator>();
    builder.Services.AddScoped<PortfolioDbSeeder>();
    builder.Services.AddScoped<IPortfolioContentService, EfCorePortfolioContentService>();
}
else
{
    builder.Services.AddScoped<IPortfolioContentService, PortfolioContentService>();
}

var app = builder.Build();

if (!isEfDesignTime &&
    !app.Environment.IsEnvironment("Testing") &&
    !string.IsNullOrWhiteSpace(portfolioDbConnectionString))
{
    using var scope = app.Services.CreateScope();
    var migrator = scope.ServiceProvider.GetRequiredService<PortfolioDbMigrator>();
    await migrator.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<PortfolioDbSeeder>();
    await seeder.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";

        var csp = BuildCspHeaderValue(context.Request.IsHttps);
        context.Response.Headers["Content-Security-Policy"] = csp;

        return Task.CompletedTask;
    });

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthResponse
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Name == "self",
    ResponseWriter = WriteHealthResponse
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
});

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

app.UseRateLimiter();
app.UseAuthentication();
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

if (!isEfDesignTime)
{
    app.Run();
}

static Task WriteHealthResponse(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var payload = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description
        })
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
    {
        WriteIndented = true
    }));
}

static string BuildCspHeaderValue(bool isHttps)
{
    var directives = new List<string>
    {
        "default-src 'self'",
        "base-uri 'self'",
        "object-src 'none'",
        "frame-ancestors 'none'",
        "form-action 'self'",
        "img-src 'self' data: https:",
        "font-src 'self' data:",
        "connect-src 'self'",
        // Inline styles are needed (MVC uses small inline style attrs; Angular also sets styles dynamically).
        "style-src 'self' 'unsafe-inline'",
        "script-src 'self'"
    };

    if (isHttps)
    {
        directives.Add("upgrade-insecure-requests");
    }

    return string.Join("; ", directives);
}

public partial class Program { }
