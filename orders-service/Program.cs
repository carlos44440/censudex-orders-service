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
using Shared.OrderCreatedMessage;

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
        cfg.Host(Environment.GetEnvironmentVariable("RABBITMQ_HOST")?? "localhost", "/", h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")?? "guest");
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")?? "guest");
        });

        cfg.Message<OrderCreatedMessage>(e =>
        {
            e.SetEntityName("order_events");
        });

        cfg.Publish<OrderCreatedMessage>(e =>
        {
            e.ExchangeType = "topic";
        });

        cfg.Send<OrderCreatedMessage>(config =>
        {
            config.UseRoutingKeyFormatter(context => "order.created");
        });

        cfg.ReceiveEndpoint("order-failed-stock-queue", e =>
        {
            e.ConfigureConsumer<OrderFailedStockConsumer>(context);
            
            // Bind para escuchar mensajes desde order_events con order.failed.stock
            e.Bind("order_events", x =>
            {
                x.RoutingKey = "order.failed.stock";
                x.ExchangeType = "topic";
            });
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

