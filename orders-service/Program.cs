using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using orders_service.Src.Data;
using orders_service.Src.Interfaces;
using orders_service.Src.Repositories;
using orders_service.Src.GrpcServices;
using orders_service.Src.Services;
using orders_service.Src.Helpers;
using MassTransit;
using ConsumerApi.Consumers;
using Shared.OrderCreatedEvent;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 6)))
);

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<UserHeaderExtractor>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderFailedStockConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Send<OrderCreatedEvent>(config =>
        {
            config.UseRoutingKeyFormatter(context => "order-created-queue");
        });

        cfg.ReceiveEndpoint("order-failed-stock-queue", e =>
        {
            e.ConfigureConsumer<OrderFailedStockConsumer>(context);
        });
    });
});

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseRouting();
app.UseGrpcWeb();

app.MapGrpcService<OrderGrpcService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

app.MapGet("/", () => "gRPC Orders Service running. Use Postman, BloomRPC o grpcurl para probarlo.");

app.Run();

