using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Services;

// Uygulama her bilgisayarda Türkçe çalışsın; ancak enlem/boylam gibi ondalık sayılar
// formlarda ve tarayıcı tarafı doğrulamada "39.92" biçiminde (nokta ile) kullanılsın.
var kultur = new CultureInfo("tr-TR");
kultur.NumberFormat.NumberDecimalSeparator = ".";
kultur.NumberFormat.NumberGroupSeparator = "";
CultureInfo.DefaultThreadCurrentCulture = kultur;
CultureInfo.DefaultThreadCurrentUICulture = kultur;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Bağlantı dizesi depoda değil, her geliştiricinin kendi user-secrets kaydında durur
builder.Services.AddDbContext<PatiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PatiDb")));

// Mahalle sınırlarını OpenStreetMap'ten çeken servis (kendi HttpClient'ı ile)
builder.Services.AddHttpClient<SinirServisi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
