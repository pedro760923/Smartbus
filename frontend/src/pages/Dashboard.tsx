import { useEffect, useRef, useState } from 'react';
import * as L from 'leaflet';
import { api } from '../lib/api';
import { Header } from '../components/Header';
import type { DashboardKpis } from '../types';
import './Dashboard.css';

export function Dashboard() {
  const [kpis, setKpis] = useState<DashboardKpis | null>(null);
  const mapaRef = useRef<HTMLDivElement>(null);
  const mapaInstancia = useRef<L.Map | null>(null);

  useEffect(() => {
    api.get<DashboardKpis>('/dashboard/kpis').then(({ data }) => setKpis(data));
  }, []);

  useEffect(() => {
    if (!kpis || !mapaRef.current || mapaInstancia.current) return;

    const pontos = kpis.mapaDeCalor;
    const centro = pontos[0] ?? { latitude: -23.6486, longitude: -46.5389 };

    const mapa = L.map(mapaRef.current).setView([centro.latitude, centro.longitude], 14);
    mapaInstancia.current = mapa;

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19
    }).addTo(mapa);

    pontos.forEach((ponto) => {
      L.circleMarker([ponto.latitude, ponto.longitude], {
        radius: 8 + ponto.intensidade * 12,
        color: '#c0392b',
        fillColor: '#e74c3c',
        fillOpacity: 0.5
      })
        .addTo(mapa)
        .bindPopup(`${ponto.paradaNome} · intensidade ${(ponto.intensidade * 100).toFixed(0)}%`);
    });

    return () => {
      mapa.remove();
      mapaInstancia.current = null;
    };
  }, [kpis]);

  return (
    <>
      <Header titulo="Painel administrativo" />

      {kpis ? (
        <main className="dashboard">
          <section className="kpis">
            <div className="kpi">
              <span className="kpi__valor">{kpis.totalReportesHoje}</span>
              <span className="kpi__label">Reportes hoje</span>
            </div>
            <div className="kpi">
              <span className="kpi__valor">{kpis.totalLinhasMonitoradas}</span>
              <span className="kpi__label">Linhas monitoradas</span>
            </div>
          </section>

          <section className="ranking">
            <h3>Linhas com maior superlotação</h3>
            <ul>
              {kpis.linhasComMaiorSuperlotacao.map((item) => (
                <li key={item.linhaNome}>
                  <span>{item.linhaNome}</span>
                  <strong>{item.percentualSuperlotado}%</strong>
                </li>
              ))}
            </ul>
          </section>

          <section className="previsao-espera">
            <h3>Previsão de tempo de espera</h3>
            <ul>
              {kpis.previsaoTempoEspera.map((item) => (
                <li key={item.faixaHoraria}>
                  <span>{item.faixaHoraria}</span>
                  <strong>{item.minutosEsperaEstimados} min</strong>
                </li>
              ))}
            </ul>
          </section>

          <section className="mapa-calor">
            <h3>Mapa de calor de ocupação</h3>
            <div ref={mapaRef} className="mapa-calor__mapa" />
          </section>
        </main>
      ) : (
        <p className="carregando">Carregando indicadores...</p>
      )}
    </>
  );
}
