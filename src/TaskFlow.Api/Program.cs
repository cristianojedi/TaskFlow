using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;
using TaskFlow.Api.Endpoints;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console(new CompactJsonFormatter()));

var connectionString = builder.Configuration.GetConnectionString("TaskFlowDb") ?? "Data Source=taskflow.db";

builder.Services.AddDbContext<TaskFlowDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TaskFlowDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapTaskEndpoints();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
