using Eadent.Identity.Configuration;
using NLog.Web;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Confidential/Eadent.Identity.settings.json", optional: false, reloadOnChange: false);

var services = builder.Services;

builder.Configuration.GetSection(EadentIdentitySettings.SectionName).Get<EadentIdentitySettings>();

Eadent.Identity.Startup.ConfigureServices(services);

// NLog: Setup NLog for Dependency Injection.
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();


var blockBytes = new byte[] { 0x00, 0x01, 0x02 };

SHA512 sha = SHA512.Create();
var referenceHashBytes = sha.ComputeHash(blockBytes);
var referenceHashBase64 = Convert.ToBase64String(referenceHashBytes);


app.Run();
