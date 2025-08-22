import { useState, useMemo } from "react";
import { Container, Spinner, Alert, Button, Card } from "react-bootstrap";
import { useSearchParams, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

import { useAuthFetch } from "../../../features/useAuthFetch/useAuthFetch";
import { useAuth } from "../../../features/Auth/AuthProvider ";

import { API_BASE } from "../../../config";


export default function EmailChangeConfirm() {
  const fetchAuth = useAuthFetch();
  const { logout } = useAuth();
   const { user, loading } = useAuth();

  const [qs] = useSearchParams();

  const newEmail = qs.get("email");
  const token = qs.get("token");

  const [busy, setBusy] = useState(false);
  const [status, setStatus] = useState("idle"); // idle | ok | error
  const [msg, setMsg] = useState("");
  const navigate = useNavigate();

  const emailMasked = useMemo(() => {
    if (!newEmail) return "";
    const [name, domain] = newEmail.split("@");
    if (!domain) return newEmail;
    const safe =
      name.length <= 2
        ? name
        : name[0] + "*".repeat(name.length - 2) + name.slice(-1);
    return `${safe}@${domain}`;
  }, [newEmail]);

  const handleConfirm = async () => {
    setBusy(true);
    setStatus("idle");
    setMsg("");

    try {
      if (!newEmail || !token) {
        throw new Error("Missing email or token.");
      }

      // отправляем именно JSON в тело (не в сегмент пути)
      const resp = await fetchAuth(`${API_BASE}/auth/email-change-confirm/${user.userId}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify({ newEmail, token }),
        credentials: "include", // не обязательно, но безвредно
      });

      if (!resp.ok) {
        const text = await resp.text();
        throw new Error(text || `HTTP ${resp.status}`);
      }

      setStatus("ok");

      (async () => {
        await logout();
        toast.success("Email изменен. Залогинься, бро!")
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
  if (!newEmail || !token) {
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
        <h4 className="mb-3 text-center">Подтверждение смены email</h4>
        <p className="text-muted text-center">
          Аккаунт: <strong>{emailMasked}</strong>
        </p>

        {status === "ok" && (
          <>
            <Alert variant="success" className="text-center">
              Email успешно изменён.
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
                "Подтвердить смену email"
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
