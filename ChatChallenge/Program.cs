using ChatWebApp.Consumer;
using ChatWebApp.Data;
using ChatWebApp.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.Password.RequireNonAlphanumeric = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(opt =>
    opt.Conventions.AuthorizePage("/ChatPage")
);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>((provider) =>
{
    var connectionFactory = new ConnectionFactory
    {
        HostName = "172.17.0.2",
        VirtualHost = "/",
        UserName = "admin",
        Password = "admin",
        DispatchConsumersAsync = true
    };
    return connectionFactory;
});

builder.Services.AddHostedService<BotConsumer>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.MapRazorPages();


app.Run();
