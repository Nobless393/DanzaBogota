import { useEffect, useState } from "react";
import { usersApi } from "../../api/users";
import { extractErrorMessage } from "../../api/client";
import {
  Alert,
  Button,
  Card,
  EmptyState,
  Input,
  PageHeader,
  PageLoader,
  Pagination,
  Select,
} from "../../components/ui";
import { getRoleLabel } from "../../utils/format";
import type { UserDto } from "../../types";

const PAGE_SIZE = 15;
const ROLES = ["Admin", "Gestor", "Grupo"];

export default function AdminUsers() {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [filtered, setFiltered] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);

  /* Cambio de rol inline */
  const [changingRole, setChangingRole] = useState<{ userId: string; newRole: string } | null>(null);
  const [savingRole, setSavingRole] = useState(false);

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await usersApi.getAll();
      setUsers(data);
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
      users.filter(
        (u) =>
          u.firstName.toLowerCase().includes(q) ||
          u.lastName.toLowerCase().includes(q) ||
          u.email.toLowerCase().includes(q)
      )
    );
    setPage(1);
  }, [search, users]);

  const handleChangeRole = async () => {
    if (!changingRole) return;
    setSavingRole(true);
    setError("");
    try {
      await usersApi.changeRole(changingRole);
      setSuccess(`Rol actualizado correctamente.`);
      setChangingRole(null);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSavingRole(false);
    }
  };

  const totalPages = Math.ceil(filtered.length / PAGE_SIZE);
  const paged = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  if (loading) return <PageLoader />;

  return (
    <div className="container page">
      <PageHeader
        title="Gestión de usuarios"
        subtitle={`${users.length} usuarios registrados en la plataforma`}
      />

      {error && <Alert tone="error">{error}</Alert>}
      {success && <Alert tone="success">{success}</Alert>}

      <Card>
        <Input
          label="Buscar usuario"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Nombre, apellido o correo…"
        />
      </Card>

      {filtered.length === 0 ? (
        <EmptyState
          title="No se encontraron usuarios"
          message={search ? "Intenta con otro término." : "No hay usuarios registrados."}
        />
      ) : (
        <>
          <div className="table-responsive" style={{ marginTop: "1rem" }}>
            <table className="table">
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Correo</th>
                  <th>Rol actual</th>
                  <th>Cambiar rol</th>
                </tr>
              </thead>
              <tbody>
                {paged.map((u) => {
                  const isChanging = changingRole?.userId === u.id;
                  return (
                    <tr key={u.id}>
                      <td>
                        {u.firstName} {u.lastName}
                      </td>
                      <td>{u.email}</td>
                      <td>
                        <span className="role-badge">
                          {getRoleLabel(u.roles?.[0] ?? "")}
                        </span>
                      </td>
                      <td>
                        {isChanging ? (
                          <div style={{ display: "flex", gap: "0.5rem", alignItems: "center" }}>
                            <select
                              className="select select--sm"
                              value={changingRole.newRole}
                              onChange={(e) =>
                                setChangingRole({ userId: u.id, newRole: e.target.value })
                              }
                            >
                              {ROLES.map((r) => (
                                <option key={r} value={r}>
                                  {getRoleLabel(r)}
                                </option>
                              ))}
                            </select>
                            <Button
                              variant="primary"
                              loading={savingRole}
                              onClick={handleChangeRole}
                            >
                              Guardar
                            </Button>
                            <Button
                              variant="ghost"
                              onClick={() => setChangingRole(null)}
                              disabled={savingRole}
                            >
                              Cancelar
                            </Button>
                          </div>
                        ) : (
                          <Button
                            variant="secondary"
                            onClick={() =>
                              setChangingRole({ userId: u.id, newRole: u.roles?.[0] ?? "Grupo" })
                            }
                          >
                            Cambiar rol
                          </Button>
                        )}
                      </td>
                    </tr>
                  );
                })}
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
    </div>
  );
}
