﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using rest_api_sigedi.Models;

namespace RestApiSigedi.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("rest_api_sigedi.Models.Articulo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<long?>("Codigo");

                    b.Property<string>("Descripcion");

                    b.Property<long>("IdCategoria");

                    b.Property<long>("IdProveedor");

                    b.HasKey("Id");

                    b.HasIndex("IdCategoria");

                    b.HasIndex("IdProveedor");

                    b.ToTable("Articulos");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Categoria", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Descripcion");

                    b.HasKey("Id");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Precio", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Descripcion");

                    b.Property<long>("IdArticulo");

                    b.Property<decimal?>("PrecioRendAgencia");

                    b.Property<decimal>("PrecioRendVendedor");

                    b.Property<decimal>("PrecioVenta");

                    b.HasKey("Id");

                    b.HasIndex("IdArticulo");

                    b.ToTable("Precios");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Proveedor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Descripcion");

                    b.Property<string>("Direccion");

                    b.Property<string>("Email");

                    b.Property<string>("NumeroDocumento")
                        .IsRequired();

                    b.Property<string>("RazonSocial")
                        .IsRequired();

                    b.Property<string>("Telefono");

                    b.HasKey("Id");

                    b.ToTable("Proveedores");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Rol", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Descripcion");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Activo = true,
                            Descripcion = "Personal encargado de la administración del sistema, posee todos los permisos",
                            Nombre = "Administrador"
                        },
                        new
                        {
                            Id = 2L,
                            Activo = true,
                            Descripcion = "Personal encargado de la recepción y distribución de artículos",
                            Nombre = "Repartidor"
                        },
                        new
                        {
                            Id = 3L,
                            Activo = true,
                            Descripcion = "Personal encargado de registrar las rendiciones y recibir las devoluciones de los vendedores",
                            Nombre = "Cajero"
                        });
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Usuario", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Apellido")
                        .IsRequired();

                    b.Property<string>("Descripcion");

                    b.Property<string>("Direccion");

                    b.Property<string>("Email");

                    b.Property<DateTime?>("FechaNacimiento");

                    b.Property<byte[]>("HashPassword")
                        .IsRequired();

                    b.Property<long>("IdRol");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.Property<string>("NumeroDocumento");

                    b.Property<string>("Telefono");

                    b.Property<string>("TipoDocumento");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("IdRol");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Vendedor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<string>("Apellido")
                        .IsRequired();

                    b.Property<string>("Descripcion");

                    b.Property<string>("Direccion");

                    b.Property<string>("Email");

                    b.Property<DateTime?>("FechaNacimiento");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.Property<string>("NumeroDocumento");

                    b.Property<string>("Telefono");

                    b.Property<string>("TipoDocumento");

                    b.Property<string>("ZonaVenta");

                    b.HasKey("Id");

                    b.ToTable("Vendedores");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Articulo", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Categoria", "Categoria")
                        .WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rest_api_sigedi.Models.Proveedor", "Proveedor")
                        .WithMany()
                        .HasForeignKey("IdProveedor")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Precio", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Articulo", "Articulo")
                        .WithMany("Precios")
                        .HasForeignKey("IdArticulo")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Usuario", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Rol", "Rol")
                        .WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
