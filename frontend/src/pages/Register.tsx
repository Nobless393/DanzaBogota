import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { Alert, Button, Card, Input, Select } from "../components/ui";
import { extractErrorMessage } from "../api/client";
import { DOCUMENT_TYPE_LABELS } from "../utils/format";

type AccountType = "Grupo" | "Gestor" | null;

export default function Register() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [accountType, setAccountType] = useState<AccountType>(null);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
    documentType: "CC",
    documentNumber: "",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const set = (field: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    if (form.password !== form.confirmPassword) {
      setError("Las contraseñas no coinciden.");
      return;
    }
    if (form.password.length < 6) {
      setError("La contraseña debe tener al menos 6 caracteres.");
      return;
    }
    setLoading(true);
    try {
      await register({
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        password: form.password,
        documentType: form.documentType,
        documentNumber: form.documentNumber || null,
        role: accountType ?? "Grupo",
      });
      navigate("/dashboard", { replace: true });
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  /* ── Paso 1: elegir tipo de cuenta ── */
  if (!accountType) {
    return (
      <div className="auth-page">
        <Card className="auth-card auth-card-selector">
          <div className="auth-heading">
            <h2 style={{ color: "var(--color-primary)" }}>Crear cuenta</h2>
            <p>Selecciona cómo quieres registrarte en DanzaBogota.</p>
          </div>
          <div className="account-type-grid">
            <button
              type="button"
              className="account-type-card"
              onClick={() => setAccountType("Grupo")}
            >
              <span className="account-type-icon" aria-hidden="true">🎭</span>
              <strong>Grupo de danza</strong>
              <span>Inscribe tu agrupación a eventos culturales en Bogotá.</span>
            </button>
            <button
              type="button"
              className="account-type-card"
              onClick={() => setAccountType("Gestor")}
            >
              <span className="account-type-icon" aria-hidden="true">🏛️</span>
              <strong>Gestor Cultural</strong>
              <span>Organiza y publica eventos de danza en la ciudad.</span>
            </button>
          </div>
          <p className="auth-alt">
            ¿Ya tienes cuenta? <Link to="/login">Inicia sesión</Link>
          </p>
        </Card>
      </div>
    );
  }

  /* ── Paso 2: formulario ── */
  const isGestor = accountType === "Gestor";

  return (
    <div className="auth-page">
      <Card className="auth-card">
        <div className="auth-heading">
          <button
            type="button"
            className="btn btn-ghost btn-sm"
            style={{ marginBottom: "0.5rem", alignSelf: "flex-start" }}
            onClick={() => { setAccountType(null); setError(""); }}
          >
            ← Volver
          </button>
          <h2>{isGestor ? "Registro de Gestor Cultural" : "Registro de Grupo de Danza"}</h2>
          <p>
            {isGestor
              ? "Tu cuenta tendrá el rol de Gestor Cultural y podrás crear y administrar eventos."
              : "Tu cuenta tendrá el rol de Grupo de danza y podrás inscribirte a eventos."}
          </p>
        </div>
        {error && <Alert tone="error">{error}</Alert>}
        <form onSubmit={handleSubmit} className="form">
          <div className="form-row">
            <Input label="Nombres" name="firstName" placeholder="Nombre" value={form.firstName} onChange={set("firstName")} required />
            <Input label="Apellidos" name="lastName" placeholder="Apellido" value={form.lastName} onChange={set("lastName")} required />
          </div>
          <Input
            label="Correo electrónico"
            name="email"
            type="email"
            autoComplete="email"
            placeholder="tucorreo@ejemplo.com"
            value={form.email}
            onChange={set("email")}
            required
          />
          <div className="form-row">
            <Select label="Tipo de documento" name="documentType" value={form.documentType} onChange={set("documentType")}>
              {Object.entries(DOCUMENT_TYPE_LABELS).map(([value, label]) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </Select>
            <Input
              label="Número de documento"
              name="documentNumber"
              placeholder="Documento"
              value={form.documentNumber}
              onChange={set("documentNumber")}
            />
          </div>
          <div className="form-row">
            <Input
              label="Contraseña"
              name="password"
              type="password"
              autoComplete="new-password"
              placeholder="Mínimo 6 caracteres"
              value={form.password}
              onChange={set("password")}
              required
            />
            <Input
              label="Confirmar contraseña"
              name="confirmPassword"
              type="password"
              autoComplete="new-password"
              placeholder="Repite la contraseña"
              value={form.confirmPassword}
              onChange={set("confirmPassword")}
              required
            />
          </div>
          <Button type="submit" loading={loading} className="btn-block">
            Crear cuenta
          </Button>
        </form>
        <p className="auth-alt">
          ¿Ya tienes cuenta? <Link to="/login">Inicia sesión</Link>
        </p>
      </Card>
    </div>
  );
}
