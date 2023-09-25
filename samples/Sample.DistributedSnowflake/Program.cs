using Lycoris.Snowflakes;
using Microsoft.AspNetCore.Mvc;
using Sample.DistributedSnowflake;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDistributedSnowflake().AddSnowflakesRedisHelper<DistributedSnowflakesRedis>().AsHelper();

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{

    Console.WriteLine(DistributedSnowflakeHelper.GetNextId()); 

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}