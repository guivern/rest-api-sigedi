﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using rest_api_sigedi.Models;

namespace RestApiSigedi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190418173619_CamposNuevosRendicionPrecio")]
    partial class CamposNuevosRendicionPrecio
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<DateTime>("FechaCreacion");

                    b.Property<DateTime?>("FechaUltimaModificacion");

                    b.Property<long>("IdCategoria");

                    b.Property<long>("IdProveedor");

                    b.Property<long?>("IdUsuarioCreador");

                    b.Property<long?>("IdUsuarioModificador");

                    b.HasKey("Id");

                    b.HasIndex("IdCategoria");

                    b.HasIndex("IdProveedor");

                    b.HasIndex("IdUsuarioCreador");

                    b.HasIndex("IdUsuarioModificador");

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

            modelBuilder.Entity("rest_api_sigedi.Models.Distribucion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<bool>("Anulable");

                    b.Property<bool>("Anulado");

                    b.Property<string>("Descripcion");

                    b.Property<bool>("Editable");

                    b.Property<DateTime>("FechaCreacion");

                    b.Property<DateTime?>("FechaUltimaModificacion");

                    b.Property<long?>("IdUsuarioCreador");

                    b.Property<long?>("IdUsuarioModificador");

                    b.Property<long>("IdVendedor");

                    b.HasKey("Id");

                    b.HasIndex("IdUsuarioCreador");

                    b.HasIndex("IdUsuarioModificador");

                    b.HasIndex("IdVendedor");

                    b.ToTable("Distribuciones");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.DistribucionDetalle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<bool>("Anulable");

                    b.Property<bool>("Anulado");

                    b.Property<long>("Cantidad");

                    b.Property<string>("Descripcion");

                    b.Property<long?>("Devoluciones");

                    b.Property<bool>("Editable");

                    b.Property<long>("IdDistribucion");

                    b.Property<long>("IdEdicion");

                    b.Property<decimal?>("Importe");

                    b.Property<decimal>("Monto");

                    b.Property<decimal?>("PrecioRendicion");

                    b.Property<decimal?>("PrecioVenta");

                    b.Property<decimal>("Saldo");

                    b.Property<bool?>("YaSeDevolvio");

                    b.HasKey("Id");

                    b.HasIndex("IdDistribucion");

                    b.HasIndex("IdEdicion");

                    b.ToTable("DistribucionDetalles");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Edicion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<bool>("Anulado");

                    b.Property<long>("CantidadActual");

                    b.Property<long>("CantidadInicial");

                    b.Property<string>("Descripcion");

                    b.Property<DateTime?>("FechaEdicion");

                    b.Property<long>("IdArticulo");

                    b.Property<long>("IdPrecio");

                    b.Property<long>("NroEdicion");

                    b.HasKey("Id");

                    b.HasIndex("IdArticulo");

                    b.HasIndex("IdPrecio");

                    b.ToTable("Ediciones");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Egreso", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<long>("Cantidad");

                    b.Property<string>("Descripcion");

                    b.Property<DateTime?>("Fecha");

                    b.Property<DateTime>("FechaCreacion");

                    b.Property<DateTime?>("FechaUltimaModificacion");

                    b.Property<long>("IdEdicion");

                    b.Property<long?>("IdUsuarioCreador");

                    b.Property<long?>("IdUsuarioModificador");

                    b.HasKey("Id");

                    b.HasIndex("IdEdicion");

                    b.HasIndex("IdUsuarioCreador");

                    b.HasIndex("IdUsuarioModificador");

                    b.ToTable("Egresos");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Ingreso", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<bool>("Anulable");

                    b.Property<bool>("Anulado");

                    b.Property<string>("Descripcion");

                    b.Property<bool>("Editable");

                    b.Property<DateTime>("FechaCreacion");

                    b.Property<DateTime?>("FechaUltimaModificacion");

                    b.Property<long>("IdProveedor");

                    b.Property<long?>("IdUsuarioCreador");

                    b.Property<long?>("IdUsuarioModificador");

                    b.Property<string>("NumeroComprobante");

                    b.Property<string>("TipoComprobante");

                    b.HasKey("Id");

                    b.HasIndex("IdProveedor");

                    b.HasIndex("IdUsuarioCreador");

                    b.HasIndex("IdUsuarioModificador");

                    b.ToTable("Ingresos");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.IngresoDetalle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Anulable");

                    b.Property<bool>("Anulado");

                    b.Property<long>("Cantidad");

                    b.Property<string>("Descripcion");

                    b.Property<bool>("Editable");

                    b.Property<DateTime?>("FechaEdicion");

                    b.Property<long>("IdArticulo");

                    b.Property<long>("IdEdicion");

                    b.Property<long>("IdIngreso");

                    b.Property<long>("IdPrecio");

                    b.Property<long>("NroEdicion");

                    b.HasKey("Id");

                    b.HasIndex("IdArticulo");

                    b.HasIndex("IdEdicion");

                    b.HasIndex("IdIngreso");

                    b.HasIndex("IdPrecio");

                    b.ToTable("IngresoDetalles");
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

                    b.Property<string>("Barrio");

                    b.Property<string>("Ciudad");

                    b.Property<string>("Contacto");

                    b.Property<string>("Descripcion");

                    b.Property<string>("Direccion");

                    b.Property<string>("DireccionWeb");

                    b.Property<string>("Email");

                    b.Property<string>("NumeroContacto");

                    b.Property<string>("NumeroDocumento")
                        .IsRequired();

                    b.Property<string>("RazonSocial")
                        .IsRequired();

                    b.Property<string>("Telefono");

                    b.HasKey("Id");

                    b.ToTable("Proveedores");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Rendicion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<bool>("Anulable");

                    b.Property<bool>("Anulado");

                    b.Property<string>("Descripcion");

                    b.Property<DateTime>("FechaCreacion");

                    b.Property<DateTime?>("FechaUltimaModificacion");

                    b.Property<long?>("IdUsuarioCreador");

                    b.Property<long?>("IdUsuarioModificador");

                    b.Property<long>("IdVendedor");

                    b.Property<decimal>("ImporteTotal");

                    b.Property<decimal>("MontoTotal");

                    b.Property<string>("NroComprobante");

                    b.Property<decimal>("SaldoTotal");

                    b.Property<string>("TipoComprobante");

                    b.HasKey("Id");

                    b.HasIndex("IdUsuarioCreador");

                    b.HasIndex("IdUsuarioModificador");

                    b.HasIndex("IdVendedor");

                    b.ToTable("Rendiciones");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.RendicionDetalle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activo");

                    b.Property<bool>("Anulable");

                    b.Property<bool>("Anulado");

                    b.Property<string>("Descripcion");

                    b.Property<long?>("Devoluciones");

                    b.Property<long>("IdDistribucionDetalle");

                    b.Property<long>("IdRendicion");

                    b.Property<decimal>("Importe");

                    b.Property<decimal>("Monto");

                    b.Property<decimal?>("PrecioRendicion");

                    b.Property<decimal?>("PrecioVenta");

                    b.Property<decimal>("Saldo");

                    b.HasKey("Id");

                    b.HasIndex("IdDistribucionDetalle");

                    b.HasIndex("IdRendicion");

                    b.ToTable("RendicionDetalles");
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

                    b.Property<DateTime?>("FechaIngreso");

                    b.Property<DateTime?>("FechaNacimiento");

                    b.Property<string>("Nacionalidad");

                    b.Property<string>("Nombre")
                        .IsRequired();

                    b.Property<string>("NumeroDocumento");

                    b.Property<string>("Telefono");

                    b.Property<string>("TelefonoMovil");

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

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioCreador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioCreador");

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioModificador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioModificador");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Distribucion", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioCreador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioCreador");

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioModificador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioModificador");

                    b.HasOne("rest_api_sigedi.Models.Vendedor", "Vendedor")
                        .WithMany()
                        .HasForeignKey("IdVendedor")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.DistribucionDetalle", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Distribucion", "Distribucion")
                        .WithMany("Detalle")
                        .HasForeignKey("IdDistribucion")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("rest_api_sigedi.Models.Edicion", "Edicion")
                        .WithMany()
                        .HasForeignKey("IdEdicion")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Edicion", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Articulo", "Articulo")
                        .WithMany()
                        .HasForeignKey("IdArticulo")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rest_api_sigedi.Models.Precio", "Precio")
                        .WithMany()
                        .HasForeignKey("IdPrecio")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Egreso", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Edicion", "Edicion")
                        .WithMany()
                        .HasForeignKey("IdEdicion")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioCreador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioCreador");

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioModificador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioModificador");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Ingreso", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Proveedor", "Proveedor")
                        .WithMany()
                        .HasForeignKey("IdProveedor")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioCreador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioCreador");

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioModificador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioModificador");
                });

            modelBuilder.Entity("rest_api_sigedi.Models.IngresoDetalle", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Articulo", "Articulo")
                        .WithMany()
                        .HasForeignKey("IdArticulo")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("rest_api_sigedi.Models.Edicion", "Edicion")
                        .WithMany()
                        .HasForeignKey("IdEdicion")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("rest_api_sigedi.Models.Ingreso", "Ingreso")
                        .WithMany("Detalle")
                        .HasForeignKey("IdIngreso")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("rest_api_sigedi.Models.Precio", "Precio")
                        .WithMany()
                        .HasForeignKey("IdPrecio")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Precio", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Articulo", "Articulo")
                        .WithMany("Detalle")
                        .HasForeignKey("IdArticulo")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.Rendicion", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioCreador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioCreador");

                    b.HasOne("rest_api_sigedi.Models.Usuario", "UsuarioModificador")
                        .WithMany()
                        .HasForeignKey("IdUsuarioModificador");

                    b.HasOne("rest_api_sigedi.Models.Vendedor", "Vendedor")
                        .WithMany()
                        .HasForeignKey("IdVendedor")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("rest_api_sigedi.Models.RendicionDetalle", b =>
                {
                    b.HasOne("rest_api_sigedi.Models.DistribucionDetalle", "DistribucionDetalle")
                        .WithMany()
                        .HasForeignKey("IdDistribucionDetalle")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("rest_api_sigedi.Models.Rendicion", "Rendicion")
                        .WithMany("Detalle")
                        .HasForeignKey("IdRendicion")
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
