import { useEffect, useState } from "react";
import { eventTypesApi } from "../../api/eventTypes";
import { extractErrorMessage } from "../../api/client";
import {
  Alert,
  Button,
  Card,
  ConfirmModal,
  EmptyState,
  Input,
  PageHeader,
  PageLoader,
  Textarea,
} from "../../components/ui";
import type { EventTypeDto } from "../../types";

type Mode = "list" | "create" | "edit";

const EMPTY: Partial<EventTypeDto> = { name: "", description: "", isActive: true };

export default function AdminEventTypes() {
  const [types, setTypes] = useState<EventTypeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [mode, setMode] = useState<Mode>("list");
  const [form, setForm] = useState<Partial<EventTypeDto>>(EMPTY);
  const [editId, setEditId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<EventTypeDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await eventTypesApi.getAll();
      setTypes(data);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const set = (field: keyof EventTypeDto) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
      setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const openCreate = () => {
    setForm(EMPTY);
    setEditId(null);
    setError("");
    setSuccess("");
    setMode("create");
  };

  const openEdit = (t: EventTypeDto) => {
    setForm({ ...t });
    setEditId(t.id);
    setError("");
    setSuccess("");
    setMode("edit");
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.name?.trim()) {
      setError("El nombre del tipo de evento es obligatorio.");
      return;
    }
    setSaving(true);
    setError("");
    setSuccess("");
    try {
      if (mode === "create") {
        await eventTypesApi.create({ name: form.name!, description: form.description });
        setSuccess("Tipo de evento creado correctamente.");
      } else if (mode === "edit" && editId !== null) {
        await eventTypesApi.update({ id: editId, ...form } as EventTypeDto);
        setSuccess("Tipo de evento actualizado correctamente.");
      }
      await load();
      setMode("list");
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    setError("");
    try {
      await eventTypesApi.delete(deleteTarget.id);
      setSuccess(`Tipo "${deleteTarget.name}" eliminado.`);
      setDeleteTarget(null);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setDeleting(false);
    }
  };

  const goBack = () => {
    setMode("list");
    setError("");
    setSuccess("");
  };

  /* ── Form ── */
  if (mode === "create" || mode === "edit") {
    return (
      <div className="container page">
        <PageHeader
          title={mode === "create" ? "Nuevo tipo de evento" : "Editar tipo de evento"}
        />
        {error && <Alert tone="error">{error}</Alert>}
        <Card>
          <form onSubmit={handleSave} className="form">
            <Input
              label="Nombre *"
              value={form.name ?? ""}
              onChange={set("name")}
              placeholder="Ej. Danza folclórica"
              required
            />
            <Textarea
              label="Descripción"
              value={form.description ?? ""}
              onChange={set("description")}
              rows={3}
              placeholder="Breve descripción del tipo de evento…"
            />
            <div className="form-actions">
              <Button type="button" variant="ghost" onClick={goBack} disabled={saving}>
                Cancelar
              </Button>
              <Button type="submit" loading={saving}>
                {mode === "create" ? "Crear" : "Guardar cambios"}
              </Button>
            </div>
          </form>
        </Card>
      </div>
    );
  }

  /* ── List ── */
  if (loading) return <PageLoader />;

  return (
    <div className="container page">
      <PageHeader
        title="Tipos de evento"
        subtitle="Categorías que clasifican los eventos de la plataforma"
        actions={
          <Button onClick={openCreate}>+ Nuevo tipo</Button>
        }
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      {types.length === 0 ? (
        <EmptyState
          title="No hay tipos de evento"
          message="Crea el primer tipo de evento para organizar el catálogo."
        />
      ) : (
        <div className="table-responsive">
          <table className="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nombre</th>
                <th>Descripción</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {types.map((t) => (
                <tr key={t.id}>
                  <td>{t.id}</td>
                  <td>
                    <strong>{t.name}</strong>
                  </td>
                  <td>{t.description ?? "—"}</td>
                  <td>
                    <span
                      className={`status-badge status-badge--${t.isActive ? "published" : "cancelled"}`}
                    >
                      {t.isActive ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td>
                    <div style={{ display: "flex", gap: "0.5rem" }}>
                      <Button variant="secondary" onClick={() => openEdit(t)}>
                        Editar
                      </Button>
                      <Button variant="danger" onClick={() => setDeleteTarget(t)}>
                        Eliminar
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmModal
        open={!!deleteTarget}
        title="Eliminar tipo de evento"
        message={`¿Seguro que quieres eliminar "${deleteTarget?.name}"? Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        tone="danger"
        loading={deleting}
        onConfirm={handleDelete}
        onClose={() => setDeleteTarget(null)}
      />
    </div>
  );
}
