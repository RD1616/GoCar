using System.ComponentModel.DataAnnotations;

namespace GoCar.Web.Models
{
    public class CategoriaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required]
        public decimal DiariaBase { get; set; }

        [Required]
        public decimal KmLivre { get; set; }

        public bool IsAtivo { get; set; }
    }
}