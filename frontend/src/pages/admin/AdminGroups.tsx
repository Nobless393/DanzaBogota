import { useEffect, useState } from "react";
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
  Pagination,
} from "../../components/ui";
import type { GroupDto } from "../../types";

const PAGE_SIZE = 15;

export default function AdminGroups() {
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [filtered, setFiltered] = useState<GroupDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [deleteTarget, setDeleteTarget] = useState<GroupDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await groupsApi.getAll();
      setGroups(data);
      setFiltered(data);
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  /* Filtro local */
  useEffect(() => {
    const q = search.toLowerCase();
    setFiltered(
      groups.filter(
        (g) =>
          g.name.toLowerCase().includes(q) ||
          (g.directorName?.toLowerCase().includes(q) ?? false) ||
          (g.localityName?.toLowerCase().includes(q) ?? false)
      )
    );
    setPage(1);
  }, [search, groups]);

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

  const totalPages = Math.ceil(filtered.length / PAGE_SIZE);
  const paged = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  if (loading) return <PageLoader />;

  return (
    <div className="container page">
      <PageHeader
        title="Gestión de grupos"
        subtitle={`${groups.length} grupos registrados en la plataforma`}
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      {/* Búsqueda */}
      <Card>
        <Input
          label="Buscar grupo"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Nombre, director o localidad…"
        />
      </Card>

      {filtered.length === 0 ? (
        <EmptyState
          title="No se encontraron grupos"
          message={search ? "Intenta con otro término de búsqueda." : "No hay grupos registrados."}
        />
      ) : (
        <>
          <div className="table-responsive" style={{ marginTop: "1rem" }}>
            <table className="table">
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Director</th>
                  <th>Localidad</th>
                  <th>Categoría</th>
                  <th>Contacto</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {paged.map((g) => (
                  <tr key={g.id}>
                    <td>
                      <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
                        {g.photoUrl && (
                          <img
                            src={g.photoUrl}
                            alt={g.name}
                            style={{ width: 32, height: 32, borderRadius: "50%", objectFit: "cover" }}
                            onError={(e) => { (e.target as HTMLImageElement).style.display = "none"; }}
                          />
                        )}
                        <strong>{g.name}</strong>
                      </div>
                    </td>
                    <td>{g.directorName ?? "—"}</td>
                    <td>{g.localityName ?? "—"}</td>
                    <td>{g.categoryName ?? "—"}</td>
                    <td>
                      {g.email && <div>{g.email}</div>}
                      {g.phone && <div>{g.phone}</div>}
                    </td>
                    <td>
                      <Button
                        variant="danger"
                        onClick={() => setDeleteTarget(g)}
                      >
                        Eliminar
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Pagination
            page={page}
            totalPages={totalPages}
            totalItems={filtered.length}
            onChange={setPage}
          />
        </>
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
