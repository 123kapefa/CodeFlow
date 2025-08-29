import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
  useCallback,
} from "react";

import Cookies from "js-cookie";
import { jwtDecode } from "jwt-decode";
import { RefreshToken } from "../RefreshToken/RefreshToken";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";


import { API_BASE } from "../../config";


const AuthContext = createContext(null);

const USER_LS_KEY = "cf.user";

function readJsonLS(key) {
  try {
    const raw = localStorage.getItem(key);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

function writeJsonLS(key, v) {
  try {
    v == null
      ? localStorage.removeItem(key)
      : localStorage.setItem(key, JSON.stringify(v));
  } catch {}
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => readJsonLS(USER_LS_KEY));
  const [loading, setLoading] = useState(true);
  const authFetch = useAuthFetch();


  useEffect(() => {
    writeJsonLS(USER_LS_KEY, user);
  }, [user]);


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


  // ── Тихий bootstrap ──
  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        // 1) всегда пробуем обновить access по refresh 
        await RefreshToken();

        // 2) берём текущий access из cookie 
        const at = Cookies.get("jwt");
        const id = at ? getUserIdFromAccess(at) : null;

        if (!id) {
          setUser(null);
          setLoading(false);
          return;
        }

        // 3) тянем профиль
        const res = await authFetch(`${API_BASE}/users/${id}`);
        if (res.ok) setUser(await res.json());
        else setUser(null);
      } catch {
        setUser(null);
      } finally {
        setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [authFetch]);


  // ── login (парольный) ──
  const login = async (email, password) => {
    const res = await fetch(`${API_BASE}/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      credentials: "include", 
      body: JSON.stringify({ email, password }),
    });
    if (!res.ok) throw new Error("Неверный логин или пароль");

    const data = await res.json().catch(() => ({}));


    if (data?.accessToken) {
      Cookies.set("jwt", data.accessToken, {
        path: "/",
        sameSite: "Lax",
        expires: (data.expiresInSeconds ?? 3600) / 86400,
      });
    }


    if (data?.refreshToken) {
      Cookies.set("refresh_token", data.refreshToken, {
        path: "/",
        sameSite: "Lax",
        expires: 30,
      });

    } else if (!data?.accessToken) {
      // если access не пришёл — попробуем через refresh-cookie
      await RefreshToken();
    }

    await refreshUser();
  };

  // ── logout ──
  const logout = async () => {
    try {
      await fetch(`${API_BASE}/auth/logout`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        credentials: "include",
        body: "null", 
      });

    } catch (e) {
      console.warn("Logout error:", e);
    } finally {
      Cookies.remove("jwt", { path: "/" });
     
      setUser(null);
    }
  };

  const value = useMemo(
    () => ({ user, loading, login, logout, authFetch, refreshUser }),
    [user, loading, authFetch, refreshUser]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);