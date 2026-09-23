using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class LocacaoConfiguration : IEntityTypeConfiguration<Locacao>
    {
        public void Configure(EntityTypeBuilder<Locacao> builder)
        {
            builder.ToTable("Locacoes");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.DataRetirada)
                .IsRequired();

            builder.Property(l => l.DataDevolucaoReal)
                .IsRequired(false);

            builder.Property(l => l.KmSaida)
                .IsRequired();

            builder.Property(l => l.KmEntrada)
                .IsRequired(false);

            builder.Property(l => l.CombustivelSaidaPercentual)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(l => l.CombustivelEntradaPercentual)
                .HasPrecision(5, 2)
                .IsRequired(false);

            builder.Property(l => l.ValorTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(l => l.Status)
                .IsRequired();

            builder.Property(l => l.Observacoes)
                .HasMaxLength(500);

            builder.Property(l => l.IsAtiva)
                .IsRequired();

            builder.Property(l => l.DataCriacao)
                .IsRequired();

            // Reserva → Locação
            builder.HasOne(l => l.Reserva)
                .WithOne(r => r.Locacao)
                .HasForeignKey<Locacao>(l => l.ReservaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Locação → Pagamentos
            builder.HasMany(l => l.Pagamentos)
                .WithOne(p => p.Locacao)
                .HasForeignKey(p => p.LocacaoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}