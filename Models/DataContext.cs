using System.Text;
using Microsoft.EntityFrameworkCore;

namespace rest_api_sigedi.Models
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aqui se agregan los roles del sistema
            modelBuilder.Entity<Rol>().HasData(
                new Rol
                {
                    Id = 1, 
                    Nombre="Administrador", 
                    Descripcion="Personal encargado de la administración del sistema, posee todos los permisos",
                    Activo = true
                },
                new Rol
                {
                    Id = 2, 
                    Nombre="Repartidor", 
                    Descripcion="Personal encargado de la recepción y distribución de artículos",
                    Activo = true
                },
                new Rol
                {
                    Id = 3, 
                    Nombre="Cajero", 
                    Descripcion="Personal encargado de registrar las rendiciones y recibir las devoluciones de los vendedores",
                    Activo = true
                }
            );

            // Aqui se configuran los OnDelete Cascade/Restrict
            modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Precio>()
            .HasOne(p => p.Articulo)
            .WithMany(a => a.Detalle)
            .HasForeignKey(p => p.IdArticulo)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IngresoDetalle>()
            .HasOne(id => id.Ingreso)
            .WithMany(i => i.Detalle)
            .HasForeignKey(id => id.IdIngreso)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IngresoDetalle>()
            .HasOne(i => i.Articulo)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IngresoDetalle>()
            .HasOne(i => i.Precio)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IngresoDetalle>()
            .HasOne(i => i.Edicion)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DistribucionDetalle>()
            .HasOne(dd => dd.Distribucion)
            .WithMany(d => d.Detalle)
            .HasForeignKey(dd => dd.IdDistribucion)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DistribucionDetalle>()
            .HasOne(dd => dd.Movimiento)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DistribucionDetalle>()
            .HasOne(dd => dd.Edicion)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Rol> Roles {get; set;}
        public DbSet<Vendedor> Vendedores {get; set;}
        public DbSet<Categoria> Categorias {get; set;}
        public DbSet<Proveedor> Proveedores {get; set;}
        public DbSet<Articulo> Articulos {get; set;}
        public DbSet<Precio> Precios {get; set;}
        public DbSet<Ingreso> Ingresos {get; set;}
        public DbSet<IngresoDetalle> IngresoDetalles {get; set;}
        public DbSet<Edicion> Ediciones {get; set;}
        public DbSet<Egreso> Egresos {get; set;}
        public DbSet<Distribucion> Distribuciones {get; set;}
        public DbSet<DistribucionDetalle> DistribucionDetalles {get; set;}
        public DbSet<Movimiento> Movimientos {get; set;}
    }
}