import React from "react";
import { Card, Spinner, Alert } from "react-bootstrap";
import { useUserSummary } from "../../features/useUserSummary/useUserSummary";

export default function ReputationChart({ userId }) {
  const { data, loading, err } = useUserSummary(userId);
  const reputation = data?.user?.reputation ?? 0;

  return (
    <Card className="mb-4">
      <Card.Header as="h5" className="d-flex justify-content-between align-items-center">
        <span>Reputation</span>
        <small className="text-muted">total: {reputation}</small>
      </Card.Header>

      {loading ? (
        <div className="text-center my-4"><Spinner animation="border" /></div>
      ) : err ? (
        <Alert variant="danger" className="m-3">{err}</Alert>
      ) : (
        <Card.Body>
          <div className="d-flex align-items-end" style={{ height: 100 }}>
            {Array.from({ length: 24 }).map((_, i) => {
              const h = 20 + ((i * 37) % 70);
              return (
                <div
                  key={i}
                  style={{
                    width: 10,
                    height: h,
                    marginRight: 6,
                    background: "#198754",
                    borderRadius: 2
                  }}
                  title={`Day ${i + 1}`}
                />
              );
            })}
          </div>
          <small className="d-block mt-2 text-muted">
            История репутации пока не приходит от API. Показана заглушка-гистограмма.
          </small>
        </Card.Body>
      )}
    </Card>
  );
}