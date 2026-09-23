using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.ToTable("Reservas");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.DataReserva)
                .IsRequired();

            builder.Property(r => r.DataRetirada)
                .IsRequired();

            builder.Property(r => r.DataDevolucaoPrevista)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.ValorTotalPrevisto)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(r => r.Observacoes)
                .HasMaxLength(500);

            builder.Property(r => r.IsAtiva)
                .IsRequired();

            // Cliente → Reservas
            builder.HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Veículo → Reservas
            builder.HasOne(r => r.Veiculo)
                .WithMany()
                .HasForeignKey(r => r.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Filial de retirada → Reservas
            builder.HasOne(r => r.FilialRetirada)
                .WithMany()
                .HasForeignKey(r => r.FilialRetiradaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Filial de devolução → Reservas
            builder.HasOne(r => r.FilialDevolucao)
                .WithMany()
                .HasForeignKey(r => r.FilialDevolucaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva → Locação
            builder.HasOne(r => r.Locacao)
                .WithOne(l => l.Reserva)
                .HasForeignKey<Locacao>(l => l.ReservaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}