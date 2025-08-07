import React, { useRef } from "react";
import { Card, Overlay, Popover, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

function TagCard({ tag, handleWatchTag, showPopoverId, setShowPopoverId }) {
  const targetRef = useRef(null);
  const navigate = useNavigate();

  return (
    <Card className="h-100 shadow-sm">
      <Card.Body>
        <div
          ref={targetRef}
          onMouseEnter={() => setShowPopoverId(tag.id)}
          onMouseLeave={() => setShowPopoverId(null)}
          style={{ display: "inline-block" }}
        >
          <h5
            style={{ cursor: "pointer" }}
            onClick={() => navigate(`/tags/${tag.id}/questions`)}
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
              onMouseEnter={() => setShowPopoverId(tag.id)}
              onMouseLeave={() => setShowPopoverId(null)}
            >
              <Popover.Header as="h3">{tag.name}</Popover.Header>
              <Popover.Body>
                {/* <p>{tag.description || "Описание отсутствует"}</p> */}
                <p>
                  <b>{tag.countWotchers} watchers</b> &nbsp; {tag.countQuestion}{" "}
                  questions
                </p>
                <Button
                  variant="primary"
                  size="sm"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleWatchTag(tag.id);
                  }}
                >
                  Watch tag
                </Button>
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
