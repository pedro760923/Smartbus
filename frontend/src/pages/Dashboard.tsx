import { useEffect, useRef, useState } from 'react';
import { api } from '../lib/api';
import { carregarGoogleMaps } from '../lib/googleMaps';
import { Header } from '../components/Header';
import type { DashboardKpis } from '../types';
import './Dashboard.css';

export function Dashboard() {
  const [kpis, setKpis] = useState<DashboardKpis | null>(null);
  const mapaRef = useRef<HTMLDivElement>(null);
  const mapaInstancia = useRef<google.maps.Map | null>(null);

  useEffect(() => {
    api.get<DashboardKpis>('/dashboard/kpis').then(({ data }) => setKpis(data));
  }, []);

  useEffect(() => {
    if (!kpis || !mapaRef.current || mapaInstancia.current) return;

    const pontos = kpis.mapaDeCalor;
    const centro = pontos[0] ?? { latitude: -23.6486, longitude: -46.5389 };

    carregarGoogleMaps().then(() => {
      if (!mapaRef.current || mapaInstancia.current) return;

      const mapa = new google.maps.Map(mapaRef.current, {
        center: { lat: centro.latitude, lng: centro.longitude },
        zoom: 14
      });
      mapaInstancia.current = mapa;

      pontos.forEach((ponto) => {
        const circulo = new google.maps.Circle({
          map: mapa,
          center: { lat: ponto.latitude, lng: ponto.longitude },
          radius: 30 + ponto.intensidade * 120,
          strokeColor: '#c0392b',
          fillColor: '#e74c3c',
          fillOpacity: 0.5
        });

        const infoWindow = new google.maps.InfoWindow({
          content: `${ponto.paradaNome} · intensidade ${(ponto.intensidade * 100).toFixed(0)}%`
        });
        circulo.addListener('click', () => infoWindow.open({ map: mapa, anchor: circulo }));
      });
    });

    return () => {
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
