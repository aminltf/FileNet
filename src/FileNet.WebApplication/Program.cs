using System.Globalization;
using FileNet.WebFramework;
using FileNet.WebFramework.ScanIngest;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services
    .AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddWebFramework(builder.Configuration);

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100L * 1024 * 1024; // 100MB
});
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 100L * 1024 * 1024;
});

builder.Services.Configure<ScanIngestOptions>(
    builder.Configuration.GetSection("ScanIngest"));

builder.Services.AddSingleton<ScanFileNameParser>();

builder.Services.AddHostedService<ScanIngestHostedService>();

var supportedCultures = new[] { new CultureInfo("fa-IR") };

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var locOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fa-IR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

locOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
locOptions.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");

app.UseRequestLocalization(locOptions);

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
