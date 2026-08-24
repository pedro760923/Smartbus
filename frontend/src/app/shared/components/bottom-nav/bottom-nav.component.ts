import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-bottom-nav',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav class="bottom-nav">
      <a routerLink="/principal" routerLinkActive="ativo">
        <span>🏠</span>
        <small>Início</small>
      </a>
      <a routerLink="/linhas" routerLinkActive="ativo">
        <span>🚌</span>
        <small>Linhas</small>
      </a>
      <a routerLink="/reportar" routerLinkActive="ativo">
        <span>📢</span>
        <small>Reportar</small>
      </a>
      @if (auth.isAdmin()) {
        <a routerLink="/dashboard" routerLinkActive="ativo">
          <span>📊</span>
          <small>Painel</small>
        </a>
      }
    </nav>
  `,
  styleUrl: './bottom-nav.component.scss'
})
export class BottomNavComponent {
  constructor(public auth: AuthService) {}
}
