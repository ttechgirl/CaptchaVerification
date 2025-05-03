using CaptchaVerification;
using Microsoft.Extensions.DependencyInjection;
using CaptchaVerification.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CaptchaVerification.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CaptchaVerificationContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'CaptchaVerificationContextConnection' not found.");

builder.Services.AddDbContext<CaptchaVerificationContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<CaptchaVerificationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CaptchaVerificationContext>();

// Add services to the container.
builder.Services.AddScoped<CaptchaService>();

var captchaModel = new CaptchaSettings();
builder.Configuration.Bind("CaptchaSettings", captchaModel);
builder.Services.AddSingleton(captchaModel);
var baseUrl = builder.Configuration.GetValue<string>("CaptchaSettings:BaseAddress");

builder.Services.AddHttpClient<CaptchaService>(x =>
{
    x.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddRazorPages();

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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.Run();
