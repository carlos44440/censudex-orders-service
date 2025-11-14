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

/// <summary>
/// Carga las variables de entorno desde el archivo .env.
/// </summary>
Env.Load();

/// <summary>
/// Configuración del contexto de base de datos MySQL para el servicio de órdenes.
/// </summary>
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 6)))
);

/// <summary>
/// Registro de servicios internos: repositorio, servicio de correos y extractor de headers.
/// </summary>
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<UserHeaderExtractor>();

/// <summary>
/// Configuración de MassTransit con RabbitMQ para el procesamiento de eventos.
/// </summary>
builder.Services.AddMassTransit(x =>
{
    // Registro del consumidor encargado de procesar eventos de stock fallido.
    x.AddConsumer<OrderFailedStockConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // Configuración del host de RabbitMQ (URL, usuario y contraseña).
        cfg.Host(Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost", "/", h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest");
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest");
        });

        // Configuración del mensaje OrderCreatedMessage para publicarse en el exchange "order_events".
        cfg.Message<OrderCreatedMessage>(e =>
        {
            e.SetEntityName("order_events");
        });

        // Publicación del mensaje usando exchange tipo topic.
        cfg.Publish<OrderCreatedMessage>(e =>
        {
            e.ExchangeType = "topic";
        });

        // Definición de la routing key para mensajes enviados del tipo order.created.
        cfg.Send<OrderCreatedMessage>(config =>
        {
            config.UseRoutingKeyFormatter(context => "order.created");
        });

        // Endpoint para recibir eventos order.failed.stock provenientes del servicio de inventario.
        cfg.ReceiveEndpoint("order-failed-stock-queue", e =>
        {
            e.ConfigureConsumer<OrderFailedStockConsumer>(context);

            // Vinculación del queue al exchange order_events filtrando por routing key.
            e.Bind("order_events", x =>
            {
                x.RoutingKey = "order.failed.stock";
                x.ExchangeType = "topic";
            });
        });
    });
});

/// <summary>
/// Registro de servicios gRPC y utilidades de reflexión.
/// </summary>
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseRouting();
app.UseGrpcWeb();

/// <summary>
/// Mapeo del servicio gRPC que gestiona las operaciones sobre órdenes.
/// </summary>
app.MapGrpcService<OrderGrpcService>().EnableGrpcWeb();

/// <summary>
/// Activación del servicio de reflexión para inspección de endpoints gRPC.
/// </summary>
app.MapGrpcReflectionService();

/// <summary>
/// Endpoint raíz para verificar que el servicio está en ejecución.
/// </summary>
app.MapGet("/", () => "gRPC Orders Service running. Use Postman, BloomRPC o grpcurl para probarlo.");

app.Run();
