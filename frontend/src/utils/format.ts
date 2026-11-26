/**
 * Returns the initials of a full name.
 * Example: "Alexander Castillo" → "AC"
 */
export function getInitials(firstName?: string, lastName?: string): string {
  const f = (firstName ?? "").trim();
  const l = (lastName ?? "").trim();
  if (!f && !l) return "?";
  return `${f.charAt(0)}${l.charAt(0)}`.toUpperCase();
}

/**
 * Formats an ISO date string to dd/mm/yyyy
 */
export function formatDate(iso?: string): string {
  if (!iso) return "";
  try {
    return new Date(iso).toLocaleDateString("es-CO");
  } catch {
    return iso;
  }
}

/**
 * Returns a human-readable label for a user role.
 */
export function getRoleLabel(role?: string): string {
  switch (role) {
    case "Admin": return "Administrador";
    case "Gestor": return "Gestor Cultural";
    case "Grupo": return "Grupo de Danza";
    default: return role ?? "";
  }
}

/**
 * Formats an ISO date string to dd/mm/yyyy HH:mm
 */
export function formatDateTime(iso?: string): string {
  if (!iso) return "";
  try {
    return new Date(iso).toLocaleString("es-CO", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return iso;
  }
}

/**
 * Converts an ISO string to a value usable in <input type="datetime-local">
 * e.g. "2024-06-15T14:30:00.000Z" → "2024-06-15T14:30"
 */
export function toLocalDateTimeInput(iso?: string): string {
  if (!iso) return "";
  try {
    const d = new Date(iso);
    const pad = (n: number) => String(n).padStart(2, "0");
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  } catch {
    return "";
  }
}

/**
 * Labels for document types used in registration.
 */
export const DOCUMENT_TYPE_LABELS: Record<string, string> = {
  CC: "Cédula de Ciudadanía",
  CE: "Cédula de Extranjería",
  TI: "Tarjeta de Identidad",
  PA: "Pasaporte",
  NIT: "NIT",
};
