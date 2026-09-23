using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");

            builder.HasKey(p => p.Id);

            // =====================================================
            // PROPRIEDADES
            // =====================================================

            builder.Property(p => p.Tipo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.FormaPagamento)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.Valor)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.DataPagamento)
                .IsRequired(false);

            builder.Property(p => p.Observacoes)
                .HasMaxLength(500);

            builder.Property(p => p.IsAtivo)
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .IsRequired();

            // =====================================================
            // LOCAÇÃO → PAGAMENTOS
            // =====================================================
            //
            // Usado para:
            // - Saldo restante da locação
            // - Multas
            // - Adicionais
            //
            // LocacaoId agora é opcional porque o pagamento
            // também pode pertencer diretamente a uma Reserva.
            // =====================================================

            builder.HasOne(p => p.Locacao)
                .WithMany(l => l.Pagamentos)
                .HasForeignKey(p => p.LocacaoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // RESERVA → PAGAMENTOS
            // =====================================================
            //
            // Usado principalmente para:
            // - Entrada obrigatória de 30%
            //
            // Esse pagamento existe antes da criação da Locação.
            // =====================================================

            builder.HasOne(p => p.Reserva)
                .WithMany(r => r.Pagamentos)
                .HasForeignKey(p => p.ReservaId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}