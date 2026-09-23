using GoCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace GoCar.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =====================================================
        // LOGIN - GET
        // =====================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // =====================================================
        // LOGIN - POST
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client =
                _httpClientFactory
                    .CreateClient("GoCarApi");

            var response =
                await client.PostAsJsonAsync(
                    "api/Auth/login",
                    new
                    {
                        email =
                            model.Email,

                        senha =
                            model.Senha
                    });

            if (!response.IsSuccessStatusCode)
            {
                model.MensagemErro =
                    "E-mail ou senha inválidos.";

                return View(model);
            }

            var loginResponse =
                await response.Content
                    .ReadFromJsonAsync<
                        LoginResponseViewModel>();

            if (loginResponse == null ||
                string.IsNullOrWhiteSpace(
                    loginResponse.Token))
            {
                model.MensagemErro =
                    "Não foi possível realizar o login.";

                return View(model);
            }

            CriarSessao(
                loginResponse);

            if (loginResponse.Perfil ==
                    "Administrador" ||
                loginResponse.Perfil ==
                    "Gerente" ||
                loginResponse.Perfil ==
                    "Atendente")
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =====================================================
        // CADASTRO - GET
        // =====================================================

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View(
                new CadastroViewModel());
        }

        // =====================================================
        // CADASTRO - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(
            CadastroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client =
                _httpClientFactory
                    .CreateClient("GoCarApi");

            var dto = new
            {
                nome =
                    model.Nome,

                email =
                    model.Email,

                senha =
                    model.Senha,

                cpf =
                    model.CPF,

                telefone =
                    model.Telefone,

                dataNascimento =
                    model.DataNascimento,

                cnh =
                    model.CNH,

                categoriaCNH =
                    model.CategoriaCNH,

                dataValidadeCNH =
                    model.DataValidadeCNH,

                endereco =
                    model.Endereco,

                numero =
                    model.Numero,

                bairro =
                    model.Bairro,

                cidade =
                    model.Cidade,

                estado =
                    model.Estado,

                cep =
                    model.CEP
            };

            var response =
                await client.PostAsJsonAsync(
                    "api/Auth/cadastro",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                var mensagem =
                    "Não foi possível realizar o cadastro.";

                try
                {
                    var erro =
                        await response.Content
                            .ReadFromJsonAsync<
                                MensagemApi>();

                    if (!string.IsNullOrWhiteSpace(
                        erro?.Mensagem))
                    {
                        mensagem =
                            erro.Mensagem;
                    }
                }
                catch
                {
                    // Mantém a mensagem padrão.
                }

                ModelState.AddModelError(
                    string.Empty,
                    mensagem);

                return View(model);
            }

            var loginResponse =
                await response.Content
                    .ReadFromJsonAsync<
                        LoginResponseViewModel>();

            if (loginResponse == null ||
                string.IsNullOrWhiteSpace(
                    loginResponse.Token))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cadastro realizado, mas não foi possível iniciar sua sessão.");

                return View(model);
            }

            // ================================================
            // LOGIN AUTOMÁTICO APÓS CADASTRO
            // ================================================

            CriarSessao(
                loginResponse);

            TempData["Sucesso"] =
                "Cadastro realizado com sucesso! Bem-vindo à GoCar.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =====================================================
        // LOGOUT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =====================================================
        // CRIAR SESSÃO
        // =====================================================

        private void CriarSessao(
            LoginResponseViewModel loginResponse)
        {
            HttpContext.Session.SetString(
                "JWT",
                loginResponse.Token);

            HttpContext.Session.SetString(
                "UsuarioNome",
                loginResponse.Nome);

            HttpContext.Session.SetString(
                "UsuarioEmail",
                loginResponse.Email);

            HttpContext.Session.SetString(
                "UsuarioPerfil",
                loginResponse.Perfil);
        }

        // =====================================================
        // RESPOSTA DA API
        // =====================================================

        private class MensagemApi
        {
            public string Mensagem { get; set; }
                = string.Empty;
        }
    }
}