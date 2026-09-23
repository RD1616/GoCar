namespace GoCar.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public decimal DiariaBase { get; set; }

        public decimal KmLivre { get; set; }

        public bool IsAtivo { get; set; }
    }
}