import { useState, type FormEvent } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { Alert, Button, Card, Input } from "../components/ui";
import { extractErrorMessage } from "../api/client";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string })?.from ?? "/dashboard";

  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const set = (field: keyof typeof form) =>
    (e: React.ChangeEvent<HTMLInputElement>) =>
      setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await login({ email: form.email, password: form.password });
      navigate(from, { replace: true });
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <Card className="auth-card">
        <div className="auth-heading">
          <h2 style={{ color: "var(--color-primary)" }}>Iniciar sesión</h2>
          <p>Ingresa a tu cuenta de DanzaBogota.</p>
        </div>

        {error && <Alert tone="error">{error}</Alert>}

        <form onSubmit={handleSubmit} className="form">
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
          <Input
            label="Contraseña"
            name="password"
            type="password"
            autoComplete="current-password"
            placeholder="Tu contraseña"
            value={form.password}
            onChange={set("password")}
            required
          />
          <Button type="submit" loading={loading} className="btn-primary btn-block">
            Ingresar
          </Button>
        </form>

        <p className="auth-alt">
          ¿No tienes cuenta? <Link to="/register">Regístrate aquí</Link>
        </p>
      </Card>
    </div>
  );
}
