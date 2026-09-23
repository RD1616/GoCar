using System.ComponentModel.DataAnnotations;

namespace GoCar.Web.Models
{
    public class VeiculoViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Placa { get; set; } = string.Empty;

        [Required]
        public string Chassi { get; set; } = string.Empty;

        [Required]
        public string Renavam { get; set; } = string.Empty;

        [Required]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public short AnoFabricacao { get; set; }

        [Required]
        public short AnoModelo { get; set; }

        [Required]
        public string Cor { get; set; } = string.Empty;

        public int Combustivel { get; set; }

        public int Cambio { get; set; }

        public int Status { get; set; }

        public int KmAtual { get; set; }

        public decimal ValorDiaria { get; set; }

        public bool IsAtivo { get; set; }

        public int CategoriaId { get; set; }

        public int FilialId { get; set; }
    }
}