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
        }

        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Rol> Roles {get; set;}
        public DbSet<Vendedor> Vendedores {get; set;}

        public DbSet<Categoria> Categorias {get; set;}
    }
}