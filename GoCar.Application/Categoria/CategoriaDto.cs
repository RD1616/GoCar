namespace GoCar.Application.DTOs.Categoria
{
    public class CategoriaDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public decimal DiariaBase { get; set; }

        public decimal KmLivre { get; set; }

        public bool IsAtivo { get; set; }
    }
}