import { createContext, useContext, useEffect, useMemo, useState, useCallback } from "react";
import Cookies from "js-cookie";
import { jwtDecode } from "jwt-decode";
import { RefreshToken } from "../RefreshToken/RefreshToken";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";


import { API_BASE } from "../../config";


const AuthContext = createContext(null);

// Ключ для локального кэша профиля (чтобы UI не моргал и не редиректил при F5)
const USER_LS_KEY = "cf.user";

function readJsonLS(key) {
  try {
    const raw = localStorage.getItem(key);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}
function writeJsonLS(key, value) {
  try {
    if (value == null) localStorage.removeItem(key);
    else localStorage.setItem(key, JSON.stringify(value));
  } catch {
    /* игнорируем квоты/инкогнито */
  }
}

export const AuthProvider = ({ children }) => {
  // 1) мгновенная ре-гидрация профиля из localStorage (устраняет "мигание" и ложный редирект)
  const [user, setUser] = useState(() => readJsonLS(USER_LS_KEY));
  const [loading, setLoading] = useState(true);

  // единый обёрнутый fetch (как у тебя)
  const authFetch = useAuthFetch();

  // синхронизация user ↔ localStorage
  useEffect(() => {
    writeJsonLS(USER_LS_KEY, user);
  }, [user]);

  // безопасное извлечение id из JWT
  const getUserIdFromAccess = (token) => {
    try {
      const { sub } = jwtDecode(token);
      return sub ?? null;
    } catch {
      return null;
    }
  };

  const refreshUser = useCallback(async () => {
    const at = Cookies.get("jwt");
    const id = at ? getUserIdFromAccess(at) : null;
    if (!id) {
      setUser(null);
      return;
    }

    const res = await authFetch(`${API_BASE}/users/${id}`);

    if (res.ok) setUser(await res.json());
    else setUser(null);
  }, [authFetch]);

  /* ── 1. «Тихий» bootstrap при старте ── */
  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        // Если нет refresh — сразу гость (но не сносим user из LS до конца проверки)
        if (!Cookies.get("refresh_token")) {
          if (!cancelled) {
            setUser(null);
            setLoading(false);
          }
          return;
        }

        // Пробуем обновить access (ставит новую куку "jwt", если ок)
        await RefreshToken();

        const at = Cookies.get("jwt");
        const id = at ? getUserIdFromAccess(at) : null;
        if (!id) {
          if (!cancelled) {
            setUser(null);
            setLoading(false);
          }
          return;
        }

        // Подтягиваем профиль
        const res = await authFetch(`${API_BASE}/users/${id}`);


        if (!cancelled) {
          if (res.ok) setUser(await res.json());
          else setUser(null);
          setLoading(false);
        }
      } catch {
        if (!cancelled) {
          setUser(null);
          setLoading(false);
        }
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [authFetch]);

  /* ── 2. login ── */
  const login = async (email, password) => {
    const res = await fetch(`${API_BASE}/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({ email, password }),
      credentials: "include",
    });

    if (!res.ok) throw new Error("Неверный логин или пароль");

    // сервер может вернуть access/refresh — если да, кладём, как и раньше
    const data = await res.json();

    if (data?.accessToken) {
      Cookies.set("jwt", data.accessToken, {
        path: "/",
        sameSite: "Lax", // если фронт и бэк на одном origin; иначе нужен None+Secure
        expires: (data.expiresInSeconds ?? 3600) / 86400,
      });
    }
    if (data?.refreshToken) {
      Cookies.set("refresh_token", data.refreshToken, {
        path: "/",
        sameSite: "Lax",
        expires: 30,
      });
    }

    // сразу подтянем профиль тем же способом
    await refreshUser();
  };

  /* ── 3. logout ── */
  const logout = async () => {
    try {
      const rt = Cookies.get("refresh_token");
      if (rt) {
        await fetch(`${API_BASE}/auth/logout`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          credentials: "include",
          body: JSON.stringify(rt), // оставил как у тебя; если бэк ждёт {refreshToken}, поправь тут
        });
      }
    } catch (e) {
      console.warn("Logout error:", e);
    } finally {
      Cookies.remove("jwt", { path: "/" });
      Cookies.remove("refresh_token", { path: "/" });
      setUser(null);
    }
  };

  /* ── наружу ── */
  const value = useMemo(
    () => ({ user, loading, login, logout, authFetch, refreshUser }),
    [user, loading, authFetch, refreshUser]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);