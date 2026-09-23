using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class ManutencaoVeiculoConfiguration : IEntityTypeConfiguration<ManutencaoVeiculo>
    {
        public void Configure(EntityTypeBuilder<ManutencaoVeiculo> builder)
        {
            builder.ToTable("ManutencoesVeiculos");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Tipo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.Oficina)
                .HasMaxLength(150);

            builder.Property(m => m.DataEntrada)
                .IsRequired();

            builder.Property(m => m.DataSaida)
                .IsRequired(false);

            builder.Property(m => m.Custo)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(m => m.KmVeiculo)
                .IsRequired();

            builder.Property(m => m.Concluida)
                .IsRequired();

            builder.Property(m => m.Observacoes)
                .HasMaxLength(500);

            // Veículo → Manutenções
            builder.HasOne(m => m.Veiculo)
                .WithMany()
                .HasForeignKey(m => m.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}