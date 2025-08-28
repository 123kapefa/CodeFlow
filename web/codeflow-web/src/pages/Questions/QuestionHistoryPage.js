import React, { useEffect, useState } from "react";
import { Container, Spinner, Card, Button } from "react-bootstrap";
import { useParams, useNavigate } from "react-router-dom";
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
import hljs from "highlight.js";
import "highlight.js/styles/github.css";

import { API_BASE } from "../../config";

dayjs.extend(relativeTime);


export default function QuestionHistoryPage() {
  const { id } = useParams(); // questionId
  const navigate = useNavigate();
  const [items, setItems] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const r = await fetch(`${API_BASE}/questions/${id}/history`, {
          headers: { Accept: "application/json" },
        });
        if (!r.ok) throw new Error(`HTTP ${r.status}`);
        const data = await r.json(); // QuestionHistoryDTO[]
        
        const sorted = [...data].sort(
          (a, b) => new Date(b.updatedAt) - new Date(a.updatedAt)
        );
        setItems(sorted);
      } catch (e) {
        console.error(e);
        setItems([]);
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  // Подсветка кода внутри HTML содержимого
  useEffect(() => {
    if (!items) return;
    setTimeout(() => {
      document
        .querySelectorAll("pre.ql-syntax, pre code")
        .forEach((el) => hljs.highlightElement(el));
    }, 0);
  }, [items]);

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
        <h3 className="mb-0">Question edit history</h3>
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
