using ChatChallenge.WebApp.Consumer;
using ChatChallenge.WebApp.Data;
using ChatChallenge.WebApp.Hubs;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbit", "/", x =>
        {
            x.Username("admin");
            x.Password("admin");
        });


        cfg.ConfigureEndpoints(context);
    });

    // This will be competing consumer if the Mvc scales horizontally, which is fine because the backplane is enabled with MT
    x.AddConsumersFromNamespaceContaining<BroadcastMessageConsumer>();
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbcontext = scope.ServiceProvider.GetService<ApplicationDbContext>();
if (dbcontext != null && !dbcontext.Database.EnsureCreated())
{
    dbcontext.Database.Migrate();
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
