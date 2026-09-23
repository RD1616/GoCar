namespace GoCar.Web.Helpers
{
    public static class VeiculoImagemHelper
    {
        public static string ObterImagem(
            string? marca,
            string? modelo)
        {
            var texto =
                $"{marca} {modelo}".ToLowerInvariant();

            // =========================
            // ECONÔMICO
            // =========================

            if (texto.Contains("mobi"))
                return "/images/veiculos/economico5.png";

            if (texto.Contains("kwid"))
                return "/images/veiculos/economico3.png";

            if (texto.Contains("polo track"))
                return "/images/veiculos/economico4.png";

            if (texto.Contains("uno"))
                return "/images/veiculos/economico6.png";

            if (texto.Contains("ford ka") ||
                texto.Contains(" ka "))
                return "/images/veiculos/economico.png";

            if (texto.Contains("hb20 sense"))
                return "/images/veiculos/economico2.png";


            // =========================
            // HATCH
            // =========================

            if (texto.Contains("argo"))
                return "/images/veiculos/hatch.png";

            if (texto.Contains("c3"))
                return "/images/veiculos/hatch2.png";

            if (texto.Contains("onix") &&
                !texto.Contains("plus"))
                return "/images/veiculos/hatch3.png";

            if (texto.Contains("polo mpi"))
                return "/images/veiculos/hatch4.png";

            if (texto.Contains("208"))
                return "/images/veiculos/hatch5.png";

            if (texto.Contains("hb20") &&
                !texto.Contains("hb20s"))
                return "/images/veiculos/hatch6.png";


            // =========================
            // SEDAN
            // =========================

            if (texto.Contains("virtus"))
                return "/images/veiculos/sedan.png";

            if (texto.Contains("corolla"))
                return "/images/veiculos/sedan2.png";

            if (texto.Contains("onix plus"))
                return "/images/veiculos/sedan3.png";

            if (texto.Contains("hb20s"))
                return "/images/veiculos/sedan4.png";

            if (texto.Contains("versa"))
                return "/images/veiculos/sedan5.png";

            if (texto.Contains("cronos"))
                return "/images/veiculos/sedan6.png";


            // =========================
            // SUV
            // =========================

            if (texto.Contains("kicks"))
                return "/images/veiculos/suv.png";

            if (texto.Contains("pulse"))
                return "/images/veiculos/suv2.png";

            if (texto.Contains("renegade"))
                return "/images/veiculos/suv3.png";

            if (texto.Contains("t-cross") ||
                texto.Contains("t cross"))
                return "/images/veiculos/suv4.png";

            if (texto.Contains("tracker"))
                return "/images/veiculos/suv5.png";

            if (texto.Contains("creta"))
                return "/images/veiculos/suv6.png";


            // =========================
            // PADRÃO
            // =========================

            return "/images/veiculos/economico5.png";
        }
    }
}