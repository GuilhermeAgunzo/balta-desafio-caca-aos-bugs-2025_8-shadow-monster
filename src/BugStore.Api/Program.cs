using BugStore.Api.Endpoints;
using BugStore.Application;
using BugStore.Infrastructure;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply EF Core migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

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
