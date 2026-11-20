import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { authApi } from "../api/auth";
import { getToken, setToken } from "../api/client";
import { usersApi } from "../api/users";
import type { LoginDto, RegisterDto, Role } from "../types";

const USER_KEY = "danzabogota_user";

export interface AuthUser {
  firstName: string;
  lastName: string;
  email: string;
  roles: Role[];
}

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isReady: boolean;
  login: (payload: LoginDto) => Promise<void>;
  register: (payload: RegisterDto) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function loadUser(): AuthUser | null {
  try {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    return JSON.parse(raw) as AuthUser;
  } catch {
    return null;
  }
}

function toRoles(roles: string[] | null | undefined): Role[] {
  if (!roles) return [];
  return roles.filter((r): r is Role => r === "Admin" || r === "Gestor" || r === "Grupo");
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => loadUser());
  // isReady = false mientras verificamos el token contra el backend.
  // El nav no se renderiza hasta que isReady sea true, evitando el flash
  // de links de Admin cuando el token ya expiró.
  const [isReady, setIsReady] = useState(() => !getToken());

  useEffect(() => {
    if (!getToken()) {
      setIsReady(true);
      return;
    }
    let mounted = true;
    usersApi
      .me()
      .then((me) => {
        if (!mounted) return;
        const fresh: AuthUser = {
          firstName: me.firstName ?? "",
          lastName: me.lastName ?? "",
          email: me.email ?? "",
          roles: toRoles(me.roles),
        };
        localStorage.setItem(USER_KEY, JSON.stringify(fresh));
        setUser(fresh);
      })
      .catch(() => {
        if (!mounted) return;
        setToken(null);
        localStorage.removeItem(USER_KEY);
        setUser(null);
      })
      .finally(() => {
        if (mounted) setIsReady(true);
      });
    return () => {
      mounted = false;
    };
  }, []);

  const value = useMemo<AuthContextValue>(() => {
    const persist = (user: AuthUser | null) => {
      if (user) {
        localStorage.setItem(USER_KEY, JSON.stringify(user));
      } else {
        localStorage.removeItem(USER_KEY);
      }
      setUser(user);
    };

    return {
      user,
      isAuthenticated: Boolean(user && getToken()),
      isReady,
      async login(payload) {
        const res = await authApi.login(payload);
        if (!res.success || !res.token) {
          throw new Error(res.errorMessage ?? "Correo o contraseña incorrectos.");
        }
        setToken(res.token);
        persist({
          firstName: res.firstName ?? "",
          lastName: res.lastName ?? "",
          email: res.email ?? "",
          roles: toRoles(res.roles),
        });
      },
      async register(payload) {
        const res = await authApi.register(payload);
        if (!res.success || !res.token) {
          throw new Error(res.errorMessage ?? "No fue posible completar el registro.");
        }
        setToken(res.token);
        persist({
          firstName: res.firstName ?? "",
          lastName: res.lastName ?? "",
          email: res.email ?? "",
          roles: toRoles(res.roles),
        });
      },
      logout() {
        setToken(null);
        persist(null);
      },
    };
  }, [user, isReady]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return ctx;
}
