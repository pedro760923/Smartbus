import { test, expect } from '@playwright/test';

/**
 * Fluxo de cadastro + login. É o caminho crítico de entrada no app:
 * se quebrar, ninguém consegue usar mais nada — por isso é o primeiro
 * E2E que escrevemos.
 */
test.describe('Autenticação', () => {
  test('aluno consegue criar conta e cair na tela principal', async ({ page }) => {
    const email = `e2e.${Date.now()}@fsa.edu.br`;

    await page.goto('/registro');

    await page.getByLabel('Nome completo').fill('Aluno E2E');
    await page.getByLabel('E-mail acadêmico').fill(email);
    await page.getByLabel('Senha').fill('senha123');
    await page.getByRole('button', { name: 'Criar conta' }).click();

    await expect(page).toHaveURL(/\/principal/);
  });

  test('login com senha errada mostra mensagem de erro e não navega', async ({ page }) => {
    await page.goto('/login');

    await page.getByLabel('E-mail acadêmico').fill('naoexiste@fsa.edu.br');
    await page.getByLabel('Senha').fill('senhaerrada');
    await page.getByRole('button', { name: 'Entrar' }).click();

    await expect(page.getByText('E-mail ou senha inválidos.')).toBeVisible();
    await expect(page).toHaveURL(/\/login/);
  });
});
