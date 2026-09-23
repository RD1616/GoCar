namespace GoCar.Application.DTOs.Categoria
{
    public class AtualizarCategoriaDto
    {
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public decimal DiariaBase { get; set; }

        public decimal KmLivre { get; set; }

        public bool IsAtivo { get; set; }
    }
}