import React from 'react';
import { Card, Placeholder } from 'react-bootstrap';

export default function AnswersList({ userId }) {
  return (
    <Card className="mb-4">
      <Card.Header as="h5">Answers</Card.Header>
      <Card.Body>
        <Placeholder as="p" animation="wave">
          <Placeholder xs={12} />
          <Placeholder xs={10} />
          <Placeholder xs={9} />
        </Placeholder>
        <small className="text-muted">
          TODO: вывести ответы пользователя (userId = {userId})
        </small>
      </Card.Body>
    </Card>
  );
}