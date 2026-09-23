using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.CPF)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(c => c.DataNascimento)
                .IsRequired();

            builder.Property(c => c.Telefone)
                .HasMaxLength(20);

            builder.Property(c => c.CNH)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.CategoriaCNH)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(c => c.DataValidadeCNH)
                .IsRequired();

            builder.Property(c => c.Endereco)
                .HasMaxLength(200);

            builder.Property(c => c.Numero)
                .HasMaxLength(10);

            builder.Property(c => c.Bairro)
                .HasMaxLength(100);

            builder.Property(c => c.Cidade)
                .HasMaxLength(100);

            builder.Property(c => c.Estado)
                .HasMaxLength(2);

            builder.Property(c => c.CEP)
                .HasMaxLength(9);

            builder.Property(c => c.IsAtivo)
                .IsRequired();

            builder.Property(c => c.DataCadastro)
                .IsRequired();

            builder.HasOne(c => c.Usuario)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}