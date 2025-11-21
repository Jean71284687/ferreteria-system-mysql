using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Models;

namespace FerreteriaAPI.Data
{
    public class FerreteriaContext : DbContext
    {
        public FerreteriaContext(DbContextOptions<FerreteriaContext> options) : base(options) { }
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones adicionales si las necesitas
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.NumeroDocumento)
                .IsUnique();
        }
    }
}Restauración completada (0.4s)
 