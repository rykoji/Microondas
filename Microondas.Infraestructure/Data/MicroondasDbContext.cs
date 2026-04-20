using Microondas.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microondas.Domain.Entities;

namespace Microondas.Infrastructure.Data;

public class MicroondasDbContext(DbContextOptions<MicroondasDbContext> options) : DbContext(options)
{
    public DbSet<AquecimentoCustomizado> ProgramasCustomizados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AquecimentoCustomizado>(entity =>
        {
            entity.HasKey(e => e.Nome);
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Alimento).HasMaxLength(200);
            entity.Property(e => e.Instrucoes).HasMaxLength(500).IsRequired(false);
            entity.Property(e => e.CaracterAquecimento).HasMaxLength(1);
        });
    }
}