using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
builder.Services.AddScoped<IReportService, ReportService>();

const string CORSPolicy = "CORSPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSPolicy,
                      policy =>
                      {

                          policy.WithOrigins("http://localhost",
                                             "https://localhost")
                                .AllowAnyHeader()
                                .AllowAnyOrigin()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddMvc()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

string? conn = builder.Configuration.GetConnectionString("ProjectManagementDatabase");
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(conn));
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Parameters ??= [];
        operation.Parameters.Add(new ()
        {
            Name = "LoggedUserId",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema()
            {
                Type = "string"
            }
        });

        operation.Parameters.Add(new ()
        {
            Name = "LoggedUserRole",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema()
            {
                Type = "string"
            }
        });

        operation.Servers.Add(new OpenApiServer()
        {
            Url = "http://localhost:8080",
            Description = "Local Http"
        });

        operation.Servers.Add(new OpenApiServer()
        {
            Url = "https://localhost:8081",
            Description = "Local Https"
        });

        return Task.CompletedTask;
    });

});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");
app.UseCors(CORSPolicy);

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
