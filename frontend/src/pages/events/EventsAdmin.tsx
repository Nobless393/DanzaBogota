import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { eventsApi } from "../../api/events";
import { extractErrorMessage } from "../../api/client";
import type { EventDto } from "../../types";
import {
  Alert,
  Button,
  ConfirmModal,
  EmptyState,
  PageHeader,
  PageLoader,
  Pagination,
  Select,
  StatusBadge,
} from "../../components/ui";
import { formatDateTime } from "../../utils/format";
import { useAuth } from "../../auth/AuthContext";

const PAGE_SIZE = 10;

/** Devuelve true si el evento ya terminó o faltan ≤5 días para su inicio */
function isLocked(event: EventDto): boolean {
  const now = new Date();
  const start = new Date(event.startDate);
  const end = new Date(event.endDate);
  if (end < now) return true; // ya finalizó
  const diffDays = (start.getTime() - now.getTime()) / (1000 * 60 * 60 * 24);
  return diffDays <= 5; // faltan 5 días o menos
}

export default function EventsAdmin() {
  const { user } = useAuth();
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [toast, setToast] = useState("");
  const [target, setTarget] = useState<{ event: EventDto; action: "publish" | "cancel" | "delete" } | null>(null);
  const [busy, setBusy] = useState(false);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);
  const [statusFilter, setStatusFilter] = useState("");

  const isAdmin = user?.roles.includes("Admin");

  const load = () => {
    setLoading(true);
    setError("");
    const params = {
      page,
      pageSize: PAGE_SIZE,
      status: statusFilter || undefined,
    };
    eventsApi
      .getAll(params)
      .then((data) => {
        setEvents(data.items);
        setTotalPages(data.totalPages);
        setTotalItems(data.totalItems);
      })
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  };

  useEffect(load, [page, statusFilter]);

  const showToast = (message: string) => {
    setToast(message);
    window.setTimeout(() => setToast(""), 4000);
  };

  const runAction = async () => {
    if (!target) return;
    setBusy(true);
    try {
      let message = "";
      if (target.action === "publish") message = await eventsApi.publish(target.event.id);
      if (target.action === "cancel") message = await eventsApi.cancel(target.event.id);
      if (target.action === "delete") message = await eventsApi.remove(target.event.id);
      showToast(message);
      setTarget(null);
      load();
    } catch (err) {
      setError(extractErrorMessage(err));
      setTarget(null);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="container page">
      <PageHeader
        title="Gestión de eventos"
        subtitle="Crea, publica y administra los eventos culturales de la plataforma."
        actions={
          <Link to="/admin/events/new" className="btn btn-primary">
            + Nuevo evento
          </Link>
        }
      />

      {toast && <Alert tone="success">{toast}</Alert>}
      {error && <Alert tone="error">{error}</Alert>}

      <div className="toolbar">
        <Select value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}>
          <option value="">Todos los estados</option>
          <option value="Draft">Borrador</option>
          <option value="Published">Publicado</option>
          <option value="Closed">Cerrado</option>
          <option value="Cancelled">Cancelado</option>
          <option value="Finished">Finalizado</option>
        </Select>
      </div>

      {loading ? (
        <PageLoader />
      ) : events.length === 0 ? (
        <EmptyState title="No hay eventos" message="Crea el primer evento cultural con el botón 'Nuevo evento'." />
      ) : (
        <>
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Evento</th>
                  <th>Fechas</th>
                  <th>Localidad</th>
                  <th>Estado</th>
                  <th>Cupos</th>
                  <th className="table-actions-col">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {events.map((event) => (
                  <tr key={event.id}>
                    <td>
                      <Link to={`/events/${event.id}`} className="table-link">
                        {event.title}
                      </Link>
                      <span className="table-sub">{event.createdByName}</span>
                    </td>
                    <td>
                      {formatDateTime(event.startDate)}
                      <span className="table-sub">{formatDateTime(event.endDate)}</span>
                    </td>
                    <td>{event.localityName}</td>
                    <td>
                      <StatusBadge status={event.status} />
                    </td>
                    <td>
                      {event.maxParticipants}
                      <span className="table-sub">máx. {event.maxParticipantsPerGroup}/grupo</span>
                    </td>
                    <td>
                      <div className="row-actions">
                        {!isLocked(event) && (
                          <Link to={`/admin/events/${event.id}/edit`} className="btn btn-ghost btn-sm">
                            Editar
                          </Link>
                        )}
                        {event.status === "Draft" && (
                          <Button variant="success" className="btn-sm" onClick={() => setTarget({ event, action: "publish" })}>
                            Publicar
                          </Button>
                        )}
                        {event.status === "Published" && !isLocked(event) && (
                          <Button variant="secondary" className="btn-sm" onClick={() => setTarget({ event, action: "cancel" })}>
                            Cancelar
                          </Button>
                        )}
                        {isAdmin && (
                          <Button variant="danger" className="btn-sm" onClick={() => setTarget({ event, action: "delete" })}>
                            Eliminar
                          </Button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <Pagination page={page} totalPages={totalPages} totalItems={totalItems} onChange={setPage} />
        </>
      )}

      <ConfirmModal
        open={target !== null}
        title={
          target?.action === "publish"
            ? "Publicar evento"
            : target?.action === "cancel"
              ? "Cancelar evento"
              : "Eliminar evento"
        }
        message={
          target?.action === "publish"
            ? `¿Publicar "${target?.event.title}"? Será visible para todos los usuarios.`
            : target?.action === "cancel"
              ? `¿Cancelar el evento "${target?.event.title}"?`
              : `¿Eliminar el evento "${target?.event.title}"? Esta acción es definitiva.`
        }
        confirmLabel={target?.action === "delete" ? "Eliminar" : "Confirmar"}
        tone={target?.action === "delete" ? "danger" : "primary"}
        loading={busy}
        onConfirm={runAction}
        onClose={() => setTarget(null)}
      />
    </div>
  );
}
