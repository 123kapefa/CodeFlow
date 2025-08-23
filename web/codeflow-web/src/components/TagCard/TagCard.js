import React, { useState, useRef, useEffect } from "react";
import { Card, Overlay, Popover, Button, Modal, Form } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

import { API_BASE } from "../../config";

function TagCard({
  tag,
  isWatched: isWatchedFromProps,
  showPopoverId,
  onPopoverShow,
  onPopoverHide,
}) {
  const [isWatched, setIsWatched] = useState(undefined);
  const [displayName, setDisplayName] = useState(tag.name);
  const [displayDesc, setDisplayDesc] = useState(tag.description ?? "");
  const [showEdit, setShowEdit] = useState(false);
  const [editName, setEditName] = useState(tag.name);
  const [editDesc, setEditDesc] = useState(tag.description ?? "");
  const [saving, setSaving] = useState(false);

  const { user } = useAuth();
  const authFetch = useAuthFetch();
  const targetRef = useRef(null);
  const navigate = useNavigate();

  const tagRegex = /^[a-z0-9.+-]+$/;

  // когда приходят новые пропсы — пересинхронизируем локальное отображение
  useEffect(() => {
    setIsWatched(undefined);
    setDisplayName(tag.name);
    setDisplayDesc(tag.description ?? "");
    setEditName(tag.name);
    setEditDesc(tag.description ?? "");
  }, [isWatchedFromProps, tag.id, tag.name, tag.description]);
  const fetchIsWatched = async () => {
    if (!user) {
      setIsWatched(false);
      return;
    }
    try {
      const res = await authFetch(
        `${API_BASE}/tags/watched/user/${user.userId}/tag/${tag.id}`
      );
      if (!res.ok) throw new Error("Ошибка при проверке отслеживания");
      const data = await res.json();
      setIsWatched(data);
    } catch (e) {
      setIsWatched(false);
      console.error(e);
    }
  };

  const handleMouseEnter = () => {
    onPopoverShow(tag.id);
    if (isWatched === undefined) fetchIsWatched();
  };

  const handleWatchTag = async (e) => {
    e.stopPropagation();
    if (!user) return navigate("/login");
    try {
      const response = await authFetch(
        `${API_BASE}/tags/watched/${user.userId}/${tag.id}`,
        { method: "POST" }
      );
      if (!response.ok) throw new Error("Ошибка при добавлении");
      setIsWatched(true);
    } catch {
      alert("Не удалось добавить тэг в отслеживаемые.");
    }
  };

  const handleUnwatchTag = async (e) => {
    e.stopPropagation();
    if (!user) return navigate("/login");
    try {
      const response = await authFetch(
        `${API_BASE}/tags/watched/${tag.id}/${user.userId}`,
        { method: "DELETE" }
      );
      if (!response.ok) throw new Error("Ошибка при удалении");
      setIsWatched(false);
    } catch {
      alert("Не удалось удалить тэг из отслеживаемых.");
    }
  };

  const canEdit = user?.reputation >= 1000;

  const openEdit = (e) => {
    e?.stopPropagation?.();
    setEditName(displayName);
    setEditDesc(displayDesc);
    setShowEdit(true);
  };

  const saveEdit = async () => {
    setSaving(true);
    try {
      const res = await authFetch(`${API_BASE}/tags/${tag.id}`, {

        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: editName?.trim() || null,
          description: editDesc?.trim() || null,
        }),
      });
      if (!res.ok) throw new Error("PUT /tags неудачен");

      // локально обновим отображение
      setDisplayName(editName);
      setDisplayDesc(editDesc);
      setShowEdit(false);
    } catch (e) {
      alert("Не удалось сохранить изменения тэга.");
      console.error(e);
    } finally {
      setSaving(false);
    }
  };

  return (
    <Card className="h-100 shadow-sm position-relative">

      <Card.Body>
        {/* Заголовок по центру */}
        <div
          className="position-relative text-center"
          ref={targetRef}
          onMouseEnter={handleMouseEnter}
          onMouseLeave={onPopoverHide}
        >
          <h5
            className="m-0"
            style={{ cursor: "pointer", display: "inline-block" }}
            onClick={() =>
              navigate(`/tags/${tag.id}/questions`, {
                state: { tagName: displayName },
              })
            }
          >
            <span className="badge bg-secondary tag-badge">{displayName}</span>
          </h5>
        </div>

        <Overlay
          target={targetRef.current}
          show={showPopoverId === tag.id}
          placement="bottom"
        >
          {(props) => (
            <Popover
              id={`popover-${tag.id}`}
              {...props}
              onMouseEnter={handleMouseEnter}
              onMouseLeave={onPopoverHide}
              style={{ minWidth: "15vw", ...props.style }}
            >
              <Popover.Header as="h3">
                {/* Кнопка-карандаш (только для 1000+) */}
                {canEdit && (
                  <Button
                    variant="link"
                    className="position-absolute top-0 end-0 p-2 text-muted"
                    title="Edit tag"
                    onClick={openEdit}
                    onMouseEnter={(e) => e.stopPropagation()}
                  >
                    <i
                      className="bi bi-pencil"
                      style={{ fontSize: "0.8rem" }}
                    ></i>
                  </Button>
                )}
                {displayName}
              </Popover.Header>

              <Popover.Body>
                <p className="text-muted" style={{ minHeight: "60px" }}>
                  {displayDesc || "Описание отсутствует"}
                </p>
                <div
                  className="d-flex justify-content-between align-items-center mb-2"
                  style={{ fontSize: "0.85rem" }}
                >
                  <span className="text-muted">
                    <b>
                      {tag.countWotchers?.toLocaleString?.() ??
                        tag.countWotchers}
                    </b>{" "}
                    watchers
                  </span>
                  <span className="text-muted">
                    <b>
                      {tag.countQuestion?.toLocaleString?.() ??
                        tag.countQuestion}
                    </b>{" "}
                    questions
                  </span>
                </div>

                <div className="d-flex justify-content-center">
                  {isWatched ? (
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={handleUnwatchTag}
                    >
                      Unwatch tag
                    </Button>
                  ) : (
                    <Button
                      variant="primary"
                      size="sm"
                      onClick={handleWatchTag}
                    >
                      Watch tag
                    </Button>
                  )}
                </div>
              </Popover.Body>
            </Popover>
          )}
        </Overlay>

        {/* Нижняя строка: вопросы слева, статистика справа */}
        <div className="d-flex justify-content-between align-items-end mt-auto pt-4">
          <small className="text-muted" style={{ fontSize: "0.8rem" }}>
            {tag.countQuestion} questions
          </small>
          <div className="text-end">
            <small
              className="text-muted d-block"
              style={{ fontSize: "0.75rem" }}
            >
              {tag.dailyRequestCount} asked today
            </small>
            <small
              className="text-muted d-block"
              style={{ fontSize: "0.75rem" }}
            >
              {tag.weeklyRequestCount} this week
            </small>
          </div>
        </div>
      </Card.Body>

      {/* Модалка редактирования */}
      <Modal show={showEdit} onHide={() => setShowEdit(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Edit tag</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3" controlId="editTagName">
              <Form.Label>Name</Form.Label>
              <Form.Control
                type="text"
                value={editName}
                onChange={(e) => {
                  const raw = e.target.value.toLowerCase(); // сразу переводим в lower
                  if (raw === "" || tagRegex.test(raw)) {
                    setEditName(raw);
                  }
                  // если не проходит regex — просто не обновляем state
                }}
                placeholder="Tag name"
              />
              <Form.Text className="text-muted">
                Only lowercase letters, digits, `+`, `.` and `-` allowed
              </Form.Text>
            </Form.Group>
            <Form.Group className="mb-0" controlId="editTagDesc">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                rows={4}
                value={editDesc}
                onChange={(e) => setEditDesc(e.target.value)}
                placeholder="Short description"
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={() => setShowEdit(false)}
            disabled={saving}
          >
            Cancel
          </Button>
          <Button variant="primary" onClick={saveEdit} disabled={saving}>
            {saving ? "Saving..." : "Save"}
          </Button>
        </Modal.Footer>
      </Modal>
    </Card>
  );
}

export default TagCard;
