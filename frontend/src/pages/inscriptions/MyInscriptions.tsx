import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { inscriptionsApi } from "../../api/inscriptions";
import { extractErrorMessage } from "../../api/client";
import {
  Alert,
  Button,
  Card,
  ConfirmModal,
  EmptyState,
  PageHeader,
  PageLoader,
  StatusBadge,
} from "../../components/ui";
import { formatDate } from "../../utils/format";
import type { InscriptionDto } from "../../types";

export default function MyInscriptions() {
  const [inscriptions, setInscriptions] = useState<InscriptionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [cancelTarget, setCancelTarget] = useState<InscriptionDto | null>(null);
  const [cancelling, setCancelling] = useState(false);

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await inscriptionsApi.getMyInscriptions();
      setInscriptions(data);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleCancel = async () => {
    if (!cancelTarget) return;
    setCancelling(true);
    setError("");
    try {
      await inscriptionsApi.cancel(cancelTarget.id);
      setSuccess("Inscripción cancelada correctamente.");
      setCancelTarget(null);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setCancelling(false);
    }
  };

  const canCancel = (status?: string) =>
    status === "Pending" || status === "Approved";

  if (loading) return <PageLoader />;

  const pending = inscriptions.filter((i) => i.status === "Pending");
  const approved = inscriptions.filter((i) => i.status === "Approved");
  const others = inscriptions.filter(
    (i) => i.status !== "Pending" && i.status !== "Approved"
  );

  return (
    <div className="container page">
      <PageHeader
        title="Mis inscripciones"
        subtitle="Consulta y gestiona las inscripciones de tus grupos a eventos"
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      {inscriptions.length === 0 ? (
        <EmptyState
          title="No tienes inscripciones aún"
          message="Explora los eventos disponibles e inscribe tu grupo."
        >
          <Link to="/events" className="btn btn-primary" style={{ marginTop: "1rem" }}>
            Ver eventos
          </Link>
        </EmptyState>
      ) : (
        <>
          {pending.length > 0 && (
            <section className="dashboard-section">
              <h2>⏳ Pendientes de revisión ({pending.length})</h2>
              <InscriptionTable
                items={pending}
                canCancel={canCancel}
                onCancel={setCancelTarget}
              />
            </section>
          )}

          {approved.length > 0 && (
            <section className="dashboard-section">
              <h2>✅ Aprobadas ({approved.length})</h2>
              <InscriptionTable
                items={approved}
                canCancel={canCancel}
                onCancel={setCancelTarget}
              />
            </section>
          )}

          {others.length > 0 && (
            <section className="dashboard-section">
              <h2>Historial</h2>
              <InscriptionTable
                items={others}
                canCancel={() => false}
                onCancel={() => {}}
              />
            </section>
          )}
        </>
      )}

      <ConfirmModal
        open={!!cancelTarget}
        title="Cancelar inscripción"
        message={`¿Seguro que quieres cancelar la inscripción al evento "${cancelTarget?.eventTitle ?? cancelTarget?.eventName}"?`}
        confirmLabel="Sí, cancelar"
        tone="danger"
        loading={cancelling}
        onConfirm={handleCancel}
        onClose={() => setCancelTarget(null)}
      />
    </div>
  );
}

function InscriptionTable({
  items,
  canCancel,
  onCancel,
}: {
  items: InscriptionDto[];
  canCancel: (status?: string) => boolean;
  onCancel: (i: InscriptionDto) => void;
}) {
  return (
    <div className="table-responsive">
      <table className="table">
        <thead>
          <tr>
            <th>Evento</th>
            <th>Grupo</th>
            <th>Participantes</th>
            <th>Estado</th>
            <th>Fecha inscripción</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {items.map((i) => (
            <tr key={i.id}>
              <td>
                <Link to={`/events/${i.eventId}`} className="link">
                  {i.eventTitle ?? i.eventName ?? `Evento #${i.eventId}`}
                </Link>
              </td>
              <td>{i.groupName ?? `Grupo #${i.groupId}`}</td>
              <td>{i.participantCount ?? "—"}</td>
              <td>
                <StatusBadge status={i.status} />
              </td>
              <td>{i.registeredAt ? formatDate(i.registeredAt) : "—"}</td>
              <td>
                <div style={{ display: "flex", gap: "0.5rem" }}>
                  <Link
                    to={`/participants?inscriptionId=${i.id}`}
                    className="btn btn-ghost btn-sm"
                  >
                    Participantes
                  </Link>
                  {canCancel(i.status) && (
                    <Button
                      variant="danger"
                      onClick={() => onCancel(i)}
                    >
                      Cancelar
                    </Button>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
