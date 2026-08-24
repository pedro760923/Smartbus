import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-splash',
  standalone: true,
  template: `
    <div class="splash">
      <div class="splash__logo">
        <svg width="72" height="72" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="3" y="4" width="18" height="12" rx="4" fill="#1e4d94"/>
          <circle cx="7.5" cy="18.5" r="1.6" fill="#1e4d94"/>
          <circle cx="16.5" cy="18.5" r="1.6" fill="#1e4d94"/>
          <path d="M6 8h12M6 11.5h8" stroke="#fff" stroke-width="1.4" stroke-linecap="round"/>
          <path d="M17 3.5c1.4.4 2.4 1.5 2.7 3" stroke="#2fa360" stroke-width="1.4" stroke-linecap="round"/>
        </svg>
      </div>
      <h1>SmartBus</h1>
      <p>Santo André</p>
    </div>
  `,
  styleUrl: './splash.component.scss'
})
export class SplashComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit(): void {
    setTimeout(() => this.router.navigateByUrl('/boas-vindas'), 1400);
  }
}
