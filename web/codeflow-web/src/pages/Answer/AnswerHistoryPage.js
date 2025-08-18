import React, { useEffect, useState } from "react";
import { Container, Spinner, Card, Button } from "react-bootstrap";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
import hljs from "highlight.js";
import "highlight.js/styles/github.css";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import { API_BASE } from "../../config";

dayjs.extend(relativeTime);


export default function AnswerHistoryPage() {
  const { answerId } = useParams();
  const navigate = useNavigate();
  const location = useLocation();

  const { user, loading: authLoading } = useAuth();
  const authFetch = useAuthFetch();

  const [items, setItems] = useState(null);
  const [loading, setLoading] = useState(true);

  // Guard: только авторизованные
  useEffect(() => {
    if (!authLoading && !user) {
      navigate("/login", {
        replace: true,
        state: { returnUrl: location.pathname }, // вернемся после логина
      });
    }
  }, [authLoading, user, navigate, location.pathname]);

  useEffect(() => {
    if (authLoading || !user) return;

    const ac = new AbortController();

    (async () => {
      try {
        setLoading(true);

        const r = await authFetch(`${API_BASE}/answers/${answerId}/history`, {
          headers: { Accept: "application/json" },
          signal: ac.signal,
        });

        if (r.status === 401 || r.status === 403) {
          navigate("/login", {
            replace: true,
            state: { returnUrl: location.pathname },
          });
          return;
        }

        if (!r.ok) throw new Error(`HTTP ${r.status}`);
        const data = await r.json(); // [{ userId, content, updatedAt }]

        const sorted = [...data].sort(
          (a, b) => new Date(b.updatedAt) - new Date(a.updatedAt)
        );
        setItems(sorted);
      } catch (e) {
        if (e.name !== "AbortError") {
          console.error(e);
          setItems([]);
        }
      } finally {
        setLoading(false);
      }
    })();

    return () => ac.abort();
  }, [answerId, authLoading, user, authFetch, navigate, location.pathname]);

  // Подсветка кода
  useEffect(() => {
    if (!items) return;
    setTimeout(() => {
      document
        .querySelectorAll("pre.ql-syntax, pre code")
        .forEach((el) => hljs.highlightElement(el));
    }, 0);
  }, [items]);

  if (authLoading || (!user && loading)) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  return (
    <Container className="my-4" style={{ maxWidth: 940 }}>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h3 className="mb-0">Answer edit history</h3>
        <Button variant="outline-secondary" onClick={() => navigate(-1)}>
          Back
        </Button>
      </div>

      {!items || items.length === 0 ? (
        <Card className="shadow-sm">
          <Card.Body>No edits yet.</Card.Body>
        </Card>
      ) : (
        items.map((h, idx) => (
          <Card className="shadow-sm mb-3" key={idx}>
            <Card.Header className="d-flex justify-content-between">
              <span>
                <strong>Edited</strong> {dayjs(h.updatedAt).fromNow()}
              </span>
              <small className="text-muted">by {h.userId}</small>
            </Card.Header>
            <Card.Body>
              <div
                className="post-content"
                dangerouslySetInnerHTML={{ __html: h.content }}
              />
            </Card.Body>
          </Card>
        ))
      )}
    </Container>
  );
}