import { Link, NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { getInitials } from "../utils/format";

const publicLinks: { to: string; label: string }[] = [];

const adminLinks = [
  { to: "/dashboard", label: "Panel" },
  { to: "/admin/events", label: "Eventos" },
  { to: "/admin/event-types", label: "Tipos de evento" },
  { to: "/admin/groups", label: "Grupos" },
  { to: "/admin/inscriptions", label: "Inscripciones" },
  { to: "/admin/users", label: "Usuarios" },
];

const gestorLinks = [
  { to: "/dashboard", label: "Panel" },
  { to: "/admin/events", label: "Eventos" },
  { to: "/admin/inscriptions", label: "Inscripciones" },
];

const groupLinks = [
  { to: "/dashboard", label: "Panel" },
  { to: "/my-groups", label: "Mis grupos" },
  { to: "/participants", label: "Participantes" },
  { to: "/my-inscriptions", label: "Mis inscripciones" },
];

export function Layout() {
  const { user, isAuthenticated, isReady, logout } = useAuth();
  const navigate = useNavigate();

  // Mientras verificamos el token contra el backend, usamos publicLinks
  // para no mostrar links de rol incorrecto en el flash inicial.
  const links = !isReady
    ? publicLinks
    : user?.roles.includes("Admin")
      ? adminLinks
      : user?.roles.includes("Gestor")
        ? gestorLinks
        : user?.roles.includes("Grupo")
          ? groupLinks
          : publicLinks;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="app-shell">
      <header className="navbar">
        <div className="navbar-inner">
          <Link to="/" className="brand">
            <svg
              className="brand-logo"
              width="32"
              height="38"
              viewBox="0 0 200 240"
              xmlns="http://www.w3.org/2000/svg"
              aria-hidden="true"
            >
              <defs>
                <radialGradient id="bl-skirt" cx="50%" cy="30%" r="70%">
                  <stop offset="0%" stopColor="#1a8a72" />
                  <stop offset="60%" stopColor="#0E6655" />
                  <stop offset="100%" stopColor="#083d30" />
                </radialGradient>
                <radialGradient id="bl-petal" cx="40%" cy="40%" r="60%">
                  <stop offset="0%" stopColor="#f0a0d0" />
                  <stop offset="50%" stopColor="#d060a8" />
                  <stop offset="100%" stopColor="#8a2060" />
                </radialGradient>
                <radialGradient id="bl-labelo" cx="50%" cy="30%" r="70%">
                  <stop offset="0%" stopColor="#ff60d0" />
                  <stop offset="40%" stopColor="#cc2090" />
                  <stop offset="100%" stopColor="#880060" />
                </radialGradient>
              </defs>
              {/* Falda */}
              <path d="M 60 148 C 30 152, 10 165, 18 178 C 26 191, 55 198, 85 200 C 115 202, 145 196, 160 186 C 175 176, 168 163, 148 155 C 130 148, 100 144, 60 148 Z" fill="#083d30" opacity="0.7" />
              <path d="M 65 142 C 38 145, 22 156, 28 170 C 34 184, 62 192, 92 193 C 122 194, 150 188, 162 176 C 174 164, 165 151, 142 145 C 122 140, 95 138, 65 142 Z" fill="url(#bl-skirt)" />
              <path d="M 72 140 C 55 142, 44 152, 48 162 C 52 172, 72 178, 96 178 C 120 178, 142 172, 148 161 C 154 150, 143 142, 124 139 C 108 137, 90 138, 72 140 Z" fill="#1a8a72" opacity="0.85" />
              <path d="M 28 170 C 34 184, 62 194, 92 195 C 122 196, 152 188, 163 177" stroke="#B7950B" strokeWidth="1.8" fill="none" opacity="0.7" />
              {/* Cinturilla */}
              <path d="M 76 134 C 80 131, 90 129, 100 129 C 110 129, 120 131, 124 134 C 118 137, 108 138, 100 138 C 92 138, 82 137, 76 134 Z" fill="#B7950B" />
              {/* Torso */}
              <path d="M 88 100 C 85 105, 84 120, 86 130 C 90 133, 110 133, 114 130 C 116 120, 115 105, 112 100 C 108 96, 92 96, 88 100 Z" fill="#B7950B" />
              {/* Brazos */}
              <path d="M 88 108 C 80 104, 68 98, 54 88 C 44 80, 36 70, 32 60" stroke="#d4b012" strokeWidth="5" strokeLinecap="round" fill="none" />
              <path d="M 112 108 C 120 104, 132 98, 146 88 C 156 80, 164 70, 168 60" stroke="#d4b012" strokeWidth="5" strokeLinecap="round" fill="none" />
              <ellipse cx="31" cy="59" rx="5" ry="4" fill="#d4b012" transform="rotate(-20 31 59)" />
              <ellipse cx="169" cy="59" rx="5" ry="4" fill="#d4b012" transform="rotate(20 169 59)" />
              {/* Cabeza */}
              <ellipse cx="100" cy="87" rx="13" ry="14" fill="#d4b012" />
              {/* Sombrero vueltiao */}
              <path d="M 86 80 C 87 70, 90 62, 100 59 C 110 62, 113 70, 114 80 Z" fill="#1a1005" />
              <path d="M 72 80 C 76 74, 86 72, 100 71 C 114 72, 124 74, 128 80 C 122 84, 110 86, 100 86.5 C 90 86, 78 84, 72 80 Z" fill="#2a1e08" />
              <path d="M 76 80 L 80 75" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.7" />
              <path d="M 82 79 L 86 74" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.7" />
              <path d="M 88 78 L 92 73" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.65" />
              <path d="M 94 77 L 98 73" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.65" />
              <path d="M 100 77 L 104 73" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.65" />
              <path d="M 106 77 L 110 73" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.65" />
              <path d="M 112 78 L 116 74" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.7" />
              <path d="M 118 79 L 122 75" stroke="#e8d5a0" strokeWidth="1.2" opacity="0.7" />
              <path d="M 75 81 C 79 76, 89 73.5, 100 73 C 111 73.5, 121 76, 125 81" stroke="#B7950B" strokeWidth="1.5" fill="none" />
              {/* Orquídea izquierda */}
              <path d="M 31 57 C 28 48, 24 38, 20 28" stroke="#0E6655" strokeWidth="1.8" fill="none" strokeLinecap="round" />
              <path d="M 20 28 C 18 18, 22 10, 28 8 C 32 10, 32 20, 28 26 C 26 28, 22 28, 20 28 Z" fill="url(#bl-petal)" />
              <path d="M 20 28 C 10 26, 4 20, 6 14 C 10 12, 18 18, 22 24 C 22 26, 21 28, 20 28 Z" fill="url(#bl-petal)" />
              <path d="M 28 26 C 36 22, 44 22, 46 28 C 42 32, 34 30, 28 28 C 27 27, 27 26, 28 26 Z" fill="url(#bl-petal)" />
              <path d="M 20 28 C 18 32, 16 38, 20 42 C 22 44, 26 44, 28 42 C 32 38, 30 32, 28 28 C 26 26, 22 26, 20 28 Z" fill="url(#bl-labelo)" />
              <ellipse cx="24" cy="28" rx="4" ry="3" fill="#fff" opacity="0.9" />
              <ellipse cx="24" cy="28" rx="2.5" ry="2" fill="#d4b012" />
              {/* Orquídea derecha */}
              <path d="M 169 57 C 172 48, 176 38, 180 28" stroke="#0E6655" strokeWidth="1.8" fill="none" strokeLinecap="round" />
              <path d="M 180 28 C 182 18, 178 10, 172 8 C 168 10, 168 20, 172 26 C 174 28, 178 28, 180 28 Z" fill="url(#bl-petal)" />
              <path d="M 172 26 C 164 22, 156 22, 154 28 C 158 32, 166 30, 172 28 Z" fill="url(#bl-petal)" />
              <path d="M 180 28 C 190 26, 196 20, 194 14 C 190 12, 182 18, 178 24 Z" fill="url(#bl-petal)" />
              <path d="M 172 28 C 170 32, 168 38, 172 42 C 174 44, 178 44, 180 42 C 184 38, 182 32, 180 28 C 178 26, 174 26, 172 28 Z" fill="url(#bl-labelo)" />
              <ellipse cx="176" cy="28" rx="4" ry="3" fill="#fff" opacity="0.9" />
              <ellipse cx="176" cy="28" rx="2.5" ry="2" fill="#d4b012" />
            </svg>
            <span>DanzaBogota</span>
          </Link>
          <nav className="nav-links" aria-label="Navegación principal">
            {links.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                className={({ isActive }) => (isActive ? "nav-link active" : "nav-link")}
                end={link.to === "/"}
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
          <div className="nav-user">
            {isAuthenticated && user ? (
              <>
                <div className="user-chip">
                  <span className="avatar">{getInitials(user.firstName, user.lastName)}</span>
                  <span className="user-name">
                    {user.firstName} {user.lastName}
                  </span>
                </div>
                <button type="button" className="btn btn-ghost btn-sm" onClick={handleLogout}>
                  Salir
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="btn btn-ghost btn-sm">
                  Ingresar
                </Link>
                <Link to="/register" className="btn btn-primary btn-sm">
                  Registrarse
                </Link>
              </>
            )}
          </div>
        </div>
      </header>

      <main className="main-content">
        <Outlet />
      </main>

      <footer className="footer">
        <div className="footer-inner">
          <p>DanzaBogota — Plataforma de gestión de eventos de danza cultural en Bogotá.</p>
          <p>Proyecto de tesis — Ingeniería de Sistemas, UNAD.</p>
        </div>
      </footer>
    </div>
  );
}
