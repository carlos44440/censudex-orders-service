using Microsoft.EntityFrameworkCore;
using orders_service.Src.Models;

namespace orders_service.Src.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core para el servicio de órdenes.
    /// Gestiona las entidades y su mapeo hacia la base de datos.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor del contexto, recibe la configuración a través de inyección de dependencias.
        /// </summary>
        /// <param name="options">Opciones de configuración del DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Representa la tabla de órdenes en la base de datos.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Representa la tabla de ítems asociados a una orden.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Configuración adicional del modelo, incluyendo relaciones entre entidades.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo utilizado por EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración: Una orden tiene muchos ítems.
            // Un ítem pertenece a una única orden.
            // Si la orden se elimina, sus ítems también (DeleteBehavior.Cascade).
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(o => o.Order)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
