import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-page">
      <h1>Entrar</h1>
      <form [formGroup]="form" (ngSubmit)="entrar()">
        <label>
          E-mail acadêmico
          <input type="email" formControlName="email" placeholder="voce@fsa.edu.br" />
        </label>
        <label>
          Senha
          <input type="password" formControlName="senha" placeholder="••••••••" />
        </label>

        @if (erro) {
          <p class="erro">{{ erro }}</p>
        }

        <button class="btn btn--primario" type="submit" [disabled]="form.invalid || carregando">
          {{ carregando ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>
      <p class="auth-page__rodape">
        Ainda não tem conta? <a routerLink="/registro">Cadastre-se</a>
      </p>
    </div>
  `,
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  erro = '';
  carregando = false;
  form: ReturnType<FormBuilder['group']>;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  entrar(): void {
    if (this.form.invalid) return;
    this.carregando = true;
    this.erro = '';

    this.auth
      .login({ email: this.form.value.email!, senha: this.form.value.senha! })
      .subscribe({
        next: () => this.router.navigate(['/principal']),
        error: () => {
          this.erro = 'E-mail ou senha inválidos.';
          this.carregando = false;
        }
      });
  }
}
