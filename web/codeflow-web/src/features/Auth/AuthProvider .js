import { createContext, useContext, useEffect, useMemo, useState } from "react";
import Cookies from "js-cookie";
import { jwtDecode } from "jwt-decode";
import { RefreshToken } from "../RefreshToken/RefreshToken";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";
import { useCallback } from "react";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null); // профиль
  const [loading, setLoading] = useState(true);

  /* единый обёрнутый fetch */
  const authFetch = useAuthFetch();

  const refreshUser = useCallback(async () => {
    const at = Cookies.get("jwt");
    if (!at) {
      setUser(null);
      return;
    }
    const { sub: id } = jwtDecode(at);
    const res = await authFetch(`http://localhost:5000/api/users/${id}`);
    if (res.ok) setUser(await res.json());
  }, [authFetch]);

  /* ── 1. «Тихий» bootstrap при старте ── */
  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        /* если нет refresh_token – сразу гость */
        if (!Cookies.get("refresh_token")) {
          if (!cancelled) setUser(null);
          return;
        }

        /* пробуем обновить access*/
        await RefreshToken();

        const at = Cookies.get("jwt");
        if (!at) {
          if (!cancelled) setUser(null);
          return;
        }

        const { sub: id } = jwtDecode(at);
        const res = await authFetch(`http://localhost:5000/api/users/${id}`);

        if (!cancelled) {
          if (res.ok) setUser(await res.json());
          else setUser(null);
        }
      } catch {
        if (!cancelled) setUser(null);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [authFetch]);

  /* ── 2. login ── */
  const login = async (email, password) => {
    const res = await fetch("http://localhost:5000/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({ email, password }),
      credentials: "include",
    });

    if (!res.ok) throw new Error("Неверный логин или пароль");
    const data = await res.json();

    /* кладём токены */
    Cookies.set("jwt", data.accessToken, {
      path: "/",
      sameSite: "Lax",
      expires: (data.expiresInSeconds ?? 3600) / 86400,
    });
    if (data.refreshToken) {
      Cookies.set("refresh_token", data.refreshToken, {
        path: "/",
        sameSite: "Lax",
        expires: 30,
      });
    }

    /* грузим профиль тем же authFetch */
    const { sub: id } = jwtDecode(data.accessToken);
    const profRes = await authFetch(`http://localhost:5000/api/users/${id}`);
    if (!profRes.ok) throw new Error("Profile load failed");
    setUser(await profRes.json());
  };

  /* ── 3. logout ── */
  const logout = async () => {
    try {
      const rt = Cookies.get("refresh_token");
      if (rt) {
        await fetch("http://localhost:5000/api/auth/logout", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          credentials: "include",
          body: JSON.stringify(rt),
        });
      }
    } catch (e) {
      console.warn("Logout error:", e);
    } finally {
      /* локально выходим в любом случае */
      Cookies.remove("jwt", { path: "/" });
      Cookies.remove("refresh_token", { path: "/" });
      setUser(null);
    }
  };

  /* ── что отдаём наружу ── */
  const value = useMemo(
    () => ({ user, loading, login, logout, authFetch, refreshUser }),
    [user, loading, authFetch, refreshUser]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);
