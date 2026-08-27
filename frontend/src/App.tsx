import { Navigate, Route, Routes } from 'react-router-dom';
import { RequireAdmin, RequireAuth } from './components/RequireAuth';
import { Splash } from './pages/Splash';
import { BoasVindas } from './pages/BoasVindas';
import { Login } from './pages/Login';
import { Registro } from './pages/Registro';
import { Principal } from './pages/Principal';
import { OnibusDisponiveis } from './pages/OnibusDisponiveis';
import { MapaRotas } from './pages/MapaRotas';
import { ReportarLotacao } from './pages/ReportarLotacao';
import { ConfirmacaoReporte } from './pages/ConfirmacaoReporte';
import { Dashboard } from './pages/Dashboard';

export default function App() {
  return (
    <div className="app-shell">
      <div className="app-shell__tela">
        <Routes>
          <Route path="/" element={<Navigate to="/splash" replace />} />
          <Route path="/splash" element={<Splash />} />
          <Route path="/boas-vindas" element={<BoasVindas />} />
          <Route path="/login" element={<Login />} />
          <Route path="/registro" element={<Registro />} />

          <Route element={<RequireAuth />}>
            <Route path="/principal" element={<Principal />} />
            <Route path="/linhas" element={<OnibusDisponiveis />} />
            <Route path="/mapa-rotas/:linhaId" element={<MapaRotas />} />
            <Route path="/reportar" element={<ReportarLotacao />} />
            <Route path="/confirmacao" element={<ConfirmacaoReporte />} />

            <Route element={<RequireAdmin />}>
              <Route path="/dashboard" element={<Dashboard />} />
            </Route>
          </Route>

          <Route path="*" element={<Navigate to="/boas-vindas" replace />} />
        </Routes>
      </div>
    </div>
  );
}
