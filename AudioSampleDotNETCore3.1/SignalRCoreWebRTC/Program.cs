using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRCoreWebRTC.Hubs;
using SignalRCoreWebRTC.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddRazorPages();
services.AddControllersWithViews();

//Cross-origin policy to accept request from localhost:8084.
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        x => x.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

services.AddSignalR();

services.AddSingleton<List<User>>();
services.AddSingleton<List<UserCall>>();
services.AddSingleton<List<CallOffer>>();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json",
                        optional: false,
                        reloadOnChange: true);
    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment}.json",
                        optional: true,
                        reloadOnChange: true);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseFileServer();
app.UseCors("CorsPolicy");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    //endpoints.MapRazorPages();
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapHub<ConnectionHub>("/ConnectionHub", options =>
    {
        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
    });
});

app.Run();

//namespace SignalRCoreWebRTC
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder
//                    .UseContentRoot(Directory.GetCurrentDirectory())
//                    .UseStartup<Startup>();
//                });
//    }
//}
