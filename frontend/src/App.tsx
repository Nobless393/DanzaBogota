import { Navigate, Route, Routes } from "react-router-dom";
import { Layout } from "./components/Layout";
import { useAuth } from "./auth/AuthContext";
import Home from "./pages/Home";
import Dashboard from "./pages/Dashboard";
import Register from "./pages/Register";
import Login from "./pages/Login";
import EventsAdmin from "./pages/events/EventsAdmin";
import EventForm from "./pages/events/EventForm";
import EventsList from "./pages/events/EventsList";
import EventDetail from "./pages/events/EventDetail";
import MyGroups from "./pages/groups/MyGroups";
import MyInscriptions from "./pages/inscriptions/MyInscriptions";
import Participants from "./pages/inscriptions/Participants";
import AdminInscriptions from "./pages/admin/AdminInscriptions";
import AdminGroups from "./pages/admin/AdminGroups";
import AdminUsers from "./pages/admin/AdminUsers";
import AdminEventTypes from "./pages/admin/AdminEventTypes";

function NotFound() {
  return (
    <div style={{ padding: "2rem", textAlign: "center" }}>
      <h2>404 — Página no encontrada</h2>
    </div>
  );
}

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isReady } = useAuth();
  if (!isReady) return null;
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return <>{children}</>;
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* Públicas */}
        <Route index element={<Home />} />
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />

        {/* Eventos públicos */}
        <Route path="events" element={<EventsList />} />
        <Route path="events/:id" element={<EventDetail />} />

        {/* Protegidas — Dashboard */}
        <Route path="dashboard" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />

        {/* Admin / Gestor — Eventos */}
        <Route path="admin/events" element={<ProtectedRoute><EventsAdmin /></ProtectedRoute>} />
        <Route path="admin/events/new" element={<ProtectedRoute><EventForm /></ProtectedRoute>} />
        <Route path="admin/events/:id/edit" element={<ProtectedRoute><EventForm /></ProtectedRoute>} />

        {/* Admin / Gestor — Inscripciones */}
        <Route path="admin/inscriptions" element={<ProtectedRoute><AdminInscriptions /></ProtectedRoute>} />

        {/* Admin — Grupos */}
        <Route path="admin/groups" element={<ProtectedRoute><AdminGroups /></ProtectedRoute>} />

        {/* Admin — Usuarios */}
        <Route path="admin/users" element={<ProtectedRoute><AdminUsers /></ProtectedRoute>} />

        {/* Admin — Tipos de evento */}
        <Route path="admin/event-types" element={<ProtectedRoute><AdminEventTypes /></ProtectedRoute>} />

        {/* Grupo — Mis grupos */}
        <Route path="my-groups" element={<ProtectedRoute><MyGroups /></ProtectedRoute>} />

        {/* Grupo — Mis inscripciones */}
        <Route path="my-inscriptions" element={<ProtectedRoute><MyInscriptions /></ProtectedRoute>} />

        {/* Grupo — Participantes */}
        <Route path="participants" element={<ProtectedRoute><Participants /></ProtectedRoute>} />

        {/* Catch-all */}
        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
}
