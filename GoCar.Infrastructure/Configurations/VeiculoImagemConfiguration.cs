using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class VeiculoImagemConfiguration : IEntityTypeConfiguration<VeiculoImagem>
    {
        public void Configure(EntityTypeBuilder<VeiculoImagem> builder)
        {
            builder.ToTable("VeiculoImagens");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.Descricao)
                .HasMaxLength(250);

            builder.Property(v => v.Principal)
                .IsRequired();

            builder.Property(v => v.Ordem)
                .IsRequired();

            builder.Property(v => v.DataCadastro)
                .IsRequired();

            // Veículo → Imagens
            builder.HasOne(v => v.Veiculo)
                .WithMany()
                .HasForeignKey(v => v.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}