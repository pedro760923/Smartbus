# Testes E2E (Playwright)

## Instalar

```bash
npm install
npx playwright install --with-deps chromium
```

## Rodar

A API precisa estar de pé antes (o Playwright sobe o Angular sozinho
via `webServer` no `playwright.config.ts`, mas não sobe o backend):

```bash
# terminal 1
cd ../backend/src/SmartBus.Api && dotnet run

# terminal 2
cd ../frontend && npx playwright test
```

Modo interativo (útil pra debugar seletor que não bate):

```bash
npx playwright test --ui
```

## Cobertura atual

- `auth.spec.ts` — cadastro com sucesso, login com credenciais inválidas.
- `reportar-lotacao.spec.ts` — listagem de linhas do seed, busca por código.

Ainda não cobrimos o envio de reporte em si (POST /api/reportes) fim a
fim pela UI porque o componente `reportar-lotacao` depende de geo-
localização do navegador — dá pra mockar com
`page.context().grantPermissions(['geolocation'])` +
`page.context().setGeolocation(...)`, mas isso fica pra uma próxima
iteração pra não travar a entrega desse primeiro lote de testes.
