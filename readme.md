# Microondas Digital

Simulador de microondas digital desenvolvido em .NET 8, com interface desktop (WPF), API REST e persistência em banco de dados SQL Server.

## Tecnologias

- **Linguagem:** C# / .NET 8.0
- **Interface:** WPF (Windows Presentation Foundation)
- **API:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core 8 + SQL Server
- **Autenticação:** JWT Bearer Token
- **Testes:** xUnit 2.5.3
- **Documentação da API:** Swagger (Swashbuckle 6.6.2)

## Estrutura do Projeto

```
Microondas/
├── Microondas.Domain/              # Regras de negócio (entidades, serviços, validações)
├── Microondas.Infraestructure/     # Persistência com EF Core + SQL Server
├── Microondas.WebApplication/      # API REST (ASP.NET Core 8)
├── Microondas.WPF/                 # Interface desktop (padrão MVVM)
├── Microondas.Console/             # Demonstração em console
└── Microondas.Test.Domain/         # Testes unitários (xUnit)
```

## Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server ou SQL Server LocalDB
- Visual Studio 2022 ou superior

## Como instalar e usar

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd Microondas
```

### 2. Banco de dados

A connection string é armazenada criptografada (AES) no `appsettings.json`. A connection string descriptografada é:

```
Server=(localdb)\mssqllocaldb;Database=MicroondasDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Chave de criptografia utilizada: `MicroondasBenner2026`

Para alterar, edite `Microondas.WebApplication/appsettings.json`.

Crie o banco executando as migrations:

```bash
dotnet ef database update --project Microondas.Infraestructure --startup-project WebApplication1
```

### 3. Executar a API

```bash
dotnet run --project WebApplication1
```

A API estará disponível em `http://localhost:5094`.  
Acesse a documentação Swagger em: `http://localhost:5094/swagger`

### 4. Executar a interface WPF

```bash
dotnet run --project Microondas.WPF
```

Na tela de login, utilize as credenciais padrão:

| Campo | Valor |
|-------|-------|
| Usuário | `admin` |
| Senha | `admin123` |

> A interface WPF consome a API REST. Certifique-se de que a API esteja em execução antes de iniciar o WPF.

### 5. Executar os testes

```bash
dotnet test Microondas.Test.Domain
```

## API — Endpoints

> Todos os endpoints exceto `/api/auth/login` requerem o header: `Authorization: Bearer <token>`

### Autenticação

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/auth/login` | Autentica e retorna JWT token |

**Body:**
```json
{ "username": "admin", "password": "admin123" }
```

### Microondas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/api/microondas/iniciar` | Inicia aquecimento |
| `POST` | `/api/microondas/parar` | Pausa ou cancela aquecimento |
| `GET` | `/api/microondas/status` | Retorna estado atual |
| `POST` | `/api/microondas/iniciar-programa/{nome}` | Inicia com programa predefinido |

**Query params opcionais para `/iniciar`:** `tempo` (1–120), `potencia` (1–10)

### Programas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/programas` | Lista todos os programas |
| `GET` | `/api/programas/{nome}` | Retorna programa pelo nome |
| `POST` | `/api/programas` | Cadastra programa customizado |
| `DELETE` | `/api/programas/{nome}` | Remove programa customizado |

## Nível implementado

Este projeto atende ao **Nível 4** de todos os requisitos.

---

> This is a challenge by [Coodesh](http://coodesh.com/)
