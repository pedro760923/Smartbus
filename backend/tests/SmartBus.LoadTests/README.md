# Testes de carga (k6)

## Instalar o k6

```bash
# macOS
brew install k6
# Windows (choco)
choco install k6
# Linux (apt)
sudo gpg -k && sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6ACFD8
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update && sudo apt-get install k6
```

## Rodar (com a API rodando localmente)

```bash
k6 run linhas-listar.js
k6 run reportes-enviar.js
k6 run login.js

# apontando para outro ambiente:
k6 run -e BASE_URL=https://staging.smartbus.exemplo.com linhas-listar.js
```

## Cenários

| Script                | O que testa                                            | Perfil de carga                          |
|-----------------------|---------------------------------------------------------|-------------------------------------------|
| `linhas-listar.js`    | Throughput do endpoint de leitura mais usado             | 30 VUs constantes por 1min                |
| `reportes-enviar.js`  | Escrita concorrente (crowdsourcing) no horário de pico    | Rampa 0→50 VUs, sustenta, desce            |
| `login.js`            | Custo de BCrypt.Verify sob taxa de chegada constante      | 20 req/s constantes por 30s                |

Cada script já define seus próprios `thresholds` (p95/p99 de latência,
taxa de erro máxima) — o `k6 run` sai com código de erro se algum
threshold for violado, então dá pra plugar direto num pipeline de CI
como gate de performance.

**Atenção:** `reportes-enviar.js` e `login.js` criam usuários reais no
banco a cada execução (não há endpoint de limpeza). Rode contra um
banco de desenvolvimento/staging, nunca contra produção.
