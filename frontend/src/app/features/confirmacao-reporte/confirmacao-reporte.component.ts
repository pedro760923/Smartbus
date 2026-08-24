import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-confirmacao-reporte',
  standalone: true,
  template: `
    <main class="confirmacao">
      <button class="voltar" (click)="fechar()" aria-label="Voltar">‹</button>

      <div class="confirmacao__selo">
        <svg width="52" height="52" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M5 12.5 10 17 19 7" stroke="#fff" stroke-width="2.6" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </div>

      <h1>REPORTE CONFIRMADO!</h1>
      <h2>Obrigado, {{ auth.usuario()?.nome ?? 'você' }}!</h2>
      <p>
        Seu reporte foi registrado com sucesso e já está ajudando nossa
        comunidade a melhorar as rotas para todos.
      </p>

      <div class="confirmacao__rodape">
        <span>🌿</span>
        Obrigado pela sua contribuição contínua!
      </div>

      <button class="btn btn--primario" (click)="fechar()">Fechar</button>
    </main>
  `,
  styleUrl: './confirmacao-reporte.component.scss'
})
export class ConfirmacaoReporteComponent {
  constructor(public auth: AuthService, private router: Router) {}

  fechar(): void {
    this.router.navigate(['/principal']);
  }
}
