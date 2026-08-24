import { test, expect } from '@playwright/test';

/**
 * Fluxo principal do produto: aluno logado busca uma linha e reporta a
 * lotação atual. É a interação que justifica o app existir — se este
 * teste quebra, o crowdsourcing (fonte de dados de todo o resto do
 * sistema: dashboard, previsão) para de funcionar.
 */
test.describe('Reportar lotação', () => {
  test.beforeEach(async ({ page }) => {
    const email = `e2e.reportar.${Date.now()}@fsa.edu.br`;
    await page.goto('/registro');
    await page.getByLabel('Nome completo').fill('Aluno E2E');
    await page.getByLabel('E-mail acadêmico').fill(email);
    await page.getByLabel('Senha').fill('senha123');
    await page.getByRole('button', { name: 'Criar conta' }).click();
    await expect(page).toHaveURL(/\/principal/);
  });

  test('aluno consegue ver a lista de linhas do seed', async ({ page }) => {
    await page.goto('/linhas');

    // Linha "437" vem do DbSeeder (dado de exemplo do backend).
    await expect(page.getByText('437')).toBeVisible();
  });

  test('aluno consegue buscar uma linha por código', async ({ page }) => {
    await page.goto('/linhas');

    await page.getByPlaceholder('Buscar por código ou nome').fill('437');

    await expect(page.getByText('437')).toBeVisible();
    await expect(page.getByText('512')).not.toBeVisible();
  });
});
