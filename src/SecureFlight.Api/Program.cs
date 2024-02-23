using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SecureFlight.Api.Utils;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;
using SecureFlight.Core.Services;
using SecureFlight.Infrastructure;
using SecureFlight.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SecureFlight API",
        Version = "v1",
    });
});

builder.Services.AddControllers(options => options.Filters.Add(typeof(ErrorResultFilter)));

builder.Services.AddDbContext<SecureFlightDbContext>(options => options.UseInMemoryDatabase("SecureFlight"));

builder.Services.AddScoped(typeof(IRepository<Flight>), typeof(FlightRepository));
builder.Services.AddScoped(typeof(IService<Flight>), typeof(FlightService));

builder.Services.AddScoped(typeof(IRepository<Passenger>), typeof(PassengerRepository));
builder.Services.AddScoped(typeof(IService<Passenger>), typeof(PassengerService));

builder.Services.AddScoped(typeof(IRepository<Airport>), typeof(AirportRepository));
builder.Services.AddScoped(typeof(IService<Airport>), typeof(AirportService));

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        using (var context = scope.ServiceProvider.GetRequiredService<SecureFlightDbContext>())
        {
            context.Database.EnsureCreated();
        }
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();