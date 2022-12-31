using Infrastructure.Messaging.RabbitMq;
using Infrastructure.Telemetry;
using OpenTelemetry.Trace;
using WebClient;
using WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging
    .ClearProviders()
    .AddOpenTelemetryLogging();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables();

var rabbitMqConfig = new RabbitMqConfiguration();
builder.Configuration.GetSection(nameof(RabbitMqConfiguration)).Bind(rabbitMqConfig);
var webClientConfiguration = new WebClientConfiguration();
builder.Configuration.GetSection(nameof(WebClientConfiguration)).Bind(webClientConfiguration);

builder.Services
    .InstallRabbitMqInfrastructure()
    .AddSingleton(rabbitMqConfig)
    .AddSingleton(webClientConfiguration)
    .AddSingleton<IPingRepository, InMemoryPingRepository>()
    .AddTransient<IWorkRequestPublisherService, WorkRequestPublisherService>()
    .AddHostedService<WorkResponseConsumerHostedService>();

builder.Services.AddOpenTelemetryStack(
    builder.Environment.EnvironmentName,
    b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
