using GoCar.Domain.Enums;

namespace GoCar.Domain.Entities
{
    public class Veiculo
    {
        public int Id { get; set; }

        public string Placa { get; set; } = string.Empty;
        public string Chassi { get; set; } = string.Empty;
        public string Renavam { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;

        public short AnoFabricacao { get; set; }
        public short AnoModelo { get; set; }

        public string Cor { get; set; } = string.Empty;

        public TipoCombustivel Combustivel { get; set; }
        public TipoCambio Cambio { get; set; }
        public StatusVeiculo Status { get; set; }

        public int KmAtual { get; set; }

        public decimal ValorDiaria { get; set; }

        public bool IsAtivo { get; set; }

        public int CategoriaId { get; set; }
        public int FilialId { get; set; }

        public Categoria Categoria { get; set; } = null!;
        public Filial Filial { get; set; } = null!;
    }
}