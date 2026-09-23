using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoCar.Infrastructure.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.SenhaHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.CPF)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(u => u.Telefone)
                .HasMaxLength(20);

            builder.Property(u => u.Perfil)
                .IsRequired();

            builder.Property(u => u.IsAtivo)
                .IsRequired();

            builder.Property(u => u.DataCriacao)
                .IsRequired();

            builder.HasOne(u => u.Cliente)
                .WithOne(c => c.Usuario)
                .HasForeignKey<Cliente>(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}