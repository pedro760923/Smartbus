import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-page">
      <h1>Criar conta</h1>
      <form [formGroup]="form" (ngSubmit)="registrar()">
        <label>
          Nome completo
          <input type="text" formControlName="nome" placeholder="Seu nome" />
        </label>
        <label>
          E-mail acadêmico
          <input type="email" formControlName="email" placeholder="voce@fsa.edu.br" />
        </label>
        <label>
          Senha
          <input type="password" formControlName="senha" placeholder="mínimo 6 caracteres" />
        </label>

        @if (erro) {
          <p class="erro">{{ erro }}</p>
        }

        <button class="btn btn--primario" type="submit" [disabled]="form.invalid || carregando">
          {{ carregando ? 'Criando...' : 'Criar conta' }}
        </button>
      </form>
      <p class="auth-page__rodape">
        Já tem conta? <a routerLink="/login">Entrar</a>
      </p>
    </div>
  `,
  styleUrl: './registro.component.scss'
})
export class RegistroComponent {
  erro = '';
  carregando = false;
  form: ReturnType<FormBuilder['group']>;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      nome: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  registrar(): void {
    if (this.form.invalid) return;
    this.carregando = true;
    this.erro = '';

    this.auth
      .registrar({
        nome: this.form.value.nome!,
        email: this.form.value.email!,
        senha: this.form.value.senha!
      })
      .subscribe({
        next: () => this.router.navigate(['/principal']),
        error: () => {
          this.erro = 'Não foi possível criar sua conta. Verifique os dados.';
          this.carregando = false;
        }
      });
  }
}
