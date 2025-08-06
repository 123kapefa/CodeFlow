import { createContext, useContext, useEffect, useMemo, useState } from "react";
import Cookies from "js-cookie";
import { jwtDecode } from "jwt-decode";
import { RefreshToken } from "../RefreshToken/RefreshToken";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null); // профиль пользователя
  const [loading, setLoading] = useState(true); // загрузка состояния авторизации

  // ---- 1) Авто-восстановление сессии при старте ----
  useEffect(() => {
    let cancelled = false;

    const bootstrap = async () => {
      try {
        const rt = Cookies.get("refresh_token");
        if (!rt) {
          // Нет refresh — считать гостем, ничего не запрашиваем
          if (!cancelled) setUser(null);
          return;
        }

        // Пытаемся «тихо» обновить токен
        const ok = await RefreshToken();
        if (!ok) {
          Cookies.remove("jwt", { path: "/" });
          Cookies.remove("refresh_token", { path: "/" });
          if (!cancelled) setUser(null);
          return;
        }

        // Получили access — грузим профиль
        const access = Cookies.get("jwt");
        if (!access) {
          if (!cancelled) setUser(null);
          return;
        }

        const headers = {
          "Content-Type": "application/json",
          Accept: "application/json",
          Authorization: `Bearer ${access}`,
        };

        const { sub: id } = jwtDecode(access);
        let res = await fetch(`http://localhost:5000/api/users/${id}`, {
          method: "GET",
          headers,
          // credentials: "include", // раскомментируй, если сервер использует куки
        });

        // На случай гонки/просрочки — одна попытка обновить ещё раз
        if (res.status === 401) {
          const ok2 = await RefreshToken();
          if (ok2) {
            const access2 = Cookies.get("jwt");
            res = await fetch(`http://localhost:5000/api/users/${id}`, {
              method: "GET",
              headers: {
                ...headers,
                Authorization: access2 ? `Bearer ${access2}` : undefined,
              },
            });
          }
        }

        if (!cancelled) {
          if (res.ok) {
            const profile = await res.json();
            setUser(profile);
          } else {
            setUser(null);
          }
        }
      } catch {
        if (!cancelled) setUser(null);
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    bootstrap();
    return () => {
      cancelled = true;
    };
  }, []);

  // ---- 2) Логин ----
  const login = async (email, password) => {
    const res = await fetch("http://localhost:5000/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({ email, password }),
      // credentials: "include",
    });

    if (!res.ok) throw new Error("Wrong credentials");

    // Ожидаем, что сервер вернёт { accessToken, refreshToken?, expiresInSeconds? }
    const data = await res.json();

    // Кладём access в куки (логика как в RefreshToken)
    const accessDays = data.expiresInSeconds
      ? data.expiresInSeconds / 86400
      : 1 / 24; // по умолчанию 1 час

    Cookies.set("jwt", data.accessToken, {
      path: "/",
      sameSite: "Lax",
      expires: accessDays,
      // secure: true, // включи в проде (HTTPS)
    });

    // Если пришёл refreshToken — тоже сохраним
    if (data.refreshToken) {
      Cookies.set("refresh_token", data.refreshToken, {
        path: "/",
        sameSite: "Lax",
        expires: 30,
        // secure: true, // включи в проде
      });
    }

    // Грузим профиль
    const headers = {
      "Content-Type": "application/json",
      Accept: "application/json",
      Authorization: `Bearer ${data.accessToken}`,
    };

    const { sub: id } = jwtDecode(data.accessToken);
    const profileRes = await fetch(`http://localhost:5000/api/users/${id}`, {
      method: "GET",
      headers,
      // credentials: "include",
    });

    if (!profileRes.ok) throw new Error("Failed to load profile");

    const profile = await profileRes.json();
    setUser(profile);
  };

  // ---- 3) Логаут ----
  const logout = async () => {
    let ok = false;

    try {
      const rt = Cookies.get("refresh_token");
      const at = Cookies.get("jwt");

      // если нет refresh_token — просто локально выходим
      if (!rt) {
        ok = true;
        return ok;
      }

      let headers = {
        "Content-Type": "application/json",
        Accept: "application/json",
        ...(at ? { Authorization: `Bearer ${at}` } : {}),
      };

      const url = "http://localhost:5000/api/auth/logout";

      let response = await fetch(url, {
        method: "POST",
        headers,
        credentials: "include", // если сервер опирается на куки
        body: JSON.stringify(rt ), // <--- ключевое: ASP.NET ждёт поле token
      });

      if (response.status === 401) {
        const refreshed = await RefreshToken(); // <--- обязательно await
        if (refreshed) {
          const newAt = Cookies.get("jwt"); // перечитать новый access
          headers = {
            ...headers,
            ...(newAt ? { Authorization: `Bearer ${newAt}` } : {}),
          };
          response = await fetch(url, {
            method: "POST",
            headers,
            credentials: "include",
            body: JSON.stringify(rt),
          });
        }
      }

      ok = response.ok;
      if (!ok) {
        // полезно увидеть, что ответил сервер
        const text = await response.text().catch(() => "");
        console.warn("Logout failed:", response.status, text);
      }
    } catch (e) {
      console.warn("Logout network error:", e);
    } finally {
      // локально выходим в любом случае
      Cookies.remove("jwt", { path: "/" });
      Cookies.remove("refresh_token", { path: "/" });
      setUser(null);
    }

    return ok;
  };

  const value = useMemo(
    () => ({ user, login, logout, loading }),
    [user, loading]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);
