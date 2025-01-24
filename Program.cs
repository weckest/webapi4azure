using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudentMinimalApi.Data;
using StudentMinimalApi.Models;
using StudentMinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<SchoolDbContext>(option => option.UseSqlite(connStr));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(o => o.AddPolicy("Policy", builder => {
  builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("Policy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.RoutePrefix = "";
        options.SwaggerEndpoint("/openapi/v1.json", "My API");
    });
    app.MapScalarApiReference(options => {
        options
        .WithTitle("My WebAPI")
        .WithTheme(ScalarTheme.Moon)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

//put decluttered code here

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SchoolDbContext>();    
    context.Database.Migrate();
}

var route = app.MapGroup("/api/students");

route.MapGet("/", StudentService.GetAllStudents);
route.MapGet("/school/{school}", StudentService.GetStudentsBySchool);
route.MapGet("/{id}", StudentService.GetStudent);
route.MapPost("/", StudentService.CreateSttudent);
route.MapPut("/{id}", StudentService.UpdateStudent);
route.MapDelete("/{id}", StudentService.DeleteStudent);

app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
