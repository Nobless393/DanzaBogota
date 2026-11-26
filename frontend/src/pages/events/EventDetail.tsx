import { useEffect, useState, type FormEvent } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import { eventsApi } from "../../api/events";
import { commentsApi, type EventCommentDto } from "../../api/comments";
import { extractErrorMessage } from "../../api/client";
import type { EventDto } from "../../types";
import { Alert, Button, Card, PageLoader, StatusBadge } from "../../components/ui";
import { formatDate, formatDateTime } from "../../utils/format";
import { useAuth } from "../../auth/AuthContext";

export default function EventDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();

  const [event, setEvent] = useState<EventDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // comentarios
  const [comments, setComments] = useState<EventCommentDto[]>([]);
  const [commentText, setCommentText] = useState("");
  const [commentError, setCommentError] = useState("");
  const [commentLoading, setCommentLoading] = useState(false);
  const [commentsLoading, setCommentsLoading] = useState(true);

  const isAdminOrGestor = user?.roles.some((r) => r === "Admin" || r === "Gestor");

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    eventsApi
      .getById(Number(id))
      .then(setEvent)
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [id]);

  useEffect(() => {
    if (!id) return;
    setCommentsLoading(true);
    commentsApi
      .getByEvent(Number(id))
      .then(setComments)
      .catch(() => setComments([]))
      .finally(() => setCommentsLoading(false));
  }, [id]);

  const handleComment = async (e: FormEvent) => {
    e.preventDefault();
    if (!commentText.trim()) return;
    setCommentError("");
    setCommentLoading(true);
    try {
      const created = await commentsApi.create(Number(id), { content: commentText.trim() });
      if (created) setComments((prev) => [created, ...prev]);
      setCommentText("");
    } catch (err) {
      setCommentError(extractErrorMessage(err));
    } finally {
      setCommentLoading(false);
    }
  };

  const handleDeleteComment = async (commentId: number) => {
    try {
      await commentsApi.delete(Number(id), commentId);
      setComments((prev) => prev.filter((c) => c.id !== commentId));
    } catch {
      // silenciar
    }
  };

  const handleToggleLike = async (commentId: number) => {
    try {
      const result = await commentsApi.toggleLike(Number(id), commentId);
      setComments((prev) =>
        prev.map((c) =>
          c.id === commentId
            ? { ...c, likesCount: result.likesCount, liked: result.liked }
            : c
        )
      );
    } catch {
      // silenciar
    }
  };

  if (loading) return <PageLoader />;

  if (error || !event) {
    return (
      <div className="container page">
        <Alert tone="error">{error || "Evento no encontrado."}</Alert>
        <Button variant="ghost" onClick={() => navigate(-1)}>
          ← Volver
        </Button>
      </div>
    );
  }

  const isUpcoming = new Date(event.startDate) > new Date();
  const isActive = event.status === "Published";
  const canInscribe = isActive && isUpcoming;

  return (
    <div className="container page page-narrow">
      <Link to="/events" className="btn btn-ghost btn-sm">
        ← Volver a eventos
      </Link>

      {/* Imagen de cabecera */}
      {event.imageUrl && (
        <img
          src={event.imageUrl}
          alt={event.title}
          className="event-detail-image"
        />
      )}

      <div className="event-detail-header">
        <div className="event-detail-meta">
          {event.localityName && (
            <span className="badge badge-locality">{event.localityName}</span>
          )}
          <StatusBadge status={event.status} />
        </div>
        <h1 className="event-detail-title">{event.title}</h1>
        {event.createdByName && (
          <p className="event-detail-organizer">Organizado por {event.createdByName}</p>
        )}
      </div>

      <div className="event-detail-grid">
        {/* Descripción + comentarios */}
        <div>
          <Card className="event-detail-desc">
            <h2>Acerca del evento</h2>
            <p>{event.description || "Sin descripción disponible."}</p>
          </Card>

          {/* Sección de comentarios */}
          <Card title="Comentarios" className="comments-section">
            {/* Formulario nuevo comentario */}
            {isAuthenticated ? (
              <form onSubmit={handleComment} className="comment-form">
                <textarea
                  className="input"
                  rows={3}
                  placeholder="Escribe un comentario..."
                  value={commentText}
                  onChange={(e) => setCommentText(e.target.value)}
                  required
                />
                {commentError && <Alert tone="error">{commentError}</Alert>}
                <Button type="submit" loading={commentLoading} variant="primary">
                  Comentar
                </Button>
              </form>
            ) : (
              <p className="comment-login-hint">
                <Link to="/login">Inicia sesión</Link> para dejar un comentario.
              </p>
            )}

            {/* Lista de comentarios */}
            {commentsLoading ? (
              <p className="comments-loading">Cargando comentarios...</p>
            ) : comments.length === 0 ? (
              <p className="comments-empty">Sé el primero en comentar.</p>
            ) : (
              <ul className="comment-list">
                {comments.map((c) => (
                  <li key={c.id} className="comment-item">
                    <div className="comment-header">
                      <strong>{c.authorName ?? "Usuario"}</strong>
                      {c.createdAt && (
                        <span className="comment-date">{formatDate(c.createdAt)}</span>
                      )}
                    </div>
                    <p className="comment-body">{c.content}</p>
                    <div className="comment-actions">
                      {isAuthenticated && (
                        <button
                          type="button"
                          className={`btn-like ${c.liked ? "liked" : ""}`}
                          onClick={() => handleToggleLike(c.id)}
                        >
                          👍 {c.likesCount ?? 0}
                        </button>
                      )}
                      {(c.isOwn || isAdminOrGestor) && (
                        <button
                          type="button"
                          className="btn btn-ghost btn-sm btn-danger-ghost"
                          onClick={() => handleDeleteComment(c.id)}
                        >
                          Eliminar
                        </button>
                      )}
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </Card>
        </div>

        {/* Info lateral */}
        <aside className="event-detail-aside">
          <Card title="Detalles del evento">
            <dl className="detail-list">
              <dt>Fecha inicio</dt>
              <dd>{formatDateTime(event.startDate)}</dd>

              <dt>Fecha fin</dt>
              <dd>{formatDate(event.endDate)}</dd>

              {event.locationName && (
                <>
                  <dt>Lugar</dt>
                  <dd>{event.locationName}</dd>
                </>
              )}

              {event.address && (
                <>
                  <dt>Dirección</dt>
                  <dd>{event.address}</dd>
                </>
              )}

              {event.maxParticipants != null && (
                <>
                  <dt>Cupo máximo</dt>
                  <dd>{event.maxParticipants} participantes</dd>
                </>
              )}

              {event.maxParticipantsPerGroup != null && (
                <>
                  <dt>Máximo por grupo</dt>
                  <dd>{event.maxParticipantsPerGroup}</dd>
                </>
              )}
            </dl>

            <div className="detail-actions">
              {canInscribe && isAuthenticated ? (
                <Link
                  to={`/my-groups`}
                  className="btn btn-primary"
                  style={{ display: "block", textAlign: "center" }}
                >
                  Inscribir mi grupo
                </Link>
              ) : canInscribe && !isAuthenticated ? (
                <Link
                  to="/login"
                  className="btn btn-primary"
                  style={{ display: "block", textAlign: "center" }}
                >
                  Inicia sesión para inscribirte
                </Link>
              ) : !isActive ? (
                <p className="detail-note">Este evento no está disponible para inscripciones.</p>
              ) : (
                <p className="detail-note">Las inscripciones han cerrado.</p>
              )}

              {isAdminOrGestor && (
                <Link
                  to={`/admin/events/${event.id}/edit`}
                  className="btn btn-ghost btn-sm"
                  style={{ display: "block", textAlign: "center", marginTop: "0.5rem" }}
                >
                  Editar evento
                </Link>
              )}
            </div>
          </Card>
        </aside>
      </div>
    </div>
  );
}
