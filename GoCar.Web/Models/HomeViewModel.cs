namespace GoCar.Web.Models
{
    public class HomeViewModel
    {
        public List<FilialViewModel> Filiais { get; set; } = new();

        public List<VeiculoViewModel> Veiculos { get; set; } = new();
    }
}