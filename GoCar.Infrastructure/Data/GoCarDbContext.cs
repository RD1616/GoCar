using GoCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoCar.Infrastructure.Data
{
    public class GoCarDbContext : DbContext
    {
        public GoCarDbContext(DbContextOptions<GoCarDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<VeiculoImagem> VeiculoImagens { get; set; }
        public DbSet<ManutencaoVeiculo> ManutencoesVeiculos { get; set; }
        public DbSet<MovimentacaoVeiculo> MovimentacoesVeiculos { get; set; }
        public DbSet<AcessoSistema> AcessosSistema { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoCarDbContext).Assembly);
        }
    }
}