# 🚗 GoCar --- Sistema de Gestão para Locadora de Veículos

O **GoCar** é um sistema de gerenciamento para locadoras de veículos,
desenvolvido com **C# e .NET**. O projeto reúne uma aplicação desktop
administrativa, uma API e persistência de dados em SQL Server.

O objetivo é centralizar as principais operações de uma locadora,
permitindo administrar veículos, clientes, reservas, locações e
pagamentos em uma única solução.

## ✨ Funcionalidades

-   🔐 Login administrativo com autenticação via JWT
-   📊 Dashboard com indicadores e dados reais do sistema
-   🚘 Cadastro e gerenciamento de veículos
-   🏷️ Gerenciamento de categorias de veículos
-   🏢 Cadastro e gerenciamento de filiais
-   👥 Cadastro e consulta de clientes
-   📅 Criação e acompanhamento de reservas
-   🔑 Controle e finalização de locações
-   💳 Registro e acompanhamento de pagamentos
-   📈 Relatórios de receita, reservas, locações e ticket médio
-   🔎 Pesquisa e filtros nas principais telas
-   ⚙️ Configurações e encerramento de sessão

## 📊 Dashboard

O painel administrativo apresenta uma visão geral da locadora:

-   Total de veículos
-   Locações ativas
-   Clientes cadastrados
-   Receita do mês
-   Locações dos últimos 7 dias
-   Categorias mais alugadas
-   Últimos veículos cadastrados
-   Atalhos rápidos

## 🛠️ Tecnologias utilizadas

-   **C#**
-   **.NET**
-   **Windows Forms (WinForms)**
-   **ASP.NET Core Web API**
-   **Entity Framework Core**
-   **SQL Server**
-   **JWT (JSON Web Token)**
-   **Swagger / OpenAPI**
-   **Git e GitHub**

## 🏗️ Estrutura do projeto

``` text
GoCar
├── GoCar.API
├── GoCar.Application
├── GoCar.Domain
├── GoCar.Infrastructure
├── GoCar.Desktop
├── GoCar.UI
└── GoCar.Web
```

### GoCar.API

Responsável pelos endpoints da aplicação, autenticação e comunicação com
o sistema.

### GoCar.Application

Contém serviços e regras de aplicação utilizadas pelas demais camadas.

### GoCar.Domain

Concentra entidades, enums e elementos centrais do domínio.

### GoCar.Infrastructure

Responsável pela infraestrutura e persistência dos dados.

### GoCar.Desktop

Aplicação administrativa em **Windows Forms**, onde são realizadas as
principais operações da locadora.

## 🔐 Acesso administrativo

O sistema desktop possui autenticação exclusiva para administradores.
Após o login, o token JWT retornado pela API é utilizado nas próximas
requisições.

## 🔄 Fluxo principal

``` text
Cliente
   ↓
Reserva
   ↓
Entrada / Pagamento
   ↓
Locação
   ↓
Finalização
   ↓
Pagamento
   ↓
Relatórios
```

## 🔎 Pesquisa e filtros

**Veículos:** modelo, marca, placa e status.

**Clientes:** nome, CPF, telefone, CNH, cidade e estado.

**Reservas:** cliente, veículo, placa e status da reserva.

**Locações:** identificação da locação/reserva e status.

**Financeiro:** pagamentos, status e forma de pagamento.

## 📋 Relatórios

O módulo de relatórios permite acompanhar, por período:

-   Receita
-   Quantidade de reservas
-   Locações finalizadas
-   Ticket médio

## ▶️ Como executar

### Pré-requisitos

-   Visual Studio com suporte ao desenvolvimento .NET
-   .NET SDK compatível com a solução
-   SQL Server ou SQL Server LocalDB

### Passos

1.  Clone o repositório:

``` bash
git clone https://github.com/RD1616/GoCar.git
```

2.  Abra a solução `GoCar.slnx` no Visual Studio.
3.  Configure a conexão com o SQL Server de acordo com seu ambiente.
4.  Inicie a API.
5.  Execute o projeto `GoCar.Desktop`.
6.  Entre com um usuário de perfil **Administrador**.

> Credenciais, tokens e outras informações sensíveis não devem ser
> armazenados diretamente no repositório.

## 📸 Screenshots

Espaço recomendado para imagens de:

-   Login
-   Dashboard
-   Veículos
-   Clientes
-   Reservas
-   Locações
-   Financeiro
-   Relatórios

## 📌 Status do projeto

✅ Sistema funcional\
✅ Integração Desktop + API\
✅ Autenticação administrativa\
✅ Persistência em SQL Server\
✅ Fluxo de locação e pagamento validado\
✅ Dashboard e relatórios\
✅ Pesquisa e filtros nas principais telas

## 👨‍💻 Autor

Desenvolvido por **Rodrigo Lima**.

GitHub: **RD1616**

------------------------------------------------------------------------

Projeto desenvolvido para fins acadêmicos e de aprendizado, aplicando
conceitos de C#, APIs, banco de dados, arquitetura em camadas e
interfaces desktop.
