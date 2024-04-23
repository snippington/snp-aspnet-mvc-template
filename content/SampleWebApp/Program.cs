using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.Data;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add identity roles
builder.Services.AddDefaultIdentity<IdentityUser>(
        options => {options.SignIn.RequireConfirmedAccount = true;}
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(43800);
        options.LoginPath = "/Account/Login"; // Set here your login path
        options.LogoutPath = "/Account/Logout"; // Set here your logout path
        options.AccessDeniedPath = "/Account/AccessDenied"; // Set here your access denied path
        options.SlidingExpiration = true;
});
builder.Services.AddControllersWithViews();

// Use lower case URLs only
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Toggle to Start Logging
//builder.Services.AddOpenTelemetry()
//    .WithMetrics(metricsProviderBuilder =>
//        metricsProviderBuilder
//        .ConfigureResource(resource => resource
//            .AddService(DiagnosticsConfig.ServiceName)
//        )
//    )
//    .WithTracing(tracingProviderBuilder =>
//        tracerProviderBuilder
//            .AddSource(DiagnosticsConfig.ActivitySource.Name)
//            .ConfigureResource(resource => resource
//                .AddService(DiagnosticsConfig.ServiceName)
//            )
//    .AddAspNetCoreInstrumentation()
//    .AddConsoleExporter());


var app = builder.Build();

// Configure the HTTP Request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();