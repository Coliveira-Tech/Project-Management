using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Extensions;
using ProjectManagement.Api.Infra.Data;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskHistoryService, TaskHistoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddMvc()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                });

string? conn = builder.Configuration.GetConnectionString("ProjectManagementDatabase");
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(conn));
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Swagger"));
    app.ApplyMigrations();
}

bool isInContainer = builder.Configuration.GetValue("DOTNET_RUNNING_IN_CONTAINER", false);

if (!isInContainer)
    app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
