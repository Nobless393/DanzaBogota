import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { inscriptionsApi } from "../../api/inscriptions";
import { eventsApi } from "../../api/events";
import { extractErrorMessage } from "../../api/client";
import {
  Alert,
  Button,
  Card,
  ConfirmModal,
  EmptyState,
  PageHeader,
  PageLoader,
  Pagination,
  Select,
  StatusBadge,
  Textarea,
} from "../../components/ui";
import { formatDate } from "../../utils/format";
import type { InscriptionDto, EventDto } from "../../types";

const PAGE_SIZE = 15;

export default function AdminInscriptions() {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [selectedEventId, setSelectedEventId] = useState<number | null>(null);
  const [inscriptions, setInscriptions] = useState<InscriptionDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [loadingEvents, setLoadingEvents] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [page, setPage] = useState(1);

  /* Modal de revisión */
  const [reviewTarget, setReviewTarget] = useState<InscriptionDto | null>(null);
  const [reviewApprove, setReviewApprove] = useState(true);
  const [reviewNotes, setReviewNotes] = useState("");
  const [reviewing, setReviewing] = useState(false);

  /* Stats rápidas */
  const [stats, setStats] = useState({ totalInscriptions: 0, approved: 0, pending: 0, rejected: 0 });

  useEffect(() => {
    const init = async () => {
      setLoadingEvents(true);
      setError("");
      try {
        const [evtsPage, s] = await Promise.all([
          eventsApi.getAll({ page: 1, pageSize: 100 }),
          inscriptionsApi.getStats(),
        ]);
        setEvents(evtsPage.items);
        setStats({
          totalInscriptions: s.totalInscriptions ?? 0,
          approved: s.approved ?? 0,
          pending: s.pending ?? 0,
          rejected: s.rejected ?? 0,
        });
      } catch (err) {
        setError(extractErrorMessage(err));
      } finally {
        setLoadingEvents(false);
      }
    };
    init();
  }, []);

  const loadInscriptions = async (eventId: number) => {
    setLoading(true);
    setError("");
    setInscriptions([]);
    setPage(1);
    try {
      const data = await inscriptionsApi.getByEvent(eventId);
      setInscriptions(data);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleEventChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const id = e.target.value ? Number(e.target.value) : null;
    setSelectedEventId(id);
    if (id) loadInscriptions(id);
    else setInscriptions([]);
  };

  const openReview = (insc: InscriptionDto, approve: boolean) => {
    setReviewTarget(insc);
    setReviewApprove(approve);
    setReviewNotes("");
  };

  const handleReview = async () => {
    if (!reviewTarget) return;
    setReviewing(true);
    setError("");
    try {
      await inscriptionsApi.review({
        inscriptionId: reviewTarget.id,
        approve: reviewApprove,
        notes: reviewNotes || undefined,
      });
      setSuccess(
        reviewApprove
          ? `Inscripción de "${reviewTarget.groupName}" aprobada.`
          : `Inscripción de "${reviewTarget.groupName}" rechazada.`
      );
      setReviewTarget(null);
      if (selectedEventId) loadInscriptions(selectedEventId);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setReviewing(false);
    }
  };

  /* Paginar */
  const totalPages = Math.ceil(inscriptions.length / PAGE_SIZE);
  const paged = inscriptions.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  if (loadingEvents) return <PageLoader />;

  return (
    <div className="container page">
      <PageHeader
        title="Gestión de inscripciones"
        subtitle="Revisa y aprueba las inscripciones de los grupos"
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      {/* Stats */}
      <div className="stats-grid" style={{ marginBottom: "1.5rem" }}>
        {[
          { label: "Total", value: stats.totalInscriptions, icon: "📋" },
          { label: "Pendientes", value: stats.pending, icon: "⏳" },
          { label: "Aprobadas", value: stats.approved, icon: "✅" },
          { label: "Rechazadas", value: stats.rejected, icon: "❌" },
        ].map((s) => (
          <Card key={s.label} className="stat-card">
            <div className="stat-icon">{s.icon}</div>
            <div>
              <p className="stat-value">{s.value}</p>
              <p className="stat-label">{s.label}</p>
            </div>
          </Card>
        ))}
      </div>

      {/* Filtro por evento */}
      <Card>
        <Select
          label="Seleccionar evento"
          value={selectedEventId ?? ""}
          onChange={handleEventChange}
        >
          <option value="">— Selecciona un evento para ver sus inscripciones —</option>
          {events.map((e) => (
            <option key={e.id} value={e.id}>
              {e.title ?? e.name} — {e.localityName ?? ""}
            </option>
          ))}
        </Select>
      </Card>

      {/* Tabla */}
      {selectedEventId !== null && (
        loading ? (
          <PageLoader />
        ) : inscriptions.length === 0 ? (
          <EmptyState
            title="Sin inscripciones"
            message="Este evento aún no tiene inscripciones."
          />
        ) : (
          <>
            <div className="table-responsive" style={{ marginTop: "1rem" }}>
              <table className="table">
                <thead>
                  <tr>
                    <th>Grupo</th>
                    <th>Participantes</th>
                    <th>Estado</th>
                    <th>Fecha</th>
                    <th>Acciones</th>
                  </tr>
                </thead>
                <tbody>
                  {paged.map((insc) => (
                    <tr key={insc.id}>
                      <td>{insc.groupName ?? `Grupo #${insc.groupId}`}</td>
                      <td>{insc.participantCount ?? "—"}</td>
                      <td>
                        <StatusBadge status={insc.status} />
                      </td>
                      <td>
                        {insc.registeredAt ? formatDate(insc.registeredAt) : "—"}
                      </td>
                      <td>
                        <div style={{ display: "flex", gap: "0.5rem" }}>
                          {insc.status === "Pending" && (
                            <>
                              <Button
                                variant="success"
                                onClick={() => openReview(insc, true)}
                              >
                                Aprobar
                              </Button>
                              <Button
                                variant="danger"
                                onClick={() => openReview(insc, false)}
                              >
                                Rechazar
                              </Button>
                            </>
                          )}
                          <Link
                            to={`/participants?inscriptionId=${insc.id}`}
                            className="btn btn-ghost btn-sm"
                          >
                            Ver participantes
                          </Link>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <Pagination
              page={page}
              totalPages={totalPages}
              totalItems={inscriptions.length}
              onChange={setPage}
            />
          </>
        )
      )}

      {/* Modal revisión */}
      {reviewTarget && (
        <div className="modal-overlay" role="dialog" aria-modal="true">
          <div className="modal">
            <h3 className="modal-title">
              {reviewApprove ? "✅ Aprobar inscripción" : "❌ Rechazar inscripción"}
            </h3>
            <p className="modal-message">
              Grupo: <strong>{reviewTarget.groupName}</strong>
            </p>
            {error && <Alert tone="error">{error}</Alert>}
            <Textarea
              label="Notas (opcional)"
              value={reviewNotes}
              onChange={(e) => setReviewNotes(e.target.value)}
              rows={3}
              placeholder="Motivo del rechazo u observaciones…"
            />
            <div className="modal-actions">
              <Button
                variant="ghost"
                onClick={() => setReviewTarget(null)}
                disabled={reviewing}
              >
                Cancelar
              </Button>
              <Button
                variant={reviewApprove ? "primary" : "danger"}
                loading={reviewing}
                onClick={handleReview}
              >
                {reviewApprove ? "Confirmar aprobación" : "Confirmar rechazo"}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
