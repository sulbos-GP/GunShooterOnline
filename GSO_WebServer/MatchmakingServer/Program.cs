using MatchmakingServer.Background;
using MatchmakingServer.Models;
using MatchmakingServer.Repository;
using System;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.Configure<TestConfig>(configuration.GetSection(nameof(TestConfig)));

// Add services to the container.
builder.Services.AddTransient<IGameDB, GameDB>();

// Add backgroud services to the container.
builder.Services.AddHostedService<BackgroundMatchMaking>();

builder.Services.AddSingleton<IMatchingQueue, MemoryMatchingQueue>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
