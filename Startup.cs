using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Data;
using NuGet.ContentModel;
using Microsoft.AspNetCore.Builder;
using BookStore.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreContext") ?? throw new InvalidOperationException("Connection string 'BookStoreContext' not found.")));

// Add services to the container.
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class StartUp
{
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<Cart>(sp => Cart.GetCart(sp));
        services.AddControllersWithViews();
        services.AddDbContext<BookStoreContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString(name: "BookStoreContext")));

        services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
        services.AddScoped<Cart>(sp => Cart.GetCart(sp));
        
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromSeconds(10);
        });
    }
    public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(errorHandlingPath: "/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthorization();
        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Store}/{action=Index}/{id?}");
        });
    }
}
