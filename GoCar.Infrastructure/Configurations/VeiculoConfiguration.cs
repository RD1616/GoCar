using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("Veiculos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Placa)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(v => v.Placa)
                .IsUnique();

            builder.Property(v => v.Chassi)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(v => v.Chassi)
                .IsUnique();

            builder.Property(v => v.Renavam)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(v => v.Renavam)
                .IsUnique();

            builder.Property(v => v.Modelo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Marca)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.AnoFabricacao)
                .IsRequired();

            builder.Property(v => v.AnoModelo)
                .IsRequired();

            builder.Property(v => v.Cor)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Combustivel)
                .IsRequired();

            builder.Property(v => v.Cambio)
                .IsRequired();

            builder.Property(v => v.KmAtual)
                .IsRequired();

            builder.Property(v => v.Status)
                .IsRequired();

            builder.Property(v => v.ValorDiaria)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(v => v.IsAtivo)
                .IsRequired();

            // Categoria → Veículos
            builder.HasOne(v => v.Categoria)
                .WithMany()
                .HasForeignKey(v => v.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Filial → Veículos
            builder.HasOne(v => v.Filial)
                .WithMany()
                .HasForeignKey(v => v.FilialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}