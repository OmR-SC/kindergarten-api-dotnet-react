using System;
using System.Collections.Generic;
using KindergartenAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KindergartenAPI.Data;

public partial class KindergartenContext : DbContext
{
    public KindergartenContext()
    {
    }

    public KindergartenContext(DbContextOptions<KindergartenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asistencium> Asistencia { get; set; }

    public virtual DbSet<Consumo> Consumos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Nino> Ninos { get; set; }

    public virtual DbSet<Nino_Autorizado> Nino_Autorizados { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Plato> Platos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asistencium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Asistenc__3214EC07040D550C");

            entity.HasIndex(e => new { e.Matricula, e.Fecha }, "UK_Asistencia_Nino_Fecha").IsUnique();

            entity.HasOne(d => d.MatriculaNavigation).WithMany(p => p.Asistencia)
                .HasForeignKey(d => d.Matricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asistencia_Nino");
        });

        modelBuilder.Entity<Consumo>(entity =>
        {
            entity.HasKey(e => new { e.Matricula, e.Fecha });

            entity.ToTable("Consumo");

            entity.HasIndex(e => e.MenuId, "IX_Consumo_MenuId");

            entity.HasOne(d => d.MatriculaNavigation).WithMany(p => p.Consumos)
                .HasForeignKey(d => d.Matricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumo_Nino");

            entity.HasOne(d => d.Menu).WithMany(p => p.Consumos)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumo_Menu");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.FacturaId).HasName("PK__Factura__5C024865B4858025");

            entity.ToTable("Factura");

            entity.HasIndex(e => new { e.Matricula, e.Año, e.Mes }, "IX_Factura_Nino_AñoMes");

            entity.Property(e => e.CosteComidas).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CosteFijo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total)
                .HasComputedColumnSql("([CosteFijo]+[CosteComidas])", true)
                .HasColumnType("decimal(11, 2)");

            entity.HasOne(d => d.MatriculaNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.Matricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Nino");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.Nombre).HasName("PK__Ingredie__75E3EFCE88E8F71B");

            entity.ToTable("Ingrediente");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED230773A8381");

            entity.ToTable("Menu");

            entity.Property(e => e.MenuId).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).HasMaxLength(200);

            entity.HasMany(d => d.Platos).WithMany(p => p.Menus)
                .UsingEntity<Dictionary<string, object>>(
                    "MenuPlato",
                    r => r.HasOne<Plato>().WithMany()
                        .HasForeignKey("Plato")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MenuPlato_Plato"),
                    l => l.HasOne<Menu>().WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MenuPlato_Menu"),
                    j =>
                    {
                        j.HasKey("MenuId", "Plato");
                        j.ToTable("MenuPlato");
                        j.IndexerProperty<string>("Plato").HasMaxLength(100);
                    });
        });

        modelBuilder.Entity<Nino>(entity =>
        {
            entity.HasKey(e => e.Matricula).HasName("PK__Nino__0FB9FB4E63764537");

            entity.ToTable("Nino");

            entity.HasIndex(e => e.CedulaPagador, "IX_Nino_CedulaPagador");

            entity.Property(e => e.Matricula).ValueGeneratedNever();
            entity.Property(e => e.CedulaPagador)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.CedulaPagadorNavigation).WithMany(p => p.Ninos)
                .HasForeignKey(d => d.CedulaPagador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Nino_PersonaPagador");

            entity.HasMany(d => d.Ingredientes).WithMany(p => p.Matriculas)
                .UsingEntity<Dictionary<string, object>>(
                    "Alergium",
                    r => r.HasOne<Ingrediente>().WithMany()
                        .HasForeignKey("Ingrediente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Alergia_Ingrediente"),
                    l => l.HasOne<Nino>().WithMany()
                        .HasForeignKey("Matricula")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Alergia_Nino"),
                    j =>
                    {
                        j.HasKey("Matricula", "Ingrediente");
                        j.ToTable("Alergia");
                        j.IndexerProperty<string>("Ingrediente").HasMaxLength(100);
                    });
        });

        modelBuilder.Entity<Nino_Autorizado>(entity =>
        {
            entity.HasKey(e => new { e.Matricula, e.Cedula });

            entity.ToTable("Nino_Autorizado");

            entity.Property(e => e.Cedula)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Relacion).HasMaxLength(50);

            entity.HasOne(d => d.CedulaNavigation).WithMany(p => p.Nino_Autorizados)
                .HasForeignKey(d => d.Cedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Autorizado_Persona");

            entity.HasOne(d => d.MatriculaNavigation).WithMany(p => p.Nino_Autorizados)
                .HasForeignKey(d => d.Matricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Autorizado_Nino");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Cedula).HasName("PK__Persona__B4ADFE39FC4AFAD3");

            entity.ToTable("Persona");

            entity.Property(e => e.Cedula)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CuentaCorriente).HasMaxLength(30);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.Nombre).HasName("PK__Plato__75E3EFCE27CC895C");

            entity.ToTable("Plato");

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasMany(d => d.Ingredientes).WithMany(p => p.Platos)
                .UsingEntity<Dictionary<string, object>>(
                    "PlatoIngrediente",
                    r => r.HasOne<Ingrediente>().WithMany()
                        .HasForeignKey("Ingrediente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PlatoIngrediente_Ingrediente"),
                    l => l.HasOne<Plato>().WithMany()
                        .HasForeignKey("Plato")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PlatoIngrediente_Plato"),
                    j =>
                    {
                        j.HasKey("Plato", "Ingrediente");
                        j.ToTable("PlatoIngrediente");
                        j.IndexerProperty<string>("Plato").HasMaxLength(100);
                        j.IndexerProperty<string>("Ingrediente").HasMaxLength(100);
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
