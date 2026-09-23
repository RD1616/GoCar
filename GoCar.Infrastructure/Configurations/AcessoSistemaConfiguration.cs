using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class AcessoSistemaConfiguration : IEntityTypeConfiguration<AcessoSistema>
    {
        public void Configure(EntityTypeBuilder<AcessoSistema> builder)
        {
            builder.ToTable("AcessosSistema");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Acao)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Entidade)
                .HasMaxLength(100);

            builder.Property(a => a.Descricao)
                .HasMaxLength(500);

            builder.Property(a => a.EnderecoIP)
                .HasMaxLength(45);

            builder.Property(a => a.DataHora)
                .IsRequired();

            // Usuário → Acessos do sistema
            builder.HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}