# Testes mutantes (Stryker.NET)

Alvo: `SmartBus.Application` — é onde mora a lógica de negócio de fato
(cálculo de lotação atual, validação de reportes, motor de previsão).
Domain, Infrastructure e Api ficam de fora de propósito: Domain é
essencialmente dado, e Infrastructure/Api são principalmente
orquestração/plumbing — mutar ali gera muito ruído (mutantes
"equivalentes") para pouco sinal.

DTOs (`*Dtos.cs`) e `DependencyInjection.cs` são excluídos do escopo de
mutação porque são código estrutural sem lógica condicional — mutá-los
só infla o denominador do score sem testar nada relevante.

## Instalar (uma vez por máquina)

```bash
dotnet tool install -g dotnet-stryker
```

## Rodar

A partir da pasta `tests/SmartBus.UnitTests`:

```bash
dotnet stryker
```

O relatório HTML abre em `StrykerOutput/<data>/reports/mutation-report.html`.

## Threshold

- **break: 50** — build falha (exit code != 0) se o mutation score cair
  abaixo de 50%. Pense nisso como o "não regredir" pra CI.
- **low/high: 60/80** — só afeta a cor do relatório (vermelho/amarelo/verde),
  não quebra o build.

Ajuste esses números conforme o projeto amadurece — começar com break
baixo (50) e subir aos poucos é mais sustentável do que começar em 80
e o time aprender a ignorar o Stryker.
