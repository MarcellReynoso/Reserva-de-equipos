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
    public virtual DbSet<Conductor> Conductores { get; set; }
    public virtual DbSet<Empresa> Empresas { get; set; }
    public virtual DbSet<Equipo> Equipos { get; set; }
    public virtual DbSet<Reserva> Reservas { get; set; }
    public virtual DbSet<Rol> Rols { get; set; }
    public virtual DbSet<TipoEquipo> TipoEquipos { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Name=DbReserva");
    }

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

        modelBuilder.Entity<Conductor>(entity =>
        {
            entity.ToTable("Conductor", "Reserva");

            entity.Property(c => c.Disponible).HasDefaultValue(true);

            entity.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(c => c.SegundoNombre)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(c => c.ApellidoPaterno)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(c => c.ApellidoMaterno)
                .HasMaxLength(200)
                .IsUnicode(false);
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
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(e => e.TipoEquipoId).HasColumnName("Tipo_EquipoId");

            entity.Property(e => e.FechaInicio)
                .HasColumnName("Fecha_Inicio")
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(e => e.FechaFin)
                .HasColumnName("Fecha_Fin")
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            entity.HasOne(d => d.TipoEquipo).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.TipoEquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipo_Tipo_Equipo");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Equipos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Equipo_Usuario");
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
