import React, { useState, useRef, useEffect } from "react";
import { Card, Overlay, Popover, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../features/Auth/AuthProvider ";
import { useAuthFetch } from "../../features/useAuthFetch/useAuthFetch";

function TagCard({
  tag,
  isWatched: isWatchedFromProps,
  showPopoverId,
  onPopoverShow,
  onPopoverHide,
}) {
  const [isWatched, setIsWatched] = useState(undefined);
  const { user } = useAuth();
  const authFetch = useAuthFetch();
  const targetRef = useRef(null);
  const navigate = useNavigate();

  // Синхронизируем локальное состояние с пропсом, если нужно
  useEffect(() => {
    setIsWatched(undefined);
  }, [isWatchedFromProps]);

  // Функция проверки
  const fetchIsWatched = async () => {
    if (!user) {
      setIsWatched(false);
      return;
    }
    try {
      const res = await authFetch(
        `http://localhost:5000/api/tags/watched/user/${user.userId}/tag/${tag.id}`
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
    // если статус ещё не определён, то проверяем
    if (isWatched === undefined) {
      fetchIsWatched();
    }
  };

  const handleWatchTag = async (e) => {
    e.stopPropagation();
    if (!user) {
      navigate("/login");
      return;
    }
    try {
      const response = await authFetch(
        `http://localhost:5000/api/tags/watched/${user.userId}/${tag.id}`,
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
    if (!user) {
      navigate("/login");
      return;
    }
    try {
      const response = await authFetch(
        `http://localhost:5000/api/tags/watched/${tag.id}/${user.userId}`,
        { method: "DELETE" }
      );
      if (!response.ok) throw new Error("Ошибка при удалении");
      setIsWatched(false);
    } catch {
      alert("Не удалось удалить тэг из отслеживаемых.");
    }
  };

  return (
    <Card className="h-100 shadow-sm">
      <Card.Body>
        <div
          ref={targetRef}
          onMouseEnter={handleMouseEnter}
          onMouseLeave={onPopoverHide}
          style={{ display: "inline-block" }}
        >
          <h5
            style={{ cursor: "pointer" }}
            onClick={() =>
              navigate(`/tags/${tag.id}/questions`, {
                state: { tagName: tag.name },
              })
            }
          >
            <span className="badge bg-secondary">{tag.name}</span>
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
            >
              <Popover.Header as="h3">{tag.name}</Popover.Header>
              <Popover.Body>
                <p>
                  <b>{tag.countWotchers} watchers</b> &nbsp; {tag.countQuestion}{" "}
                  questions
                </p>

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

        <p className="text-muted" style={{ minHeight: "60px" }}>
          {tag.description || "Описание отсутствует"}
        </p>
        <small className="text-muted d-block">
          {tag.countQuestion} questions
        </small>
        <small className="text-muted d-block">
          {tag.weeklyRequestCount} asked this week
        </small>
        <small className="text-muted d-block">
          {tag.dailyRequestCount} asked today
        </small>
      </Card.Body>
    </Card>
  );
}

export default TagCard;
