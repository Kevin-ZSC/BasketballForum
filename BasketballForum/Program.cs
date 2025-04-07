using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BasketballForum.Data;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BasketballForumContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BasketballForumContext") ?? throw new InvalidOperationException("Connection string 'BasketballForumContext' not found.")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<BasketballForumContext>();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<CloudinaryService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages().WithStaticAssets();
app.Run();
