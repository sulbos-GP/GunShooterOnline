using AuthenticationServer.Models;
using AuthenticationServer.Repository;
using AuthenticationServer.Service;
using System;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.Configure<GoogleConfig>(configuration.GetSection(nameof(GoogleConfig)));

// Add services to the container.
builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddTransient<IService, GoogleService>();

builder.Services.AddSingleton<IMemoryDB, MemoryDB>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
