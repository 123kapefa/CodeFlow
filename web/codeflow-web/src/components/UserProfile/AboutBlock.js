import React from 'react';
import { Card } from 'react-bootstrap';

export default function AboutBlock({ text }) {
  return (
    <Card className="mb-4">
      <Card.Header as="h5">About</Card.Header>
      <Card.Body style={{ whiteSpace: 'pre-wrap' }}>
        {text}
      </Card.Body>
    </Card>
  );
}