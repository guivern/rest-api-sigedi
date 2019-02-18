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
                    Descripcion="Posee todos los permisos del sistema",
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
    }
}