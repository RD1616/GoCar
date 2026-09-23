using GoCar.Application.Interfaces;
using GoCar.Application.Services;
using GoCar.Infrastructure.Data;
using GoCar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONTROLLERS
// =====================================================

builder.Services.AddControllers();


// =====================================================
// SERVIÇOS DE CLIENTE
// =====================================================

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();


// =====================================================
// SERVIÇOS DE USUÁRIO / LOGIN
// =====================================================

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();


// =====================================================
// SERVIÇOS DE VEÍCULO
// =====================================================

builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();


// =====================================================
// SERVIÇOS DE CATEGORIA
// =====================================================

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();


// =====================================================
// SERVIÇOS DE FILIAL
// =====================================================

builder.Services.AddScoped<IFilialService, FilialService>();
builder.Services.AddScoped<IFilialRepository, FilialRepository>();


// =====================================================
// SERVIÇOS DE LOCAÇÃO
// =====================================================

builder.Services.AddScoped<ILocacaoService, LocacaoService>();
builder.Services.AddScoped<ILocacaoRepository, LocacaoRepository>();


// =====================================================
// SERVIÇOS DE RESERVA
// =====================================================

builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();


// =====================================================
// SERVIÇOS DE PAGAMENTO
// =====================================================

builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();


// =====================================================
// BANCO DE DADOS
// =====================================================

builder.Services.AddDbContext<GoCarDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "GoCarConnection"));
});


// =====================================================
// JWT
// =====================================================

var jwtKey =
    builder.Configuration["Jwt:Key"];

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    builder.Configuration["Jwt:Audience"];


// Validação das configurações
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "A configuração Jwt:Key não foi encontrada.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "A configuração Jwt:Issuer não foi encontrada.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "A configuração Jwt:Audience não foi encontrada.");
}


// =====================================================
// CHAVE JWT
// =====================================================

var securityKey =
    new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtKey));

// IMPORTANTE:
// Deve ser o mesmo KeyId usado no UsuarioService.
securityKey.KeyId = "GoCarJwtKey";


// =====================================================
// AUTENTICAÇÃO
// =====================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // -----------------------------------------
                // ASSINATURA
                // -----------------------------------------

                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    securityKey,

                TryAllIssuerSigningKeys = true,


                // -----------------------------------------
                // ISSUER
                // -----------------------------------------

                ValidateIssuer = true,

                ValidIssuer =
                    jwtIssuer,


                // -----------------------------------------
                // AUDIENCE
                // -----------------------------------------

                ValidateAudience = true,

                ValidAudience =
                    jwtAudience,


                // -----------------------------------------
                // TEMPO DE EXPIRAÇÃO
                // -----------------------------------------

                ValidateLifetime = true,

                RequireExpirationTime = true,

                ClockSkew =
                    TimeSpan.Zero
            };
    });


// =====================================================
// AUTORIZAÇÃO
// =====================================================

builder.Services.AddAuthorization();


// =====================================================
// SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // ---------------------------------------------
    // JWT NO SWAGGER
    // ---------------------------------------------

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type =
                SecuritySchemeType.Http,

            Scheme =
                "bearer",

            BearerFormat =
                "JWT",

            In =
                ParameterLocation.Header,

            Description =
                "Digite somente o token JWT."
        });


    // ---------------------------------------------
    // EXIGIR TOKEN NOS ENDPOINTS PROTEGIDOS
    // ---------------------------------------------

    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)
                ] = []
            });
});


// =====================================================
// CRIAR APLICAÇÃO
// =====================================================

var app =
    builder.Build();


// =====================================================
// SWAGGER
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// =====================================================
// HTTPS
// =====================================================

app.UseHttpsRedirection();


// =====================================================
// AUTENTICAÇÃO
// =====================================================

// A ordem é importante.
// Authentication deve vir antes de Authorization.

app.UseAuthentication();

app.UseAuthorization();


// =====================================================
// CONTROLLERS
// =====================================================

app.MapControllers();


// =====================================================
// EXECUTAR API
// =====================================================

app.Run();