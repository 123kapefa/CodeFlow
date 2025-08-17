import React, { useEffect, useMemo, useState } from "react";
import { Container, Card, Form, Button, Spinner, Alert } from "react-bootstrap";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import ReactQuill from "react-quill";
import "react-quill/dist/quill.snow.css";
import { toast } from "react-toastify";
import { InfoCircle } from "react-bootstrap-icons";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import { API_BASE } from "../../config";

const modules = {
  toolbar: [
    [{ header: [1, 2, false] }],
    ["bold", "italic", "underline", "strike", "code", "blockquote"],
    [
      { list: "ordered" },
      { list: "bullet" },
      { indent: "-1" },
      { indent: "+1" },
    ],
    ["link"],
    ["clean"],
  ],
};

const formats = [
  "header",
  "bold",
  "italic",
  "underline",
  "strike",
  "code",
  "blockquote",
  "list",
  "bullet",
  "indent",
  "link",
];

export default function EditAnswerPage() {
  const { answerId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const { user, loading } = useAuth();
  const authFetch = useAuthFetch();

  // можно притащить контент из navigate(..., { state: { content, questionId } })
  const initialContentFromState = location.state?.content || "";
  const questionId = location.state?.questionId; // пригодится для возврата

  const [content, setContent] = useState(initialContentFromState);
  const [saving, setSaving] = useState(false);
  const [prefilling, setPrefilling] = useState(!initialContentFromState);

  const stateAuthorId = location.state?.authorId;

  // если контента нет в state — пробуем забрать с бэка
  useEffect(() => {
    if (loading) return;
    if (!user) {
      navigate("/login");
      return;
    }

    // Быстрая проверка по state
    if (stateAuthorId && stateAuthorId !== user.userId) {
      toast.error("Вы не можете редактировать этот ответ.", { toastId: "not-author" });
      navigate(`/questions/${questionId}`);
      return;
    }

    // Если автора не передали — тянем ответ и проверяем
    if (!stateAuthorId) {
      (async () => {
        try {
          const r = await fetch(
            `${API_BASE}/answers/${answerId}`,
            {
              headers: { Accept: "application/json" },
            }
          );
          if (!r.ok) throw new Error();
          const ans = await r.json(); // ожидаем { userId, content, ... }
          if (ans.userId !== user.userId) {
            toast.error("Вы не можете редактировать этот ответ.", { toastId: "not-author" });
            navigate(-1);
          }
        } catch {
          toast.error("Не удалось получить ответ");
          navigate(-1);
        }
      })();
    }
  }, [loading, user, stateAuthorId, answerId, navigate]);

  // редирект, если не залогинен
  useEffect(() => {
    if (!loading && !user) navigate("/login");
  }, [loading, user, navigate]);

  const plain = useMemo(
    () =>
      content
        .replace(/<[^>]*>/g, "")
        .replace(/&nbsp;/g, " ")
        .trim(),
    [content]
  );

  const handleSave = async (e) => {
    e.preventDefault();
    if (saving) return;

    if (!plain || plain.length < 5) {
      toast.error("Answer must contain at least 5 characters");
      return;
    }

    try {
      setSaving(true);

      const body = {
        editedUserId: user.userId,
        content, // HTML из редактора
      };

      const res = await authFetch(`${API_BASE}/answers/edit/${answerId}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify(body),
      });

      if (!res.ok)
        throw new Error((await res.text()) || "Failed to save edits");

      toast.success("Edits saved.", {
        onClose: () => navigate(questionId ? `/questions/${questionId}` : -1),
        autoClose: 900,
      });
    } catch (err) {
      toast.error(err.message);
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    navigate(questionId ? `/questions/${questionId}` : -1);
  };

  if (loading || prefilling) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  return (
    <Container className="my-4" style={{ maxWidth: 940 }}>
      {/* Info-блок над формой */}
      <Alert variant="info" className="mb-3">
        <div className="d-flex">
          <InfoCircle className="me-2 flex-shrink-0" size={18} />
          <div>
            <div className="fw-semibold">
              Your edit will be placed immediately.
            </div>
            <div className="mt-2">
              We welcome edits that make the post easier to understand and more
              valuable for readers. Because community members review edits,
              please try to make the post substantially better than how you
              found it, for example, by fixing grammar or adding additional
              resources and hyperlinks.
            </div>
          </div>
        </div>
      </Alert>

      <div className="text-end mb-3">
        <small className="text-muted">
          Required fields<span className="text-danger">*</span>
        </small>
      </div>
      <Form onSubmit={handleSave}>
        {/* Блок как на скрине */}
        <Form.Group className="mb-3 text-start">
          <Form.Label className="fw-semibold">
            Answer<span className="text-danger">*</span>
          </Form.Label>
          <Card className="shadow-sm">
            <Card.Body>
              <ReactQuill
                theme="snow"
                value={content}
                onChange={setContent}
                modules={modules}
                formats={formats}
                style={{ minHeight: 180 }}
              />
            </Card.Body>
          </Card>
        </Form.Group>

        <div className="d-flex gap-2">
          <Button type="submit" variant="primary" disabled={saving}>
            {saving ? "Saving..." : "Save edits"}
          </Button>
          <Button
            variant="outline-secondary"
            onClick={handleCancel}
            disabled={saving}
          >
            Cancel
          </Button>
        </div>
      </Form>
    </Container>
  );
}
