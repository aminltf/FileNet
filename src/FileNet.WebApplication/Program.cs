using FileNet.WebFramework;
using FileNet.WebFramework.ScanIngest;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
