namespace GoCar.Application.DTOs.Categoria
{
    public class CriarCategoriaDto
    {
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public decimal DiariaBase { get; set; }

        public decimal KmLivre { get; set; }
    }
}