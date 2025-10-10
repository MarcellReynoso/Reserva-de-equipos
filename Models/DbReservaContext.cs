using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Reserva_de_equipos.Models;

public partial class DbReservaContext : DbContext
{
    public DbReservaContext()
    {
    }

    public DbReservaContext(DbContextOptions<DbReservaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<AreaDisponible> AreaDisponibles { get; set; }

    public virtual DbSet<DetalleReserva> DetalleReservas { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Equipo> Equipos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Responsable> Responsables { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoEquipo> TipoEquipos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PC-MARCELL;Database=db_Reserva;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Area", "Reserva");

            entity.Property(e => e.Descripción)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Empresa).WithMany(p => p.Areas)
                .HasForeignKey(d => d.EmpresaId)
                .HasConstraintName("FK_Area_Empresa");
        });

        modelBuilder.Entity<AreaDisponible>(entity =>
        {
            entity.ToTable("Area_Disponible", "Reserva");

            entity.Property(e => e.AreaDisponibleId).HasColumnName("Area_DisponibleId");

            entity.HasOne(d => d.Area).WithMany(p => p.AreaDisponibles)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Area_Disponible_Area");

            entity.HasOne(d => d.Equipo).WithMany(p => p.AreaDisponibles)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Area_Disponible_Equipo");
        });

        modelBuilder.Entity<DetalleReserva>(entity =>
        {
            entity.ToTable("Detalle_Reserva", "Reserva");

            entity.Property(e => e.DetalleReservaId).HasColumnName("Detalle_ReservaId");
            entity.Property(e => e.CantidadEquipos).HasColumnName("Cantidad_Equipos");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Inicio");
            entity.Property(e => e.Ubicación)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Reserva).WithMany(p => p.DetalleReservas)
                .HasForeignKey(d => d.ReservaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Detalle_Reserva_Reserva");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("Empresa", "Reserva");

            entity.Property(e => e.Descripción)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.ToTable("Equipo", "Reserva");

            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TipoEquipoId).HasColumnName("Tipo_EquipoId");

            entity.HasOne(d => d.Responsable).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.ResponsableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipo_Responsable");

            entity.HasOne(d => d.TipoEquipo).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.TipoEquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipo_Tipo_Equipo");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("Reserva", "Reserva");

            entity.Property(e => e.Indefinido).HasColumnName("Indefinido");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Inicio");
            entity.Property(e => e.Ubicación)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Equipo).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_Equipo");
        });

        modelBuilder.Entity<Responsable>(entity =>
        {
            entity.ToTable("Responsable", "Reserva");

            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ApellidoPaterno)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SegundoNombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Rol", "Reserva");

            entity.Property(e => e.Descripción)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoEquipo>(entity =>
        {
            entity.ToTable("Tipo_Equipo", "Reserva");

            entity.Property(e => e.TipoEquipoId).HasColumnName("Tipo_EquipoId");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario", "Reserva");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ApellidoPaterno)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SegundoNombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Area).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK_Usuario_Area");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
