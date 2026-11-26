import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { groupsApi } from "../../api/groups";
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
import type { GroupDto } from "../../types";

type Mode = "list" | "create" | "edit";

const EMPTY_FORM: Partial<GroupDto> = {
  name: "",
  description: "",
  phone: "",
  email: "",
  directorName: "",
  photoUrl: "",
};

export default function MyGroups() {
  const navigate = useNavigate();
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [mode, setMode] = useState<Mode>("list");
  const [form, setForm] = useState<Partial<GroupDto>>(EMPTY_FORM);
  const [editId, setEditId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<GroupDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await groupsApi.getMyGroups();
      setGroups(data);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const set = (field: keyof GroupDto) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
      setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const openCreate = () => {
    setForm(EMPTY_FORM);
    setEditId(null);
    setError("");
    setSuccess("");
    setMode("create");
  };

  const openEdit = (g: GroupDto) => {
    setForm({ ...g });
    setEditId(g.id);
    setError("");
    setSuccess("");
    setMode("edit");
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.name?.trim()) {
      setError("El nombre del grupo es obligatorio.");
      return;
    }
    setSaving(true);
    setError("");
    setSuccess("");
    try {
      if (mode === "create") {
        await groupsApi.create(form);
        setSuccess("¡Grupo creado correctamente!");
      } else if (mode === "edit" && editId !== null) {
        await groupsApi.update(editId, form);
        setSuccess("¡Grupo actualizado correctamente!");
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
      await groupsApi.delete(deleteTarget.id);
      setSuccess(`Grupo "${deleteTarget.name}" eliminado.`);
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
          title={mode === "create" ? "Nuevo grupo" : "Editar grupo"}
          subtitle={mode === "edit" ? form.name : undefined}
        />
        {error && <Alert tone="error">{error}</Alert>}
        <Card>
          <form onSubmit={handleSave} className="form">
            <Input
              label="Nombre del grupo *"
              value={form.name ?? ""}
              onChange={set("name")}
              placeholder="Ej. Ballet Folclórico Bogotá"
              required
            />
            <div className="form-row">
              <Input
                label="Nombre del director"
                value={form.directorName ?? ""}
                onChange={set("directorName")}
                placeholder="Nombre completo"
              />
              <Input
                label="Teléfono de contacto"
                value={form.phone ?? ""}
                onChange={set("phone")}
                placeholder="+57 300 000 0000"
              />
            </div>
            <Input
              label="Correo del grupo"
              type="email"
              value={form.email ?? ""}
              onChange={set("email")}
              placeholder="grupo@ejemplo.com"
            />
            <Input
              label="URL foto / logo"
              value={form.photoUrl ?? ""}
              onChange={set("photoUrl")}
              placeholder="https://..."
            />
            <Textarea
              label="Descripción"
              value={form.description ?? ""}
              onChange={set("description")}
              rows={4}
              placeholder="Cuéntanos sobre tu grupo de danza…"
            />
            <div className="form-actions">
              <Button type="button" variant="ghost" onClick={goBack} disabled={saving}>
                Cancelar
              </Button>
              <Button type="submit" loading={saving}>
                {mode === "create" ? "Crear grupo" : "Guardar cambios"}
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
        title="Mis grupos"
        subtitle="Gestiona los grupos de danza asociados a tu cuenta"
        actions={
          <Button onClick={openCreate}>+ Nuevo grupo</Button>
        }
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      {groups.length === 0 ? (
        <EmptyState
          title="Aún no tienes grupos registrados"
          message="Crea tu primer grupo para poder inscribirte a eventos."
        />
      ) : (
        <div className="groups-grid">
          {groups.map((g) => (
            <Card key={g.id} className="group-card">
              {g.photoUrl && (
                <img
                  src={g.photoUrl}
                  alt={g.name}
                  className="group-card-img"
                  onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}
                />
              )}
              <div className="group-card-body">
                <h3 className="group-card-title">{g.name}</h3>
                {g.directorName && (
                  <p className="group-card-meta">🎭 Director: {g.directorName}</p>
                )}
                {g.localityName && (
                  <p className="group-card-meta">📍 {g.localityName}</p>
                )}
                {g.categoryName && (
                  <p className="group-card-meta">🏷️ {g.categoryName}</p>
                )}
                {g.description && (
                  <p className="group-card-desc">{g.description}</p>
                )}
              </div>
              <div className="group-card-actions">
                <Button variant="ghost" onClick={() => openEdit(g)}>
                  Editar
                </Button>
                <Link
                  to="/my-inscriptions"
                  className="btn btn-secondary btn-sm"
                >
                  Inscripciones
                </Link>
                <Button
                  variant="danger"
                  onClick={() => setDeleteTarget(g)}
                >
                  Eliminar
                </Button>
              </div>
            </Card>
          ))}
        </div>
      )}

      <ConfirmModal
        open={!!deleteTarget}
        title="Eliminar grupo"
        message={`¿Seguro que quieres eliminar el grupo "${deleteTarget?.name}"? Esta acción no se puede deshacer.`}
        confirmLabel="Eliminar"
        tone="danger"
        loading={deleting}
        onConfirm={handleDelete}
        onClose={() => setDeleteTarget(null)}
      />
    </div>
  );
}
