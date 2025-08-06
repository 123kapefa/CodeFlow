import React from 'react';
import { Card, Placeholder } from 'react-bootstrap';

export default function TagsBlock({ userId }) {
  return (
    <Card className="mb-4">
      <Card.Header as="h5">Tags</Card.Header>
      <Card.Body>
        <Placeholder as="p" animation="wave">
          <Placeholder xs={6} /> <Placeholder xs={5} /> <Placeholder xs={4} />
        </Placeholder>
        <small className="text-muted">
          TODO: популярные теги пользователя (userId = {userId})
        </small>
      </Card.Body>
    </Card>
  );
}