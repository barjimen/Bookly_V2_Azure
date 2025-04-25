using System.Configuration;
using Microsoft.EntityFrameworkCore;
using StoryConnect.Context;
using StoryConnect.Repositories;
using StoryConnect_V2.Helper;
using StoryConnect_V2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});


// Add services to the container.
//string connectionString = builder.Configuration.GetConnectionString("StorySQL");
builder.Services.AddDbContext<StoryContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("StorySQL")));
builder.Services.AddTransient<BooklyService>();
builder.Services.AddTransient<IRepositoryLibros, RepositoryLibros>();
builder.Services.AddTransient<HelperImages>();
//builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
