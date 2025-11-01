using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using orders_service.Src.Data;
using orders_service.Src.Interfaces;
using orders_service.Src.Repositories;
using orders_service.Src.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 6)))
);

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseRouting();
app.UseGrpcWeb();

app.MapGrpcService<OrderGrpcService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

app.MapGet("/", () => "gRPC Orders Service running. Use Postman, BloomRPC o grpcurl para probarlo.");

app.Run();

