using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class MovimentacaoVeiculoConfiguration : IEntityTypeConfiguration<MovimentacaoVeiculo>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoVeiculo> builder)
        {
            builder.ToTable("MovimentacoesVeiculos");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.DataMovimentacao)
                .IsRequired();

            builder.Property(m => m.Motivo)
                .HasMaxLength(250);

            builder.Property(m => m.KmVeiculo)
                .IsRequired();

            builder.Property(m => m.Observacoes)
                .HasMaxLength(500);

            // Veículo → Movimentações
            builder.HasOne(m => m.Veiculo)
                .WithMany()
                .HasForeignKey(m => m.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Filial de origem → Movimentações
            builder.HasOne(m => m.FilialOrigem)
                .WithMany()
                .HasForeignKey(m => m.FilialOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Filial de destino → Movimentações
            builder.HasOne(m => m.FilialDestino)
                .WithMany()
                .HasForeignKey(m => m.FilialDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}