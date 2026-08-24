import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="app-header">
      <span class="app-header__titulo">{{ titulo }}</span>
      @if (auth.autenticado()) {
        <button class="app-header__sair" (click)="sair()" aria-label="Sair">Sair</button>
      }
    </header>
  `,
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  @Input() titulo = 'SmartBus';

  constructor(public auth: AuthService, private router: Router) {}

  sair(): void {
    this.auth.logout();
    this.router.navigate(['/boas-vindas']);
  }
}
