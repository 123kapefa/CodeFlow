import React from "react";
import { Card, Spinner, Alert } from "react-bootstrap";
import { Link } from "react-router-dom";
import dayjs from "dayjs";
import { useUserSummary } from "../../features/useUserSummary/useUserSummary";

function ScorePill({ value }) {
  return (
    <span className="badge bg-success me-2" style={{ borderRadius: "0.25rem" }}>
      {value}
    </span>
  );
}

export default function AnswersList({ userId, limit = 5 }) {
  const { data, loading, err } = useUserSummary(userId);
  const items = data?.questionsAnswerList?.value ?? [];
  const total = data?.questionsAnswerList?.pagedInfo?.totalRecords ?? 0;

  return (
    <Card className="mb-4">
      <Card.Header
        as="h5"
        className="d-flex justify-content-between align-items-center"
      >
        <span>Answers</span>
        <small className="text-muted">View all {total} answers</small>
      </Card.Header>

      {loading ? (
        <div className="text-center my-4">
          <Spinner animation="border" />
        </div>
      ) : err ? (
        <Alert variant="danger" className="m-3">
          {err}
        </Alert>
      ) : (
        <ul className="list-group list-group-flush">
          {items.slice(0, limit).map((q) => (
            <li
              key={q.id}
              className="list-group-item d-flex justify-content-between align-items-center"
            >
              <div className="text-truncate" style={{ maxWidth: "75%" }}>
                <ScorePill value={(q.upvotes ?? 0) - (q.downvotes ?? 0)} />
                <Link
                  to={`/questions/${q.id}`}
                  className="text-decoration-none"
                >
                  {q.title}
                </Link>
              </div>
              <small className="text-muted">
                {dayjs(q.createdAt).format("MMM D, YYYY")}
              </small>
            </li>
          ))}
          {items.length === 0 && (
            <li className="list-group-item text-muted">Нет ответов</li>
          )}
        </ul>
      )}
    </Card>
  );
}
