using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Customers.Admin.Components;
using Customers.Admin.Models;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Customers.Admin;
using Microsoft.OData.ModelBuilder;
using Customers.Data.Contracts;
using Customers.Data.Repositories;
using Customers.Services.Contracts;
using Customers.Services;
using Customers.Services.Services;
using Customers.Data;
using Customers.Data.Models;
using Customers.Infrastructure.Mapping;
using Customers.Admin.Services;
using Customers.Admin.Components.Pages.Account;
using Customers.Infrastructure.Helper;
using System.Configuration;
//using Serilog.Sinks.Console;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 1024 * 1024);

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ErrorDialogService>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailHelper>();

builder.Services.AddDbContextFactory<CustomersDBContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("CustomersDB"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("CustomersDB")));
}, ServiceLifetime.Transient);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<CustomersDBContext>().AddDefaultTokenProviders();
builder.Services.AddAutoMapper(typeof(ProfileMapping));
builder.Services.AddHttpClient("Customers.User").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<DateTimeHelper>()
                .AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>()
                .AddScoped<SecurityService>()
                .AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<ICollectionService, CollectionService>()
    .AddTransient<IKnownWordService, KnownWordService>()
    .AddTransient<IUserService, UserService>()
    .AddTransient<ICollectionSOWService, CollectionSOWService>()
    .AddTransient<ICollectionProposalService, CollectionProposalService>()
    .AddTransient<ICollectionAnalysisService, CollectionAnalysisService>()
    .AddTransient<IAnalysisDetailsService, AnalysisDetailsService>()
    .AddTransient<ICustomerService, CustomerService>()
    .AddTransient<IActivityLogService, ActivitiyLogService>()
    .AddTransient<LicenseService>();

builder.Services.AddSingleton<OtpService>();
builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: "Data Source=.;Initial Catalog=LearningDB;Integrated Security=true;TrustServerCertificate=true;",
        sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents" })
    .CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();