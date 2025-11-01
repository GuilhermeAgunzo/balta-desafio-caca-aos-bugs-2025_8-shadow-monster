using BugStore.Api.Endpoints;
using BugStore.Application;
using BugStore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map all endpoints
app.MapCustomerEndpoints();
app.MapOrderEndpoints();
app.MapProductEndpoints();
app.MapReportEndpoints();

app.MapGet("/", () => "BugStore API is running!");

app.Run();
