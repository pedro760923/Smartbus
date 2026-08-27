import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../lib/auth';

export function RequireAuth() {
  const { autenticado } = useAuth();
  return autenticado ? <Outlet /> : <Navigate to="/login" replace />;
}

export function RequireAdmin() {
  const { isAdmin } = useAuth();
  return isAdmin ? <Outlet /> : <Navigate to="/principal" replace />;
}
