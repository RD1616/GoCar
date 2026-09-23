using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Descricao)
                .HasMaxLength(500);

            builder.Property(c => c.DiariaBase)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.KmLivre)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.IsAtivo)
                .IsRequired();
        }
    }
}