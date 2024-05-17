using Bookify.Web.Core.Mapping;
using Bookify.Web.DataSeeding;
using Bookify.Web.Settings;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using Bookify.Web.Data;
using Microsoft.EntityFrameworkCore;
using Bookify.Web.Helper;
using Bookify.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
	            .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(optios =>
{
    optios.Password.RequiredLength = 8;
});
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,ApplicationUserClaimsFactory>();
builder.Services.AddTransient<IDocumentSettings,DocumentSettings>();
// Using nameof(CloudinarySettings) Instead Of "CloudinarySettings" In Case [The Class & AppSettings Section Having The Same Name]
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
builder.Services.AddExpressiveAnnotations();
var app = builder.Build();

// To Apply Dependency Injection In Program Class
var scopeProvider = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope   = scopeProvider.CreateScope();
var rolemaneger   = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var usermaneger   = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
await DefaultRoles.SeedRolesAsync(rolemaneger);
await DefaultUsers.SeedAdminUserAsync(usermaneger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

