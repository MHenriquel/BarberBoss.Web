# BarberBossWeb — .NET 9 + EF Core + SQL Server (Clean Architecture)

Aplicação **Web** (para PC local via navegador) acessando a nuvem, com suporte a **filiais**, **comissão mista** e pagamentos (**PIX** e **Cartão**) via abstração de gateway.

## Projetos
- `BarberBoss.Domain` — entidades e regras
- `BarberBoss.Application` — casos de uso (MediatR) + validações (FluentValidation)
- `BarberBoss.Infrastructure` — EF Core + SQL Server + repositórios + gateway fake
- `BarberBoss.Web` — ASP.NET Core Web App (UI + endpoints API)
- `BarberBoss.Client` — cliente HTTP opcional (para futuros apps)

## Como rodar
1) Ajuste a connection string em `src/BarberBoss.Web/appsettings.Development.json`
2) Rode migrations:

```bash
cd src/BarberBoss.Web

dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project ../BarberBoss.Infrastructure --startup-project .
dotnet ef database update --project ../BarberBoss.Infrastructure --startup-project .

dotnet run
```

Acesse:
- UI: `https://localhost:5001`
- Swagger: `https://localhost:5001/swagger`

## Pagamentos (PIX/Cartão)
Existe um `FakePaymentGateway` para desenvolvimento. Para produção, implemente `IPaymentGateway` para um provedor e valide webhooks.
