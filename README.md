# SmartBus — Backend C# (.NET 10) + Frontend React

Implementação do projeto SmartBus (monitoramento colaborativo de lotação de
ônibus) descrito no documento `SmartBus_ABNT_v5.docx`, com:

- **Backend**: ASP.NET Core 10 Web API (C#), Entity Framework Core + SQLite, autenticação JWT.
- **Frontend**: React

## Estrutura

O backend segue Clean Architecture (4 projetos, dependência sempre
apontando para dentro: Api → Infrastructure → Application → Domain):

```
smartbus/
├── backend/
│   ├── SmartBus.sln
│   ├── src/
│   │   ├── SmartBus.Domain/          # Entidades, enums, GeoUtils (regra pura, zero dependências)
│   │   ├── SmartBus.Application/     # Casos de uso (services), DTOs, abstrações (IApplicationDbContext, IJwtService...)
│   │   ├── SmartBus.Infrastructure/  # EF Core + SQLite, JwtService, PasswordHasher (implementações concretas)
│   │   └── SmartBus.Api/             # Controllers finos, Program.cs, HttpExtensions (rotas centralizadas)
│   └── tests/
│       ├── SmartBus.UnitTests/       # xUnit + FluentAssertions + EF InMemory — foco na Application
│       │   └── stryker-config.json   # testes mutantes (dotnet stryker)
│       ├── SmartBus.IntegrationTests/# xUnit + WebApplicationFactory — pipeline HTTP completo (SQLite in-memory)
│       └── SmartBus.LoadTests/       # scripts k6 (login, listagem, envio de reportes)
└── frontend/                          # SPA Angular 18
    ├── e2e/                           # testes Playwright (auth, reportar-lotação)
    ├── playwright.config.ts
    └── src/app/
        ├── core/                      # models, services HTTP, guards, interceptor JWT
        ├── shared/components/         # header, bottom-nav
        └── features/                  # boas-vindas, login, registro, principal,
                                        # linhas, mapa-rotas, reportar-lotacao,
                                        # confirmacao-reporte, dashboard (admin)
```

### Rodando os testes

```bash
# unitários + integrados
cd backend && dotnet test

# mutantes (instalar uma vez: dotnet tool install -g dotnet-stryker)
cd backend/tests/SmartBus.UnitTests && dotnet stryker

# carga (instalar k6: https://k6.io/docs/get-started/installation)
cd backend/tests/SmartBus.LoadTests && k6 run linhas-listar.js

# E2E (com a API rodando)
cd frontend && npm run e2e
```

## Como as telas do documento foram mapeadas

| Tela do documento          | Componente Angular              |
|-----------------------------|----------------------------------|
| Boas-vindas / apresentação  | `features/boas-vindas`          |
| Tela principal + paradas próximas | `features/principal`      |
| Consulta de linhas           | `features/linhas`               |
| Mapa de rotas                | `features/mapa-rotas` (Leaflet) |
| Reportar lotação             | `features/reportar-lotacao`     |
| Confirmação de reporte       | `features/confirmacao-reporte`  |
| Dashboard administrativo (KPIs, mapa de calor, previsão de espera) | `features/dashboard` |

## Endpoints da API

| Método | Rota                                  | Descrição                                    | Auth        |
|--------|----------------------------------------|-----------------------------------------------|-------------|
| POST   | `/api/auth/registro`                   | Cria conta de aluno                           | Público     |
| POST   | `/api/auth/login`                      | Autentica e retorna JWT                       | Público     |
| GET    | `/api/linhas?termo=`                   | Lista/busca linhas + nível de lotação atual   | JWT         |
| GET    | `/api/linhas/{id}`                     | Detalhe de uma linha                          | JWT         |
| GET    | `/api/paradas`                         | Lista todas as paradas                        | JWT         |
| GET    | `/api/paradas/proximas?latitude=&longitude=&raioMetros=` | Paradas próximas ao usuário | JWT |
| GET    | `/api/rotas/{linhaId}`                 | Paradas ordenadas da rota (para o mapa)       | JWT         |
| POST   | `/api/reportes`                        | Envia reporte de lotação (crowdsourcing)      | JWT         |
| GET    | `/api/reportes/linha/{linhaId}/recentes` | Últimos reportes válidos de uma linha       | JWT         |
| GET    | `/api/previsao/linha/{linhaId}`        | Previsão heurística de lotação                | JWT         |
| GET    | `/api/dashboard/kpis`                  | KPIs, ranking de superlotação, mapa de calor, previsão de espera | JWT + papel Admin |

Documentação interativa via Swagger em `/swagger` (ambiente de desenvolvimento).

## Decisões de arquitetura e por quê

- **SQLite + EF Core**: banco de arquivo único, zero configuração de servidor,
  ideal para desenvolvimento/demonstração acadêmica. Trocar para SQL Server ou
  PostgreSQL é uma mudança de uma linha em `Program.cs`
  (`UseSqlite` → `UseSqlServer`/`UseNpgsql`) + pacote NuGet correspondente.
- **JWT nativo do ASP.NET Core** (em vez de um identity provider dedicado):
  suficiente para o escopo do projeto (um único tipo de client — o app Angular
  — e dois papéis, Aluno/Admin). Caso o projeto precise de SSO institucional
  ou múltiplos clients no futuro, migrar para Keycloak/IdentityServer é viável
  sem reescrever a lógica de negócio.
- **Motor de previsão "leve"**: em vez de um modelo de ML, `PrevisaoService`
  calcula médias históricas por linha/dia da semana/faixa horária de 2h, com
  fallback para a média geral da linha quando há poucas amostras. Atende ao
  objetivo de leveza computacional citado no documento, mantendo o código
  simples de auditar e evoluir depois para um modelo estatístico mais
  sofisticado, se necessário.
- **Validação de crowdsourcing**: `ValidacaoService` marca um reporte como
  inválido (mas não o descarta) quando ele diverge muito da média dos
  reportes recentes da mesma linha — uma defesa simples contra ruído/abuso,
  sem bloquear a experiência do usuário.

## Rodando o backend

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

`backend/src/SmartBus.Api/appsettings.json` só tem placeholders para a
connection string e a chave JWT (não versionamos segredos reais). Antes do
primeiro `dotnet run`, configure os valores locais via `user-secrets`
(ficam fora do repositório, em `%APPDATA%\Microsoft\UserSecrets\`):

```bash
cd backend/src/SmartBus.Api
dotnet restore

dotnet user-secrets set "ConnectionStrings:Default" "Server=127.0.0.1;Port=3306;Database=smartbus;User=root;Password=root;"
dotnet user-secrets set "Jwt:ChaveSecreta" "uma-chave-com-no-minimo-32-caracteres"

dotnet run
```

> Alternativa sem `user-secrets`: defina as mesmas chaves como variáveis de
> ambiente (`ConnectionStrings__Default` e `Jwt__ChaveSecreta`) — o ASP.NET
> Core lê ambas as fontes automaticamente.

A API sobe em `https://localhost:5001` (Swagger em `/swagger`). No primeiro
start, o schema é criado e populado automaticamente com 3 linhas, 4 paradas
e um usuário admin de exemplo:

- **E-mail**: `admin@fsa.edu.br`
- **Senha**: `admin123`

> Este projeto usa `EnsureCreated()` para criar o schema automaticamente,
> pois não há migrations versionadas no sandbox onde foi gerado. Antes de
> evoluir o schema, gere a primeira migration:
> ```bash
> dotnet tool install --global dotnet-ef   # se ainda não tiver
> dotnet ef migrations add InicialSmartBus
> ```
> e troque `db.Database.EnsureCreated()` por `db.Database.Migrate()` em
> `Data/DbSeeder.cs`.

**Importante**: `Jwt:ChaveSecreta` precisa ter no mínimo 32 caracteres e
nunca deve ser commitada em texto plano — use `user-secrets` em
desenvolvimento e variáveis de ambiente/secret manager do provedor (Azure
Key Vault, AWS Secrets Manager etc.) em produção.

## Rodando o frontend

Pré-requisito: Node.js 18+.

```bash
cd smartbus-web
npm install
npm start   # ng serve, http://localhost:4200
```

O frontend já foi compilado e validado neste ambiente (`ng build` sem erros).
O arquivo `src/environments/environment.ts` aponta para
`https://localhost:5001/api` — ajuste se a API rodar em outra porta/host.

## O que falta para produção (não coberto aqui)

- Testes automatizados (unitários/integração) de backend e frontend.
- Paginação nos endpoints de listagem quando o volume de linhas/reportes crescer.
- Rate limiting no endpoint de reportes para reforçar a proteção anti-abuso.
- CI/CD e Dockerfile para deploy.
- Refinamento visual fiel aos mockups originais do documento (o layout atual é
  funcional e limpo, mas não foi pixel-matched contra as figuras do TCC).
