import React from 'react';
import { Card, Placeholder } from 'react-bootstrap';

export default function ReputationChart({ userId }) {
  return (
    <Card className="mb-4">
      <Card.Header as="h5">Reputation</Card.Header>
      <Card.Body>
        {/* Здесь потом будет график; пока – «полоска-заглушка» */}
        <Placeholder xs={12} style={{ height: '100px' }} animation="wave" />

        <small className="d-block mt-2 text-muted">
          TODO: график репутации пользователя (userId = {userId})
        </small>
      </Card.Body>
    </Card>
  );
}