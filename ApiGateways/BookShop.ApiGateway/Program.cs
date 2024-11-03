using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog.Events;
using Serilog;
using BookShop.ApiGateway.Providers;
using BookShop.ApiGateway.Models;
using BookShop.ApiGateway.Utils;

try
{
    var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Logs.json")
                .Build();

    Log.Logger = new LoggerConfiguration()
                   .ReadFrom.Configuration(config)
                   .MinimumLevel.Debug()
                   .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                   .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                   .CreateLogger();

    Log.Information("Application Starting : " + DateTime.Now.ToString());

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.RegisterServices(builder.Configuration);

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    //Registering the Models.AppSettings => appsettings.json
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

#region Ocelot Configuration

    // Excluding Ocelot Non required endpoints. Will refactor later if required
    var enpoints = OcelotNonEndpoints.OcelotNonRequiredEnpoints();
    builder.Services.AddSwaggerGen(swagger =>
    {
        swagger.DocInclusionPredicate((docName, apiDesc) =>
        {
            var routeTemplate = apiDesc.RelativePath;
            var routeMethod = apiDesc.HttpMethod;
            if (!string.IsNullOrWhiteSpace(routeTemplate) && enpoints.Where(x => routeTemplate.Contains(x.Endpoint) && x.Method.Equals(routeMethod)).Count() > 0)
                return false;
            return true;
        });
    });

    // Reading the Ocelot config for Upstream and Downstream API mapping and other configurations
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
    builder.Services.AddOcelot(builder.Configuration);

#endregion Ocelot Configuration


    var app = builder.Build();

    // Load environment variables from .env file
    Utilities.LoadEnvFile(".env");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Required to decrypt the encrypted token to get claims
    app.UseMiddleware<TokenDecryptor>();

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    // We will be needing routing to map the route with endpoints if using Ocelot with controllers
    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    // Global Common Handler to authorized request and add Headers req parameters for subsequent microservices.
    // As well as handling Exception at global level and capture any missing catch block.
    app.UseMiddleware<CommonHandler>();


#pragma warning disable ASP0014
    // Map endpoint controllers in below format to achieve Ocelot with custom controllers. Instead of app.MapControllerRoute();
    // Suppressing the warning coming for below line of code Suggesting to use top level route registrations

    app.UseEndpoints(e => e.MapControllers());
#pragma warning restore ASP0014

    app.UseOcelot().Wait();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start." + DateTime.Now.ToString());
}
finally
{
    Log.CloseAndFlush();
}