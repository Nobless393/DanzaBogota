import axios from "axios";

const TOKEN_KEY = "danzabogota_token";

export function getToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_KEY);
  } catch {
    return null;
  }
}

export function setToken(token: string | null): void {
  try {
    if (token) {
      localStorage.setItem(TOKEN_KEY, token);
    } else {
      localStorage.removeItem(TOKEN_KEY);
    }
  } catch {
    // ignore
  }
}

const client = axios.create({
  baseURL: "/api",
  headers: {
    "Content-Type": "application/json",
  },
});

client.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

client.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      setToken(null);
    }
    return Promise.reject(err);
  }
);

export default client;

/**
 * Extrae un mensaje legible de cualquier error (Axios, Error, string, etc.)
 */
export function extractErrorMessage(err: unknown): string {
  if (!err) return "Error desconocido.";
  // Axios error with response body
  if (typeof err === "object" && err !== null) {
    const e = err as Record<string, unknown>;
    // ApiResponse shape
    const data = e["response"] && typeof e["response"] === "object"
      ? (e["response"] as Record<string, unknown>)["data"]
      : null;
    if (data && typeof data === "object") {
      const d = data as Record<string, unknown>;
      if (typeof d["errorMessage"] === "string" && d["errorMessage"]) return d["errorMessage"];
      if (Array.isArray(d["errors"]) && d["errors"].length > 0) return String(d["errors"][0]);
      if (typeof d["title"] === "string" && d["title"]) return d["title"];
    }
    if (typeof e["message"] === "string" && e["message"]) return e["message"];
  }
  if (typeof err === "string") return err;
  return "Ocurrió un error inesperado.";
}
