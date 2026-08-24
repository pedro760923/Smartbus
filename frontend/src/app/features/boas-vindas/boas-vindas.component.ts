import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-boas-vindas',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="aviso">
      <div class="aviso__ilustracao">
        <svg width="120" height="120" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <circle cx="12" cy="12" r="12" fill="#eaf1fc"/>
          <rect x="5" y="9" width="14" height="9" rx="3" fill="#1e4d94"/>
          <circle cx="8.5" cy="18.5" r="1.5" fill="#133769"/>
          <circle cx="15.5" cy="18.5" r="1.5" fill="#133769"/>
          <path d="M7 12.5h10M7 15.5h6" stroke="#fff" stroke-width="1.2" stroke-linecap="round"/>
          <path d="M7 6.5 9 9M17 6.5 15 9" stroke="#2fa360" stroke-width="1.4" stroke-linecap="round"/>
        </svg>
      </div>

      <h1>Entenda o SmartBus<br />Santo André</h1>

      <ul class="aviso__passos">
        <li>
          <span class="aviso__icone">📍</span>
          Veja o ônibus no mapa
        </li>
        <li>
          <span class="aviso__icone">⏱️</span>
          Verifique o tempo e lotação
        </li>
        <li>
          <span class="aviso__icone">📣</span>
          Colabore e informe
        </li>
      </ul>

      <p class="aviso__legenda">
        Rastreamento em tempo real e lotação colaborativa na sua cidade.
      </p>

      <a routerLink="/login" class="btn btn--primario">COMEÇAR VIAGEM</a>
      <p class="aviso__rodape">
        Já tem conta? <a routerLink="/login">Entrar</a> · <a routerLink="/registro">Criar conta</a>
      </p>
    </div>
  `,
  styleUrl: './boas-vindas.component.scss'
})
export class BoasVindasComponent {}
