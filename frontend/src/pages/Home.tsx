import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { eventsApi } from "../api/events";
import { extractErrorMessage } from "../api/client";
import type { EventDto } from "../types";
import { Card, PageLoader, Alert, EmptyState } from "../components/ui";
import { StatusBadge } from "../components/ui";
import { formatDate } from "../utils/format";
import { useAuth } from "../auth/AuthContext";

const features = [
  {
    icon: "🎭",
    title: "Encuentra eventos",
    description:
      "Descubre festivales, muestras y encuentros de danza cultural en las 20 localidades de Bogotá.",
  },
  {
    icon: "🩰",
    title: "Inscribe tu grupo",
    description:
      "Los grupos de danza pueden registrarse, cargar su RUT y aplicar a los eventos disponibles.",
  },
  {
    icon: "🗓️",
    title: "Gestión integral",
    description:
      "Administradores y gestores controlan eventos, inscripciones y participantes en una sola plataforma.",
  },
];

const LOCALITIES = [
  "Usaquén","Chapinero","Santa Fe","San Cristóbal","Usme",
  "Tunjuelito","Bosa","Kennedy","Fontibón","Engativá",
  "Suba","Barrios Unidos","Teusaquillo","Los Mártires","Antonio Nariño",
  "Puente Aranda","La Candelaria","Rafael Uribe Uribe","Ciudad Bolívar","Sumapaz",
];

export default function Home() {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [totalEvents, setTotalEvents] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    eventsApi
      .getPublished({ page: 1, pageSize: 3 })
      .then((data) => {
        setEvents(data.items);
        setTotalEvents(data.totalItems);
      })
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div>
      {/* ── Hero ── */}
      <section className="hero">
        <div className="hero-inner">
          <p className="hero-eyebrow">Plataforma cultural de Bogotá</p>
          <h1>La danza cultural que mueve a Bogotá</h1>
          <p className="hero-subtitle">
            DanzaBogota conecta a los grupos de danza con los eventos culturales de la ciudad:
            inscripciones digitales, gestión de participantes y control administrativo en un solo
            lugar.
          </p>
          <div className="hero-actions">
            <Link to="/events" className="btn btn-primary">
              Explorar eventos
            </Link>
            {!isAuthenticated && (
              <Link to="/register" className="btn btn-secondary">
                Registrarme
              </Link>
            )}
          </div>
        </div>
      </section>

      {/* ── Stats ── */}
      <section className="section section-stats">
        <div className="container">
          <div className="home-stats-grid">
            <div className="home-stat-card">
              <span className="home-stat-icon">🗺️</span>
              <span className="home-stat-value">{LOCALITIES.length}</span>
              <span className="home-stat-label">Localidades de Bogotá</span>
            </div>
            <div className="home-stat-card">
              <span className="home-stat-icon">🩰</span>
              <span className="home-stat-value">100%</span>
              <span className="home-stat-label">Digital y gratuito</span>
            </div>
            <div className="home-stat-card">
              <span className="home-stat-icon">🎭</span>
              <span className="home-stat-value">
                {totalEvents !== null ? totalEvents : "—"}
              </span>
              <span className="home-stat-label">Convocatorias activas</span>
            </div>
          </div>
        </div>
      </section>

      {/* ── Cómo funciona ── */}
      <section className="section">
        <div className="container">
          <div className="section-heading">
            <h2>¿Cómo funciona?</h2>
            <p>Una plataforma pensada para la comunidad de danza de Bogotá.</p>
          </div>
          <div className="features-grid">
            {features.map((f) => (
              <Card key={f.title} className="feature-card">
                <span className="feature-icon" aria-hidden="true">
                  {f.icon}
                </span>
                <h3>{f.title}</h3>
                <p>{f.description}</p>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* ── Próximos eventos ── */}
      <section className="section section-alt">
        <div className="container">
          <div className="section-heading">
            <h2>Próximos eventos</h2>
            <p>Muestras culturales publicadas por los gestores de la plataforma.</p>
          </div>

          {loading ? (
            <PageLoader />
          ) : error ? (
            <Alert tone="error">{error}</Alert>
          ) : events.length === 0 ? (
            <EmptyState
              title="Aún no hay eventos publicados"
              message="Vuelve pronto para conocer las convocatorias disponibles."
            />
          ) : (
            <div className="event-grid">
              {events.map((event) => (
                <Link to={`/events/${event.id}`} key={event.id} className="event-card-link">
                  <Card className="event-card">
                    <div className="event-card-body">
                      <div className="event-card-meta">
                        <span>{event.localityName}</span>
                        <StatusBadge status={event.status} />
                      </div>
                      <h3>{event.title}</h3>
                      <p className="event-card-date">{formatDate(event.startDate)}</p>
                      <p className="event-card-location">
                        {event.locationName} — {event.address}
                      </p>
                    </div>
                  </Card>
                </Link>
              ))}
            </div>
          )}

          <div className="section-actions">
            <Link to="/events" className="btn btn-secondary">
              Ver todos los eventos
            </Link>
          </div>
        </div>
      </section>

      {/* ── Localidades ── */}
      <section className="section section-localities">
        <div className="container">
          <div className="section-heading">
            <h2>Cobertura en toda Bogotá</h2>
            <p>Eventos y grupos en las 20 localidades del Distrito Capital.</p>
          </div>
          <div className="localities-wrap">
            {LOCALITIES.map((loc) => (
              <span key={loc} className="locality-chip">
                {loc}
              </span>
            ))}
          </div>
        </div>
      </section>

      {/* ── CTA registro ── */}
      {!isAuthenticated && (
        <section className="section section-cta">
          <div className="container">
            <div className="cta-box">
              <h2>¿Tienes un grupo de danza?</h2>
              <p>
                Regístrate gratis, completa el perfil de tu grupo y postúlate a los eventos
                disponibles en tu localidad.
              </p>
              <div className="hero-actions">
                <Link to="/register" className="btn btn-primary">
                  Crear cuenta
                </Link>
                <Link to="/login" className="btn btn-ghost">
                  Ya tengo cuenta
                </Link>
              </div>
            </div>
          </div>
        </section>
      )}
    </div>
  );
}
