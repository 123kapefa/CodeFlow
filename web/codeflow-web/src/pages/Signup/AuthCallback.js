import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import Cookies from "js-cookie";
import { API_BASE } from "../../config";

export default function AuthCallback() {
  const [qs] = useSearchParams();
  const navigate = useNavigate();
  const [error, setError] = useState(null);

  useEffect(() => {
    const finish = async () => {
      try {
        const urlJwt = qs.get("jwt");
        const returnUrl = qs.get("returnUrl") || "/";

        let jwt = urlJwt || "";

        if (!jwt) {
          // через gateway 
          const resp = await fetch(`${API_BASE}/auth/refresh-token`, {
            method: "POST",
            headers: { Accept: "application/json" },
            credentials: "include",
            body: "null" 
          });
          if (!resp.ok) throw new Error("Не удалось обновить токен");
          const data = await resp.json().catch(() => ({}));
          jwt = data?.accessToken || data?.token || data?.jwt || "";
        }

        if (!jwt) throw new Error("JWT не получен");

        Cookies.set("jwt", jwt, {
          path: "/",
          sameSite: "Lax",
          // secure: true // включить в проде на https
        });

        navigate(returnUrl, { replace: true });
      } catch (e) {
        setError(e?.message || "Auth error");
      }
    };

    finish();
  }, [qs, navigate]);

  if (error) return <div className="container py-5">Ошибка авторизации: {error}</div>;
  return <div className="container py-5">Завершаем вход…</div>;
}