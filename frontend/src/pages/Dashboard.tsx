import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { eventsApi } from "../api/events";
import { inscriptionsApi } from "../api/inscriptions";
import { groupsApi } from "../api/groups";
import { extractErrorMessage } from "../api/client";
import { Alert, Card, PageLoader, StatusBadge } from "../components/ui";
import { getRoleLabel, formatDate } from "../utils/format";
import type { EventDto, InscriptionDto } from "../types";

export default function Dashboard() {
  const { user } = useAuth();

  const [publishedCount, setPublishedCount] = useState(0);
  const [myGroupsCount, setMyGroupsCount] = useState(0);
  const [myInscriptions, setMyInscriptions] = useState<InscriptionDto[]>([]);
  const [recentEvents, setRecentEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const isAdmin = user?.roles.includes("Admin");
  const isGestor = user?.roles.includes("Gestor");
  const isGroup = user?.roles.includes("Grupo");

  useEffect(() => {
    let mounted = true;
    const load = async () => {
      try {
        const evts = await eventsApi.getPublished({ page: 1, pageSize: 4 });
        if (!mounted) return;
        setPublishedCount(evts.totalItems);
        setRecentEvents(evts.items);

        if (isGroup) {
          const [groups, insc] = await Promise.all([
            groupsApi.getMyGroups(),
            inscriptionsApi.getMyInscriptions(),
          ]);
          if (!mounted) return;
          setMyGroupsCount(groups.length);
          setMyInscriptions(insc);
        }
      } catch (err) {
        if (mounted) setError(extractErrorMessage(err));
      } finally {
        if (mounted) setLoading(false);
      }
    };
    load();
    return () => {
      mounted = false;
    };
  }, [isGroup]);

  if (loading) return <PageLoader />;
  if (error) return <Alert tone="error">{error}</Alert>;

  const roleLabel = user ? getRoleLabel(user.roles[0]) : "";

  const pendingCount = myInscriptions.filter((i) => i.status === "Pending").length;
  const approvedCount = myInscriptions.filter((i) => i.status === "Approved").length;

  return (
    <div className="container page">
      {/* ── Welcome ── */}
      <div className="welcome-banner">
        <div className="welcome-banner-inner">
          <div>
            <h1>Hola, {user?.firstName} 👋</h1>
            <p className="welcome-role">
              <span className="role-badge">{roleLabel}</span>
              Panel de control — DanzaBogota
            </p>
          </div>
          <div className="welcome-actions">
            {(isAdmin || isGestor) && (
              <Link to="/admin/events" className="btn btn-primary btn-sm">
                Gestionar eventos
              </Link>
            )}
            {isGroup && (
              <Link to="/my-groups" className="btn btn-primary btn-sm">
                Mis grupos
              </Link>
            )}
          </div>
        </div>
      </div>

      {/* ── Stats ── */}
      <div className="stats-grid">
        <Card className="stat-card stat-card--primary">
          <div className="stat-icon" aria-hidden="true">🎭</div>
          <div>
            <p className="stat-value">{publishedCount}</p>
            <p className="stat-label">Eventos publicados</p>
          </div>
        </Card>

        {isGroup && (
          <>
            <Card className="stat-card">
              <div className="stat-icon" aria-hidden="true">🩰</div>
              <div>
                <p className="stat-value">{myGroupsCount}</p>
                <p className="stat-label">Mis grupos</p>
              </div>
            </Card>
            <Card className="stat-card">
              <div className="stat-icon" aria-hidden="true">📝</div>
              <div>
                <p className="stat-value">{myInscriptions.length}</p>
                <p className="stat-label">Inscripciones totales</p>
              </div>
            </Card>
            <Card className="stat-card stat-card--warn">
              <div className="stat-icon" aria-hidden="true">⏳</div>
              <div>
                <p className="stat-value">{pendingCount}</p>
                <p className="stat-label">Pendientes de revisión</p>
              </div>
            </Card>
            <Card className="stat-card stat-card--ok">
              <div className="stat-icon" aria-hidden="true">✅</div>
              <div>
                <p className="stat-value">{approvedCount}</p>
                <p className="stat-label">Aprobadas</p>
              </div>
            </Card>
          </>
        )}

        {(isAdmin || isGestor) && (
          <>
            <Card className="stat-card">
              <div className="stat-icon" aria-hidden="true">🗂️</div>
              <div>
                <p className="stat-value">—</p>
                <p className="stat-label">Inscripciones pendientes</p>
              </div>
            </Card>
          </>
        )}
      </div>

      {/* ── Accesos rápidos ── */}
      <section className="dashboard-section">
        <h2>Accesos rápidos</h2>
        <div className="quick-access-grid">
          {isAdmin && (
            <>
              <Link to="/admin/events" className="quick-access-card">
                <span className="quick-access-icon">🗓️</span>
                <span>Eventos</span>
              </Link>
              <Link to="/admin/event-types" className="quick-access-card">
                <span className="quick-access-icon">🏷️</span>
                <span>Tipos de evento</span>
              </Link>
              <Link to="/admin/groups" className="quick-access-card">
                <span className="quick-access-icon">🩰</span>
                <span>Grupos</span>
              </Link>
              <Link to="/admin/inscriptions" className="quick-access-card">
                <span className="quick-access-icon">📋</span>
                <span>Inscripciones</span>
              </Link>
              <Link to="/admin/users" className="quick-access-card">
                <span className="quick-access-icon">👥</span>
                <span>Usuarios</span>
              </Link>
            </>
          )}
          {isGestor && (
            <>
              <Link to="/admin/events" className="quick-access-card">
                <span className="quick-access-icon">🗓️</span>
                <span>Gestionar eventos</span>
              </Link>
              <Link to="/admin/inscriptions" className="quick-access-card">
                <span className="quick-access-icon">📋</span>
                <span>Revisar inscripciones</span>
              </Link>
              <Link to="/events" className="quick-access-card">
                <span className="quick-access-icon">👁️</span>
                <span>Ver eventos públicos</span>
              </Link>
            </>
          )}
          {isGroup && (
            <>
              <Link to="/my-groups" className="quick-access-card">
                <span className="quick-access-icon">🩰</span>
                <span>Mis grupos</span>
              </Link>
              <Link to="/my-inscriptions" className="quick-access-card">
                <span className="quick-access-icon">📝</span>
                <span>Mis inscripciones</span>
              </Link>
              <Link to="/participants" className="quick-access-card">
                <span className="quick-access-icon">👥</span>
                <span>Participantes</span>
              </Link>
              <Link to="/events" className="quick-access-card">
                <span className="quick-access-icon">🎭</span>
                <span>Explorar eventos</span>
              </Link>
            </>
          )}
        </div>
      </section>

      {/* ── Inscripciones recientes (Grupo) ── */}
      {isGroup && myInscriptions.length > 0 && (
        <section className="dashboard-section">
          <h2>Mis inscripciones recientes</h2>
          <div className="recent-inscriptions-list">
            {myInscriptions.slice(0, 5).map((insc) => (
              <div key={insc.id} className="recent-inscription-row">
                <div className="recent-inscription-info">
                  <span className="recent-inscription-event">{insc.eventTitle}</span>
                  <span className="recent-inscription-group">{insc.groupName}</span>
                </div>
                <StatusBadge status={insc.status} />
              </div>
            ))}
          </div>
          {myInscriptions.length > 5 && (
            <div className="section-actions" style={{ marginTop: "0.75rem" }}>
              <Link to="/my-inscriptions" className="btn btn-ghost btn-sm">
                Ver todas ({myInscriptions.length})
              </Link>
            </div>
          )}
        </section>
      )}

      {/* ── Eventos recientes publicados ── */}
      {recentEvents.length > 0 && (
        <section className="dashboard-section">
          <h2>Eventos abiertos en la plataforma</h2>
          <div className="recent-events-list">
            {recentEvents.map((event) => (
              <Link
                key={event.id}
                to={`/events/${event.id}`}
                className="recent-event-row"
              >
                <div className="recent-event-info">
                  <span className="recent-event-title">{event.title}</span>
                  <span className="recent-event-meta">
                    {event.localityName} · {formatDate(event.startDate)}
                  </span>
                </div>
                <StatusBadge status={event.status} />
              </Link>
            ))}
          </div>
          <div className="section-actions" style={{ marginTop: "0.75rem" }}>
            <Link to="/events" className="btn btn-ghost btn-sm">
              Ver todos los eventos
            </Link>
          </div>
        </section>
      )}
    </div>
  );
}
