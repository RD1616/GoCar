using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class FilialConfiguration : IEntityTypeConfiguration<Filial>
    {
        public void Configure(EntityTypeBuilder<Filial> builder)
        {
            builder.ToTable("Filiais");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.CNPJ)
                .IsRequired()
                .HasMaxLength(18);

            builder.Property(f => f.Telefone)
                .HasMaxLength(20);

            builder.Property(f => f.Email)
                .HasMaxLength(150);

            builder.Property(f => f.Endereco)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.Numero)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(f => f.Bairro)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Estado)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(f => f.CEP)
                .IsRequired()
                .HasMaxLength(9);

            builder.Property(f => f.IsAtivo)
                .IsRequired();
        }
    }
}