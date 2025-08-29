//

import { useState, useMemo } from "react";
import { Container, Spinner, Alert, Button, Card } from "react-bootstrap";
import { useSearchParams, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

import { useAuthFetch } from "../../../features/useAuthFetch/useAuthFetch";
import { useAuth } from "../../../features/Auth/AuthProvider ";

import { API_BASE } from "../../../config";

export default function PasswordChangeConfirm() {
  const fetchAuth = useAuthFetch();
  const { logout } = useAuth();

  const [qs] = useSearchParams();
  const email = qs.get("email");
  const token = qs.get("token");

  const [busy, setBusy] = useState(false);
  const [status, setStatus] = useState("idle"); // idle | ok | error
  const [msg, setMsg] = useState("");
  const navigate = useNavigate();

  const emailMasked = useMemo(() => {
    if (!email) return "";
    const [name, domain] = email.split("@");
    if (!domain) return email;
    const safe =
      name.length <= 2
        ? name
        : name[0] + "*".repeat(name.length - 2) + name.slice(-1);
    return `${safe}@${domain}`;
  }, [email]);

  const handleConfirm = async () => {
    setBusy(true);
    setStatus("idle");
    setMsg("");

    try {
      if (!email || !token) {
        throw new Error("Missing email or token.");
      }
      
      const resp = await fetchAuth(`${API_BASE}/auth/password-change-confirm`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify({ email, token }),
        credentials: "include", 
      });

      if (!resp.ok) {
        const text = await resp.text();
        throw new Error(text || `HTTP ${resp.status}`);
      }

      setStatus("ok");

      (async () => {
        await logout();
        toast.success("Пароль изменен. Залогинься, бро!")
        navigate("/login", { replace: true });
      })();
    } catch (e) {
      setStatus("error");
      setMsg(e?.message || "Confirmation failed.");
    } finally {
      setBusy(false);
    }
  };

  // базовые проверки query
  if (!email || !token) {
    return (
      <Container className="py-5 text-center">
        <Alert variant="danger">
          Ссылка некорректна: отсутствует email или token.
        </Alert>
        <Button variant="secondary" onClick={() => navigate("/")}>
          На главную
        </Button>
      </Container>
    );
  }

  return (
    <Container className="py-5" style={{ maxWidth: 640 }}>
      <Card className="p-4">
        <h4 className="mb-3 text-center">Подтверждение смены пароля</h4>
        <p className="text-muted text-center">
          Аккаунт: <strong>{emailMasked}</strong>
        </p>

        {status === "ok" && (
          <>
            <Alert variant="success" className="text-center">
              Пароль успешно изменён.
            </Alert>
            <div className="text-center">
              <Button onClick={() => navigate("/login")}>
                Перейти к входу
              </Button>
            </div>
          </>
        )}

        {status === "error" && (
          <Alert variant="danger" className="text-center">
            Не удалось подтвердить: {msg}
          </Alert>
        )}

        {status !== "ok" && (
          <div className="text-center mt-3">
            <Button variant="primary" onClick={handleConfirm} disabled={busy}>
              {busy ? (
                <Spinner animation="border" size="sm" />
              ) : (
                "Подтвердить смену пароля"
              )}
            </Button>
            <div className="mt-3">
              <Button variant="link" onClick={() => navigate("/")}>
                Отмена
              </Button>
            </div>
          </div>
        )}
      </Card>
    </Container>
  );
}
