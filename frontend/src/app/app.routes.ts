import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'splash', pathMatch: 'full' },
  {
    path: 'splash',
    loadComponent: () => import('./features/splash/splash.component').then((m) => m.SplashComponent)
  },
  {
    path: 'boas-vindas',
    loadComponent: () =>
      import('./features/boas-vindas/boas-vindas.component').then((m) => m.BoasVindasComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () =>
      import('./features/registro/registro.component').then((m) => m.RegistroComponent)
  },
  {
    path: 'principal',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/principal/principal.component').then((m) => m.PrincipalComponent)
  },
  {
    path: 'linhas',
    canActivate: [authGuard],
    loadComponent: () => import('./features/linhas/linhas.component').then((m) => m.LinhasComponent)
  },
  {
    path: 'mapa-rotas/:linhaId',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/mapa-rotas/mapa-rotas.component').then((m) => m.MapaRotasComponent)
  },
  {
    path: 'reportar',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/reportar-lotacao/reportar-lotacao.component').then(
        (m) => m.ReportarLotacaoComponent
      )
  },
  {
    path: 'confirmacao',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/confirmacao-reporte/confirmacao-reporte.component').then(
        (m) => m.ConfirmacaoReporteComponent
      )
  },
  {
    path: 'dashboard',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
  },
  { path: '**', redirectTo: 'boas-vindas' }
];
